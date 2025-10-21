using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBooking.Migrations
{
    /// <inheritdoc />
    public partial class CreateUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    Username = table.Column<string>(type: "nvarchar(100)", nullable: false),

                    Password = table.Column<string>(type: "nvarchar(255)", nullable: false),

                    Email = table.Column<string>(type: "nvarchar(150)", nullable: false),

                    FullName = table.Column<string>(type: "nvarchar(150)", nullable: true),

                    Role = table.Column<string>(type: "nvarchar(50)", nullable: true, defaultValue: "User"),

                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", nullable: true),

                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),

                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}