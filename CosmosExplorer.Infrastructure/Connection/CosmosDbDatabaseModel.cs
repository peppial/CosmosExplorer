using System;
using CosmosExplorer.Domain.Models;
using CosmosExplorer.Infrastructure.Connection;
using System.Linq;

namespace CosmosExplorer.Infrastructure.Connection
{
	public record CosmosDbDatabaseModel : IDatabaseModel
	{
        public string Id { get; set; }
        public int Index { get; set; }
        public string? ETag { get; set; }
        public string? SelfLink { get; set; }
        public int? Throughput { get; set; } 
        public bool IsServerless { get; set; }

        public IEnumerable<IContainerModel> Containers { get; set; } = new List<CosmosDbContainerModel>();
	}


}

