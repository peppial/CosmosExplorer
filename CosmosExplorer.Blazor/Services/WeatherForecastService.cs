using CosmosExplorer.Domain.Connection;
using CosmosExplorer.Domain.Query;

namespace CosmosExplorer.Blazor.Services;

public class WeatherForecastService
{
	private static readonly string[] Summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};
    private readonly IConnectionService connectionService;
    private readonly IQueryService queryService;
    private readonly StateContainer stateContainer;

    public WeatherForecastService(StateContainer stateContainer, IConnectionService connectionService, IQueryService queryService)
	{
        this.connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        this.queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        this.stateContainer = stateContainer ?? throw new ArgumentNullException(nameof(stateContainer));

    }

	public async Task<IEnumerable<DatabaseModel>> GetDatabasesAsync()
	{
        stateContainer.Database = "";
        stateContainer.Container = "";

		return await connectionService.GetDatabasesAsync(stateContainer.ConnectionString, new CancellationToken());

	}
    public async Task ChangeContainerAsync(string databaseName, string containerName)
    {
        stateContainer.Database = databaseName;
        stateContainer.Container = containerName;
        await connectionService.ChangeContainerAsync(stateContainer.ConnectionString,databaseName, containerName);  
	}
    public async Task<IEnumerable<(string, string)>> QueryAsync(string query)
    {
        var result = await queryService.QueryAsync(null, query, 100, new CancellationToken());

        return result.Items.Select(i => (i.Id, i.PartitionKey));

    }
    public async Task<string> GetDocumentAsync(string id, string partitionKey)
    {
        return await queryService.GetDocumentAsync(partitionKey, id);

    }
}


