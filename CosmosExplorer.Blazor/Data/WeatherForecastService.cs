using CosmosExplorer.Domain;
namespace CosmosExplorer.Blazor.Data;

public class WeatherForecastService
{
	private static readonly string[] Summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};
    private readonly IConnectionService connectionService;

    public WeatherForecastService(IConnectionService connectionService)
	{
        this.connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
    }

	public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
	{
		var a = await connectionService.GetDataAsync();

        return await Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
		{
			Date = startDate.AddDays(index),
			TemperatureC = Random.Shared.Next(-20, 55),
			Summary = a
        }).ToArray());
	}
}


