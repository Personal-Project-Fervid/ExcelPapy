namespace ExcelPapy.ViewModels;

public partial class CellViewModel : ObservableObject
{
    [ObservableProperty]
    private string _value = string.Empty;

    [ObservableProperty]
    private bool _isSelected = false;

    [ObservableProperty]
    private bool _isEditing = false;

    [ObservableProperty]
    private Thickness _selectionBorderThickness = new Thickness(0);

    public int Row { get; init; }
    public int Column { get; init; }

    public ColumnHeaderViewModel? ColumnHeader { get; init; }
    public RowHeaderViewModel? RowHeader { get; init; }


    // Personalization properties

    [ObservableProperty]
    private string _fontFamily = "Segoe UI"; 

    [ObservableProperty]
    private double _fontSize = 12; 

    [ObservableProperty]
    private Brush _foreground = new SolidColorBrush(Microsoft.UI.Colors.Black); 
}
