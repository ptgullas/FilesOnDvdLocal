using FilesOnDvdLocal.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
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
            // GetScreenshots(dbLocation);
            GetHeadshots(dbLocation);
        }

        private static void GetScreenshots(string dbLocation) {
            AccessAttachmentsRetriever retriever = new AccessAttachmentsRetriever(dbLocation);
            retriever.RetrieveScreenshots();
        }

        private static void GetHeadshots(string dbLocation) {
            AccessAttachmentsRetriever retriever = new AccessAttachmentsRetriever(dbLocation);
            string headshotsDirectory = @"C:\temp\headshots";
            CreateFolderIfNeeded(headshotsDirectory);
            retriever.RetrieveHeadshots(headshotsDirectory);
        }

        private static void CreateFolderIfNeeded(string saveFolder) {
            if (!Directory.Exists(saveFolder)) {
                Directory.CreateDirectory(saveFolder);
            }
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
