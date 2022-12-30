using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Domain;

public class ConnectionService : IConnectionService
{
    public async Task<string> GetDataAsync()
    {
        CosmosClient cosmosClient = new CosmosClient(
            "",
            "");

        Database db = cosmosClient.GetDatabase("hometraining");
        Container container =  db.GetContainer("HomeTrainingContext");
        return container.Id;
    }
}

