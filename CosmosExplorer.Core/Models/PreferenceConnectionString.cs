namespace CosmosExplorer.Core.Models;

public record PreferenceConnectionString
{
    public string Name { get; set; }

    public string ConnectionString { get; set; }

    public bool Selected { get; set; }
}