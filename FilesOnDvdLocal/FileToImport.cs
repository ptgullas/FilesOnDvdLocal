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
using System.Text.RegularExpressions;

namespace FilesOnDvdLocal {
    public class FileToImport :INotifyPropertyChanged {
        public FileInfo File { get; set; }
        public int? DatabaseId { get; set; }
        public int? Genre { get; set; }
        public SeriesLocalDto Series { get; set; }
        public int? DiscId { get; set; }

        private List<PerformerLocalDto> performers;
        public List<PerformerLocalDto> Performers { 
            get => performers; 
            set => SetField(ref performers, value); 
        }

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
                            // only set the Filename field if the rename doesn't throw exception
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

        public int OverflowFilenameCharacterCount { 
            get {
                if (!NameIsTooLong) { return 0; }
                else { return Filename.Length - 98;  }
            }
        }

        public string FilenameDisplayNonAscii { get; set; }

        public List<string> PerformersString { get; set; }
        public string SeriesString { get; set; }

        public FileToImport(string path, IPerformerRepository perfRepository, 
            ISeriesRepository seriesRepository = null) {
            File = new FileInfo(path);
            Filename = File.Name;
            DatabaseId = null;
            DiscId = null;
            Notes = null;
            SeriesString = FilenameParser.GetSeriesName(Filename);
            Series = new SeriesLocalDto();
            PopulateSeriesFromRepository(seriesRepository);

            PerformersString = new List<string>();
            PerformersString = FilenameParser.GetPerformers(Filename);
            Performers = new List<PerformerLocalDto>();
            PopulatePerformersFromRepository(perfRepository);

            FilenameDisplayNonAscii = GetFilenameWithVisibleNonAscii();
        }

        private void PopulatePerformersFromRepository(IPerformerRepository dataRepository) {
            if (PerformersString.Count > 0) {
                foreach (string perf in PerformersString) {
                    var performerDto = dataRepository.Get(perf);
                    Performers.Add(performerDto);
                }
            }
        }

        private void PopulateSeriesFromRepository(ISeriesRepository seriesRepository) {
            if (SeriesString != null) {
                Series = seriesRepository.Get(SeriesString);
            }
        }

        private void RenameFile(string value) {
            string folderName = Path.GetDirectoryName(File.FullName);
            string destinationPath = Path.Combine(folderName, value);
            Log.Information("Renaming {0} to {1}", filename, destinationPath);
            File.MoveTo(destinationPath);
        }


        public bool NameIsTooLong {
            get { return NameIsTooLongForDvd(); }
        }

        public bool NameContainsNonAscii {
            get { return NameContainsNonAsciiCharacters(); }
        }

        public bool NameContainsDoubleSpaces {
            get => Filename.Contains("  ");
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

        private string GetFilenameWithVisibleNonAscii() {
            string filenameWithVisibleNonAscii = null;
            for (int i = 0; i < Filename.Length; i++) {
                char c = Filename[i];
                if (c < 128) {
                    filenameWithVisibleNonAscii += c;
                }
                else {
                    filenameWithVisibleNonAscii += "Œ";
                }
            }
            return filenameWithVisibleNonAscii;
        }

    }
}
