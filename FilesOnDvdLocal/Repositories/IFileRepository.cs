using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories {
    public interface IFileRepository {
        int Add(FileToImport file);
        int Add(FileLocalDto fileDto);
        List<FileLocalDto> GetByDisc(int discId);
    }
}
