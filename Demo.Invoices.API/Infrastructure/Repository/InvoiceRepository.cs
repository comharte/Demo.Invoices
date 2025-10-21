
namespace Demo.Invoices.API.Infrastructure.Repository;
public interface IInvoiceRepository
{
    public Task<Invoice> CreateAsync(Invoice invoice);

    public Task<Invoice> GetByGlobalIdAsync(Guid globalId);
}

public class InvoiceRepository : IInvoiceRepository
{
    public Task<Invoice> CreateAsync(Invoice invoice)
    {
        throw new NotImplementedException();
    }

    public Task<Invoice> GetByGlobalIdAsync(Guid globalId)
    {
        throw new NotImplementedException();
    }
}