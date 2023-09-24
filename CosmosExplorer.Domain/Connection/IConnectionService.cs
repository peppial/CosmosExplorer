using CosmosExplorer.Domain.Models;
using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Domain.Connection;

public interface IConnectionService
{
    DatabaseProperties? databaseProperties { get; }
    ContainerProperties? containerProperties { get; }
    Database? database { get; }
    Container? container { get; }

    Task<string> ChangeContainerAsync(string connectionString, string databaseName, string containerName);
    Task<IEnumerable<IDatabaseModel>> GetDatabasesAsync(string connectionString, CancellationToken cancellationToken);

}

