namespace EMSLeaveManagementPortal.DTOs;
public class ApplyLeaveDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; }
    public EMSLeaveManagementPortal.Entities.LeaveType Type { get; set; } 
}
