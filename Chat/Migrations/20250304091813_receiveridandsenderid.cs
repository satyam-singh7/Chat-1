﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Migrations
{
    /// <inheritdoc />
    public partial class receiveridandsenderid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceiverId",
                table: "ChatMessages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "ChatMessages");
        }
    }
}
