using Refit;

namespace WebApi.Features.GetPositionsPortfolio.ExternalApi;

public interface IPositionsApi
{
    [Get("/port/v1/positions")]
    Task<SaxoResponse<IndividualPosition[]>> GetPositions(CancellationToken cancellationToken);
}