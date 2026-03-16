using System.Collections.Generic;

namespace Infrastructure.Database.Seeder.Seeders;

public class RelationalSeedReport
{
    public int AddedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<string> Warnings { get; set; } = new List<string>();
}
