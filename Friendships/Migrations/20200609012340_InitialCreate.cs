using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Friendships.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    RequesterEmail = table.Column<string>(nullable: true),
                    ConfirmerEmail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Friendships",
                columns: new[] { "Id", "ConfirmerEmail", "IsConfirmed", "RequesterEmail" },
                values: new object[,]
                {
                    { 1, "roger@fakemail.com", true, "pete@fakemail.com" },
                    { 2, "john@fakemail.com", true, "pete@fakemail.com" },
                    { 3, "keith@fakemail.com", true, "pete@fakemail.com" },
                    { 4, "john@fakemail.com", true, "roger@fakemail.com" },
                    { 5, "keith@fakemail.com", true, "roger@fakemail.com" },
                    { 6, "john@fakemail.com", true, "keith@fakemail.com" },
                    { 7, "neil@fakemail.com", true, "geddy@fakemail.com" },
                    { 8, "alex@fakemail.com", true, "geddy@fakemail.com" },
                    { 9, "neil@fakemail.com", true, "alex@fakemail.com" },
                    { 10, "freddie@fakemail.com", true, "jimmi@fakemail.com" },
                    { 11, "pete@fakemail.com", true, "jimmi@fakemail.com" },
                    { 12, "geddy@fakemail.com", false, "jimmi@fakemail.com" },
                    { 13, "pete@fakemail.com", true, "freddie@fakemail.com" },
                    { 14, "roger@fakemail.com", true, "freddie@fakemail.com" },
                    { 15, "keith@fakemail.com", false, "geddy@fakemail.com" },
                    { 16, "freddie@fakemail.com", false, "geddy@fakemail.com" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friendships");
        }
    }
}
