namespace PrintZPL.Core.Exceptions;
internal sealed class PrinterNotFoundException : Exception
{
    public PrinterNotFoundException(string ipAddress)
        : base($"No printer found on {ipAddress}")
    {
        IpAddress = ipAddress;
    }

    public string IpAddress { get; }
}
