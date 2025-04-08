using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Medecins");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Medecins");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Medecins");
        }
    }
}
