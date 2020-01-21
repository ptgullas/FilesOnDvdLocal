using System;
using FilesOnDvdLocal.LocalDbDtos;
using FilesOnDvdLocal.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests.IntegrationTests
{
    [TestClass]
    public class DiscRepositoryTests
    {
        [TestMethod]
        public void Add_NewDisc_AddsToDiscTable() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            DiscRepository repository = new DiscRepository(dbLocation);
            DiscLocalDto discDto = new DiscLocalDto() { DiscName = "TestingDisc", Wallet = 3, Notes = "A Test from .NET" };

            repository.Add(discDto);

            Console.WriteLine("Debug this line");

        }
    }
}
