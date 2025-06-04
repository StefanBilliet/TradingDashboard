namespace WebApi.Features.GetPositionsPortfolio.Data.ReadModels;

public record PositionsPortfolio(params Position[] Positions)
{
}