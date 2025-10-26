namespace Demo.Invoices.API.Infrastructure.Repository;

public class InvoiceEntity
{
    public int Id { get; set; }

    public Guid GlobalId { get; set; }

    public string Description { get; set; } = null!;
    
    public Guid CustomerRef { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public DateTime TransactionDate { get; set; }
}