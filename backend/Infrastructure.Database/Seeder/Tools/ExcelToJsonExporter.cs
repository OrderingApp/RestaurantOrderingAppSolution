using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Infrastructure.Database.Seeder.SeedModels;
using Domain;
using OfficeOpenXml;

namespace Infrastructure.Database.Seeder.Tools;

// Dev-only exporter: read Excel worksheets and produce typed JSON files matching SeedModels
public class ExcelToJsonExporter
{
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
    
    public ExcelToJsonExporter()
    {
        // Ensure enum values are serialized as strings in exported JSON (e.g. "Available")
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<ExcelExportReport> ExportAsync(string excelPath, string? outputFolder = null)
    {
        if (string.IsNullOrWhiteSpace(excelPath))
            throw new ArgumentException("excelPath is required", nameof(excelPath));

        if (!File.Exists(excelPath))
            throw new FileNotFoundException("Excel file not found", excelPath);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage(new FileInfo(excelPath));

        outputFolder ??= Path.Combine(Directory.GetCurrentDirectory(), "Seeder", "SeedData");
        Directory.CreateDirectory(outputFolder);

        // List of sheets to attempt to export. If a sheet does not exist it's skipped.
        var reports = new List<SheetReport?>();

        var tasks = new List<Task<SheetReport?>>()
        {
            ExportIfExistsAsync<MenuCategorySeedModel>(package, "MenuCategory", Path.Combine(outputFolder, "menu-categories.json"), MapMenuCategory),
            ExportIfExistsAsync<SubCategorySeedModel>(package, "SubCategory", Path.Combine(outputFolder, "sub-categories.json"), MapSubCategory),
            ExportIfExistsAsync<IngredientSeedModel>(package, "Ingredient", Path.Combine(outputFolder, "ingredients.json"), MapIngredient),
            ExportIfExistsAsync<TagSeedModel>(package, "Tag", Path.Combine(outputFolder, "tags.json"), MapTag),
            ExportIfExistsAsync<AreaSeedModel>(package, "Area", Path.Combine(outputFolder, "areas.json"), MapArea),
            ExportIfExistsAsync<TableSeedModel>(package, "Table", Path.Combine(outputFolder, "tables.json"), MapTable),
            ExportIfExistsAsync<MenuItemSeedModel>(package, "MenuItem", Path.Combine(outputFolder, "menu-items.json"), MapMenuItem),
            ExportIfExistsAsync<IngredientTagRelSeedModel>(package, "IngredientTagRel", Path.Combine(outputFolder, "ingredient-tag-rels.json"), MapIngredientTagRel),
            ExportIfExistsAsync<MenuItemIngredientRelSeedModel>(package, "MenuItemIngredientRel", Path.Combine(outputFolder, "menu-item-ingredient-rels.json"), MapMenuItemIngredientRel),

            // optional sheets
            ExportIfExistsAsync<IngredientCategorySeedModel>(package, "IngredientCategory", Path.Combine(outputFolder, "ingredient-categories.json"), MapIngredientCategory),
            ExportIfExistsAsync<AllergenSeedModel>(package, "Allergen", Path.Combine(outputFolder, "allergens.json"), MapAllergen),
            ExportIfExistsAsync<IngredientAllergenRelSeedModel>(package, "IngredientAllergenRel", Path.Combine(outputFolder, "ingredient-allergen-rels.json"), MapIngredientAllergenRel)
        };

        // run sequentially to avoid excessive file IO in parallel
        foreach (var t in tasks)
        {
            if (t != null)
            {
                var rep = await t;
                reports.Add(rep);
            }
        }

        return new ExcelExportReport { Sheets = reports.Where(r => r != null).Cast<SheetReport>().ToList() };
    }

    private async Task<SheetReport?> ExportIfExistsAsync<TModel>(ExcelPackage package, string sheetName, string outputPath, Func<ExcelWorksheet, SheetReport, List<TModel>> mapper)
    {
        var sheet = package.Workbook.Worksheets[sheetName];
        if (sheet == null)
            return null;

        var report = new SheetReport { SheetName = sheetName };
        var models = mapper(sheet, report) ?? new List<TModel>();
        report.ExportedCount = models.Count;

        await WriteJsonAsync(outputPath, models);
        return report;
    }

    private async Task WriteJsonAsync<T>(string path, List<T> data)
    {
        await using var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(fs, data, _jsonOptions);
    }

    // Generic reader: build rows as dictionary header->text
    private record RowData(int RowNumber, Dictionary<string, string?> Values);

    private static List<RowData> ReadRowsAsDictionary(ExcelWorksheet sheet)
    {
        var result = new List<RowData>();
        if (sheet.Dimension == null)
            return result;

        var startRow = sheet.Dimension.Start.Row;
        var endRow = sheet.Dimension.End.Row;
        var startCol = sheet.Dimension.Start.Column;
        var endCol = sheet.Dimension.End.Column;

        // read headers from first row
        var headers = new List<string>();
        for (int col = startCol; col <= endCol; col++)
        {
            var raw = (sheet.Cells[startRow, col].Text ?? string.Empty).Trim();
            headers.Add(raw);
        }

        for (int row = startRow + 1; row <= endRow; row++)
        {
            var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            var allEmpty = true;
            for (int col = startCol; col <= endCol; col++)
            {
                var header = headers[col - startCol];
                var value = sheet.Cells[row, col].Text;
                if (!string.IsNullOrEmpty(value)) allEmpty = false;
                dict[header ?? string.Empty] = string.IsNullOrWhiteSpace(value) ? null : value;
            }

            // skip fully empty rows
            if (allEmpty)
                continue;

            result.Add(new RowData(row, dict));
        }

        return result;
    }

    // helpers to read from dictionary (case-insensitive by key)
    private static string? GetString(Dictionary<string, string?> dict, params string[] possibleKeys)
    {
        foreach (var k in possibleKeys)
        {
            if (dict.TryGetValue(k, out var v) && v != null)
                return v;
        }
        // try forgiving matching by normalized keys
        foreach (var kv in dict)
        {
            var keyNormalized = Normalize(kv.Key);
            foreach (var k in possibleKeys)
            {
                if (Normalize(k) == keyNormalized && kv.Value != null)
                    return kv.Value;
            }
        }

        return null;
    }

    private static Guid? GetGuid(Dictionary<string, string?> dict, params string[] possibleKeys)
    {
        var s = GetString(dict, possibleKeys);
        if (string.IsNullOrWhiteSpace(s))
            return null;
        return Guid.TryParse(s, out var g) ? g : null;
    }

    private static bool? GetBool(Dictionary<string, string?> dict, params string[] possibleKeys)
    {
        var s = GetString(dict, possibleKeys);
        if (s == null) return null;
        if (bool.TryParse(s, out var b)) return b;
        s = s.Trim();
        if (s == "1" || s.Equals("yes", StringComparison.OrdinalIgnoreCase) || s.Equals("y", StringComparison.OrdinalIgnoreCase)) return true;
        if (s == "0" || s.Equals("no", StringComparison.OrdinalIgnoreCase) || s.Equals("n", StringComparison.OrdinalIgnoreCase)) return false;
        return null;
    }

    private static int? GetInt(Dictionary<string, string?> dict, params string[] possibleKeys)
    {
        var s = GetString(dict, possibleKeys);
        if (string.IsNullOrWhiteSpace(s)) return null;
        return int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : null;
    }

    private static decimal? GetDecimal(Dictionary<string, string?> dict, params string[] possibleKeys)
    {
        var s = GetString(dict, possibleKeys);
        if (string.IsNullOrWhiteSpace(s)) return null;
        return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : null;
    }

    private static DateTime? GetDateTime(Dictionary<string, string?> dict, params string[] possibleKeys)
    {
        var s = GetString(dict, possibleKeys);
        if (string.IsNullOrWhiteSpace(s)) return null;
        return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt) ? dt : null;
    }

    private static string Normalize(string s)
    {
        return new string((s ?? string.Empty).Where(c => !char.IsWhiteSpace(c) && c != '_' && c != '-').ToArray()).ToLowerInvariant();
    }

    // Mapping functions for each sheet -> List<SeedModel>
    private List<MenuCategorySeedModel> MapMenuCategory(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<MenuCategorySeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var id = GetGuid(r, "Id", "ID");
            if (id == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing or invalid Id");
                continue;
            }

            var name = GetString(r, "Name") ?? string.Empty;
            var isUsed = GetBool(r, "IsUsed", "Used") ?? true;
            var isDeleted = GetBool(r, "IsDeleted", "Deleted") ?? false;
            var seq = GetInt(r, "SequenceNumber", "Sequence", "Seq");
            if (seq == null)
            {
                seq = excelRow - sheet.Dimension.Start.Row; // row index starting at 1 for first data row
                report.Warnings.Add($"Defaulted SequenceNumber in {report.SheetName} row {excelRow} to {seq}");
            }

            list.Add(new MenuCategorySeedModel
            {
                Id = id.Value,
                Name = name,
                IsUsed = isUsed,
                IsDeleted = isDeleted,
                SequenceNumber = seq.Value
            });
        }

        return list;
    }

    private List<SubCategorySeedModel> MapSubCategory(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<SubCategorySeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var id = GetGuid(r, "Id");
            if (id == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing or invalid Id");
                continue;
            }

            var menuCategoryId = GetGuid(r, "MenuCategoryId", "MenuCategory", "MenuCategoryID");
            if (menuCategoryId == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing MenuCategoryId (required)");
                continue;
            }

            var name = GetString(r, "Name") ?? string.Empty;
            var isUsed = GetBool(r, "IsUsed") ?? true;
            var isDeleted = GetBool(r, "IsDeleted") ?? false;
            var seq = GetInt(r, "SequenceNumber", "Sequence", "Seq");
            if (seq == null)
            {
                seq = excelRow - sheet.Dimension.Start.Row;
                report.Warnings.Add($"Defaulted SequenceNumber in {report.SheetName} row {excelRow} to {seq}");
            }

            list.Add(new SubCategorySeedModel
            {
                Id = id.Value,
                Name = name,
                IsUsed = isUsed,
                IsDeleted = isDeleted,
                MenuCategoryId = menuCategoryId.Value,
                SequenceNumber = seq.Value
            });
        }

        return list;
    }

    private List<IngredientCategorySeedModel> MapIngredientCategory(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<IngredientCategorySeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var id = GetGuid(r, "Id");
            if (id == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing or invalid Id");
                continue;
            }

            var name = GetString(r, "Name") ?? string.Empty;
            var isUsed = GetBool(r, "IsUsed") ?? true;
            var isDeleted = GetBool(r, "IsDeleted") ?? false;

            list.Add(new IngredientCategorySeedModel
            {
                Id = id.Value,
                Name = name,
                IsUsed = isUsed,
                IsDeleted = isDeleted
            });
        }

        return list;
    }

    private List<IngredientSeedModel> MapIngredient(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<IngredientSeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var id = GetGuid(r, "Id");
            if (id == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing or invalid Id");
                continue;
            }

            var name = GetString(r, "Name") ?? string.Empty;
            var price = GetDecimal(r, "Price") ?? 0m;
            var canBeUsedAsExtra = GetBool(r, "CanBeUsedAsExtra", "CanBeExtra", "CanBeUsed") ?? false;
            var isDeleted = GetBool(r, "IsDeleted") ?? false;
            var categoryId = GetGuid(r, "CategoryId", "IngredientCategoryId", "Category");

            list.Add(new IngredientSeedModel
            {
                Id = id.Value,
                Name = name,
                Price = price,
                CanBeUsedAsExtra = canBeUsedAsExtra,
                IsDeleted = isDeleted,
                CategoryId = categoryId
            });
        }

        return list;
    }

    private List<TagSeedModel> MapTag(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<TagSeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var id = GetGuid(r, "Id");
            if (id == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing or invalid Id");
                continue;
            }

            var name = (GetString(r, "Name") ?? string.Empty).Trim();
            var isUsed = GetBool(r, "IsUsed") ?? true;
            var isDeleted = GetBool(r, "IsDeleted") ?? false;

            list.Add(new TagSeedModel
            {
                Id = id.Value,
                Name = name,
                IsUsed = isUsed,
                IsDeleted = isDeleted
            });
        }

        return list;
    }

    private List<AllergenSeedModel> MapAllergen(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<AllergenSeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var id = GetGuid(r, "Id");
            if (id == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing or invalid Id");
                continue;
            }

            var name = GetString(r, "Name") ?? string.Empty;
            var eu = GetInt(r, "EuNumber", "EU", "Eu");
            var isUsed = GetBool(r, "IsUsed") ?? true;
            var isDeleted = GetBool(r, "IsDeleted") ?? false;

            list.Add(new AllergenSeedModel
            {
                Id = id.Value,
                Name = name,
                EuNumber = eu,
                IsUsed = isUsed,
                IsDeleted = isDeleted
            });
        }

        return list;
    }

    private List<AreaSeedModel> MapArea(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<AreaSeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var id = GetGuid(r, "Id");
            if (id == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing or invalid Id");
                continue;
            }

            var name = GetString(r, "Name") ?? string.Empty;
            var isUsed = GetBool(r, "IsUsed") ?? true;
            var isDeleted = GetBool(r, "IsDeleted") ?? false;
            var seq = GetInt(r, "SequenceNumber", "Sequence", "Seq");
            if (seq == null)
            {
                seq = excelRow - sheet.Dimension.Start.Row;
                report.Warnings.Add($"Defaulted SequenceNumber in {report.SheetName} row {excelRow} to {seq}");
            }

            list.Add(new AreaSeedModel
            {
                Id = id.Value,
                Name = name,
                IsUsed = isUsed,
                IsDeleted = isDeleted,
                SequenceNumber = seq.Value
            });
        }

        return list;
    }

    private List<TableSeedModel> MapTable(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<TableSeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var id = GetGuid(r, "Id");
            if (id == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing or invalid Id");
                continue;
            }

            var areaId = GetGuid(r, "AreaId", "Area");
            if (areaId == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing AreaId (required)");
                continue;
            }

            var name = GetString(r, "Name") ?? string.Empty;
            var capacity = GetInt(r, "Capacity") ?? 0;
            var seq = GetInt(r, "SequenceNumber", "Sequence", "Seq");
            if (seq == null)
            {
                seq = excelRow - sheet.Dimension.Start.Row;
                report.Warnings.Add($"Defaulted SequenceNumber in {report.SheetName} row {excelRow} to {seq}");
            }

            var isPrepared = GetBool(r, "IsPrepared") ?? false; // default false because old Excel lacked this column
            if (!r.ContainsKey("IsPrepared") || GetString(r, "IsPrepared") == null)
            {
                report.Warnings.Add($"Defaulted IsPrepared in {report.SheetName} row {excelRow} to false");
            }

            var activeSince = GetDateTime(r, "ActiveSince", "ActiveSinceUtc");
            if (activeSince == null)
            {
                report.Warnings.Add($"Defaulted ActiveSince in {report.SheetName} row {excelRow} to null");
            }

            var isUsed = GetBool(r, "IsUsed") ?? true;
            var isDeleted = GetBool(r, "IsDeleted") ?? false;
            var statusStr = GetString(r, "Status") ?? string.Empty;
            var status = TableStatus.Available;
            if (!string.IsNullOrWhiteSpace(statusStr) && Enum.TryParse<TableStatus>(statusStr, true, out var parsedStatus))
                status = parsedStatus;

            list.Add(new TableSeedModel
            {
                Id = id.Value,
                Name = name,
                Capacity = capacity,
                SequenceNumber = seq.Value,
                IsPrepared = isPrepared,
                ActiveSince = activeSince,
                IsUsed = isUsed,
                IsDeleted = isDeleted,
                Status = status,
                AreaId = areaId.Value
            });
        }

        return list;
    }

    private List<MenuItemSeedModel> MapMenuItem(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<MenuItemSeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var id = GetGuid(r, "Id");
            if (id == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing or invalid Id");
                continue;
            }

            var name = GetString(r, "Name") ?? string.Empty;
            var description = GetString(r, "Description", "Desc");
            var price = GetDecimal(r, "Price") ?? 0m;
            var isUsed = GetBool(r, "IsUsed") ?? true;
            var isDeleted = GetBool(r, "IsDeleted") ?? false;
            var menuCategoryId = GetGuid(r, "MenuCategoryId", "MenuCategory");
            var subCategoryId = GetGuid(r, "SubCategoryId", "SubCategory");
            var seq = GetInt(r, "SequenceNumber", "Sequence", "Seq");
            if (seq == null)
            {
                seq = excelRow - sheet.Dimension.Start.Row;
                report.Warnings.Add($"Defaulted SequenceNumber in {report.SheetName} row {excelRow} to {seq}");
            }

            list.Add(new MenuItemSeedModel
            {
                Id = id.Value,
                Name = name,
                Description = description,
                Price = price,
                IsUsed = isUsed,
                IsDeleted = isDeleted,
                MenuCategoryId = menuCategoryId,
                SubCategoryId = subCategoryId,
                SequenceNumber = seq.Value
            });
        }

        return list;
    }

    private List<IngredientTagRelSeedModel> MapIngredientTagRel(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<IngredientTagRelSeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var ingredientId = GetGuid(r, "IngredientId", "Ingredient");
            var tagId = GetGuid(r, "TagId", "Tag");
            if (ingredientId == null || tagId == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing IngredientId or TagId");
                continue;
            }

            list.Add(new IngredientTagRelSeedModel
            {
                IngredientId = ingredientId.Value,
                TagId = tagId.Value
            });
        }

        return list;
    }

    private List<IngredientAllergenRelSeedModel> MapIngredientAllergenRel(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<IngredientAllergenRelSeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var ingredientId = GetGuid(r, "IngredientId", "Ingredient");
            var allergenId = GetGuid(r, "AllergenId", "Allergen");
            if (ingredientId == null || allergenId == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing IngredientId or AllergenId");
                continue;
            }

            list.Add(new IngredientAllergenRelSeedModel
            {
                IngredientId = ingredientId.Value,
                AllergenId = allergenId.Value
            });
        }

        return list;
    }

    private List<MenuItemIngredientRelSeedModel> MapMenuItemIngredientRel(ExcelWorksheet sheet, SheetReport report)
    {
        var rows = ReadRowsAsDictionary(sheet);
        var list = new List<MenuItemIngredientRelSeedModel>();
        foreach (var row in rows)
        {
            var r = row.Values;
            var excelRow = row.RowNumber;
            var menuItemId = GetGuid(r, "MenuItemId", "MenuItem");
            var ingredientId = GetGuid(r, "IngredientId", "Ingredient");
            if (menuItemId == null || ingredientId == null)
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped row {excelRow} in {report.SheetName}: missing MenuItemId or IngredientId");
                continue;
            }

            list.Add(new MenuItemIngredientRelSeedModel
            {
                MenuItemId = menuItemId.Value,
                IngredientId = ingredientId.Value
            });
        }

        return list;
    }

    // Reporting types
    public class ExcelExportReport
    {
        public List<SheetReport> Sheets { get; set; } = new();
    }

    public class SheetReport
    {
        public string SheetName { get; set; } = string.Empty;
        public int ExportedCount { get; set; }
        public int SkippedCount { get; set; }
        public List<string> Warnings { get; set; } = new();
    }
}
