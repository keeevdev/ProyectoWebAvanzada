using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Dtos;
using ProyectoApi.Interfaces;

namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IUserRepository _userRepo;

        public AuthController(IAuthService auth, IUserRepository userRepo)
        {
            _auth = auth;
            _userRepo = userRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _auth.RegisterAsync(dto);
            return Created(string.Empty, new { Message = "Usuario registrado exitosamente." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _auth.LoginAsync(dto);
            return Ok(new { Token = token });
        }

        [HttpGet("admin-only")]
        [Authorize(Roles = "admin")]
        public IActionResult AdminOnly()
        {
            return Ok("Sólo admins pueden ver esto");
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _userRepo.GetByIdAsync(userId);
            if (user is null)
                return NotFound();

            return Ok(new ProfileDto(user.Username, user.Email));
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(ProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await _userRepo.UpdateAsync(userId, dto.Username, dto.Email);
            return NoContent();
        }
    }
}


