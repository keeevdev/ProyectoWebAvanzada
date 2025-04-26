// ProyectoWeb/Controllers/MenuController.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ProyectoWeb.Dtos;
using ProyectoWeb.Models;

namespace ProyectoWeb.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IWebHostEnvironment _env;

        public MenuController(IHttpClientFactory http, IWebHostEnvironment env)
        {
            _http = http;
            _env = env;
        }

        private HttpClient GetClient()
        {
            var client = _http.CreateClient("ApiClient");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private async Task<List<CategoryViewModel>> LoadCategoriesAsync()
            => await GetClient()
                .GetFromJsonAsync<List<CategoryViewModel>>("api/category")
                ?? new List<CategoryViewModel>();

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? selectedCategoryId)
        {
            var categories = await LoadCategoriesAsync();

            var url = "api/menu" +
                      (selectedCategoryId.HasValue
                          ? $"?categoryId={selectedCategoryId.Value}"
                          : "");

            var items = await GetClient()
                .GetFromJsonAsync<List<MenuItemViewModel>>(url)
                ?? new List<MenuItemViewModel>();

            var vm = new MenuIndexViewModel
            {
                Categories = categories,
                SelectedCategoryId = selectedCategoryId,
                Items = items
            };
            return View(vm);
        }

        // GET: /Menu/Details/{id}
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var item = await GetClient()
                .GetFromJsonAsync<MenuItemViewModel>($"api/menu/{id}");
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Menu/Create
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await LoadCategoriesAsync();
            return View(new CreateMenuItemViewModel());
        }

        // POST: /Menu/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CreateMenuItemViewModel vm)
        {
            ViewBag.Categories = await LoadCategoriesAsync();
            if (!ModelState.IsValid)
                return View(vm);

            if (vm.ImageFile != null)
            {
                var imagesFolder = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(imagesFolder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ImageFile.FileName)}";
                var path = Path.Combine(imagesFolder, fileName);
                await using var fs = new FileStream(path, FileMode.Create);
                await vm.ImageFile.CopyToAsync(fs);
                vm.ImageUrl = $"/images/{fileName}";
            }

            var dto = new CreateMenuItemDto(
                vm.Name,
                vm.Description,
                vm.Price,
                vm.ImageUrl,
                vm.CategoryId
            );
            var res = await GetClient().PostAsJsonAsync("api/menu", dto);
            var apiBody = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", $"API Error {res.StatusCode}: {apiBody}");
            return View(vm);
        }

        // GET: /Menu/Edit/{id}
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var apiVm = await GetClient()
                .GetFromJsonAsync<MenuItemViewModel>($"api/menu/{id}");
            if (apiVm == null) return NotFound();

            ViewBag.Categories = await LoadCategoriesAsync();
            return View(new CreateMenuItemViewModel
            {
                Name = apiVm.Name,
                Description = apiVm.Description,
                Price = apiVm.Price,
                ImageUrl = apiVm.ImageUrl,
                CategoryId = apiVm.CategoryId
            });
        }

        // POST: /Menu/Edit/{id}
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, CreateMenuItemViewModel vm)
        {
            ViewBag.Categories = await LoadCategoriesAsync();
            if (!ModelState.IsValid)
                return View(vm);

            if (vm.ImageFile != null)
            {
                var imagesFolder = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(imagesFolder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ImageFile.FileName)}";
                var path = Path.Combine(imagesFolder, fileName);
                await using var fs = new FileStream(path, FileMode.Create);
                await vm.ImageFile.CopyToAsync(fs);
                vm.ImageUrl = $"/images/{fileName}";
            }

            var dto = new CreateMenuItemDto(vm.Name, vm.Description, vm.Price, vm.ImageUrl, vm.CategoryId);
            var res = await GetClient().PutAsJsonAsync($"api/menu/{id}", dto);
            var apiBody = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", $"API Error {res.StatusCode}: {apiBody}");
            return View(vm);
        }

        // POST: /Menu/Delete/{id}
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await GetClient().DeleteAsync($"api/menu/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}














