namespace CosmosExplorer.Core.Models;

public record LastQuery(string Query, string ConnectionString, string Database, string Container)
{
    public static implicit operator LastQuery((string Query, string ConnectionString, string Database, string Container) tuple) => new(tuple.Query, tuple.ConnectionString, tuple.Database, tuple.Container);
}
