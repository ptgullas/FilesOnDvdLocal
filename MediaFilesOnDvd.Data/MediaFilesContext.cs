using MediaFilesOnDvd.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaFilesOnDvd.Data {
    public class MediaFilesContext : DbContext {
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Disc> Discs { get; set; }

        public string? DbPath { get; }
        public DbSet<PerformerType> PerformerTypes { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<ScreenshotUrl> ScreenshotUrls { get; set; }
        /*
        public DbSet<FileGenre> FileGenres { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<SeriesPublisher> SeriesPublishers { get; set; }
        */

        public MediaFilesContext() {
            /*
            */
        }

        public MediaFilesContext(DbContextOptions<MediaFilesContext> options) 
            :base(options) {
        }

        // may have to comment this out after creating model
        // but uncomment when doing DB changes
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                var folder = Environment.SpecialFolder.Personal;
                var path = Environment.GetFolderPath(folder);
                path = Path.Combine(path, "MediaFilesOnDvd");
                string dbPath = Path.Join(path, "MediaFilesOnDvd.db");

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // comment this out since I'm using the "direct many-to-many setup"
            /*
            modelBuilder.Entity<Performer>()
                .HasMany(p => p.MediaFiles)
                .WithMany(mf => mf.Performers)
                .UsingEntity(e => e.ToTable("PerformerMediaFile"));
            */
        }
        


    }
}