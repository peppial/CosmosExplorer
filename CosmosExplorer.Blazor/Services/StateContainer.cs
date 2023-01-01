using System;
using Newtonsoft.Json;
using Microsoft.Maui.Storage;
namespace CosmosExplorer.Blazor.Services
{
	public class StateContainer
	{

        private const string ConnectionStringName = "ConnectionString";
        private const string ConnectionStringsName = "ConnectionStrings";
        private const string DatabaseName = "Database";
        private const string ContainerName = "Container";

        public string ConnectionString
        {
            get { return Preferences.Default.Get<string>(ConnectionStringName, string.Empty); }
            set { Preferences.Default.Set(ConnectionStringName, value); }
        }

        public List<PreferenceConnectionString> ConnectionStrings
		{
			get
            {
                return  JsonConvert.DeserializeObject<List<PreferenceConnectionString>>(Preferences.Default.Get<string>(ConnectionStringsName, "[]"));
            }
			set { Preferences.Default.Set(ConnectionStringsName, JsonConvert.SerializeObject(value)); }
        }


        public string Database
        {
            get { return Preferences.Default.Get<string>(DatabaseName, string.Empty); }
            set { Preferences.Default.Set(DatabaseName, value); }
        }

        public string Container
        {
            get { return Preferences.Default.Get<string>(ContainerName, string.Empty); }
            set { Preferences.Default.Set(ContainerName, value); }
        }
    }

    public record PreferenceConnectionString
    {
        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public bool Selected { get; set; }
    }
}

