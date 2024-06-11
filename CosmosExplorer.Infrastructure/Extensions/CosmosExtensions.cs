using CosmosExplorer.Core.Models;
using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Infrastructure.Extensions;

public class CosmosExtensions
{
 
    public static PartitionKey GetPartitionKey(string partitionName) => new (partitionName);
    public static PartitionKey GetPartitionKey(Partition partition)
    {
        PartitionKeyBuilder builder = new();
        
        builder.Add(partition.PartitionName1);
        builder.Add(partition.PartitionName2);
        builder.Add(partition.PartitionName3);

        return builder.Build();
    }
}