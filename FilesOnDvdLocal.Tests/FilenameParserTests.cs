using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests {
    [TestClass]
    public class FilenameParserTests {
        [TestMethod]
        public void GetSeriesName_FilenameHasSeries_ReturnsSeriesName() {
            string filename = @"Watchmen - Jean Smart, Jeremy Irons, Regina King - Little Fear of Lightning (2019-12-05).mkv";
            string expected = "Watchmen";

            string result = FilenameParser.GetSeriesName(filename);
            Assert.AreEqual(expected, result);

        }
    }
}
