namespace WebApi.Features.GetPositionsPortfolio.Data.ReadModels;

public record Position(params Leg[] Legs);