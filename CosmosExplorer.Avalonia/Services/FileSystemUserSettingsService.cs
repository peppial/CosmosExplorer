using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CosmosExplorer.Core;
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
    private IStateContainer stateContainer;
    SemaphoreSlim semaphore = new SemaphoreSlim(initialCount:1);
    
    public async Task<IStateContainer> GetSettingsAsync()
    {
        if (stateContainer is null)
        {
            if (!File.Exists(settingsFilePath))
            {
                await SaveSettingsAsync(new StateContainer());
            }

            await semaphore.WaitAsync();
            try
            {
                using var settingsFileStream = new FileStream(settingsFilePath, FileMode.Open);
                stateContainer = await JsonSerializer.DeserializeAsync<StateContainer>(settingsFileStream);            
            }
            finally
            {
                semaphore.Release();
            }

            if (stateContainer is null)
            {
                throw new InvalidOperationException("Can't deserialize user settings");
            }
        }

        return stateContainer;
    }

    public async Task SaveSettingsAsync(IStateContainer stateContainer)
    {
        if (!Directory.Exists(settingsFolderPath))
        {
            Directory.CreateDirectory(settingsFolderPath);
        }
        await semaphore.WaitAsync();
        try
        {
            using var settingsFileStream = new FileStream(settingsFilePath, FileMode.Create);
            await JsonSerializer.SerializeAsync(settingsFileStream, stateContainer);
        }
        finally
        {
            semaphore.Release();
        }

        this.stateContainer = stateContainer;
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
