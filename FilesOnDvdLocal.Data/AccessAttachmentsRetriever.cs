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

        public void RetrieveHeadshots(string saveFolder) {
            string attachmentColumnName = "Headshot";
            string additionalSelectCommand = ", " + "Performer as performer";
            string tableName = "tblPerformers";

            DataTable dt = GetAccessAttachmentsAsTable(attachmentColumnName, tableName, additionalSelectCommand);
            foreach (DataRow row in dt.Rows) {
                var filename = row["filename"].ToString();
                if (string.IsNullOrWhiteSpace(filename)) continue;

                string performerName = row["performer"].ToString();
                string performerNameWithUnderscores = performerName.Replace(' ', '_');
                var newFileName
                    = $"{performerNameWithUnderscores}{Path.GetExtension(filename)}".ToLower();

                byte[] fileContents = GetAttachmentContents(row);

                string pathToSave = Path.Combine(saveFolder, newFileName);
                File.WriteAllBytes(pathToSave, fileContents);
            }
        }


        private DataTable GetAccessAttachmentsAsTable(string attachmentColumnName, string tableName, string additionalSelectCommand = null) {
            string sqlCommand
                = $@"select {attachmentColumnName}.FileData as filedata, {attachmentColumnName}.FileName as filename, {attachmentColumnName}.FileType as filetype {additionalSelectCommand} FROM {tableName}";

            var ds = GetAccessTableAsDataSet(sqlCommand, tableName);
            return ds.Tables[0];
        }

        private static byte[] GetAttachmentContents(DataRow row) {
            byte[] filedata = (byte[])row["filedata"];
            int header = (int)filedata[0];
            byte[] actualFile = new byte[filedata.Length - header];
            Buffer.BlockCopy(filedata, header, actualFile, 0, actualFile.Length);
            // do stuff with byte array!
            return actualFile;
        }

        public void RetrieveScreenshots() {
            string sqlCommand = @"select Screenshot.FileData as filedata, Screenshot.FileName as filename, Screenshot.FileType as filetype, ID FROM tblFilenames";

            var ds = GetAccessTableAsDataSet(sqlCommand, "tblFilenames");
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows) {
                var filename = row["filename"].ToString();
                if (string.IsNullOrWhiteSpace(filename)) continue;
                var Id = row["ID"].ToString();
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
