using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaFilesOnDvd.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Performers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Performers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerformerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScreenshotUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenshotUrls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    StorageUnitId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeadshotUrl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    PerformerId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadshotUrl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeadshotUrl_Performers_PerformerId",
                        column: x => x.PerformerId,
                        principalTable: "Performers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Discs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    WalletId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discs_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsUnwatched = table.Column<bool>(type: "INTEGER", nullable: false),
                    UnknownPerformerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DiscId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaFiles_Discs_DiscId",
                        column: x => x.DiscId,
                        principalTable: "Discs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaFilePerformer",
                columns: table => new
                {
                    MediaFilesId = table.Column<int>(type: "INTEGER", nullable: false),
                    PerformersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFilePerformer", x => new { x.MediaFilesId, x.PerformersId });
                    table.ForeignKey(
                        name: "FK_MediaFilePerformer_MediaFiles_MediaFilesId",
                        column: x => x.MediaFilesId,
                        principalTable: "MediaFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaFilePerformer_Performers_PerformersId",
                        column: x => x.PerformersId,
                        principalTable: "Performers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discs_WalletId",
                table: "Discs",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_HeadshotUrl_PerformerId",
                table: "HeadshotUrl",
                column: "PerformerId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFilePerformer_PerformersId",
                table: "MediaFilePerformer",
                column: "PerformersId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_DiscId",
                table: "MediaFiles",
                column: "DiscId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeadshotUrl");

            migrationBuilder.DropTable(
                name: "MediaFilePerformer");

            migrationBuilder.DropTable(
                name: "PerformerTypes");

            migrationBuilder.DropTable(
                name: "ScreenshotUrls");

            migrationBuilder.DropTable(
                name: "MediaFiles");

            migrationBuilder.DropTable(
                name: "Performers");

            migrationBuilder.DropTable(
                name: "Discs");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
