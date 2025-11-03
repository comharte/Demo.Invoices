namespace Demo.Invoices.API.Application;

public interface IInvoiceService
{
    public Task<List<string>> GetAvailableCurrenciesAsync(CancellationToken cancellationToken);

    public Task<Dictionary<Guid, string>> GetAvailableCustomersAsync(CancellationToken cancellationToken);

    public Task DeleteInvoiceAsync(int id, CancellationToken cancellationToken);

    public Task<InvoiceModel> ModifyInvoiceAsync(InvoiceModel invoiceModel, CancellationToken cancellationToken);

    public Task<InvoiceModel> ModifyInvoiceDescriptionAsync(int id, string description, CancellationToken cancellationToken);

    public Task<InvoiceModel> GetInvoiceAsync(int id, CancellationToken cancellationToken);

    public Task<List<InvoiceModel>> GetInvoicesAsync(CancellationToken cancellationToken);

    public Task<InvoiceModel> AddInvoiceItem(int id, string productCode, decimal value, CancellationToken cancellationToken);

    public Task<InvoiceModel> RemoveInvoiceItem(int itemId, CancellationToken cancellationToken);
}
