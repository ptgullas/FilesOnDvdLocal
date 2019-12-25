using FilesOnDvdLocal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DvdImportClient {
    public class FolderToImportViewModel : INotifyPropertyChanged {
        private string folderPath;
        public DvdFolderToImport FolderToImport;
        public string FolderPath {
            get { return folderPath; }
            set {
                folderPath = value;
                FolderToImport.FolderPath = folderPath;
                FolderToImport.GetFiles();
                OnPropertyChange("FolderPath");

            }
        }

        public string FolderName {
            get { return Path.GetFileName(FolderPath); }
        }

        public List<FileToImport> Files {
            get; set;
        }
        public ICommand BrowseFolderCommand { get; private set; }

        public FolderToImportViewModel() {
            string pathToGetFromSettingsFile = @"C:\temp\Transfer to HD\keepinganonymous1";
            FolderToImport = new DvdFolderToImport(pathToGetFromSettingsFile);
            FolderPath = FolderToImport.FolderPath;
            Files = FolderToImport.Files;
            BrowseFolderCommand = new RelayCommand(param => BrowseToFolder());
        }


        private void BrowseToFolder() {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog {
                Description = "Browse to the folder to import to the database",
                UseDescriptionForTitle = true
            };
            if (Directory.Exists(FolderPath)) {
                dialog.SelectedPath = FolderPath;
            }
            bool? result = dialog.ShowDialog();
            if (result == true) {
                FolderPath = dialog.SelectedPath;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChange(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
