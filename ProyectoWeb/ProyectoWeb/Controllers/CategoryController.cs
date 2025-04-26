// ProyectoWeb/Controllers/CategoryController.cs
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoWeb.Dtos;
using ProyectoWeb.Models;

namespace ProyectoWeb.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _http;
        public CategoryController(IHttpClientFactory http) => _http = http;

        private HttpClient GetClient()
        {
            var client = _http.CreateClient("ApiClient");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IActionResult> Index()
        {
            var cats = await GetClient()
                .GetFromJsonAsync<List<CategoryViewModel>>("api/category")
                     ?? new List<CategoryViewModel>();
            return View(cats);
        }

        public IActionResult Create()
            => View(new CategoryViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = new CreateCategoryDto(vm.Name);
            var res = await GetClient().PostAsJsonAsync("api/category", dto);
            var apiBody = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", $"API Error {res.StatusCode}: {apiBody}");
            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await GetClient()
                .GetFromJsonAsync<CategoryViewModel>($"api/category/{id}");
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = new CreateCategoryDto(vm.Name);
            var res = await GetClient().PutAsJsonAsync($"api/category/{id}", dto);
            var apiBody = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", $"API Error {res.StatusCode}: {apiBody}");
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await GetClient().DeleteAsync($"api/category/{id}");
            if (!res.IsSuccessStatusCode)
            {
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

