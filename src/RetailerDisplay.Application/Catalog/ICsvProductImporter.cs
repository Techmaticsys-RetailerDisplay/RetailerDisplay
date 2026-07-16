namespace RetailerDisplay.Application.Catalog;

/// <summary>A single parsed CSV row — either valid product values or a row-level error.</summary>
public record ParsedProductRow(
    int RowNumber,
    bool IsValid,
    string? Error,
    string? Sku,
    string? ProductName,
    string? Description,
    string? Category,
    string? Brand,
    string? ProductType,
    decimal? Abv,
    string? ContainerType,
    string? Volume,
    int? PackSize,
    int? Vintage,
    decimal Price,
    decimal? SalePrice,
    string? Currency);

/// <summary>Parses a product CSV into typed rows with per-row validation. Implemented with CsvHelper.</summary>
public interface ICsvProductImporter
{
    IReadOnlyList<ParsedProductRow> Parse(Stream csv);
}
