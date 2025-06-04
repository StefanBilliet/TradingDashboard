using WebApi.Features.GetPositionsPortfolio.Data.ReadModels;
using WebApi.Features.GetPositionsPortfolio.ExternalApi;

namespace WebApi.Features.GetPositionsPortfolio.Data;

public class PositionsPortfolioDataService
{
    private readonly IPositionsApi _positionsApi;

    public PositionsPortfolioDataService(IPositionsApi positionsApi)
    {
        _positionsApi = positionsApi;
    }

    public async Task<PositionsPortfolio> Get(CancellationToken cancellationToken)
    {
        var positionsResponse = await _positionsApi.GetPositions(cancellationToken);

        return new PositionsPortfolio(positionsResponse.Data
            .Where(position => position.PositionBase.AssetType == AssetType.StockOption)
            .GroupBy(position => position.PositionBase.CorrelationKey)
            .Select(positionGroup => new Position(
                positionGroup.Select(position => new Leg(position.DisplayAndFormat.Symbol, position.PositionBase.OptionsData!.Strike,
                        position.PositionBase.Amount,
                        position.PositionBase.OpenPriceIncludingCosts, position.PositionBase.Status, position.PositionBase.CorrelationKey))
                    .ToArray()
            )).ToArray()
        );
    }
}