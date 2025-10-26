namespace Demo.Invoices.API.Infrastructure.Repository;

public interface IInvoiceRepository
{
    public Task<InvoiceEntity> ModifyAsync(InvoiceEntity invoice, CancellationToken cancellation);

    public Task<InvoiceEntity> GetByGlobalIdAsync(int id, CancellationToken cancellation);

    public Task<bool> DeleteAsync(Guid globalId, CancellationToken cancellation);
}

public class InvoiceRepository : IInvoiceRepository
{
    public Task<bool> DeleteAsync(Guid globalId, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    public Task<InvoiceEntity> GetByGlobalIdAsync(int id, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    public Task<InvoiceEntity> ModifyAsync(InvoiceEntity invoice, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}