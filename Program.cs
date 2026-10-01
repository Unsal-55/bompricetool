using System.Text.Json;

string? apiKey = Environment.GetEnvironmentVariable("ELEMENT14_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("API key not found. Set the ELEMENT14_API_KEY environment variable.");
    return;
}

string orderCode = "2425985";

string url = "https://api.element14.com/catalog/products"
    + $"?term=id%3A{orderCode}"
    + "&storeInfo.id=tr.farnell.com"
    + "&resultsSettings.offset=0"
    + "&resultsSettings.numberOfResults=1"
    + "&resultsSettings.responseGroup=medium"
    + "&callInfo.responseDataFormat=JSON"
    + $"&callInfo.apiKey={apiKey}";

using HttpClient client = new HttpClient();
HttpResponseMessage response = await client.GetAsync(url);
string body = await response.Content.ReadAsStringAsync();

// Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
// Console.WriteLine(body);

if (!response.IsSuccessStatusCode)
{
    Console.WriteLine($"Request failed: {(int)response.StatusCode} {response.StatusCode}");
    Console.WriteLine(body);
    return;
}

JsonSerializerOptions options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

SearchResponse? result = JsonSerializer.Deserialize<SearchResponse>(body, options);
Product? product = result?.PremierFarnellPartNumberReturn?.Products.FirstOrDefault();

if (product is null)
{
    Console.WriteLine($"No product found for order code {orderCode}.");
    return;
}

Console.WriteLine($"{product.Sku} - {product.DisplayName}");
Console.WriteLine($"Manufacturer part number: {product.TranslatedManufacturerPartNumber}");
Console.WriteLine($"Stock: {product.Stock?.Level}");
Console.WriteLine("Price breaks:");
foreach (PriceBreak pb in product.Prices)
{
    Console.WriteLine($"  {pb.From} - {pb.To} pcs: {pb.Cost}");
}