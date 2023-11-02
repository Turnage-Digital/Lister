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
    [Migration("20231102234259_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.ColumnDefEntity", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ListDefEntityId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ListDefEntityId");

                    b.ToTable("ColumnDefs");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.ListDefEntity", b =>
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

                    b.ToTable("ListDefs");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.StatusDefEntity", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("ListDefEntityId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("ListDefEntityId");

                    b.ToTable("StatusDefs");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.ColumnDefEntity", b =>
                {
                    b.HasOne("Lister.Core.SqlDB.Entities.ListDefEntity", null)
                        .WithMany("ColumnDefs")
                        .HasForeignKey("ListDefEntityId");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.StatusDefEntity", b =>
                {
                    b.HasOne("Lister.Core.SqlDB.Entities.ListDefEntity", null)
                        .WithMany("StatusDefs")
                        .HasForeignKey("ListDefEntityId");
                });

            modelBuilder.Entity("Lister.Core.SqlDB.Entities.ListDefEntity", b =>
                {
                    b.Navigation("ColumnDefs");

                    b.Navigation("StatusDefs");
                });
#pragma warning restore 612, 618
        }
    }
}
