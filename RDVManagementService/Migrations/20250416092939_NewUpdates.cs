using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RDVManagementService.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Motif",
                table: "RendezVous");

            migrationBuilder.AlterColumn<string>(
                name: "Statut",
                table: "RendezVous",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Commentaire",
                table: "RendezVous",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "RendezVous",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Commentaire",
                table: "RendezVous");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "RendezVous");

            migrationBuilder.AlterColumn<int>(
                name: "Statut",
                table: "RendezVous",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Motif",
                table: "RendezVous",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
