using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesOnDvdLocal.Data;
using FilesOnDvdLocal.LocalDbDtos;
using Serilog;

namespace FilesOnDvdLocal.Repositories {
    public class SeriesRepository : ISeriesRepository {
        private readonly string DatabasePath;
        private List<SeriesLocalDto> Series;

        public SeriesRepository(string databasePath) {
            DatabasePath = databasePath;
            Series = RetrieveSeriesFromDatabase();
        }

        private List<SeriesLocalDto> RetrieveSeriesFromDatabase() {
            List<SeriesLocalDto> allSeries = new List<SeriesLocalDto>();
            AccessRetriever retriever = new AccessRetriever(DatabasePath);
            try {
                var dataSet = retriever.GetAllSeries();
                DataTable seriesTable = dataSet.Tables[0];
                foreach (DataRow row in seriesTable.Rows) {
                    string idStr = row["ID"].ToString();
                    bool result = int.TryParse(idStr, out int id);
                    SeriesLocalDto series = new SeriesLocalDto() {
                        Id = id,
                        Name = row["Series"].ToString()
                    };
                    allSeries.Add(series);
                }
            }
            catch (Exception e) {
                Log.Error(e, "Could not retrieve all Series in database");
            }
            return allSeries;
        }

        public List<SeriesLocalDto> Get() {
            return Series;
        }

        public SeriesLocalDto Get(string name) {
            SeriesLocalDto series = Series.FirstOrDefault(s =>
                s.Name.ToLower() == name.ToLower());
            if (series == null) {
                series = new SeriesLocalDto() {
                    Id = -1,
                    Name = name
                };
            }
            return series;
        }
    }
}
