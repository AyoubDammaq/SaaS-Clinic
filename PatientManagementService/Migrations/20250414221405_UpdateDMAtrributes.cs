using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientManagementService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDMAtrributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutresInformations",
                table: "DossiersMedicaux");

            migrationBuilder.DropColumn(
                name: "ExamensComplementaires",
                table: "DossiersMedicaux");

            migrationBuilder.DropColumn(
                name: "Observations",
                table: "DossiersMedicaux");

            migrationBuilder.RenameColumn(
                name: "TraitementsEnCours",
                table: "DossiersMedicaux",
                newName: "GroupeSanguin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GroupeSanguin",
                table: "DossiersMedicaux",
                newName: "TraitementsEnCours");

            migrationBuilder.AddColumn<string>(
                name: "AutresInformations",
                table: "DossiersMedicaux",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExamensComplementaires",
                table: "DossiersMedicaux",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Observations",
                table: "DossiersMedicaux",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
