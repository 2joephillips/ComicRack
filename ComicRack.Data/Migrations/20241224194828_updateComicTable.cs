using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicRack.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateComicTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NeedsMetaData",
                table: "Comics",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UnableToOpen",
                table: "Comics",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedsMetaData",
                table: "Comics");

            migrationBuilder.DropColumn(
                name: "UnableToOpen",
                table: "Comics");
        }
    }
}
