using FilesOnDvdLocal.LocalDbDtos;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories {
    public class FileMockRepository : IFileRepository {

        private List<FileLocalDto> fileDtos;
        private readonly string pathToJson;

        public FileMockRepository(string pathToJson) {
            this.pathToJson = pathToJson;
            fileDtos = new List<FileLocalDto>();
            RetrieveFiles();
        }

        public void RetrieveFiles() {
            if (File.Exists(pathToJson)) {
                string jsonContents = File.ReadAllText(pathToJson);
                fileDtos = JsonConvert.DeserializeObject<List<FileLocalDto>>(jsonContents);
            }
            else {
                throw new FileNotFoundException("Can't find json file", pathToJson);
            }
        }

        public int Add(FileToImport file) {
            FileLocalDto fileDto = new FileLocalDto(file);
            return Add(fileDto);
        }

        public int Add(FileLocalDto fileDto) {
            int newId = AddFileToList(fileDto);
            SerializeFilenamesAndSave();
            return newId;
        }

        private void SerializeFilenamesAndSave() {
            string jsonFiles = SerializeFilenames();
            SaveToFile(jsonFiles, pathToJson);
        }

        public int AddFileToList(FileLocalDto fileDto) {
            int highestId = 0;
            if (fileDtos.Any()) {
                highestId = (int) fileDtos.Max(f => f.Id);
                highestId++;
            }
            fileDto.Id = highestId;
            fileDtos.Add(fileDto);
            return highestId;
        }

        private string SerializeFilenames() {
            return JsonConvert.SerializeObject(fileDtos, Formatting.Indented);

        }

        private void SaveToFile(string jsonContents, string filePath) {
            try {
                File.WriteAllText(filePath, jsonContents);
            }
            catch (Exception e) {
                Log.Error(e, "Could not save Filenames json to file {0}", filePath);
            }
        }


        public List<FileLocalDto> GetByDisc(int discId) {
            throw new NotImplementedException();
        }

        public void Add(List<FileToImport> files) {
            if (files?.Count == 0) { throw new ArgumentOutOfRangeException(nameof(files), "No files to import!"); }
            foreach (FileToImport file in files) {
                FileLocalDto fileDto = new FileLocalDto(file);
                AddFileToList(fileDto);
            }
            SerializeFilenamesAndSave();
        }
    }
}
