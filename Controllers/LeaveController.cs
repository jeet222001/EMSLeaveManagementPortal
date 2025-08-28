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

        public LeaveController(ILeaveRepository leaveRepo)
        {
            _leaveRepo = leaveRepo;
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("apply")]
        public IActionResult ApplyLeave([FromBody] ApplyLeaveDto dto)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(nameIdentifier))
            {
                return BadRequest("User name is missing.");
            }
            var userId = Guid.Parse(nameIdentifier);

            // Calculate requested days
            int requestedDays = (dto.EndDate - dto.StartDate).Days + 1;

            // Get leave balance
            var balance = _leaveRepo.GetLeaveBalance(userId, dto.Type);
            if (balance == null || balance.Balance < requestedDays)
            {
                return BadRequest("Insufficient leave balance.");
            }

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
            return Ok(leave);
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("approve-reject")]
        public IActionResult ApproveRejectLeave([FromBody] ApproveRejectLeaveDto dto)
        {
            var leave = _leaveRepo.GetById(dto.LeaveId);
            if (leave == null) return NotFound();

            if (dto.Approve && leave.Status == LeaveStatus.Pending)
            {
                int days = (leave.EndDate - leave.StartDate).Days + 1;
                var balance = _leaveRepo.GetLeaveBalance(leave.UserId, leave.Type);
                if (balance == null || balance.Balance < days)
                {
                    return BadRequest("Insufficient leave balance for approval.");
                }
                balance.Balance -= days;
                _leaveRepo.UpdateLeaveBalance(balance);
                leave.Status = LeaveStatus.Approved;
            }
            else
            {
                leave.Status = LeaveStatus.Rejected;
            }
            _leaveRepo.Update(leave);
            return Ok(leave);
        }
        [Authorize]
        [HttpGet("leave-balances")]
        public IActionResult GetLeaveBalances()
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(nameIdentifier))
            {
                return BadRequest("User identifier is missing.");
            }
            var userId = Guid.Parse(nameIdentifier);
            var balances = _leaveRepo.GetLeaveBalancesByUser(userId);
            return Ok(balances);
        }
        [Authorize]
        [HttpGet("initialize-leave-balance")]
        public void InitializeLeaveBalances(Guid userId)
        {
            foreach (LeaveType type in Enum.GetValues(typeof(LeaveType)))
            {
                var balance = new LeaveBalance
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = type,
                    Balance = 20 // Example: 20 days per type
                };
                _leaveRepo.AddLeaveBalance(balance);
            }
        }
    }
}