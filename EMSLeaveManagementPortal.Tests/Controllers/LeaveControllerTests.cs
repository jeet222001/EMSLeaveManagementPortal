using EMSLeaveManagementPortal.Controllers;
using EMSLeaveManagementPortal.DTOs;
using EMSLeaveManagementPortal.Entities;
using EMSLeaveManagementPortal.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Moq; // Add this using directive at the top of the file
using Xunit; // Add this using directive at the top of the file
using Assert = Xunit.Assert;
using Microsoft.AspNetCore.Http; // Add this alias at the top of the file


public class LeaveControllerTests
{
    private readonly Mock<ILeaveRepository> _leaveRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();

    private LeaveController CreateControllerWithUser(Guid userId)
    {
        var controller = new LeaveController(_leaveRepoMock.Object, _userRepoMock.Object, _emailServiceMock.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "mock"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        return controller;
    }

    [Fact]
    public async Task ApplyLeave_ReturnsOk_WhenValid()
    {
        var userId = Guid.NewGuid();
        var dto = new ApplyLeaveDto
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(2),
            Reason = "Vacation",
            Type = LeaveType.CasualLeave
        };
        _leaveRepoMock.Setup(r => r.AddAsync(It.IsAny<Leave>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>
        {
            new User { Id = userId, Username = "testuser", Name = "Test User" }
        });
        _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var controller = CreateControllerWithUser(userId);
        var result = await controller.ApplyLeave(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponseDto<Leave>>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task ApproveRejectLeave_ReturnsNotFound_WhenLeaveMissing()
    {
        var dto = new ApproveRejectLeaveDto { LeaveId = Guid.NewGuid(), Approve = true };
        _leaveRepoMock.Setup(r => r.GetByIdAsync(dto.LeaveId)).ReturnsAsync((Leave)null);

        var controller = CreateControllerWithUser(Guid.NewGuid());
        var result = await controller.ApproveRejectLeave(dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAppliedLeavesByUserId_ReturnsNotFound_WhenNoLeaves()
    {
        var userId = Guid.NewGuid();
        _leaveRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new List<Leave>());

        var controller = CreateControllerWithUser(userId);
        var result = await controller.GetAppliedLeavesByUserId(userId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CancelLeave_ReturnsNotFound_WhenLeaveNotFound()
    {
        var userId = Guid.NewGuid();
        var leaveId = Guid.NewGuid();
        _leaveRepoMock.Setup(r => r.GetByIdAsync(leaveId)).ReturnsAsync((Leave)null);

        var controller = CreateControllerWithUser(userId);
        var result = await controller.CancelLeave(userId, leaveId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAllLeaves_ReturnsOk()
    {
        _leaveRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Leave>());
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

        var controller = CreateControllerWithUser(Guid.NewGuid());
        var result = await controller.GetAllLeaves(null, null, null, null);

        Assert.IsType<OkObjectResult>(result);
    }
}