using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InsureYouAI.Models
{
    public class ChatHub : Hub
    {
        private const string apikey = "csk-dc9x3hn3dwfwcyt9c43v6w9j8merj4jy9f9p6n3kmpt6jryr";
        private const string modelCrb = "llama3.1-8b";

        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly Dictionary<string, List<Dictionary<string, string>>> _history = new();

        public ChatHub(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override Task OnConnectedAsync()
        {
            _history[Context.ConnectionId] =
            [
                new()
                {
                    ["role"] = "system",
                    ["content"] = "You are a helpful assistant for insurance customers. Keep answers concise."
                }
            ];

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _history.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string userMessage)
        {
            try
            {
                await Clients.Caller.SendAsync("ReceiveUserEcho", userMessage);

                var history = _history[Context.ConnectionId];
                history.Add(new() { ["role"] = "user", ["content"] = userMessage });

                await StreamCerebras(history, Context.ConnectionAborted);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveAIError", "Mesaj işlenirken hata oluştu: " + ex.Message);
            }
        }

        private async Task StreamCerebras(List<Dictionary<string, string>> history, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("cerebras");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);

            var payload = new
            {
                model = modelCrb,
                messages = history,
                stream = true,
                temperature = 0.2
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions");
            req.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                using var resp = await client.SendAsync(
                    req,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken
                );

                var rawError = await resp.Content.ReadAsStringAsync(cancellationToken);

                if (!resp.IsSuccessStatusCode)
                {
                    await Clients.Caller.SendAsync(
                        "ReceiveAIError",
                        $"Cerebras API hatası: {(int)resp.StatusCode} - {rawError}",
                        cancellationToken
                    );
                    return;
                }

                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawError));
                using var reader = new StreamReader(stream);

                var sb = new StringBuilder();

                while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("data:")) continue;

                    var data = line["data:".Length..].Trim();
                    if (data == "[DONE]") break;

                    try
                    {
                        var chunk = JsonSerializer.Deserialize<ChatStreamChunk>(data);
                        var delta = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;

                        if (!string.IsNullOrEmpty(delta))
                        {
                            sb.Append(delta);
                            await Clients.Caller.SendAsync("ReceiveAIDelta", delta, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        await Clients.Caller.SendAsync(
                            "ReceiveAIError",
                            "Chunk parse hatası: " + ex.Message + " | Data: " + data,
                            cancellationToken
                        );
                    }
                }

                var full = sb.ToString();

                if (string.IsNullOrWhiteSpace(full))
                {
                    await Clients.Caller.SendAsync(
                        "ReceiveAIError",
                        "Model boş yanıt döndürdü.",
                        cancellationToken
                    );
                    return;
                }

                history.Add(new() { ["role"] = "assistant", ["content"] = full });
                await Clients.Caller.SendAsync("CompleteMessage", full, cancellationToken);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveAIError", "Stream hatası: " + ex.Message, cancellationToken);
            }
        }

        private sealed class ChatStreamChunk
        {
            [JsonPropertyName("choices")]
            public List<Choice>? Choices { get; set; }
        }

        private sealed class Choice
        {
            [JsonPropertyName("delta")]
            public Delta? Delta { get; set; }

            [JsonPropertyName("finish_reason")]
            public string? FinishReason { get; set; }
        }

        private sealed class Delta
        {
            [JsonPropertyName("content")]
            public string? Content { get; set; }

            [JsonPropertyName("role")]
            public string? Role { get; set; }
        }
    }
}