string? apiKey = Environment.GetEnvironmentVariable("ELEMENT14_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("API key not found. Set the ELEMENT14_API_KEY environment variable.");
    return;
}

int boardCount = 1;   // how many boards we are quoting; affects price breaks

List<BomLine> bom = BomReader.Read("farnell_bom.csv");
FarnellClient farnell = new FarnellClient(apiKey);

decimal total = 0;
int priced = 0;
List<string> notPriced = new List<string>();

foreach (BomLine line in bom)
{
    string? code = line.FarnellOrderCode;
    if (code is null)
    {
        notPriced.Add($"{line.Value} (x{line.Quantity}): no Farnell code ('{line.StockCode}')");
        continue;
    }

    int quantity = line.Quantity * boardCount;

    try
    {
        Product? product = await farnell.GetProductAsync(code);
        decimal? unitPrice = product?.GetUnitPrice(quantity);

        if (unitPrice is null)
        {
            notPriced.Add($"{line.Value} (Farnell {code}): not found or no price");
        }
        else
        {
            decimal lineTotal = unitPrice.Value * quantity;
            total += lineTotal;
            priced++;
            Console.WriteLine($"{line.Value,-22} {quantity,5} x {unitPrice.Value,9:F4} = {lineTotal,10:F2}");
        }
    }
    catch (Exception ex)
    {
        notPriced.Add($"{line.Value} (Farnell {code}): request failed - {ex.Message}");
    }

    await Task.Delay(600);   // stay below the 2 calls/second limit
}

Console.WriteLine();
Console.WriteLine($"Lines priced: {priced} of {bom.Count}");
Console.WriteLine($"Total for {boardCount} board(s): {total:F2}");

if (notPriced.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine("NOT included in the total:");
    foreach (string problem in notPriced)
    {
        Console.WriteLine($"  {problem}");
    }
}