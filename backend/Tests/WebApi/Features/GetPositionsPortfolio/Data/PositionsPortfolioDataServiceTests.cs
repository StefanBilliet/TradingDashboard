using AutoFixture;
using FakeItEasy;
using Shouldly;
using WebApi.Features.GetPositionsPortfolio.Data;
using WebApi.Features.GetPositionsPortfolio.Data.ReadModels;
using WebApi.Features.GetPositionsPortfolio.ExternalApi;

namespace Tests.WebApi.Features.GetPositionsPortfolio.Data;

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
            .AsShortCall()
            .Create();
        var longCall = _fixture.Build<IndividualPosition>()
            .AsLongCall()
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

    [Fact]
    public async Task GIVEN_two_related_positions_in_the_portfolio_WHEN_Get_THEN_returns_portfolio_with_one_combined_position()
    {
        var correlationKey = new Guid("AEFD9794-1863-4736-BDF0-B2BA2FF21654");
        var shortCall = _fixture.Build<IndividualPosition>()
            .AsShortCall(correlationKey)
            .Create();
        var longCall = _fixture.Build<IndividualPosition>()
            .AsLongCall(correlationKey)
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
                ),
                new Leg(
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

