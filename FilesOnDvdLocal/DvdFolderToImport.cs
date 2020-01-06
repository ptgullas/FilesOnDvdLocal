using FilesOnDvdLocal.LocalDbDtos;
using Serilog;
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
        public List<PerformerLocalDto> PerformersInFolderAll { get; set; }

        public string DiscName { get; set; }
        public int WalletType { get; set; } // 3 will be the usual one
        public string Notes { get; set; }
        public int DatabaseId { get; set; }


        public DvdFolderToImport(string folderPath, IDataRepository dataRepository) {
            FolderPath = folderPath;
            Files = new List<FileToImport>();
            PopulateFiles(dataRepository);
            PerformersInFolderAll = new List<PerformerLocalDto>();
            CompileAllPerformersInFolder();
        }

        // constructor accepts list of files (mainly for testing)
        public DvdFolderToImport(string folderPath, List<FileToImport> files) {
            Files = files;
            FolderPath = folderPath;
            PerformersInFolderAll = new List<PerformerLocalDto>();
            CompileAllPerformersInFolder();
        }

        public void PopulateFiles(IDataRepository dataRepository) {
            Files.Clear();
            if (Directory.Exists(FolderPath)) {
                var files = Directory.GetFiles(FolderPath);
                foreach (string file in files) {
                    FileToImport fileToImport = new FileToImport(file, dataRepository);
                    Files.Add(fileToImport);
                }
            }
        }

        public void CompileAllPerformersInFolder() {
            PerformersInFolderAll.Clear();
            if (Files.Count > 0) {
                foreach (var file in Files) {
                    if (file.Performers.Count > 0) {
                        foreach (var performer in file.Performers) {
                            // change this to Id later?
                            if (!PerformersInFolderAll.Any(b => b.Name == performer.Name)) {
                                PerformersInFolderAll.Add(performer);
                            }
                        }
                    }
                }
            }
        }

        public bool HasNamingErrors() {
            bool hasNamingErrors = false;
            if (Files.Count > 0) {
                foreach (FileToImport file in Files) {
                    if (file.NameContainsNonAscii || file.NameIsTooLong) {
                        hasNamingErrors = true;
                    }
                }
            }
            return hasNamingErrors;
        }

        public bool IsTooLarge() {
            bool isTooLarge = false;
            try {
                isTooLarge = CreateDvdBin().IsTooLarge();
            }
            catch (Exception e) {
                Log.Error(e, "Error creating DvdBin for checking size");
            }
            return isTooLarge;
        }

        private DvdBin CreateDvdBin() {
            string DvdName = Path.GetDirectoryName(FolderPath);
            DvdBin dvdBin = new DvdBin(DvdName);
            foreach (FileToImport file in Files) {
                dvdBin.Files.Add(file.File);
            }
            return dvdBin;
        }

        private List<string> GetFilenameList() {
            List<string> filenameList = new List<string>();
            if (Files.Count > 0) {

                foreach (FileToImport file in Files.OrderBy(b =>b.File.Name) ) {
                    filenameList.Add(file.File.Name);
                }
            }

            return filenameList;
        }

        public async Task<bool> SaveFilenameListToTextFile(string folderPathToSave) {
            bool success = false;
            List<string> filenameList = GetFilenameList();
            if (Directory.Exists(folderPathToSave)) {
                string textfileName = Path.GetFileName(FolderPath) + ".txt";
                string pathToSave = Path.Combine(folderPathToSave, textfileName);
                try {
                    using (StreamWriter outputfile = new StreamWriter(pathToSave, false)) {
                        foreach (string filename in filenameList) {
                            await outputfile.WriteLineAsync(filename);
                        }
                    }
                    success = true;
                }
                catch (Exception e) {
                    Log.Error(e, "Error saving filename list {0}", pathToSave);
                }
            }
            return success;
        }
    }
}
