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

	public async Task<IEnumerable<WeatherForecast>> GetForecastAsync(DateTime startDate)
	{
		var databases = await connectionService.GetDatabasesAsync(stateContainer.ConnectionString, new CancellationToken());

		stateContainer.Database = databases[0].Id;
		stateContainer.Container = databases[0].Containers[0].Id;

		var result = new List<WeatherForecast>();
		foreach(var database in databases)
		{
			foreach (var container in database.Containers) result.Add(new WeatherForecast { Container = container.Id, Database = database.Id });
		}
		return result;
	}
    public void SelectContainer(string databaseName, string containerName)
    {
        stateContainer.Database = databaseName;
        stateContainer.Container = containerName;
    
	}
    public async Task<IEnumerable<(string, string)>> QueryAsync(string query)
    {
        var result = await queryService.QueryAsync(stateContainer.ConnectionString, stateContainer.Database, stateContainer.Container, null, query, 100, new CancellationToken());

        return result.Items.Select(i => (i.Id, i.PartitionKey));

    }
    public async Task<string> GetDocumentAsync(string id, string partitionKey)
    {
        return await queryService.GetDocumentAsync(stateContainer.ConnectionString, stateContainer.Database, stateContainer.Container, partitionKey, id);

    }
}


