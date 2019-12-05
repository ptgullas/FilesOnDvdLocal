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

            bool result = fileToImport.NameIsTooLong();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameIsTooLong_NameTooLongForDvd_ReturnTrue() {
            string filePath = @"C:\temp\saturday.night.live.s45e05.kristen.stewart.internal.480p.web.mp4.rmteamthisshouldbeover98characters.mkv";
            bool expected = true;
            FileToImport fileToImport = new FileToImport(filePath);

            bool result = fileToImport.NameIsTooLong();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameContainsNonAscii_NameHasOnlyAscii_ReturnFalse() {
            string filePath = @"C:\temp\saturday.night.live.s45e05.kristen.stewart.internal.480p.web.mp4.rmteamthisshouldbeover98characters.mkv";
            bool expected = false;
            FileToImport fileToImport = new FileToImport(filePath);

            bool result = fileToImport.NameContainsNonAscii();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NameContainsNonAscii_NameHasEmDash_ReturnTrue() {
            string filePath = @"C:\temp\ThisHasAnEmDash—see.mkv";
            bool expected = true;
            FileToImport fileToImport = new FileToImport(filePath);

            bool result = fileToImport.NameContainsNonAscii();
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

    }
}
