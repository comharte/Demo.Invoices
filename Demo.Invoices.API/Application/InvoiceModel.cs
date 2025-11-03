namespace Demo.Invoices.API.Application;

public class InvoiceModel
{
    public int Id { get; set; }

    public Guid GlobalId { get; set; }

    public string Description { get; set; } = null!;

    public Guid CustomerRef { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public DateTime TransactionDate { get; set; }

    public List<InvoiceItemModel> Items { get; set; } = [];
}


public class InvoiceItemModel
{
    public int Id { get; set; }

    public string ProductCode { get; set; } = null!;

    public decimal Value { get; set; }
}