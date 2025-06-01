namespace Tests.PositionsApi;

public record PositionView
{
    public required double Bid { get; init; }
    public required double Ask { get; init; }
    public required double Mid { get; init; }
}