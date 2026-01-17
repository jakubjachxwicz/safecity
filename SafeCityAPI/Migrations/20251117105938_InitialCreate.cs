using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bijedrona_API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DroneReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                                         Latitude = table.Column<double>(type: "double precision", nullable: false),
                                         Longitude = table.Column<double>(type: "double precision", nullable: false),
                                         Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                                         ReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                                         UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DroneReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                                         IsBanned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                                         PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                                         Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "user"),
                                         Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DroneReports_IpAddress",
                table: "DroneReports",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DroneReports");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
