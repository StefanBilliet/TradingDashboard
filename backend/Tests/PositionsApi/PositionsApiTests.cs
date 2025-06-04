using System.Net;
using Refit;
using Shouldly;
using WebApi.Features.GetPositionsPortfolio.ExternalApi;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Tests.PositionsApi;

public sealed class PositionsApiTests : IAsyncLifetime
{
    private const string ClientKey = "MY-CLIENT-KEY";
    private readonly WireMockServer _saxoMockServer;
    private readonly PositionsApiFactory _positionsApiFactory;

    public PositionsApiTests()
    {
        _saxoMockServer = WireMockServer.Start();
        _positionsApiFactory = new PositionsApiFactory();
    }

    [Fact]
    public async Task WHEN_GetPositions_without_correct_client_key_THEN_throws_exception()
    {
        _saxoMockServer
            .Given(
                Request.Create()
                    .WithPath("/port/v1/positions")
                    .WithParam("ClientKey", new RegexMatcher($"^(?!{ClientKey}$).*"))
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(400)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("""

                              {
                                "Message": "One or more properties of the request are invalid!",
                                "ModelState": {
                                  "ClientKey": [
                                    "Invalid Client Key: abc123
                                  ]
                                },
                                "ErrorCode": "InvalidModelState"
                              }

                              """)
            );
        var sut = _positionsApiFactory.Create(_saxoMockServer.Urls[0], "abc123");

        var result = await Should.ThrowAsync<ApiException>(async () => await sut.GetPositions(CancellationToken.None));
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WHEN_GetPositions_THEN_returns_correctly_deserialised_positions()
    {
        _saxoMockServer
            .Given(
                Request
                    .Create()
                    .WithPath("/port/v1/positions")
                    .WithParam("ClientKey", ClientKey)
                    .UsingGet()
            )
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(TestData.SpyBullCall));
        var sut = _positionsApiFactory.Create(_saxoMockServer.Urls[0], ClientKey);

        var positions = await sut.GetPositions(CancellationToken.None);

        positions.ShouldBeEquivalentTo(new SaxoResponse<IndividualPosition[]>([
            new IndividualPosition
            {
                NetPositionId = "49862353__CO__S",
                PositionId = "5023725059",
                PositionBase = new PositionBase
                {
                    AccountKey = "abc123",
                    AccountId = "99999999",
                    ClientId = "99999999",
                    AssetType = AssetType.StockOption,
                    Uic = 49862353,
                    Amount = -1,
                    OpenPrice = 13.95m,
                    OpenPriceIncludingCosts = 13.9195m,
                    Status = PositionStatus.Open,
                    CorrelationKey = Guid.Parse("25c8f0e8-f432-4ab2-8c6b-e351b282c42c"),
                    CanBeClosed = true,
                    ExecutionTimeOpen = new DateTime(2025,6,2,14,27,29,144,DateTimeKind.Utc),
                    OpenBondPoolFactor = 1,
                    RelatedOpenOrders = [],
                    SourceOrderId = "5035095572",
                    ValueDate = new DateTime(2025,6,2,0,0,0,DateTimeKind.Utc),
                    OptionsData = new OptionsData
                    {
                        CanBeExercised= false,
                        ExerciseStyle= "American",
                        ExpiryCut= "None",
                        ExpiryDate= new DateTime(2025,07,11,0,0,0,DateTimeKind.Utc),
                        PutCall= "Call",
                        SettlementStyle= "PhysicalDelivery",
                        Strike= 587
                    }
                },
                DisplayAndFormat = new DisplayAndFormat
                {
                    Description = "SPDR S&P 500 ETF Trust Jul2025 587 C",
                    Symbol = "SPY/11N25C587:xcbf",
                    UnderlyingInstrumentDescription = "SPDR S&P 500 ETF Trust"
                }
            },
            new IndividualPosition
            {
                NetPositionId = "49862358__CO__L",
                PositionId = "5023725057",
                PositionBase = new PositionBase
                {
                    AccountKey = "abc123",
                    AccountId = "99999999",
                    ClientId = "99999999",
                    AssetType = AssetType.StockOption,
                    Uic = 49862358,
                    Amount = 1,
                    OpenPrice = 14.64m,
                    OpenPriceIncludingCosts = 14.6705m,
                    Status = PositionStatus.Open,
                    CorrelationKey = Guid.Parse("25c8f0e8-f432-4ab2-8c6b-e351b282c42c"),
                    CanBeClosed = true,
                    ExecutionTimeOpen = new DateTime(2025,6,2,14,27,29,144,DateTimeKind.Utc),
                    OpenBondPoolFactor = 1,
                    RelatedOpenOrders = [],
                    SourceOrderId = "5035095571",
                    ValueDate = new DateTime(2025,6,2,0,0,0,DateTimeKind.Utc),
                    OptionsData = new OptionsData
                    {
                        CanBeExercised= true,
                        ExerciseStyle= "American",
                        ExpiryCut= "None",
                        ExpiryDate= new DateTime(2025,07,11,0,0,0,DateTimeKind.Utc),
                        PutCall= "Call",
                        SettlementStyle= "PhysicalDelivery",
                        Strike= 586
                    }
                },
                DisplayAndFormat = new DisplayAndFormat()
                {
                    Description = "SPDR S&P 500 ETF Trust Jul2025 586 C",
                    Symbol = "SPY/11N25C586:xcbf",
                    UnderlyingInstrumentDescription = "SPDR S&P 500 ETF Trust"
                }
            }
        ]));
    }

    public ValueTask DisposeAsync()
    {
        _saxoMockServer.Stop();
        _positionsApiFactory.Dispose();

        return ValueTask.CompletedTask;
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
}