using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Core.State;

public interface IUserSettingsService
{
    Task<UserSettings> GetSettingsAsync();
    Task SaveSettingsAsync(UserSettings userSettings);
}