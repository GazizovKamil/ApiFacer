using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiFacer.Migrations
{
    /// <inheritdoc />
    public partial class new256 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "faceDescriptor",
                table: "Users",
                newName: "FaceDescriptor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FaceDescriptor",
                table: "Users",
                newName: "faceDescriptor");
        }
    }
}
