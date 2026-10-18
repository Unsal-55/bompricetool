using System.Text.Json;

public class FarnellClient
{
    private readonly HttpClient _http = new HttpClient();
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public FarnellClient(string apiKey)
    {
        _apiKey = apiKey;
    }

    // Returns the product, or null if Farnell has no such product.
    // Throws an exception if the request itself fails.
    public async Task<Product?> GetProductAsync(string orderCode)
    {
        string url = "https://api.element14.com/catalog/products"
            + $"?term=id%3A{orderCode}"
            + "&storeInfo.id=tr.farnell.com"
            + "&resultsSettings.offset=0"
            + "&resultsSettings.numberOfResults=1"
            + "&resultsSettings.responseGroup=medium"
            + "&callInfo.responseDataFormat=JSON"
            + $"&callInfo.apiKey={_apiKey}";

        HttpResponseMessage response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();
        SearchResponse? result = JsonSerializer.Deserialize<SearchResponse>(body, _jsonOptions);

        return result?.PremierFarnellPartNumberReturn?.Products.FirstOrDefault();
    }
}