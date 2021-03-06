﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesOnDvdLocal.LocalDbDtos;
using FilesOnDvdLocal.Options;
using Newtonsoft.Json;
using Serilog;

namespace FilesOnDvdLocal.Repositories
{
    public class PerformerMockRepository : IPerformerRepository {

        private List<PerformerLocalDto> performers;
        private readonly string pathToJson;
        private readonly string pathToJoinsJson;


        public PerformerMockRepository(LocalPathOptions localPathOptions) {
            pathToJson = localPathOptions.PerformerMockRepositoryPath;
            pathToJoinsJson = localPathOptions.JoinsMockRepositoryPath;
            RetrievePerformers();
        }

        public PerformerMockRepository() {
            performers = GetMockPerformers();
        }

        private List<PerformerLocalDto> GetMockPerformers() {
            List<PerformerLocalDto> mockPerformers = new List<PerformerLocalDto>() {
                new PerformerLocalDto() {Id = 1, Name = "Shiv Roy"},
                new PerformerLocalDto() {Id = 2, Name = "Roman Roy"},
                new PerformerLocalDto() {Id = 3, Name = "Kendall Roy"},
                new PerformerLocalDto() {Id = 4, Name = "Tandy Bowen"},
                new PerformerLocalDto() {Id = 5, Name = "Tyrone Johnson"},
                new PerformerLocalDto() {Id = 6, Name = "The Doctor"},
                new PerformerLocalDto() {Id = 7, Name = "Jeff Winger"},
                new PerformerLocalDto() {Id = 8, Name = "Annie Edison"},
                new PerformerLocalDto() {Id = 9, Name = "Britta Perry"},
                new PerformerLocalDto() {Id = 10, Name = "Matt Murdock"},
                new PerformerLocalDto() {Id = 11, Name = "Archie Andrews"},
                new PerformerLocalDto() {Id = 12, Name = "Toni Topaz"},
                new PerformerLocalDto() {Id = 13, Name = "Baby Yoda"},
                new PerformerLocalDto() {Id = 14, Name = "The Mandalorian"},
                new PerformerLocalDto() {Id = 15, Name = "Jon Osterman"},
                new PerformerLocalDto() {Id = 16, Name = "Laurie Blake"},
                new PerformerLocalDto() {Id = 17, Name = "Angela Abar"},
            };
            return mockPerformers;
        }

        private void RetrievePerformers() {
            if (File.Exists(pathToJson)) {
                string jsonContents = File.ReadAllText(pathToJson);
                performers = JsonConvert.DeserializeObject<List<PerformerLocalDto>>(jsonContents);
            }
            else {
                throw new FileNotFoundException("Can't find json file", pathToJson);
            }
        }

        private List<PerformerFilenameJoinDto> RetrieveJoins() {
            List<PerformerFilenameJoinDto> joins;
            if (File.Exists(pathToJoinsJson)) {
                string jsonContents = File.ReadAllText(pathToJoinsJson);
                joins = JsonConvert.DeserializeObject<List<PerformerFilenameJoinDto>>(jsonContents);
            }
            else {
                throw new FileNotFoundException("Can't find json file", pathToJoinsJson);
            }
            return joins;
        }

        public List<PerformerLocalDto> Get() {
            return performers.OrderBy(p => p.Name).ToList();
        }

        public PerformerLocalDto Get(string name) {
            var performer = performers
                .FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            if (performer != null) {
                return performer;
            }
            else {
                return new PerformerLocalDto() {
                    Id = -1,
                    Name = name
                };
            }
        }

        public PerformerLocalDto Get(int id) {
            throw new NotImplementedException();
        }

        public void JoinPerformerToFile(List<PerformerFilenameJoinDto> joinsToAdd) {
            var joinsFromFile = RetrieveJoins();
            foreach (var join in joinsToAdd) {
                joinsFromFile.Add(join);
            }
            string jsonJoins = SerializeJoins(joinsFromFile);
            SaveToFile(jsonJoins, pathToJoinsJson);
        }

        private string SerializeJoins(List<PerformerFilenameJoinDto> joins) {
            return JsonConvert.SerializeObject(joins, Formatting.Indented);
        }

        private void SaveToFile(string jsonContents, string filePath) {
            try {
                File.WriteAllText(filePath, jsonContents);
            }
            catch (Exception e) {
                Log.Error(e, "Could not save Joins json to file {0}", filePath);
            }
        }

    }
}
