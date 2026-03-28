namespace MediaFilesOnDvd.Services.Dtos {
    public class PerformerSummaryDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? HeadshotUrl { get; set; }
    }

    public class PerformerDetailDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? HeadshotUrl { get; set; }
        public List<MediaFileSummaryDto> MediaFiles { get; set; } = new();
    }

    public class PerformerWithGalleryDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? HeadshotUrl { get; set; }
        public string? GalleryPhotoUrl { get; set; }
    }
}
