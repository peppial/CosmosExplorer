using System;
using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Core.Models
{
    public interface IContainerModel
    {
        string Name { get; set; }
        string Id { get; set; }
        int Index { get; set; }
        string ETag { get; set; }
        string SelfLink { get; set; }
        string PartitionKeyPath { get; set; }
        string? PartitionKeyJsonPath { get;  }
        bool? IsLargePartitionKey { get; }
        int? DefaultTimeToLive { get; set; } 
        string IndexingPolicy { get; set; }
    }
}

