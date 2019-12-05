using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public class FileToImport {
        public FileInfo File { get; set; }
        public FileToImport(string path) {
            File = new FileInfo(path);
        }

        public bool NameIsTooLong() {
            int fileNameWithoutExtensionLength = File.Name.Length - File.Extension.Length;
            if (fileNameWithoutExtensionLength > 98) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
