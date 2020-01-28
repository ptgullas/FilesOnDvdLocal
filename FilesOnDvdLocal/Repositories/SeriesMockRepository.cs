using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories {
    public class SeriesMockRepository : ISeriesRepository {

        private List<SeriesLocalDto> series;
        private string pathToJson;

        public List<SeriesLocalDto> Get() {
            throw new NotImplementedException();
        }

        public SeriesLocalDto Get(string name) {
            throw new NotImplementedException();
        }
    }
}
