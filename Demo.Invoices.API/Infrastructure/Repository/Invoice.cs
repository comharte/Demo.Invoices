namespace Demo.Invoices.API.Infrastructure.Repository;

public class Invoice
{
    public int Id { get; set; }

    public Guid GlobalId { get; set; }

    public Guid CustomerRef { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }
}