using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core;

public interface ICosmosDBDocumentService
{
    Task<IEnumerable<IDatabaseModel>> GetDatabasesAsync();
    Task<string> ChangeContainerAsync(string databaseName, string containerName);
    Task<(IEnumerable<(string, string)>, int)> QueryAsync(string query, int count);
    Task<string> GetDocumentAsync(string id, string partitionKey);
    Task UpdateDocumentAsync(string id, string partitionKey, string documentString);
    Task DeleteDocumentAsync(string id, string partitionKey);
}