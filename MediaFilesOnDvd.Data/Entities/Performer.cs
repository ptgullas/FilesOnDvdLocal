using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class Performer {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int? PerformerTypeId { get; set; }
        public PerformerType? Type { get; set; }

        // Id from the MS Access database.
        // Used for matching MediaFile with Performer during Access migration
        public int? LegacyId { get; set; }

        public virtual ICollection<HeadshotUrl> HeadshotUrls { get; set; } = new List<HeadshotUrl>();
        public virtual ICollection<GalleryPhotoUrl> GalleryPhotoUrls { get; set; } = new List<GalleryPhotoUrl>();
        public virtual ICollection<PerformerAlias> PerformerAliases { get; set; } = new List<PerformerAlias>();
        // comment this out until I'm ready
        public virtual ICollection<MediaFile>? MediaFiles { get; set; } = new List<MediaFile>();

        public string? Notes { get; set; }

        public Performer() {

        }

        public Performer(string name, string headshotUrl = null) {
            Name = name;
            HeadshotUrls.Add(new HeadshotUrl(headshotUrl));
        }
    }
}
