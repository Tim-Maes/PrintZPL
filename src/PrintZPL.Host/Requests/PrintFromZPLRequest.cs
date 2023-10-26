using System.ComponentModel.DataAnnotations;

namespace PrintZPL.Host.Requests;

public sealed class PrintFromZPLRequest
{
    [Required]
    public string ZPL { get; set; }

    [Required]
    public string IpAddress { get; set; }

    public int Port { get; set; } = 6101; // Default port
}