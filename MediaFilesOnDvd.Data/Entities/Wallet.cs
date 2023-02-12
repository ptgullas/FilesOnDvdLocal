using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class Wallet {
        // set Id to nullable (int?) to avoid Cascade Deleting Discs if the Wallet is deleted
        public int? Id { get; set; }
        public string Name { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
        public int? StorageUnitId { get; set; }

        public List<Disc> Discs { get; set; } = new List<Disc>();
    }
}
