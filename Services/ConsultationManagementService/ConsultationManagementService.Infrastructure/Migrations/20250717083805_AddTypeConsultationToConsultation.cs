using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultationManagementService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeConsultationToConsultation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Consultations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Consultations");
        }
    }
}
