using System;
namespace CosmosExplorer.Domain.Query
{
	public interface IQueryService
	{
        public Task<string> GetDocumentAsync(string connectionString, string databaseName, string containerName, string partitionName, string id);
        public Task<QueryResultModel<IReadOnlyCollection<IDocumentModel>>> QueryAsync(string connectionString, string databaseName, string containerName, string partitionName, string filter, int maxItems, CancellationToken cancellationToken);


    }
}

