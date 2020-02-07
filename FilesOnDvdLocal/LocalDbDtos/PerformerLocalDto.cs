using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.LocalDbDtos {
    public class PerformerLocalDto {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAlias { get; set; }

        public PerformerLocalDto() {
            IsAlias = false;
        }
    }
}
