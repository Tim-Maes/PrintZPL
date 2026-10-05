using PrintZPL.Core.Services;
using Xunit;

namespace PrintZPL.Tests;

public sealed class TemplateServiceTests
{
    [Fact]
    public void PopulateZplTemplate_ReplacesValuesLiterallyAndIgnoresKeyCase()
    {
        var service = new TemplateService();

        var result = service.PopulateZplTemplate(
            new Dictionary<string, string> { ["Name"] = "$1\\box" },
            "^FD$name$^FS",
            "$");

        Assert.Equal("^FD$1\\box^FS", result);
    }

    [Fact]
    public void PopulateZplTemplate_RejectsEmptyDelimiter()
    {
        var service = new TemplateService();

        Assert.Throws<ArgumentException>(() => service.PopulateZplTemplate([], "^XA^XZ", ""));
    }

}
