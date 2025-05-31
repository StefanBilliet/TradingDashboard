using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Refit;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace Tests;

public sealed class PortfolioPositionsDataServiceTests : IAsyncLifetime
{
    private readonly WireMockServer _saxoMockServer;
    private readonly IPositionsApi _positionsApi;

    public PortfolioPositionsDataServiceTests()
    {
        _saxoMockServer = WireMockServer.Start();
        
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var settings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(jsonOptions)
        };
        var handler = new ClientKeyHandler("MY-CLIENT-KEY")
        {
            InnerHandler = new HttpClientHandler()
        };
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(_saxoMockServer.Urls[0])
        };
        _positionsApi = RestService.For<IPositionsApi>(httpClient, settings);
    }

    [Fact]
    public async Task TestPositionsReturnsBullPut_WithoutAttributes()
    {
        _saxoMockServer
          .Given(
              Request
                  .Create()
                  .WithPath("/port/v1/positions")
                  .WithParam("ClientKey", new RegexMatcher(".+"))
                  .UsingGet()
              )
          .RespondWith(Response.Create()
              .WithStatusCode(200)
              .WithHeader("Content-Type", "application/json")
              .WithBody("""{ "positions": [ { "symbol": "SPY", "strategy": "bullput" } ] }"""));

        var portfolioPositions = await _positionsApi.GetPositions();

        Assert.Single(portfolioPositions.Positions);
        Assert.Equal("SPY", portfolioPositions.Positions.First().Symbol);
        Assert.Equal("bullput", portfolioPositions.Positions.First().Strategy);
    }

    public interface IPositionsApi
    {
        [Get("/port/v1/positions")]
        Task<PortfolioPositions> GetPositions();
    }

    public record PortfolioPositions(IReadOnlyCollection<Position> Positions);

    public record Position(string Symbol, string Strategy);

    public ValueTask DisposeAsync()
    {
        _saxoMockServer.Stop();
        return ValueTask.CompletedTask;
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
}

public class ClientKeyHandler : DelegatingHandler
{
    private readonly string _clientKey;

    public ClientKeyHandler(string clientKey)
    {
        _clientKey = clientKey;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri!;
        var queryString = System.Web.HttpUtility.ParseQueryString(uri.Query);
        queryString["ClientKey"] = _clientKey;

        var uriBuilder = new UriBuilder(uri)
        {
            Query = queryString.ToString()
        };
        request.RequestUri = uriBuilder.Uri;

        return base.SendAsync(request, cancellationToken);
    }
}
