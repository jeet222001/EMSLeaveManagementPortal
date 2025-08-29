using EMSLeaveManagementPortal.DTOs;
using EMSLeaveManagementPortal.Entities;
using EMSLeaveManagementPortal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMSLeaveManagementPortal.Controllers
{
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
        public IActionResult ApplyLeave([FromBody] ApplyLeaveDto dto)
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
                _leaveRepo.Add(leave);
                return Ok(new ApiResponseDto<Leave>(true, "Leave applied successfully.", leave, 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("approve-reject")]
        public IActionResult ApproveRejectLeave([FromBody] ApproveRejectLeaveDto dto)
        {
            try
            {
                var leave = _leaveRepo.GetById(dto.LeaveId);
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
                _leaveRepo.Update(leave);
                return Ok(new ApiResponseDto<Leave>(true, "Leave status updated.", leave, 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }
        [Authorize(Roles = "Admin,Employee,Manager")]
        [HttpGet("user/{userId}/leaves")]
        public IActionResult GetAppliedLeavesByUserId([FromRoute] Guid userId)
        {
            try
            {
                var leaves = _leaveRepo.GetByUserId(userId);
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
        public IActionResult CancelLeave([FromRoute] Guid userId, [FromRoute] Guid leaveId)
        {
            try
            {
                var leave = _leaveRepo.GetById(leaveId);
                if (leave == null || leave.UserId != userId)
                {
                    return NotFound(new ApiResponseDto<object>(false, "Leave not found for the specified user.", null, 404));
                }

                _leaveRepo.Delete(leaveId);
                return Ok(new ApiResponseDto<object>(true, "Leave cancelled successfully.", null, 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("all")]
        public IActionResult GetAllLeaves(
            [FromQuery] string? search,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? sortBy)
        {
            try
            {
                var leaves = _leaveRepo.GetAll().ToList();

                // Email filter (requires user lookup)
                if (!string.IsNullOrWhiteSpace(search))
                {
                    // Get users matching the search string in email
                    var matchedUsers = _userRepo.GetAll()
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
                    _ => leaves.OrderByDescending(l => l.StartDate).ToList() // Default sort
                };

                return Ok(new ApiResponseDto<IEnumerable<Leave>>(true, "Leaves fetched successfully.", leaves, 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>(false, ex.Message, null, 500));
            }
        }
    }
}