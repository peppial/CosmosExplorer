using CosmosExplorer.Core.Connection;
using Microsoft.Azure.Cosmos;
using CosmosExplorer.Infrastructure.Extensions;
using CosmosExplorer.Core.Models;
using CosmosExplorer.Core.Command;
using Microsoft.Azure.Documents.Client;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosExplorer.Infrastructure.Command
{
    public class CosmosDbCommandService : ICommandService
    {
        private readonly IConnectionService connectionService;

        public CosmosDbCommandService(IConnectionService connectionService)
        {
            this.connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        }
        public async Task<string> UpdateDocumentAsync(string id, Partition partition, string documentString)
        {
            if (connectionService.container is null) throw new ArgumentException(nameof(connectionService.container));

            Container container = connectionService.container;
            ItemResponse <JObject> response = null;
            if (id == null)
            {
                response = await container.CreateItemAsync(JsonConvert.DeserializeObject<dynamic>(documentString));
            }
            else
            {
                response = await container.ReplaceItemAsync(JsonConvert.DeserializeObject<dynamic>(documentString), id, CosmosExtensions.GetPartitionKeyNullable(partition));
            }

            return response.Resource?.ToString();
        }
        
        public async Task DeleteDocumentAsync(string id, Partition partition)
        {
            if (connectionService.container is null) throw new ArgumentException(nameof(connectionService.container));

            Container container = connectionService.container;
          
            await container.DeleteItemAsync<dynamic>(id, CosmosExtensions.GetPartitionKeyNullable(partition));
        }
    }
}