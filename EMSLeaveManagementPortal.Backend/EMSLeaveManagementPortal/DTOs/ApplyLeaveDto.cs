using EMSLeaveManagementPortal.Entities;
using System.ComponentModel.DataAnnotations;

namespace EMSLeaveManagementPortal.DTOs;
public class ApplyLeaveDto
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required, StringLength(200)]
    public string Reason { get; set; }

    [Required]
    public LeaveType Type { get; set; } 
}
