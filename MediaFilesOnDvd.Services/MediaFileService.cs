using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Services {
    public class MediaFileService {
        private readonly MediaFilesContext _context;
        public MediaFileService(MediaFilesContext context) {
            _context = context;
        }

        public IEnumerable<MediaFile> Get() {
            return _context.MediaFiles.OrderBy(f => f.Id);
        }

        public MediaFile? Get(int id) {
            return _context.MediaFiles
                .Include(m => m.Performers)
                .Include(m => m.Screenshots)
                .Include(m => m.Disc)
                .FirstOrDefault(f => f.Id == id)
                ;
        }

        public IEnumerable<MediaFile> Get(string name) {
            return _context.MediaFiles.Where(
                n => n.Name.ToLower() == name.ToLower())
                .Include(m => m.Performers)
                .Include(m => m.Screenshots)
                .Include(m => m.Disc);
        }
        public void Add(MediaFile mediaFile) {
            _context.MediaFiles.Add(mediaFile);
            _context.SaveChanges();
        }

        public bool Add(MediaFile[] mediaFiles) {
            try {
                _context.MediaFiles.AddRange(mediaFiles);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e) {
                // Log.Exception(e, "Error adding MediaFiles to database);
                return false;
            }
        }

        public OperationResult AddPerformerToMediaFile(MediaFile mf, Performer p) {
            try {
                if (mf.Performers.Any(p => p.Name.ToLower() == p.Name.ToLower())) {
                    return new(false, $"MediaFile '{mf.Name}' already contains Performer {p.Name}");
                }
                mf.Performers.Add(p);
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception e) {
                // Log.Exception(e, "Error adding MediaFiles to database);
                return new(false, "Error adding performer to database");
            }
        }
    }
}
