#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Lister.Core.SqlDB.Migrations.Lister;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "ListDefs",
                table => new
                {
                    Id = table.Column<Guid>("char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>("datetime(6)", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ListDefs", x => x.Id); })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "ColumnDefs",
                table => new
                {
                    Id = table.Column<Guid>("char(36)", nullable: false, collation: "ascii_general_ci"),
                    ListDefEntityId = table.Column<Guid>("char(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>("int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnDefs", x => x.Id);
                    table.ForeignKey(
                        "FK_ColumnDefs_ListDefs_ListDefEntityId",
                        x => x.ListDefEntityId,
                        "ListDefs",
                        "Id");
                })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "StatusDefs",
                table => new
                {
                    Id = table.Column<Guid>("char(36)", nullable: false, collation: "ascii_general_ci"),
                    ListDefEntityId = table.Column<Guid>("char(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Color = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusDefs", x => x.Id);
                    table.ForeignKey(
                        "FK_StatusDefs_ListDefs_ListDefEntityId",
                        x => x.ListDefEntityId,
                        "ListDefs",
                        "Id");
                })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            "IX_ColumnDefs_ListDefEntityId",
            "ColumnDefs",
            "ListDefEntityId");

        migrationBuilder.CreateIndex(
            "IX_StatusDefs_ListDefEntityId",
            "StatusDefs",
            "ListDefEntityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "ColumnDefs");

        migrationBuilder.DropTable(
            "StatusDefs");

        migrationBuilder.DropTable(
            "ListDefs");
    }
}