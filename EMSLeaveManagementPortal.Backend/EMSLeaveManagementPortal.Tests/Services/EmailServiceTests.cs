using EMSLeaveManagementPortal.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Assert = Xunit.Assert; // Add this alias at the top of the file

public class EmailServiceTests
{
    [Fact]
    public void Constructor_AssignsConfig()
    {
        var configMock = new Mock<IConfiguration>();
        var service = new EmailService(configMock.Object);
        Assert.NotNull(service);
    }

    [Fact]
    public async Task SendEmailAsync_DoesNotThrow()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Smtp:Host"]).Returns("smtp.gmail.com");
        configMock.Setup(c => c["Smtp:Port"]).Returns("587");
        configMock.Setup(c => c["Smtp:From"]).Returns("jhrayththa@gmail.com");
        configMock.Setup(c => c["Smtp:Password"]).Returns("vomx rbvc dnck fcsq");

        var service = new EmailService(configMock.Object);

        var exception = await Record.ExceptionAsync(() => service.SendEmailAsync("recipient@example.com", "Subject", "Body"));
        Assert.Null(exception);
    }

    [Fact]
    public async Task SendEmailAsync_Throws_WhenConfigMissing()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Smtp:Host"]).Returns((string)null);
        configMock.Setup(c => c["Smtp:Port"]).Returns((string)null);
        configMock.Setup(c => c["Smtp:From"]).Returns((string)null);
        configMock.Setup(c => c["Smtp:Password"]).Returns((string)null);

        var service = new EmailService(configMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendEmailAsync("recipient@example.com", "Subject", "Body"));
    }

    [Fact]
    public async Task SendEmailAsync_Throws_WhenInvalidEmail()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Smtp:Host"]).Returns("smtp.gmail.com");
        configMock.Setup(c => c["Smtp:Port"]).Returns("587");
        configMock.Setup(c => c["Smtp:From"]).Returns("jhrayththa@gmail.com");
        configMock.Setup(c => c["Smtp:Password"]).Returns("vomx rbvc dnck fcsq");

        var service = new EmailService(configMock.Object);

        await Assert.ThrowsAsync<FormatException>(() =>
            service.SendEmailAsync("invalid-email", "Subject", "Body"));
    }
}