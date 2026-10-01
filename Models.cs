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