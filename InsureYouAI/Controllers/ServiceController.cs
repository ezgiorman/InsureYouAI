using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace InsureYouAI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly InsureContext _context;
        public ServiceController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult ServiceList()
        {
            var values = _context.Services.ToList();
            return View(values);
        }
        [HttpGet]

        public IActionResult CreateService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateService(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }
        [HttpGet]
        public IActionResult UpdateService(int id)
        {
            var value = _context.Services.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateService(Service service)
        {
            _context.Services.Update(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }
        public IActionResult DeleteService(int id)
        {
            var value = _context.Services.Find(id);
            _context.Services.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");

        }
        public async Task<IActionResult> CreateServiceWithCerebras()
        {
            string apiKey = "api-key";

            string prompt = "Bir sigorta sirketi icin hizmetler bolumu hazirla. "
            + "5 farkli hizmet olsun. Her biri maksimum 100 karakterlik cumle olsun. "
            + "Sadece liste halinde yaz.";

            using var client = new HttpClient();

            client.BaseAddress = new Uri("https://api.cerebras.ai/");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "llama3.1-8b",
                max_tokens = 300,
                temperature = 0.5,
                messages = new[]
                {
            new
            {
                role = "user",
                content = prompt
            }
        }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("v1/chat/completions", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.services = new List<string>
        {
            $"Cerebras API'den cevap alinamadi. Hata: {response.StatusCode}"
        };
                return View();
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);

            var fullText = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var services = fullText.Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.TrimStart('1', '2', '3', '4', '5', '.', '-', ' '))
                .ToList();

            ViewBag.services = services;

            return View();
        }
    }
}
