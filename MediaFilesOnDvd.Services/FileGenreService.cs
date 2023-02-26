using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Services {
    public class FileGenreService {
        private readonly MediaFilesContext _context;

        public FileGenreService(MediaFilesContext context) {
            _context = context;
        }

        public IEnumerable<FileGenre> Get() {
            return _context.FileGenres.OrderBy(f => f.Id);
        }
    }
}
