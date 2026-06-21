using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class PerformerAlias {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public int PerformerId { get; set; }
        public virtual Performer Performer { get; set; } = null!;
    }
}
