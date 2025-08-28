using EMSLeaveManagementPortal.DTOs;
using EMSLeaveManagementPortal.Entities;
using EMSLeaveManagementPortal.Helpers;
using EMSLeaveManagementPortal.Repositories;
using EMSLeaveManagementPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace EMSLeaveManagementPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;

        public AuthController(IUserRepository userRepo, ITokenService tokenService)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
        }

        [HttpPost("signup")]
        public IActionResult SignUp(SignUpDto dto)
        {
            if (_userRepo.GetByUsername(dto.Username) != null)
                return BadRequest("Username already exists.");

            PasswordHelper.CreatePasswordHash(dto.Password, out var hash, out var salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = dto.Role
            };

            _userRepo.Add(user);

            return Ok("User registered successfully.");
        }

        [HttpPost("signin")]
        public IActionResult SignIn(SignInDto dto)
        {
            var user = _userRepo.GetByUsername(dto.Username);
            if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid credentials.");

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }
    }
}
