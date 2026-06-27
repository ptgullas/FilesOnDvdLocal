using System.Collections.Generic;

namespace MediaFilesOnDvd.Services.Dtos {
    public class PerformerEditDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<string> HeadshotUrls { get; set; } = new List<string>();
        public string? PreferredHeadshotUrl { get; set; }
        public int PerformerTypeId { get; set; }
        public List<string> GalleryPhotoUrls { get; set; } = new List<string>();

        // List for easy tag-based editing
        public List<string> Aliases { get; set; } = new List<string>();
    }
}
