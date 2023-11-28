﻿// <auto-generated />
using System;
using Lister.Core.SqlDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Lister.Core.SqlDB.Migrations.Lister
{
    [DbContext(typeof(ListerDbContext))]
    [Migration("20231105010407_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.ColumnEntity", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ListEntityId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ListEntityId");

                    b.ToTable("Columns");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.ListEntity", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Lists");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.StatusEntity", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("ListEntityId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("ListEntityId");

                    b.ToTable("Statuses");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.ColumnEntity", b =>
                {
                    b.HasOne("Lister.Core.SqlDB.Entities.ListEntity", null)
                        .WithMany("Columns")
                        .HasForeignKey("ListEntityId");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.StatusEntity", b =>
                {
                    b.HasOne("Lister.Core.SqlDB.Entities.ListEntity", null)
                        .WithMany("Statuses")
                        .HasForeignKey("ListEntityId");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.ListEntity", b =>
                {
                    b.Navigation("Columns");

                    b.Navigation("Statuses");
                });
#pragma warning restore 612, 618
        }
    }
}
