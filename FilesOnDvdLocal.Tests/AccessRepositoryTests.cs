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
    }
}
