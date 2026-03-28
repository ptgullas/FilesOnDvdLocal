namespace MediaFilesOnDvd.Services.Dtos {
    public class MediaFileSummaryDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? DiscName { get; set; }
        public string? ScreenshotUrl { get; set; }
    }

    public class MediaFileDetailDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int GenreId { get; set; }
        public string? GenreName { get; set; }
        public int DiscId { get; set; }
        public string? DiscName { get; set; }
        public int? SeriesId { get; set; }
        public string? SeriesName { get; set; }
        public List<PerformerSummaryDto> Performers { get; set; } = new();
        public List<string> ScreenshotUrls { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}
