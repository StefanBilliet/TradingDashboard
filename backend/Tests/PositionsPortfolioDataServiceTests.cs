using AutoFixture;
using AutoFixture.Xunit3;
using FakeItEasy;
using Shouldly;
using Tests.PositionsApi;
using Vogen;

namespace Tests;

public class PositionsPortfolioDataServiceTests
{
    private readonly PositionsPortfolioDataService _sut;
    private readonly IPositionsApi _positionsApi;
    private readonly Fixture _fixture;

    public PositionsPortfolioDataServiceTests()
    {
        _positionsApi = A.Fake<IPositionsApi>();
        _sut = new PositionsPortfolioDataService(_positionsApi);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GIVEN_no_positions_in_portfolio_WHEN_Get_THEN_returns_empty_portfolio()
    {
        A.CallTo(() => _positionsApi.GetPositions(A<CancellationToken>.Ignored)).Returns(new SaxoResponse<IndividualPosition[]>([]));

        var result = await _sut.Get(CancellationToken.None);

        result.Positions.ShouldBeEmpty();
    }

    [Fact]
    public async Task GIVEN_no_options_in_portfolio_WHEN_Get_THEN_returns_empty_portfolio()
    {
        A.CallTo(() => _positionsApi.GetPositions(A<CancellationToken>.Ignored)).Returns(new SaxoResponse<IndividualPosition[]>([
            _fixture.Build<IndividualPosition>()
                .With(
                    position => position.PositionBase, _fixture.Build<PositionBase>().With(@base => @base.AssetType, AssetType.Stock).Create()
                )
                .Create()
        ]));

        var result = await _sut.Get(CancellationToken.None);

        result.Positions.ShouldBeEmpty();
    }

    [Fact]
    public async Task GIVEN_two_unrelated_positions_in_the_portfolio_WHEN_Get_THEN_returns_portfolio_with_two_separate_positions()
    {
        var shortCall = _fixture.Build<IndividualPosition>()
            .With(
                position => position.PositionBase, _fixture.Build<PositionBase>()
                    .With(@base => @base.AssetType, AssetType.StockOption)
                    .With(@base => @base.Amount, -1)
                    .With(@base => @base.OpenPrice, 13.95m)
                    .With(@base => @base.OpenPriceIncludingCosts, 13.9195m)
                    .With(@base => @base.Status, PositionStatus.Open)
                    .With(@base => @base.ValueDate, new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc))
                    .With(@base => @base.OptionsData, _fixture
                        .Build<OptionsData>()
                        .With(optionsData => optionsData.Strike, 587)
                        .With(optionsData => optionsData.ExpiryDate, new DateTime(2025, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc))
                        .With(optionsData => optionsData.PutCall, "Call")
                        .Create()
                    )
                    .Create()
            )
            .With(position => position.DisplayAndFormat, _fixture
                .Build<DisplayAndFormat>()
                .With(displayAndFormat => displayAndFormat.Symbol, "SPY/11N25C587:xcbf")
                .With(displayAndFormat => displayAndFormat.Description, "SPDR S&P 500 ETF Trust Jul2025 587 C")
                .With(displayAndFormat => displayAndFormat.UnderlyingInstrumentDescription, "SPDR S&P 500 ETF Trust")
                .Create())
            .Create();
        var longCall = _fixture.Build<IndividualPosition>()
            .With(
                position => position.PositionBase, _fixture.Build<PositionBase>()
                    .With(@base => @base.AssetType, AssetType.StockOption)
                    .With(@base => @base.Amount, 1)
                    .With(@base => @base.OpenPrice, 14.64m)
                    .With(@base => @base.OpenPriceIncludingCosts, 14.6705m)
                    .With(@base => @base.Status, PositionStatus.Open)
                    .With(@base => @base.ValueDate, new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc))
                    .With(@base => @base.OptionsData, _fixture
                        .Build<OptionsData>()
                        .With(optionsData => optionsData.Strike, 586)
                        .With(optionsData => optionsData.ExpiryDate, new DateTime(2025, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc))
                        .With(optionsData => optionsData.PutCall, "Call")
                        .Create()
                    )
                    .Create()
            )
            .With(position => position.DisplayAndFormat, _fixture
                .Build<DisplayAndFormat>()
                .With(displayAndFormat => displayAndFormat.Symbol, "SPY/11N25C586:xcbf")
                .With(displayAndFormat => displayAndFormat.Description, "SPDR S&P 500 ETF Trust Jul2025 586 C")
                .With(displayAndFormat => displayAndFormat.UnderlyingInstrumentDescription, "SPDR S&P 500 ETF Trust")
                .Create())
            .Create();
        var positionsResponse = new SaxoResponse<IndividualPosition[]>([shortCall, longCall]);
        A.CallTo(() => _positionsApi.GetPositions(A<CancellationToken>.Ignored)).Returns(positionsResponse);

        var result = await _sut.Get(CancellationToken.None);

        result.Positions.ShouldBeEquivalentTo(new[]
        {
            new Position(new Leg(
                shortCall.DisplayAndFormat.Symbol,
                shortCall.PositionBase.OptionsData!.Strike,
                shortCall.PositionBase.Amount,
                shortCall.PositionBase.OpenPriceIncludingCosts,
                shortCall.PositionBase.Status,
                shortCall.PositionBase.CorrelationKey
            )),
            new Position(new Leg(
                longCall.DisplayAndFormat.Symbol,
                longCall.PositionBase.OptionsData!.Strike,
                longCall.PositionBase.Amount,
                longCall.PositionBase.OpenPriceIncludingCosts,
                longCall.PositionBase.Status,
                longCall.PositionBase.CorrelationKey
            ))
        });
    }
}

public record Leg(string Symbol, decimal Strike, double Amount, decimal OpenPriceIncludingCosts, PositionStatus Status, Guid CorrelationKey);

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
            .Select(position => new Position(
                new Leg(position.DisplayAndFormat.Symbol, position.PositionBase.OptionsData!.Strike, position.PositionBase.Amount,
                    position.PositionBase.OpenPriceIncludingCosts, position.PositionBase.Status, position.PositionBase.CorrelationKey)
            )).ToArray()
        );
    }
}

public record PositionsPortfolio(params Position[] Positions)
{
}

public record Position(params Leg[] Legs);