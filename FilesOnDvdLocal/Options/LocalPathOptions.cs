using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Options {
    public class LocalPathOptions {
        public string DvdToImportStartFolder { get; set; }
        public string MockRepositoryFolder { get; set; }
        public string FileMockRepositoryPath { get; set; }
        public string PerformerMockRepositoryPath { get; set; }
        public string DiscMockRepositoryPath { get; set; }
        public string SeriesMockRepositoryPath { get; set; }
        public string FileListingFolder { get; set; }
        public string DatabasePath { get; set; }
    }
}
