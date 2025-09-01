using EMSLeaveManagementPortal.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Assert = Xunit.Assert; // Add this alias at the top of the file

public class EmailServiceTests
{
    [Fact]
    public async Task SendEmailAsync_DoesNotThrow()
    {
        var configMock = new Mock<IConfiguration>();
        var smtpSectionMock = new Mock<IConfigurationSection>();
        smtpSectionMock.Setup(s => s["Smtp:From"]).Returns("jhrayththa@gmail.com");
        smtpSectionMock.Setup(s => s["Smtp:Host"]).Returns("smtp.gmail.com");
        smtpSectionMock.Setup(s => s["Smtp:Port"]).Returns("587");
        smtpSectionMock.Setup(s => s["Smtp:Username"]).Returns("Leave Management");
        smtpSectionMock.Setup(s => s["Smtp:Password"]).Returns("vomx rbvc dnck fcsq");
        configMock.Setup(c => c.GetSection("Smtp")).Returns(smtpSectionMock.Object);

        var service = new EmailService(configMock.Object);
        var exception = await Record.ExceptionAsync(() => service.SendEmailAsync("recipient@example.com", "Subject", "Body"));
        Assert.Null(exception);
    }
}