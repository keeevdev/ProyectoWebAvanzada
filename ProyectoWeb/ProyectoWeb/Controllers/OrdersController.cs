using System.Linq;
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
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _http;
        public OrdersController(IHttpClientFactory http) => _http = http;

        private HttpClient GetClient()
        {
            var client = _http.CreateClient("ApiClient");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }


        [Authorize(Roles = "usuario")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var menu = await GetClient().GetFromJsonAsync<List<MenuItemViewModel>>("api/menu");
            var vm = new CreateOrderViewModel
            {
                Items = menu!
                    .Select(m => new OrderItemViewModel
                    {
                        MenuItemId = m.Id,
                        Name = m.Name,
                        Price = m.Price,
                        Quantity = 0
                    })
                    .ToList()
            };
            return View(vm);
        }

        [Authorize(Roles = "usuario")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderViewModel vm)
        {
            if (!ModelState.IsValid || !vm.Items.Any(i => i.Quantity > 0))
            {
                var menu = await GetClient().GetFromJsonAsync<List<MenuItemViewModel>>("api/menu");
                vm.Items = menu!
                    .Select(m => new OrderItemViewModel
                    {
                        MenuItemId = m.Id,
                        Name = m.Name,
                        Price = m.Price,
                        Quantity = vm.Items
                                        .FirstOrDefault(x => x.MenuItemId == m.Id)?
                                        .Quantity ?? 0
                    }).ToList();

                if (!vm.Items.Any(i => i.Quantity > 0))
                    ModelState.AddModelError("", "Debes seleccionar al menos un ítem.");
                return View(vm);
            }

            var dto = new CreateOrderDto(
                vm.TableNumber,
                vm.PaymentMethod,
                vm.Items
                  .Where(i => i.Quantity > 0)
                  .Select(i => new OrderItemDto(i.MenuItemId, i.Quantity))
            );

            var res = await GetClient().PostAsJsonAsync("api/orders", dto);
            if (res.IsSuccessStatusCode)
                return RedirectToAction("Success");

            ModelState.AddModelError("", "Error al enviar el pedido.");
            var menuOnError = await GetClient().GetFromJsonAsync<List<MenuItemViewModel>>("api/menu");
            vm.Items = menuOnError!
                .Select(m => new OrderItemViewModel
                {
                    MenuItemId = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    Quantity = vm.Items
                                    .FirstOrDefault(x => x.MenuItemId == m.Id)?
                                    .Quantity ?? 0
                }).ToList();
            return View(vm);
        }

        [Authorize(Roles = "usuario")]
        public IActionResult Success() => View();


        [Authorize(Roles = "empleado")]
        public async Task<IActionResult> Pending()
        {
            var orders = await GetClient()
                .GetFromJsonAsync<List<PendingOrderViewModel>>("api/orders/employee");
            return View(orders!);
        }

        [Authorize(Roles = "empleado")]
        public async Task<IActionResult> Details(int orderId)
        {
            var items = await GetClient()
                .GetFromJsonAsync<List<OrderItemDetailViewModel>>($"api/orders/{orderId}/items")
                ?? new List<OrderItemDetailViewModel>();

            var vm = new OrderDetailsViewModel
            {
                OrderId = orderId,
                Items = items,
                AssignTime = new AssignTimeViewModel()
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "empleado")]
        public async Task<IActionResult> AssignTime(int orderId, AssignTimeViewModel AssignTime)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", new { orderId });

            await GetClient()
                .PutAsJsonAsync($"api/orders/employee/assign/{orderId}", AssignTime);
            return RedirectToAction("Pending");
        }

        [HttpPost]
        [Authorize(Roles = "empleado")]
        public async Task<IActionResult> Complete(int orderId)
        {
            await GetClient()
                .PutAsync($"api/orders/employee/complete/{orderId}", null);
            return RedirectToAction("Pending");
        }


        [Authorize(Roles = "usuario")]
        public async Task<IActionResult> MyOrders()
        {
            var orders = await GetClient()
                .GetFromJsonAsync<List<PendingOrderViewModel>>("api/orders/user");
            return View(orders!);
        }
    }
}







