using EMSLeaveManagementPortal.DTOs;
using EMSLeaveManagementPortal.Entities;
using EMSLeaveManagementPortal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMSLeaveManagementPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveController : ControllerBase
{
    private readonly ILeaveRepository _leaveRepo;
    private readonly IUserRepository _userRepo;

    public LeaveController(ILeaveRepository leaveRepo, IUserRepository userRepo)
    {
        _leaveRepo = leaveRepo;
        _userRepo = userRepo;
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPost("apply")]
    public async Task<IActionResult> ApplyLeave([FromBody] ApplyLeaveDto dto)
    {
        try
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(nameIdentifier))
            {
                return BadRequest(new ApiResponseDto<object>(false, "User name is missing.", null, 400));
            }
            var userId = Guid.Parse(nameIdentifier);

            var leave = new Leave
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Type = dto.Type,
                Status = LeaveStatus.Pending
            };
            await _leaveRepo.AddAsync(leave);
            return Ok(new ApiResponseDto<Leave>(true, "Leave applied successfully.", leave, 200));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("approve-reject")]
    public async Task<IActionResult> ApproveRejectLeave([FromBody] ApproveRejectLeaveDto dto)
    {
        try
        {
            var leave = await _leaveRepo.GetByIdAsync(dto.LeaveId);
            if (leave == null)
                return NotFound(new ApiResponseDto<object>(false, "Leave not found.", null, 404));

            if (dto.Approve && leave.Status == LeaveStatus.Pending)
            {
                leave.Status = LeaveStatus.Approved;
            }
            else
            {
                leave.Status = LeaveStatus.Rejected;
            }
            await _leaveRepo.UpdateAsync(leave);
            return Ok(new ApiResponseDto<Leave>(true, "Leave status updated.", leave, 200));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
        }
    }

    [Authorize(Roles = "Admin,Employee,Manager")]
    [HttpGet("user/{userId}/leaves")]
    public async Task<IActionResult> GetAppliedLeavesByUserId([FromRoute] Guid userId)
    {
        try
        {
            var leaves = await _leaveRepo.GetByUserIdAsync(userId);
            if (leaves == null || !leaves.Any())
            {
                return NotFound(new ApiResponseDto<object>(false, "No leaves found for the specified user.", null, 404));
            }
            return Ok(new ApiResponseDto<IEnumerable<Leave>>(true, "Leaves fetched successfully.", leaves, 200));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
        }
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpDelete("user/{userId}/leave/{leaveId}")]
    public async Task<IActionResult> CancelLeave([FromRoute] Guid userId, [FromRoute] Guid leaveId)
    {
        try
        {
            var leave = await _leaveRepo.GetByIdAsync(leaveId);
            if (leave == null || leave.UserId != userId)
            {
                return NotFound(new ApiResponseDto<object>(false, "Leave not found for the specified user.", null, 404));
            }

            await _leaveRepo.DeleteAsync(leaveId);
            return Ok(new ApiResponseDto<object>(true, "Leave cancelled successfully.", null, 200));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllLeaves(
        [FromQuery] string? search,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? sortBy)
    {
        try
        {
            var leaves = await _leaveRepo.GetAllAsync();
            var users = await _userRepo.GetAllAsync();

            // Email filter (requires user lookup)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var matchedUsers = users
                    .Where(u => u.Username.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .Select(u => u.Id)
                    .ToList();

                leaves = leaves.Where(l =>
                    (matchedUsers.Contains(l.UserId)) ||
                    (l.Reason != null && l.Reason.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (Enum.GetName(typeof(LeaveType), l.Type)?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (Enum.GetName(typeof(LeaveStatus), l.Status)?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            // Date range filter
            if (startDate.HasValue)
                leaves = leaves.Where(l => l.StartDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                leaves = leaves.Where(l => l.EndDate <= endDate.Value).ToList();

            // Sorting
            leaves = sortBy?.ToLower() switch
            {
                "startdate" => leaves.OrderBy(l => l.StartDate).ToList(),
                "enddate" => leaves.OrderBy(l => l.EndDate).ToList(),
                "type" => leaves.OrderBy(l => l.Type).ToList(),
                "status" => leaves.OrderBy(l => l.Status).ToList(),
                _ => leaves.OrderByDescending(l => l.StartDate).ToList()
            };

            // Map leaves to DTO including UserName
            var leaveDtos = leaves.Select(l =>
            {
                var user = users.FirstOrDefault(u => u.Id == l.UserId);
                return new
                {
                    l.Id,
                    l.UserId,
                    UserName = user?.Username,
                    l.StartDate,
                    l.EndDate,
                    l.Reason,
                    l.Type,
                    l.Status
                };
            }).ToList();

            return Ok(new ApiResponseDto<IEnumerable<object>>(true, "Leaves fetched successfully.", leaveDtos, 200));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
        }
    }
}