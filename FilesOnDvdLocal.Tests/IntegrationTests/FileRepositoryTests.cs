using System;
using System.Collections.Generic;
using FilesOnDvdLocal.LocalDbDtos;
using FilesOnDvdLocal.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests.IntegrationTests
{
    [TestClass]
    public class FileRepositoryTests
    {
        [TestMethod]
        public void Add_AddFileDtosList_AddsToDb() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            FileRepository repository = new FileRepository(dbLocation);
            List<FileLocalDto> fileDtos = new List<FileLocalDto>() {
                new FileLocalDto() {Disc = 225, Filename = "2020-01-27 WWE RAW.mkv", Genre = 4, Series = 88 },
                new FileLocalDto() {Disc = 225, Filename = "2020-01-22 WWE NXT.mkv", Genre = 4, Series = 70 },
                new FileLocalDto() {Disc = 225, Filename = "2020-01-26 WWE Royal Rumble 2020.mp4", Genre = 4},
                new FileLocalDto() {Disc = 225, Filename = "2020-01-22 AEW Dynamite.avi", Genre = 4},
            };
            repository.Add(fileDtos);

            Console.WriteLine("Debug this line");
        
        }
    }
}
