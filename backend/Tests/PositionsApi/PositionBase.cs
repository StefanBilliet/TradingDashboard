namespace Tests.PositionsApi;

public record PositionBase
{
    public required string AccountId { get; init; }
    public required string AssetType { get; init; }
    public required int Uic { get; init; }
    public required string OptionType { get; init; }
    public required double Strike { get; init; }
    public required DateTime ExpiryDate { get; init; }
    public required int Amount { get; init; }
    public required double OpenPrice { get; init; }
    public required string Status { get; init; }
}