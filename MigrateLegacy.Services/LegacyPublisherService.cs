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
            return _legacyContext.LegacyPublishers
                .OrderBy(p => p.Id);
        }

        public IEnumerable<SeriesPublisher> MigrateToSeriesPublishers(IEnumerable<LegacyPublisher> legacyPublishers) {
            List<SeriesPublisher> seriesPublishers = new();
            foreach (LegacyPublisher legacyPublisher in legacyPublishers) {
                SeriesPublisher seriesPublisher = new() {
                    Name = legacyPublisher.Name
                };
                seriesPublishers.Add(seriesPublisher);
            }
            return seriesPublishers;
        }
    }
}
