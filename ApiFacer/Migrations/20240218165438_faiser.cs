using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiFacer.Migrations
{
    /// <inheritdoc />
    public partial class faiser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    path = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    sessionkey = table.Column<string>(type: "TEXT", nullable: false),
                    ipAdress = table.Column<string>(type: "TEXT", nullable: false),
                    userId = table.Column<int>(type: "INTEGER", nullable: false),
                    id_role = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonVk",
                columns: table => new
                {
                    userImagesId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    tag = table.Column<string>(type: "TEXT", nullable: false),
                    confidence = table.Column<double>(type: "REAL", nullable: false),
                    awesomeness = table.Column<double>(type: "REAL", nullable: false),
                    similarity = table.Column<double>(type: "REAL", nullable: false),
                    sex = table.Column<string>(type: "TEXT", nullable: false),
                    emotion = table.Column<string>(type: "TEXT", nullable: false),
                    age = table.Column<int>(type: "INTEGER", nullable: false),
                    valence = table.Column<double>(type: "REAL", nullable: false),
                    arousal = table.Column<double>(type: "REAL", nullable: false),
                    frontality = table.Column<double>(type: "REAL", nullable: false),
                    visibility = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonVk", x => x.userImagesId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserImages",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userId = table.Column<int>(type: "INTEGER", nullable: false),
                    yandexPath = table.Column<string>(type: "TEXT", nullable: false),
                    localPath = table.Column<string>(type: "TEXT", nullable: false),
                    eventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserImages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    login = table.Column<string>(type: "TEXT", nullable: true),
                    password = table.Column<string>(type: "TEXT", nullable: true),
                    surname = table.Column<string>(type: "TEXT", nullable: true),
                    first_name = table.Column<string>(type: "TEXT", nullable: true),
                    last_name = table.Column<string>(type: "TEXT", nullable: true),
                    id_role = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Name", "path" },
                values: new object[,]
                {
                    { 1, "Future Games 2024", "FutureGames2024" },
                    { 2, "10 Future Games 2023", "10FutureGames2023" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Фотограф" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "first_name", "id_role", "last_name", "login", "password", "surname" },
                values: new object[] { 1, "Арсений", 1, "А", "root", "rimezaaa", "Артюхин" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "PersonVk");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UserImages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
