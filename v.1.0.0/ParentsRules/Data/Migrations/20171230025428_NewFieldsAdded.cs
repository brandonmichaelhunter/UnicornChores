using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ParentsRules.Data.Migrations
{
    public partial class NewFieldsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublishStatus",
                table: "UserChores",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChoreID",
                table: "ChildrenWork",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishStatus",
                table: "UserChores");

            migrationBuilder.DropColumn(
                name: "ChoreID",
                table: "ChildrenWork");
        }
    }
}
