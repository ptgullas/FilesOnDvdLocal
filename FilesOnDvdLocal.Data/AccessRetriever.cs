using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace FilesOnDvdLocal.Data
{
    public class AccessRetriever {
        private OleDbConnection _dbConnection;

        public void OpenDbConnection() {
            _dbConnection = new OleDbConnection {
                ConnectionString = GetConnectionString()
            };
        }

        private string GetConnectionString() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            return $"Provider=Microsoft.ACE.OLEDB.14.0;Data Source={dbLocation}";
        }

        public void CloseDbConnection() {
            _dbConnection.Close();
        }

        public void GetSeries() {
            string strSql = "SELECT * FROM tblSeries";
            OpenDbConnection();
            OleDbDataAdapter myCmd = new OleDbDataAdapter(strSql, _dbConnection);
            _dbConnection.Open();
            DataSet dtSet = new DataSet();
            myCmd.Fill(dtSet, "tblSeries");
            DataTable dataTable = dtSet.Tables[0];
            CloseDbConnection();
        }


    }
}
