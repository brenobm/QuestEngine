using Microsoft.EntityFrameworkCore.Migrations;

namespace QuestEngine.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerProgresses",
                columns: table => new
                {
                    PlayerId = table.Column<string>(maxLength: 50, nullable: false),
                    QuestId = table.Column<int>(nullable: false),
                    QuestPointsEarned = table.Column<long>(nullable: false),
                    LastMilestoneCompletedId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerProgresses", x => new { x.PlayerId, x.QuestId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerProgresses");
        }
    }
}
