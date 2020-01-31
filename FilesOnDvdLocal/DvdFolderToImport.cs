using FilesOnDvdLocal.LocalDbDtos;
using FilesOnDvdLocal.Repositories;
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
        public int? DatabaseId { get; set; }

        public bool IsReadyToImport { 
            get {
                if (IsTooLarge() | HasNamingErrors()) {
                    return false;
                }
                else { return true; }
            }
        }


        public DvdFolderToImport(string folderPath, 
            IPerformerRepository performerRepository, ISeriesRepository seriesRepository) {
            DatabaseId = null;
            FolderPath = folderPath;
            DiscName = Path.GetFileName(FolderPath);
            Files = new List<FileToImport>();
            PopulateFiles(performerRepository, seriesRepository);
            PerformersInFolderAll = new List<PerformerLocalDto>();
            CompileAllPerformersInFolder();

            // let the user pick this via the UI later
            GenreLocalDto genreDto = new GenreLocalDto() { Id = 3, Genre = "Stuff" };
            SetFilesGenre(genreDto);
        }

        // constructor accepts list of files (mainly for testing)
        public DvdFolderToImport(string folderPath, List<FileToImport> files) {
            Files = files;
            FolderPath = folderPath;
            PerformersInFolderAll = new List<PerformerLocalDto>();
            CompileAllPerformersInFolder();
        }

        public void PopulateFiles(IPerformerRepository performerRepository, ISeriesRepository seriesRepository) {
            Files.Clear();
            if (Directory.Exists(FolderPath)) {
                var files = Directory.GetFiles(FolderPath);
                foreach (string file in files) {
                    FileToImport fileToImport = new FileToImport(file, performerRepository, seriesRepository);
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
                    if (file.NameContainsNonAscii || file.NameIsTooLong || file.NameContainsDoubleSpaces) {
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
                        await outputfile.WriteLineAsync(Path.GetFileName(FolderPath));
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

        public void SetFilesGenre(GenreLocalDto genreDto) {
            if (Files.Count > 0) {
                foreach (FileToImport file in Files) {
                    file.Genre = genreDto.Id;
                }
            }
        }

        public OperationResult SetFilesDiscId() {
            if (DatabaseId != null) {
                foreach (FileToImport file in Files) {
                    file.DiscId = DatabaseId;
                }
                return new OperationResult() { Success = true, Message = null };
            }
            else {
                return new OperationResult() { Success = false, Message = "DatabaseId not set" };
            }
        }

        public void SetFilesIdsFromDatabase(List<FileLocalDto> fileDtosInDisc) {
            if (fileDtosInDisc.Count != Files.Count) {
                Log.Error("Count of files imported to Database doesn't match count of files in DVDFolderToImport");
            }
            foreach (FileToImport file in Files) {
                file.DatabaseId = fileDtosInDisc
                    .FirstOrDefault(f => f.Filename == file.Filename).Id;
            }
        }

        public List<PerformerFilenameJoinDto> GetPerformerFilenameJoinDtos() {
            List<PerformerFilenameJoinDto> joinDtos = new List<PerformerFilenameJoinDto>();
            foreach (FileToImport file in Files) {
                foreach (PerformerLocalDto performer in file.Performers) {
                    PerformerFilenameJoinDto joinDto = new PerformerFilenameJoinDto() {
                        PerformerId = performer.Id,
                        FilenameId = (int) file.DatabaseId
                    };
                    joinDtos.Add(joinDto);
                }
            }
            return joinDtos;
        }
    }
}
