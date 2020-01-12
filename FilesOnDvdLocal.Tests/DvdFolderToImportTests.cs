using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FilesOnDvdLocal.Repositories;

namespace FilesOnDvdLocal.Tests {
    [TestClass]
    public class DvdFolderToImportTests {
        [TestMethod]
        public void HasNamingErrors_ContainsTooLongFileName_ReturnsTrue() {
            PerformerMockRepository mockRepository = new PerformerMockRepository();

            string filePath1 = @"C:\temp\Arrow - Oliver Queen, Laurel Lance & Harbinger - Little Fear of Lightning Little Fear of Lightning Little Fear of Lightning (2019-12-05).mkv";
            FileToImport file1 = new FileToImport(filePath1, mockRepository);
            string filePath2 = @"C:\temp\short filename1.mkv";
            FileToImport file2 = new FileToImport(filePath2, mockRepository);
            string filePath3 = @"C:\temp\short filename2.mkv";
            FileToImport file3 = new FileToImport(filePath3, mockRepository);

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
            PerformerMockRepository mockRepository = new PerformerMockRepository();
            string filePath2 = @"C:\temp\short filename1.mkv";
            FileToImport file2 = new FileToImport(filePath2, mockRepository);
            string filePath3 = @"C:\temp\short filename2.mkv";
            FileToImport file3 = new FileToImport(filePath3, mockRepository);

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
            PerformerMockRepository mockRepository = new PerformerMockRepository();
            string filePath1 = @"C:\temp\thishas an em—dash.mkv";
            FileToImport file1 = new FileToImport(filePath1, mockRepository);
            string filePath2 = @"C:\temp\short filename1.mkv";
            FileToImport file2 = new FileToImport(filePath2, mockRepository);
            string filePath3 = @"C:\temp\short filename2.mkv";
            FileToImport file3 = new FileToImport(filePath3, mockRepository);

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
        public void CompileAllPerformersInFolder_AllUniquePerformers_ReturnsCorrectNumber() {
            PerformerMockRepository mockRepository = new PerformerMockRepository();
            string filePath1 = @"C:\temp\Succession - Logan Roy & Shiv Roy, Roman Roy - Little Fear of Lightning (2019-12-05).mkv";
            FileToImport file1 = new FileToImport(filePath1, mockRepository);
            string filePath2 = @"C:\temp\Watchmen - Looking Glass & Dr. Manhattan - A God Walks Into Abar (2019-09-27).mp4";
            FileToImport file2 = new FileToImport(filePath2, mockRepository);
            string filePath3 = @"C:\temp\Arrow - Oliver Queen, Laurel Lance & Harbinger - Little Fear of Lightning (2019-12-05).mkv";
            FileToImport file3 = new FileToImport(filePath3, mockRepository);

            List<FileToImport> files = new List<FileToImport>();
            files.Add(file1);
            files.Add(file2);
            files.Add(file3);

            DvdFolderToImport dvd = new DvdFolderToImport("C:\temp", files);

            int expectedResult = 8;

            var performerList = dvd.PerformersInFolderAll.OrderBy(p => p.Name);
            int result = performerList.Count();

            Assert.AreEqual(expectedResult, result);

        }

    }
}
