using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiFacer.Migrations
{
    /// <inheritdoc />
    public partial class new2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "first_name", "id_role", "last_name", "login", "password", "surname" },
                values: new object[] { 1, "Арсений", 1, "А", "admin", "admin", "Артюхин" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
