using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DossierMedicalId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Sexe",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DossiersMedicaux",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaladiesChroniques = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicamentsActuels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AntécédentsFamiliaux = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AntécédentsPersonnels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExamensComplementaires = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TraitementsEnCours = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AutresInformations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DossiersMedicaux", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DossiersMedicaux_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DossiersMedicaux_PatientId",
                table: "DossiersMedicaux",
                column: "PatientId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DossiersMedicaux");

            migrationBuilder.DropColumn(
                name: "Sexe",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "DossierMedicalId",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
