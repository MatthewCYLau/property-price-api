namespace property_price_purchase_service;

public class ProcessOrderResult
{
    private ProcessOrderResult(
           ProcessOrderResultType type,
           string message)
    {
        Type = type;
        Message = message;
    }

    public ProcessOrderResultType Type { get; }

    public string? Message { get; }

    public static ProcessOrderResult NotProcessable() =>
      new(ProcessOrderResultType.NotProcessable, "Not processable");

    public static ProcessOrderResult Success() =>
      new(ProcessOrderResultType.Success, "Success");
}
