using ExcelPapy.ViewModels;
using Microsoft.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ExcelPapy.Objects;

public sealed partial class CellPersonalization : UserControl
{

    private MainViewModel? _mainViewModel;
    public void SetViewModel(MainViewModel vm)
    {
        _mainViewModel = vm;
    }

    private Brush _background;
    public Brush Background
    {
        get => _background;
        set
        {
            _background = value;
        }
    }

    public CellPersonalization()
    {
        this.InitializeComponent();

        SelectedBorderText.Text = "Bordure Supérieure";
        UpdateSelectedIcon("Top");

        SelectedBackgroundText.Text = "Couleur du Fond";
    }

    private void OnBorderPickerClick(object sender, RoutedEventArgs e)
    {
        BorderPickerBorder.Width = BorderPickerButton.ActualWidth;
        BorderPickerPopup.IsOpen = !BorderPickerPopup.IsOpen;
    }

    private void OnBorderSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var tag = btn.Tag?.ToString();
            SelectedBorderText.Text = tag switch
            {
                "Top" => "Bordure Supérieure",
                "Left" => "Bordure Gauche",
                "Right" => "Bordure Droite",
                "Bottom" => "Bordure Inférieure",
                "None" => "Aucun Bord",
                "All" => "Toutes les Bordures",
                "Outer" => "Bordures Extérieurs",
                _ => "Bordure..."
            };

            //Mettre à jour l'icône du bouton principal
            UpdateSelectedIcon(tag);

            BorderPickerPopup.IsOpen = false;
        }
    }



    private void UpdateSelectedIcon(string? tag)
    {
        SelectedBorderIcon.Children.Clear();

        var lines = tag switch
        {
            "Top" => new[] { (0, 24, 0, 0, false), (0, 24, 12, 12, true), (0, 24, 24, 24, true), (0, 0, 0, 24, true), (12, 12, 0, 24, true), (24, 24, 0, 24, true) },
            "Left" => new[] { (0, 24, 0, 0, true), (0, 24, 12, 12, true), (0, 24, 24, 24, true), (0, 0, 0, 24, false), (12, 12, 0, 24, true), (24, 24, 0, 24, true) },
            "Right" => new[] { (0, 24, 0, 0, true), (0, 24, 12, 12, true), (0, 24, 24, 24, true), (0, 0, 0, 24, true), (12, 12, 0, 24, true), (24, 24, 0, 24, false) },
            "Bottom" => new[] { (0, 24, 0, 0, true), (0, 24, 12, 12, true), (0, 24, 24, 24, false), (0, 0, 0, 24, true), (12, 12, 0, 24, true), (24, 24, 0, 24, true) },
            "All" => new[] { (0, 24, 0, 0, false), (0, 24, 12, 12, false), (0, 24, 24, 24, false), (0, 0, 0, 24, false), (12, 12, 0, 24, false), (24, 24, 0, 24, false) },
            "Outer" => new[] { (0, 24, 0, 0, false), (0, 24, 12, 12, true), (0, 24, 24, 24, false), (0, 0, 0, 24, false), (12, 12, 0, 24, true), (24, 24, 0, 24, false) },
            _ => new[] { (0, 24, 0, 0, true), (0, 24, 12, 12, true), (0, 24, 24, 24, true), (0, 0, 0, 24, true), (12, 12, 0, 24, true), (24, 24, 0, 24, true) }
        };

        foreach (var (x1, x2, y1, y2, dashed) in lines)
        {
            var line = new Line
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                Stroke = new SolidColorBrush(Microsoft.UI.Colors.Black),
                StrokeThickness = 2
            };
            if (dashed)
                line.StrokeDashArray = new DoubleCollection { 2, 3 };
            SelectedBorderIcon.Children.Add(line);
        }
    }

    private void OnBackgroundPickerClick(object sender, RoutedEventArgs e)
    {
        BackgroundPickerBorder.Width = BackgroundPickerButton.ActualWidth;
        BackgroundPickerPopup.IsOpen = !BackgroundPickerPopup.IsOpen;
    }

    private void OnBackgroundSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var colorname = btn.Tag?.ToString();
            SelectedBackgroundText.Text = colorname switch
            {
                //Noir
                "Black" => "Fond Noir",
                "#1A1A1A" => "Fond Noir",
                "#1D1D1D" => "Fond Gris Foncé",
                "#2C2C2C" => "Fond Gris Foncé",
                "#404040" => "Fond Gris Moyen",
                "#565656" => "Fond Gris Moyen",
                "#6C6C6C" => "Fond Gris Moyen",
                "#828282" => "Fond Gris Moyen",

                //Blanc
                "White" => "Fond Blanc",
                "#E5E5E5" => "Fond Gris Clair",
                "#CBCBCB" => "Fond Gris Clair",
                "#B2B2B2" => "Fond Gris Clair",
                "#9A9A9A" => "Fond Gris Clair",

                //Rouge
                "Red" => "Fond Rouge",
                "#FF3B1E" => "Fond Rouge",
                "#FF5835" => "Fond Rouge",
                "#FF6E4A" => "Fond Rouge",
                "#FF8360" => "Fond Rouge clair",
                "#FF9675" => "Fond Rouge clair",
                "#FFA88B" => "Fond Rouge clair",
                "#FFBAA2" => "Fond Rouge clair",

                //Bleu
                "#0084FF" => "Fond Bleu",
                "#438EFF" => "Fond Bleu",
                "#6099FF" => "Fond Bleu",
                "#77A3FF" => "Fond Bleu",
                "#8CAEFF" => "Fond Bleu clair",
                "#9EB9FF" => "Fond Bleu clair",
                "#B0C5FF" => "Fond Bleu clair",
                "#C0D0FF" => "Fond Bleu clair",

                //Jaune
                "Yellow" => "Fond Jaune",
                "#FFFF6E" => "Fond Jaune",
                "#FFFF82" => "Fond Jaune",
                "#FFFF95" => "Fond Jaune",
                "#FFFFA7" => "Fond Jaune clair",
                "#FFFFB9" => "Fond Jaune clair",
                "#FFFFCB" => "Fond Jaune clair",
                "#FFFFDC" => "Fond Jaune clair",

                //Vert
                "#00C700" => "Fond Vert",
                "#3FCD30" => "Fond Vert",
                "#5CD249" => "Fond Vert",
                "#73D85F" => "Fond Vert",
                "#88DD74" => "Fond Vert clair",
                "#9AE288" => "Fond Vert clair",
                "#ACE89C" => "Fond Vert clair",
                "#BEEDB0" => "Fond Vert clair",

                //Violet
                "Purple" => "Fond Violet",
                "#8D298B" => "Fond Violet",
                "#994197" => "Fond Violet",
                "#A557A2" => "Fond Violet",
                "#B16CAD" => "Fond Violet clair",
                "#BD81B9" => "Fond Violet clair",
                "#C996C4" => "Fond Violet clair",
                "#D4AAD0" => "Fond Violet clair",

                //Rose
                "#EB469C" => "Fond Rose",
                "#EF5CA5" => "Fond Rose",
                "#F26FAD" => "Fond Rose",
                "#F581B6" => "Fond Rose",
                "#F892BF" => "Fond Rose clair",
                "#FAA2C8" => "Fond Rose clair",
                "#FCB2D1" => "Fond Rose clair",
                "#FEC2DA" => "Fond Rose clair",

                _ => "Fond de Couleur..."
            };

            SelectedBackgroundIcon.Background = colorname switch
            {
                //Noir
                "Black" => new SolidColorBrush(Microsoft.UI.Colors.Black),
                "#1A1A1A" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X1A, 0X1A, 0X1A)),
                "#1D1D1D" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X1D, 0X1D, 0X1D)),
                "#2C2C2C" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X2C, 0X2C, 0X2C)),
                "#404040" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X40, 0X40, 0X40)),
                "#565656" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X56, 0X56, 0X56)),
                "#6C6C6C" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X6C, 0X6C, 0X6C)),
                "#828282" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X82, 0X82, 0X82)),

                //Blanc
                "White" => new SolidColorBrush(Microsoft.UI.Colors.White),
                "#E5E5E5" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XE5, 0XE5, 0XE5)),
                "#CBCBCB" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XCB, 0XCB, 0XCB)),
                "#B2B2B2" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XB2, 0XB2, 0XB2)),
                "#9A9A9A" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X9A, 0X9A, 0X9A)),

                //Rouge
                "Red" => new SolidColorBrush(Microsoft.UI.Colors.Red),
                "#FF3B1E" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0X3B, 0X1E)),
                "#FF5835" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0X58, 0X35)),
                "#FF6E4A" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0X6E, 0X4A)),
                "#FF8360" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0X83, 0X60)),
                "#FF9675" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0X96, 0X75)),
                "#FFA88B" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0XA8, 0X8B)),
                "#FFBAA2" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0XBA, 0xA2)),

                //Bleu
                "#0084FF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X00, 0X84, 0XFF)),
                "#438EFF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X43, 0X8E, 0XFF)),
                "#6099FF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X60, 0X99, 0XFF)),
                "#77A3FF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X77, 0XA3, 0XFF)),
                "#8CAEFF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X8C, 0XAE, 0XFF)),
                "#9EB9FF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X9E, 0XB9, 0XFF)),
                "#B0C5FF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XB0, 0XC5, 0XFF)),
                "#C0D0FF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XC0, 0XD0, 0XFF)),

                //Jaune
                "Yellow" => new SolidColorBrush(Microsoft.UI.Colors.Yellow),
                "#FFFF6E" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0XFF, 0X6E)),
                "#FFFF82" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0XFF, 0X82)),
                "#FFFF95" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0XFF, 0X95)),
                "#FFFFA7" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0XFF, 0XA7)),
                "#FFFFB9" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0XFF, 0XB9)),
                "#FFFFCB" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0XFF, 0XCB)),
                "#FFFFDC" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFF, 0XFF, 0XDC)),

                //Vert
                "#00C700" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X00, 0XC7, 0X00)),
                "#3FCD30" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X3F, 0XCD, 0X30)),
                "#5CD249" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X5C, 0XD2, 0X49)),
                "#73D85F" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X73, 0XD8, 0X5F)),
                "#88DD74" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X88, 0XDD, 0X74)),
                "#9AE288" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X9A, 0XE2, 0X88)),
                "#ACE89C" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XAC, 0XE8, 0X9C)),
                "#BEEDB0" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XBE, 0XED, 0XB0)),

                //Violet
                "Purple" => new SolidColorBrush(Microsoft.UI.Colors.Purple),
                "#8D298B" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X8D, 0X29, 0X8B)),
                "#994197" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0X99, 0X41, 0X97)),
                "#A557A2" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XA5, 0X57, 0XA2)),
                "#B16CAD" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XB1, 0X6C, 0XAD)),
                "#BD81B9" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XBD, 0X81, 0XB9)),
                "#C996C4" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XC9, 0X96, 0XC4)),
                "#D4AAD0" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XD4, 0XAA, 0XD0)),

                //Rose
                "#EB469C" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XEB, 0X46, 0X9C)),
                "#EF5CA5" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XEF, 0X5C, 0XA5)),
                "#F26FAD" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XF2, 0X6F, 0XAD)),
                "#F581B6" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XF5, 0X81, 0XB6)),
                "#F892BF" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XF8, 0X92, 0XBF)),
                "#FAA2C8" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFA, 0XA2, 0XC8)),
                "#FCB2D1" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFC, 0XB2, 0XD1)),
                "#FEC2DA" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0XFE, 0XC2, 0XDA)),

                _ => new SolidColorBrush(Microsoft.UI.Colors.White)
            };

            Background = SelectedBackgroundIcon.Background;
            _mainViewModel?.ApplyBackgroundToSelection(Background);

            BackgroundPickerPopup.IsOpen = false;
        }
    }

    private void OnVerticalAlignmentSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            _mainViewModel?.ApplyVerticalAlignmentToSelection(btn.Tag?.ToString() ?? "Center");
        }
    }

    private void OnHorizontalAlignmentSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            _mainViewModel?.ApplyHorizontalAlignmentToSelection(btn.Tag?.ToString() ?? "Left");
        }
    }

    private void BackgroundPersonalization(object sender, RoutedEventArgs e)
    {
        _mainViewModel?.ApplyBackgroundToSelection(Background);
    }

    private void BorderPersonalization(object sender, RoutedEventArgs e)
    {

    }
}
