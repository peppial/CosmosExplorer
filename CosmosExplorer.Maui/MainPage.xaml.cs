using CosmosExplorer.Maui.Services;

namespace CosmosExplorer.Maui;

public partial class MainPage : ContentPage
{
    private readonly StateService stateService;
    int count = 0;


	public MainPage()
	{
		InitializeComponent();
		stateService = new StateService();
		if(stateService.Database !="")
        count = int.Parse(stateService.Database);
    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		stateService.Database = count.ToString();


        SemanticScreenReader.Announce(CounterBtn.Text);
	}
}


