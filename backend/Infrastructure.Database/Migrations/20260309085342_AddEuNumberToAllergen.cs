using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEuNumberToAllergen : Migration
    {
        // The 14 mandatory EU allergens per Annex II of the FIC Regulation (EU) No 1169/2011.
        // Deterministic GUIDs ensure the same allergen always gets the same ID across all environments.
        private static readonly (string id, int euNumber, string name)[] EuAllergens =
        [
            ("a1000001-0000-0000-0000-000000000001", 1,  "Cereals containing gluten"),
            ("a1000001-0000-0000-0000-000000000002", 2,  "Crustaceans"),
            ("a1000001-0000-0000-0000-000000000003", 3,  "Eggs"),
            ("a1000001-0000-0000-0000-000000000004", 4,  "Fish"),
            ("a1000001-0000-0000-0000-000000000005", 5,  "Peanuts"),
            ("a1000001-0000-0000-0000-000000000006", 6,  "Soybeans"),
            ("a1000001-0000-0000-0000-000000000007", 7,  "Milk"),
            ("a1000001-0000-0000-0000-000000000008", 8,  "Nuts"),
            ("a1000001-0000-0000-0000-000000000009", 9,  "Celery"),
            ("a1000001-0000-0000-0000-000000000010", 10, "Mustard"),
            ("a1000001-0000-0000-0000-000000000011", 11, "Sesame seeds"),
            ("a1000001-0000-0000-0000-000000000012", 12, "Sulphur dioxide and sulphites"),
            ("a1000001-0000-0000-0000-000000000013", 13, "Lupin"),
            ("a1000001-0000-0000-0000-000000000014", 14, "Molluscs"),
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EuNumber",
                table: "Allergens",
                type: "INTEGER",
                nullable: true);

            // Seed the 14 mandatory EU allergens.
            foreach (var (id, euNumber, name) in EuAllergens)
            {
                migrationBuilder.InsertData(
                    table: "Allergens",
                    columns: ["Id", "Name", "EuNumber", "IsUsed", "IsDeleted"],
                    values: [id, name, euNumber, true, false]
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the seeded EU allergens before dropping the column.
            foreach (var (id, _, _) in EuAllergens)
            {
                migrationBuilder.DeleteData(
                    table: "Allergens",
                    keyColumn: "Id",
                    keyValue: id
                );
            }

            migrationBuilder.DropColumn(
                name: "EuNumber",
                table: "Allergens");
        }
    }
}
