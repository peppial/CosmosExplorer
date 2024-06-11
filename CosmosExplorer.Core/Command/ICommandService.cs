using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core.Command
{
	public interface ICommandService
	{
        Task UpdateDocumentAsync(string id, Partition partition, string documentString);

        Task DeleteDocumentAsync(string id, Partition partition);

    }
}

