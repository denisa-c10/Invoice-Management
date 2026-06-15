using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facturi.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clienti",
                columns: table => new
                {
                    IdClient = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeClient = table.Column<string>(type: "TEXT", nullable: false),
                    Adresa = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clienti", x => x.IdClient);
                });

            migrationBuilder.CreateTable(
                name: "Produse",
                columns: table => new
                {
                    IdProdus = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeProdus = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produse", x => x.IdProdus);
                });

            migrationBuilder.CreateTable(
                name: "FacturaModel2",
                columns: table => new
                {
                    NrFactura = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataFactura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClientFacturaIdClient = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaModel2", x => x.NrFactura);
                    table.ForeignKey(
                        name: "FK_FacturaModel2_Clienti_ClientFacturaIdClient",
                        column: x => x.ClientFacturaIdClient,
                        principalTable: "Clienti",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facturi",
                columns: table => new
                {
                    NrFactura = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataFactura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClientFacturaIdClient = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturi", x => x.NrFactura);
                    table.ForeignKey(
                        name: "FK_Facturi_Clienti_ClientFacturaIdClient",
                        column: x => x.ClientFacturaIdClient,
                        principalTable: "Clienti",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProduseFactura2",
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
                    table.PrimaryKey("PK_ProduseFactura2", x => x.IdProdusFactura);
                    table.ForeignKey(
                        name: "FK_ProduseFactura2_FacturaModel2_NrFactura",
                        column: x => x.NrFactura,
                        principalTable: "FacturaModel2",
                        principalColumn: "NrFactura",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProduseFactura2_Produse_IdProdus",
                        column: x => x.IdProdus,
                        principalTable: "Produse",
                        principalColumn: "IdProdus",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProduseFactura",
                columns: table => new
                {
                    IdProdusFactura = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cantitate = table.Column<int>(type: "INTEGER", nullable: false),
                    Pret = table.Column<double>(type: "REAL", nullable: false),
                    NrFactura = table.Column<int>(type: "INTEGER", nullable: false),
                    IdProdus = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeProdus = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "IX_FacturaModel2_ClientFacturaIdClient",
                table: "FacturaModel2",
                column: "ClientFacturaIdClient");

            migrationBuilder.CreateIndex(
                name: "IX_Facturi_ClientFacturaIdClient",
                table: "Facturi",
                column: "ClientFacturaIdClient");

            migrationBuilder.CreateIndex(
                name: "IX_ProduseFactura_NrFactura",
                table: "ProduseFactura",
                column: "NrFactura");

            migrationBuilder.CreateIndex(
                name: "IX_ProduseFactura2_IdProdus",
                table: "ProduseFactura2",
                column: "IdProdus");

            migrationBuilder.CreateIndex(
                name: "IX_ProduseFactura2_NrFactura",
                table: "ProduseFactura2",
                column: "NrFactura");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProduseFactura");

            migrationBuilder.DropTable(
                name: "ProduseFactura2");

            migrationBuilder.DropTable(
                name: "Facturi");

            migrationBuilder.DropTable(
                name: "FacturaModel2");

            migrationBuilder.DropTable(
                name: "Produse");

            migrationBuilder.DropTable(
                name: "Clienti");
        }
    }
}
