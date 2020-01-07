using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories {
    public interface IPerformerRepository {
        List<PerformerLocalDto> Get();
        PerformerLocalDto Get(string name);
        PerformerLocalDto Get(int id);
    }
}
