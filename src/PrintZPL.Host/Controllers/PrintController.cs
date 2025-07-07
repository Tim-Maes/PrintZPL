using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PrintZPL.Core.Services;
using PrintZPL.Host.Requests;

namespace PrintZPL.Host.Controllers;

[ApiController]
[Route("print")]
public class PrintController : ControllerBase
{
    private readonly IPrintService _printerService;
    private readonly ILogger<PrintController> _logger;

    public PrintController(
        IPrintService printerService,
        ILogger<PrintController> logger)
    {
        _printerService = printerService;
        _logger = logger;
    }

    [HttpPost]
    [Route("from-zpl")]
    public async Task<IActionResult> PrintZPL([FromBody] PrintFromZPLRequest request)
    {
        try
        {
            _logger.LogInformation("Received print request for {IpAddress}:{Port}", request.IpAddress, request.Port);
            
            await _printerService.PrintZPL(
                zplString: request.ZPL,
                printerIpAddress: request.IpAddress,
                port: request.Port,
                data: request.Data,
                delimiter: request.Delimiter);

            return Ok(new { success = true, message = "ZPL sent to printer successfully" });
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogWarning("Invalid request: {Message}", ex.Message);
            return BadRequest(new { success = false, message = "Invalid ZPL data provided" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Printer connection failed: {Message}", ex.Message);
            return StatusCode(502, new { success = false, message = "Failed to connect to printer", details = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while printing ZPL: {Message}", ex.Message);
            return StatusCode(500, new { success = false, message = "Internal server error", details = ex.Message });
        }
    }
}