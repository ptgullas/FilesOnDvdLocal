using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public class FileToImport {
        public FileInfo File { get; set; }
        public SeriesLocalDto Series {get; set;}
        public List<PerformerLocalDto> Performers { get; set; }

        public string Notes { get; set; }
        public bool Unwatched { get; set; }

        public string Filename { get; set; }

        public List<string> PerformersString { get; set; }
        public string SeriesString { get; set; }

        public FileToImport(string path) {
            File = new FileInfo(path);
            Filename = File.Name;
            PerformersString = new List<string>();
            PerformersString = GetPerformersFromFilename();
            SeriesString = GetSeriesNameFromFilename();
        }

        public bool NameIsTooLong {
            get { return NameIsTooLongForDvd(); }
        }

        public bool NameContainsNonAscii {
            get { return NameContainsNonAsciiCharacters(); }
        }

        private bool NameIsTooLongForDvd(int maxLength = 98) {
            int fileNameLength = File.Name.Length;
            if (fileNameLength > maxLength) {
                return true;
            }
            else {
                return false;
            }

        }


        private bool NameContainsNonAsciiCharacters() {
            bool NonAsciiInName = false;
            foreach (char c in File.Name) {
                if (c >= 128) {
                    NonAsciiInName = true;
                }
            }
            return NonAsciiInName;
        }

        public List<int> GetPositionsOfNonAsciiInName() {
            List<int> positions = new List<int>();
            for (int i = 0; i < File.Name.Length; i++) {
                char c = File.Name[i];
                if (c >= 128) {
                    positions.Add(i);
                }
            }
            return positions;
        }

        public string GetSeriesNameFromFilename() {
            string seriesName = null;
            string filename = File.Name;
            int indexOfHyphen = filename.IndexOf("-");
            if (indexOfHyphen >= 0) {
                seriesName = filename.Substring(0, indexOfHyphen - 1).Trim();
            }
            return seriesName;
        }

        public List<string> GetPerformersFromFilename() {
            List<string> performers = new List<string>();
            string filename = File.Name;
            int indexOfHyphen = filename.IndexOf("-");
            if (indexOfHyphen >= 0) {
                int indexOfSecondDelimeter = GetIndexOfNextDelimeter(filename, indexOfHyphen + 1);
                if (indexOfSecondDelimeter >= 0) {
                    string performersSubstring = filename.Substring(indexOfHyphen + 1, indexOfSecondDelimeter - indexOfHyphen - 1).Trim();
                    List<string> tempPerformers = performersSubstring.Split(',').ToList();
                    if (performersSubstring.Contains("&")) {
                        foreach (string tempPerformersSplit in tempPerformers) {
                            if (!tempPerformersSplit.Contains("&")) {
                                performers.Add(tempPerformersSplit.Trim());
                            }
                            else {
                                List<string> tempPerformersSplitAmpersand = tempPerformersSplit.Split('&').ToList();
                                foreach (string performer in tempPerformersSplitAmpersand) {
                                    performers.Add(performer.Trim());
                                }
                            }
                        }
                    }
                    else {
                        foreach (string performer in tempPerformers) {
                            performers.Add(performer.Trim());
                        }
                    }
                }
            }
            return performers;
        }



        private static int GetIndexOfNextDelimeter(string filename, int start) {
            int indexOfSecondDelimeter = filename.IndexOf("-", start);
            if (indexOfSecondDelimeter < 0) {
                indexOfSecondDelimeter = filename.IndexOf("(", start);
            }

            return indexOfSecondDelimeter;
        }
    }
}
