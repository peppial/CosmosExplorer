using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core;

public interface ICosmosDBDocumentService
{
    Task<IEnumerable<IDatabaseModel>> GetDatabasesAsync();
    Task<string> ChangeContainerAsync(string databaseName, string containerName);
    Task<(IEnumerable<(string, Partition)>, int, double)> QueryAsync(string query, int count);
    Task<string> GetDocumentAsync(string id, Partition partition);
    Task UpdateDocumentAsync(string id, Partition partition, string documentString);
    Task DeleteDocumentAsync(string id, Partition partition);
}