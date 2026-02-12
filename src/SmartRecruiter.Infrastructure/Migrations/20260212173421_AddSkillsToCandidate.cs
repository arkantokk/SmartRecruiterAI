using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRecruiter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillsToCandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Evaluation_Skills",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Evaluation_Skills",
                table: "Candidates");
        }
    }
}
