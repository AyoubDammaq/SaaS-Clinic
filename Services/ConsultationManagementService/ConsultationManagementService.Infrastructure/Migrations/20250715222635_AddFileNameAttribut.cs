using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultationManagementService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameAttribut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "DocumentsMedicaux",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "DocumentsMedicaux");
        }
    }
}
