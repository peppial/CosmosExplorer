using System;
namespace CosmosExplorer.Blazor.Shared
{
    public class TreeItemData
    {
        public int Index { get; set; }

        public string Id { get; set; }

        public string Container { get; set; }

        public string Database { get; set; }

        public bool CanExpand { get; set; } = false;

        public int ParentId { get; set; }

        public bool Expanded { get; set; }

    }
}

