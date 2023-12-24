using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CosmosExplorer.Avalonia.Services;
using CosmosExplorer.Avalonia.ViewModels;
using CosmosExplorer.Avalonia.Views;
using CosmosExplorer.Core;
using CosmosExplorer.Core.Models;
using CosmosExplorer.Domain;
using CosmosExplorer.Infrastructure.Command;
using CosmosExplorer.Infrastructure.Connection;
using CosmosExplorer.Infrastructure.Query;

namespace CosmosExplorer.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        CosmosExplorer.Infrastructure.Connection.CosmosDbConnectionService cosmosDbConnectionService = new();
        StateContainer stateContainer = new();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                
                DataContext = new MainWindowViewModel(new FileSystemUserSettingsService(),new CosmosDBDocumentService(
                    stateContainer,cosmosDbConnectionService,new CosmosDbQueryService(cosmosDbConnectionService),
                    new CosmosDbCommandService(cosmosDbConnectionService)),stateContainer)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}

