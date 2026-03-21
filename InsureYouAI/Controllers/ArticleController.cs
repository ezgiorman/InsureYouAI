using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace InsureYouAI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly InsureContext _context;
        public ArticleController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult ArticleList()
        {
            var values = _context.Articles.ToList();
            return View(values);
        }
        [HttpGet]

        public IActionResult CreateArticle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateArticle(Article article)
        {
            article.CreatedDate = DateTime.Now;
            _context.Articles.Add(article);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }
        [HttpGet]
        public IActionResult UpdateArticle(int id)
        {
            var value = _context.Articles.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateArticle(Article article)
        {
            _context.Articles.Update(article);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }
        public IActionResult DeleteArticle(int id)
        {
            var value = _context.Articles.Find(id);
            _context.Articles.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");

        }

        [HttpGet]

        public IActionResult CreateArticleWithOpenAI()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticleWithOpenAI(string prompt)
        {
            var apiKey = "api-key";

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var requestData = new
            {
                model = "llama3.1-8b",
                messages = new[]
                {
            new {
                role = "system",
                content = "You are an insurance content writer. Write a detailed insurance article minimum 3000 characters."
            },
            new {
                role = "user",
                content = prompt
            }
        },
                temperature = 0.7,
                max_tokens = 1500
            };

            var response = await client.PostAsJsonAsync(
                "https://api.cerebras.ai/v1/chat/completions",
                requestData
            );

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CerebrasResponse>();
                var content = result.choices[0].message.content;
                ViewBag.article = content;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.article = "Cerebras API hatasi: " + error;
            }

            return View();
        }

        public class CerebrasResponse
        {
            public List<Choice> choices { get; set; }
        }

        public class Choice
        {
            public Message message { get; set; }
        }
        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}

