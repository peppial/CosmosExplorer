using CosmosExplorer.Core;
using CosmosExplorer.Core.Command;
using CosmosExplorer.Core.Query;
using CosmosExplorer.Core.Connection;
using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Domain;

public class CosmosDBDocumentService : ICosmosDBDocumentService
{
    private readonly IConnectionService connectionService;
    private readonly IQueryService queryService;
    private readonly ICommandService commandService;
    private readonly IStateContainer stateContainer;

    public CosmosDBDocumentService(IStateContainer stateContainer, IConnectionService connectionService,
        IQueryService queryService, ICommandService commandService)
    {
        this.connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        this.queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        this.commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        this.stateContainer = stateContainer ?? throw new ArgumentNullException(nameof(stateContainer));
    }

    public async Task<IEnumerable<IDatabaseModel>> GetDatabasesAsync()
    {
        if (string.IsNullOrEmpty(stateContainer.ConnectionString))
        {
            return new List<IDatabaseModel>();
        }

        return await connectionService.GetDatabasesAsync(stateContainer.ConnectionString, new CancellationToken());
    }

    public async Task<string> ChangeContainerAsync(string databaseName, string containerName)
    {
        stateContainer.Database = databaseName;
        stateContainer.Container = containerName;
        return await connectionService.ChangeContainerAsync(stateContainer.ConnectionString, databaseName,
            containerName);
    }

    public async Task<(IEnumerable<(string, Partition)>, int, double)> QueryAsync(string query, int count)
    {
        var result = await queryService.QueryAsync(query, count, new CancellationToken());

        return (result.Items.Select(i => (i.Id, i.Partition)), result.Items.Count(), result.RequestCharge);
    }

    public async Task<string> GetDocumentAsync(string id, Partition partition)
    {
        return await queryService.GetDocumentAsync(partition, id);
    }

    public async Task UpdateDocumentAsync(string id, Partition partition, string documentString)
    {
        await commandService.UpdateDocumentAsync(id, partition, documentString);
    }

    public async Task DeleteDocumentAsync(string id, Partition partition)
    {
        await commandService.DeleteDocumentAsync(id, partition);
    }
}