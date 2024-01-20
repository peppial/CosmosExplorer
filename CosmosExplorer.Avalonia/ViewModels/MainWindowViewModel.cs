using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmosExplorer.Core;
using CosmosExplorer.Core.Models;
using CosmosExplorer.Core.State;
using DynamicData;

namespace CosmosExplorer.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IUserSettingsService userSettingsService;
        private readonly ICosmosDBDocumentService cosmosDbDocumentService;
        private IStateContainer stateContainer;
        
        private bool isBusy;
        private string errorMessage;
        private string? connectionString;
        private DocumentViewModel? selectedDocument;
        private DatabaseViewModel? selectedDatabase;
        private string? query;
        private PreferenceConnectionString selectedConnectionString;
        private string? connectionStringName;
        private string fullDocument;
        private int selectedTabIndex;
        private bool addConnectionString;
        public ICommand LoadSettingsAsyncCommand { get; }
        public ICommand SaveSettingsAsyncCommand { get; }
        public ICommand QueryAsyncCommand { get; }
        public ICommand GetDocumentAsyncCommand { get; }
        public ICommand ChangeConnectionStringCommand { get; }
        public ICommand ReLoadAsyncCommand { get; }
        
        public ObservableCollection<DocumentViewModel> Documents { get; set; } = [];
        public ObservableCollection<DatabaseViewModel> Databases { get; set; } = []; 
        public ObservableCollection<PreferenceConnectionString> ConnectionStrings { get; set; } = [];
        public ObservableCollection<string> LastQueries { get; set; } = [];

        public MainWindowViewModel(IUserSettingsService userSettingsService, ICosmosDBDocumentService cosmosDbDocumentService,IStateContainer stateContainer )
        {
            this.userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
            this.cosmosDbDocumentService = cosmosDbDocumentService ?? throw new ArgumentNullException(nameof(cosmosDbDocumentService));
            this.stateContainer = stateContainer ?? throw new ArgumentNullException(nameof(stateContainer));
            
            LoadSettingsAsyncCommand = ReactiveCommand.CreateFromTask(LoadSettingsAsync);
            SaveSettingsAsyncCommand = ReactiveCommand.CreateFromTask(SaveSettingsAsync);
            QueryAsyncCommand = ReactiveCommand.CreateFromTask(QueryAsync); 
            GetDocumentAsyncCommand = ReactiveCommand.CreateFromTask(GetDocumentAsync);
            ChangeConnectionStringCommand = ReactiveCommand.CreateFromTask(ChangeConnectionStringAsync); 
            ReLoadAsyncCommand = ReactiveCommand.CreateFromTask(ReloadAsync); 
            LoadSettingsAsyncCommand.Execute(null);
        }
        public bool IsBusy
        {
            get => isBusy;
            set => this.RaiseAndSetIfChanged(ref isBusy, value);
        }
        
        public string ErrorMessage
        {
            get => errorMessage;
            set => this.RaiseAndSetIfChanged(ref errorMessage, value);
        }
        public string? ConnectionString
        {
            get => connectionString;
            set => this.RaiseAndSetIfChanged(ref connectionString, value);
        }
        
        public string? ConnectionStringName
        {
            get => connectionStringName;
            set => this.RaiseAndSetIfChanged(ref connectionStringName, value);
        }
        public string? FullDocument
        {
            get => fullDocument;
            set => this.RaiseAndSetIfChanged(ref fullDocument, value);
        }
        
        public string? Query
        {
            get => query;
            set => this.RaiseAndSetIfChanged(ref query, value);
        }
        public DocumentViewModel? SelectedDocument
        {
            get => selectedDocument;
            set
            {
                if (selectedDocument == value)
                    return;
                selectedDocument = value;
                
                GetDocumentAsyncCommand.Execute(null);
            }
        }
        public DatabaseViewModel? SelectedDatabase
        {
            get => selectedDatabase;
            set
            {
                if (selectedDatabase == value)
                    return;
                selectedDatabase = value;
                Documents.Clear();
                FullDocument = "";
                ReloadLastQueries();
                this.RaiseAndSetIfChanged(ref selectedDatabase, value);
            }
        }
        public PreferenceConnectionString SelectedConnectionString
        {
            get => selectedConnectionString;
            set
            {
                if (selectedConnectionString == value || value is null)
                    return;
                selectedConnectionString = value;
                ChangeConnectionStringCommand.Execute(null);
            }
        }
        public int SelectedTabIndex
        {
            get => selectedTabIndex;
            set => this.RaiseAndSetIfChanged(ref selectedTabIndex, value);
        }
        public bool AddConnectionString
        {
            get => addConnectionString;
            set => this.RaiseAndSetIfChanged(ref addConnectionString, value);
        }
        
        private async Task LoadSettingsAsync()
        {
            var loaded = await userSettingsService.GetSettingsAsync();
            stateContainer.ConnectionStrings = loaded.ConnectionStrings;
            stateContainer.ConnectionString = loaded.ConnectionString;
            AddConnectionString = string.IsNullOrEmpty(stateContainer.ConnectionString);
            if(AddConnectionString) SelectedTabIndex = 1;
            
            stateContainer.LastQueries = loaded.LastQueries;
            SelectedConnectionString = loaded.ConnectionStrings.FirstOrDefault(c => c.Selected);

            ConnectionStrings = new (this.stateContainer.ConnectionStrings);
            await ReloadAsync();
        }

        private async Task ReloadAsync()
        {
            IsBusy = true;
            ErrorMessage = "";

            Databases.Clear();
            try
            {
                var databases = await cosmosDbDocumentService.GetDatabasesAsync();
             
                foreach (var database in databases.OrderBy(x => x.Id))
                {

                    foreach (var container in database.Containers.OrderBy(x => x.Id))
                    {
                        Databases.Add((database.Id, container.Id));
                    }
                }
                if(Databases.Count>0) SelectedDatabase = Databases[0];
                ReloadLastQueries();
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;

            }
            IsBusy = false;

            
        }

        private async Task SaveSettingsAsync()
        {
            var addedConnectionString = new PreferenceConnectionString(connectionStringName, connectionString, true);
            stateContainer.ConnectionStrings.Add(addedConnectionString);
            await userSettingsService.SaveSettingsAsync(stateContainer);
            ConnectionStrings.Add(addedConnectionString);
            ConnectionStringName = ConnectionString = "";
            SelectedConnectionString = addedConnectionString;

            SelectedTabIndex = 0;
            AddConnectionString = false;
        }
        
        private async Task QueryAsync()
        {
            IsBusy = true;
            ErrorMessage = "";
            try
            {
                await cosmosDbDocumentService.ChangeContainerAsync(selectedDatabase?.Database, selectedDatabase?.Container);
                
                (var result, int count) = (await cosmosDbDocumentService.QueryAsync(query, 100));
                selectedDocument = null;
                Documents.Clear();
                Documents.AddRange(result.Select(x => new DocumentViewModel(x.Item1, x.Item2)));
                AddLastQuery(query);
                await userSettingsService.SaveSettingsAsync(stateContainer);
                
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;

            }
            IsBusy = false;

        }

        
        private void AddLastQuery(string query)
        {
            if (string.IsNullOrEmpty(query)) return;
            var list = stateContainer.LastQueries;
            if (list.Count > 0 && list[0].Query == query) return;
            var item = new LastQuery(query,stateContainer.ConnectionString, stateContainer.Database, stateContainer.Container);
            var itemToRemove = list.FirstOrDefault(q => q.Query == query && q.Database == stateContainer.Database
                                                                 && q.Container == stateContainer.Container &&
                                                                 q.ConnectionString == stateContainer.ConnectionString);
            if (itemToRemove is not null) list.Remove(itemToRemove);
            
            list.Insert(0, item);
            if (list.Count > 10) list.RemoveAt(10);
            stateContainer.LastQueries = list;
            ReloadLastQueries();
        }

        private async Task GetDocumentAsync()
        {
            FullDocument = await cosmosDbDocumentService.GetDocumentAsync(selectedDocument?.Id,selectedDocument?.Partition);
        }

        private async Task ChangeConnectionStringAsync()
        {
            if (selectedConnectionString is null) return;

            stateContainer.ConnectionString = selectedConnectionString.ConnectionString;

            SetSelectedConnectionString();
            Documents.Clear();
            FullDocument = "";

            await userSettingsService.SaveSettingsAsync(stateContainer);
            await ReloadAsync();
        }

        private void SetSelectedConnectionString()
        {
            for (int i = 0; i < stateContainer.ConnectionStrings.Count; i++)
            {
                stateContainer.ConnectionStrings[i] = stateContainer.ConnectionStrings[i] with
                {
                    Selected =
                    selectedConnectionString.ConnectionString==stateContainer.ConnectionStrings[i].ConnectionString
                };
            }
        }

        private void ReloadLastQueries()
        {
            if(selectedDatabase is null) return;
            LastQueries.Clear();
            LastQueries.Add(stateContainer.LastQueries.Where(c =>
                c.Database == selectedDatabase.Database
                && c.Container == selectedDatabase.Container
                && c.ConnectionString == SelectedConnectionString.ConnectionString
            ).Select(x => x.Query).ToArray()); 
        }
    }
}
