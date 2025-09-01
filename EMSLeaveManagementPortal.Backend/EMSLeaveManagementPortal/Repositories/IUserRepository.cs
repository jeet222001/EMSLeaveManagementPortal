using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<List<User>> GetAllAsync();
    Task DeleteUserAsync(User user);
    Task UpdateUserAsync(User user);
}
