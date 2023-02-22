using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
using MediaFilesOnDvd.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MigrateLegacy.Services {
    public class LegacyGenreService {
        private LegacyMediaFilesContext _legacyContext;

        public LegacyGenreService(LegacyMediaFilesContext legacyContext) {
            _legacyContext = legacyContext;
        }

        public IEnumerable<LegacyGenre> Get() {
            return _legacyContext.LegacyGenres
                .OrderBy(x => x.Id);
        }

        public static IEnumerable<FileGenre> MigrateToNewFileGenres(IEnumerable<LegacyGenre> legacyGenres) {
            List<FileGenre> newFileGenres = new();
            foreach (var legacyGenre in legacyGenres) {
                FileGenre fileGenre = new() {
                    Name = legacyGenre.Name
                };
                newFileGenres.Add(fileGenre);
            }
            return newFileGenres;
        }
    }
}
