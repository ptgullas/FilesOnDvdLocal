using FilesOnDvdLocal.Options;
using FilesOnDvdLocal.Repositories;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public class RepositoryFactory {
        private LocalPathOptions localPathOptions;
        private bool useMockRepositories;
        public RepositoryFactory(LocalPathOptions localPathOptions, bool useMockRepositories) {
            this.localPathOptions = localPathOptions;
            this.useMockRepositories = useMockRepositories;
        }

        public IFileRepository GetFileRepository() {
            if (useMockRepositories) { return CreateFileMockRepository(); }
            else { return CreateFileRepository(); }
        }

        private FileMockRepository CreateFileMockRepository() {
            string pathToJson = localPathOptions.FileMockRepositoryPath;
            ThrowExceptionIfFileNotFound(pathToJson);
            return new FileMockRepository(pathToJson);
        }

        private static void ThrowExceptionIfFileNotFound(string pathToJson) {
            if (!File.Exists(pathToJson)) {
                Log.Error("Path {0} not found!", pathToJson);
                throw new FileNotFoundException("File not found!", Path.GetFileName(pathToJson));
            }
        }

        private FileRepository CreateFileRepository() {
            string dbPath = GetDbPath();
            return new FileRepository(dbPath);
        }

        private string GetDbPath() {
            string dbPath = localPathOptions.DatabasePath;
            ThrowExceptionIfFileNotFound(dbPath);
            return dbPath;
        }

        public IPerformerRepository GetPerformerRepository() {
            if (useMockRepositories) { return CreatePerformerMockRepository(); }
            else { return CreatePerformerRepository(); }
        }
        private PerformerMockRepository CreatePerformerMockRepository() {
            string pathToJson = localPathOptions.PerformerMockRepositoryPath;
            ThrowExceptionIfFileNotFound(pathToJson);
            return new PerformerMockRepository(pathToJson);
        }

        private PerformerRepository CreatePerformerRepository() {
            string dbPath = GetDbPath();
            return new PerformerRepository(dbPath);
        }
    }
}
