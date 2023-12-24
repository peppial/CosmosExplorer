using Avalonia;
using Avalonia.ReactiveUI;
using System;
using CosmosExplorer.Avalonia.Services;
using CosmosExplorer.Core;
using CosmosExplorer.Core.Command;
using CosmosExplorer.Core.Connection;
using CosmosExplorer.Core.Models;
using CosmosExplorer.Core.Query;
using CosmosExplorer.Core.State;
using CosmosExplorer.Domain;
using CosmosExplorer.Infrastructure.Command;
using CosmosExplorer.Infrastructure.Connection;
using CosmosExplorer.Infrastructure.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CosmosExplorer.Avalonia;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args);
        hostBuilder.RunConsoleAsync();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    { 
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
        
        
    } 
    
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).
            ConfigureServices((hostingContext, services) =>
            {
                ConfigureServices(services);
            });
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ICosmosDBDocumentService, CosmosDBDocumentService>();
        services.AddSingleton<IUserSettingsService, FileSystemUserSettingsService>();
        services.AddSingleton<IContainerModel, CosmosDbContainerModel>();
        services.AddSingleton<IDatabaseModel, CosmosDbDatabaseModel>();
        services.AddSingleton<IConnectionService,CosmosDbConnectionService>();
        services.AddScoped<IQueryService, CosmosDbQueryService>();
        services.AddScoped<ICommandService, CosmosDbCommandService>();
    }
}