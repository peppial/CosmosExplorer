using CosmosExplorer.Domain.Connection;
using CosmosExplorer.Domain.Models;
using CosmosExplorer.Domain.Query;
using CosmosExplorer.Infrastructure.Connection;
using CosmosExplorer.Infrastructure.Query;
using CosmosExplorer.Maui.Services;
using Microsoft.Extensions.Logging;

namespace CosmosExplorer.Maui;

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
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<StateService>();
        builder.Services.AddSingleton<IContainerModel, CosmosDbContainerModel>();
        builder.Services.AddSingleton<IDatabaseModel, CosmosDbDatabaseModel>();
        builder.Services.AddSingleton<IConnectionService, CosmosDbConnectionService>();
        builder.Services.AddScoped<IQueryService, CosmosDbQueryService>();
        return builder.Build();
	}
}

