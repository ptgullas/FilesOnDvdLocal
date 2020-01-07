using System;
using System.Collections.Generic;
using System.Linq;
using FilesOnDvdLocal.LocalDbDtos;
using FilesOnDvdLocal.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests
{
    [TestClass]
    public class PerformerRepositoryTests
    {
        [TestMethod]
        public void Get_GetAllPerformers_ReturnsAllPerformers() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            PerformerRepository repository = new PerformerRepository(dbLocation);
            PerformerLocalDto expected = new PerformerLocalDto() {
                Id = 177,
                Name = "Steve Austin"
            };

            List<PerformerLocalDto> performersInDb = repository.Get();

            var result = performersInDb.FirstOrDefault(p => p.Name.ToLower() == "steve austin");
            Assert.AreEqual(expected.Id, result.Id);
        }

        [TestMethod]
        public void Get_ByName_PerformerExists_ReturnsPerformer() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            PerformerRepository repository = new PerformerRepository(dbLocation);
            PerformerLocalDto expected = new PerformerLocalDto() {
                Id = 177,
                Name = "Steve Austin"
            };

            string performerNameToFind = "steve austin";

            var result = repository.Get(performerNameToFind);

            Assert.AreEqual(expected.Id, result.Id);

        }
    }
}
