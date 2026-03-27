using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class MediaFile {
        public int Id { get; set; }
        public string Name { get; set; }

        // public FileGenre Genre { get; set; }
        public virtual ICollection<Performer> Performers { get; set; } = new List<Performer>();
        public string? Notes { get; set; }
        public bool IsUnwatched { get; set; } = false;
        public int UnknownPerformerCount { get; set; } = 0;
        public int? SeriesId { get; set; }
        public Series? Series { get; set; }
        public int DiscId { get; set; }
        public Disc Disc { get; set; }
        public int FileGenreId { get; set; }
        public FileGenre FileGenre { get; set; }
        public virtual ICollection<ScreenshotUrl> Screenshots { get; set; } = new List<ScreenshotUrl>();
        public virtual ICollection<FileTag> Tags { get; set; } = new List<FileTag>();

        private MediaFile() {

        }

        // Constructor for testing
        public MediaFile(string name, List<Performer> performers, string? notes = null, List<ScreenshotUrl>? screenshots = null) {
            Name = name;
            Performers = performers;
            Notes = notes;
            Screenshots = screenshots;
        }

        // Constructor for Legacy migration
        public MediaFile(string name, string? notes, string legacyScreenshotFilename) {
            Name = name;
            Notes = notes;
            ScreenshotUrl screenshotUrl = new() { Url = legacyScreenshotFilename };
            Screenshots.Add(screenshotUrl);
        }
    }
}
