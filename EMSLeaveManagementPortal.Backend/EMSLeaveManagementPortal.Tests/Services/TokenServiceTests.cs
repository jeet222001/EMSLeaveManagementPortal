using EMSLeaveManagementPortal.Entities;
using EMSLeaveManagementPortal.Services;
using Microsoft.Extensions.Configuration;
using Xunit;
using Assert = Xunit.Assert; // Add this alias at the top of the file

public class TokenServiceTests
{
    [Fact]
    public void CreateToken_ReturnsString()
    {
        var configMock = new Moq.Mock<IConfiguration>();
        // Use a key with at least 256 bits (32 ASCII characters for HS256)
        configMock.Setup(c => c["Jwt:Key"]).Returns("A1b2C3d4E5f6G7h8I9j0K1l2M3n4O5p6");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");
        var service = new TokenService(configMock.Object);
        var user = new User { Id = Guid.NewGuid(), Username = "user", Name = "Name", Role = UserRole.Employee };
        var token = service.CreateToken(user);
        Assert.False(string.IsNullOrWhiteSpace(token));
    }
}