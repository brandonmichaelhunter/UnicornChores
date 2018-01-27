using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ParentsRules.Data.Migrations
{
    public partial class ChildComplete_And_TotalEarned_NewColumns_To_ChildrenWork_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ChildCompleted",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "TotalEarned",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChildCompleted",
                table: "ChildrenWork");

            migrationBuilder.DropColumn(
                name: "TotalEarned",
                table: "ChildrenWork");
        }
    }
}
