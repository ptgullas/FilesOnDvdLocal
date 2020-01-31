using FilesOnDvdLocal.Options;
using FilesOnDvdLocal.Repositories;
using System;
using System.Collections.Generic;
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
            return new FileMockRepository(localPathOptions.FileMockRepositoryPath);
        }

        private FileRepository CreateFileRepository() {
            return new FileRepository(localPathOptions.DatabasePath);
        }
    }
}
