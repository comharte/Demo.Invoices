namespace Demo.Invoices.API.Infrastructure.API;

public interface ICustomerApiClient
{
    public Task<List<CustomerInfo>> GetActiveCustomersAsync(CancellationToken cancellationToken);
}

public record CustomerInfo(Guid GlobalId, string Name, bool IsKey);


public class CustomerApiClient : ICustomerApiClient
{
    private readonly HashSet<CustomerInfo> _fakeCustomers = new()
    {
        new (Guid.Parse("a1b2c3d4-e5f6-4789-9012-abcdefabcdef"), "Acme Corporation", true),
        new (Guid.Parse("b2c3d4e5-f678-4901-2345-bcdefabcdefa"), "Globex Inc.", false),
        new (Guid.Parse("c3d4e5f6-7890-1234-5678-cdefabcdefab"), "Soylent Corp.", true),
        new (Guid.Parse("d4e5f678-9012-3456-7890-defabcdefabc"), "Initech", true),
    };

    public Task<List<CustomerInfo>> GetActiveCustomersAsync(CancellationToken cancellationToken)
        => Task.FromResult(_fakeCustomers.ToList());
}