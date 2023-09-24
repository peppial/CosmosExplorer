using CosmosExplorer.Domain.Connection;
using Microsoft.Azure.Cosmos;
using CosmosExplorer.Domain.Models;
using CosmosExplorer.Infrastructure.Extensions;
using CosmosExplorer.Domain.Query;

namespace CosmosExplorer.Infrastructure.Query
{
    public class CosmosDbQueryService : IQueryService
    {
        private readonly IConnectionService connectionService;

        public CosmosDbQueryService(IConnectionService connectionService)
        {
            this.connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        }

        public async Task<string> GetDocumentAsync(string partitionName, string id)
        {

            var response = await connectionService.container.ReadItemAsync<dynamic>(id, new PartitionKey(partitionName));

            return response.Resource.ToString();
        }
        public async Task<QueryResultModel<IReadOnlyCollection<IDocumentModel>>> QueryAsync(string partitionName, string filter, int maxItems, CancellationToken cancellationToken)
        {
            filter = filter.RemoveSpecialCharacters();
            var result = new QueryResultModel<IReadOnlyCollection<IDocumentModel>>();
            var containerProperties = await connectionService.container.ReadContainerAsync(null, cancellationToken);

            var token = containerProperties.Resource.PartitionKeyPath;
            if (token != null)
            {
                token = $", c{token.Replace('/', '.')} as _partitionKey, true as _hasPartitionKey";
            }


            var sql = $"SELECT c.id, c._self, c._etag, c._ts, c._attachments {token} FROM c {filter}";

            var options = new QueryRequestOptions
            {
                MaxItemCount = maxItems,
                // TODO: Handle Partition key and other IHaveRequestOptions values
                //PartitionKey = 
            };

            using (var resultSet = connectionService.container.GetItemQueryIterator<DocumentModel>(
                queryText: sql,
                //continuationToken: continuationToken,
                requestOptions: options))
            {
                var response = await resultSet.ReadNextAsync(cancellationToken);

                result.RequestCharge = response.RequestCharge;
                result.ContinuationToken = response.ContinuationToken;
                result.Items = response.Resource.ToArray();
                //result.Headers = response.Headers.ToDictionary();
            }

            return result;
        }
    }
}

