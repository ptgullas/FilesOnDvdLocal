using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.LocalDbDtos {
    public class DiscLocalDto {
        public int Id { get; set; }
        public string DiscName { get; set; }
        public int Wallet { get; set; }
        public string Notes { get; set; }

        public DiscLocalDto() {
        }

        public DiscLocalDto(DvdFolderToImport dvdFolder) {
            DiscName = dvdFolder.DiscName;
            Wallet = dvdFolder.WalletType;
            Notes = dvdFolder.Notes;
        }
    }
}
