using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public class DvdBin {
        public string Name { get; set; }
        public double SizeInBytes { get; set; }
        public List<FileInfo> Files { get; set; }

        public DvdBin(string name, double bytes = 4692251770.88) {
            SizeInBytes = bytes;
            Name = name;
            Files = new List<FileInfo>();
        }

        public void Add(FileInfo file) {
            Files.Add(file);
        }

        public bool FitsFile(FileInfo file) {
            if ((GetFreeRoomOnDisc() - file.Length) > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        public double GetFreeRoomOnDisc() {
            return SizeInBytes - GetCurrentSize();
        }

        public double GetCurrentSize() {
            return Files.Sum(fi => fi.Length);
        }

        public double GetCurrentSizeInKb() {
            return GetCurrentSize() / 1024;
        }

        public double GetCurrentSizeInMb() {
            return GetCurrentSizeInKb() / 1024;
        }

        public double GetCurrentSizeInGb() {
            return GetCurrentSizeInMb() / 1024;
        }

        public string GetBinContents() {
            if (Files.Count > 0) {
                var FilesBySize = Files.OrderByDescending(fi => fi.Length);
                string report = $"{Name}\n";
                foreach (FileInfo file in FilesBySize) {
                    report += $"{file.Name}\t{file.Length}\n";
                }
                report += $"Total Size: {GetCurrentSizeInGb().ToString("N2")} Gb\n";
                report += "-------------------------\n";
                return report;
            }
            else {
                return $"{Name} is empty";
            }
        }
    }
}
