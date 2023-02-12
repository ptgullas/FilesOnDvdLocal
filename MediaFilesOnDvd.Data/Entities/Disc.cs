using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class Disc {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Notes { get; set; }
        // public ICollection<MediaFile> Files { get; set; } = new List<MediaFile>();
        public int? WalletId { get; set; }
        public Wallet Wallet { get; set; }

        public Disc() {

        }
        /*
        public Disc(string name, string notes, ICollection<MediaFile> files) {
            Name = name;
            Notes = notes;
            Files = files;
        }
        */
    }
}
