using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.DTOs
{
    public class SignUpDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
