using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CosmosExplorer.Avalonia.Services;
using CosmosExplorer.Core.State;
using ReactiveUI;

namespace CosmosExplorer.Avalonia.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IUserSettingsService _userSettingsService;

    public MainWindowViewModel(IUserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService;
    }
    
    public string Greeting => "Welcome to Avalonia!";

    // Add our SimpleViewModel.
    // Note: We need at least a get-accessor for our Properties.
    public SimpleViewModel SimpleViewModel { get; } = new SimpleViewModel();


    // Add our ReactiveViewModel
    public ReactiveViewModel ReactiveViewModel { get; } = new ReactiveViewModel(new FileSystemUserSettingsService());

  
}



    // Instead of implementing "INotifyPropertyChanged" on our own we use "ReactiveObject" as 
    // our base class. Read more about it here: https://www.reactiveui.net
    public partial class ReactiveViewModel : ObservableObject
    {
        private readonly IUserSettingsService userSettingsService;

        public ReactiveViewModel(IUserSettingsService userSettingsService)
        {
            this.userSettingsService = userSettingsService;
            // We can listen to any property changes with "WhenAnyValue" and do whatever we want in "Subscribe".
            //this.WhenAnyValue(o => o.Name)
            //    .Subscribe(o => this.RaisePropertyChanged(nameof(Greeting)));
        }

        private string? connectionString; // This is our backing field for Name
        
        public async Task SaveAsync(string parameter)
        {
            var userSettings = await userSettingsService.GetSettingsAsync();
            userSettings.ConnectionString = connectionString;
            await userSettingsService.SaveSettingsAsync(userSettings);
        }
        public string? Name
        {
            get
            {
                return connectionString;
            }
            set
            {

                connectionString = value;
            }
        }

        // Greeting will change based on a Name.
        public string Greeting
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    // If no Name is provided, use a default Greeting
                    return "Hello World from Avalonia.Samples";
                }
                else
                {
                    // else Greet the User.
                    return $"Hello {Name}";
                }
            }
        }
    }



    // This is our simple ViewModel. We need to implement the interface "INotifyPropertyChanged"
    // in order to notify the View if any of our properties changed.
    public class SimpleViewModel : INotifyPropertyChanged
    {
        // This event is implemented by "INotifyPropertyChanged" and is all we need to inform 
        // our View about changes.
        public event PropertyChangedEventHandler? PropertyChanged;

        // For convenience we add a method which will raise the above event.
        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ---- Add some Properties ----

        private string? _Name; // This is our backing field for Name

        public string? Name
        {
            get { return _Name; }
            set
            {
                // We only want to update the UI if the Name actually changed, so we check if the value is actually new
                if (_Name != value)
                {
                    // 1. update our backing field
                    _Name = value;

                    // 2. We call RaisePropertyChanged() to notify the UI about changes. 
                    // We can omit the property name here because [CallerMemberName] will provide it for us.  
                    RaisePropertyChanged();

                    // 3. Greeting also changed. So let's notify the UI about it. 
                    RaisePropertyChanged(nameof(Greeting));
                }
            }
        }


        // Greeting will change based on a Name.
        public string Greeting
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    // If no Name is provided, use a default Greeting
                    return "Hello World from Avalonia.Samples";
                }
                else
                {
                    // else Greet the User.
                    return $"Hello {Name}";
                }
            }
        }
    }
