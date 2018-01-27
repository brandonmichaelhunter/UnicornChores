using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ParentsRules.Data.Migrations
{
    public partial class UpdateChildrenWorkTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FridayCompleted",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MondayCompleted",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SaturdayCompleted",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SundayCompleted",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ThursdayCompleted",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TuesdayCompleted",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WednesdayCompleted",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FridayCompleted",
                table: "ChildrenWork");

            migrationBuilder.DropColumn(
                name: "MondayCompleted",
                table: "ChildrenWork");

            migrationBuilder.DropColumn(
                name: "SaturdayCompleted",
                table: "ChildrenWork");

            migrationBuilder.DropColumn(
                name: "SundayCompleted",
                table: "ChildrenWork");

            migrationBuilder.DropColumn(
                name: "ThursdayCompleted",
                table: "ChildrenWork");

            migrationBuilder.DropColumn(
                name: "TuesdayCompleted",
                table: "ChildrenWork");

            migrationBuilder.DropColumn(
                name: "WednesdayCompleted",
                table: "ChildrenWork");
        }
    }
}
