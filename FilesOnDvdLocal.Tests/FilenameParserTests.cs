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

        [TestMethod]
        public void ContainsYearAndSceneNumber_FilenameHasYearAndScene_ReturnsTrue() {
            string filename = @"Joker (2019) - Scene 3. Joaquin Phoenix & Zazie Beetz.mkv";
            bool result = FilenameParser.ContainsYearAndSceneNumber(filename);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsYearAndSceneNumber_FilenameHasYearAndMultipleScenes_ReturnsTrue() {
            string filename = @"Joker (2019) - Scenes 13 & 14. Mark Maron & Robert De Niro.mkv";
            bool result = FilenameParser.ContainsYearAndSceneNumber(filename);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsYearAndSceneNumber_FilenameDoesNotHaveYearAndScene_ReturnsFalse() {
            string filename = @"Arrow - Oliver Queen, Laurel Lance & Harbinger - Little Fear of Lightning (2019-12-05).mkv";
            bool result = FilenameParser.ContainsYearAndSceneNumber(filename);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetPerformersSubstring_FilenameHasYearAndMultipleScene_ReturnsPerformersSubstring() {
            string filename = @"Joker (2019) - Scenes 13 & 14. Mark Maron & Robert De Niro.mkv";
            string expected = "Mark Maron & Robert De Niro";

            string result = FilenameParser.GetPerformersSubstring(filename);
            
            Assert.AreEqual(expected,result);
        }

        [TestMethod]
        public void GetPerformersSubstring_FilenameIsNormal_ReturnsPerformersSubstring() {
            string filename = @"Arrow - Oliver Queen, Laurel Lance & Harbinger - Little Fear of Lightning (2019-12-05).mkv";
            string expected = "Oliver Queen, Laurel Lance & Harbinger";

            string result = FilenameParser.GetPerformersSubstring(filename);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPerformers_NameHasCommas_ReturnsPerformers() {
            string filename = @"Watchmen - Jean Smart, Jeremy Irons, Regina King - Little Fear of Lightning (2019-12-05).mkv";
            int expectedNumber = 3;
            string expectedName1 = "Jean Smart";
            string expectedName2 = "Jeremy Irons";
            string expectedName3 = "Regina King";

            var result = FilenameParser.GetPerformers(filename);
            int resultNumber = result.Count;

            Assert.AreEqual(expectedNumber, resultNumber);
            Assert.AreEqual(expectedName1, result[0]);
            Assert.AreEqual(expectedName2, result[1]);
            Assert.AreEqual(expectedName3, result[2]);
        }

        [TestMethod]
        public void GetPerformers_NameHasAmpersand_ReturnsPerformers() {
            string filename = @"Watchmen - Trent Reznor & Atticus Ross - Little Fear of Lightning (2019-12-05).mkv";
            int expectedNumber = 2;
            string expectedName1 = "Trent Reznor";
            string expectedName2 = "Atticus Ross";

            var result = FilenameParser.GetPerformers(filename);
            int resultNumber = result.Count;

            Assert.AreEqual(expectedNumber, resultNumber);
            Assert.AreEqual(expectedName1, result[0]);
            Assert.AreEqual(expectedName2, result[1]);
        }

        [TestMethod]
        public void GetPerformers_NameHasAmpersandThenComma_ReturnsPerformers() {
            string filename = @"Succession - Logan Roy & Shiv Roy, Roman Roy - Little Fear of Lightning (2019-12-05).mkv";
            int expectedNumber = 3;
            string expectedName1 = "Logan Roy";
            string expectedName2 = "Shiv Roy";
            string expectedName3 = "Roman Roy";

            var result = FilenameParser.GetPerformers(filename);
            int resultNumber = result.Count;

            Assert.AreEqual(expectedNumber, resultNumber);
            Assert.AreEqual(expectedName1, result[0]);
            Assert.AreEqual(expectedName2, result[1]);
            Assert.AreEqual(expectedName3, result[2]);
        }

        [TestMethod]
        public void GetPerformers_NameHasCommasThenAmpersand_ReturnsPerformers() {
            string filename = @"Arrow - Oliver Queen, Laurel Lance & Harbinger - Little Fear of Lightning (2019-12-05).mkv";
            int expectedNumber = 3;
            string expectedName1 = "Oliver Queen";
            string expectedName2 = "Laurel Lance";
            string expectedName3 = "Harbinger";

            var result = FilenameParser.GetPerformers(filename);
            int resultNumber = result.Count;

            Assert.AreEqual(expectedNumber, resultNumber);
            Assert.AreEqual(expectedName1, result[0]);
            Assert.AreEqual(expectedName2, result[1]);
            Assert.AreEqual(expectedName3, result[2]);
        }
        [TestMethod]
        public void GetPerformers_NameHasCommasThenAmpersandEndsWithParentheses_ReturnsPerformers() {
            string filename = @"Arrow - Oliver Queen, Laurel Lance & Harbinger (2019-12-05).mkv";
            int expectedNumber = 3;
            string expectedName1 = "Oliver Queen";
            string expectedName2 = "Laurel Lance";
            string expectedName3 = "Harbinger";

            var result = FilenameParser.GetPerformers(filename);
            int resultNumber = result.Count;

            Assert.AreEqual(expectedNumber, resultNumber);
            Assert.AreEqual(expectedName1, result[0]);
            Assert.AreEqual(expectedName2, result[1]);
            Assert.AreEqual(expectedName3, result[2]);
        }

        [TestMethod]
        public void GetPerformers_NameHasYearAndSceneAndAmpersands_ReturnsPerformers() {
            string filename = @"The Farewell (2019) - Scene 1. Awkwafina & Nai Nai.mkv";
            int expectedNumber = 2;
            string expectedName1 = "Awkwafina";
            string expectedName2 = "Nai Nai";

            var result = FilenameParser.GetPerformers(filename);
            int resultNumber = result.Count;

            Assert.AreEqual(expectedNumber, resultNumber);
            Assert.AreEqual(expectedName1, result[0]);
            Assert.AreEqual(expectedName2, result[1]);
        }

        [TestMethod]
        public void GetPerformers_NameHasYearAndSceneAndCommas_ReturnsPerformers() {
            string filename = @"Hustlers (2019) - Scene 1. Jennifer Lopez, Constance Wu.mkv";
            int expectedNumber = 2;
            string expectedName1 = "Jennifer Lopez";
            string expectedName2 = "Constance Wu";

            var result = FilenameParser.GetPerformers(filename);
            int resultNumber = result.Count;

            Assert.AreEqual(expectedNumber, resultNumber);
            Assert.AreEqual(expectedName1, result[0]);
            Assert.AreEqual(expectedName2, result[1]);
        }

        [TestMethod]
        public void GetPerformers_NameHasNoHyphens_ReturnsEmptyList() {
            string filename = @"hi I am a file.mkv";
            int expectedNumber = 0;

            var result = FilenameParser.GetPerformers(filename);
            int resultNumber = result.Count;

            Assert.AreEqual(expectedNumber, resultNumber);
        }

        [TestMethod]
        public void GetPerformers_NameHasSeriesButNoTitleOrDate_ReturnsPerformers() {
            string filename = @"Doctor Who - The Doctor, Amy Pond, Sarah Jane Smith.mp4";
            string expectedName1 = "The Doctor";
            string expectedName2 = "Amy Pond";
            string expectedName3 = "Sarah Jane Smith";
            int expectedNumber = 3;

            var result = FilenameParser.GetPerformers(filename);
            int resultNumber = result.Count;

            Assert.AreEqual(expectedNumber, resultNumber);
            Assert.AreEqual(expectedName1, result[0]);
            Assert.AreEqual(expectedName2, result[1]);
            Assert.AreEqual(expectedName3, result[2]);

        }
    }
}
