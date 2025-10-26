using Microsoft.EntityFrameworkCore;

namespace Demo.Invoices.API.Infrastructure.Repository;

public interface IInvoiceRepository
{
    public Task<InvoiceEntity> ModifyAsync(InvoiceEntity invoice, CancellationToken cancellation);

    public Task<InvoiceEntity> GetByIdAsync(int id, CancellationToken cancellation);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellation);

    public Task Initialize(CancellationToken cancellation);
}

public class InvoiceRepository : IInvoiceRepository
{
    private readonly InvoiceDbContext _dbContext;

    public InvoiceRepository(InvoiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellation)
    {
        var invoice = await GetByIdAsync(id, cancellation);
        _dbContext.Invoices.Remove(invoice);
        await _dbContext.SaveChangesAsync(cancellation);
        return true;
    }

    public async Task<InvoiceEntity> GetByIdAsync(int id, CancellationToken cancellation)
    {
        return await _dbContext.Invoices.SingleAsync(inv => inv.Id.Equals(id), cancellation);
    }

    public async Task<InvoiceEntity> ModifyAsync(InvoiceEntity invoice, CancellationToken cancellation)
    {
        _dbContext.Invoices.Update(invoice);
        await _dbContext.SaveChangesAsync(cancellation);
        return invoice;
    }

    public async Task Initialize(CancellationToken cancellation)
    {
        await _dbContext.Database.EnsureCreatedAsync(cancellation);
    }
}