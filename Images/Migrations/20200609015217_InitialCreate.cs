using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Images.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfilePhotoPaths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false),
                    PhotoPath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilePhotoPaths", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProfilePhotoPaths",
                columns: new[] { "Id", "Email", "PhotoPath" },
                values: new object[,]
                {
                    { 1, "pete@fakemail.com", "Media/Profiles/pete.png" },
                    { 2, "roger@fakemail.com", "Media/Profiles/roger.png" },
                    { 3, "john@fakemail.com", "Media/Profiles/john.png" },
                    { 4, "keith@fakemail.com", "Media/Profiles/keith.png" },
                    { 9, "jimmi@fakemail.com", "Media/Profiles/jimmi.png" },
                    { 5, "geddy@fakemail.com", "Media/Profiles/geddy.png" },
                    { 6, "alex@fakemail.com", "Media/Profiles/alex.png" },
                    { 7, "neil@fakemail.com", "Media/Profiles/neil.png" },
                    { 8, "freddie@fakemail.com", "Media/Profiles/freddie.png" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfilePhotoPaths");
        }
    }
}
