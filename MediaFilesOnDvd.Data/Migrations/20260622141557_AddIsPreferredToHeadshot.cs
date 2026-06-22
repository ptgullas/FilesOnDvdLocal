using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaFilesOnDvd.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPreferredToHeadshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HeadshotUrl_Performers_PerformerId",
                table: "HeadshotUrl");

            migrationBuilder.DropColumn(
                name: "Aliases",
                table: "Performers");

            migrationBuilder.AlterColumn<int>(
                name: "PerformerId",
                table: "HeadshotUrl",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPreferred",
                table: "HeadshotUrl",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PerformerAliases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PerformerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformerAliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformerAliases_Performers_PerformerId",
                        column: x => x.PerformerId,
                        principalTable: "Performers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerformerAliases_PerformerId",
                table: "PerformerAliases",
                column: "PerformerId");

            migrationBuilder.AddForeignKey(
                name: "FK_HeadshotUrl_Performers_PerformerId",
                table: "HeadshotUrl",
                column: "PerformerId",
                principalTable: "Performers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HeadshotUrl_Performers_PerformerId",
                table: "HeadshotUrl");

            migrationBuilder.DropTable(
                name: "PerformerAliases");

            migrationBuilder.DropColumn(
                name: "IsPreferred",
                table: "HeadshotUrl");

            migrationBuilder.AddColumn<string>(
                name: "Aliases",
                table: "Performers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PerformerId",
                table: "HeadshotUrl",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_HeadshotUrl_Performers_PerformerId",
                table: "HeadshotUrl",
                column: "PerformerId",
                principalTable: "Performers",
                principalColumn: "Id");
        }
    }
}
