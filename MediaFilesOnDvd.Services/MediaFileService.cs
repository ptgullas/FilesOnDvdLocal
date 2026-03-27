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
        public OperationResult Add(MediaFile mediaFile) {
            try {
                _context.MediaFiles.Add(mediaFile);
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception e) {
                // Log.Exception(e, "Error adding MediaFile to database);
                return new(false, "Error adding MediaFile to database");
            }
        }

        public OperationResult Add(MediaFile[] mediaFiles) {
            try {
                _context.MediaFiles.AddRange(mediaFiles);
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception e) {
                // Log.Exception(e, "Error adding MediaFiles to database);
                return new(false, "Error adding MediaFiles to database");
            }
        }

        public OperationResult AddPerformerToMediaFile(MediaFile mf, Performer p) {
            try {
                if (mf.Performers.Any(perf => perf.Name.ToLower() == p.Name.ToLower())) {
                    return new(false, $"MediaFile '{mf.Name}' already contains Performer {p.Name}");
                }
                mf.Performers.Add(p);
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception e) {
                // Log.Exception(e, "Error adding performer to database);
                return new(false, "Error adding performer to database");
            }
        }

        public OperationResult AddTagToMediaFile(MediaFile mf, FileTag tag) {
            try {
                if (mf.Tags.Any(t => t.Name.ToLower() == tag.Name.ToLower())) {
                    return new(false, $"MediaFile '{mf.Name}' already contains Tag {tag.Name}");
                }
                mf.Tags.Add(tag);
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception e) {
                return new(false, "Error adding tag to media file");
            }
        }

        public OperationResult RemoveTagFromMediaFile(MediaFile mf, FileTag tag) {
            try {
                var tagToRemove = mf.Tags.FirstOrDefault(t => t.Id == tag.Id);
                if (tagToRemove is null) {
                    return new(false, $"MediaFile '{mf.Name}' does not contain Tag {tag.Name}");
                }
                mf.Tags.Remove(tagToRemove);
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception e) {
                return new(false, "Error removing tag from media file");
            }
        }

        public IEnumerable<MediaFile> GetByTag(string tagName) {
            return _context.MediaFiles
                .Where(mf => mf.Tags.Any(t => t.Name.ToLower() == tagName.ToLower()))
                .Include(m => m.Performers)
                .Include(m => m.Screenshots)
                .Include(m => m.Disc)
                .Include(m => m.Tags);
        }
    }
}

