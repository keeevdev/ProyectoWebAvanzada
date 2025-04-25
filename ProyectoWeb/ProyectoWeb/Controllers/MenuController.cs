using System;
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

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var items = await GetClient()
                .GetFromJsonAsync<List<MenuItemViewModel>>("api/menu");
            return View(items!);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var item = await GetClient()
                .GetFromJsonAsync<MenuItemViewModel>($"api/menu/{id}");
            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
            => View(new CreateMenuItemViewModel());

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CreateMenuItemViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (vm.ImageFile != null)
            {
                try
                {
                    var imagesFolder = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(imagesFolder);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ImageFile.FileName)}";
                    var filePath = Path.Combine(imagesFolder, fileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await vm.ImageFile.CopyToAsync(stream);
                    vm.ImageUrl = $"/images/{fileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "No se pudo guardar la imagen: " + ex.Message);
                    return View(vm);
                }
            }

            var dto = new CreateMenuItemDto(vm.Name, vm.Description, vm.Price, vm.ImageUrl);
            var res = await GetClient().PostAsJsonAsync("api/menu", dto);
            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al crear el ítem de menú.");
            return View(vm);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var apiVm = await GetClient()
                .GetFromJsonAsync<MenuItemViewModel>($"api/menu/{id}");
            if (apiVm is null) return NotFound();

            var vm = new CreateMenuItemViewModel
            {
                Name = apiVm.Name,
                Description = apiVm.Description,
                Price = apiVm.Price,
                ImageUrl = apiVm.ImageUrl
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, CreateMenuItemViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (vm.ImageFile != null)
            {
                try
                {
                    var imagesFolder = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(imagesFolder);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ImageFile.FileName)}";
                    var filePath = Path.Combine(imagesFolder, fileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await vm.ImageFile.CopyToAsync(stream);
                    vm.ImageUrl = $"/images/{fileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "No se pudo guardar la imagen: " + ex.Message);
                    return View(vm);
                }
            }

            var dto = new CreateMenuItemDto(vm.Name, vm.Description, vm.Price, vm.ImageUrl);
            var res = await GetClient().PutAsJsonAsync($"api/menu/{id}", dto);
            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al editar el ítem de menú.");
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await GetClient().DeleteAsync($"api/menu/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}








