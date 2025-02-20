using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIR_Capstone.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "XmlData",
                table: "CorporateStructureXML",
                type: "XML",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "XmlData",
                table: "CorporateStructureXML",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "XML");
        }
    }
}
