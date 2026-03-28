using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Embedding;
using System.Reflection;

namespace MediaFilesOnDvd.Desktop
{
    public static partial class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Load configuration
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("MediaFilesOnDvd.Desktop.appsettings.json");
            
            // Note: Since we are in a WinUI host, we can also just load from file system
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            builder.Configuration.AddConfiguration(config);

            builder.UseMauiEmbeddedApp<MyApp>()
                   .ConfigureFonts(fonts =>
                   {
                       fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                       fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                   });

            builder.Services.AddMauiBlazorWebView();

            // Register Database
            builder.Services.AddDbContext<MediaFilesContext>(options =>
                options.UseSqlite(config.GetConnectionString("ModernDb")));

            // Register Services
            builder.Services.AddScoped<WalletService>();
            builder.Services.AddScoped<FileGenreService>();
            builder.Services.AddScoped<SeriesPublisherService>();
            builder.Services.AddScoped<PerformerService>();
            builder.Services.AddScoped<PerformerTypeService>();
            builder.Services.AddScoped<DiscService>();
            builder.Services.AddScoped<SeriesService>();
            builder.Services.AddScoped<MediaFileService>();
            builder.Services.AddScoped<FileTagService>();
            builder.Services.AddScoped<PathResolverService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
