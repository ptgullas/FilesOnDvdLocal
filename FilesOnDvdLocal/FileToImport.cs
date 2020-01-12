using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FilesOnDvdLocal.Repositories;

namespace FilesOnDvdLocal {
    public class FileToImport : INotifyPropertyChanged {
        public FileInfo File { get; set; }
        public SeriesLocalDto Series {get; set;}
        public List<PerformerLocalDto> Performers { get; set; }

        public string Notes { get; set; }
        public bool Unwatched { get; set; }

        private string filename;
        public string Filename { 
            get => filename;
            set { 
                if (!string.IsNullOrEmpty(filename)) {
                    if (value != filename) {
                        try {
                            RenameFile(value);
                            SetField(ref filename, value);
                        }
                        catch (Exception e) {
                            Log.Error(e, "Could not rename file {0} to {1}", filename, value);
                        }
                    }
                }
                // filename is null/empty; it is being set for the first time, so don't rename it
                else {
                    SetField(ref filename, value);
                }
            } 
        }

        private void RenameFile(string value) {
            string folderName = Path.GetDirectoryName(File.FullName);
            value += "_moved";
            string destinationPath = Path.Combine(folderName, value);
            Log.Information("Renaming {0} to {1}", filename, destinationPath);
            File.MoveTo(destinationPath);
        }

        public List<string> PerformersString { get; set; }
        public string SeriesString { get; set; }



        public FileToImport(string path, IPerformerRepository dataRepository) {
            File = new FileInfo(path);
            Filename = File.Name;
            PerformersString = new List<string>();
            PerformersString = GetPerformersFromFilename();
            SeriesString = GetSeriesNameFromFilename();
            Performers = new List<PerformerLocalDto>();
            PopulatePerformersFromRepository(dataRepository);
        }

        private void PopulatePerformersFromRepository(IPerformerRepository dataRepository) {
            if (PerformersString.Count > 0) {
                foreach (string perf in PerformersString) {
                    var performerDto = dataRepository.Get(perf);
                    Performers.Add(performerDto);
                }
            }
        }

        public bool NameIsTooLong {
            get { return NameIsTooLongForDvd(); }
        }

        public bool NameContainsNonAscii {
            get { return NameContainsNonAsciiCharacters(); }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChange([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChange(propertyName);
            return true;
        }

        private bool NameIsTooLongForDvd(int maxLength = 98) {
            int fileNameLength = File.Name.Length;
            if (fileNameLength > maxLength) {
                return true;
            }
            else {
                return false;
            }
        }

        private bool NameContainsNonAsciiCharacters() {
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

        public void RenameFileFromFilename() {
            string currentPath = File.FullName;
            string directory = Path.GetDirectoryName(currentPath);
            string newPath = Path.Combine(directory, Filename);
            if (newPath != currentPath) {
                try {
                    File.MoveTo(newPath);
                }
                catch (Exception e) {
                    Log.Error(e, "Error renaming file {0} to {1}", currentPath, newPath);
                }
            }
        }

        public string GetSeriesNameFromFilename() {
            string seriesName = null;
            string filename = File.Name;
            int indexOfHyphen = filename.IndexOf("-");
            if (indexOfHyphen >= 2) {
                seriesName = filename.Substring(0, indexOfHyphen - 1).Trim();
            }
            return seriesName;
        }

        // We're assuming that a list of performers will always be after a hyphen and a 
        // SecondDelimeter (hyphen or open paren).
        public List<string> GetPerformersFromFilename() {
            List<string> performers = new List<string>();
            string filename = File.Name;
            int indexOfHyphen = filename.IndexOf("-");
            if (indexOfHyphen >= 0) {
                int indexOfSecondDelimeter = GetIndexOfNextDelimeter(filename, indexOfHyphen + 1);
                if (indexOfSecondDelimeter >= 0) {
                    string performersSubstring = filename.Substring(indexOfHyphen + 1, indexOfSecondDelimeter - indexOfHyphen - 1).Trim();
                    List<string> performersSubstringSplitByComma = performersSubstring.Split(',').ToList();
                    if (performersSubstring.Contains("&")) {
                        SplitByAmpersandsAndAddToPerformers(performers, performersSubstringSplitByComma);
                    }
                    else {
                        foreach (string performer in performersSubstringSplitByComma) {
                            performers.Add(performer.Trim());
                        }
                    }
                }
            }
            return performers;
        }

        private static void SplitByAmpersandsAndAddToPerformers(List<string> performers, List<string> performersSubstringSplitByComma) {
            foreach (string performerMightContainAmpersand in performersSubstringSplitByComma) {
                if (!performerMightContainAmpersand.Contains("&")) {
                    performers.Add(performerMightContainAmpersand.Trim());
                }
                else {
                    List<string> tempPerformersSplitAmpersand = performerMightContainAmpersand.Split('&').ToList();
                    foreach (string performer in tempPerformersSplitAmpersand) {
                        performers.Add(performer.Trim());
                    }
                }
            }
        }

        private static int GetIndexOfNextDelimeter(string filename, int start) {
            int indexOfHyphen = filename.IndexOf("-", start);
            int indexOfOpenParen = filename.IndexOf("(", start);

            if (indexOfHyphen < 0) {
                return indexOfOpenParen;
            }
            else if ((indexOfHyphen >= 0) && (indexOfOpenParen >= 0)) {
                return Math.Min(indexOfHyphen, indexOfOpenParen);
            }
            else {
                return indexOfHyphen;
            }
        }
    }
}
