namespace CosmosExplorer.Domain;

public interface IConnectionService
{
    public Task<string> GetDataAsync();
}

