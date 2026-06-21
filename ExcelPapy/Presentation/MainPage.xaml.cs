using ExcelPapy.Objects;
using ExcelPapy.ViewModels;

namespace ExcelPapy.Presentation;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new();
    public MainPage()
    {
        this.InitializeComponent();
        Cellules.SetViewModel(ViewModel);
        TextPersonalization.SetViewModel(ViewModel);
        CellPersonalization.SetViewModel(ViewModel);

        TextPersonalization.CaptureRoot = RootGrid;
        CellPersonalization.CaptureRoot = RootGrid;
    }
}
