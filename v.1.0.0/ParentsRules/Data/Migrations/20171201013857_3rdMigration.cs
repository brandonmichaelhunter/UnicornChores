using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ParentsRules.Data.Migrations
{
    public partial class _3rdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AccountUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AccountUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "AccountUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentUserID",
                table: "AccountUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AccountUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AccountUsers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "AccountUsers");

            migrationBuilder.DropColumn(
                name: "ParentUserID",
                table: "AccountUsers");
        }
    }
}
