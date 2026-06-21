using System.Collections.Generic;

namespace MediaFilesOnDvd.Services.Dtos {
    public class PerformerEditDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? HeadshotUrl { get; set; }
        public string? GalleryPhotoUrl { get; set; }

        // List for easy tag-based editing
        public List<string> Aliases { get; set; } = new List<string>();
    }
}
