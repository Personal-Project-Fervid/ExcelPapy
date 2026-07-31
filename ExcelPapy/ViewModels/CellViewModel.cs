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

    [ObservableProperty]
    private bool _fontUnderline = false;

    public Thickness UnderlineMargin
    {
        get
        {
            return VerticalAlignment switch
            {
                "Top" => new Thickness(6, FontSize * 1.2, 6, 0),
                "Center" => new Thickness(6, 0, 6, -(FontSize)),
                "Bottom" => new Thickness(6, 0, 6, 4),
                _ => new Thickness(6, 0, 6, 4)
            };
        }
    }

    partial void OnVerticalAlignmentChanged(string value) => OnPropertyChanged(nameof(UnderlineMargin));
    partial void OnFontSizeChanged(double value) => OnPropertyChanged(nameof(UnderlineMargin));

    [ObservableProperty]
    private string _verticalAlignment = "Center";

    [ObservableProperty]
    private string _horizontalAlignment = "Left";

    [ObservableProperty]
    private Brush _background = new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XF5, 0XF5, 0XF5));

    private static readonly Brush GreyBorder = new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xBA, 0xBA, 0xBA));

    // Épaisseur indépendante par côté (grille Excel par défaut : droite + bas)
    [ObservableProperty]
    private double _borderThicknessTop = 0;

    [ObservableProperty]
    private double _borderThicknessLeft = 0;

    [ObservableProperty]
    private double _borderThicknessRight = 3;

    [ObservableProperty]
    private double _borderThicknessBottom = 3;

    // Couleur indépendante par côté
    [ObservableProperty]
    private Brush _borderBrushTop = GreyBorder;

    [ObservableProperty]
    private Brush _borderBrushLeft = GreyBorder;

    [ObservableProperty]
    private Brush _borderBrushRight = GreyBorder;

    [ObservableProperty]
    private Brush _borderBrushBottom = GreyBorder;

    private static readonly Brush TransparentBrush = new SolidColorBrush(Microsoft.UI.Colors.Transparent);

    [ObservableProperty]
    private Brush _cornerBottomRight = TransparentBrush;

    public MainViewModel? Owner { get; init; }

    public string DisplayValue
    {
        get
        {
            if (IsEditing || Owner == null) 
                return Value;

            return FormulaEngine.IsFormula(Value) 
                ? FormulaEngine.Evaluate(this, Owner)
                : Value;
        }
    }

    partial void OnValueChanged(string value)
    {
        if (!IsEditing)
            OnPropertyChanged(nameof(DisplayValue));
    }

    private string? _editingOriginalValue;
    public string? EditingOriginalValue => _editingOriginalValue;
    partial void OnIsEditingChanged(bool value)
    {
        if (value)
            _editingOriginalValue = Value;
        if (!value)
            OnPropertyChanged(nameof(DisplayValue));
    }

    public void RaiseDisplayValueChanged() => OnPropertyChanged(nameof(DisplayValue));

}
