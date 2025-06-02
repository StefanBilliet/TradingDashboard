using Refit;

namespace Tests.PositionsApi;

public interface IPositionsApi
{
    [Get("/port/v1/positions")]
    Task<SaxoResponse<IndividualPosition[]>> GetPositions(CancellationToken cancellationToken);
}