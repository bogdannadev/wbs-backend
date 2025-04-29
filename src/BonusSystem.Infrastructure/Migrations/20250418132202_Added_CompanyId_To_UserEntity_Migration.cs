using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonusSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_CompanyId_To_UserEntity_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "bonus",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_CompanyId",
                schema: "bonus",
                table: "users",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_companies_CompanyId",
                schema: "bonus",
                table: "users",
                column: "CompanyId",
                principalSchema: "bonus",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_companies_CompanyId",
                schema: "bonus",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_CompanyId",
                schema: "bonus",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "bonus",
                table: "users");
        }
    }
}
