﻿using System;
using CosmosExplorer.Maui.Models;
using Newtonsoft.Json;

namespace CosmosExplorer.Maui.Services
{
    public class StateService
    {

        private const string ConnectionStringName = "ConnectionString";

        private const string DatabaseName = "Database";
        private const string ContainerName = "Container";

        //preferences
        private const string ConnectionStringsName = "ConnectionStrings";
        private const string LastQueriesName = "LastQueries";

        public string ConnectionString
        {
            get { return Preferences.Default.Get<string>(ConnectionStringName, string.Empty); }
            set { Preferences.Default.Set(ConnectionStringName, value); }
        }

        public  ConnectionStringsModel ConnectionStrings
        {
            get
            {
                return new ConnectionStringsModel(JsonConvert.DeserializeObject<List<PreferenceConnectionString>>(Preferences.Default.Get<string>(ConnectionStringsName, "[]")));
            }
            set { Preferences.Default.Set(ConnectionStringsName, JsonConvert.SerializeObject(value.ConnectionStrings)); }
        }

        public List<(string query, string connectionString, string database, string container)> LastQueries
        {
            get
            {
                //List<(string, string, string)> values = new List<(string, string, string)>();
                //Preferences.Default.Set(LastQueriesName, JsonConvert.SerializeObject(values));
                return JsonConvert.DeserializeObject<List<(string, string, string, string)>>(Preferences.Default.Get<string>(LastQueriesName, "[]"));
            }
            set { Preferences.Default.Set(LastQueriesName, JsonConvert.SerializeObject(value)); }
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

