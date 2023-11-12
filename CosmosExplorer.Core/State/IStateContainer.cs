using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core;

public interface IStateContainer
{
    string ConnectionString { get; set; }
    List<PreferenceConnectionString> ConnectionStrings { get; set; }
    List<(string query, string connectionString, string database, string container)> LastQueries { get; set; }
    string Database { get; set; }
    string Container { get; set; }
}