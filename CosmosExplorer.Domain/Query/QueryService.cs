using System;
using System.Dynamic;
using System.Net;
using System.Reflection;
using CosmosExplorer.Domain.Connection;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CosmosExplorer.Domain.Query
{
    public class QueryService : IQueryService
    {
        private readonly Client client;

        public QueryService(Client client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string> GetDocumentAsync(string connectionString, string databaseName, string containerName, string partitionName, string id)
        {
            client.OpenConnectionAsync(connectionString);
            Container container = client.Default!.GetContainer(databaseName, containerName);
            //temp
            var containerProperties = await container.ReadContainerAsync();

            var response = await container.ReadItemAsync<dynamic>(id, new PartitionKey(partitionName));

            return response.Resource.ToString();
        }
        public async Task<QueryResultModel<IReadOnlyCollection<IDocumentModel>>> QueryAsync(string connectionString, string databaseName, string containerName, string partitionName, string filter, int maxItems, CancellationToken cancellationToken)
        {
            client.OpenConnectionAsync(connectionString);

            Container container = client.Default!.GetContainer(databaseName, containerName);

            var result = new QueryResultModel<IReadOnlyCollection<IDocumentModel>>();
            var containerProperties = await container.ReadContainerAsync();

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

            using (var resultSet = container.GetItemQueryIterator<DocumentModel>(
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

