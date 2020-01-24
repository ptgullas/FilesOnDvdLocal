using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.LocalDbDtos {
    public class FileLocalDto {

        public int? Id { get; set; }
        public string Filename { get; set; }
        public int? Genre { get; set; }
        public int? Disc { get; set; }
        public int? Series { get; set; }

        public FileLocalDto() {

        }
        public FileLocalDto(FileToImport file) {
            Id = file.DatabaseId;
            Filename = file.Filename;
            Genre = file.Genre;
            Disc = file.DiscId;
            Series = file.Series.Id;
        }
    }
}
