using FilesOnDvdLocal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                FolderToImport.PopulateFiles();
                Files = new ObservableCollection<FileToImport>(FolderToImport.Files);
                OnPropertyChange("FolderPath");
                OnPropertyChange("Files");
                OnPropertyChange("FolderName");
            }
        }

        public string FolderName { get { return Path.GetFileName(FolderPath); } }

        public ObservableCollection<FileToImport> Files {
            get; set;
        }
        public ICommand BrowseFolderCommand { get; private set; }
        public ICommand SaveFilenameListCommand { get; private set; }

        public FolderToImportViewModel() {
            string pathToGetFromSettingsFile = @"C:\temp\Transfer to HD\keepinganonymous1";
            FolderToImport = new DvdFolderToImport(pathToGetFromSettingsFile);
            folderPath = FolderToImport.FolderPath;
            Files = new ObservableCollection<FileToImport>(FolderToImport.Files);
            BrowseFolderCommand = new RelayCommand(param => BrowseToFolder());
            SaveFilenameListCommand = new RelayCommand(async param => await SaveFilenameList());
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

        private async Task SaveFilenameList() {
            // will need to grab this path from settings later.
            string folderPathToSave = @"c:\temp";
            bool successfulSave = await FolderToImport.SaveFilenameListToTextFile(folderPathToSave);
            string message = "Could not save filelist!!";
            if (successfulSave) {
                string fileName = $"{FolderName}.txt";
                string pathToSave = Path.Combine(folderPathToSave, fileName);
                message = $"Successfully saved to {pathToSave}";
            }
            MessageBox.Show(message);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChange(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
