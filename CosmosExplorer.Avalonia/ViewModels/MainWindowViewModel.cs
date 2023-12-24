using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmosExplorer.Core;
using CosmosExplorer.Core.State;
using DynamicData;

namespace CosmosExplorer.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IUserSettingsService userSettingsService;
        private readonly ICosmosDBDocumentService cosmosDbDocumentService;
        private readonly IStateContainer stateContainer;
        public ICommand LoadSettingsAsyncCommand { get; }
        public ICommand SaveSettingsAsyncCommand { get; }
        public ICommand QueryAsyncCommand { get; }
        public ICommand GetDocumentAsyncCommand { get; }

        public ObservableCollection<string> FullDocuments { get; } = new ObservableCollection<string>();
       
        private string? connectionString;
        public string? ConnectionString
        {
            get => connectionString;
            set => this.RaiseAndSetIfChanged(ref connectionString, value);
        }
        
        private string? query;
        public string? Query
        {
            get => query;
            set => this.RaiseAndSetIfChanged(ref query, value);
        }
        
        public ObservableCollection<(string, string)> Documents { get; } = new ObservableCollection<(string, string)>();

        private (string,string) selectedDocument;
        public (string,string) SelectedDocument
        {
            get { return selectedDocument; }
            set
            {
                if (selectedDocument == value)
                    return;
                selectedDocument = value;
                
                GetDocumentAsyncCommand.Execute(null);
            }
        }
        
        public ObservableCollection<(string database, string container)> Databases { get; } = new ObservableCollection<(string, string)>();

        private (string database, string container) selectedDatabase;
        public (string database, string container) SelectedDatabase
        {
            get { return selectedDatabase; }
            set
            {
                if (selectedDatabase == value)
                    return;
                selectedDatabase = value;
                
                GetDocumentAsyncCommand.Execute(null);

            }
        }
        public MainWindowViewModel(IUserSettingsService userSettingsService, ICosmosDBDocumentService cosmosDbDocumentService,IStateContainer stateContainer )
        {
            this.userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
            this.cosmosDbDocumentService = cosmosDbDocumentService ?? throw new ArgumentNullException(nameof(cosmosDbDocumentService));
            this.stateContainer = stateContainer ?? throw new ArgumentNullException(nameof(stateContainer));
            
             // see: https://www.reactiveui.net/docs/handbook/commands/
             
            // Init OpenThePodBayDoorsFellowRobotCommand
            // The IObservable<bool> is needed to enable or disable the command depending on valid parameters
            // The Observable listens to RobotName and will enable the Command if the name is not empty.
            //IObservable<bool> canExecuteFellowRobotCommand =
            //    this.WhenAnyValue(vm => vm.ConnectionString, (name) => !string.IsNullOrEmpty(name));

            //OpenThePodBayDoorsFellowRobotCommand = 
            //    ReactiveCommand.Create<string?>(name => OpenThePodBayDoorsFellowRobot(name), canExecuteFellowRobotCommand);

            LoadSettingsAsyncCommand = ReactiveCommand.CreateFromTask(LoadSettingsAsync);
            SaveSettingsAsyncCommand = ReactiveCommand.CreateFromTask(SaveSettingsAsync);
            QueryAsyncCommand = ReactiveCommand.CreateFromTask(QueryAsync); 
            GetDocumentAsyncCommand = ReactiveCommand.CreateFromTask(GetDocumentAsync); 

            LoadSettingsAsyncCommand.Execute(null);
            
        } 
        private async Task LoadSettingsAsync()
        {
            var connectionString = await userSettingsService.GetSettingsAsync();
            ConnectionString = connectionString.ConnectionString;
            stateContainer.ConnectionString = connectionString.ConnectionString;
            var databases = await cosmosDbDocumentService.GetDatabasesAsync();
            
            Databases.Clear();
            foreach (var database in databases)
            {
                foreach (var container in database.Containers)
                {
                    Databases.Add((database.Id, container.Id));
                }
            }

            SelectedDatabase = Databases[0];
        }
        private async Task SaveSettingsAsync()
        {
           var settings = await userSettingsService.GetSettingsAsync();
           settings.ConnectionString = ConnectionString;
           await userSettingsService.SaveSettingsAsync(settings);
        }
        
        private async Task QueryAsync()
        {
            try
            {
                await cosmosDbDocumentService.ChangeContainerAsync(selectedDatabase.database, selectedDatabase.container);
                
                (var result, int count) = (await cosmosDbDocumentService.QueryAsync(query, 100));

                Documents.Clear();
                Documents.AddRange(result.ToArray());

            }
            catch (Exception e)
            {
                
            }
        }

        private async Task GetDocumentAsync()
        {
            var doc = await cosmosDbDocumentService.GetDocumentAsync(selectedDocument.Item1,selectedDocument.Item2);
            FullDocuments.Clear();
            FullDocuments.Add(doc);
        }
    }
}
