using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class Performer {
        public int Id { get; set; }
        public string Name { get; set; }
        public PerformerType Type { get; set; }
        public List<string> Aliases { get; set; } = new List<string>();
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
