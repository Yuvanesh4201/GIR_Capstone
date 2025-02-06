using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIR_Capstone.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Corporates",
                columns: table => new
                {
                    StructureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MNEName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XMLSubmissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corporates", x => x.StructureId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Corporates");
        }
    }
}
