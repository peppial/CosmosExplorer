using CosmosExplorer.Domain.Connection;
using Microsoft.Azure.Cosmos;
using CosmosExplorer.Infrastructure.Extensions;
using CosmosExplorer.Domain.Models;
using CosmosExplorer.Domain.Command;
using Microsoft.Azure.Documents.Client;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;

namespace CosmosExplorer.Infrastructure.Command
{
    public class CosmosDbCommandService : ICommandService
    {
        private readonly IConnectionService connectionService;

        public CosmosDbCommandService(IConnectionService connectionService)
        {
            this.connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        }

        public async Task DeleteDocumentAsync(string id, string partitionKey)
        {
            if (connectionService.container is null) throw new ArgumentException(nameof(connectionService.container));

            Container container = connectionService.container;
          
            await container.DeleteItemAsync<dynamic>(id, new PartitionKey(partitionKey));
         }

        public async Task UpdateDocumentAsync(string id, string partitionKey, string documentString)
        {
            if (connectionService.container is null) throw new ArgumentException(nameof(connectionService.container));

            Container container = connectionService.container;

            if (id == null)
            {
                await container.CreateItemAsync(JsonConvert.DeserializeObject<dynamic>(documentString));
            }
            else
            {
                await container.ReplaceItemAsync(JsonConvert.DeserializeObject<dynamic>(documentString), id, new PartitionKey(partitionKey));
            }
           
        }
    }
}