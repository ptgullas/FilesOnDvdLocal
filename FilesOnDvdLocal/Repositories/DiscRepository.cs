using FilesOnDvdLocal.Data;
using FilesOnDvdLocal.LocalDbDtos;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories
{
    public class DiscRepository : IDiscRepository
    {
        private List<DiscLocalDto> discs;
        private string databasePath;
        public DiscRepository(string databasePath) {
            this.databasePath = databasePath;
            discs = new List<DiscLocalDto>();
        }

        private void PopulateDiscsFromDatabase(DataTable discsTable) {
            foreach (DataRow row in discsTable.Rows) {
                string idStr = row["ID"].ToString();
                bool result = int.TryParse(idStr, out int id);
                string walletTypeStr = row["Wallet"].ToString();
                result = int.TryParse(walletTypeStr, out int walletType);
                DiscLocalDto disc = new DiscLocalDto() {
                    Id = id,
                    DiscName = row["DiscName"].ToString(),
                    Wallet = walletType,
                    Notes = row["Notes"].ToString()
                };
                discs.Add(disc);
            }
        }

        public void Add(DvdFolderToImport disc) {
            DiscLocalDto discDto = new DiscLocalDto(disc);
            Add(discDto);
        }

        public void Add(DiscLocalDto discDto) {
            AccessRetriever retriever = new AccessRetriever(databasePath);
            DataSet dataSet;
            try {
                dataSet = retriever.GetDiscs();
            }
            catch (Exception e) {
                Log.Error(e, "Could not retrieve discs from database");
                throw new ArgumentException("Could not retrieve discs from database", e);
            }
            DataTable discsTable = dataSet.Tables[0];
            PopulateDiscsFromDatabase(discsTable);
            if (discs.Any(d => d.DiscName == discDto.DiscName)) {
                throw new ArgumentOutOfRangeException(discDto.DiscName, "Disc name already exists in database!");
            }
            else {
                DataRow newRow = discsTable.NewRow();
                newRow["DiscName"] = discDto.DiscName;
                newRow["Wallet"] = discDto.Wallet;
                newRow["Notes"] = discDto.Notes;
                // newRow[2] = 1; // this is supposed to be Performer Type. Can use the column number instead
                discsTable.Rows.Add(newRow);
                var changes = dataSet.GetChanges();
                retriever.UpdateAccessTableFromDataSet(dataSet, "tblDiscs");
            }

        }
    }
}
