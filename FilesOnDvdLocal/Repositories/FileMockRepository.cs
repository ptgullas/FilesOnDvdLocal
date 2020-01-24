using FilesOnDvdLocal.LocalDbDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories {
    public class FileMockRepository : IFileRepository {
        public int Add(FileToImport file) {
            FileLocalDto fileDto = new FileLocalDto(file);
            return Add(fileDto);
        }

        public int Add(FileLocalDto fileDto) {
            throw new NotImplementedException();
        }

        public List<FileLocalDto> GetByDisc(int discId) {
            throw new NotImplementedException();
        }
    }
}
