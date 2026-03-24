using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;

namespace InsureYouAI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly InsureContext _context;

        public DefaultController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public PartialViewResult SendMessage()
        {
            return PartialView();
        }
        [HttpPost]

        public async Task<IActionResult> SendMessage(Message message)
        {
            message.SendDate = DateTime.Now;
            message.IsRead = false;
            _context.Messages.Add(message);
            _context.SaveChanges();

            #region AI_Analiz
            string apikey = "api-key";
            string prompt = "Sen bir sigorta firmasının müşteri iletişim asistanısın.\r\n\r\nKurumsal ama samimi, net ve anlaşılır bir dille yaz.\r\n\r\nYanıtlarını 2–3 paragrafla sınırla.\r\n\r\nEksik bilgi (poliçe numarası, kimlik vb.) varsa kibarca talep et.\r\n\r\nFiyat, ödeme, teminat gibi kritik konularda kesin rakam verme, müşteri temsilcisine yönlendir.\r\n\r\nHasar ve sağlık gibi hassas durumlarda empati kur.\r\n\r\nCevaplarını teşekkür ve iyi dilekle bitir.\r\n\r\n Kullanıcının sana gönderdiği mesaj şu şekilde:' {message.MessageDetail}.'";

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.cerebras.ai/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apikey);
            client.DefaultRequestHeaders.Add("cerebras-version", "llama3.1-8b");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var requestBody = new
            {
                model = "llama3.1-8b",
                max_tokens = 1000,
                temperature = 0.5,
                messages = new object[]
{
    new
    {
        role = "system",
        content = "Sen bir sigorta firmasının müşteri iletişim asistanısın. Kurumsal ama samimi, net ve anlaşılır bir dille yaz. Yanıtlarını 2-3 paragrafla sınırla. Eksik bilgi varsa kibarca talep et. Fiyat, ödeme, teminat gibi kritik konularda kesin rakam verme. Hasar ve sağlık gibi hassas durumlarda empati kur. Cevaplarını teşekkür ve iyi dilekle bitir."
    },
    new
    {
        role = "user",
        content = message.MessageDetail
    }
}

            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/chat/completions", jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();

            var json = JsonNode.Parse(responseString);
            using var doc = JsonDocument.Parse(responseString);
            string? textContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
            ViewBag.v = textContent;

            #endregion

            #region Email_Gönderme

            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailboxAddressFrom = new MailboxAddress("InsureYouAI Admin", "ezgiorman10@gmail.com");
            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User", message.Email);
            mimeMessage.To.Add(mailboxAddressTo);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = textContent;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            mimeMessage.Subject = "InsureYou AI Email Yanıtı";

            SmtpClient client2 = new SmtpClient();
            client2.Connect("smtp.gmail.com", 587, false);
            client2.Authenticate("ezgiorman10@gmail.com", "alxe dfwc tqet lqmz");
            client2.Send(mimeMessage);
            client2.Disconnect(true);
            #endregion

            #region AIMessage_Kaydetme

            AIMessage aiMessage = new AIMessage()
            {
                MessageDetail = textContent,
                ReceiverEmail = message.Email,
                ReceiverNameSurname = message.NameSurname,
                SendDate = DateTime.Now
            };

            _context.AIMessages.Add(aiMessage);
            _context.SaveChanges();
            #endregion

            return RedirectToAction("Index");
        }
        public PartialViewResult SubscribeEmail()
        {
            return PartialView();
        }
        [HttpPost]

        public IActionResult SubscribeEmail(string email)
        {
            return View();
        }
    }
}
