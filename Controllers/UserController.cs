using EMSLeaveManagementPortal.DTOs;
using EMSLeaveManagementPortal.Entities;
using EMSLeaveManagementPortal.Helpers;
using EMSLeaveManagementPortal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMSLeaveManagementPortal.Controllers;


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
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = (await _userRepo.GetAllAsync())
                .Select(u => new UserDetailsDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Name = u.Name,
                    Role = u.Role
                });
            return Ok(new ApiResponseDto<IEnumerable<UserDetailsDto>>(true, "Users fetched successfully.", users, 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users.");
            return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var user = (await _userRepo.GetAllAsync()).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ApiResponseDto<object>(false, "User not found.", null, 404));
            var userDto = new UserDetailsDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Role = user.Role
            };
            return Ok(new ApiResponseDto<UserDetailsDto>(true, "User fetched successfully.", userDto, 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching user {id}.");
            return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SignUpDto dto)
    {
        try
        {
            if (await _userRepo.GetByUsernameAsync(dto.Username) != null)
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
            _logger.LogInformation("User created: {Username} ({Id})", user.Username, user.Id);

            var userDto = new UserDetailsDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Role = user.Role
            };

            return Ok(new ApiResponseDto<UserDetailsDto>(true, "User created successfully.", userDto, 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user.");
            return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto updateDto)
    {
        try
        {
            var user = (await _userRepo.GetAllAsync()).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ApiResponseDto<object>(false, "User not found.", null, 404));

            user.Username = updateDto.Username ?? user.Username;
            user.Name = updateDto.Name ?? user.Name;
            user.Role = updateDto.Role;

            // If password is provided, update hash and salt
            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                PasswordHelper.CreatePasswordHash(updateDto.Password, out var hash, out var salt);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;
            }

            await _userRepo.UpdateUserAsync(user);

            _logger.LogInformation("User updated: {Username} ({Id})", user.Username, user.Id);

            var userDto = new UserDetailsDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Role = user.Role
            };

            return Ok(new ApiResponseDto<UserDetailsDto>(true, "User updated successfully.", userDto, 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating user {id}.");
            return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var user = (await _userRepo.GetAllAsync()).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ApiResponseDto<object>(false, "User not found.", null, 404));

            await _userRepo.DeleteUserAsync(user);
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

