namespace PrintZPL.Host.Requests;

public sealed class PrintBatchFromZPLRequest
{
    [System.ComponentModel.DataAnnotations.MaxLength(50)]
    public List<PrintFromZPLRequest>? PrintRequests { get; set; }

}
