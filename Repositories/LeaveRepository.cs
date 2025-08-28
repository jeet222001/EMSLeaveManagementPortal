using EMSLeaveManagementPortal.Data;
using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.Repositories;
public class LeaveRepository : ILeaveRepository
{
    private readonly AppDbContext _context;

    public LeaveRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Leave leave)
    {
        _context.Leaves.Add(leave);
        _context.SaveChanges();
    }

    public Leave GetById(Guid id)
    {
        return _context.Leaves.Find(id);
    }

    public IEnumerable<Leave> GetByUserId(Guid userId)
    {
        return _context.Leaves.Where(l => l.UserId == userId).ToList();
    }

    public IEnumerable<Leave> GetAll()
    {
        return _context.Leaves.ToList();
    }

    public void Update(Leave leave)
    {
        _context.Leaves.Update(leave);
        _context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var leave = _context.Leaves.Find(id);
        if (leave != null)
        {
            _context.Leaves.Remove(leave);
            _context.SaveChanges();
        }
    }
    // Example for EF Core
    public LeaveBalance GetLeaveBalance(Guid userId, LeaveType type)
    {
        return _context.LeaveBalances.FirstOrDefault(lb => lb.UserId == userId && lb.Type == type);
    }

    public void UpdateLeaveBalance(LeaveBalance balance)
    {
        _context.LeaveBalances.Update(balance);
        _context.SaveChanges();
    }

    public void AddLeaveBalance(LeaveBalance balance)
    {
        _context.LeaveBalances.Add(balance);
        _context.SaveChanges();
    }

    public IEnumerable<LeaveBalance> GetLeaveBalancesByUser(Guid userId)
    {
        return _context.LeaveBalances.Where(lb => lb.UserId == userId).ToList();
    }
}