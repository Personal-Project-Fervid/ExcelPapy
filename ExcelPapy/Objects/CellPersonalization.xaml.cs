using Microsoft.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ExcelPapy.Objects;

public sealed partial class CellPersonalization : UserControl
{
    public CellPersonalization()
    {
        this.InitializeComponent();

        SelectedBorderText.Text = "Bordure Supérieure";
        UpdateSelectedIcon("Top");

        SelectedBackgroundText.Text = "Couleur du Fond";
        UpdateBackgroundIcon("Blanc");
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
                "White" => "Fond Blanc",
                "Grey" => "Fond Gris Clair",
                "Black" => "Fond Noir",
                "Green" => "Fond Vert",
                "Purple" => "Fond Violet",
                "Pink" => "Fond Rose",
                "Blue" => "Fond Bleu",
                "Red" => "Fond Rouge",
                "Yellow" => "Fond Jaune",
                _ => "Fond de Couleur..."
            };

            SelectedBackgroundIcon.Background = colorname switch
            {
                "White" => new SolidColorBrush(Microsoft.UI.Colors.White),
                "Grey" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xF5, 0xF5, 0xF5)),
                "Black" => new SolidColorBrush(Microsoft.UI.Colors.Black),
                "Red" => new SolidColorBrush(Microsoft.UI.Colors.Red),
                "Blue" => new SolidColorBrush(Microsoft.UI.Colors.Blue),
                "Yellow" => new SolidColorBrush(Microsoft.UI.Colors.Yellow),
                "Green" => new SolidColorBrush(Microsoft.UI.Colors.Green),
                "Purple" => new SolidColorBrush(Microsoft.UI.Colors.Purple),
                "Pink" => new SolidColorBrush(Microsoft.UI.Colors.Pink),
                _ => new SolidColorBrush(Microsoft.UI.Colors.White)
            };

            BackgroundPickerPopup.IsOpen = false;
        }
    }

    private void UpdateBackgroundIcon(string? colorname)
    {
           
    }
}
