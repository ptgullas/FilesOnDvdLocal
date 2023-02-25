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
        // public Series Series { get; set; }
        public virtual ICollection<Performer> Performers { get; set; } = new List<Performer>();
        public string? Notes { get; set; }
        public bool IsUnwatched { get; set; } = false;
        public int UnknownPerformerCount { get; set; } = 0;        
        public int DiscId { get; set; }
        public Disc Disc { get; set; }
        public virtual ICollection<ScreenshotUrl> Screenshots { get; set; } = new List<ScreenshotUrl>();

        public MediaFile() {

        }
        
    }
}
