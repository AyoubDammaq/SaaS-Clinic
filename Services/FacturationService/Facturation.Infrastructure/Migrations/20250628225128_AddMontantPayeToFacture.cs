using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facturation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMontantPayeToFacture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MontantPaye",
                table: "Factures",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontantPaye",
                table: "Factures");
        }
    }
}
