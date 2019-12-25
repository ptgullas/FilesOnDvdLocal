using FilesOnDvdLocal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvdImportClient {
    public class FolderToImportViewModel : INotifyPropertyChanged {
        public DvdFolderToImport FolderToImport;
        public string FolderPath {
            get { return FolderToImport.FolderPath; } 
            set {
                FolderToImport.FolderPath = value;
                FolderToImport.GetFiles();
                OnPropertyChange("Files");

            } 
        }

        public string FolderName {
            get { return Path.GetFileName(FolderPath); }
        }

        public List<FileToImport> Files {
            get; set;
        }

        public FolderToImportViewModel() {
            string pathToGetFromSettingsFile = @"C:\temp\Transfer to HD\keepinganonymous1";
            FolderToImport = new DvdFolderToImport(pathToGetFromSettingsFile);
            Files = FolderToImport.Files;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChange(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
