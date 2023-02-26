using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class Series {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SeriesPublisherId { get; set; }
        public SeriesPublisher Publisher { get; set; }

        public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
    }
}
