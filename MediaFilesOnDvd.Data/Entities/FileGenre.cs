using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class FileGenre {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Series> Series { get; set; } = new List<Series>();
        public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
    }
}
