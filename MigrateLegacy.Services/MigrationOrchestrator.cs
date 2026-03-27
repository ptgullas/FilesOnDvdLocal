using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;
using MigrateLegacy.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MigrateLegacy.Services {
    public class MigrationOrchestrator {
        private readonly LegacyWalletService _legacyWalletService;
        private readonly WalletService _walletService;
        private readonly LegacyGenreService _legacyGenreService;
        private readonly FileGenreService _fileGenreService;
        private readonly LegacyPublisherService _legacyPublisherService;
        private readonly SeriesPublisherService _seriesPublisherService;
        private readonly LegacyPerformerService _legacyPerformerService;
        private readonly PerformerService _performerService;
        private readonly PerformerTypeService _performerTypeService;
        private readonly LegacyDiscService _legacyDiscService;
        private readonly DiscService _discService;
        private readonly LegacySeriesService _legacySeriesService;
        private readonly SeriesService _seriesService;
        private readonly LegacyFilenameService _legacyFilenameService;
        private readonly MediaFileService _mediaFileService;

        public event EventHandler<string>? ProgressReported;

        public MigrationOrchestrator(
            LegacyWalletService legacyWalletService, WalletService walletService,
            LegacyGenreService legacyGenreService, FileGenreService fileGenreService,
            LegacyPublisherService legacyPublisherService, SeriesPublisherService seriesPublisherService,
            LegacyPerformerService legacyPerformerService, PerformerService performerService,
            PerformerTypeService performerTypeService,
            LegacyDiscService legacyDiscService, DiscService discService,
            LegacySeriesService legacySeriesService, SeriesService seriesService,
            LegacyFilenameService legacyFilenameService, MediaFileService mediaFileService) {
            _legacyWalletService = legacyWalletService;
            _walletService = walletService;
            _legacyGenreService = legacyGenreService;
            _fileGenreService = fileGenreService;
            _legacyPublisherService = legacyPublisherService;
            _seriesPublisherService = seriesPublisherService;
            _legacyPerformerService = legacyPerformerService;
            _performerService = performerService;
            _performerTypeService = performerTypeService;
            _legacyDiscService = legacyDiscService;
            _discService = discService;
            _legacySeriesService = legacySeriesService;
            _seriesService = seriesService;
            _legacyFilenameService = legacyFilenameService;
            _mediaFileService = mediaFileService;
        }

        public void MigrateAll() {
            ReportProgress("Starting migration...");

            // 1. Wallets
            ReportProgress("Step 1/8: Migrating Wallets...");
            var legacyWallets = _legacyWalletService.Get();
            var newWallets = LegacyWalletService.MigrateToNewWallets(legacyWallets);
            foreach (var w in newWallets) {
                _walletService.Add(w);
            }

            // 2. FileGenres
            ReportProgress("Step 2/8: Migrating FileGenres...");
            var legacyGenres = _legacyGenreService.Get();
            var newGenres = LegacyGenreService.MigrateToNewFileGenres(legacyGenres);
            foreach (var g in newGenres) {
                _fileGenreService.Add(g);
            }

            // 3. SeriesPublishers
            ReportProgress("Step 3/8: Migrating SeriesPublishers...");
            var legacyPublishers = _legacyPublisherService.Get();
            foreach (var lp in legacyPublishers) {
                _seriesPublisherService.Add(lp.Name);
            }

            // 4. PerformerTypes
            ReportProgress("Step 4/8: Migrating PerformerTypes...");
            var legacyPerformerTypes = _legacyPerformerService.GetPerformerTypes();
            var newPerformerTypes = LegacyPerformerService.MigrateToPerformerTypes(legacyPerformerTypes);
            foreach (var pt in newPerformerTypes) {
                _performerTypeService.Add(pt);
            }

            // 5. Performers
            ReportProgress("Step 5/8: Migrating Performers...");
            var legacyPerformers = _legacyPerformerService.Get();
            var modernPerformerTypes = _performerTypeService.Get().ToList();
            var newPerformers = LegacyPerformerService.MigrateToPerformers(legacyPerformers, modernPerformerTypes);
            foreach (var p in newPerformers) {
                _performerService.Add(p);
            }

            // 6. Discs
            ReportProgress("Step 6/8: Migrating Discs...");
            var legacyDiscs = _legacyDiscService.Get();
            var newDiscs = _legacyDiscService.MigrateToNewDiscs(legacyDiscs, _walletService);
            foreach (var d in newDiscs) {
                _discService.Add(d);
            }

            // 7. Series
            ReportProgress("Step 7/8: Migrating Series...");
            var legacySeries = _legacySeriesService.Get();
            var newSeries = _legacySeriesService.MigrateToNewSeries(legacySeries, _seriesPublisherService, _fileGenreService);
            foreach (var s in newSeries) {
                _seriesService.Add(s);
            }

            // 8. MediaFiles
            ReportProgress("Step 8/8: Migrating MediaFiles (this may take a while)...");
            var legacyFilenames = _legacyFilenameService.Get();
            var mediaFiles = _legacyFilenameService.MigrateToMediaFiles(legacyFilenames, _discService, _fileGenreService, _performerService, _seriesService);
            foreach (var mf in mediaFiles) {
                _mediaFileService.Add(mf);
            }

            ReportProgress("Migration complete!");
        }

        private void ReportProgress(string message) {
            ProgressReported?.Invoke(this, message);
        }
    }
}
