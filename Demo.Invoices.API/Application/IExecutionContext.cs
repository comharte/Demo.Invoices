namespace Demo.Invoices.API.Application;

/// <summary>
/// Instead of passing around CancellationToken, RequestId, CorrelationId and adding them to method signatures,
/// We can expect hosting layer to register relevant object to DI container.
/// Similar interface could be defined in shared library or in each layer separately (application/infrastruture).
/// </summary>
public interface IExecutionContext
{
    public CancellationToken CancellationToken { get; }

    public string RequestId { get; }

    public string CorrelationId { get; }
}