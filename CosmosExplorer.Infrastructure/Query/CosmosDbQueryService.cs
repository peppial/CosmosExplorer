using CosmosExplorer.Core.Connection;
using Microsoft.Azure.Cosmos;
using CosmosExplorer.Core.Models;
using CosmosExplorer.Infrastructure.Extensions;
using CosmosExplorer.Core.Query;

namespace CosmosExplorer.Infrastructure.Query
{
    public class CosmosDbQueryService : IQueryService
    {
        private readonly IConnectionService connectionService;

        public CosmosDbQueryService(IConnectionService connectionService)
        {
            this.connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        }

        public async Task<string> GetDocumentAsync(Partition partition, string id)
        {
            if (id is null) return null;

            PartitionKey key;
            if (connectionService.containerProperties.PartitionKeyPaths.Count > 1)
            {
                key = CosmosExtensions.GetPartitionKey(partition);
            }
            else
            {
                key = CosmosExtensions.GetPartitionKey(partition.PartitionName1);
            }
            var response = await connectionService.container.ReadItemAsync<dynamic>(id, key);

            return response.Resource.ToString();
        }
        public async Task<QueryResultModel<IReadOnlyCollection<IDocumentModel>>> QueryAsync(string filter, int maxItems, CancellationToken cancellationToken)
        {
            filter = filter.RemoveSpecialCharacters();
            var result = new QueryResultModel<IReadOnlyCollection<IDocumentModel>>();
            
            Partition partition = connectionService.Partition;
            string token = null;
            bool hasPartition = false;
            
            if (partition.PartitionName1 is not null)
            {
                token += $", c.{partition.PartitionName1} as _partitionKey1";
                hasPartition = true;
            }
            if(partition.PartitionName2 is not null)
            {
                token += $", c.{partition.PartitionName2} as _partitionKey2";
            }
            if(partition.PartitionName3 is not null)
            {
                token += $", c.{partition.PartitionName3} as _partitionKey3";
            }
            
            if(hasPartition)  token += ", true as _hasPartitionKey";

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

