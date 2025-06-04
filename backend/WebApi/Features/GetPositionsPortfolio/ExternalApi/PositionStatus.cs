using Vogen;

namespace WebApi.Features.GetPositionsPortfolio.ExternalApi;

[ValueObject<string>]
public readonly partial struct PositionStatus 
{
    public static PositionStatus Open => From("Open");
    public static PositionStatus Closed => From("Closed");

    public static Validation Validate(string value) =>
        value != "Open" && value != "Closed" ? 
            Validation.Invalid("Status must be either Open or Closed") : 
            Validation.Ok;
    private static string NormalizeInput(string input)
    {
        return input;
    }
}