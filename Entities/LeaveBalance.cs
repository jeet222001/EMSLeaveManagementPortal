namespace EMSLeaveManagementPortal.Entities;
public class LeaveBalance
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public LeaveType Type { get; set; }
    public int Balance { get; set; }
}
