﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIR_Capstone.Server.Migrations
{
    /// <inheritdoc />
    public partial class ModelConfigChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityOwnerships_CorporateEntities_OwnedEntityId",
                table: "EntityOwnerships");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityOwnerships_CorporateEntities_OwnedEntityId",
                table: "EntityOwnerships",
                column: "OwnedEntityId",
                principalTable: "CorporateEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityOwnerships_CorporateEntities_OwnedEntityId",
                table: "EntityOwnerships");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityOwnerships_CorporateEntities_OwnedEntityId",
                table: "EntityOwnerships",
                column: "OwnedEntityId",
                principalTable: "CorporateEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
