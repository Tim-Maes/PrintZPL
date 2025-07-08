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
    public async Task PrintZPL_WithWhitespaceZpl_ThrowsArgumentNullException()
    {
        // Arrange
        string whitespaceZpl = "   ";
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _printService.PrintZPL(whitespaceZpl, "192.168.1.100", 9100, null, null));
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
            // Expected to fail due to no actual printer
        }

        // Assert
        _mockTemplateService.Verify(x => x.PopulateZplTemplate(data, zpl, delimiter), Times.Once);
    }

    [Fact]
    public async Task PrintZPL_WithNullData_DoesNotCallTemplateService()
    {
        // Arrange
        var zpl = "^XA^FO50,50^FDTest^FS^XZ";
        Dictionary<string, string>? nullData = null;

        // Act
        try
        {
            await _printService.PrintZPL(zpl, "192.168.1.100", 9100, nullData, "$");
        }
        catch (Exception)
        {
            // Expected to fail due to no actual printer
        }

        // Assert
        _mockTemplateService.Verify(x => x.PopulateZplTemplate(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PrintZPL_WithEmptyData_DoesNotCallTemplateService()
    {
        // Arrange
        var zpl = "^XA^FO50,50^FDTest^FS^XZ";
        var emptyData = new Dictionary<string, string>(); // Empty dictionary

        // Act
        try
        {
            await _printService.PrintZPL(zpl, "192.168.1.100", 9100, emptyData, "$");
        }
        catch (Exception)
        {
            // Expected to fail due to no actual printer
        }

        // Assert - Empty dictionary should NOT call template service since Count = 0
        _mockTemplateService.Verify(x => x.PopulateZplTemplate(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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

    [Fact]
    public async Task PrintZPL_WithZeroPort_ThrowsInvalidOperationException()
    {
        // Arrange
        var zpl = "^XA^FO50,50^FDTest^FS^XZ";
        var zeroPort = 0;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _printService.PrintZPL(zpl, "192.168.1.100", zeroPort, null, null));
        
        Assert.Contains("Failed to connect to printer", exception.Message);
    }

    [Theory]
    [InlineData("localhost")]
    [InlineData("127.0.0.1")]
    [InlineData("192.168.1.1")]
    [InlineData("10.0.0.1")]
    public async Task PrintZPL_WithValidIpFormats_AttemptsConnection(string ipAddress)
    {
        // Arrange
        var zpl = "^XA^FO50,50^FDTest^FS^XZ";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _printService.PrintZPL(zpl, ipAddress, 9100, null, null));
        
        Assert.Contains("Failed to connect to printer", exception.Message);
    }

    [Theory]
    [InlineData("^XA^FO50,50^FDTest^FS^XZ")]
    [InlineData("^XA^FO50,50^FDTest^FS^XZ\n")]
    [InlineData("^XA^FO50,50^FDTest^FS^XZ\r\n")]
    public async Task PrintZPL_WithVariousZplFormats_AttemptsConnection(string zpl)
    {
        // Arrange & Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _printService.PrintZPL(zpl, "192.168.1.100", 9100, null, null));
        
        Assert.Contains("Failed to connect to printer", exception.Message);
    }

    [Fact]
    public async Task PrintZPL_WithComplexZpl_AttemptsConnection()
    {
        // Arrange
        var complexZpl = @"^XA
^FO50,50^A0N,50,50^FDHello World!^FS
^FO50,120^A0N,30,30^FDLine 2^FS
^FO50,180^A0N,30,30^FDLine 3^FS
^XZ";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _printService.PrintZPL(complexZpl, "192.168.1.100", 9100, null, null));
        
        Assert.Contains("Failed to connect to printer", exception.Message);
    }

    [Fact]
    public async Task PrintZPL_WithTemplateAndData_ProcessesCorrectly()
    {
        // Arrange
        var zpl = "^XA^FO50,50^FD$Name$^FS^FO50,100^FD$Date$^FS^XZ";
        var data = new Dictionary<string, string> 
        { 
            { "Name", "John Doe" }, 
            { "Date", "2023-12-01" } 
        };
        var delimiter = "$";
        var processedZpl = "^XA^FO50,50^FDJohn Doe^FS^FO50,100^FD2023-12-01^FS^XZ";

        _mockTemplateService
            .Setup(x => x.PopulateZplTemplate(data, zpl, delimiter))
            .Returns(processedZpl);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _printService.PrintZPL(zpl, "192.168.1.100", 9100, data, delimiter));
        
        Assert.Contains("Failed to connect to printer", exception.Message);
        _mockTemplateService.Verify(x => x.PopulateZplTemplate(data, zpl, delimiter), Times.Once);
    }
}