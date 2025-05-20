using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClinicModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodePostal",
                table: "Cliniques",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Cliniques",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pays",
                table: "Cliniques",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SiteWeb",
                table: "Cliniques",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "Cliniques",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeClinique",
                table: "Cliniques",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Ville",
                table: "Cliniques",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodePostal",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "Pays",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "SiteWeb",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "TypeClinique",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "Ville",
                table: "Cliniques");
        }
    }
}
