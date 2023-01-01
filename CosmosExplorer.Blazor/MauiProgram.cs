using Microsoft.Extensions.Logging;
using CosmosExplorer.Blazor.Services;
using CosmosExplorer.Domain;
using CosmosExplorer.Domain.Connection;
using CosmosExplorer.Domain.Query;

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
#endif

		builder.Services.AddSingleton<WeatherForecastService>();

        builder.Services.AddSingleton<StateContainer>();
        builder.Services.AddSingleton<IConnectionService,ConnectionService>();
        builder.Services.AddScoped<IQueryService, QueryService>();
        return builder.Build();
	}
}

