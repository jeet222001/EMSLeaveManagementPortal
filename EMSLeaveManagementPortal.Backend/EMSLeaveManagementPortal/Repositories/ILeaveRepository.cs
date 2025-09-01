using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.Repositories;
public interface ILeaveRepository
{
    Task AddAsync(Leave leave);
    Task<Leave?> GetByIdAsync(Guid id);
    Task<List<Leave>> GetByUserIdAsync(Guid userId);
    Task<List<Leave>> GetAllAsync();
    Task UpdateAsync(Leave leave);
    Task DeleteAsync(Guid id);
    Task<LeaveBalance?> GetLeaveBalanceAsync(Guid userId, LeaveType type);
    Task UpdateLeaveBalanceAsync(LeaveBalance balance);
    Task AddLeaveBalanceAsync(LeaveBalance balance);
    Task<List<LeaveBalance>> GetLeaveBalancesByUserAsync(Guid userId);
}
