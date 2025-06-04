namespace Tests.PositionsApi;

public record PositionView
{
    public double? Bid { get; init; }
    public double? Ask { get; init; }
    public double? Mid { get; init; }
    public string? CalculationReliability { get; init; }
}