using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facturi.Migrations
{
    /// <inheritdoc />
    public partial class AdaugaStatusPlata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstePlatita",
                table: "Facturi",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstePlatita",
                table: "Facturi");
        }
    }
}
