using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRecruiter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeMultitenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "JobVacancies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "JobVacancies");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Candidates");
        }
    }
}
