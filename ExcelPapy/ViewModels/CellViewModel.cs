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
    private double _fontSize = 24; 

    [ObservableProperty]
    private Brush _foreground = new SolidColorBrush(Microsoft.UI.Colors.Black);

    [ObservableProperty]
    private bool _isBold = false;

    [ObservableProperty]
    private string _fontWeight = "Normal";

    [ObservableProperty]
    private bool _isItalic = false;

    [ObservableProperty]
    private string _fontStyle = "Normal";

    //À voir
    [ObservableProperty]
    private bool _fontUnderline = false;

    [ObservableProperty]
    private string _verticalAlignment = "Center";

    [ObservableProperty]
    private string _horizontalAlignment = "Left";

    [ObservableProperty]
    private Brush _background = new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XF5, 0XF5, 0XF5));

    [ObservableProperty]
    private Brush _borderBrush = new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XBA, 0XBA, 0XBA));
}
