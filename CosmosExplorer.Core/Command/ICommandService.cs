using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core.Command
{
	public interface ICommandService
	{
        Task UpdateDocumentAsync(string id, string partitionKey, string documentString);

        Task DeleteDocumentAsync(string id, string partitionKey);

    }
}

