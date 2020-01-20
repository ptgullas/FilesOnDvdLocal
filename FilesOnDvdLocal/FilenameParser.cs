using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
