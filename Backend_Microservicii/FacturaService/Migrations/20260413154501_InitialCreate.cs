using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturaService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Facturi",
                columns: table => new
                {
                    NrFactura = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataFactura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IdClient = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturi", x => x.NrFactura);
                });

            migrationBuilder.CreateTable(
                name: "ProduseFactura",
                columns: table => new
                {
                    IdProdusFactura = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cantitate = table.Column<int>(type: "INTEGER", nullable: false),
                    Pret = table.Column<decimal>(type: "TEXT", nullable: false),
                    NrFactura = table.Column<int>(type: "INTEGER", nullable: false),
                    IdProdus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProduseFactura", x => x.IdProdusFactura);
                    table.ForeignKey(
                        name: "FK_ProduseFactura_Facturi_NrFactura",
                        column: x => x.NrFactura,
                        principalTable: "Facturi",
                        principalColumn: "NrFactura",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProduseFactura_NrFactura",
                table: "ProduseFactura",
                column: "NrFactura");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProduseFactura");

            migrationBuilder.DropTable(
                name: "Facturi");
        }
    }
}
