using MediaFilesOnDvd.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaFilesOnDvd.Data {
    public class MediaFilesContext : DbContext {
        public DbSet<Disc> Discs { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<PerformerType> PerformerTypes { get; set; }
        public DbSet<FileGenre> FileGenres { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<SeriesPublisher> SeriesPublishers { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
    }
}