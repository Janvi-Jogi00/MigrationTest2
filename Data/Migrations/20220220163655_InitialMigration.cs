using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MigrationStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartingNum = table.Column<int>(type: "int", nullable: false),
                    EndingNum = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MigrationStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstNumber = table.Column<int>(type: "int", nullable: false),
                    SecondNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DestinationTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sum = table.Column<int>(type: "int", nullable: false),
                    SourceTableId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinationTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinationTable_SourceTable_SourceTableId",
                        column: x => x.SourceTableId,
                        principalTable: "SourceTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DestinationTable_SourceTableId",
                table: "DestinationTable",
                column: "SourceTableId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DestinationTable");

            migrationBuilder.DropTable(
                name: "MigrationStatus");

            migrationBuilder.DropTable(
                name: "SourceTable");
        }
    }
}
