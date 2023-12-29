using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CosmosExplorer.Avalonia.Services;
using CosmosExplorer.Avalonia.ViewModels;
using CosmosExplorer.Avalonia.Views;
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
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace CosmosExplorer.Avalonia;

public partial class App : Application
{
    public IServiceProvider Container { get; private set; }
    public IHost host { get; set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
      
        
        host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.UseMicrosoftDependencyResolver();
                var resolver = Locator.CurrentMutable;
                resolver.InitializeSplat();
                resolver.InitializeReactiveUI();
                
                Locator.CurrentMutable.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
                Locator.CurrentMutable.RegisterConstant(new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
                RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

                services.AddTransient<MainWindow>();
                services.AddTransient<MainWindowViewModel>();
                
                services.AddSingleton<ICosmosDBDocumentService, CosmosDBDocumentService>();
                services.AddSingleton<IUserSettingsService, FileSystemUserSettingsService>();
                services.AddSingleton<IStateContainer, StateContainer>();
                services.AddSingleton<IContainerModel, CosmosDbContainerModel>();
                services.AddSingleton<IDatabaseModel, CosmosDbDatabaseModel>();
                services.AddSingleton<IConnectionService,CosmosDbConnectionService>();
                services.AddSingleton<IQueryService, CosmosDbQueryService>();
                services.AddSingleton<ICommandService, CosmosDbCommandService>();

            })
            .Build();
        Container = host.Services;
        Container.UseMicrosoftDependencyResolver();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
             
            desktop.MainWindow = new MainWindow
            {
                DataContext = host.Services.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}

