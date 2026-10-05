using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PrintZPL.Core.Services;
using PrintZPL.Host.Requests;

namespace PrintZPL.Host.Controllers;

[ApiController]
[Route("batch-print")]
public class BatchPrintController : ControllerBase
{
    private readonly IPrintService _printerService;
    private readonly ILogger<BatchPrintController> _logger;

    public BatchPrintController(
        IPrintService printerService,
        ILogger<BatchPrintController> logger)
    {
        _printerService = printerService;
        _logger = logger;
    }

    [HttpPost]
    [Route("from-zpl")]
    public async Task<IActionResult> PrintZPL([FromBody] PrintBatchFromZPLRequest batchRequest, CancellationToken cancellationToken)
    {
        var results = new List<object>();
        var hasErrors = false;
        var successfulItems = 0;

        try
        {
            _logger.LogInformation("Received batch print request with {Count} items", batchRequest.PrintRequests?.Count ?? 0);

            if (batchRequest.PrintRequests == null || batchRequest.PrintRequests.Count == 0 || batchRequest.PrintRequests.Count > 50)
            {
                return BadRequest(new { success = false, message = "Provide between 1 and 50 print requests." });
            }

            foreach (var (request, index) in batchRequest.PrintRequests.Select((r, i) => (r, i)))
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    _logger.LogDebug("Processing batch item {Index} for {IpAddress}:{Port}", index, request.IpAddress, request.Port);
                    
                    await _printerService.PrintZPL(
                        zplString: request.ZPL,
                        printerIpAddress: request.IpAddress,
                        port: request.Port,
                        data: request.Data,
                        delimiter: request.Delimiter,
                        cancellationToken: cancellationToken);

                    successfulItems++;
                    results.Add(new { 
                        index = index,
                        success = true, 
                        message = "ZPL sent to printer successfully",
                        printer = $"{request.IpAddress}:{request.Port}"
                    });
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    _logger.LogError(ex, "Error processing batch item {Index}", index);
                    
                    results.Add(new { 
                        index = index,
                        success = false, 
                        message = "Print request failed.",
                        printer = $"{request.IpAddress}:{request.Port}"
                    });
                }
            }

            var response = new
            {
                success = !hasErrors,
                totalItems = results.Count,
                successfulItems,
                failedItems = results.Count - successfulItems,
                results = results
            };

            return hasErrors ? StatusCode(207, response) : Ok(response); // 207 Multi-Status for partial success
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during batch printing");
            return StatusCode(500, new { success = false, message = "Internal server error." });
        }
    }
}
