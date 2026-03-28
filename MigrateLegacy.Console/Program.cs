using LegacyMediaFilesOnDvd.Data.Context;
using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Services;
using MigrateLegacy.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddDbContext<LegacyMediaFilesContext>(options =>
        options.UseSqlite(configuration.GetConnectionString("LegacyDb")))
    .AddDbContext<MediaFilesContext>(options =>
        options.UseSqlite(configuration.GetConnectionString("ModernDb")))
    .AddScoped<LegacyWalletService>()
    .AddScoped<WalletService>()
    .AddScoped<LegacyGenreService>()
    .AddScoped<FileGenreService>()
    .AddScoped<LegacyPublisherService>()
    .AddScoped<SeriesPublisherService>()
    .AddScoped<LegacyPerformerService>()
    .AddScoped<PerformerService>()
    .AddScoped<PerformerTypeService>()
    .AddScoped<LegacyDiscService>()
    .AddScoped<DiscService>()
    .AddScoped<LegacySeriesService>()
    .AddScoped<SeriesService>()
    .AddScoped<LegacyFilenameService>()
    .AddScoped<MediaFileService>()
    .AddScoped<MigrationOrchestrator>()
    .BuildServiceProvider();

using var scope = serviceProvider.CreateScope();
var modernContext = scope.ServiceProvider.GetRequiredService<MediaFilesContext>();
var orchestrator = scope.ServiceProvider.GetRequiredService<MigrationOrchestrator>();

AnsiConsole.Write(
    new FigletText("MediaFiles Migrator")
        .Centered()
        .Color(Color.Green));

while (true) {
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("What would you like to do?")
            .PageSize(10)
            .AddChoices(new[] {
                "1. Browse Wallets",
                "2. Browse Genres",
                "3. Browse Performers",
                "4. Browse Discs",
                "5. Browse Series",
                "6. Search MediaFiles",
                "Z. RUN MIGRATION",
                "Exit"
            }));

    switch (choice[0]) {
        case '1': BrowseWallets(scope); break;
        case '2': BrowseGenres(scope); break;
        case '3': BrowsePerformers(scope); break;
        case '4': BrowseDiscs(scope); break;
        case '5': BrowseSeries(scope); break;
        case '6': SearchMediaFiles(scope); break;
        case 'Z': RunMigration(orchestrator, modernContext); break;
        case 'E': return;
    }
}

void BrowseWallets(IServiceScope scope) {
    var service = scope.ServiceProvider.GetRequiredService<WalletService>();
    var wallets = service.Get();
    var table = new Table().AddColumns("ID", "Name", "Notes");
    foreach (var w in wallets) {
        if (w != null) table.AddRow(w.Id.ToString(), w.Name ?? "", w.Notes ?? "");
    }
    AnsiConsole.Write(table);
    Pause();
}

void BrowseGenres(IServiceScope scope) {
    var service = scope.ServiceProvider.GetRequiredService<FileGenreService>();
    var genres = service.Get();
    var table = new Table().AddColumns("ID", "Name");
    foreach (var g in genres) {
        if (g != null) table.AddRow(g.Id.ToString(), g.Name ?? "");
    }
    AnsiConsole.Write(table);
    Pause();
}

void BrowsePerformers(IServiceScope scope) {
    var service = scope.ServiceProvider.GetRequiredService<PerformerService>();
    var name = AnsiConsole.Ask<string>("Enter performer name (or leave blank for all):", "");
    var performers = string.IsNullOrEmpty(name) ? service.GetPerformerDbEntities() : service.GetPerformerDbEntities().Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    
    var table = new Table().AddColumns("ID", "Name", "LegacyID");
    foreach (var p in performers.Take(50)) {
        if (p != null) table.AddRow(p.Id.ToString(), p.Name ?? "", p.LegacyId?.ToString() ?? "");
    }
    AnsiConsole.Write(table);
    if (performers.Count() > 50) AnsiConsole.MarkupLine("[yellow]Showing first 50 results...[/]");
    Pause();
}

void BrowseDiscs(IServiceScope scope) {
    var service = scope.ServiceProvider.GetRequiredService<DiscService>();
    var discs = service.Get();
    var table = new Table().AddColumns("ID", "Name", "Wallet ID");
    foreach (var d in discs.Take(50)) {
        if (d != null) table.AddRow(d.Id.ToString(), d.Name ?? "", d.WalletId?.ToString() ?? "");
    }
    AnsiConsole.Write(table);
    Pause();
}

void BrowseSeries(IServiceScope scope) {
    var service = scope.ServiceProvider.GetRequiredService<SeriesService>();
    var series = service.Get();
    var table = new Table().AddColumns("ID", "Name");
    foreach (var s in series.Take(50)) {
        if (s != null) table.AddRow(s.Id.ToString(), s.Name ?? "");
    }
    AnsiConsole.Write(table);
    Pause();
}

void SearchMediaFiles(IServiceScope scope) {
    var service = scope.ServiceProvider.GetRequiredService<MediaFileService>();
    var searchTerm = AnsiConsole.Ask<string>("Enter search term:");
    var files = service.Get().Where(f => f.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    
    var table = new Table().AddColumns("ID", "Name", "Disc");
    foreach (var f in files.Take(50)) {
        if (f != null) table.AddRow(f.Id.ToString(), f.Name ?? "", f.Disc?.Name ?? "");
    }
    AnsiConsole.Write(table);
    Pause();
}

void Pause() {
    AnsiConsole.MarkupLine("\nPress [blue]ENTER[/] to return to menu...");
    Console.ReadLine();
}

void RunMigration(MigrationOrchestrator orchestrator, MediaFilesContext context) {
    AnsiConsole.MarkupLine("[bold red]WARNING:[/] This will migrate all data from the legacy database.");
    AnsiConsole.MarkupLine("It will [bold yellow]NOT[/] clear existing data in the modern database, which may cause duplicates.");
    
    if (AnsiConsole.Ask<string>("Type [bold green]YES[/] to confirm migration:") != "YES") {
        AnsiConsole.MarkupLine("[yellow]Migration cancelled.[/]");
        return;
    }

    // Ensure database is created
    context.Database.EnsureCreated();

    AnsiConsole.Status()
        .Start("Processing migration...", ctx => {
            orchestrator.ProgressReported += (s, msg) => {
                ctx.Status(msg);
                AnsiConsole.MarkupLine($"[grey]LOG:[/] {msg}");
            };
            orchestrator.MigrateAll();
        });

    AnsiConsole.MarkupLine("[bold green]Migration process finished![/]");
}
