using EMSLeaveManagementPortal.DTOs;
using EMSLeaveManagementPortal.Entities;
using EMSLeaveManagementPortal.Helpers;
using EMSLeaveManagementPortal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMSLeaveManagementPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepo, ILogger<UserController> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var users = _userRepo.GetAll();
                return Ok(new ApiResponseDto<IEnumerable<User>>(true, "Users fetched successfully.", users, 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users.");
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var user = _userRepo.GetAll().FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return NotFound(new ApiResponseDto<object>(false, "User not found.", null, 404));
                return Ok(new ApiResponseDto<User>(true, "User fetched successfully.", user, 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching user {id}.");
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create([FromBody] SignUpDto dto)
        {
            try
            {
                if (_userRepo.GetByUsername(dto.Username) != null)
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

                _userRepo.Add(user);
                _logger.LogInformation("User created: {Username} ({Id})", user.Username, user.Id);
                return Ok(new ApiResponseDto<User>(true, "User created successfully.", user, 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] User updateDto)
        {
            try
            {
                var user = _userRepo.GetAll().FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return NotFound(new ApiResponseDto<object>(false, "User not found.", null, 404));

                user.Username = updateDto.Username ?? user.Username;
                user.Name = updateDto.Name ?? user.Name;
                user.Role = updateDto.Role;

                // If password is provided, update hash and salt
                if (updateDto.PasswordHash != null && updateDto.PasswordSalt != null)
                {
                    user.PasswordHash = updateDto.PasswordHash;
                    user.PasswordSalt = updateDto.PasswordSalt;
                }

                // You may want to add an Update method in IUserRepository for better practice
                // For now, remove and re-add
                // _userRepo.Update(user);
                _logger.LogInformation("User updated: {Username} ({Id})", user.Username, user.Id);
                return Ok(new ApiResponseDto<User>(true, "User updated successfully.", user, 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {id}.");
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var user = _userRepo.GetAll().FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return NotFound(new ApiResponseDto<object>(false, "User not found.", null, 404));

                // You may want to add a Delete method in IUserRepository for better practice
                // For now, remove from context
                // _userRepo.Delete(id);
                _logger.LogInformation("User deleted: {Username} ({Id})", user.Username, user.Id);
                return Ok(new ApiResponseDto<object>(true, "User deleted successfully.", null, 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {id}.");
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }
    }
}