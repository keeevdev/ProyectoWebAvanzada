using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoWeb.Models;

namespace ProyectoWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _http;
        public HomeController(IHttpClientFactory http) => _http = http;

        private HttpClient GetClient()
        {
            var client = _http.CreateClient("ApiClient");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // GET: /Home/Index
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var menu = await client.GetFromJsonAsync<List<MenuItemViewModel>>("api/menu");
            return View(menu);
        }
    }
}
