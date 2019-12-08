using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests {
    [TestClass]
    public class DvdFolderToImportTests {
        [TestMethod]
        public void HasNamingErrors_ContainsTooLongFileName_ReturnsTrue() {
            string filePath1 = @"C:\temp\Arrow - Oliver Queen, Laurel Lance & Harbinger - Little Fear of Lightning Little Fear of Lightning Little Fear of Lightning (2019-12-05).mkv";
            FileToImport file1 = new FileToImport(filePath1);
            string filePath2 = @"C:\temp\short filename1.mkv";
            FileToImport file2 = new FileToImport(filePath2);
            string filePath3 = @"C:\temp\short filename2.mkv";
            FileToImport file3 = new FileToImport(filePath3);

            List<FileToImport> files = new List<FileToImport>();
            files.Add(file1);
            files.Add(file2);
            files.Add(file3);

            DvdFolderToImport dvd = new DvdFolderToImport("C:\temp", files);

            bool expectedResult = true;

            bool result = dvd.HasNamingErrors();

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void HasNamingErrors_ContainsGoodFileNames_ReturnsFalse() {
            string filePath2 = @"C:\temp\short filename1.mkv";
            FileToImport file2 = new FileToImport(filePath2);
            string filePath3 = @"C:\temp\short filename2.mkv";
            FileToImport file3 = new FileToImport(filePath3);

            List<FileToImport> files = new List<FileToImport>();
            files.Add(file2);
            files.Add(file3);

            DvdFolderToImport dvd = new DvdFolderToImport("C:\temp", files);

            bool expectedResult = false;

            bool result = dvd.HasNamingErrors();

            Assert.AreEqual(expectedResult, result);
        }
        [TestMethod]
        public void HasNamingErrors_ContainsNonAsciiFileNames_ReturnsTrue() {
            string filePath1 = @"C:\temp\thishas an em—dash.mkv";
            FileToImport file1 = new FileToImport(filePath1);
            string filePath2 = @"C:\temp\short filename1.mkv";
            FileToImport file2 = new FileToImport(filePath2);
            string filePath3 = @"C:\temp\short filename2.mkv";
            FileToImport file3 = new FileToImport(filePath3);

            List<FileToImport> files = new List<FileToImport>();
            files.Add(file1);
            files.Add(file2);
            files.Add(file3);

            DvdFolderToImport dvd = new DvdFolderToImport("C:\temp", files);

            bool expectedResult = true;

            bool result = dvd.HasNamingErrors();

            Assert.AreEqual(expectedResult, result);
        }
    }
}
