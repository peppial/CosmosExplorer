using System;
using CosmosExplorer.Maui.Services;

namespace CosmosExplorer.Maui.Models
{
	public class ConnectionStringsModel
	{
		public ConnectionStringsModel(List<PreferenceConnectionString> connectionStrings)
		{
			this.ConnectionStrings = connectionStrings;
		}
		public List<PreferenceConnectionString> ConnectionStrings { get; set; } = new List<PreferenceConnectionString>();
    }
}

