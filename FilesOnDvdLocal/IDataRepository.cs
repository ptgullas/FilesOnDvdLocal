using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public interface IDataRepository {
        List<PerformerLocalDto> GetPerformers();
        PerformerLocalDto GetPerformerByName(string performerName);
    }
}
