using FilesOnDvdLocal;
using FilesOnDvdLocal.LocalDbDtos;
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
        private IDataRepository DataRepository;

        public FileToImport SelectedFile { get; set; }

        public DvdFolderToImport FolderToImport;
        public string FolderPath {
            get { return folderPath; }
            set {
                folderPath = value;
                FolderToImport.FolderPath = folderPath;
                FolderToImport.PopulateFiles(DataRepository);
                Files = new ObservableCollection<FileToImport>(FolderToImport.Files);
                FolderToImport.CompileAllPerformersInFolder();
                PerformersInFolder = new ObservableCollection<PerformerLocalDto>(FolderToImport.PerformersInFolderAll);
                OnPropertyChange("FolderPath");
                OnPropertyChange("Files");
                OnPropertyChange("FolderName");
                OnPropertyChange("PerformersInFolder");
            }
        }

        public string FolderName { get { return Path.GetFileName(FolderPath); } }
        public ObservableCollection<FileToImport> Files { get; set; }
        public ObservableCollection<PerformerLocalDto> PerformersInFolder { get; set; }
        public ObservableCollection<PerformerLocalDto> PerformersInDatabase { get; set; }

        public ICommand BrowseFolderCommand { get; private set; }
        public ICommand SaveFilenameListCommand { get; private set; }
        public ICommand RemovePerformerCommand { get; private set; }

        public FolderToImportViewModel() {
            string pathToGetFromSettingsFile = @"C:\temp\SampleFilesToImport";

            // replace this with the real repository later
            AccessMockRepository repository = new AccessMockRepository();
            DataRepository = repository;

            FolderToImport = new DvdFolderToImport(pathToGetFromSettingsFile, repository);
            folderPath = FolderToImport.FolderPath;
            Files = new ObservableCollection<FileToImport>(FolderToImport.Files);
            PerformersInFolder = new ObservableCollection<PerformerLocalDto>(FolderToImport.PerformersInFolderAll);
            PerformersInDatabase = new ObservableCollection<PerformerLocalDto>(DataRepository.GetPerformers().OrderBy(b => b.Name));
            BrowseFolderCommand = new RelayCommand(param => BrowseToFolder());
            SaveFilenameListCommand = new RelayCommand(async param => await SaveFilenameList());
            RemovePerformerCommand = new RelayCommand(param => RemovePerformer(param));
        }

        private void RemovePerformer(object perfName) {
            string perfNameStr = perfName as string;
            string outputMessage;
            if (SelectedFile.Performers.Any(b => b.Name == perfNameStr)) {
                var performerToRemove = SelectedFile.Performers
                    .FirstOrDefault(b => b.Name == perfNameStr);
                SelectedFile.Performers.Remove(performerToRemove);
                outputMessage = $"Removing performer {perfNameStr} from {SelectedFile.Filename}";

                // we don't want to remove the performer directly from PerformersInFolder
                // because they might appear on another file in the folder
                RefreshAllPerformersInFolder();
            }
            else {
                outputMessage = $"Could not find performer {perfNameStr} in {SelectedFile.Filename}";
            }
            MessageBox.Show(outputMessage);
        }

        private void RefreshAllPerformersInFolder() {
            FolderToImport.CompileAllPerformersInFolder();
            PerformersInFolder.Clear();
            PerformersInFolder = new ObservableCollection<PerformerLocalDto>(FolderToImport.PerformersInFolderAll);
            OnPropertyChange("PerformersInFolder");
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
