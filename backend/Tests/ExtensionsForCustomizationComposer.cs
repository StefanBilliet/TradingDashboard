using AutoFixture;
using AutoFixture.Dsl;
using Tests.PositionsApi;

namespace Tests;

public static class ExtensionsForCustomizationComposer
{
    private static readonly IFixture Fixture = new Fixture();
    
    public static IPostprocessComposer<IndividualPosition> AsShortCall(this ICustomizationComposer<IndividualPosition> composer, Guid? correlationKey = null)
    {
        return composer
            .With(
                position => position.PositionBase, Fixture.Build<PositionBase>()
                    .With(@base => @base.CorrelationKey, correlationKey ?? Guid.NewGuid())
                    .With(@base => @base.AssetType, AssetType.StockOption)
                    .With(@base => @base.Amount, -1)
                    .With(@base => @base.OpenPrice, 13.95m)
                    .With(@base => @base.OpenPriceIncludingCosts, 13.9195m)
                    .With(@base => @base.Status, PositionStatus.Open)
                    .With(@base => @base.ValueDate, new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc))
                    .With(@base => @base.OptionsData, Fixture
                        .Build<OptionsData>()
                        .With(optionsData => optionsData.Strike, 587)
                        .With(optionsData => optionsData.ExpiryDate, new DateTime(2025, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc))
                        .With(optionsData => optionsData.PutCall, "Call")
                        .Create()
                    )
                    .Create()
            )
            .With(position => position.DisplayAndFormat, Fixture
                .Build<DisplayAndFormat>()
                .With(displayAndFormat => displayAndFormat.Symbol, "SPY/11N25C587:xcbf")
                .With(displayAndFormat => displayAndFormat.Description, "SPDR S&P 500 ETF Trust Jul2025 587 C")
                .With(displayAndFormat => displayAndFormat.UnderlyingInstrumentDescription, "SPDR S&P 500 ETF Trust")
                .Create());
    }
    
    public static IPostprocessComposer<IndividualPosition> AsLongCall(this ICustomizationComposer<IndividualPosition> composer, Guid? correlationKey = null)
    {
        return composer
            .With(
                position => position.PositionBase, Fixture.Build<PositionBase>()
                    .With(@base => @base.CorrelationKey, correlationKey ?? Guid.NewGuid())
                    .With(@base => @base.AssetType, AssetType.StockOption)
                    .With(@base => @base.Amount, 1)
                    .With(@base => @base.OpenPrice, 14.64m)
                    .With(@base => @base.OpenPriceIncludingCosts, 14.6705m)
                    .With(@base => @base.Status, PositionStatus.Open)
                    .With(@base => @base.ValueDate, new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc))
                    .With(@base => @base.OptionsData, Fixture
                        .Build<OptionsData>()
                        .With(optionsData => optionsData.Strike, 586)
                        .With(optionsData => optionsData.ExpiryDate, new DateTime(2025, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc))
                        .With(optionsData => optionsData.PutCall, "Call")
                        .Create()
                    )
                    .Create()
            )
            .With(position => position.DisplayAndFormat, Fixture
                .Build<DisplayAndFormat>()
                .With(displayAndFormat => displayAndFormat.Symbol, "SPY/11N25C586:xcbf")
                .With(displayAndFormat => displayAndFormat.Description, "SPDR S&P 500 ETF Trust Jul2025 586 C")
                .With(displayAndFormat => displayAndFormat.UnderlyingInstrumentDescription, "SPDR S&P 500 ETF Trust")
                .Create());
    }
}