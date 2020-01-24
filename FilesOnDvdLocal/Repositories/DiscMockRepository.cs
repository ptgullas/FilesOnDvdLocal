using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Serilog;

namespace FilesOnDvdLocal.Repositories {
    public class DiscMockRepository : IDiscRepository {

        private List<DiscLocalDto> Discs;
        private readonly string PathToJson;

        public DiscMockRepository(string pathToJson) {
            Discs = new List<DiscLocalDto>();
            PathToJson = pathToJson;
        }
        public int Add(DvdFolderToImport disc) {
            int newId = AddDiscToList(disc);
            string jsonDiscs = SerializeDiscs();
            SaveToFile(jsonDiscs, PathToJson);
            return newId;
        }

        // for testing
        public void Add(DiscLocalDto disc) {
            Discs.Add(disc);
        }
        // for testing
        public void Save() {
            string jsonDiscs = SerializeDiscs();
            SaveToFile(jsonDiscs, PathToJson);
        }

        public void RetrieveDiscs() {
            if (File.Exists(PathToJson)) {
                string jsonContents = File.ReadAllText(PathToJson);
                Discs = JsonConvert.DeserializeObject<List<DiscLocalDto>>(jsonContents);
            }
            else {
                throw new FileNotFoundException("Can't find json file", PathToJson);
            }

        }

        private int AddDiscToList(DvdFolderToImport disc) {
            DiscLocalDto discDto = new DiscLocalDto(disc);
            if (!Discs.Any(d => d.DiscName == discDto.DiscName)) {
                int highestId = Discs.Max(d => d.Id);
                discDto.Id = highestId + 1;
                Discs.Add(discDto);
            }
            else {
                throw new ArgumentOutOfRangeException(disc.DiscName, "Disc name already exists in Database");
            }
            return discDto.Id;
        }

        private string SerializeDiscs() {
            return JsonConvert.SerializeObject(Discs, Formatting.Indented);
        }

        private void SaveToFile(string jsonContents, string filePath) {
            try {
                File.WriteAllText(filePath, jsonContents);
            }
            catch(Exception e) {
                Log.Error(e, "Could not save Discs json to file {0}", filePath);
            }
        }
    }
}
