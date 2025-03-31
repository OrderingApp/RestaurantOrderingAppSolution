using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameCustomerInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInformations_Orders_OrderId",
                table: "CustomerInformations"
            );

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerInformations",
                table: "CustomerInformations"
            );

            migrationBuilder.RenameTable(
                name: "CustomerInformations",
                newName: "CustomerInformation"
            );

            migrationBuilder.RenameIndex(
                name: "IX_CustomerInformations_OrderId",
                table: "CustomerInformation",
                newName: "IX_CustomerInformation_OrderId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerInformation",
                table: "CustomerInformation",
                column: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInformation_Orders_OrderId",
                table: "CustomerInformation",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInformation_Orders_OrderId",
                table: "CustomerInformation"
            );

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerInformation",
                table: "CustomerInformation"
            );

            migrationBuilder.RenameTable(
                name: "CustomerInformation",
                newName: "CustomerInformations"
            );

            migrationBuilder.RenameIndex(
                name: "IX_CustomerInformation_OrderId",
                table: "CustomerInformations",
                newName: "IX_CustomerInformations_OrderId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerInformations",
                table: "CustomerInformations",
                column: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInformations_Orders_OrderId",
                table: "CustomerInformations",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
