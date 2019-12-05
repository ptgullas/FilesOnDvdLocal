using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public class DvdFolderToImport {
        public string FolderPath { get; set; }
        public List<FileToImport> Files { get; set; }

        public DvdFolderToImport(string folderPath) {
            FolderPath = folderPath;
            Files = new List<FileToImport>();
        }

        public void GetFiles() {
            if (Directory.Exists(FolderPath)) {
                var files = Directory.GetFiles(FolderPath);
                foreach (string file in files) {
                    FileToImport fileToImport = new FileToImport(file);
                    Files.Add(fileToImport);
                }
            }
        }

        public bool HasNamingErrors() {
            bool hasNamingErrors = false;
            if (Files.Count > 0) {
                foreach (FileToImport file in Files) {
                    if (file.NameContainsNonAscii() || file.NameIsTooLong()) {
                        hasNamingErrors = true;
                    }
                }
            }
            return hasNamingErrors;
        }
    }
}
