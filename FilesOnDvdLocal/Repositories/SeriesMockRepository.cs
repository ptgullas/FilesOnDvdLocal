using FilesOnDvdLocal.LocalDbDtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories {
    public class SeriesMockRepository : ISeriesRepository {

        private List<SeriesLocalDto> series;
        private string pathToJson;

        public SeriesMockRepository(string pathToJson) {
            series = new List<SeriesLocalDto>();
            this.pathToJson = pathToJson;
            RetrieveSeries();
        }

        private void RetrieveSeries() {
            if (File.Exists(pathToJson)) {
                string jsonContents = File.ReadAllText(pathToJson);
                series = JsonConvert.DeserializeObject<List<SeriesLocalDto>>(jsonContents);
            }
            else {
                throw new FileNotFoundException("Can't find json file", pathToJson);
            }
        }


        public List<SeriesLocalDto> Get() {
            return series;
        }

        public SeriesLocalDto Get(string name) {
            return series.FirstOrDefault(s => s.Name.ToLower() == name.ToLower());
        }
    }
}
