using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notif.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Preferences");

            migrationBuilder.DropColumn(
                name: "DateEnvoi",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "UtilisateurId",
                table: "Notifications",
                newName: "RecipientId");

            migrationBuilder.RenameColumn(
                name: "Titre",
                table: "Notifications",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Statut",
                table: "Notifications",
                newName: "RecipientType");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Notifications",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "DateLecture",
                table: "Notifications",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "Canaux",
                table: "Notifications",
                newName: "Content");

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "Notifications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredChannels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotificationSettings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: "fr"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ScheduledAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Notifications",
                newName: "Titre");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Notifications",
                newName: "DateLecture");

            migrationBuilder.RenameColumn(
                name: "RecipientType",
                table: "Notifications",
                newName: "Statut");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Notifications",
                newName: "UtilisateurId");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Notifications",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notifications",
                newName: "Canaux");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnvoi",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Preferences",
                columns: table => new
                {
                    UtilisateurId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Langue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecevoirEmail = table.Column<bool>(type: "bit", nullable: false),
                    RecevoirInApp = table.Column<bool>(type: "bit", nullable: false),
                    RecevoirSms = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => x.UtilisateurId);
                });
        }
    }
}
