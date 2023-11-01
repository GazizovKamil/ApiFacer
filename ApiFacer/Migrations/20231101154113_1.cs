using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiFacer.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "PeopleId",
                table: "UserImages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    descriptor = table.Column<byte[]>(type: "BLOB", nullable: false),
                    first_name = table.Column<string>(type: "TEXT", nullable: false),
                    middle_name = table.Column<string>(type: "TEXT", nullable: false),
                    last_name = table.Column<string>(type: "TEXT", nullable: false),
                    phone_number = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserImages_ImageId",
                table: "UserImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserImages_PeopleId",
                table: "UserImages",
                column: "PeopleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserImages_Images_ImageId",
                table: "UserImages",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserImages_People_PeopleId",
                table: "UserImages",
                column: "PeopleId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserImages_Images_ImageId",
                table: "UserImages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserImages_People_PeopleId",
                table: "UserImages");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropIndex(
                name: "IX_UserImages_ImageId",
                table: "UserImages");

            migrationBuilder.DropIndex(
                name: "IX_UserImages_PeopleId",
                table: "UserImages");

            migrationBuilder.DropColumn(
                name: "PeopleId",
                table: "UserImages");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "name" },
                values: new object[] { 3, "Гость" });
        }
    }
}
