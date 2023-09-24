using System;
using CosmosExplorer.Domain.Models;
using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Infrastructure.Connection
{
	public class CosmosDbContainerModel : IContainerModel
	{
		public string Name { get; set; }
        public string Id { get; set; }
        public int Index { get; set; }
        public string ETag { get; set; }
        public string SelfLink { get; set; }
        public string PartitionKeyPath { get; set; }
        public string? PartitionKeyJsonPath => string.IsNullOrEmpty(PartitionKeyPath) ? null : PartitionKeyPath.Replace('/', '.');
        public bool? IsLargePartitionKey => PartitionKeyDefVersion > PartitionKeyDefinitionVersion.V1;
        public int? DefaultTimeToLive { get; set; } // null = off, -1 = Default
        public PartitionKeyDefinitionVersion? PartitionKeyDefVersion { get; set; }
        public string IndexingPolicy { get; set; }
        //public CosmosGeospatialType GeospatialType { get; set; }
    }
}

