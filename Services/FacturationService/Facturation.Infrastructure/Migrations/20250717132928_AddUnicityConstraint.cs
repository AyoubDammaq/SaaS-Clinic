using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facturation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnicityConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TarifsConsultation_ClinicId_ConsultationType",
                table: "TarifsConsultation",
                columns: new[] { "ClinicId", "ConsultationType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TarifsConsultation_ClinicId_ConsultationType",
                table: "TarifsConsultation");
        }
    }
}
