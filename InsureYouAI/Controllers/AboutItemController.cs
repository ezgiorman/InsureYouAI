using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace InsureYouAI.Controllers
{
    public class AboutItemController : Controller
    {
        private readonly InsureContext _context;
        public AboutItemController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult AboutItemList()
        {
            var values = _context.AboutItems.ToList();
            return View(values);
        }
        [HttpGet]

        public IActionResult CreateAboutItem()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAboutItem(AboutItem aboutItem)
        {
            _context.AboutItems.Add(aboutItem);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }
        [HttpGet]
        public IActionResult UpdateAboutItem(int id)
        {
            var value = _context.AboutItems.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateAboutItem(AboutItem aboutItem)
        {
            _context.AboutItems.Update(aboutItem);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }
        public IActionResult DeleteAboutItem(int id)
        {
            var value = _context.AboutItems.Find(id);
            _context.AboutItems.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");

        }
        [HttpGet]
        public async Task<IActionResult> CreateAboutItemWithCerebras()
        {
            var apiKey = "api-key";

            var requestBody = new
            {
                model = "llama3.1-8b",
                messages = new[]
                {
            new
            {
                role = "user",
                content = "Kurumsal bir sigorta firmasi icin etkileyici, guven verici ve profesyonel bir 'hakkimizda alanlari' yazisi olustur. Ornegin: 'Geleceğinizi güvence altına alan kapsamlı sigorta çzöümleri sunuypruz.' şeklinde veya bunun gibi ve buna benzer daha zengin icerikler gelsin. En az 10 tane item istiyorum."
            }
        },
                max_tokens = 800
            };

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync(
                "https://api.cerebras.ai/v1/chat/completions",
                content
            );

            var responseJson = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseJson);

            if (jsonDoc.RootElement.TryGetProperty("choices", out var choices))
            {
                var aboutText = choices[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                ViewBag.AboutText = aboutText;
            }
            else
            {
                ViewBag.AboutText = "API cevabi: " + responseJson;
            }
            return View();
        }
    }
}
