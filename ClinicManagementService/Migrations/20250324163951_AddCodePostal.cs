using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddCodePostal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodePsotal",
                table: "Cliniques",
                newName: "CodePostal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodePostal",
                table: "Cliniques",
                newName: "CodePsotal");
        }
    }
}
