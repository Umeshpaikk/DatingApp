using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class Extended2UserEntitiy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateofBirth",
                table: "Users",
                newName: "DateOfBirth");

            migrationBuilder.RenameColumn(
                name: "Interest",
                table: "Users",
                newName: "Interests");

            migrationBuilder.RenameColumn(
                name: "publicId",
                table: "Photos",
                newName: "PublicId");

            migrationBuilder.RenameColumn(
                name: "isMain",
                table: "Photos",
                newName: "IsMain");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "Users",
                newName: "DateofBirth");

            migrationBuilder.RenameColumn(
                name: "Interests",
                table: "Users",
                newName: "Interest");

            migrationBuilder.RenameColumn(
                name: "PublicId",
                table: "Photos",
                newName: "publicId");

            migrationBuilder.RenameColumn(
                name: "IsMain",
                table: "Photos",
                newName: "isMain");
        }
    }
}
