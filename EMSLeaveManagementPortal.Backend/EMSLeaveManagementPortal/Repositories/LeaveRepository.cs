using EMSLeaveManagementPortal.Data;
using EMSLeaveManagementPortal.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMSLeaveManagementPortal.Repositories;
public class LeaveRepository : ILeaveRepository
{
    private readonly AppDbContext _context;

    public LeaveRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Leave leave)
    {
        await _context.Leaves.AddAsync(leave);
        await _context.SaveChangesAsync();
    }

    public async Task<Leave?> GetByIdAsync(Guid id)
    {
        return await _context.Leaves.FindAsync(id);
    }

    public async Task<List<Leave>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Leaves.Where(l => l.UserId == userId).ToListAsync();
    }

    public async Task<List<Leave>> GetAllAsync()
    {
        return await _context.Leaves.ToListAsync();
    }

    public async Task UpdateAsync(Leave leave)
    {
        _context.Leaves.Update(leave);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var leave = await _context.Leaves.FindAsync(id);
        if (leave != null)
        {
            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<LeaveBalance?> GetLeaveBalanceAsync(Guid userId, LeaveType type)
    {
        return await _context.LeaveBalances.FirstOrDefaultAsync(lb => lb.UserId == userId && lb.Type == type);
    }

    public async Task UpdateLeaveBalanceAsync(LeaveBalance balance)
    {
        _context.LeaveBalances.Update(balance);
        await _context.SaveChangesAsync();
    }

    public async Task AddLeaveBalanceAsync(LeaveBalance balance)
    {
        await _context.LeaveBalances.AddAsync(balance);
        await _context.SaveChangesAsync();
    }

    public async Task<List<LeaveBalance>> GetLeaveBalancesByUserAsync(Guid userId)
    {
        return await _context.LeaveBalances.Where(lb => lb.UserId == userId).ToListAsync();
    }
}
