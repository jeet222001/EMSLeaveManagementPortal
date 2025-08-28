using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.Repositories;
public interface ILeaveRepository
{
    void Add(Leave leave);
    Leave GetById(Guid id);
    IEnumerable<Leave> GetByUserId(Guid userId);
    IEnumerable<Leave> GetAll();
    void Update(Leave leave);
    void Delete(Guid id);
    LeaveBalance GetLeaveBalance(Guid userId, LeaveType type);
    void UpdateLeaveBalance(LeaveBalance balance);
    void AddLeaveBalance(LeaveBalance balance);
    IEnumerable<LeaveBalance> GetLeaveBalancesByUser(Guid userId);

}
