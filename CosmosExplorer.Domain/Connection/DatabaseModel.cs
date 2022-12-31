using System;
namespace CosmosExplorer.Domain.Connection
{
	public record DatabaseModel
	{
        public string Id { get; set; }
        public string? ETag { get; set; }
        public string? SelfLink { get; set; }
        public int? Throughput { get; set; } 
        public bool IsServerless { get; set; }

        public IList<ContainerModel> Containers { get; set; } = new List<ContainerModel>();
	}


}

