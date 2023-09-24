using CosmosExplorer.Maui.Services;
using CosmosExplorer.Maui.Models;
using System.Linq;

namespace CosmosExplorer.Maui.Views;

public partial class ConnectionStrings : ContentPage
{
    private StateService stateService;
    public ConnectionStrings()
    {
        InitializeComponent();

        stateService = new StateService();
        
        
        BindingContext = (ConnectionStringsModel) stateService.ConnectionStrings;
    }

    async void connectionStrings_SelectionChanged(System.Object sender, Microsoft.Maui.Controls.SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count != 0)
        {
            var list = stateService.ConnectionStrings;
            foreach (var item in list.ConnectionStrings) item.Selected = false;
            var selectedItem = list.ConnectionStrings.FirstOrDefault(x => x.Name == ((PreferenceConnectionString)e.CurrentSelection).Name);
            selectedItem.Selected = true;
            stateService.ConnectionStrings = list;
        }
    }
}
