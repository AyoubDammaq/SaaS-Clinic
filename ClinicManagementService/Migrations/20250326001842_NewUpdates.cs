using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagementService.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodePostal",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "Pays",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "Téléphone",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "Ville",
                table: "Cliniques");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Cliniques",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Adresse",
                table: "Cliniques",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "NumeroTelephone",
                table: "Cliniques",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Cliniques",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroTelephone",
                table: "Cliniques");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Cliniques");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Cliniques",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Adresse",
                table: "Cliniques",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "CodePostal",
                table: "Cliniques",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Pays",
                table: "Cliniques",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Téléphone",
                table: "Cliniques",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ville",
                table: "Cliniques",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
