using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteOS.Web.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Computers",
                columns: table => new
                {
                    Address = table.Column<Guid>(type: "TEXT", nullable: false),
                    X = table.Column<float>(type: "REAL", nullable: true),
                    Y = table.Column<float>(type: "REAL", nullable: true),
                    Z = table.Column<float>(type: "REAL", nullable: true),
                    Facing = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Computers", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "World",
                columns: table => new
                {
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false),
                    Z = table.Column<int>(type: "INTEGER", nullable: false),
                    Hardness = table.Column<float>(type: "REAL", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    Argb = table.Column<int>(type: "INTEGER", nullable: true),
                    HarvestLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    HarvestTool = table.Column<string>(type: "TEXT", nullable: true),
                    Meta = table.Column<int>(type: "INTEGER", nullable: true),
                    ModName = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Props = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_World", x => new { x.X, x.Y, x.Z });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Computers");

            migrationBuilder.DropTable(
                name: "World");
        }
    }
}
