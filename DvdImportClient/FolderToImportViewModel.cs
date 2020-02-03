using FilesOnDvdLocal;
using FilesOnDvdLocal.LocalDbDtos;
using FilesOnDvdLocal.Options;
using FilesOnDvdLocal.Repositories;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        public static IConfigurationRoot Configuration;

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
                FolderName = Path.GetFileName(folderPath);
                FolderToImport.DiscName = FolderName;

                FolderToImport.PopulateFiles(performerRepository, seriesRepository);

                GenreLocalDto genreDto = new GenreLocalDto() { Id = 3, Genre = "Stuff" };
                FolderToImport.SetFilesGenre(genreDto);


                Files.Clear();
                Files = new ObservableCollection<FileToImport>(FolderToImport.Files);
                FolderToImport.CompileAllPerformersInFolder();
                PerformersInFolder = new ObservableCollection<PerformerLocalDto>(FolderToImport.PerformersInFolderAll);


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
        public ObservableCollection<SeriesLocalDto> SeriesInDatabase { get; set; }
        public ObservableCollection<OperationResult> ResultMessages { get; set; }

        public ICommand BrowseFolderCommand { get; private set; }
        public ICommand SaveFilenameListCommand { get; private set; }
        public ICommand RemovePerformerCommand { get; private set; }
        public ICommand AddPerformerCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }

        public FolderToImportViewModel() {
            SetUpConfiguration();
            var localFoldersOptions = GetLocalPathOptions();
            bool useMockRepositories = Configuration.GetValue<bool>("UseMockRepositories");

            ResultMessages = new ObservableCollection<OperationResult>();

            if (CheckIfNecessaryLocalFilesExist(localFoldersOptions, useMockRepositories)) {

                RepositoryFactory repositoryFactory = new RepositoryFactory(localFoldersOptions, useMockRepositories);

                IFileRepository fileRepository = repositoryFactory.GetFileRepository();
                performerRepository = repositoryFactory.GetPerformerRepository();

                IDiscRepository discRepository = repositoryFactory.GetDiscRepository();
                importer = new Importer(discRepository, performerRepository, fileRepository);

                seriesRepository = repositoryFactory.GetSeriesRepository();

                string pathToGetFromSettingsFile = localFoldersOptions.DvdToImportStartFolder;
                FolderToImport = new DvdFolderToImport(pathToGetFromSettingsFile, performerRepository, seriesRepository);
                folderPath = FolderToImport.FolderPath;
                FolderName = FolderToImport.DiscName;
                Files = new ObservableCollection<FileToImport>(FolderToImport.Files);
                PerformersInFolder = new ObservableCollection<PerformerLocalDto>(FolderToImport.PerformersInFolderAll);
                PerformersInDatabase = new ObservableCollection<PerformerLocalDto>(performerRepository.Get().OrderBy(b => b.Name));

                SeriesInDatabase = new ObservableCollection<SeriesLocalDto>(seriesRepository.Get().OrderBy(s => s.Name));
                SeriesInDatabase.Insert(0, new SeriesLocalDto());

                BrowseFolderCommand = new RelayCommand(param => BrowseToFolder());
                SaveFilenameListCommand = new RelayCommand(async param => await SaveFilenameList(),d => FolderToImport.IsReadyToImport);
                RemovePerformerCommand = new RelayCommand(param => RemovePerformer(param));
                AddPerformerCommand = new RelayCommand(param => AddPerformer(param));
                ImportCommand = new RelayCommand(param => Import(), d => FolderToImport.IsReadyToImport);
            }
        }


        private static void SetUpConfiguration() {
            string projectRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf(@"\bin"));
            var builder = new ConfigurationBuilder()
                .SetBasePath(projectRoot)
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
            // IConfigurationRoot configuration = builder.Build();
            Configuration = builder.Build();
        }


        private static LocalPathOptions GetLocalPathOptions() {
            LocalPathOptions localPathOptions = new LocalPathOptions();
            IConfigurationSection localFoldersConfig = Configuration.GetSection("LocalPaths");
            ConfigurationBinder.Bind(localFoldersConfig, localPathOptions);
            return localPathOptions;
        }

        private bool CheckIfNecessaryLocalFilesExist(LocalPathOptions localPathOptions, bool useMocks) {
            bool necessaryFilesExist;
            if (!useMocks) {
                necessaryFilesExist = CheckIfFileExistsAndDisplayMessage(localPathOptions.DatabasePath, nameof(localPathOptions.DatabasePath));
            }
            else {
                bool discMockRepoExists = CheckIfFileExistsAndDisplayMessage(localPathOptions.DiscMockRepositoryPath, nameof(localPathOptions.DiscMockRepositoryPath));
                bool fileMockRepoExists = CheckIfFileExistsAndDisplayMessage(localPathOptions.FileMockRepositoryPath, nameof(localPathOptions.FileMockRepositoryPath));
                bool seriesMockRepoExists = CheckIfFileExistsAndDisplayMessage(localPathOptions.SeriesMockRepositoryPath, nameof(localPathOptions.SeriesMockRepositoryPath));
                bool perfMockRepoExists = CheckIfFileExistsAndDisplayMessage(localPathOptions.PerformerMockRepositoryPath, nameof(localPathOptions.PerformerMockRepositoryPath));
                bool joinsMockRepoExists = CheckIfFileExistsAndDisplayMessage(localPathOptions.JoinsMockRepositoryPath, nameof(localPathOptions.JoinsMockRepositoryPath));
                necessaryFilesExist = discMockRepoExists && fileMockRepoExists 
                    && seriesMockRepoExists && perfMockRepoExists && joinsMockRepoExists;
            }
            return necessaryFilesExist;
        }

        private bool CheckIfFileExistsAndDisplayMessage(string path, string nameOfVar) {
            OperationResult result = new OperationResult() { Success = true, Message = $"{nameOfVar} {path} exists!."};
            if (!File.Exists(path)) {
                string message = $"{nameOfVar} {path} not found.";
                result.Success = false;
                result.Message = message;
                Log.Error(message);
            }
            DisplayMessage(result);
            return result.Success;
        }

        private void AddPerformer(object perf) {
            PerformerLocalDto performerToAdd = perf as PerformerLocalDto;
            OperationResult operationResult = new OperationResult();
            if (!SelectedFile.Performers.Any(p => p.Id == performerToAdd.Id)) {
                SelectedFile.Performers.Add(performerToAdd);

                operationResult.Success = true;
                operationResult.Message = $"Added {performerToAdd.Name} to {SelectedFile.Filename}";
                RefreshAllPerformersInFolder();
            }
            else {
                operationResult.Success = false;
                operationResult.Message = $"{performerToAdd.Name} is already in {SelectedFile.Filename}";
            }
            DisplayMessage(operationResult);
        }

        private void RemovePerformer(object perfName) {
            string perfNameStr = perfName as string;
            OperationResult outputResult = new OperationResult();
            if (SelectedFile.Performers.Any(b => b.Name == perfNameStr)) {
                var performerToRemove = SelectedFile.Performers
                    .FirstOrDefault(b => b.Name == perfNameStr);
                SelectedFile.Performers.Remove(performerToRemove);
                outputResult.Success = true;
                outputResult.Message = $"Removing performer {perfNameStr} from {SelectedFile.Filename}";

                // we don't want to remove the performer directly from PerformersInFolder
                // because they might appear on another file in the folder
                RefreshAllPerformersInFolder();
            }
            else {
                outputResult.Success = false;
                outputResult.Message = $"Could not find performer {perfNameStr} in {SelectedFile.Filename}";
            }
            DisplayMessage(outputResult);
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
            var result = importer.Import(FolderToImport);
            if (result.Success) {
                result.Message = $"Successfully imported {FolderToImport.DiscName}!";
            }
            DisplayMessage(result);
        }

        private async Task SaveFilenameList() {
            string fileListingFolder = GetOrCreateFileListingFolder();

            bool successfulSave = await FolderToImport.SaveFilenameListToTextFile(fileListingFolder);
            OperationResult operationResult = new OperationResult() {
                Success = false,
                Message = "Could not save filelist!!"
            };
            if (successfulSave) {
                string fileName = $"{FolderName}.txt";
                string pathToSave = Path.Combine(fileListingFolder, fileName);
                operationResult.Success = true;
                operationResult.Message = $"Saved file listing to {pathToSave}";
            }
            DisplayMessage(operationResult);
        }

        private static string GetOrCreateFileListingFolder() {
            var localPathOptions = GetLocalPathOptions();
            string fileListingFolder = localPathOptions.FileListingFolder;
            if (!Directory.Exists(fileListingFolder)) {
                Directory.CreateDirectory(fileListingFolder);
            }
            return fileListingFolder;
        }

        private void DisplayMessage(OperationResult opResult) {
            ResultMessages.Insert(0, opResult);
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
