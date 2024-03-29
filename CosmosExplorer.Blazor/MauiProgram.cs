﻿using Microsoft.Extensions.Logging;
using CosmosExplorer.Blazor.Services;
using CosmosExplorer.Core;
using CosmosExplorer.Core.Connection;
using CosmosExplorer.Core.Query;
using DotNet.Meteor.HotReload.Plugin;
using CosmosExplorer.Infrastructure.Query;
using CosmosExplorer.Core.Models;
using CosmosExplorer.Infrastructure.Connection;
using CosmosExplorer.Core.Command;
using CosmosExplorer.Domain;
using CosmosExplorer.Infrastructure.Command;

namespace CosmosExplorer.Blazor;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
		builder.EnableHotReload();
#endif

        builder.Services.AddSingleton<ICosmosDBDocumentService, CosmosDBDocumentService>();
        builder.Services.AddSingleton<IStateContainer, StateContainer>();
        builder.Services.AddSingleton<IContainerModel, CosmosDbContainerModel>();
        builder.Services.AddSingleton<IDatabaseModel, CosmosDbDatabaseModel>();
        builder.Services.AddSingleton<IConnectionService,CosmosDbConnectionService>();
        builder.Services.AddScoped<IQueryService, CosmosDbQueryService>();
        builder.Services.AddScoped<ICommandService, CosmosDbCommandService>();
        return builder.Build();
	}
}

