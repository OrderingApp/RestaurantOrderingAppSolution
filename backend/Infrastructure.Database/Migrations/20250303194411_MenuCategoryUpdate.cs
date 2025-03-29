using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class MenuCategoryUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_MenuCategories_MenuCategoryId",
                table: "SubCategories"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_MenuCategories_MenuCategoryId",
                table: "SubCategories",
                column: "MenuCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_MenuCategories_MenuCategoryId",
                table: "SubCategories"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_MenuCategories_MenuCategoryId",
                table: "SubCategories",
                column: "MenuCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
