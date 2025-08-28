using EMSLeaveManagementPortal.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMSLeaveManagementPortal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
    }
}
