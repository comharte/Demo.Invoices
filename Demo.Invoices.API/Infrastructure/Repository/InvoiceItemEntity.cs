namespace Demo.Invoices.API.Infrastructure.Repository;

public class InvoiceItemEntity
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public string ProductCode { get; set; } = null!;

    public decimal Amount { get; set; }
}