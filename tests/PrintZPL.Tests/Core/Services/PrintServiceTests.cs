using Microsoft.Extensions.Logging;
using Moq;
using PrintZPL.Core.Services;

namespace PrintZPL.Tests.Core.Services;

public class PrintServiceTests
{
    private readonly Mock<ILogger<PrintService>> _mockLogger;
    private readonly Mock<ITemplateService> _mockTemplateService;
    private readonly PrintService _printService;

    public PrintServiceTests()
    {
        _mockLogger = new Mock<ILogger<PrintService>>();
        _mockTemplateService = new Mock<ITemplateService>();
        _printService = new PrintService(_mockLogger.Object, _mockTemplateService.Object);
    }

    [Fact]
    public async Task PrintZPL_WithNullZpl_ThrowsArgumentNullException()
    {
        // Arrange
        string nullZpl = null!;
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _printService.PrintZPL(nullZpl, "192.168.1.100", 9100, null, null));
    }

    [Fact]
    public async Task PrintZPL_WithEmptyZpl_ThrowsArgumentNullException()
    {
        // Arrange
        string emptyZpl = "";
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _printService.PrintZPL(emptyZpl, "192.168.1.100", 9100, null, null));
    }

    [Fact]
    public async Task PrintZPL_WithData_CallsTemplateService()
    {
        // Arrange
        var zpl = "^XA^FO50,50^FD$Name$^FS^XZ";
        var data = new Dictionary<string, string> { { "Name", "Test" } };
        var delimiter = "$";
        var expectedZpl = "^XA^FO50,50^FDTest^FS^XZ";
        
        _mockTemplateService
            .Setup(x => x.PopulateZplTemplate(data, zpl, delimiter))
            .Returns(expectedZpl);

        // Act
        try
        {
            await _printService.PrintZPL(zpl, "192.168.1.100", 9100, data, delimiter);
        }
        catch (Exception)
        {
        }

        // Assert
        _mockTemplateService.Verify(x => x.PopulateZplTemplate(data, zpl, delimiter), Times.Once);
    }

    [Fact]
    public async Task PrintZPL_WithInvalidIpAddress_ThrowsInvalidOperationException()
    {
        // Arrange
        var zpl = "^XA^FO50,50^FDTest^FS^XZ";
        var invalidIp = "999.999.999.999";

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _printService.PrintZPL(zpl, invalidIp, 9100, null, null));
        
        // Assert
        Assert.Contains("Failed to connect to printer", exception.Message);
    }

    [Fact]
    public async Task PrintZPL_WithUnreachableIpAddress_ThrowsInvalidOperationException()
    {
        // Arrange
        var zpl = "^XA^FO50,50^FDTest^FS^XZ";
        var unreachableIp = "192.168.255.255"; // Typically unreachable

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _printService.PrintZPL(zpl, unreachableIp, 9100, null, null));
        
        // Assert
        Assert.Contains("Failed to connect to printer", exception.Message);
    }
}