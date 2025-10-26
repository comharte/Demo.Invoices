namespace Demo.Invoices.API.Application;

public interface IInvoiceService
{
    public Task<List<string>> GetAvailableCurrenciesAsync(CancellationToken cancellationToken);

    public Task<Dictionary<Guid, string>> GetAvailableCustomersAsync(CancellationToken cancellationToken);

    public Task DeleteInvoiceAsync(int id, CancellationToken cancellationToken);

    public Task<InvoiceModel> ModifyInvoiceAsync(InvoiceModel invoiceModel, CancellationToken cancellationToken);

    public Task<InvoiceModel> GetInvoiceAsync(int id, CancellationToken cancellationToken);
}

public class InvoiceService : IInvoiceService
{
    private readonly ICurrencyApiClient _currencyApiClient;

    private readonly ICustomerApiClient _customerApiClient;

    private readonly IInvoiceRepository _invoiceRepository;

    private readonly InvoiceServiceConfiguration _invoiceServiceConfiguration;

    public InvoiceService(ICurrencyApiClient currencyApiClient, ICustomerApiClient customerApiClient, IInvoiceRepository invoiceRepository, InvoiceServiceConfiguration invoiceServiceConfiguration)
    {
        _currencyApiClient = currencyApiClient;
        _customerApiClient = customerApiClient;
        _invoiceRepository = invoiceRepository;
        _invoiceServiceConfiguration = invoiceServiceConfiguration;
    }

    public async Task<InvoiceModel> ModifyInvoiceAsync(InvoiceModel invoiceModel, CancellationToken cancellationToken)
    {
        await ValidateModelAsync(invoiceModel, cancellationToken);

        return await SaveModel(invoiceModel, cancellationToken);
    }

    public async Task DeleteInvoiceAsync(int id, CancellationToken cancellationToken)
    {
        await _invoiceRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<List<string>> GetAvailableCurrenciesAsync(CancellationToken cancellationToken)
    {
        var currenciesExchangeRates = await _currencyApiClient.GetEuroExchangeRateAsync(cancellationToken);

        var availableCurrencies = currenciesExchangeRates.Keys.Where(key => _invoiceServiceConfiguration.AllowedCurrencies.Contains(key));

        return availableCurrencies.ToList();
    }

    public async Task<Dictionary<Guid, string>> GetAvailableCustomersAsync(CancellationToken cancellationToken)
    {
        var customersInfo = await _customerApiClient.GetActiveCustomersAsync(cancellationToken);

        return customersInfo.ToDictionary(c => c.GlobalId, c => c.Name);
    }

    private async Task ValidateModelAsync(InvoiceModel invoiceModel, CancellationToken cancellationToken)
    {
        var customers = await GetAvailableCustomersAsync(cancellationToken);

        if (!customers.Any(c => c.Key.Equals(invoiceModel.CustomerRef)))
        {
            throw new ApplicationException($"Invalid customer id.");
        }

        var currenciesExchangeRates = await _currencyApiClient.GetEuroExchangeRateAsync(cancellationToken);

        var availableCurrencies = currenciesExchangeRates.Keys.Where(key => _invoiceServiceConfiguration.AllowedCurrencies.Contains(key));

        if (!availableCurrencies.Contains(invoiceModel.CurrencyCode))
        {
            throw new ApplicationException($"Invalid currency.");
        }

        if (invoiceModel.ExchangeRate == 0 && !invoiceModel.CurrencyCode.Equals("EUR"))
        {
            invoiceModel.ExchangeRate = currenciesExchangeRates[invoiceModel.CurrencyCode];
        }
    }

    private async Task<InvoiceModel> SaveModel(InvoiceModel invoiceModel, CancellationToken cancellationToken)
    {
        var entity = await _invoiceRepository.ModifyAsync(new InvoiceEntity
        {
            Id = invoiceModel.Id,
            GlobalId = invoiceModel.GlobalId,
            Description = invoiceModel.Description,
            CustomerRef = invoiceModel.CustomerRef,
            CurrencyCode = invoiceModel.CurrencyCode,
            ExchangeRate = invoiceModel.ExchangeRate,
            TransactionDate = invoiceModel.TransactionDate
        }, cancellationToken);

        invoiceModel.Id = entity.Id;

        return invoiceModel;
    }

    public async Task<InvoiceModel> GetInvoiceAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _invoiceRepository.GetByIdAsync(id, cancellationToken);

        return new InvoiceModel()
        {
            Id = entity.Id,
            GlobalId = entity.GlobalId,
            Description = entity.Description,
            CustomerRef = entity.CustomerRef,
            CurrencyCode = entity.CurrencyCode,
            ExchangeRate = entity.ExchangeRate,
            TransactionDate = entity.TransactionDate
        };
    }
}