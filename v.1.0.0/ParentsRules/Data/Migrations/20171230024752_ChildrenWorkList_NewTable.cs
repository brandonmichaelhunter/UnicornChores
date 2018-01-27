using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ParentsRules.Data.Migrations
{
    public partial class ChildrenWorkList_NewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChildrenWork",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Chore = table.Column<string>(nullable: false),
                    ChoreCompleted = table.Column<bool>(nullable: false),
                    DateChoreCompleted = table.Column<DateTime>(nullable: true),
                    DateDue = table.Column<DateTime>(nullable: true),
                    DollarAmount = table.Column<float>(nullable: false),
                    Friday = table.Column<bool>(nullable: false),
                    Monday = table.Column<bool>(nullable: false),
                    ParentID = table.Column<string>(nullable: true),
                    ParentVerified = table.Column<bool>(nullable: false),
                    ParentVerifiedDate = table.Column<DateTime>(nullable: true),
                    RoomID = table.Column<int>(nullable: false),
                    Saturday = table.Column<bool>(nullable: false),
                    Sunday = table.Column<bool>(nullable: false),
                    Thursday = table.Column<bool>(nullable: false),
                    Tuesday = table.Column<bool>(nullable: false),
                    UserID = table.Column<string>(nullable: false),
                    Wednesday = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildrenWork", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildrenWork");
        }
    }
}
