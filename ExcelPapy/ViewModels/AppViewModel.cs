using Windows.UI.Text;

namespace ExcelPapy.ViewModels;

public partial class AppViewModel : ObservableObject
{
    [ObservableProperty]
    private FontFamily _fontApp = new FontFamily("Segoe UI");

    [ObservableProperty]
    private string _WeightApp = "DemiBold";
}
