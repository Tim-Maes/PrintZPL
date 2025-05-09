using Microsoft.AspNetCore.Mvc;
using PrintZPL.Core.Services;

namespace PrintZPL.Host.Controllers;

[ApiController]
[Route("printers")]
public class PrintersController : ControllerBase
{
    private readonly IPrinterDiscoveryService _discoveryService;

    public PrintersController(IPrinterDiscoveryService discoveryService)
    {
        _discoveryService = discoveryService;
    }

    /// <summary>
    /// GET /printers
    /// Returns list of printers discovered via mDNS.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPrinters(CancellationToken cancellationToken)
    {
        var printers = await _discoveryService.DiscoverAsync(cancellationToken);
        return Ok(printers);
    }
}
