using FilesOnDvdLocal.Data;
using FilesOnDvdLocal.LocalDbDtos;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories {
    public class PerformerRepository : IPerformerRepository {
        private readonly string DatabasePath;
        private List<PerformerLocalDto> Performers;

        public PerformerRepository(string databasePath) {
            DatabasePath = databasePath;
            Performers = RetrievePerformersFromDatabase();

        }
        private List<PerformerLocalDto> RetrievePerformersFromDatabase() {
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

        public List<PerformerLocalDto> Get() {
            return Performers;
        }

        public PerformerLocalDto Get(string name) {
            PerformerLocalDto perf = Performers.FirstOrDefault(b =>
                b.Name.ToUpper() == name.ToUpper());
            // should null check be here or in caller?
            if (perf == null) {
                perf = new PerformerLocalDto() {
                    Id = 0,
                    Name = name
                };
            }
            return perf;
        }

        public PerformerLocalDto Get(int id) {
            throw new NotImplementedException();
        }

        public void JoinPerformerToFile(int performerId, int filenameId) {
            AccessRetriever retriever = new AccessRetriever(DatabasePath);
            DataSet dataSet = retriever.GetPerformersFilenamesJoinTable();
            DataRow newPerfFileJoin = dataSet.Tables[0].NewRow();
            newPerfFileJoin["PerformerID"] = performerId;
            newPerfFileJoin["FilenameID"] = filenameId;
            dataSet.Tables[0].Rows.Add(newPerfFileJoin);
            retriever.UpdateJoinTable(dataSet);
        }
    }
}
