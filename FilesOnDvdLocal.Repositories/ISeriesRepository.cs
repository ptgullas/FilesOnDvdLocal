using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories {
    public interface ISeriesRepository {
        List<SeriesLocalDto> Get();
        SeriesLocalDto Get(string name);
    }
}
