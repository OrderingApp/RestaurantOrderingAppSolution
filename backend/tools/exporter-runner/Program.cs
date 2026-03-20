using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Infrastructure.Database.Seeder.Tools;

Console.WriteLine("Exporter runner starting...");
if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run --project tools/exporter-runner/ExporterRunner.csproj -- <excelPath> [outputFolder]");
    return 1;
}

var excelPath = args[0];
var output = args.Length > 1 ? args[1] : Path.Combine(Directory.GetCurrentDirectory(), "Seeder", "SeedData");

try
{
    var exporter = new ExcelToJsonExporter();
    var report = await exporter.ExportAsync(excelPath, output);
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(report, options);
    Console.WriteLine(json);

    Console.WriteLine($"JSON files written to: {output}");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Export failed: {ex.Message}");
    return 2;
}