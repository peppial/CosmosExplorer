using CosmosExplorer.Core.Connection;
using CosmosExplorer.Core.Models;
using CosmosExplorer.Infrastructure.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO;

namespace CosmosExplorer.Infrastructure.Connection;

public class CosmosDbConnectionService : IConnectionService, IDisposable
{
    public CosmosClient? cosmosClient { get; private set; }

    public DatabaseProperties? databaseProperties { get; private set; }
    public ContainerProperties? containerProperties { get; private set; }
    public Database? database { get; private set; }
    public Container? container { get; private set; }
    private string? connectionString { get; set; }

    public async Task<string?> ChangeContainerAsync(string connectionString, string databaseName, string containerName)
    {
        cosmosClient = new CosmosClient(connectionString);
        this.connectionString = connectionString;
        try
        { 
            database = cosmosClient.GetDatabase(databaseName);
            databaseProperties = await database.ReadAsync();
            container = cosmosClient.GetContainer(databaseName, containerName);
            containerProperties = await container.ReadContainerAsync();
        }
        catch (Exception e)
        {
            return e.Message;
        }
        return null;

    }
    public async Task<IEnumerable<IDatabaseModel>> GetDatabasesAsync(string connectionString, CancellationToken cancellationToken)
    {
        if (cosmosClient == null || this.connectionString != connectionString )
        {
            cosmosClient = new Microsoft.Azure.Cosmos.CosmosClient(connectionString);
            this.connectionString = connectionString;
        }
        var databaseProperties = cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
        var databases = new List<CosmosDbDatabaseModel>();

        while (databaseProperties.HasMoreResults)
        {
            var feedResponseDatabase = await databaseProperties.ReadNextAsync(cancellationToken);

            int index = 0;
            foreach (var databaseProperty in feedResponseDatabase)
            {
                var database = cosmosClient.GetDatabase(databaseProperty.Id);


                CosmosDbDatabaseModel databaseModel = new() { Id = database.Id, Index = ++index };

                var containerProperties = database.GetContainerQueryIterator<ContainerProperties>();

                while (containerProperties.HasMoreResults)
                {
                    var feedResponseContainer = await containerProperties.ReadNextAsync(cancellationToken);

                    var containers = new List<CosmosDbContainerModel>();
                    foreach (var containerProperty in feedResponseContainer)
                    {
                        var container = database.GetContainer(containerProperty.Id);

                        containers.Add(new CosmosDbContainerModel { Id = container.Id, Index = ++index });

                    }
                    databaseModel.Containers = containers;
                }

                containerProperties.Dispose();
                databases.Add(databaseModel);

            }
        }
        databaseProperties.Dispose();
        return databases;
        
    }

    public Partition Partition 
    {
        get
        {
            if (containerProperties.PartitionKeyPaths.Count > 1)
            {
                string[] paths = containerProperties.PartitionKeyPaths.Select(x => x.Substring(1)).ToArray();
                return new Partition(paths[0], paths.Length > 1 ? paths[1] : null, paths.Length > 2 ? paths[2] : null);
            } 
            
            if (containerProperties.PartitionKeyPath is not null)
            {
                return new Partition(containerProperties.PartitionKeyPath.Substring(1),null, null);
            }

            return null;
        }
    }

    void IDisposable.Dispose()
    {
        cosmosClient?.Dispose();
    }
}

