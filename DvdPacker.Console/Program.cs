using FilesOnDvdLocal;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvdPacker {
    class Program
    {
        static void Main(string[] args) {
            string folderPath = args[0];
            BinPacker binPacker = new BinPacker(folderPath, "P");
            binPacker.ProcessFolder();
            string binListing = binPacker.GetBinListing();
            Console.WriteLine(binListing);

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
