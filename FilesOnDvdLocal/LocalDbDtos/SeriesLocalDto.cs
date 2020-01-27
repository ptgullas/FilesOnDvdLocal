using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.LocalDbDtos {
    public class SeriesLocalDto {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Genre { get; set; }
        public int? Publisher { get; set; }
    }
}
