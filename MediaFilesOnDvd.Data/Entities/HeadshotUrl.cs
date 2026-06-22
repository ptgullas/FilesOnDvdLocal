using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class HeadshotUrl {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsPreferred { get; set; }

        public int PerformerId { get; set; }
        public Performer Performer { get; set; }

        public HeadshotUrl(string url, bool isPreferred = false) {
            Url = url;
            IsPreferred = isPreferred;
        }
        
        public HeadshotUrl() {}
    }
}
