using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public class FileToPack {
        public string DvdBinName { get; set; }
        public FileInfo File { get; set; }

        public FileToPack() {
            DvdBinName = null;
        }
    }
}
