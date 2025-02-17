﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GIR_Capstone.Server.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250217101349_AddCorporateStructureXML")]
    partial class AddCorporateStructureXML
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CodeDecodeGlobeStatus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DecodeDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CodeDecodeGlobeStatus");
                });

            modelBuilder.Entity("CodeDecodeOwnershipType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DecodeDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CodeDecodeOwnershipType");
                });

            modelBuilder.Entity("Corporate", b =>
                {
                    b.Property<Guid>("StructureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MneName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StructureId");

                    b.ToTable("Corporates");
                });

            modelBuilder.Entity("CorporateEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CorporationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Is_Excluded")
                        .HasColumnType("bit");

                    b.Property<string>("Jurisdiction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("QIIR_Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CorporationId");

                    b.HasIndex("ParentId");

                    b.ToTable("CorporateEntities");
                });

            modelBuilder.Entity("CorporateStructureXML", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateTimeCreated")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("StructureId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("XmlData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("StructureId");

                    b.ToTable("CorporateStructureXML");
                });

            modelBuilder.Entity("EntityOwnership", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OwnedEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OwnerEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("OwnershipPercentage")
                        .HasPrecision(4, 1)
                        .HasColumnType("decimal(4,1)");

                    b.Property<string>("OwnershipType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OwnedEntityId");

                    b.HasIndex("OwnerEntityId");

                    b.ToTable("EntityOwnerships");
                });

            modelBuilder.Entity("EntityStatus", b =>
                {
                    b.Property<Guid>("EntityId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(0);

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnOrder(1);

                    b.HasKey("EntityId", "Status");

                    b.ToTable("EntityStatuses");
                });

            modelBuilder.Entity("CorporateEntity", b =>
                {
                    b.HasOne("Corporate", "Corporate")
                        .WithMany("Entities")
                        .HasForeignKey("CorporationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CorporateEntity", "ParentEntity")
                        .WithMany("ChildEntities")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Corporate");

                    b.Navigation("ParentEntity");
                });

            modelBuilder.Entity("CorporateStructureXML", b =>
                {
                    b.HasOne("Corporate", "Corporate")
                        .WithMany()
                        .HasForeignKey("StructureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Corporate");
                });

            modelBuilder.Entity("EntityOwnership", b =>
                {
                    b.HasOne("CorporateEntity", "OwnedEntity")
                        .WithMany("Ownerships")
                        .HasForeignKey("OwnedEntityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CorporateEntity", "OwnerEntity")
                        .WithMany()
                        .HasForeignKey("OwnerEntityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("OwnedEntity");

                    b.Navigation("OwnerEntity");
                });

            modelBuilder.Entity("EntityStatus", b =>
                {
                    b.HasOne("CorporateEntity", "CorporateEntity")
                        .WithMany("Statuses")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CorporateEntity");
                });

            modelBuilder.Entity("Corporate", b =>
                {
                    b.Navigation("Entities");
                });

            modelBuilder.Entity("CorporateEntity", b =>
                {
                    b.Navigation("ChildEntities");

                    b.Navigation("Ownerships");

                    b.Navigation("Statuses");
                });
#pragma warning restore 612, 618
        }
    }
}
