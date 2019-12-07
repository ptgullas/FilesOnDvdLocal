using Microsoft.Office.Interop.Access.Dao;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Data
{
    public class AccessAttachmentsRetriever {

        public string DatabasePath { get; set; }

        public AccessAttachmentsRetriever(string databasePath) {
            DatabasePath = databasePath;
        }

        public void GetHeadshots() {
            string sqlCommand = @"select Headshot.FileData as filedata, Headshot.FileName as filename, Headshot.FileType as filetype, Performer as performer FROM tblPerformers";

            var ds = GetAccessTableAsDataSet(sqlCommand, "tblPerformers");
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows) {
                var filename = row["filename"].ToString();
                if (string.IsNullOrWhiteSpace(filename)) continue;
                string performerName = row["performer"].ToString();
                string performerNameUnderscores = performerName.Replace(' ', '_');
                var newFileName = $"{performerNameUnderscores}{Path.GetExtension(filename)}".ToLower();
                var filedata = (byte[])row["filedata"];
                int header = (int)filedata[0];
                byte[] actualFile = new byte[filedata.Length - header];
                Buffer.BlockCopy(filedata, header, actualFile, 0, actualFile.Length);
                // do stuff with byte array!
                string headshotsDirectory = @"C:\temp\headshots";

                if (!Directory.Exists(headshotsDirectory)) {
                    Directory.CreateDirectory(headshotsDirectory);
                }
                string pathToSave = @"C:\temp\headshots\" + newFileName;
                File.WriteAllBytes(pathToSave, actualFile);
            }

        }

        public void GetScreenshots() {
            string sqlCommand = @"select Screenshot.FileData as filedata, Screenshot.FileName as filename, Screenshot.FileType as filetype, ID FROM tblFilenames";

            var ds = GetAccessTableAsDataSet(sqlCommand, "tblFilenames");
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows) {
                var filename = row["filename"].ToString();
                var Id = row["ID"].ToString();
                if (string.IsNullOrWhiteSpace(filename)) continue;
                var newFileName = $"{Id}{Path.GetExtension(filename)}";
                var filedata = (byte[])row["filedata"];
                int header = (int)filedata[0];
                byte[] actualFile = new byte[filedata.Length - header];
                Buffer.BlockCopy(filedata, header, actualFile, 0, actualFile.Length);
                // do stuff with byte array!
                string pathToSave = @"C:\temp\screenshots\" + newFileName;
                File.WriteAllBytes(pathToSave, actualFile);
            }
        }

        public DataSet GetAccessTableAsDataSet(string sqlCommand, string tableName) {
            DataSet ds = new DataSet();
            using (OleDbConnection connection = new OleDbConnection(GetConnectionString())) {
                OleDbCommand selectDiscsCmd = new OleDbCommand($"{sqlCommand}", connection);
                using (OleDbDataAdapter adapter = new OleDbDataAdapter()) {
                    adapter.SelectCommand = selectDiscsCmd;
                    try {
                        adapter.Fill(ds, tableName);
                    }
                    catch (Exception e) {
                        Log.Error(e, "Could not retrieve Access Table");
                    }
                }
            }
            return ds;
        }

        private string GetConnectionString() {
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={DatabasePath}";
        }
    }
}
