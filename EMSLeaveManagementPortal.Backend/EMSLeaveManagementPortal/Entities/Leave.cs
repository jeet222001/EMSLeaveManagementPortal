namespace EMSLeaveManagementPortal.Entities;
public class Leave
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; }
    public LeaveType Type { get; set; } 
    public LeaveStatus Status { get; set; }
}
