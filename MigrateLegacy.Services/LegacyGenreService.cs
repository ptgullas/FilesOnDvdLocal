using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
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
    }
}
