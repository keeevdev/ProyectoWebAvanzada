using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoWeb.Models;

namespace ProyectoWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _http;
        public AccountController(IHttpClientFactory http) => _http = http;

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _http.CreateClient("ApiClient");
            var res = await client.PostAsJsonAsync("api/auth/register", vm);

            if (res.IsSuccessStatusCode)
                return RedirectToAction("Login");

            ModelState.AddModelError("", "No se pudo registrar.");
            return View(vm);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _http.CreateClient("ApiClient");
            var res = await client.PostAsJsonAsync("api/auth/login", vm);

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Usuario o contraseña inválidos.");
                return View(vm);
            }

            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var token = doc.RootElement.GetProperty("token").GetString()!;

            HttpContext.Session.SetString("JWToken", token);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Limpia la sesión y la cookie
            HttpContext.Session.Remove("JWToken");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _http.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync("api/auth/profile");
            if (!res.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var vm = await res.Content.ReadFromJsonAsync<ProfileViewModel>();
            return View(vm!);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _http.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await client.PutAsJsonAsync("api/auth/profile", vm);
            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "No se pudo actualizar el perfil.");
                return View(vm);
            }

            var roles = User.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => new Claim(ClaimTypes.Role, c.Value))
                            .ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                new Claim(ClaimTypes.Name, vm.Username)
            };
            claims.AddRange(roles);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
    }
}



