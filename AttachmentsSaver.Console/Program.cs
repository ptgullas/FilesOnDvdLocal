using FilesOnDvdLocal.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttachmentsSaver
{
    class Program
    {
        static void Main(string[] args) {
            SetUpLogging();
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            // RetrieveScreenshots(dbLocation);
            RetrieveHeadshots(dbLocation);
        }

        private static void RetrieveScreenshots(string dbLocation) {
            AccessAttachmentsRetriever retriever = new AccessAttachmentsRetriever(dbLocation);
            retriever.GetScreenshots();
        }

        private static void RetrieveHeadshots(string dbLocation) {
            AccessAttachmentsRetriever retriever = new AccessAttachmentsRetriever(dbLocation);
            retriever.GetHeadshots();
        }


        private static void SetUpLogging() {
            try {
                //string logFolder = Configuration.GetSection("LocalFolders").GetValue<string>("logFolder");
                //string logFile = Configuration.GetSection("LocalFolders").GetValue<string>("logFileName");
                //string logPath = Path.Combine(logFolder, logFile);
                string logPath = @"c:\Logs\FilesOnDvdLocal\FilesOnDvdLocalLog.txt";

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
            catch (Exception e) {
                Console.WriteLine($"Could not set up logging");
                Console.WriteLine(e.GetBaseException());
            }
        }

    }
}
