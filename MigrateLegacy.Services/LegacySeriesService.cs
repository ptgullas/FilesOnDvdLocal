using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrateLegacy.Services {
    public class LegacySeriesService {
        private LegacyMediaFilesContext _legacyContext;

        public LegacySeriesService(LegacyMediaFilesContext legacyContext) {
            _legacyContext = legacyContext;
        }

        public IEnumerable<LegacySeries> Get() {
            return _legacyContext.LegacySeries
                // leave out LegacyFilenames because we don't want to migrate them 
                // at the same time as LegacySeries (which comes earlier)
                // .Include(ls => ls.LegacyFilenames)
                .Include (ls => ls.Publisher)
                .Include (ls => ls.Genre)
                .OrderBy(ls => ls.Id);
        }

        public IEnumerable<Series> MigrateToNewSeries(IEnumerable<LegacySeries> legacySeries, SeriesPublisherService seriesPublisherService, FileGenreService fileGenreService) {
            List<Series> newSeries = new();
            var fileGenres = fileGenreService.Get();
            var seriesPublishers = seriesPublisherService.Get();
            foreach (var legacySeriesItem in legacySeries) {
                Series series = new() {
                    Name = legacySeriesItem.Name
                };

                AddNewSeriesToNewGenre(fileGenres, legacySeriesItem, series);
                AddNewSeriesToNewPublisher(seriesPublishers, legacySeriesItem, series);

                newSeries.Add(series);
            }

            return newSeries;
        }

        private static void AddNewSeriesToNewPublisher(IEnumerable<SeriesPublisher> seriesPublishers, LegacySeries legacySeriesItem, Series series) {
            if (legacySeriesItem.Publisher is not null) {
                var newPublisher = seriesPublishers.FirstOrDefault(
                    p => p.Name.ToLower() == legacySeriesItem.Publisher.Name.ToLower());
                if (newPublisher is not null) {
                    newPublisher.Series.Add(series);
                }
                else {
                    throw new ArgumentException($"{legacySeriesItem.Publisher.Name} not found in NewPublishers.");
                }
            }
        }

        private static void AddNewSeriesToNewGenre(IEnumerable<FileGenre> fileGenres, LegacySeries legacySeriesItem, Series series) {
            var newParentGenre = fileGenres.FirstOrDefault(
                f => f.Name.ToLower() == legacySeriesItem.Genre.Name.ToLower());
            if (newParentGenre is not null) {
                newParentGenre.Series.Add(series);
            }
            else {
                throw new ArgumentException($"{legacySeriesItem.Genre.Name} not found in fileGenres.");
            }
        }
    }
}
