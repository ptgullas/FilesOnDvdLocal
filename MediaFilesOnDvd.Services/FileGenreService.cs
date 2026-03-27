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

        public OperationResult Add(FileGenre genre) {
            if (_context.FileGenres.Any(g => g.Name.ToLower() == genre.Name.ToLower())) {
                return new(false, $"Genre {genre.Name} already exists in database");
            }
            try {
                _context.FileGenres.Add(genre);
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception e) {
                return new(false, "Error adding Genre to database");
            }
        }
    }
}
