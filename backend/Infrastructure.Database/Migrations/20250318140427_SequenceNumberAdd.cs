using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SequenceNumberAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Order",
                table: "MenuCategories",
                newName: "SequenceNumber"
            );

            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "MenuItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "SequenceNumber", table: "MenuItems");

            migrationBuilder.RenameColumn(
                name: "SequenceNumber",
                table: "MenuCategories",
                newName: "Order"
            );
        }
    }
}
