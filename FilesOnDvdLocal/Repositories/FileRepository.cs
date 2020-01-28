using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesOnDvdLocal.Data;
using FilesOnDvdLocal.LocalDbDtos;
using Serilog;

namespace FilesOnDvdLocal.Repositories
{
    public class FileRepository : IFileRepository
    {
        private string databasePath;

        public FileRepository(string databasePath) {
            this.databasePath = databasePath;
        }

        public int Add(FileToImport file) {
            throw new NotImplementedException();
        }

        public int Add(FileLocalDto fileDto) {
            throw new NotImplementedException();
        }

        public void Add(List<FileToImport> files) {
            if (files != null && files.Count > 0) {
                List<FileLocalDto> fileDtos = new List<FileLocalDto>();
                foreach (FileToImport file in files) {
                    FileLocalDto fileDto = new FileLocalDto(file);
                    fileDtos.Add(fileDto);
                }
                Add(fileDtos);
            }
        }

        public void Add(List<FileLocalDto> fileDtos) {
            AccessRetriever retriever = new AccessRetriever(databasePath);

            DataSet dataSet = RetrieveFileEntriesFromDatabase(retriever);
            DataTable filenamesTable = dataSet.Tables[0];
            foreach (FileLocalDto file in fileDtos) {
                DataRow newRow = filenamesTable.NewRow();
                newRow["Filename"] = file.Filename;
                newRow["Genre"] = file.Genre;
                newRow["Disc"] = file.Disc;
                if (file.Series is null) {
                    newRow["Series"] = DBNull.Value;
                }
                else {
                    newRow["Series"] = file.Series;
                }
                filenamesTable.Rows.Add(newRow);
            }
            retriever.UpdateFileEntries(dataSet);
        }

        private DataSet RetrieveFileEntriesFromDatabase(AccessRetriever retriever) {
            DataSet dataSet;
            try {
                dataSet = retriever.GetFileEntries();
            }
            catch (Exception e) {
                Log.Error(e, "Could not retrieve filenames from Access DB");
                throw new InvalidOperationException("Could not retrieve filenames from Access DB", e);
            }
            if (dataSet is null) { throw new InvalidOperationException("Filenames dataSet is null"); }
            return dataSet;
        }

        public List<FileLocalDto> GetByDisc(int discId) {
            throw new NotImplementedException();
        }
    }
}
