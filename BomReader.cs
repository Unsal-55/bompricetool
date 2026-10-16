using System.Text;

public static class BomReader
{
    public static List<BomLine> Read(string path, char separator = ',')
    {
        List<BomLine> lines = new List<BomLine>();
        string[] fileLines = File.ReadAllLines(path);

        if (fileLines.Length == 0)
        {
            Console.WriteLine("The BOM file is empty.");
            return lines;
        }

        // Find where each column is, using the names in the header line.
        List<string> header = SplitLine(fileLines[0], separator);
        int categoryCol = FindColumn(header, "Category");
        int quantityCol = FindColumn(header, "Quantity");
        int referencesCol = FindColumn(header, "References");
        int valueCol = FindColumn(header, "Value");
        int stockCodeCol = FindColumn(header, "Stock Code");

        if (quantityCol < 0 || stockCodeCol < 0)
        {
            Console.WriteLine("The header must contain at least the columns 'Quantity' and 'Stock Code'.");
            return lines;
        }

        for (int i = 1; i < fileLines.Length; i++)
        {
            string text = fileLines[i];
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            List<string> fields = SplitLine(text, separator);

            if (!int.TryParse(GetField(fields, quantityCol), out int quantity))
            {
                Console.WriteLine($"Line {i + 1} skipped (quantity unreadable): {text}");
                continue;
            }

            lines.Add(new BomLine
            {
                Category = GetField(fields, categoryCol),
                Quantity = quantity,
                References = GetField(fields, referencesCol),
                Value = GetField(fields, valueCol),
                StockCode = GetField(fields, stockCodeCol)
            });
        }

        return lines;
    }

    // Returns the position of a column in the header, or -1 if it is not there.
    private static int FindColumn(List<string> header, string name)
    {
        for (int i = 0; i < header.Count; i++)
        {
            if (string.Equals(header[i].Trim(), name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1;
    }

    // Returns the text of one column, or "" if the column does not exist.
    private static string GetField(List<string> fields, int column)
    {
        if (column < 0 || column >= fields.Count)
        {
            return "";
        }
        return fields[column].Trim();
    }

    // Splits one CSV line at the separators, but not at separators inside quotes.
    private static List<string> SplitLine(string line, char separator)
    {
        List<string> fields = new List<string>();
        StringBuilder current = new StringBuilder();
        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');   // two quotes in a row mean one literal quote
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (c == separator && !insideQuotes)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }
}