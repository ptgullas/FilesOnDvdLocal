using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
using MediaFilesOnDvd.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MigrateLegacy.Services {
    public class LegacyPublisherService {
        private LegacyMediaFilesContext _legacyContext;

        public LegacyPublisherService(LegacyMediaFilesContext legacyContext) {
            _legacyContext = legacyContext;
        }

        public IEnumerable<LegacyPublisher> Get() {
            throw new NotImplementedException();
        }

        // MigrateToNewPublisher
    }
}
