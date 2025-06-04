namespace WebApi.Features.GetPositionsPortfolio.ExternalApi;

public record PositionBase
{
    public required string AccountId { get; init; }
    public required AssetType AssetType { get; init; }
    public required int Uic { get; init; }
    public required double Amount { get; init; }
    public required decimal OpenPrice { get; init; }
    public required PositionStatus Status { get; init; }
    public Guid CorrelationKey { get; init; }
    public string? AccountKey { get; init; }
    public bool CanBeClosed { get; init; }
    public string? ClientId { get; init; }
    public bool CloseConversionRateSettled { get; init; }
    public DateTime? ExecutionTimeOpen { get; init; }
    public bool IsForceOpen { get; init; }
    public bool IsMarketOpen { get; init; }
    public bool LockedByBackOffice { get; init; }
    public double OpenBondPoolFactor { get; init; }
    public decimal OpenPriceIncludingCosts { get; init; }
    public OptionsData? OptionsData { get; init; }
    public object[]? RelatedOpenOrders { get; init; }
    public string? SourceOrderId { get; init; }
    public DateTime? ValueDate { get; init; }
}

public record OptionsData
{
    public bool CanBeExercised { get; init; }
    public required string ExerciseStyle { get; init; }
    public required string ExpiryCut { get; init; }
    public required DateTime ExpiryDate { get; init; }
    public required string PutCall { get; init; }
    public required string SettlementStyle { get; init; }
    public required decimal Strike { get; init; }
}