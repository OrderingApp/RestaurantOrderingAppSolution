using System.Threading.Tasks;

namespace Infrastructure.Database.Seeder.Seeders;

public interface ISeeder
{
    Task SeedAsync();
}
