using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using RetailerDisplay.Application.Catalog;

namespace RetailerDisplay.Infrastructure.Catalog;

/// <summary>
/// Parses a product CSV with CsvHelper. Expected headers (case-insensitive):
/// Sku, ProductName, Description, Category, Brand, ProductType, Abv, ContainerType,
/// Volume, PackSize, Vintage, Price, SalePrice, Currency.
/// Sku, ProductName and a valid Price are required per row.
/// </summary>
public class CsvProductImporter : ICsvProductImporter
{
    public IReadOnlyList<ParsedProductRow> Parse(Stream csv)
    {
        var results = new List<ParsedProductRow>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant(),
            MissingFieldFound = null,
            HeaderValidated = null,
            TrimOptions = TrimOptions.Trim
        };

        using var reader = new StreamReader(csv);
        using var csvReader = new CsvReader(reader, config);

        if (!csvReader.Read() || !csvReader.ReadHeader())
            return results;

        var rowNumber = 1; // header is row 1
        while (csvReader.Read())
        {
            rowNumber++;
            try
            {
                var sku = csvReader.GetField("sku");
                var name = csvReader.GetField("productname");

                if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(name))
                {
                    results.Add(Invalid(rowNumber, "Sku and ProductName are required."));
                    continue;
                }

                var price = OptDecimal(csvReader, "price");
                if (price is null or < 0)
                {
                    results.Add(Invalid(rowNumber, "Price is missing or invalid."));
                    continue;
                }

                results.Add(new ParsedProductRow(
                    rowNumber, true, null,
                    sku, name,
                    Opt(csvReader, "description"),
                    Opt(csvReader, "category"),
                    Opt(csvReader, "brand"),
                    Opt(csvReader, "producttype"),
                    OptDecimal(csvReader, "abv"),
                    Opt(csvReader, "containertype"),
                    Opt(csvReader, "volume"),
                    OptInt(csvReader, "packsize"),
                    OptInt(csvReader, "vintage"),
                    price.Value,
                    OptDecimal(csvReader, "saleprice"),
                    Opt(csvReader, "currency")));
            }
            catch (Exception ex)
            {
                results.Add(Invalid(rowNumber, $"Could not parse row: {ex.Message}"));
            }
        }

        return results;
    }

    private static ParsedProductRow Invalid(int row, string error) =>
        new(row, false, error, null, null, null, null, null, null, null, null, null, null, null, 0, null, null);

    private static string? Opt(CsvReader r, string name)
    {
        var v = SafeGet(r, name);
        return string.IsNullOrWhiteSpace(v) ? null : v.Trim();
    }

    private static string? SafeGet(CsvReader r, string name)
        => r.TryGetField<string>(name, out var v) ? v : null;

    private static decimal? OptDecimal(CsvReader r, string name)
    {
        var raw = SafeGet(r, name);
        if (string.IsNullOrWhiteSpace(raw)) return null;
        return decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : null;
    }

    private static int? OptInt(CsvReader r, string name)
    {
        var raw = SafeGet(r, name);
        if (string.IsNullOrWhiteSpace(raw)) return null;
        return int.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : null;
    }
}
