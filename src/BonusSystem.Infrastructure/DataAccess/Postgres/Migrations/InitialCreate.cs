using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Migrations
{
    /// <summary>
    /// Initial database schema creation
    /// </summary>
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create schema
            migrationBuilder.EnsureSchema(
                name: "bonus");
            
            // Create users table
            migrationBuilder.CreateTable(
                name: "users",
                schema: "bonus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    BonusBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            // Create companies table
            migrationBuilder.CreateTable(
                name: "companies",
                schema: "bonus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: false),
                    BonusBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    OriginalBonusBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                });

            // Create stores table
            migrationBuilder.CreateTable(
                name: "stores",
                schema: "bonus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stores_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "bonus",
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create transactions table
            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "bonus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transactions_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "bonus",
                        principalTable: "companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_transactions_stores_StoreId",
                        column: x => x.StoreId,
                        principalSchema: "bonus",
                        principalTable: "stores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_transactions_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "bonus",
                        principalTable: "users",
                        principalColumn: "Id");
                });

            // Create notifications table
            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "bonus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notifications_users_RecipientId",
                        column: x => x.RecipientId,
                        principalSchema: "bonus",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create store_sellers table
            migrationBuilder.CreateTable(
                name: "store_sellers",
                schema: "bonus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_sellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_sellers_stores_StoreId",
                        column: x => x.StoreId,
                        principalSchema: "bonus",
                        principalTable: "stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_store_sellers_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "bonus",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_companies_Name",
                schema: "bonus",
                table: "companies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_companies_Status",
                schema: "bonus",
                table: "companies",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_CreatedAt",
                schema: "bonus",
                table: "notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_IsRead",
                schema: "bonus",
                table: "notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_RecipientId",
                schema: "bonus",
                table: "notifications",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_Type",
                schema: "bonus",
                table: "notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_store_sellers_StoreId_UserId",
                schema: "bonus",
                table: "store_sellers",
                columns: new[] { "StoreId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_sellers_UserId",
                schema: "bonus",
                table: "store_sellers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_stores_CompanyId",
                schema: "bonus",
                table: "stores",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_stores_Name",
                schema: "bonus",
                table: "stores",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_stores_Status",
                schema: "bonus",
                table: "stores",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_CompanyId",
                schema: "bonus",
                table: "transactions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_Status",
                schema: "bonus",
                table: "transactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_StoreId",
                schema: "bonus",
                table: "transactions",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_Timestamp",
                schema: "bonus",
                table: "transactions",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_Type",
                schema: "bonus",
                table: "transactions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_UserId",
                schema: "bonus",
                table: "transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                schema: "bonus",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Role",
                schema: "bonus",
                table: "users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                schema: "bonus",
                table: "users",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications",
                schema: "bonus");

            migrationBuilder.DropTable(
                name: "store_sellers",
                schema: "bonus");

            migrationBuilder.DropTable(
                name: "transactions",
                schema: "bonus");

            migrationBuilder.DropTable(
                name: "stores",
                schema: "bonus");

            migrationBuilder.DropTable(
                name: "users",
                schema: "bonus");

            migrationBuilder.DropTable(
                name: "companies",
                schema: "bonus");
        }
    }
}