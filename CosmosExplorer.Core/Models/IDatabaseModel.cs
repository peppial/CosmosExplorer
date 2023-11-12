namespace CosmosExplorer.Core.Models
{
    public interface IDatabaseModel
    {
        public string Id { get; set; }
        public int Index { get; set; }
        public string? ETag { get; set; }
        public string? SelfLink { get; set; }
        public int? Throughput { get; set; }
        public bool IsServerless { get; set; }

        public IEnumerable<IContainerModel> Containers { get; set; }

    }
}