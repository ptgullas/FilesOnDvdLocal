using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using Serilog;

namespace FilesOnDvdLocal.Data
{
    public class AccessRetriever {
        public string DatabasePath { get; set; }

        public AccessRetriever(string databasePath) {
            DatabasePath = databasePath;
        }

        private string GetConnectionString() {
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={DatabasePath}";
        }

        public DataSet GetDiscs() {
            return GetAccessTableAsDataSet("SELECT * FROM tblDiscs", "tblDiscs");
        }

        public void UpdateDiscs(DataSet dataSet) {
            using (OleDbConnection connection = new OleDbConnection(GetConnectionString())) {
                OleDbDataAdapter discsAdapter = new OleDbDataAdapter("SELECT * FROM tblDiscs", connection) {
                   // InsertCommand = new OleDbCommand("INSERT INTO tblDiscs (DiscName, Wallet, Notes) VALUES (@DiscName, @WalletNum, @Notes)", connection)
                };
                OleDbCommandBuilder builder = new OleDbCommandBuilder(discsAdapter);
                builder.GetInsertCommand();
                connection.Open();

                discsAdapter.Update(dataSet, "tblDiscs");
            }
        }

        public DataSet GetAccessTableAsDataSet(string sqlCommand, string tableName) {
            DataSet dataSet = new DataSet();
            using (OleDbConnection connection = new OleDbConnection(GetConnectionString())) {
                OleDbCommand selectDiscsCmd = new OleDbCommand($"{sqlCommand}", connection);
                OleDbDataAdapter discsAdapter = new OleDbDataAdapter {
                    SelectCommand = selectDiscsCmd
                };
                discsAdapter.Fill(dataSet, tableName);
            }
            return dataSet;
        }

        public DataSet GetSeriesAndPerformersAndAliases() {
            DataSet dataSet = new DataSet();
            using (OleDbConnection connection = new OleDbConnection(GetConnectionString())) {
                OleDbCommand selectSeriesAndPerformersCmd = new OleDbCommand("SELECT * FROM tblSeries", connection);
                OleDbDataAdapter seriesAdapter = new OleDbDataAdapter {
                    SelectCommand = selectSeriesAndPerformersCmd
                };
                seriesAdapter.Fill(dataSet, "Series");

                OleDbCommand getPerformersCmd = new OleDbCommand("SELECT * FROM tblPerformers", connection);
                OleDbDataAdapter performersAdapter = new OleDbDataAdapter {
                    SelectCommand = getPerformersCmd
                };
                performersAdapter.Fill(dataSet, "Performers");

                OleDbCommand getAliasesCmd = new OleDbCommand("SELECT * FROM tblPerformerAliases", connection);
                OleDbDataAdapter aliasesAdapter = new OleDbDataAdapter {
                    SelectCommand = getAliasesCmd
                };
                aliasesAdapter.Fill(dataSet, "Aliases");
            }
            return dataSet;
        }

        public DataTable GetSeries() {
            DataSet dataSet = new DataSet();
            using (OleDbConnection connection = new OleDbConnection(GetConnectionString())) {
                OleDbCommand getSeriesCmd = new OleDbCommand("SELECT * FROM tblSeries", connection);
                OleDbDataAdapter seriesAdapter = new OleDbDataAdapter {
                    SelectCommand = getSeriesCmd
                };
                try {
                    seriesAdapter.Fill(dataSet);
                }
                catch (Exception e) {
                    Log.Error(e, "Error retrieving Series");
                }
                DataTable seriesTable = dataSet.Tables[0];
                return seriesTable;
            }

        }


    }
}
