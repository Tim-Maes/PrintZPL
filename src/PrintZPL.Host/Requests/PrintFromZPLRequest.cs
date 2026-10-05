using System.ComponentModel.DataAnnotations;

namespace PrintZPL.Host.Requests;

public sealed class PrintFromZPLRequest
{
    [Required]
    [StringLength(262144, MinimumLength = 1)]
    public string ZPL { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(?:\d{1,3}\.){3}\d{1,3}$|^[0-9a-fA-F:]+$")]
    public string IpAddress { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 6101;

    public Dictionary<string, string>? Data { get; set; }

    public string Delimiter { get; set; } = "$";
}
