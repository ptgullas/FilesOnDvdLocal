using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests {
    [TestClass]
    public class StringExtensionsTests {
        [TestMethod]
        public void GetIndexOfFirstHyphen_HasHyphenReturnFirstHyphen_ReturnsIndexOfFirstHyphen() {
            string fileName = "Archer - 10.01 - 1999-Bort the Garj.mkv";
            int expected = 7;
            int result = fileName.GetIndexOfFirstHyphen();
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void GetIndexOfFirstHyphen_HasHyphenSkipFirstHyphen_ReturnsIndexOfSecondHyphen() {
            string fileName = "Archer - 10.01 - 1999-Bort the Garj.mkv";
            int expected = 7;
            int result = fileName.GetIndexOfFirstHyphen(1);
            Assert.AreEqual(expected, result);
        }
    }
}
