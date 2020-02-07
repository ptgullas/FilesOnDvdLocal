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
        private List<AliasLocalDto> Aliases;

        public PerformerRepository(string databasePath) {
            DatabasePath = databasePath;
            Performers = RetrievePerformersFromDatabase();
            Aliases = RetrieveAliasesFromDatabase();
        }
        private List<PerformerLocalDto> RetrievePerformersFromDatabase() {
            List<PerformerLocalDto> allPerformers = new List<PerformerLocalDto>();
            try {
                AccessRetriever retriever = new AccessRetriever(DatabasePath);
                DataSet resultsetFromDb = retriever.GetPerformers();
                DataTable performersTable = resultsetFromDb.Tables[0];
                foreach (DataRow row in performersTable.Rows) {
                    int? id = GetStringValueAsInt(row, "ID");
                    PerformerLocalDto performer = new PerformerLocalDto() {
                        Id = (int) id,
                        Name = row["Performer"].ToString()
                    };
                    allPerformers.Add(performer);
                }
                Log.Information("Read {0} performers from database.", allPerformers.Count);
            }
            catch (Exception e) {
                Log.Error(e, "Could not retrieve performers from database");
            }
            return allPerformers;
        }

        private List<AliasLocalDto> RetrieveAliasesFromDatabase() {
            List<AliasLocalDto> aliases = new List<AliasLocalDto>();
            try {
                AccessRetriever retriever = new AccessRetriever(DatabasePath);
                DataSet ds = retriever.GetAliases();
                DataTable dt = ds.Tables[0];
                foreach (DataRow row in dt.Rows) {
                    int? id = GetStringValueAsInt(row, "ID");
                    int? performerId = GetStringValueAsInt(row, "Performer");
                    AliasLocalDto aliasDto = new AliasLocalDto() {
                        Id = (int) id,
                        Performer = (int) performerId,
                        Alias = row["Alias"].ToString()
                    };
                    aliases.Add(aliasDto);
                }
                Log.Information("Read {0} aliases from database", aliases.Count);
            }
            catch (Exception e) {
                Log.Error(e, "Could not retrieve aliases from database");
            }
            return aliases;
        }

        private int? GetStringValueAsInt(DataRow row, string columnName) {
            string intStr = row[columnName].ToString();
            bool result = int.TryParse(intStr, out int intInt);
            if (result) { return intInt; }
            else { return null;  }
        }

        public List<PerformerLocalDto> Get() {
            return Performers;
        }

        public PerformerLocalDto Get(string name) {
            PerformerLocalDto perf = Performers.FirstOrDefault(b =>
                b.Name.ToUpper() == name.ToUpper());
            if (perf == null) {
                AliasLocalDto aliasDto = GetAlias(name);
                if (aliasDto == null) {
                    perf = new PerformerLocalDto() {
                        Id = -1,
                        Name = name
                    };
                }
                else {
                    perf = Performers.Find(p => p.Id == aliasDto.Performer);
                }
            }
            return perf;
        }

        private AliasLocalDto GetAlias(string name) {
            AliasLocalDto alias = Aliases
                .FirstOrDefault(a => a.Alias.ToUpper() == name.ToUpper());
            return alias;
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

        public void JoinPerformerToFile(List<PerformerFilenameJoinDto> joins) {
            AccessRetriever retriever = new AccessRetriever(DatabasePath);
            DataSet dataSet = retriever.GetPerformersFilenamesJoinTable();
            DataTable joinTable = dataSet.Tables[0];
            foreach (PerformerFilenameJoinDto join in joins) {
                DataRow newPerfFileJoin = joinTable.NewRow();
                newPerfFileJoin["PerformerID"] = join.PerformerId;
                newPerfFileJoin["FilenameID"] = join.FilenameId;
                joinTable.Rows.Add(newPerfFileJoin);
            }
            retriever.UpdateJoinTable(dataSet);
        }

        public void Add(PerformerLocalDto performer) {
            AccessRetriever retriever = new AccessRetriever(DatabasePath);
            DataSet dataSet = retriever.GetPerformers();
            DataTable performersTable = dataSet.Tables[0];
            DataRow newRow = dataSet.Tables[0].NewRow();
            newRow["Performer"] = performer.Name;
            // newRow[2] = 1; // this is supposed to be Performer Type. Can use the column number instead
            performersTable.Rows.Add(newRow);
            var changes = dataSet.GetChanges();
            retriever.UpdateAccessTableFromDataSet(dataSet, "tblPerformers", "ID, Performer");

        }
    }
}
