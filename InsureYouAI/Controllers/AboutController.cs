using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders.Composite;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class AboutController : Controller
    {
        private readonly InsureContext _context;
        public AboutController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult AboutList()
        {
            var values = _context.Abouts.ToList();
            return View(values);
        }
        [HttpGet]

        public IActionResult CreateAbout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAbout(About about)
        {
            _context.Abouts.Add(about);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }
        public IActionResult DeleteAbout(int id)
        {
            var value = _context.Abouts.Find(id);
            _context.Abouts.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("AboutList");

        }
        [HttpGet]
        public IActionResult UpdateAbout(int id)
        {
            var value = _context.Abouts.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateAbout(About about)
        {
            _context.Abouts.Update(about);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }
        [HttpGet]
        public async Task<IActionResult> CreateAboutWithCerebras()
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
                content = "Kurumsal bir sigorta firmasi icin etkileyici, guven verici ve profesyonel bir hakkimizda yazisi olustur."
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