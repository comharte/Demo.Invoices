using Microsoft.EntityFrameworkCore;

namespace Demo.Invoices.API.Infrastructure.Repository;

public interface IInvoiceRepository
{
    public Task<InvoiceEntity> ModifyAsync(InvoiceEntity invoice, CancellationToken cancellation);

    public Task<InvoiceEntity> GetByIdAsync(int id, CancellationToken cancellation);

    public Task<InvoiceEntity> GetByItemIdAsync(int itemId, CancellationToken cancellation);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellation);

    public Task Initialize(CancellationToken cancellation);

    public Task SaveChanges(CancellationToken cancellation);

    public Task<List<InvoiceEntity>> GetByAllAsync(CancellationToken cancellation);
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
        // If we already track entity it's easier to call Remove method.
        //var invoice = await GetByIdAsync(id, cancellation);
        //_dbContext.Invoices.Remove(invoice);
        //await _dbContext.SaveChangesAsync(cancellation);

        // Otherwise we can use ExecuteDeleteAsync for better performance. This will send single DELETE statement to database without loading entity first.
        var rowCount = await _dbContext.Invoices.Where(i => i.Id.Equals(id)).ExecuteDeleteAsync(cancellation);

        return rowCount > 0;
    }

    public async Task<InvoiceEntity> GetByIdAsync(int id, CancellationToken cancellation)
    {
        return await _dbContext.Invoices.Include(i => i.Items).SingleAsync(inv => inv.Id.Equals(id), cancellation);
    }

    public async Task<List<InvoiceEntity>> GetByAllAsync(CancellationToken cancellation)
    {
        return await _dbContext.Invoices.Include(i => i.Items).ToListAsync(cancellation);
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

    public async Task SaveChanges(CancellationToken cancellation)
    {
        await _dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<InvoiceEntity> GetByItemIdAsync(int itemId, CancellationToken cancellation)
    {
        return await _dbContext.Invoices.Include(i => i.Items).SingleAsync(inv => inv.Items.Any(it => it.Id.Equals(itemId)), cancellation);
    }
}