using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;
using Microsoft.EntityFrameworkCore;

namespace MigrateLegacy.Services {
    public class LegacyFilenameService {
        private LegacyMediaFilesContext _legacyContext;

        public LegacyFilenameService(LegacyMediaFilesContext legacyContext) {
            _legacyContext = legacyContext;
        }

        public IEnumerable<LegacyFilename> Get() {
            var legacyFilenames = _legacyContext.LegacyFilenames
                .OrderBy(lf => lf.Id);
            return legacyFilenames;
        }

        public LegacyFilename? Get(int id) {
            return _legacyContext.LegacyFilenames
                .Include(lf => lf.Genre)
                .Include(lf => lf.Series)
                .Include(lf => lf.Disc)
                .Include(lf => lf.Performers.OrderBy(p => p.Name))
                .FirstOrDefault(lf => lf.Id == id)
                ;
        }

        public IEnumerable<LegacyFilename> GetByDiscName(string discName) {
            return _legacyContext.LegacyFilenames
                .Where(lf => lf.Disc.Name.ToLower() == discName.ToLower())
                .OrderBy(lf => lf.Name);
        }

        public IEnumerable<LegacyFilename> GetBySeries(string seriesName) {
            return _legacyContext.LegacyFilenames
                .Where(lf => lf.Series.Name.ToLower() == seriesName.ToLower())
                .OrderBy(lf => lf.Name.ToLower());
        }

        public IEnumerable<MediaFile> MigrateToMediaFiles(IEnumerable<LegacyFilename> legacyFiles, DiscService discService, FileGenreService fileGenreService) {
            List<MediaFile> mediaFiles = new();
            var fileGenres = fileGenreService.Get();
            var discs = discService.Get();
            foreach (var legacyFile in legacyFiles) {
                MediaFile mf = new(legacyFile.Name, legacyFile.Notes, $"{legacyFile.Id}.jpg");
                mediaFiles.Add(mf);
                var disc = discService.Get(legacyFile.Disc.Name.ToLower());
                if (disc is not null) {
                    disc.Files.Add(mf);
                }
                var genre = fileGenres.FirstOrDefault(g => g.Name.ToLower() == legacyFile.Genre.Name.ToLower());
                if (genre is not null) {
                    genre.MediaFiles.Add(mf);
                }
            }
            return mediaFiles;
        }
    }
}
