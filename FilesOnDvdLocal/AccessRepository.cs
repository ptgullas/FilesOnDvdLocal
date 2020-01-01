using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesOnDvdLocal.Data;
using FilesOnDvdLocal.LocalDbDtos;

namespace FilesOnDvdLocal {
    public class AccessRepository : IDataRepository {

        private readonly string DatabasePath;
        private List<PerformerLocalDto> Performers;

        public AccessRepository(string databasePath) {
            DatabasePath = databasePath;
            Performers = new List<PerformerLocalDto>();
            RetrievePerformers();
        }

        public List<PerformerLocalDto> GetPerformers() {
            return Performers;
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

        private void RetrievePerformers() {
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
                Performers.Add(performer);
            }
        }
    }
}
