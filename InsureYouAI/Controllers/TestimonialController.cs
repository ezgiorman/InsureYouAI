using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace InsureYouAI.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly InsureContext _context;
        public TestimonialController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult TestimonialList()
        {
            var values = _context.Testimonials.ToList();
            return View(values);
        }
        [HttpGet]

        public IActionResult CreateTestimonial()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTestimonial(Testimonial testimonial)
        {
            _context.Testimonials.Add(testimonial);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }
        [HttpGet]
        public IActionResult UpdateTestimonial(int id)
        {
            var value = _context.Testimonials.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateTestimonial(Testimonial testimonial)
        {
            _context.Testimonials.Update(testimonial);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }
        public IActionResult DeleteTestimonial(int id)
        {
            var value = _context.Testimonials.Find(id);
            _context.Testimonials.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");

        }

        public async Task<IActionResult>CreateTestimonialWithCerebras()
        {
            string apiKey = "api-key";

            string prompt = "Bir sigorta sirketi icin müşteri deneyimlerine dair yorum oluşturmak istiyorum yani İngilizce karşılığı ile: testimonial. Bu alanda Türkçe olarak 6 tane yorum, 6 tane müşteri adı ve soyadı, bu müşterilerin unvanı olsun. Buna göre içeriği hazırla.";

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
                ViewBag.testimonials = new List<string>
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

            var testimonials = fullText.Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.TrimStart('1', '2', '3', '4', '5', '.', '-', ' '))
                .ToList();

            ViewBag.testimonials = testimonials;
            return View();
        }
    }
}

