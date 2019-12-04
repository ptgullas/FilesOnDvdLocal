using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Serilog;

namespace FilesOnDvdLocal {
    public class BinPacker {
        public string LocalFolder { get; set; }
        public List<FileToPack> Files { get; set; }
        public string Prefix { get; set; }
        public char CurrentSuffix { get; set; }
        public List<DvdBin> Bins { get; set; }

    
        public BinPacker(string localFolder, string prefix = "P") {
            LocalFolder = localFolder;
            Prefix = prefix;
            CurrentSuffix = 'a';
            Files = new List<FileToPack>();
            Bins = new List<DvdBin>();
        }

        public void ProcessFolder() {
            GetFiles();
            Log.Information("read {a} files", Files.Count);
            SortFilesByDescendingSize();
            foreach (FileToPack file in Files) {
                Log.Information("{a} \t {b}", file.File.Name, file.File.Length);
            }
            CreateBin();
            PackBins();
        }

        private void PackBins() {
            foreach (FileToPack fileToPack in Files) {
                if (fileToPack.DvdBinName == null) {
                    Log.Information("Packing {a}", fileToPack.File.Name);
                    for (int i = 0; (i <= Bins.Count) && fileToPack.DvdBinName == null; i++) {
                        if (i == Bins.Count - 1) {
                            CreateBin();
                        }
                        if (Bins[i].FitsFile(fileToPack.File)) {
                            Bins[i].Add(fileToPack.File);
                            fileToPack.DvdBinName = Bins[i].Name;
                            Log.Information("Added {a} to {b}", fileToPack.File.Name, Bins[i].Name);
                        }
                    }
                }
            }
        }

        public string GetBinListing() {
            string report = null; ;
            foreach (DvdBin bin in Bins) {
                if (bin.Files.Count > 0) {
                    report += bin.GetBinContents();
                }
            }
            return report;
        }

        public void GetFiles() {
            if (Directory.Exists(LocalFolder)) {
                var paths = Directory.GetFiles(LocalFolder);
                foreach (string path in paths) {
                    FileToPack fileToPack = new FileToPack {
                        File = new FileInfo(path)
                    };
                    Files.Add(fileToPack);
                }
            }
            else {
                throw new DirectoryNotFoundException($"Could not find folder {LocalFolder}");
            }
        }

        private void SortFilesByDescendingSize() {
            if (Files.Count > 0) {
                Files = Files
                    .OrderByDescending(fi => fi.File.Length)
                    .ToList();
            }
        }

        private void CreateBin() {
            string BinName = GetNewBinName();
            DvdBin dvdBin = new DvdBin(BinName);
            Bins.Add(dvdBin);
            Log.Information("Created bin {a}", dvdBin.Name);
        }

        private string GetNewBinName() {
            string BinNameWithoutSuffix = $"{Prefix}{GetCurrentDateWithDashes()}";
            string BinName = $"{BinNameWithoutSuffix}{CurrentSuffix}";
            Log.Information("new Bin Name: {a}", BinName);
            while (Bins.Any(dvd => dvd.Name == BinName)) {
                CurrentSuffix = (char) (Convert.ToUInt16(CurrentSuffix) + 1);
                BinName = $"{BinNameWithoutSuffix}{CurrentSuffix}";
            }
            return BinName;
        }

        private string GetCurrentDateWithDashes() {
            return $"{DateTime.Now.Year.ToString()}-{DateTime.Now.Month.ToString("d2")}-{DateTime.Now.Day.ToString("d2")}";
        }

        public void MoveFilesIntoBins() {
            if (Bins.Count > 0) {
                foreach (DvdBin dvdBin in Bins) {
                    if (dvdBin.Files.Count > 0) {
                        CreateFolder(dvdBin);
                        MoveFilesIntoBinFolder(dvdBin);
                    }
                }
            }
        }

        private void CreateFolder(DvdBin dvdBin) {
            string newFolder = Path.Combine(LocalFolder, dvdBin.Name);
            if (!Directory.Exists(newFolder)) {
                Directory.CreateDirectory(newFolder);
            }
            else {
                Log.Error("Folder {a} already exists!", newFolder);
            }
        }

        private void MoveFilesIntoBinFolder(DvdBin dvdBin) {
            string targetFolder = Path.Combine(LocalFolder, dvdBin.Name);
            if (Directory.Exists(targetFolder)) {
                foreach (FileInfo file in dvdBin.Files) {
                    file.MoveTo(Path.Combine(targetFolder, file.Name));
                }
            }
            else {
                Log.Error("Folder {a} does not exist. Skipping.", targetFolder);
            }
        }

    }
}
