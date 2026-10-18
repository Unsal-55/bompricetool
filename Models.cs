public class SearchResponse
{
    public PartNumberReturn? PremierFarnellPartNumberReturn { get; set; }
}

public class PartNumberReturn
{
    public int NumberOfResults { get; set; }
    public List<Product> Products { get; set; } = new();
}

public class Product
{
    public string Sku { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string TranslatedManufacturerPartNumber { get; set; } = "";
    public List<PriceBreak> Prices { get; set; } = new();
    public Stock? Stock { get; set; }

    // Returns the unit price for the given quantity, or null if there are no prices.
    public decimal? GetUnitPrice(int quantity)
    {
        if (Prices.Count == 0)
        {
            return null;
        }

        foreach (PriceBreak pb in Prices)
        {
            if (quantity >= pb.From && quantity <= pb.To)
            {
                return pb.Cost;
            }
        }

        // Quantity is below the first break: use the first price (we ignore minimum quantities).
        if (quantity < Prices[0].From)
        {
            return Prices[0].Cost;
        }

        // Quantity is above all breaks: use the last price.
        return Prices[Prices.Count - 1].Cost;
    }
}

public class PriceBreak
{
    public int From { get; set; }
    public int To { get; set; }
    public decimal Cost { get; set; }
}

public class Stock
{
    public int Level { get; set; }
    public int Status { get; set; }
}

public class BomLine
{
    public string Category { get; set; } = "";
    public int Quantity { get; set; }
    public string References { get; set; } = "";
    public string Value { get; set; } = "";
    public string StockCode { get; set; } = "";

    // Returns the Farnell order code, or null if this line has none.
    public string? FarnellOrderCode
    {
        get
        {
            const string prefix = "Farnell ";
            if (StockCode.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                string code = StockCode.Substring(prefix.Length).Trim();
                if (code.Length > 0 && code.All(char.IsDigit))
                {
                    return code;
                }
            }
            return null;
        }
    }
}