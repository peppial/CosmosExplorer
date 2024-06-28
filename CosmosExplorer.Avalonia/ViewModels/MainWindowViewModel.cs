using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmosExplorer.Core;
using CosmosExplorer.Core.Models;
using CosmosExplorer.Core.State;
using DynamicData;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace CosmosExplorer.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IUserSettingsService userSettingsService;
        private readonly ICosmosDBDocumentService cosmosDbDocumentService;
        private IStateContainer stateContainer;
        
        private bool isBusy;
        private string errorMessage;
        private string message;
        private string? connectionString;
        private DocumentViewModel? selectedDocument;
        private DatabaseViewModel? selectedDatabase;
        private string? filter;
        private PreferenceConnectionString selectedConnectionString;
        private string? connectionStringName;
        private string fullDocument;
        private int selectedTabIndex;
        private bool addConnectionString;
        public ICommand LoadSettingsAsyncCommand { get; }
        public ICommand SaveSettingsAsyncCommand { get; }
        public ICommand QueryAsyncCommand { get; }
        public ICommand NewAsyncCommand { get; }
        public ICommand SaveAsyncCommand { get; }

        public ICommand DeleteAsyncCommand { get; }

        public ICommand GetDocumentAsyncCommand { get; }
        public ICommand ChangeConnectionStringCommand { get; }
        
        public ICommand DeleteConnectionStringAsyncCommand { get; }
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
            NewAsyncCommand = ReactiveCommand.CreateFromTask(NewAsync); 
            SaveAsyncCommand = ReactiveCommand.CreateFromTask(SaveAsync); 
            DeleteAsyncCommand = ReactiveCommand.CreateFromTask(DeleteAsync); 
            GetDocumentAsyncCommand = ReactiveCommand.CreateFromTask(GetDocumentAsync);
            ChangeConnectionStringCommand = ReactiveCommand.CreateFromTask(ChangeConnectionStringAsync); 
            DeleteConnectionStringAsyncCommand = ReactiveCommand.CreateFromTask(DeleteConnectionStringAsync); 
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
            set=>this.RaiseAndSetIfChanged(ref errorMessage, value); 
        }
        public string Message
        {
            get => message;
            set =>this.RaiseAndSetIfChanged(ref message, value); 
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
            get => filter;
            set => this.RaiseAndSetIfChanged(ref filter, value);
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
                this.RaisePropertyChanged(nameof(SelectedDatabase)); 
                
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
            var addedConnectionString = new PreferenceConnectionString(connectionStringName, connectionString, true, false);
            stateContainer.ConnectionStrings.Add(addedConnectionString);
            await userSettingsService.SaveSettingsAsync(stateContainer);
            ConnectionStrings.Add(addedConnectionString);
            ConnectionStringName = ConnectionString = "";
            SelectedConnectionString = addedConnectionString;

            SelectedTabIndex = 0;
            AddConnectionString = false;
        }

        private async Task DeleteConnectionStringAsync()
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Delete?", "Are you sure you want to delete the connection string?",
                    ButtonEnum.YesNo);

            var result = await box.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                var index = stateContainer.ConnectionStrings.IndexOf(selectedConnectionString);
                stateContainer.ConnectionStrings.RemoveAt(index);
                await userSettingsService.SaveSettingsAsync(stateContainer);
                ConnectionStringName = ConnectionString = "";
                SelectedTabIndex = 0;
                await LoadSettingsAsync();
            }
        }
        private async Task QueryAsync()
        {
            IsBusy = true;
            ErrorMessage = "";
            Message = "";
            try
            {
                await SetContainerAsync();
                
                (var result, int count, double runits) = (await cosmosDbDocumentService.QueryAsync(filter, 100));
                selectedDocument = null;
                Documents.Clear();
                Documents.AddRange(result.Select(x => new DocumentViewModel(x.Item1, x.Item2)));
                AddLastQuery(filter);
                await userSettingsService.SaveSettingsAsync(stateContainer);
                Message = $"{count} items retrieved, {runits} RUs";

            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;

            }
            IsBusy = false;

        }

        private async Task SetContainerAsync()
        {
            await cosmosDbDocumentService.ChangeContainerAsync(selectedDatabase?.Database, selectedDatabase?.Container);
        }

        private async Task NewAsync()
        {
            IsBusy = true;
            ErrorMessage = "";
            try
            {
                await SetContainerAsync();
                Partition partition = cosmosDbDocumentService.Partition;
                StringBuilder newDoc =
                    new StringBuilder(
                        """
                        {
                             "id": "replace_with_new_document_id",
                        """);
                if (partition is not null)
                {
                    newDoc.Append(
                        $"""
                         
                              "{partition.PartitionName1}": "replace_with_new_partition_key_value"
                         """
                    );

                    if (partition.PartitionName2 is not null)
                    {
                        newDoc.Append(
                            $"""
                             ,
                                  "{partition.PartitionName2}": "replace_with_new_partition_key_value"
                             """
                        );
                    }
                    
                    if (partition.PartitionName3 is not null)
                    {
                        newDoc.Append(
                            $"""
                             ,
                                  "{partition.PartitionName3}": "replace_with_new_partition_key_value"
                             """
                        );
                    }
                    newDoc.Append("""
                                  
                                  }
                                  """);
                }

                FullDocument = newDoc.ToString();
                selectedDocument = null;
                Documents.Clear();
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;

            }

            IsBusy = false;

        }

        private async Task SaveAsync()
        {
            IsBusy = true;
            ErrorMessage = "";
            Message = "";
            try
            {
                await SetContainerAsync();
                
                FullDocument = await cosmosDbDocumentService.UpdateDocumentAsync(selectedDocument?.Id, selectedDocument?.Partition, FullDocument);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;

            }
                
            Message = (selectedDocument is null)?"Document is added.":"Document is updated.";
            IsBusy = false;

        }
        private async Task DeleteAsync()
        {
            
            ErrorMessage = "";
            Message = "";
            try
            {
                //await SetContainerAsync();
                
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Delete?", "Are you sure you want to delete the document?",
                        ButtonEnum.YesNo);

                var result = await box.ShowAsync();
                if (result == ButtonResult.Yes)
                {
                    IsBusy = true;
                    await cosmosDbDocumentService.DeleteDocumentAsync(selectedDocument.Id, selectedDocument.Partition);
                    await QueryAsync();
                    Message = "Document is deleted.";
                }
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
