using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.DTOs
{
    public class UpdateUserDto
    {
        public string? Username { get; set; }
        public string? Name { get; set; }
        public UserRole Role { get; set; }
        public string? Password { get; set; }
    }
}