using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTAuthExample.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdatesInAttributs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "refreshTokenExpiryTime",
                table: "Users",
                newName: "RefreshTokenExpiryTime");

            migrationBuilder.RenameColumn(
                name: "refreshToken",
                table: "Users",
                newName: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "passwordHashed",
                table: "Users",
                newName: "PasswordHashed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiryTime",
                table: "Users",
                newName: "refreshTokenExpiryTime");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Users",
                newName: "refreshToken");

            migrationBuilder.RenameColumn(
                name: "PasswordHashed",
                table: "Users",
                newName: "passwordHashed");
        }
    }
}
