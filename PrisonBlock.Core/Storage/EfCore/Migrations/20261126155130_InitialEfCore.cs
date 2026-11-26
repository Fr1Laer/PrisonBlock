using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrisonBlock.Core.Storage.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialEfCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prisoners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Energy = table.Column<int>(type: "int", nullable: false),
                    PrisonerType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Masterpieces = table.Column<int>(type: "int", nullable: true),
                    Wins = table.Column<int>(type: "int", nullable: true),
                    Cheats = table.Column<int>(type: "int", nullable: true),
                    Plants = table.Column<int>(type: "int", nullable: true),
                    Songs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prisoners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepositoryMemberships",
                columns: table => new
                {
                    PrisonerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Group = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepositoryMemberships", x => x.PrisonerId);
                    table.ForeignKey(
                        name: "FK_RepositoryMemberships_Prisoners_PrisonerId",
                        column: x => x.PrisonerId,
                        principalTable: "Prisoners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepositoryMemberships");

            migrationBuilder.DropTable(
                name: "Prisoners");
        }
    }
}
