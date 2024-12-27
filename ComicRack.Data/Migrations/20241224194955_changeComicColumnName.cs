using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicRack.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeComicColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdGuid",
                table: "Comics",
                newName: "Guid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "Comics",
                newName: "IdGuid");
        }
    }
}
