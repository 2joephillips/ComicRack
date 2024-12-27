using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicRack.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeComicColumnName2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "Comics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "Comics");
        }
    }
}
