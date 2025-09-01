using EMSLeaveManagementPortal.Controllers;
using EMSLeaveManagementPortal.DTOs;
using EMSLeaveManagementPortal.Entities;
using EMSLeaveManagementPortal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Assert = Xunit.Assert; // Add this alias at the top of the file

public class UserControllerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ILogger<UserController>> _loggerMock = new();

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
        var controller = new UserController(_userRepoMock.Object, _loggerMock.Object);
        var result = await controller.GetAll();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenUserMissing()
    {
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
        var controller = new UserController(_userRepoMock.Object, _loggerMock.Object);
        var result = await controller.GetById(Guid.NewGuid());
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUsernameExists()
    {
        var dto = new SignUpDto { Username = "existing", Password = "pass", Name = "Name", Role = UserRole.Employee };
        _userRepoMock.Setup(r => r.GetByUsernameAsync(dto.Username)).ReturnsAsync(new User());
        var controller = new UserController(_userRepoMock.Object, _loggerMock.Object);
        var result = await controller.Create(dto);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenUserMissing()
    {
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
        var controller = new UserController(_userRepoMock.Object, _loggerMock.Object);
        var result = await controller.Update(Guid.NewGuid(), new UpdateUserDto());
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenUserMissing()
    {
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
        var controller = new UserController(_userRepoMock.Object, _loggerMock.Object);
        var result = await controller.Delete(Guid.NewGuid());
        Assert.IsType<NotFoundObjectResult>(result);
    }
}