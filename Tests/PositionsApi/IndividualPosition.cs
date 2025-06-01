namespace Tests.PositionsApi;

public record IndividualPosition
{
    public required string NetPositionId { get; init; }
    public required PositionBase PositionBase { get; init; }
    public required string PositionId { get; init; }
    public required PositionView PositionView { get; init; }
}