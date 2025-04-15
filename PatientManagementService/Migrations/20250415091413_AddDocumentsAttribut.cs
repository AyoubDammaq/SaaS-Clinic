using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentsAttribut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contenu = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DossierMedicalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_DossiersMedicaux_DossierMedicalId",
                        column: x => x.DossierMedicalId,
                        principalTable: "DossiersMedicaux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Document_DossierMedicalId",
                table: "Document",
                column: "DossierMedicalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Document");
        }
    }
}
