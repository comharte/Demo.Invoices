namespace Demo.Invoices.API.Application;

public interface IInvoiceService
{
    public Task<List<string>> GetAvailableCurrenciesAsync(CancellationToken cancellationToken);

    public Task<Dictionary<Guid, string>> GetAvailableCustomersAsync(CancellationToken cancellationToken);
}

public class InvoiceService : IInvoiceService
{
    private readonly ICurrencyApiClient _currencyApiClient;

    private readonly ICustomerApiClient _customerApiClient;

    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(ICurrencyApiClient currencyApiClient, ICustomerApiClient customerApiClient, IInvoiceRepository invoiceRepository)
    {
        _currencyApiClient = currencyApiClient;
        _customerApiClient = customerApiClient;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<List<string>> GetAvailableCurrenciesAsync(CancellationToken cancellationToken)
    {
        var currenciesExchangeRates = await _currencyApiClient.GetEuroExchangeRateAsync(cancellationToken);
        return currenciesExchangeRates.Keys.ToList();
    }

    public async Task<Dictionary<Guid, string>> GetAvailableCustomersAsync(CancellationToken cancellationToken)
    {
        var customersInfo = await _customerApiClient.GetCustomersAsync(cancellationToken);
        return customersInfo.ToDictionary(c => c.GlobalId, c => c.Name);
    }
}