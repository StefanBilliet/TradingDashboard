using System.Text.Json;
using Refit;

namespace Tests.PositionsApi;

public sealed class PositionsApiFactory : IDisposable
{
    private readonly List<IDisposable> _thingsToBeCleanedUp;

    public PositionsApiFactory()
    {
        _thingsToBeCleanedUp = [];
    }

    public IPositionsApi Create(string url, string clientKey)
    {
        var settings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions())
        };
        var handler = new ClientKeyHandler(clientKey)
        {
            InnerHandler = new HttpClientHandler()
        };
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(url)
        };
        _thingsToBeCleanedUp.Add(httpClient);
        return RestService.For<IPositionsApi>(httpClient, settings);
    }

    public void Dispose()
    {
        foreach (var disposable in _thingsToBeCleanedUp)
        {
            disposable.Dispose();
        }
    }
}