using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddingPaymentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInformation_Orders_OrderId",
                table: "CustomerInformation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerInformation",
                table: "CustomerInformation");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "CustomerInformation",
                newName: "CustomerInformations");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerInformation_OrderId",
                table: "CustomerInformations",
                newName: "IX_CustomerInformations_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerInformations",
                table: "CustomerInformations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaymentStatus = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInformations_Orders_OrderId",
                table: "CustomerInformations",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInformations_Orders_OrderId",
                table: "CustomerInformations");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerInformations",
                table: "CustomerInformations");

            migrationBuilder.RenameTable(
                name: "CustomerInformations",
                newName: "CustomerInformation");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerInformations_OrderId",
                table: "CustomerInformation",
                newName: "IX_CustomerInformation_OrderId");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerInformation",
                table: "CustomerInformation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInformation_Orders_OrderId",
                table: "CustomerInformation",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
