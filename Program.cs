List<BomLine> bom = BomReader.Read("farnell_bom.csv");

int withCode = 0;
foreach (BomLine line in bom)
{
    string? code = line.FarnellOrderCode;
    if (code is null)
    {
        Console.WriteLine($"{line.Category,-20} {line.Quantity,3} x {line.Value,-22} NO FARNELL CODE ('{line.StockCode}')");
    }
    else
    {
        withCode++;
        Console.WriteLine($"{line.Category,-20} {line.Quantity,3} x {line.Value,-22} Farnell {code}");
    }
}

Console.WriteLine();
Console.WriteLine($"{bom.Count} lines read, {withCode} with a Farnell code, {bom.Count - withCode} without.");