using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services.Dtos;
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


        public IEnumerable<PerformerSummaryDto> GetSummaries() {
            var query = _context.Performers
                .OrderBy(p => p.Name)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    CreditCount = p.MediaFiles.Count,
                    Aliases = p.PerformerAliases.Select(a => a.Name).ToList(),
                    Headshots = p.HeadshotUrls.Select(h => new { h.Url, h.IsPreferred }).ToList()
                }).ToList();

            return query.Select(p => new PerformerSummaryDto {
                Id = p.Id,
                Name = p.Name,
                Aliases = p.Aliases,
                CreditCount = p.CreditCount,
                HeadshotUrl = p.Headshots.OrderByDescending(h => h.IsPreferred).Select(h => h.Url).FirstOrDefault()
            });
        }

        public PerformerDetailDto? GetDetails(int id) {
            var p = _context.Performers
                .Where(x => x.Id == id)
                .Select(x => new {
                    x.Id,
                    x.Name,
                    Aliases = x.PerformerAliases.Select(a => a.Name).ToList(),
                    Headshots = x.HeadshotUrls.Select(h => new { h.Url, h.IsPreferred }).ToList(),
                    GalleryPhotoUrls = x.GalleryPhotoUrls.Select(g => g.Url).ToList(),
                    MediaFiles = x.MediaFiles.Select(m => new MediaFileSummaryDto {
                        Id = m.Id,
                        Name = m.Name,
                        DiscName = m.Disc.Name,
                        ScreenshotUrl = m.Screenshots.Select(s => s.Url).FirstOrDefault()
                    }).OrderBy(m => m.Name).ToList()
                })
                .FirstOrDefault();

            if (p == null) return null;

            return new PerformerDetailDto {
                Id = p.Id,
                Name = p.Name,
                Aliases = p.Aliases,
                HeadshotUrl = p.Headshots.OrderByDescending(h => h.IsPreferred).Select(h => h.Url).FirstOrDefault(),
                HeadshotUrls = p.Headshots.Select(h => h.Url).ToList(),
                GalleryPhotoUrls = p.GalleryPhotoUrls,
                MediaFiles = p.MediaFiles
            };
        }

        public IEnumerable<PerformerWithGalleryDto> GetWithGalleries() {
            var query = _context.Performers
                .OrderBy(p => p.Name)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    Headshots = p.HeadshotUrls.Select(h => new { h.Url, h.IsPreferred }).ToList(),
                    GalleryPhotoUrl = p.GalleryPhotoUrls.Select(g => g.Url).FirstOrDefault()
                }).ToList();

            return query.Select(p => new PerformerWithGalleryDto {
                Id = p.Id,
                Name = p.Name,
                HeadshotUrl = p.Headshots.OrderByDescending(h => h.IsPreferred).Select(h => h.Url).FirstOrDefault(),
                GalleryPhotoUrl = p.GalleryPhotoUrl
            });
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

        public Performer? GetByLegacyId(int legacyId) {
            return _context.Performers.FirstOrDefault(p => p.LegacyId == legacyId);
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

        public OperationResult Update(PerformerEditDto dto) {
            Performer? performer = null;
            if (dto.Id > 0) {
                performer = _context.Performers
                    .Include(p => p.HeadshotUrls)
                    .Include(p => p.GalleryPhotoUrls)
                    .Include(p => p.PerformerAliases)
                    .FirstOrDefault(p => p.Id == dto.Id);
                
                if (performer == null) return new(false, "Performer not found");
            } else {
                performer = new Performer { Name = dto.Name };
                _context.Performers.Add(performer);
            }

            performer.Name = dto.Name;
            
            // Handle Aliases
            performer.PerformerAliases.Clear();
            if (dto.Aliases != null && dto.Aliases.Any()) {
                foreach(var alias in dto.Aliases) {
                    if (!string.IsNullOrWhiteSpace(alias)) {
                        performer.PerformerAliases.Add(new PerformerAlias { Name = alias.Trim(), PerformerId = performer.Id });
                    }
                }
            }

            // Handle HeadshotUrls
            var headshotsToRemove = performer.HeadshotUrls.Where(h => !dto.HeadshotUrls.Contains(h.Url)).ToList();
            foreach (var h in headshotsToRemove) {
                performer.HeadshotUrls.Remove(h);
            }

            foreach (var url in dto.HeadshotUrls) {
                if (!string.IsNullOrWhiteSpace(url)) {
                    var existing = performer.HeadshotUrls.FirstOrDefault(h => h.Url == url);
                    if (existing == null) {
                        existing = new HeadshotUrl(url);
                        performer.HeadshotUrls.Add(existing);
                    }
                    existing.IsPreferred = (url == dto.PreferredHeadshotUrl);
                }
            }

            // Ensure exactly one is preferred if any exist
            if (performer.HeadshotUrls.Any() && !performer.HeadshotUrls.Any(h => h.IsPreferred)) {
                performer.HeadshotUrls.First().IsPreferred = true;
            }

            // Handle GalleryPhotoUrls
            var galleriesToRemove = performer.GalleryPhotoUrls.Where(g => !dto.GalleryPhotoUrls.Contains(g.Url)).ToList();
            foreach (var g in galleriesToRemove) {
                performer.GalleryPhotoUrls.Remove(g);
            }

            foreach (var url in dto.GalleryPhotoUrls) {
                if (!string.IsNullOrWhiteSpace(url) && !performer.GalleryPhotoUrls.Any(g => g.Url == url)) {
                    performer.GalleryPhotoUrls.Add(new GalleryPhotoUrl() { Url = url });
                }
            }

            try {
                _context.SaveChanges();
                if (dto.Id == 0) {
                    dto.Id = performer.Id;
                }
                return new(true);
            }
            catch (Exception ex) {
                return new(false, ex.Message);
            }
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

        /// <summary>
        /// Get all Performer Entities, ordered by Name
        /// Used for Migration from Legacy DB
        /// </summary>
        /// <returns>An <c>IEnumerable</c> of Performer, sorted by Name</returns>
        public IEnumerable<Performer> GetPerformerDbEntities() {
            return _context.Performers.OrderBy(p => p.Name);
        }

    }
}
