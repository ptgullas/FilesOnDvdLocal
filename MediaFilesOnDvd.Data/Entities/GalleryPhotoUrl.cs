using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class GalleryPhotoUrl {
        public int Id { get; set; }
        public string Url { get; set; }

        public int PerformerId { get; set; }
        public Performer Performer { get; set; }

        public GalleryPhotoUrl() {
        }

        public GalleryPhotoUrl(string url) {
            Url = url;
        }
    }
}
