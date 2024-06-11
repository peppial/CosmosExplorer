namespace CosmosExplorer.Core.Models;

public record Partition(string? PartitionName1, string? PartitionName2, string? PartitionName3)
{
    public override string ToString()
    {
        if (string.IsNullOrEmpty(PartitionName2)) return PartitionName1;
        
        if (string.IsNullOrEmpty(PartitionName3)) return $"{PartitionName1}/{PartitionName2}";
        
        return $"{PartitionName1}/{PartitionName2}/{PartitionName3}";
    }
}