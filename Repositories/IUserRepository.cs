using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.Repositories;

public interface IUserRepository
{
    User GetByUsername(string username);
    void Add(User user);
    IEnumerable<User> GetAll();
}
