﻿using FilesOnDvdLocal;
using FilesOnDvdLocal.LocalDbDtos;
using FilesOnDvdLocal.Options;
using FilesOnDvdLocal.Repositories;
using Microsoft.Extensions.Configuration;
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
        public ObservableCollection<SeriesLocalDto> SeriesInDatabase { get; set; }

        public ICommand BrowseFolderCommand { get; private set; }
        public ICommand SaveFilenameListCommand { get; private set; }
        public ICommand RemovePerformerCommand { get; private set; }
        public ICommand AddPerformerCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }

        public FolderToImportViewModel() {
            SetUpConfiguration();
            var localFoldersOptions = GetLocalPathOptions();
            bool useMockRepositories = Configuration.GetValue<bool>("UseMockRepositories");
            RepositoryFactory repositoryFactory = new RepositoryFactory(localFoldersOptions, useMockRepositories);

            var fileRepository = repositoryFactory.GetFileRepository();
            performerRepository = repositoryFactory.GetPerformerRepository();

            string pathToDiscRepositoryJson = @"C:\MyApps\FilesOnDvdLocal\discMockRepo.json";
            DiscMockRepository discRepository = new DiscMockRepository(pathToDiscRepositoryJson);
            discRepository.RetrieveDiscs();
            importer = new Importer(discRepository, performerRepository, fileRepository);

            string pathToSeriesRepositoryJson = @"C:\MyApps\FilesOnDvdLocal\seriesMockRepo.json";
            SeriesMockRepository seriesRepo = new SeriesMockRepository(pathToSeriesRepositoryJson);
            seriesRepository = seriesRepo;

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
            string fileListingFolder = GetOrCreateFileListingFolder();

            bool successfulSave = await FolderToImport.SaveFilenameListToTextFile(fileListingFolder);
            string message = "Could not save filelist!!";
            if (successfulSave) {
                string fileName = $"{FolderName}.txt";
                string pathToSave = Path.Combine(fileListingFolder, fileName);
                message = $"Saved file listing to {pathToSave}";
            }
            MessageBox.Show(message);
        }

        private static string GetOrCreateFileListingFolder() {
            var localPathOptions = GetLocalPathOptions();
            string fileListingFolder = localPathOptions.FileListingFolder;
            if (!Directory.Exists(fileListingFolder)) {
                Directory.CreateDirectory(fileListingFolder);
            }
            return fileListingFolder;
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
