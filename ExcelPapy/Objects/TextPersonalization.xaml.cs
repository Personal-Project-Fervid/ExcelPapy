using ExcelPapy.ViewModels;

namespace ExcelPapy.Objects;

public sealed partial class TextPersonalization : UserControl
{
    private string? _policeCell;
    public string? PoliceCell
    {
        get => _policeCell;
        set
        {
            _policeCell = value;
        }
    }

    private string? _fontSizeCell;
    public string? FontSizeCell
    {
        get => _fontSizeCell;
        set
        {
            _fontSizeCell = value;
        }
    }

    private Brush? _policeColorCell;
    public Brush? PoliceColorCell
    {
        get => _policeColorCell;
        set
        {
            _policeColorCell = value;
        }
    }

    public TextPersonalization()
    {
        this.InitializeComponent();

        SelectedPoliceText.Text = "Segoe UI";
        SelectedPoliceText.FontFamily = new FontFamily("Segoe UI");
        PoliceCell = "Segoe UI";
        SelectedFontSizeText.Text = "12";
        FontSizeCell = "12";
        SelectedPoliceColorText.Text = "A";
    }

    private MainViewModel? _mainViewModel;
    public void SetViewModel(MainViewModel vm)
    {
        _mainViewModel = vm;
    }

    private void OnPolicePickerClick(object sender, RoutedEventArgs e)
    {
        PolicePickerBorder.Width = PolicePickerButton.ActualWidth;
        PolicePickerPopup.IsOpen = !PolicePickerPopup.IsOpen;
    }

    private void OnPoliceSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var police = btn.Tag?.ToString();
            SelectedPoliceText.Text = police switch
            {
                "Segoe UI" => "Segoe UI",
                "Arial" => "Arial",
                "Calibri" => "Calibri",
                "Times new roman" => "Times New Roman",
                "Verdana" => "Verdana",
                _ => "Police..."
            };

            SelectedPoliceText.FontFamily = police switch
            {
                "Segoe UI" => new FontFamily("Segoe UI"),
                "Arial" => new FontFamily("Arial"),
                "Calibri" => new FontFamily("Calibri"),
                "Times new roman" => new FontFamily("Times New Roman"),
                "Verdana" => new FontFamily("Verdana"),
                _ => new FontFamily("Segoe UI")
            };

            PoliceCell = police;


            _mainViewModel?.ApplyFontFamilyToSelection(PoliceCell ?? "Segoe UI");

            PolicePickerPopup.IsOpen = false;
        }
    }

    private void OnFontSizePickerClick(object sender, RoutedEventArgs e)
    {
        FontSizePickerBorder.Width = FontSizePickerButton.ActualWidth;
        FontSizePickerPopup.IsOpen = !FontSizePickerPopup.IsOpen;
    }

    private void OnFontSizeSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var fontSize = btn.Tag?.ToString();
            SelectedFontSizeText.Text = fontSize switch
            {
                "8" => "8",
                "9" => "9",
                "10" => "10",
                "11" => "11",
                "12" => "12",
                "14" => "14",
                "16" => "16",
                "18" => "18",
                "20" => "20",
                "22" => "22",
                "24" => "24",
                "26" => "26",
                "28" => "28",
                "36" => "36",
                "48" => "48",
                "72" => "72",

                _ => "Taille..."
            };

            FontSizeCell = fontSize;


            var x = double.Parse(FontSizeCell);
            _mainViewModel?.ApplyFontSizeToSelection(x);

            FontSizePickerPopup.IsOpen = false;
        }
    }

    private void OnPoliceColorPickerClick(object sender, RoutedEventArgs e)
    {
        PoliceColorPickerBorder.Width = PoliceColorPickerButton.ActualWidth;
        PoliceColorPickerPopup.IsOpen = !PoliceColorPickerPopup.IsOpen;
    }

    private void OnPoliceColorSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var colorname = btn.Tag?.ToString();

            SelectedPoliceColorIcon.Background = colorname switch
            {
                //Noir  Blanc
                "Black" => new SolidColorBrush(Microsoft.UI.Colors.Black),
                "#A8A8A8" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xA8, 0xA8, 0xA8)),
                "White" => new SolidColorBrush(Microsoft.UI.Colors.White),

                //Rouge
                "#B41A09"=> new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xB4, 0x1A, 0x09)),
                "Red" => new SolidColorBrush(Microsoft.UI.Colors.Red),
                "#FF9675" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xFF, 0x96, 0x75)),

                //Bleu
                "#20529A" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0x20, 0x52, 0x9A)),
                "#0084FF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0x00, 0x84, 0xFF)),
                "#8CAEFF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0x8C, 0xAE, 0xFF)),

                //Jaune
                "#B3B21A" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xB3, 0xB2, 0x1A)),
                "Yellow" => new SolidColorBrush(Microsoft.UI.Colors.Yellow),
                "#FFFF95" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xFF, 0xFF, 0x95)),

                //Vert
                "#1E7A15" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0x1E, 0x7A, 0x15)),
                "#00C700" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0x00, 0xC7, 0x00)),
                "#88DD74" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0x88, 0xDD, 0x74)),

                //Violet
                "#511150" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0x51, 0x11, 0x50)),
                "Purple" => new SolidColorBrush(Microsoft.UI.Colors.Purple),
                "#BD81B9" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xBD, 0x81, 0xB9)),

                //Rose
                "#903261" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0x90, 0x32, 0x61)),
                "#EB469C" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xEB, 0x46, 0x9C)),
                "#FAA2C8" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xFA, 0xA2, 0xC8)),  

                _ => new SolidColorBrush(Microsoft.UI.Colors.Black)
            };

            PoliceColorCell = SelectedPoliceColorIcon.Background;

            _mainViewModel?.ApplyForegroundToSelection(PoliceColorCell ?? new SolidColorBrush(Microsoft.UI.Colors.Black));

            PoliceColorPickerPopup.IsOpen = false;
        }
    }

    public void PolicePersonalization(object sender, RoutedEventArgs e)
    {
        _mainViewModel?.ApplyFontFamilyToSelection(PoliceCell ?? "Segoe UI");
    }

    public void FontSizePersonalization(object sender, RoutedEventArgs e)
    {
        double fontSize = double.Parse(FontSizeCell);
        _mainViewModel?.ApplyFontSizeToSelection(fontSize);
    }

    public void PoliceColorPersonalization(object sender, RoutedEventArgs e)
    {

        _mainViewModel?.ApplyForegroundToSelection(PoliceColorCell ?? new SolidColorBrush(Microsoft.UI.Colors.Black));
    }

    public void BoldPersonalization(object sender, RoutedEventArgs e)
    {
        _mainViewModel?.ApplyFontWeightToSelection();
    }

    public void FontStylePersonalization(object sender, RoutedEventArgs e)
    {

        _mainViewModel?.ApplyFontStyleToSelection();
    }

    public void UnderlinePersonalization(object sender, RoutedEventArgs e)
    {
        // à voir
    }


}
