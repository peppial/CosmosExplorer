using CosmosExplorer.Domain.Models;

namespace CosmosExplorer.Domain.Command
{
	public interface ICommandService
	{
        Task UpdateDocumentAsync(string id, string partitionKey, string documentString);

        Task DeleteDocumentAsync(string id, string partitionKey);

    }
}

