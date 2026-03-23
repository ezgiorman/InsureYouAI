using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class AppUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly InsureContext _context;
        public AppUserController(UserManager<AppUser> userManager, InsureContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult UserList()
        {
            var values = _userManager.Users.ToList();
            return View(values);
        }
        public async Task<IActionResult> UserProfileWithAI(string id)
        {
            var values = await _userManager.FindByIdAsync(id);
            ViewBag.name = values.Name;
            ViewBag.surname = values.Surname;
            ViewBag.imageUrl = values.ImageUrl;
            ViewBag.description = values.Description;
            ViewBag.titlevalue = values.Title;
            ViewBag.city = values.City;
            ViewBag.education = values.Education;


            //kullanıcı bilgilerini çekelim
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var articles = _context.Articles.Where(x => x.AppUserId == id).ToList();

            if (articles.Count == 0)
            { 
                ViewBag.AIResult = "Bu kullanıcı henüz makale yazmamıştır.";
                return View(user);
            }

            //makaleleri birleştirip AI'ya gönderelim
            var allArticles = string.Join("\n\n", articles);

            var apikey = "api-key";

            //prompt yazımı

            var promp = $@"Sen sigorta şirketinde bir uzman içerik analistisin. Elinizde, bir sigorta şirketinin çalışanının yazdığı tüm makaleler var. 
Bu makaleler üzerinden çalışanın içerik üretim tarzını analiz et.

Analiz Başlıklarıİ
1) Konu çeşitliliği ve odak alanları(sağlık, hayat, kasko, tamamlayıcı, BES vb.)
2) Hedef kitle tahmini (bireysel/kurumsal, sehment, persona)
3) Dil ve anlatım tarzı (tekniklik seviyesi, okunabilirlik, ikna gücü)
4) Sigorta terimlerini kullanma ve doğruluk düzeyi
5) Müşteri ihtiyaçlarına ve risk yönetimine odaklanma
6) Pazarlama/satış vurgusu, CTA netliği
) Geliştirilmesi gereken alanlar ve net aksiyon maddeleri

Makaleler:

{allArticles}

Lütfen çıktıyı profesyonel rapor formatında, madde madde ve en sonda 5 maddelik aksiyon listesi ile ver.";

            //Cerebras Chat Completions

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);

            var body = new
            {
                model = "llama3.1-8b",
                messages = new object[]
                {
                    new { role="system", content="Sen sigorta sektöründe içerik analizi yapan bir uzmansın."},
                    new {role="user", content=promp}
                },
                max_tokens = 1000,
                temperature = 0.2
            };

            //json dönüşümleri

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await client.PostAsync("https://api.cerebras.ai/v1/chat/completions", content);
            var respText = await httpResponse.Content.ReadAsStringAsync();

            if(!httpResponse.IsSuccessStatusCode)
            {
                ViewBag.AIResult = "Cerebras LLM Hatası:" + httpResponse.StatusCode;
                return View(user);
            }

            //json yapı içinden veriyi okuma
            try
            {
                using var doc = JsonDocument.Parse(respText);
                var aiText = doc.RootElement
                                .GetProperty("choices")[0]
                                .GetProperty("message")
                                .GetProperty("content")
                                .GetString();
                ViewBag.AIResult = aiText ?? "Boş yanıt döndü.";
            }
            catch
            {
                ViewBag.AIResult = "Cerebras LLM Yanıtı İşlenirken Hata Oluştu.";
            }


            return View(user);
        }

        public async Task<IActionResult> UserCommentsProfileWithAI(string id)
        {
            var values = await _userManager.FindByIdAsync(id);
            ViewBag.name = values.Name;
            ViewBag.surname = values.Surname;
            ViewBag.imageUrl = values.ImageUrl;
            ViewBag.description = values.Description;
            ViewBag.titlevalue = values.Title;
            ViewBag.city = values.City;
            ViewBag.education = values.Education;


            //kullanıcı bilgilerini çekelim
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Where(x => x.AppUserId == id)
                .Select(y => y.CommentDetail)
                .ToListAsync();

            if (comments.Count == 0)
            {
                ViewBag.AIResult = "Bu kullanıcı henüz yorum yazmamıştır.";
                return View(user);
            }

            //makaleleri birleştirip AI'ya gönderelim
            var allComments = string.Join("\n\n", comments);

            var apikey = "csk-nhh59fddyfdtdx3xnhk6hwnxm9p333ydtwwj6fdxxr44k3jc";

            //prompt yazımı

            var promp = $@"
Sen kullanıcı davranış analizi yapan bir yapay zeka uzmanısın. 
Aşağıdaki yorumlara göre kullanıcıyı değerlendir.

Analiz Başlıkları:
1) Genel duygu durumu (pozitif/negatif/nötr)
2) Toksik içerik var mı? (örnekleriyle)
3) ilgi alanları / konu başlıkları
4) iletişim tarzı (resmi, samimi, agresif, vb.)
5 geliştirilmesi gereken iletişim alanları
6) 5 maddelik kısa özet


Yorumlar:

{allComments}";


            //Cerebras Chat Completions

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);

            var body = new
            {
                model = "llama3.1-8b",
                messages = new object[]
                {
                    new { role="system", content="Sen kullanıcı yorum analizi yapan bir uzmansın."},
                    new {role="user", content=promp}
                },
                max_tokens = 1000,
                temperature = 0.2
            };

            //json dönüşümleri

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await client.PostAsync("https://api.cerebras.ai/v1/chat/completions", content);
            var respText = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                ViewBag.AIResult = "Cerebras LLM Hatası:" + httpResponse.StatusCode;
                return View(user);
            }

            //json yapı içinden veriyi okuma
            try
            {
                using var doc = JsonDocument.Parse(respText);
                var aiText = doc.RootElement
                                .GetProperty("choices")[0]
                                .GetProperty("message")
                                .GetProperty("content")
                                .GetString();
                ViewBag.AIResult = aiText ?? "Boş yanıt döndü.";
            }
            catch
            {
                ViewBag.AIResult = "Cerebras LLM Yanıtı İşlenirken Hata Oluştu.";
            }


            return View(user);
        }
    }
}
