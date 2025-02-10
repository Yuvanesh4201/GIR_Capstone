using System;
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
                    StructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MneName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XmlSubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corporates", x => x.StructureId);
                });

            migrationBuilder.CreateTable(
                name: "CorporateEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorporationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jurisdiction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Is_Excluded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateEntities_CorporateEntities_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CorporateEntities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CorporateEntities_Corporates_CorporationId",
                        column: x => x.CorporationId,
                        principalTable: "Corporates",
                        principalColumn: "StructureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityOwnerships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnedEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnershipType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnershipPercentage = table.Column<decimal>(type: "decimal(2,1)", precision: 2, scale: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityOwnerships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityOwnerships_CorporateEntities_OwnedEntityId",
                        column: x => x.OwnedEntityId,
                        principalTable: "CorporateEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityOwnerships_CorporateEntities_OwnerEntityId",
                        column: x => x.OwnerEntityId,
                        principalTable: "CorporateEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityStatuses",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityStatuses", x => new { x.EntityId, x.Status });
                    table.ForeignKey(
                        name: "FK_EntityStatuses_CorporateEntities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "CorporateEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateEntities_CorporationId",
                table: "CorporateEntities",
                column: "CorporationId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateEntities_ParentId",
                table: "CorporateEntities",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityOwnerships_OwnedEntityId",
                table: "EntityOwnerships",
                column: "OwnedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityOwnerships_OwnerEntityId",
                table: "EntityOwnerships",
                column: "OwnerEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityOwnerships");

            migrationBuilder.DropTable(
                name: "EntityStatuses");

            migrationBuilder.DropTable(
                name: "CorporateEntities");

            migrationBuilder.DropTable(
                name: "Corporates");
        }
    }
}
