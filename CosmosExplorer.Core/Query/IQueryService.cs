using System;
using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core.Query
{
	public interface IQueryService
	{
        Task<string> GetDocumentAsync(string partitionName, string id);
        Task<QueryResultModel<IReadOnlyCollection<IDocumentModel>>> QueryAsync(string partitionName, string filter, int maxItems, CancellationToken cancellationToken);


    }
}

