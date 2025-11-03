namespace Demo.Invoices.API.Application;

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
        var entity = MapEntity(invoiceModel);

        await _invoiceRepository.ModifyAsync(entity, cancellationToken);

        return MapModel(entity);
    }

    public async Task<InvoiceModel> GetInvoiceAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _invoiceRepository.GetByIdAsync(id, cancellationToken);

        return MapModel(entity);
    }

    public async Task<List<InvoiceModel>> GetInvoicesAsync(CancellationToken cancellationToken)
    {
        var entities = await _invoiceRepository.GetByAllAsync(cancellationToken);
        return entities.Select(MapModel).ToList();
    }

    /// <summary>
    /// This is example of none-CRUD operation.
    /// In this case entity framework will load whole entity and start tracking it.
    /// As we update Description field entity framework change tracker will detect it and on SaveChanges() call it will collect all changed properties 
    /// and generate update statement with only changed columns.
    /// </summary>
    /// <remarks>
    /// There is also option to update fileds without loading entity:
    /// Please, read more about ExecuteUpdateAsync (_dbContext.Invoices.ExecuteUpdateAsync) for performance optimization.
    /// </remarks>
    public async Task<InvoiceModel> ModifyInvoiceDescriptionAsync(int id, string description, CancellationToken cancellationToken)
    {
        var entity = await _invoiceRepository.GetByIdAsync(id, cancellationToken);

        entity.Description = description;

        await _invoiceRepository.SaveChanges(cancellationToken);

        return MapModel(entity);
    }

    private static InvoiceModel MapModel(InvoiceEntity entity)
        => new()
        {
            Id = entity.Id,
            GlobalId = entity.GlobalId,
            Description = entity.Description,
            CustomerRef = entity.CustomerRef,
            CurrencyCode = entity.CurrencyCode,
            ExchangeRate = entity.ExchangeRate,
            TransactionDate = entity.TransactionDate,
            Items = entity.Items.Select(ei => new InvoiceItemModel
            {
                Id = ei.Id,
                ProductCode = ei.ProductCode,
                Value = ei.Value,
            }).ToList()
        };

    private static InvoiceEntity MapEntity(InvoiceModel model)
        => new()
        {
            Id = model.Id,
            GlobalId = model.GlobalId,
            Description = model.Description,
            CustomerRef = model.CustomerRef,
            CurrencyCode = model.CurrencyCode,
            ExchangeRate = model.ExchangeRate,
            TransactionDate = model.TransactionDate,
            Items = model.Items.Select(mi => new InvoiceItemEntity
            {
                Id = mi.Id,
                InvoiceId = model.Id,
                ProductCode = mi.ProductCode,
                Value = mi.Value,
            }).ToList()
        };

    /// <summary>
    /// This method is expected to return full updated model so in order to add item the best way is to load whole invoice entity, add item to its collection and save changes.
    /// In case we wouldn't have to return full model we could create InvoiceItemEntity instance seperatly and expect repository to save it.
    /// </summary>
    public async Task<InvoiceModel> AddInvoiceItem(int invoiceId, string productCode, decimal value, CancellationToken cancellationToken)
    {
        var entity = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);

        entity.Items.Add(new InvoiceItemEntity()
        {
            ProductCode = productCode,
            Value = value,
        });

        await _invoiceRepository.SaveChanges(cancellationToken);

        return MapModel(entity);
    }

    /// <summary>
    /// This method is expected to return full updated model so in order to add item the best way is to load whole invoice entity, add item to its collection and save changes.
    /// In case we wouldn't have to return full model we could define repository method to delete item by its id without loading whole invoice entity.
    /// </summary>
    public async Task<InvoiceModel> RemoveInvoiceItem(int itemId, CancellationToken cancellationToken)
    {
        var entity = await _invoiceRepository.GetByItemIdAsync(itemId, cancellationToken);

        var itemToRemove = entity.Items.FirstOrDefault(i => i.Id == itemId);

        if (itemToRemove != null)
        {
            entity.Items.Remove(itemToRemove);
            await _invoiceRepository.SaveChanges(cancellationToken);
        }

        return MapModel(entity);
    }
}