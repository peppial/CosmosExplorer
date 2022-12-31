using System.Net;
using System.Threading;
using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Domain.Connection;

public class ConnectionService : IConnectionService
{
    private readonly Client client;

    public ConnectionService(Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }
    public async Task<IList<DatabaseModel>> GetDatabasesAsync(string connectionString, CancellationToken cancellationToken)
    {
        client.OpenConnectionAsync(connectionString);
        var databaseProperties = client.Default!.GetDatabaseQueryIterator<DatabaseProperties>();
        var databases = new List<DatabaseModel>();

        while (databaseProperties.HasMoreResults)
        {
            var feedResponseDatabase = await databaseProperties.ReadNextAsync(cancellationToken);

            foreach (var databaseProperty in feedResponseDatabase)
            {
                var database = client.Default.GetDatabase(databaseProperty.Id);


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

}

