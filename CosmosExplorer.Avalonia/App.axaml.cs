using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CosmosExplorer.Avalonia.Services;
using CosmosExplorer.Avalonia.ViewModels;
using CosmosExplorer.Avalonia.Views;

namespace CosmosExplorer.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(new FileSystemUserSettingsService()),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}