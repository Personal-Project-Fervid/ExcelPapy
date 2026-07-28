namespace ExcelPapy.ViewModels;

public partial class AppViewModel : ObservableObject
{
    [ObservableProperty]
    private FontFamily _fontApp = new FontFamily("Segoe UI");
}
