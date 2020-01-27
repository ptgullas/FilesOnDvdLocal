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

        private List<FileLocalDto> files;
        private readonly string pathToJson;

        public FileMockRepository(string pathToJson) {
            this.pathToJson = pathToJson;
            files = new List<FileLocalDto>();
        }

        public int Add(FileToImport file) {
            FileLocalDto fileDto = new FileLocalDto(file);
            return Add(fileDto);
        }

        public int Add(FileLocalDto fileDto) {
            int newId = AddFileToList(fileDto); 
            string jsonFiles = SerializeFilenames();
            SaveToFile(jsonFiles, pathToJson);
            return newId;
        }


        public int AddFileToList(FileLocalDto fileDto) {
            int highestId = 0;
            if (files.Any()) {
                highestId = (int) files.Max(f => f.Id);
                highestId++;
            }
            fileDto.Id = highestId;
            files.Add(fileDto);
            return highestId;
        }

        private string SerializeFilenames() {
            return JsonConvert.SerializeObject(files, Formatting.Indented);

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

    }
}
