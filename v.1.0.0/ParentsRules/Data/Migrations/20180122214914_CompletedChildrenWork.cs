using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ParentsRules.Data.Migrations
{
    public partial class CompletedChildrenWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChildrenViewModel",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    ConfirmPassword = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    MiddleName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(maxLength: 100, nullable: false),
                    Username = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildrenViewModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CompletedChildrenWork",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChildCompleted = table.Column<bool>(nullable: false),
                    Chore = table.Column<string>(nullable: true),
                    ChoreCompleted = table.Column<bool>(nullable: false),
                    ChoreID = table.Column<int>(nullable: false),
                    DateChoreCompleted = table.Column<DateTime>(nullable: true),
                    DateDue = table.Column<DateTime>(nullable: true),
                    DollarAmount = table.Column<float>(nullable: false),
                    Friday = table.Column<bool>(nullable: false),
                    FridayCompleted = table.Column<bool>(nullable: false),
                    Monday = table.Column<bool>(nullable: false),
                    MondayCompleted = table.Column<bool>(nullable: false),
                    ParentID = table.Column<string>(nullable: true),
                    ParentVerified = table.Column<bool>(nullable: false),
                    ParentVerifiedDate = table.Column<DateTime>(nullable: true),
                    RoomID = table.Column<int>(nullable: false),
                    Saturday = table.Column<bool>(nullable: false),
                    SaturdayCompleted = table.Column<bool>(nullable: false),
                    Sunday = table.Column<bool>(nullable: false),
                    SundayCompleted = table.Column<bool>(nullable: false),
                    Thursday = table.Column<bool>(nullable: false),
                    ThursdayCompleted = table.Column<bool>(nullable: false),
                    TotalEarned = table.Column<float>(nullable: false),
                    Tuesday = table.Column<bool>(nullable: false),
                    TuesdayCompleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<string>(nullable: true),
                    Wednesday = table.Column<bool>(nullable: false),
                    WednesdayCompleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedChildrenWork", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FriendsRequestViewModel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateRequested = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    StatusMessage = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendsRequestViewModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FriendViewModel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    FriendSince = table.Column<DateTime>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendViewModel", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildrenViewModel");

            migrationBuilder.DropTable(
                name: "CompletedChildrenWork");

            migrationBuilder.DropTable(
                name: "FriendsRequestViewModel");

            migrationBuilder.DropTable(
                name: "FriendViewModel");
        }
    }
}
