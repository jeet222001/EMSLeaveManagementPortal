using EMSLeaveManagementPortal.DTOs;
using EMSLeaveManagementPortal.Entities;
using EMSLeaveManagementPortal.Helpers;
using EMSLeaveManagementPortal.Repositories;
using EMSLeaveManagementPortal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> SignUp(SignUpDto dto)
        {
            try
            {
                if (_userRepo.GetByUsernameAsync(dto.Username) != null)
                    return BadRequest(new ApiResponseDto<object>(false, "Username already exists.", null, 400));

                PasswordHelper.CreatePasswordHash(dto.Password, out var hash, out var salt);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = dto.Username,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    Role = dto.Role,
                    Name = dto.Name
                };

               await _userRepo.AddAsync(user);

                return Ok(new ApiResponseDto<object>(true, "User registered successfully.", null, 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInDto dto)
        {
            try
            {
                var user = await _userRepo.GetByUsernameAsync(dto.Username);
                if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                    return Unauthorized(new ApiResponseDto<object>(false, "Invalid credentials.", null, 401));

                var token = _tokenService.CreateToken(user);
                // Exclude password hash and salt from response
                var userResponse = new 
                {
                    user.Id,
                    user.Username,
                    user.Name,
                    user.Role
                };
                var responseData = new { token, user = userResponse };
                return Ok(new ApiResponseDto<object>(true, "Sign in successful.", responseData, 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }
    }
}
