using FilesOnDvdLocal;
using FilesOnDvdLocal.LocalDbDtos;
using FilesOnDvdLocal.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DvdImportClient {
    public class FolderToImportViewModel : INotifyPropertyChanged {
        private string folderPath;
        private IPerformerRepository performerRepository;
        private ISeriesRepository seriesRepository;
        private Importer importer;

        private FileToImport selectedFile;
        public FileToImport SelectedFile { 
            get => selectedFile;
            set => SetField(ref selectedFile, value);
        }

        public DvdFolderToImport FolderToImport;
        public string FolderPath {
            get => folderPath;
            set {
                SetField(ref folderPath, value); // calls OnPropertyChange
                FolderToImport.FolderPath = folderPath;
                FolderToImport.PopulateFiles(performerRepository, seriesRepository);
                Files.Clear();
                Files = new ObservableCollection<FileToImport>(FolderToImport.Files);
                FolderToImport.CompileAllPerformersInFolder();
                PerformersInFolder = new ObservableCollection<PerformerLocalDto>(FolderToImport.PerformersInFolderAll);

                FolderName = Path.GetFileName(folderPath);

                OnPropertyChange("Files"); // update the Files list
                OnPropertyChange("PerformersInFolder");
            }
        }

        // public string FolderName { get { return Path.GetFileName(FolderPath); } }
        private string folderName;
        public string FolderName { get => folderName; set => SetField(ref folderName, value); }

        public ObservableCollection<FileToImport> Files { get; set; }
        public ObservableCollection<PerformerLocalDto> PerformersInFolder { get; set; }
        public ObservableCollection<PerformerLocalDto> PerformersInDatabase { get; set; }

        public ICommand BrowseFolderCommand { get; private set; }
        public ICommand SaveFilenameListCommand { get; private set; }
        public ICommand RemovePerformerCommand { get; private set; }
        public ICommand AddPerformerCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }

        public FolderToImportViewModel() {

            // replace this with the real repository later
            string pathToFileRepositoryJson = @"C:\MyApps\FilesOnDvdLocal\fileMockRepo.json";
            FileMockRepository fileRepository = new FileMockRepository(pathToFileRepositoryJson);

            PerformerMockRepository perfRepository = new PerformerMockRepository();
            performerRepository = perfRepository;

            string pathToDiscRepositoryJson = @"C:\MyApps\FilesOnDvdLocal\discMockRepo.json";
            DiscMockRepository discRepository = new DiscMockRepository(pathToDiscRepositoryJson);
            discRepository.RetrieveDiscs();
            importer = new Importer(discRepository, perfRepository, fileRepository);

            string pathToSeriesRepositoryJson = @"C:\MyApps\FilesOnDvdLocal\seriesMockRepo.json";
            SeriesMockRepository seriesRepo = new SeriesMockRepository(pathToSeriesRepositoryJson);
            seriesRepository = seriesRepo;

            string pathToGetFromSettingsFile = @"C:\temp\SampleFilesToImport";
            FolderToImport = new DvdFolderToImport(pathToGetFromSettingsFile, performerRepository, seriesRepository);
            folderPath = FolderToImport.FolderPath;
            FolderName = FolderToImport.DiscName;
            Files = new ObservableCollection<FileToImport>(FolderToImport.Files);
            PerformersInFolder = new ObservableCollection<PerformerLocalDto>(FolderToImport.PerformersInFolderAll);
            PerformersInDatabase = new ObservableCollection<PerformerLocalDto>(performerRepository.Get().OrderBy(b => b.Name));

            BrowseFolderCommand = new RelayCommand(param => BrowseToFolder());
            SaveFilenameListCommand = new RelayCommand(async param => await SaveFilenameList(),d => FolderToImport.IsReadyToImport);
            RemovePerformerCommand = new RelayCommand(param => RemovePerformer(param));
            AddPerformerCommand = new RelayCommand(param => AddPerformer(param));
            ImportCommand = new RelayCommand(param => Import(), d => FolderToImport.IsReadyToImport);
        }

        private void AddPerformer(object perf) {
            PerformerLocalDto performerToAdd = perf as PerformerLocalDto;
            string outputMessage;
            if (!SelectedFile.Performers.Any(p => p.Id == performerToAdd.Id)) {
                SelectedFile.Performers.Add(performerToAdd);
                outputMessage = $"Added {performerToAdd.Name} to {SelectedFile.Filename}";
                RefreshAllPerformersInFolder();
            }
            else {
                outputMessage = $"{performerToAdd.Name} is already in {SelectedFile.Filename}";
            }
            MessageBox.Show(outputMessage);
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

        private void Import() {
            importer.Import(FolderToImport);
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

        protected void OnPropertyChange([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChange(propertyName);
            return true;
        }
    }
}
