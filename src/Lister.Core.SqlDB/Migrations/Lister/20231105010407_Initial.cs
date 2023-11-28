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
                "Lists",
                table => new
                {
                    Id = table.Column<Guid>("char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>("datetime(6)", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Lists", x => x.Id); })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "Columns",
                table => new
                {
                    Id = table.Column<Guid>("char(36)", nullable: false, collation: "ascii_general_ci"),
                    ListEntityId = table.Column<Guid>("char(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>("int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Columns", x => x.Id);
                    table.ForeignKey(
                        "FK_Columns_Lists_ListEntityId",
                        x => x.ListEntityId,
                        "Lists",
                        "Id");
                })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "Statuses",
                table => new
                {
                    Id = table.Column<Guid>("char(36)", nullable: false, collation: "ascii_general_ci"),
                    ListEntityId = table.Column<Guid>("char(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Color = table.Column<string>("longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                    table.ForeignKey(
                        "FK_Statuses_Lists_ListEntityId",
                        x => x.ListEntityId,
                        "Lists",
                        "Id");
                })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            "IX_Columns_ListEntityId",
            "Columns",
            "ListEntityId");

        migrationBuilder.CreateIndex(
            "IX_Statuses_ListEntityId",
            "Statuses",
            "ListEntityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Columns");

        migrationBuilder.DropTable(
            "Statuses");

        migrationBuilder.DropTable(
            "Lists");
    }
}