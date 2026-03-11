using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Database.Seeder;

public class SeedDataReader
{
    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public SeedDataReader()
    {
        // Accept enum values as strings when deserializing JSON files
        _options.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<List<T>> ReadAsync<T>(string path)
    {
        try
        {
            if (!File.Exists(path))
                return new List<T>();

            using var stream = File.OpenRead(path);
            var result = await JsonSerializer.DeserializeAsync<List<T>>(stream, _options);
            return result ?? new List<T>();
        }
        catch
        {
            // Swallow exceptions for now - caller will handle missing/invalid data as empty set
            return new List<T>();
        }
    }
}
