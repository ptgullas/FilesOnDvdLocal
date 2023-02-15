using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Services {
    public class PerformerService {
        private readonly MediaFilesContext _context;

        public PerformerService(MediaFilesContext context) {
            _context = context;
        }

        /// <summary>
        /// Get all Performers, ordered by Name
        /// </summary>
        /// <returns>An <c>IEnumerable</c> of Performer, sorted by Name</returns>
        public IEnumerable<Performer> Get() {
            return _context.Performers.OrderBy(p => p.Name);
        }

        public IEnumerable<Performer> GetWithMediaFiles() {
            return _context.Performers
                .OrderBy(p => p.Name)
                .Include(p => p.HeadshotUrls)
                .Include(p => p.MediaFiles.OrderBy(m => m.Name));
        }

        public Performer? GetWithMediaFiles(int id) {
            return _context.Performers
                .Include(p => p.MediaFiles.OrderBy(m => m.Name))
                .Include(p => p.HeadshotUrls)
                .Where(p => p.Id == id)
                .FirstOrDefault();
        }

        public Performer? GetWithMediaFiles(string performerName) {
            return _context.Performers
                .Include(p => p.MediaFiles.OrderBy(m => m.Name))
                .Include(p => p.HeadshotUrls)
                .Where(p => p.Name.ToLower() == performerName.ToLower())
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns Performers that do not have Headshots listed, in reverse Id order
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Performer> GetWithMissingHeadshots() {
            return _context.Performers
                .Where(p => p.HeadshotUrls.Count == 0)
                .OrderByDescending(p => p.Id);

        }

        public OperationResult Add(Performer performer) {
            if (performer == null) {
                return new(false, "Performer is null");
            }
            if (_context.Performers.Any(p => p.Name.ToLower() == performer.Name.ToLower())) {
                return new(false, $"Performer {performer.Name} already exists in database");
            }
            _context.Performers.Add(performer);
            _context.SaveChanges();
            return new(true);
        }

        public OperationResult Add(string newPerformerName) {
            if (string.IsNullOrEmpty(newPerformerName)) {
                return new(false, "Performer name is null or empty!");
            }
            if (_context.Performers.Any(p => p.Name.ToLower() == newPerformerName.ToLower())) {
                return new(false, $"Performer {newPerformerName} already exists in database");
            }
            var performer = new Performer() {
                Name = newPerformerName
            };
            _context.Performers.Add(performer);
            _context.SaveChanges();
            return new(true);
        }
    }
}
