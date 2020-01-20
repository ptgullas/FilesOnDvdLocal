using System;
using System.IO;
using FilesOnDvdLocal.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests {
    [TestClass]
    public class FileToImportTests {
        [TestMethod]
        public void NameIsTooLong_NameFitsOnDvd_ReturnFalse() {
            string filePath = @"C:\temp\Watchmen - 1.05 - Little Fear of Lightning.mkv";
            bool expected = false;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(filePath, mockPerformerRepository);

            bool result = fileToImport.NameIsTooLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameIsTooLong_NameTooLongForDvd_ReturnTrue() {
            string filePath = @"C:\temp\saturday.night.live.s45e05.kristen.stewart.internal.480p.web.mp4.rmteamthisshouldbeover98characters.mkv";
            bool expected = true;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(filePath, mockPerformerRepository);

            bool result = fileToImport.NameIsTooLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameIsTooLong_NameIs98CharsLong_ReturnFalse() {
            string fileWith98Chars = @"c:\temp\1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234.678";
            bool expected = false;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(fileWith98Chars, mockPerformerRepository);

            bool result = fileToImport.NameIsTooLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameIsTooLong_NameIs99CharsLong_ReturnTrue() {
            string fileWith99Chars = @"c:\temp\12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345.789";
            bool expected = true;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(fileWith99Chars, mockPerformerRepository);

            bool result = fileToImport.NameIsTooLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameContainsNonAscii_NameHasOnlyAscii_ReturnFalse() {
            string filePath = @"C:\temp\saturday.night.live.s45e05.kristen.stewart.internal.480p.web.mp4.rmteamthisshouldbeover98characters.mkv";
            bool expected = false;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(filePath, mockPerformerRepository);

            bool result = fileToImport.NameContainsNonAscii;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameContainsNonAscii_NameHasEmDash_ReturnTrue() {
            string filePath = @"C:\temp\ThisHasAnEmDash—see.mkv";
            bool expected = true;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(filePath, mockPerformerRepository);

            bool result = fileToImport.NameContainsNonAscii;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameContainsDoubleSpaces_NameHasThreeSpaces_ReturnTrue() {
            string filePath = @"C:\temp\Joker (2019) - Scene 4.   Joaquin Phoenix & Zazie Beetzmkv";
            bool expected = true;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(filePath, mockPerformerRepository);

            bool result = fileToImport.NameContainsDoubleSpaces;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameContainsDoubleSpaces_NameHasTwoSpaces_ReturnTrue() {
            string filePath = @"C:\temp\Joker (2019) - Scene 4.  Joaquin Phoenix & Zazie Beetzmkv";
            bool expected = true;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(filePath, mockPerformerRepository);

            bool result = fileToImport.NameContainsDoubleSpaces;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameContainsDoubleSpaces_NameDoesNotHaveDoubleSpaces_ReturnFalse() {
            string filePath = @"C:\temp\Joker (2019) - Scene 4. Joaquin Phoenix & Zazie Beetzmkv";
            bool expected = false;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(filePath, mockPerformerRepository);

            bool result = fileToImport.NameContainsDoubleSpaces;
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void GetPositionsOfNonAsciiInName_NameHasEmDashAtEnd_ReturnLastPosition() {
            string filePath = @"C:\temp\ThisHasAnEmDashsee—.mkv";
            int expectedPosition = 18;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(filePath, mockPerformerRepository);

            var result = fileToImport.GetPositionsOfNonAsciiInName();

            Assert.AreEqual(expectedPosition, result[0]);
        }

        [TestMethod]
        public void GetPositionsOfNonAsciiInName_NameHasOnlyAscii_ReturnEmptyList() {
            string filePath = @"C:\temp\ThisHasAnEmDashsee.mkv";
            int expected = 0;
            PerformerMockRepository mockPerformerRepository = new PerformerMockRepository();
            FileToImport fileToImport = new FileToImport(filePath, mockPerformerRepository);

            var result = fileToImport.GetPositionsOfNonAsciiInName();

            Assert.AreEqual(expected, result.Count);
        }
    }
}
