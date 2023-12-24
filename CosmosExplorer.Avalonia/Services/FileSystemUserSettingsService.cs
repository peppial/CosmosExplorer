using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CosmosExplorer.Core.Models;
using CosmosExplorer.Core.State;
using Microsoft.Azure.Cosmos;

namespace CosmosExplorer.Avalonia.Services;

public class FileSystemUserSettingsService : IUserSettingsService
{
    private const string AppName = "CosmosExplorer";
    private const string SettingsFileName = "userSettings.json";

    private readonly string settingsFolderPath = GetSettingsFolderPath();
    private readonly string settingsFilePath = GetSettingsFilePath();
    private UserSettings userSettings;
    
    public async Task<UserSettings> GetSettingsAsync()
    {
        if (userSettings is null)
        {
            if (!File.Exists(settingsFilePath))
            {
                await SaveSettingsAsync(new UserSettings());
            }

            using var settingsFileStream = new FileStream(settingsFilePath, FileMode.Open);
            userSettings = await JsonSerializer.DeserializeAsync<UserSettings>(settingsFileStream);

            if (userSettings is null)
            {
                throw new InvalidOperationException("Can't deserialize user settings");
            }
        }

        return userSettings;
    }

    public async Task SaveSettingsAsync(UserSettings userSettings)
    {
        if (!Directory.Exists(settingsFolderPath))
        {
            Directory.CreateDirectory(settingsFolderPath);
        }

        using var settingsFileStream = new FileStream(settingsFilePath, FileMode.OpenOrCreate);
        await JsonSerializer.SerializeAsync(settingsFileStream, userSettings);

        this.userSettings = userSettings;
    }

    private static string GetSettingsFolderPath()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appDataFolder, AppName);
    }

    private static string GetSettingsFilePath()
    {
        var settingsFolder = GetSettingsFolderPath();
        return Path.Combine(settingsFolder, SettingsFileName);
    }
}
