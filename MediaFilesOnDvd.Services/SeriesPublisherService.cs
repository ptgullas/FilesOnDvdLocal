using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Services {
    public class SeriesPublisherService {
        private readonly MediaFilesContext _context;

        public SeriesPublisherService(MediaFilesContext context) {
            _context = context;
        }

        public IEnumerable<SeriesPublisher> Get() {
            return _context.SeriesPublishers.OrderBy(x => x.Name);
        }

        public SeriesPublisher? Get(int id) {
            return _context.SeriesPublishers.FirstOrDefault(x => x.Id == id);
        }


        public OperationResult Add(string name) {
            var result = _context.SeriesPublishers
                .FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (result is not null) {
                return new(false, $"SeriesPublisher {name} already exists in database.");
            }
            SeriesPublisher newPublisher = new() { Name = name };
            try {
                _context.SeriesPublishers.Add(newPublisher);
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception e) {
                // log exception
                return new(false, $"Unknown error adding Publisher {name} to database, {e}");
            }
        }
    }
}
