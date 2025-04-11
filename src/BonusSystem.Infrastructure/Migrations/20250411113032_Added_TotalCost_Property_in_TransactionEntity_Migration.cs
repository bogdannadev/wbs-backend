using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonusSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_TotalCost_Property_in_TransactionEntity_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                schema: "bonus",
                table: "transactions",
                newName: "TotalCost");

            migrationBuilder.AddColumn<decimal>(
                name: "BonusAmount",
                schema: "bonus",
                table: "transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusAmount",
                schema: "bonus",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "TotalCost",
                schema: "bonus",
                table: "transactions",
                newName: "Amount");
        }
    }
}
