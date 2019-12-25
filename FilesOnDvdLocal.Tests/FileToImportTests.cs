using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests {
    [TestClass]
    public class FileToImportTests {
        [TestMethod]
        public void NameIsTooLong_NameFitsOnDvd_ReturnFalse() {
            string filePath = @"C:\temp\Watchmen - 1.05 - Little Fear of Lightning.mkv";
            bool expected = false;
            FileToImport fileToImport = new FileToImport(filePath);

            bool result = fileToImport.NameIsTooLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameIsTooLong_NameTooLongForDvd_ReturnTrue() {
            string filePath = @"C:\temp\saturday.night.live.s45e05.kristen.stewart.internal.480p.web.mp4.rmteamthisshouldbeover98characters.mkv";
            bool expected = true;
            FileToImport fileToImport = new FileToImport(filePath);

            bool result = fileToImport.NameIsTooLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameContainsNonAscii_NameHasOnlyAscii_ReturnFalse() {
            string filePath = @"C:\temp\saturday.night.live.s45e05.kristen.stewart.internal.480p.web.mp4.rmteamthisshouldbeover98characters.mkv";
            bool expected = false;
            FileToImport fileToImport = new FileToImport(filePath);

            bool result = fileToImport.NameContainsNonAscii;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameContainsNonAscii_NameHasEmDash_ReturnTrue() {
            string filePath = @"C:\temp\ThisHasAnEmDash—see.mkv";
            bool expected = true;
            FileToImport fileToImport = new FileToImport(filePath);

            bool result = fileToImport.NameContainsNonAscii;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPositionsOfNonAsciiInName_NameHasEmDashAtEnd_ReturnLastPosition() {
            string filePath = @"C:\temp\ThisHasAnEmDashsee—.mkv";
            int expectedPosition = 18;
            FileToImport fileToImport = new FileToImport(filePath);

            var result = fileToImport.GetPositionsOfNonAsciiInName();

            Assert.AreEqual(expectedPosition, result[0]);
        }

        [TestMethod]
        public void GetPositionsOfNonAsciiInName_NameHasOnlyAscii_ReturnEmptyList() {
            string filePath = @"C:\temp\ThisHasAnEmDashsee.mkv";
            int expected = 0;
            FileToImport fileToImport = new FileToImport(filePath);

            var result = fileToImport.GetPositionsOfNonAsciiInName();

            Assert.AreEqual(expected, result.Count);
        }

        [TestMethod]
        public void GetSeriesNameFromFilename_NameIsNormal_ReturnsName() {
            string filePath = @"C:\temp\Watchmen - Jean Smart, Jeremy Irons, Regina King - Little Fear of Lightning (2019-12-05).mkv";
            string expected = "Watchmen";
            FileToImport fileToImport = new FileToImport(filePath);

            string result = fileToImport.GetSeriesNameFromFilename();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPerformersFromFilename_NameHasCommas_ReturnsPerformers() {
            string filePath = @"C:\temp\Watchmen - Jean Smart, Jeremy Irons, Regina King - Little Fear of Lightning (2019-12-05).mkv";
            string expected = "Jeremy Irons";
            FileToImport fileToImport = new FileToImport(filePath);

            var result = fileToImport.GetPerformersFromFilename();
            Assert.AreEqual(expected, result[1]);
        }

        [TestMethod]
        public void GetPerformersFromFilename_NameHasAmpersand_ReturnsPerformers() {
            string filePath = @"C:\temp\Watchmen - Trent Reznor & Atticus Ross - Little Fear of Lightning (2019-12-05).mkv";
            string expected = "Atticus Ross";
            FileToImport fileToImport = new FileToImport(filePath);

            var result = fileToImport.GetPerformersFromFilename();
            Assert.AreEqual(expected, result[1]);
        }

        [TestMethod]
        public void GetPerformersFromFilename_NameHasAmpersandThenComma_ReturnsPerformers() {
            string filePath = @"C:\temp\Succession - Logan Roy & Shiv Roy, Roman Roy - Little Fear of Lightning (2019-12-05).mkv";
            string expected1 = "Logan Roy";
            string expected2 = "Shiv Roy";
            string expected3 = "Roman Roy";
            FileToImport fileToImport = new FileToImport(filePath);

            var result = fileToImport.GetPerformersFromFilename();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(expected1, result[0]);
            Assert.AreEqual(expected2, result[1]);
            Assert.AreEqual(expected3, result[2]);
        }

        [TestMethod]
        public void GetPerformersFromFilename_NameHasCommasThenAmpersand_ReturnsPerformers() {
            string filePath = @"C:\temp\Arrow - Oliver Queen, Laurel Lance & Harbinger - Little Fear of Lightning (2019-12-05).mkv";
            string expected1 = "Oliver Queen";
            string expected2 = "Laurel Lance";
            string expected3 = "Harbinger";
            FileToImport fileToImport = new FileToImport(filePath);

            var result = fileToImport.GetPerformersFromFilename();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(expected1, result[0]);
            Assert.AreEqual(expected2, result[1]);
            Assert.AreEqual(expected3, result[2]);
        }

    }
}
