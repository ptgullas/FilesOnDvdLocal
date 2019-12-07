using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Data
{
    public class AccessAttachment {
        public string Filename { get; set; }
        public int DbFileEntryId { get; set; }
        public byte[] FileContents { get; set; }

    }
}
