using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaFilesOnDvd.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformerAliases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MediaFileId",
                table: "ScreenshotUrls",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Aliases",
                table: "Performers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LegacyId",
                table: "Performers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformerTypeId",
                table: "Performers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileGenreId",
                table: "MediaFiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesId",
                table: "MediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FileGenres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileGenres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GalleryPhotoUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    PerformerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryPhotoUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GalleryPhotoUrls_Performers_PerformerId",
                        column: x => x.PerformerId,
                        principalTable: "Performers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesPublishers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesPublishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileTagMediaFile",
                columns: table => new
                {
                    MediaFilesId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTagMediaFile", x => new { x.MediaFilesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_FileTagMediaFile_FileTags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "FileTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileTagMediaFile_MediaFiles_MediaFilesId",
                        column: x => x.MediaFilesId,
                        principalTable: "MediaFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SeriesPublisherId = table.Column<int>(type: "INTEGER", nullable: true),
                    FileGenreId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Series_FileGenres_FileGenreId",
                        column: x => x.FileGenreId,
                        principalTable: "FileGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Series_SeriesPublishers_SeriesPublisherId",
                        column: x => x.SeriesPublisherId,
                        principalTable: "SeriesPublishers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScreenshotUrls_MediaFileId",
                table: "ScreenshotUrls",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Performers_PerformerTypeId",
                table: "Performers",
                column: "PerformerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_FileGenreId",
                table: "MediaFiles",
                column: "FileGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_SeriesId",
                table: "MediaFiles",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_FileTagMediaFile_TagsId",
                table: "FileTagMediaFile",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryPhotoUrls_PerformerId",
                table: "GalleryPhotoUrls",
                column: "PerformerId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_FileGenreId",
                table: "Series",
                column: "FileGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_SeriesPublisherId",
                table: "Series",
                column: "SeriesPublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_FileGenres_FileGenreId",
                table: "MediaFiles",
                column: "FileGenreId",
                principalTable: "FileGenres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Series_SeriesId",
                table: "MediaFiles",
                column: "SeriesId",
                principalTable: "Series",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Performers_PerformerTypes_PerformerTypeId",
                table: "Performers",
                column: "PerformerTypeId",
                principalTable: "PerformerTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScreenshotUrls_MediaFiles_MediaFileId",
                table: "ScreenshotUrls",
                column: "MediaFileId",
                principalTable: "MediaFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_FileGenres_FileGenreId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Series_SeriesId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Performers_PerformerTypes_PerformerTypeId",
                table: "Performers");

            migrationBuilder.DropForeignKey(
                name: "FK_ScreenshotUrls_MediaFiles_MediaFileId",
                table: "ScreenshotUrls");

            migrationBuilder.DropTable(
                name: "FileTagMediaFile");

            migrationBuilder.DropTable(
                name: "GalleryPhotoUrls");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "FileTags");

            migrationBuilder.DropTable(
                name: "FileGenres");

            migrationBuilder.DropTable(
                name: "SeriesPublishers");

            migrationBuilder.DropIndex(
                name: "IX_ScreenshotUrls_MediaFileId",
                table: "ScreenshotUrls");

            migrationBuilder.DropIndex(
                name: "IX_Performers_PerformerTypeId",
                table: "Performers");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_FileGenreId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_SeriesId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "MediaFileId",
                table: "ScreenshotUrls");

            migrationBuilder.DropColumn(
                name: "Aliases",
                table: "Performers");

            migrationBuilder.DropColumn(
                name: "LegacyId",
                table: "Performers");

            migrationBuilder.DropColumn(
                name: "PerformerTypeId",
                table: "Performers");

            migrationBuilder.DropColumn(
                name: "FileGenreId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "SeriesId",
                table: "MediaFiles");
        }
    }
}
