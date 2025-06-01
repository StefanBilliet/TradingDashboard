using System.Net;
using Refit;
using Shouldly;
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
                .WithBody("""

                          {
                            "Data": [
                              {
                                "NetPositionId": "SPY_PUT_560_565_Net",
                                "PositionBase": {
                                  "AccountId": "SIM-12345678-90AB-CDEF-1234-567890ABCDEF",
                                  "AssetType": "Option",
                                  "Uic": 38765,
                                  "OptionType": "Put",
                                  "Strike": 565.0,
                                  "ExpiryDate": "2025-07-11T22:00:00Z",
                                  "Amount": -1,
                                  "OpenPrice": 1.50,
                                  "Status": "Open"
                                },
                                "PositionId": "POS-SIM-0001",
                                "PositionView": {
                                  "Bid": 1.45,
                                  "Ask": 1.55,
                                  "Mid": 1.50
                                }
                              },
                              {
                                "NetPositionId": "SPY_PUT_560_565_Net",
                                "PositionBase": {
                                  "AccountId": "SIM-12345678-90AB-CDEF-1234-567890ABCDEF",
                                  "AssetType": "Option",
                                  "Uic": 38764,
                                  "OptionType": "Put",
                                  "Strike": 560.0,
                                  "ExpiryDate": "2025-07-11T22:00:00Z",
                                  "Amount": 1,
                                  "OpenPrice": 2.00,
                                  "Status": "Open"
                                },
                                "PositionId": "POS-SIM-0002",
                                "PositionView": {
                                  "Bid": 1.95,
                                  "Ask": 2.05,
                                  "Mid": 2.00
                                }
                              }
                            ]
                          }

                          """));
        var sut = _positionsApiFactory.Create(_saxoMockServer.Urls[0], ClientKey);

        var positions = await sut.GetPositions(CancellationToken.None);

        positions.ShouldBeEquivalentTo(new SaxoResponse<IndividualPosition[]>([
            new IndividualPosition
            {
                NetPositionId = "SPY_PUT_560_565_Net",
                PositionId = "POS-SIM-0001",
                PositionBase = new PositionBase
                {
                    AccountId = "SIM-12345678-90AB-CDEF-1234-567890ABCDEF",
                    AssetType = "Option",
                    Uic = 38765,
                    OptionType = "Put",
                    Strike = 565.0,
                    ExpiryDate = new DateTime(2025, 7, 11, 22, 0, 0, DateTimeKind.Utc),
                    Amount = -1,
                    OpenPrice = 1.50,
                    Status = "Open"
                },
                PositionView = new PositionView
                {
                    Ask = 1.55,
                    Bid = 1.45,
                    Mid = 1.50,
                }
            },
            new IndividualPosition
            {
                NetPositionId = "SPY_PUT_560_565_Net",
                PositionId = "POS-SIM-0002",
                PositionBase = new PositionBase
                {
                    AccountId = "SIM-12345678-90AB-CDEF-1234-567890ABCDEF",
                    AssetType = "Option",
                    Uic = 38764,
                    OptionType = "Put",
                    Strike = 560.0,
                    OpenPrice = 2.00,
                    Status = "Open",
                    Amount = 1,
                    ExpiryDate = new DateTime(2025, 7, 11, 22, 0, 0, DateTimeKind.Utc)
                },
                PositionView = new PositionView
                {
                    Ask = 2.05,
                    Bid = 1.95,
                    Mid = 2.00,
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