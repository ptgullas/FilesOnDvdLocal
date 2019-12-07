using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public static class StringExtensions {
        public static int GetIndexOfFirstHyphen(this string str, int skip = 0) {
            int index = str.IndexOf("-");
            if (skip > 0 && index >= 0) {
                int hyphenCount = str.Count(f => f == '-');
                for (int i = 0; i <= hyphenCount; i++) {
                    index = str.IndexOf("-", index + 1);
                }
            }
            return index;
        }
    }
}
