using EMSLeaveManagementPortal.Entities;

namespace EMSLeaveManagementPortal.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
