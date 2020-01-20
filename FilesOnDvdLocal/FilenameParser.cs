using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public static class FilenameParser {
        public static string GetSeriesName(string filename) {
            string seriesName = null;
            int indexOfHyphen = filename.IndexOf("-");
            if (indexOfHyphen >= 2) {
                seriesName = filename.Substring(0, indexOfHyphen - 1).Trim();
            }
            return seriesName;
        }

        public static List<string> GetPerformers(string filename) {
            List<string> performers = new List<string>();
            string performersSubstring = GetPerformersSubstring(filename);
            if (performersSubstring != null) {
                performers = ExtractPerformersFromSubstring(performersSubstring);
            }
            return performers;
        }
        public static string GetPerformersSubstring(string filename) {
            string performersSegment;
            if (ContainsYearAndSceneNumber(filename)) {
                performersSegment = GetPerformersSubstringWhenFilenameContainsYearAndScene(filename);
            }
            else {
                performersSegment = GetPerformersSubstringBetweenHyphenAndDelimeter(filename);
            }
            return performersSegment;
        }
        public static bool ContainsYearAndSceneNumber(string filename) {
            Regex rgx = GetYearAndSceneRegEx();
            if (rgx.IsMatch(filename)) { return true; }
            else { return false; }
        }
        private static Regex GetYearAndSceneRegEx() {
            // this regex means:
            // \(\d+\): any series of digits inside parentheses
            // Scene[ s]: Either Scene (followed by space) or Scenes
            // \s*\d+: matches zero or more spaces, followed by some number of digits
            string yearAndScenePattern = @" \(\d+\) - Scene[ s]\s*\d+";
            // this part is for when the filename contains multiple scenes
            // \s?: zero or one space
            // &?: zero or one ampersand
            // \s?\d*: zero or one space, followed by zero or more digits
            yearAndScenePattern += @"\s*&?\s*\d*";
            // \.: dot
            // (.+): any character, and capture that grouping
            yearAndScenePattern += @"\.(.+)";
            Regex rgx = new Regex(yearAndScenePattern);
            return rgx;
        }

        private static List<string> ExtractPerformersFromSubstring(string performersSubstring) {
            List<string> performers = new List<string>();
            List<string> performersSubstringSplitByComma = performersSubstring.Split(',').ToList();
            if (performersSubstring.Contains("&")) {
                SplitByAmpersandsAndAddToPerformers(performers, performersSubstringSplitByComma);
            }
            else {
                foreach (string performer in performersSubstringSplitByComma) {
                    performers.Add(performer.Trim());
                }
            }
            return performers;
        }

        private static void SplitByAmpersandsAndAddToPerformers(List<string> performers, List<string> performersSubstringSplitByComma) {
            foreach (string performerMightContainAmpersand in performersSubstringSplitByComma) {
                if (!performerMightContainAmpersand.Contains("&")) {
                    performers.Add(performerMightContainAmpersand.Trim());
                }
                else {
                    List<string> tempPerformersSplitAmpersand = performerMightContainAmpersand.Split('&').ToList();
                    foreach (string performer in tempPerformersSplitAmpersand) {
                        performers.Add(performer.Trim());
                    }
                }
            }
        }

        // filenames with Year & Scene should end the scene number with a period, not " - "
        private static string GetPerformersSubstringWhenFilenameContainsYearAndScene(string filename) {
            filename = Path.GetFileNameWithoutExtension(filename);
            Regex rgx = GetYearAndSceneRegEx();
            Match match = rgx.Match(filename);
            return match.Groups[1].ToString().Trim();
        }

        private static string GetPerformersSubstringBetweenHyphenAndDelimeter(string filename) {
            string performersSubstring = null;
            int indexOfHyphen = filename.IndexOf("-");
            if (indexOfHyphen >= 0) {
                int indexOfSecondDelimeter = GetIndexOfNextDelimeter(filename, indexOfHyphen + 1);
                if (indexOfSecondDelimeter >= 0) {
                    performersSubstring = filename.Substring(indexOfHyphen + 1, indexOfSecondDelimeter - indexOfHyphen - 1).Trim();
                }
            }
            return performersSubstring;
        }

        private static int GetIndexOfNextDelimeter(string filename, int start) {
            int indexOfHyphen = filename.IndexOf("-", start);
            int indexOfOpenParen = filename.IndexOf("(", start);

            if (indexOfHyphen < 0) {
                return indexOfOpenParen;
            }
            else if ((indexOfHyphen >= 0) && (indexOfOpenParen >= 0)) {
                return Math.Min(indexOfHyphen, indexOfOpenParen);
            }
            else {
                return indexOfHyphen;
            }
        }


    }
}
