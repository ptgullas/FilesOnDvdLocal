using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
using MediaFilesOnDvd.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MigrateLegacy.Services {
    public class LegacyPerformerService {
        private readonly LegacyMediaFilesContext _legacyContext;

        public LegacyPerformerService(LegacyMediaFilesContext legacyContext) {
            _legacyContext = legacyContext;
        }

        public IEnumerable<LegacyPerformer> Get() {
            return _legacyContext.LegacyPerformers
                .Include(lp => lp.PerformerType)
                .OrderBy(lp => lp.Name);
        }

        public IEnumerable<LegacyPerformerType> GetPerformerTypes() {
            return _legacyContext.LegacyPerformerTypes
                .OrderBy(lpt => lpt.Id);
        }

        public static IEnumerable<PerformerType> MigrateToPerformerTypes(IEnumerable<LegacyPerformerType> legacyPerformerTypes) {
            return legacyPerformerTypes.Select(lpt => new PerformerType {
                Name = lpt.Name
            });
        }

        public static IEnumerable<Performer> MigrateToPerformers(IEnumerable<LegacyPerformer> legacyPerformers, IEnumerable<PerformerType> modernPerformerTypes) {
            return legacyPerformers.Select(lp => {
                var performer = new Performer {
                    Name = lp.Name,
                    LegacyId = (int)lp.Id,
                };
                if (!string.IsNullOrEmpty(lp.Headshot)) {
                    performer.HeadshotUrls.Add(new HeadshotUrl(lp.Headshot));
                }
                if (lp.PerformerType is not null) {
                    var modernType = modernPerformerTypes.FirstOrDefault(pt => pt.Name == lp.PerformerType.Name);
                    if (modernType is not null) {
                        performer.Type = modernType;
                    }
                }
                return performer;
            });
        }
    }
}
