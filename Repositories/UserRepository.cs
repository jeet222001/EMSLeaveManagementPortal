using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public User GetByUsername(string username) => _users.FirstOrDefault(u => u.Username == username);

        public void Add(User user) => _users.Add(user);

        public IEnumerable<User> GetAll() => _users;
    }
}
