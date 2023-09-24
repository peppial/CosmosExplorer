using System;
using CosmosExplorer.Domain.Models;

namespace CosmosExplorer.Domain.Query
{
	public interface IQueryService
	{
        Task<string> GetDocumentAsync(string partitionName, string id);
        Task<QueryResultModel<IReadOnlyCollection<IDocumentModel>>> QueryAsync(string partitionName, string filter, int maxItems, CancellationToken cancellationToken);


    }
}

