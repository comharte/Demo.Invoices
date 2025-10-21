namespace Demo.Invoices.API.Infrastructure.API;

public interface ICurrencyApiClient
{
    public int ExecutionCount { get; }
 
    Task<Dictionary<string, decimal>> GetEuroExchangeRateAsync(CancellationToken cancellationToken);
}


public class CurrencyApiClient : ICurrencyApiClient
{
    private sealed record CurrencyGeneratorInfo(string Currency, decimal MinRate, decimal MaxRate);

    private static readonly List<CurrencyGeneratorInfo> CurrencyInfos =
    [
        new CurrencyGeneratorInfo("USD", 1.0m, 1.2m),
        new CurrencyGeneratorInfo("GBP", 0.8m, 0.9m),
        new CurrencyGeneratorInfo("JPY", 120.0m, 140.0m)
    ];

    public int ExecutionCount { get; set; } = 0;

    public async Task<Dictionary<string, decimal>> GetEuroExchangeRateAsync(CancellationToken cancellationToken)
    {
        ExecutionCount++;

        var random = new Random();
        var rates = CurrencyInfos.ToDictionary(
            info => info.Currency,
            info => Math.Round((decimal)(random.NextDouble() * (double)(info.MaxRate - info.MinRate) + (double)info.MinRate), 4)
        );

        return await Task.FromResult(rates);
    }
}