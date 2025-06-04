using Vogen;

namespace Tests;

[ValueObject<string>]
public readonly partial struct AssetType 
{
    public static AssetType Stock => From("Stock");
    public static AssetType StockOption => From("StockOption");

    public static Validation Validate(string value) =>
        value != "Stock" && value != "StockOption" ? 
            Validation.Invalid("AssetType must be either Stock or StockOption") : 
            Validation.Ok;
    private static string NormalizeInput(string input)
    {
        return input;
    }
}