namespace CosmosExplorer.Avalonia.ViewModels;

public record DocumentViewModel(string Id, Core.Models.Partition Partition)
{
    public static implicit operator DocumentViewModel((string Id, Core.Models.Partition Partition) tuple) => new(tuple.Id, tuple.Partition);
    public override string ToString()
    {
        return $"{Id} / {Partition}";
    }
}