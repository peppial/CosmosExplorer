using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core.State;

public interface IUserSettingsService
{
    Task<IStateContainer> GetSettingsAsync();
    Task SaveSettingsAsync(IStateContainer stateContainer);
}