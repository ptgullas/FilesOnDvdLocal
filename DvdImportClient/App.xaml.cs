using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DvdImportClient {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        void OnApplicationStartup(object sender, StartupEventArgs se) {
            SetUpLogging();
        }

        private static void SetUpLogging() {
            try {
                //string logFolder = Configuration.GetSection("LocalFolders").GetValue<string>("logFolder");
                //string logFile = Configuration.GetSection("LocalFolders").GetValue<string>("logFileName");
                //string logPath = Path.Combine(logFolder, logFile);
                string logPath = @"c:\Logs\FilesOnDvdLocal\FilesOnDvdLocalLog.txt";
                string logDirectory = Path.GetDirectoryName(logPath);
                if (!Directory.Exists(logDirectory)) {
                    Directory.CreateDirectory(logDirectory);
                }

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
            catch (Exception e) {
                Console.WriteLine($"Could not set up logging");
                Console.WriteLine(e.GetBaseException());
            }
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show(e.Exception.ToString());
        }
    }
}
