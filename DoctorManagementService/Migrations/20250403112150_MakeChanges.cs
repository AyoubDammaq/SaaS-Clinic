using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorManagementService.Migrations
{
    /// <inheritdoc />
    public partial class MakeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prenom",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prenom",
                table: "Medecins");
        }
    }
}
