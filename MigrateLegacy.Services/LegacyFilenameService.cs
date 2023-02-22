﻿using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
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
                .Include(lf => lf.GenreNavigation)
                .FirstOrDefault(lf => lf.Id == id)
                ;
        }

        public IEnumerable<LegacyFilename> GetByDiscName(string discName) {
            return _legacyContext.LegacyFilenames
                .Where(lf => lf.DiscNavigation.Name.ToLower() == discName.ToLower())
                .OrderBy(lf => lf.Name)
                ;
        }
    }
}
