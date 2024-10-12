﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evently.Modules.Ticketing.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class renamed_outbox_tbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_outbox_message",
                schema: "ticketing",
                table: "outbox_message");

            migrationBuilder.RenameTable(
                name: "outbox_message",
                schema: "ticketing",
                newName: "outbox_messages",
                newSchema: "ticketing");

            migrationBuilder.AddPrimaryKey(
                name: "pk_outbox_messages",
                schema: "ticketing",
                table: "outbox_messages",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_outbox_messages",
                schema: "ticketing",
                table: "outbox_messages");

            migrationBuilder.RenameTable(
                name: "outbox_messages",
                schema: "ticketing",
                newName: "outbox_message",
                newSchema: "ticketing");

            migrationBuilder.AddPrimaryKey(
                name: "pk_outbox_message",
                schema: "ticketing",
                table: "outbox_message",
                column: "id");
        }
    }
}
