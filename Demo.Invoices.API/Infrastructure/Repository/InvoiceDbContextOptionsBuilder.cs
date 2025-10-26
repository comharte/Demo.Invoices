using Microsoft.EntityFrameworkCore;

namespace Demo.Invoices.API.Infrastructure.Repository;

public class InvoiceDbContextOptionsBuilder
{
    public static DbContextOptions<InvoiceDbContext> Create(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("InvoicesDatabase");

        var optionsBuilder = new DbContextOptionsBuilder<InvoiceDbContext>();

        optionsBuilder.UseSqlite(connectionString);
        
        return optionsBuilder.Options;
    }

}