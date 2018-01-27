using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ParentsRules.Data.Migrations
{
    public partial class _12292017_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateDue",
                table: "UserChores",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<bool>(
                name: "ChoreCompleted",
                table: "UserChores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateChoreCompleted",
                table: "UserChores",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ParentVerified",
                table: "UserChores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ParentVerifiedDate",
                table: "UserChores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChoreCompleted",
                table: "UserChores");

            migrationBuilder.DropColumn(
                name: "DateChoreCompleted",
                table: "UserChores");

            migrationBuilder.DropColumn(
                name: "ParentVerified",
                table: "UserChores");

            migrationBuilder.DropColumn(
                name: "ParentVerifiedDate",
                table: "UserChores");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateDue",
                table: "UserChores",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
