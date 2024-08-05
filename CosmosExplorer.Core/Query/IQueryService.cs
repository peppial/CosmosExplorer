using System;
using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core.Query
{
	public interface IQueryService
	{
        Task<string> GetDocumentAsync(Partition partition, string id);
        Task<QueryResultModel<IReadOnlyCollection<IDocumentModel>>> FilterAsync(string filter, int maxItems, CancellationToken cancellationToken);
        Task<QueryResultModel<IReadOnlyCollection<object>>> QueryAsync(string query, int maxItems, CancellationToken cancellationToken);

	}
}

