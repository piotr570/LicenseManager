using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenseManager.Licenses.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Vendor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Terms_MaxUsers = table.Column<int>(type: "integer", nullable: true),
                    Terms_UsageLimit = table.Column<int>(type: "integer", nullable: true),
                    Terms_Type = table.Column<int>(type: "integer", nullable: false),
                    Terms_Mode = table.Column<int>(type: "integer", nullable: false),
                    Terms_ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Terms_IsRenewable = table.Column<bool>(type: "boolean", nullable: false),
                    Terms_RenewalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    IsPaymentValid = table.Column<bool>(type: "boolean", nullable: true),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LicenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastInvokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Licenses_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "Licenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_LicenseId",
                table: "Assignments",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_UserId",
                table: "Assignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_Key",
                table: "Licenses",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Licenses");
        }
    }
}
