using System;
using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Domain
{
	public class Client :IDisposable
	{
		public  CosmosClient? Default {get;set;}

		public CosmosClient OpenConnectionAsync(string connectionString)
        {
            Default = new Microsoft.Azure.Cosmos.CosmosClient(connectionString);
            return Default;
        }

        public void Dispose()
        {
            if (Default is not null)
            {
                Default.Dispose();
            }
        }
    }
}

