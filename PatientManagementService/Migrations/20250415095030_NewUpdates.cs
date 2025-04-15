using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientManagementService.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_DossiersMedicaux_DossierMedicalId",
                table: "Document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "Documents");

            migrationBuilder.RenameIndex(
                name: "IX_Document_DossierMedicalId",
                table: "Documents",
                newName: "IX_Documents_DossierMedicalId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DossiersMedicaux_DossierMedicalId",
                table: "Documents",
                column: "DossierMedicalId",
                principalTable: "DossiersMedicaux",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DossiersMedicaux_DossierMedicalId",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Document");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_DossierMedicalId",
                table: "Document",
                newName: "IX_Document_DossierMedicalId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_DossiersMedicaux_DossierMedicalId",
                table: "Document",
                column: "DossierMedicalId",
                principalTable: "DossiersMedicaux",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
