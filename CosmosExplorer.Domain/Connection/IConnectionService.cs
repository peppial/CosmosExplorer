using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Domain.Connection;

public interface IConnectionService
{
    Task ChangeContainerAsync(string connectionString, string databaseName, string containerName);
    Task<IList<DatabaseModel>> GetDatabasesAsync(string connectionString, CancellationToken cancellationToken);

    DatabaseProperties? databaseProperties { get; }
    ContainerProperties? containerProperties { get; }
    Database? database { get; }
    Container? container { get; }

}

