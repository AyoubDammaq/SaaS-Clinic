﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthentificationService.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "refreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "refreshTokenExpiryTime",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "refreshTokenExpiryTime",
                table: "Users");
        }
    }
}
