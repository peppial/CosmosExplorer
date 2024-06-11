using System;
using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core.Query
{
	public interface IQueryService
	{
        Task<string> GetDocumentAsync(Partition partition, string id);
        Task<QueryResultModel<IReadOnlyCollection<IDocumentModel>>> QueryAsync(string filter, int maxItems, CancellationToken cancellationToken);

	}
}

