using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class FileTag {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

        public FileTag() {
        }

        public FileTag(string name) {
            Name = name;
        }
    }
}
