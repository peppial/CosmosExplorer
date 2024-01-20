namespace CosmosExplorer.Avalonia.ViewModels;

public record DocumentViewModel(string Id, string Partition)
{
    public static implicit operator DocumentViewModel((string Id, string Partition) tuple) => new(tuple.Id, tuple.Partition);
    public override string ToString()
    {
        return $"{Id} / {Partition}";
    }
}