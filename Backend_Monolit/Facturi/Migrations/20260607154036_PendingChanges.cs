using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facturi.Migrations
{
    /// <inheritdoc />
    public partial class PendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProduseFactura");

            migrationBuilder.DropTable(
                name: "FacturaModel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacturaModel",
                columns: table => new
                {
                    NrFactura = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientFacturaIdClient = table.Column<int>(type: "INTEGER", nullable: false),
                    DataFactura = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaModel", x => x.NrFactura);
                    table.ForeignKey(
                        name: "FK_FacturaModel_Clienti_ClientFacturaIdClient",
                        column: x => x.ClientFacturaIdClient,
                        principalTable: "Clienti",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProduseFactura",
                columns: table => new
                {
                    IdProdusFactura = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NrFactura = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantitate = table.Column<int>(type: "INTEGER", nullable: false),
                    IdProdus = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeProdus = table.Column<string>(type: "TEXT", nullable: false),
                    Pret = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProduseFactura", x => x.IdProdusFactura);
                    table.ForeignKey(
                        name: "FK_ProduseFactura_FacturaModel_NrFactura",
                        column: x => x.NrFactura,
                        principalTable: "FacturaModel",
                        principalColumn: "NrFactura",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacturaModel_ClientFacturaIdClient",
                table: "FacturaModel",
                column: "ClientFacturaIdClient");

            migrationBuilder.CreateIndex(
                name: "IX_ProduseFactura_NrFactura",
                table: "ProduseFactura",
                column: "NrFactura");
        }
    }
}
