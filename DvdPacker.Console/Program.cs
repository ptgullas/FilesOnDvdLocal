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
            DisplayIntro();
            SetUpLogging();
            if (args.Length == 0) {
                DisplayHelpText();
            }
            else {
                string folderPath = args[0];
                string prefix = "P";
                if (args.Length == 2) {
                    prefix = args[1];
                }
                BinPacker binPacker = new BinPacker(folderPath, prefix);
                binPacker.ProcessFolder();
                string binListing = binPacker.GetBinListing();
                Console.WriteLine(binListing);
                binPacker.MoveFilesIntoBins();
            }
        }

        static void DisplayIntro() {
            ColorHelpers.WriteColor("Thanks for running ", ConsoleColor.White);
            ColorHelpers.WriteColor("DVD", ConsoleColor.Yellow);
            ColorHelpers.WriteColor("PACKER", ConsoleColor.Magenta);
            ColorHelpers.WriteLineColor("!", ConsoleColor.White);
            ColorHelpers.WriteColor("Copyright \u00a9 2020, ");
            ColorHelpers.WriteLineColor("Paul T. Gullas", ConsoleColor.Cyan);
            ColorHelpers.WriteLineColor("https://github.com/ptgullas/FilesOnDvdLocal/tree/master/DvdPacker.Console", ConsoleColor.Magenta);
        }

        static void DisplayHelpText() {
            string applicationName = "DvdPacker";
            Console.WriteLine("Usage:");
            ColorHelpers.WriteLineColor($"{applicationName}: ");
            Console.WriteLine("\tDisplay this help text");
            ColorHelpers.WriteColor($"{applicationName} ");
            ColorHelpers.WriteColor($"<path of folder containing files to pack> ", ConsoleColor.Yellow);
            ColorHelpers.WriteLineColor($"<folder prefix (default is P)>:", ConsoleColor.Green);
            Console.WriteLine("\tOrganize files into separate folders that will optimally fit on a DVD");

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
