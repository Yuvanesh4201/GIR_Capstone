using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIR_Capstone.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCorporateStructureXML : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XmlSubmissionId",
                table: "Corporates");

            migrationBuilder.CreateTable(
                name: "CorporateStructureXML",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    XmlData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTimeCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateStructureXML", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateStructureXML_Corporates_StructureId",
                        column: x => x.StructureId,
                        principalTable: "Corporates",
                        principalColumn: "StructureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateStructureXML_StructureId",
                table: "CorporateStructureXML",
                column: "StructureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporateStructureXML");

            migrationBuilder.AddColumn<Guid>(
                name: "XmlSubmissionId",
                table: "Corporates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
