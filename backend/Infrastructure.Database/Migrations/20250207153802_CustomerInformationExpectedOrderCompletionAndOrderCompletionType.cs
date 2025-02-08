using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CustomerInformationExpectedOrderCompletionAndOrderCompletionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedOrderCompletion",
                table: "CustomerInformations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderCompletionType",
                table: "CustomerInformations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedOrderCompletion",
                table: "CustomerInformations");

            migrationBuilder.DropColumn(
                name: "OrderCompletionType",
                table: "CustomerInformations");
        }
    }
}
