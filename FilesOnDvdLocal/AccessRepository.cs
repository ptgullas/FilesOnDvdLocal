using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesOnDvdLocal.Data;
using FilesOnDvdLocal.LocalDbDtos;
using Serilog;

namespace FilesOnDvdLocal {
    public class AccessRepository : IDataRepository {

        private readonly string DatabasePath;
        private List<PerformerLocalDto> Performers;
        private List<SeriesLocalDto> Series;

        public AccessRepository(string databasePath) {
            DatabasePath = databasePath;
            Performers = RetrievePerformers();
            Series = RetrieveSeries();
        }

        public List<PerformerLocalDto> GetPerformers() {
            return Performers;
        }

        public List<SeriesLocalDto> GetAllSeries() {
            return Series;
        }

        public void AddDisc(DvdFolderToImport dvd) {
            AccessRetriever retriever = new AccessRetriever(DatabasePath);
            try {
                DataSet dataSet = retriever.GetDiscs();
                DataRow newDisc = dataSet.Tables[0].NewRow();
                newDisc["DiscName"] = dvd.DiscName;
                newDisc["Wallet"] = dvd.WalletType;
                newDisc["Notes"] = dvd.Notes;
                dataSet.Tables[0].Rows.Add(newDisc);
                retriever.UpdateDiscs(dataSet);
            }
            catch (Exception e) {
                Log.Error(e, "Error adding disc {0} to database", dvd.DiscName);
            }
        }

        public int GetDiscIdByName(string discName) {
            AccessRetriever retriever = new AccessRetriever(DatabasePath);
            int discId = -1;
            try {
                DataSet dataSet = retriever.GetDiscs();
                DataTable discsTable = dataSet.Tables[0];
                DataRow[] selectedDiscsRows = discsTable.Select($"DiscName = '{discName}'");
                string discIdStr = selectedDiscsRows[0]["ID"].ToString();
                bool isInt = int.TryParse(discIdStr, out discId);
            }
            catch (Exception e) {
                Log.Error(e, "Could not retrieve Disc {0}", discName);
            }
            return discId;
        }

        public PerformerLocalDto GetPerformerByName(string performerName) {
            PerformerLocalDto perf = Performers.FirstOrDefault(b =>
                b.Name.ToUpper() == performerName.ToUpper());
            // should null check be here or in caller?
            if (perf == null) {
                perf = new PerformerLocalDto() {
                    Id = 0,
                    Name = performerName
                };
            }
            return perf;
        }

        public SeriesLocalDto GetSeriesByName(string seriesName) {
            SeriesLocalDto series = Series.FirstOrDefault(s =>
                s.Name.ToLower() == seriesName.ToLower());
            if (series == null) {
                series = new SeriesLocalDto() {
                    Id = -1,
                    Name = seriesName
                };
            }
            return series;
        }

        private List<SeriesLocalDto> RetrieveSeries() {
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

        private List<PerformerLocalDto> RetrievePerformers() {
            List<PerformerLocalDto> allPerformers = new List<PerformerLocalDto>();
            try {
                AccessRetriever retriever = new AccessRetriever(DatabasePath);
                DataSet resultsetFromDb = retriever.GetPerformers();
                DataTable performersTable = resultsetFromDb.Tables[0];
                foreach (DataRow row in performersTable.Rows) {
                    string idStr = row["ID"].ToString();
                    bool result = int.TryParse(idStr, out int id);
                    PerformerLocalDto performer = new PerformerLocalDto() {
                        Id = id,
                        Name = row["Performer"].ToString()
                    };
                    allPerformers.Add(performer);
                }
            }
            catch (Exception e) {
                Log.Error(e, "Could not retrieve performers from database");
                throw new ArgumentException("Could not retrieve performers from database", e);
            }
            return allPerformers;
        }
    }
}
