using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public class FileToImport {
        public FileInfo File { get; set; }
        public SeriesLocalDto Series {get; set;}
        public List<PerformerLocalDto> Performers { get; set; }
        public string Notes { get; set; }
        public bool Unwatched { get; set; }

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

        public bool NameContainsNonAscii() {
            bool NonAsciiInName = false;
            foreach (char c in File.Name) {
                if (c >= 128) {
                    NonAsciiInName = true;
                }
            }
            return NonAsciiInName;
        }

        public List<int> GetPositionsOfNonAsciiInName() {
            List<int> positions = new List<int>();
            for (int i = 0; i < File.Name.Length; i++) {
                char c = File.Name[i];
                if (c >= 128) {
                    positions.Add(i);
                }
            }
            return positions;
        }
    }
}
