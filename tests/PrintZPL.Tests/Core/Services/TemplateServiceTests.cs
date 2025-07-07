using PrintZPL.Core.Services;

namespace PrintZPL.Tests.Core.Services;

public class TemplateServiceTests
{
    private readonly TemplateService _templateService;

    public TemplateServiceTests()
    {
        _templateService = new TemplateService();
    }

    [Fact]
    public void PopulateZplTemplate_WithValidData_ReplacesPlaceholders()
    {
        // Arrange
        var template = "^XA^FO50,50^FD$Name$^FS^XZ";
        var data = new Dictionary<string, string> { { "Name", "TestPrinter" } };
        var delimiter = "$";
        var expected = "^XA^FO50,50^FDTestPrinter^FS^XZ";

        // Act
        var result = _templateService.PopulateZplTemplate(data, template, delimiter);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PopulateZplTemplate_WithMultipleData_ReplacesAllPlaceholders()
    {
        // Arrange
        var template = "^XA^FO50,50^FD$Greeting$, $Name$!^FS^XZ";
        var data = new Dictionary<string, string> 
        { 
            { "Greeting", "Hello" }, 
            { "Name", "World" } 
        };
        var delimiter = "$";
        var expected = "^XA^FO50,50^FDHello, World!^FS^XZ";

        // Act
        var result = _templateService.PopulateZplTemplate(data, template, delimiter);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PopulateZplTemplate_WithEmptyData_ReturnsOriginalTemplate()
    {
        // Arrange
        var template = "^XA^FO50,50^FD$Name$^FS^XZ";
        var data = new Dictionary<string, string>();
        var delimiter = "$";

        // Act
        var result = _templateService.PopulateZplTemplate(data, template, delimiter);

        // Assert
        Assert.Equal(template, result);
    }

    [Fact]
    public void PopulateZplTemplate_WithNullValues_ReplacesWithEmptyString()
    {
        // Arrange
        var template = "^XA^FO50,50^FD$Name$^FS^XZ";
        var data = new Dictionary<string, string> { { "Name", null! } };
        var delimiter = "$";
        var expected = "^XA^FO50,50^FD^FS^XZ";

        // Act
        var result = _templateService.PopulateZplTemplate(data, template, delimiter);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PopulateZplTemplate_WithDifferentDelimiters_ReplacesCorrectly()
    {
        // Arrange
        var template = "^XA^FO50,50^FD$$Name$$^FS^XZ";
        var data = new Dictionary<string, string> { { "Name", "TestValue" } };
        var delimiter = "$$";
        var expected = "^XA^FO50,50^FDTestValue^FS^XZ";

        // Act
        var result = _templateService.PopulateZplTemplate(data, template, delimiter);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PopulateZplTemplate_WithSpecialRegexCharacters_HandlesCorrectly()
    {
        // Arrange
        var template = "^XA^FO50,50^FD$$Name$$^FS^XZ";
        var data = new Dictionary<string, string> { { "Name", "TestValue" } };
        var delimiter = "$$";
        var expected = "^XA^FO50,50^FDTestValue^FS^XZ";

        // Act
        var result = _templateService.PopulateZplTemplate(data, template, delimiter);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PopulateZplTemplate_WithCaseInsensitiveMatch_ReplacesCorrectly()
    {
        // Arrange
        var template = "^XA^FO50,50^FD$name$^FS^XZ";
        var data = new Dictionary<string, string> { { "Name", "TestValue" } };
        var delimiter = "$";
        var expected = "^XA^FO50,50^FDTestValue^FS^XZ";

        // Act
        var result = _templateService.PopulateZplTemplate(data, template, delimiter);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PopulateZplTemplate_WithNoMatchingPlaceholders_ReturnsOriginalTemplate()
    {
        // Arrange
        var template = "^XA^FO50,50^FD$Name$^FS^XZ";
        var data = new Dictionary<string, string> { { "Age", "25" } };
        var delimiter = "$";

        // Act
        var result = _templateService.PopulateZplTemplate(data, template, delimiter);

        // Assert
        Assert.Equal(template, result);
    }

    [Fact]
    public void PopulateZplTemplate_WithDuplicatePlaceholders_ReplacesAll()
    {
        // Arrange
        var template = "^XA^FO50,50^FD$Name$^FS^FO50,100^FD$Name$^FS^XZ";
        var data = new Dictionary<string, string> { { "Name", "Test" } };
        var delimiter = "$";
        var expected = "^XA^FO50,50^FDTest^FS^FO50,100^FDTest^FS^XZ";

        // Act
        var result = _templateService.PopulateZplTemplate(data, template, delimiter);

        // Assert
        Assert.Equal(expected, result);
    }
}