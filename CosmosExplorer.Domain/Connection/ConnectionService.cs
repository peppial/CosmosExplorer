using System.Net;
using System.Threading;
using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Domain.Connection;

public class ConnectionService : IConnectionService, IDisposable
{
    public CosmosClient? cosmosClient { get; private set; }

    public DatabaseProperties? databaseProperties { get; private set; }
    public ContainerProperties? containerProperties { get; private set; }
    public Database? database { get; private set; }
    public Container? container { get; private set; }
    private string? connectionString { get; set; }


    public async Task ChangeContainerAsync(string connectionString, string databaseName, string containerName)
    {
        cosmosClient = new Microsoft.Azure.Cosmos.CosmosClient(connectionString);
        this.connectionString = connectionString;
        database = cosmosClient.GetDatabase(databaseName);
        databaseProperties = await database.ReadAsync();
        container = cosmosClient.GetContainer(databaseName, containerName);
        containerProperties = await container.ReadContainerAsync();

    }
    public async Task<IList<DatabaseModel>> GetDatabasesAsync(string connectionString, CancellationToken cancellationToken)
    {

        if (cosmosClient == null || this.connectionString != connectionString)
        {
            cosmosClient = new Microsoft.Azure.Cosmos.CosmosClient(connectionString);
            this.connectionString = connectionString;
        }
        var databaseProperties = cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
        var databases = new List<DatabaseModel>();

        while (databaseProperties.HasMoreResults)
        {
            var feedResponseDatabase = await databaseProperties.ReadNextAsync(cancellationToken);

            foreach (var databaseProperty in feedResponseDatabase)
            {
                var database = cosmosClient.GetDatabase(databaseProperty.Id);


                DatabaseModel databaseModel = new DatabaseModel { Id = database.Id };

                var containerProperties = database.GetContainerQueryIterator<ContainerProperties>();

                while (containerProperties.HasMoreResults)
                {
                    var feedResponseContainer = await containerProperties.ReadNextAsync(cancellationToken);

                    foreach (var containerProperty in feedResponseContainer)
                    {
                        var container = database.GetContainer(containerProperty.Id);

                        databaseModel.Containers.Add(new ContainerModel { Id = container.Id });

                    }
                }

                containerProperties.Dispose();
                databases.Add(databaseModel);

            }
        }
        databaseProperties.Dispose();
        return databases;
        
    }

    public void Dispose()
    {
        if (cosmosClient != null) cosmosClient.Dispose();
    }
}

