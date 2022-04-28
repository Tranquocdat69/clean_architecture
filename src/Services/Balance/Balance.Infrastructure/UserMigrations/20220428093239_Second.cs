using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECom.Services.Balance.Infrastructure.UserMigrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kafkaoffset",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Command_Offset = table.Column<long>(type: "bigint", nullable: false),
                    Persistent_Offset = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kafkaoffset", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kafkaoffset");
        }
    }
}
