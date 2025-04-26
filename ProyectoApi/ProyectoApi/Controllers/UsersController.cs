using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Interfaces;
using ProyectoApi.Models;
using ProyectoApi.Dtos;

namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userRepo.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepo.GetByIdWithRoleAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dto.Password));

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hash,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.CreateAsync(user, hash);

            var createdUser = await _userRepo.GetByUsernameAsync(dto.Username);
            if (createdUser == null)
                return StatusCode(500, "User creation failed unexpectedly.");

            await _userRepo.UpdateRoleAsync(createdUser.Id, dto.RoleId);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            await _userRepo.UpdateAsync(id, dto.Username, dto.Email);

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                using var hmac = new System.Security.Cryptography.HMACSHA512();
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dto.Password));
                await _userRepo.UpdatePasswordAsync(id, passwordHash);
            }

            await _userRepo.UpdateRoleAsync(id, dto.RoleId);

            return NoContent();
        }
    }
}
