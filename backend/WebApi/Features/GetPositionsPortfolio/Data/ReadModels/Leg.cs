using WebApi.Features.GetPositionsPortfolio.ExternalApi;

namespace WebApi.Features.GetPositionsPortfolio.Data.ReadModels;

public record Leg(string Symbol, decimal Strike, double Amount, decimal OpenPriceIncludingCosts, PositionStatus Status, Guid CorrelationKey);