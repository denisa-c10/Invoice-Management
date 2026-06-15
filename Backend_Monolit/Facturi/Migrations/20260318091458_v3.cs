using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facturi.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProduseFactura_Facturi_NrFactura",
                table: "ProduseFactura");

            migrationBuilder.DropForeignKey(
                name: "FK_ProduseFactura2_FacturaModel2_NrFactura",
                table: "ProduseFactura2");

            migrationBuilder.DropTable(
                name: "FacturaModel2");

            migrationBuilder.CreateTable(
                name: "FacturaModel",
                columns: table => new
                {
                    NrFactura = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataFactura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClientFacturaIdClient = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_FacturaModel_ClientFacturaIdClient",
                table: "FacturaModel",
                column: "ClientFacturaIdClient");

            migrationBuilder.AddForeignKey(
                name: "FK_ProduseFactura_FacturaModel_NrFactura",
                table: "ProduseFactura",
                column: "NrFactura",
                principalTable: "FacturaModel",
                principalColumn: "NrFactura",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProduseFactura2_Facturi_NrFactura",
                table: "ProduseFactura2",
                column: "NrFactura",
                principalTable: "Facturi",
                principalColumn: "NrFactura",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProduseFactura_FacturaModel_NrFactura",
                table: "ProduseFactura");

            migrationBuilder.DropForeignKey(
                name: "FK_ProduseFactura2_Facturi_NrFactura",
                table: "ProduseFactura2");

            migrationBuilder.DropTable(
                name: "FacturaModel");

            migrationBuilder.CreateTable(
                name: "FacturaModel2",
                columns: table => new
                {
                    NrFactura = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientFacturaIdClient = table.Column<int>(type: "INTEGER", nullable: false),
                    DataFactura = table.Column<DateTime>(type: "TEXT", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_FacturaModel2_ClientFacturaIdClient",
                table: "FacturaModel2",
                column: "ClientFacturaIdClient");

            migrationBuilder.AddForeignKey(
                name: "FK_ProduseFactura_Facturi_NrFactura",
                table: "ProduseFactura",
                column: "NrFactura",
                principalTable: "Facturi",
                principalColumn: "NrFactura",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProduseFactura2_FacturaModel2_NrFactura",
                table: "ProduseFactura2",
                column: "NrFactura",
                principalTable: "FacturaModel2",
                principalColumn: "NrFactura",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
