using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicRack.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Settings",
                newName: "Key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Key",
                table: "Settings",
                newName: "Name");
        }
    }
}
