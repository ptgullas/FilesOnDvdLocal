using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests {
    [TestClass]
    public class FileToImportTests {
        [TestMethod]
        public void NameIsTooLong_NameFitsOnDvd_ReturnFalse() {
            string filePath = @"C:\temp\Watchmen - 1.05 - Little Fear of Lightning.mkv";
            FileToImport fileToImport = new FileToImport(filePath);
        }
    }
}
