﻿using Microsoft.AspNetCore.Mvc;
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
            await _printerService.PrintZPL(
                zplString: request.ZPL,
                printerIpAddress: request.IpAddress,
                port: request.Port,
                data: request.Data,
                delimiter: request.Delimiter);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while printing ZPL: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }
}