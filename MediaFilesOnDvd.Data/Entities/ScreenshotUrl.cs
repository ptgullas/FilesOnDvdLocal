using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class ScreenshotUrl {
        public int Id { get; set; }
        public string Url { get; set; }

        public int MediaFileId { get; set; }
        public MediaFile MediaFile { get; set; }

        public ScreenshotUrl() {
        }
    }
}
