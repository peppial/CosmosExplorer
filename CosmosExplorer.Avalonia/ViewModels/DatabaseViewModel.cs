namespace CosmosExplorer.Avalonia.ViewModels;

public record DatabaseViewModel(string Database, string Container)
{
    public static implicit operator DatabaseViewModel((string Database, string Container) tuple) => new(tuple.Database, tuple.Container);
    public override string ToString()
    {
        return $"{Database} -> {Container}";
    }
}