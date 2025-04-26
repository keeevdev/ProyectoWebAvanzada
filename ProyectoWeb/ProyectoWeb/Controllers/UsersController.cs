using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProyectoWeb.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _http;

        public UsersController(IHttpClientFactory http) => _http = http;

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var client = _http.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var users = await client.GetFromJsonAsync<List<UserViewModel>>("api/users");
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var client = _http.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var roles = await client.GetFromJsonAsync<List<RoleViewModel>>("api/roles");
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var token = HttpContext.Session.GetString("JWToken");
                var client = _http.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var roles = await client.GetFromJsonAsync<List<RoleViewModel>>("api/roles");
                ViewBag.Roles = new SelectList(roles, "Id", "Name");
                return View(vm);
            }

            var tokenPost = HttpContext.Session.GetString("JWToken");
            var clientPost = _http.CreateClient("ApiClient");
            clientPost.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenPost);

            var res = await clientPost.PostAsJsonAsync("api/users", vm);
            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create user.");
                return View(vm);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var client = _http.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var user = await client.GetFromJsonAsync<UserViewModel>($"api/users/{id}");
            var roles = await client.GetFromJsonAsync<List<RoleViewModel>>("api/roles");

            ViewBag.Roles = new SelectList(roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var token = HttpContext.Session.GetString("JWToken");
            var client = _http.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await client.PutAsJsonAsync($"api/users/{id}", vm);
            if (!res.IsSuccessStatusCode)
                ModelState.AddModelError("", "Failed to update user.");

            return RedirectToAction("Index");
        }
    }
}
