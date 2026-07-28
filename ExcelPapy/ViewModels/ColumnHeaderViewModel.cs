namespace ExcelPapy.ViewModels;

public partial class ColumnHeaderViewModel : ObservableObject
{
    [ObservableProperty]
    private double _width = 200;

    public string Label { get; init; } = string.Empty;

    [ObservableProperty]
    private FontFamily _fontFamily = new FontFamily("Segoe UI");

    [ObservableProperty]
    private string _fontWeight = "ExtraBold";
}
