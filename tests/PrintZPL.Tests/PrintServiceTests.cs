using Microsoft.Extensions.Logging.Abstractions;
using PrintZPL.Core.Services;
using Xunit;

namespace PrintZPL.Tests;

public sealed class PrintServiceTests
{
    private readonly PrintService _service = new(NullLogger<PrintService>.Instance, new TemplateService(), new TestPrinterAddressPolicy());

    [Theory]
    [InlineData("printer-name", 9100)]
    [InlineData("192.168.1.5", 0)]
    [InlineData("192.168.1.5", 65536)]
    public async Task PrintZpl_RejectsInvalidDestination(string address, int port)
    {
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.PrintZPL("^XA^XZ", address, port, null, "$"));
    }

    [Fact]
    public async Task PrintZpl_RejectsOversizedExpandedTemplate()
    {
        var zpl = new string('A', 262145);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.PrintZPL(zpl, "127.0.0.1", 9100, null, "$"));
    }

    [Fact]
    public async Task PrintZpl_RejectsAddressesOutsideConfiguredAllowlist()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.PrintZPL("^XA^XZ", "192.168.1.5", 9100, null, "$"));
    }

    private sealed class TestPrinterAddressPolicy : IPrinterAddressPolicy
    {
        public bool IsAllowed(System.Net.IPAddress address) => address.Equals(System.Net.IPAddress.Loopback);
    }
}
