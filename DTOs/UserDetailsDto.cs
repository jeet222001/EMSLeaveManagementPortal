using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.DTOs;

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
}