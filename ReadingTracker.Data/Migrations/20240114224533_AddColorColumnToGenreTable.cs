using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadingTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddColorColumnToGenreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Genres",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Genres");
        }
    }
}
