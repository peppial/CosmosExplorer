
using System.Collections.Generic;
using CosmosExplorer.Core;
using CosmosExplorer.Core.Models;

public class StateContainer : IStateContainer
{
    public string ConnectionString { get; set; }
    public List<PreferenceConnectionString> ConnectionStrings { get; set; } = [];
    public List<LastQuery> LastQueries { get; set; } = [];
    public string Database { get; set; }
    public string Container { get; set; }
}

