namespace Tests.PositionsApi;

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