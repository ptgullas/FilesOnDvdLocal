using System;
using System.Collections.Generic;
using System.Linq;
using FilesOnDvdLocal.LocalDbDtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests
{
    [TestClass]
    public class AccessRepositoryTests
    {
        [TestMethod]
        public void GetPerformers_GetActualPerformers_ReturnsPerformers() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            AccessRepository repository = new AccessRepository(dbLocation);
            PerformerLocalDto expected = new PerformerLocalDto() {
                Id = 177,
                Name = "Steve Austin"
            };

            List<PerformerLocalDto> performersInDb = repository.GetPerformers();

            var result = performersInDb.FirstOrDefault(p => p.Name.ToLower() == "steve austin");
            Assert.AreEqual(expected.Id, result.Id);
        }

        // You can debug this test instead of running it, so that the objects in memory
        // can be examined. Also, you can look at the actual Access DB to verify that
        // the disc was added
        [TestMethod]
        public void AddDisc_AddActualDisc_ActuallyUpdateDisc() {
            AccessMockRepository mockRepository = new AccessMockRepository();
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

            DvdFolderToImport dvd = new DvdFolderToImport(@"C:\temp", files) {
                DiscName = "TV2020-01-05",
                WalletType = 5,
                Notes = "Misc TV shows"
            };

            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            AccessRepository realRepository = new AccessRepository(dbLocation);
            realRepository.AddDisc(dvd);
            Console.WriteLine("Debug this line");
        }


    }
}
