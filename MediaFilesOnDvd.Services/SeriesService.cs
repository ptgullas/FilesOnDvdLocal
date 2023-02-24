using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Services {
    public class SeriesService {
        private readonly MediaFilesContext _context;

        public SeriesService(MediaFilesContext context) {
            _context = context;
        }

        public IEnumerable<Series> Get() {
            return _context.Series
                .OrderBy(s => s.Name);
        }

        public Series? Get(int id) {
            return _context.Series.FirstOrDefault(s => s.Id == id);
        }

        public OperationResult Add(Series series) {
            if ((_context.Series.Any(s => s.Id == series.Id)) ||
                (_context.Series.Any(s => s.Name.ToLower() == series.Name.ToLower()))) {
                return new(false, $"Series {series.Name} already exists in database.");
            }
            _context.Series.Add(series);
            _context.SaveChanges();
            return new(true);
        }
    }
}
