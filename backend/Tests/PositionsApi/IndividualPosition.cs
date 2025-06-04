namespace Tests.PositionsApi;

public record IndividualPosition
{
    public required string NetPositionId { get; init; }
    public required PositionBase PositionBase { get; init; }
    public required string PositionId { get; init; }
    public required DisplayAndFormat DisplayAndFormat { get; init; }
}

public record DisplayAndFormat
{
    public string Description { get; init; }
    public string Symbol { get; init; }
    public string UnderlyingInstrumentDescription { get; init; }
}