using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ParentsRules.Data.Migrations
{
    public partial class StartOfWeekDate_StartOfWeekDateDisplay_New_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartOfWeekDate",
                table: "CompletedChildrenWork",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StartOfWeekDateDisplay",
                table: "CompletedChildrenWork",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartOfWeekDate",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StartOfWeekDateDisplay",
                table: "ChildrenWork",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartOfWeekDate",
                table: "CompletedChildrenWork");

            migrationBuilder.DropColumn(
                name: "StartOfWeekDateDisplay",
                table: "CompletedChildrenWork");

            migrationBuilder.DropColumn(
                name: "StartOfWeekDate",
                table: "ChildrenWork");

            migrationBuilder.DropColumn(
                name: "StartOfWeekDateDisplay",
                table: "ChildrenWork");
        }
    }
}
