using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIR_Capstone.Server.Migrations
{
    /// <inheritdoc />
    public partial class QIIR_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QIIR_Status",
                table: "CorporateEntities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QIIR_Status",
                table: "CorporateEntities");
        }
    }
}
