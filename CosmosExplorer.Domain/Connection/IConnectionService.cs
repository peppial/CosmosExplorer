namespace CosmosExplorer.Domain.Connection;

public interface IConnectionService
{
    public Task<IList<DatabaseModel>> GetDatabasesAsync(string connectionString, CancellationToken cancellationToken);

}

