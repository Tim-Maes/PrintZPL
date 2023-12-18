namespace PrintZPL.Host.Requests;

public sealed class PrintBatchFromZPLRequest
{
    public IEnumerable<PrintFromZPLRequest> PrintRequests { get; set; }

}
