using Microsoft.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ExcelPapy.Objects;

public sealed partial class CellPersonalization : UserControl
{
    public CellPersonalization()
    {
        this.InitializeComponent();

        SelectedBorderText.Text = "Bord du Haut";
        UpdateSelectedIcon("Top");

        SelectedBackgroundText.Text = "Fond du Haut";
        UpdateBackgroundIcon("Top");
    }

    private void OnBorderPickerClick(object sender, RoutedEventArgs e)
    {
        BorderPickerPopup.Width = BorderPickerButton.ActualWidth;
        BorderPickerPopup.IsOpen = !BorderPickerPopup.IsOpen;
    }

    private void OnBackgroundPickerClick(object sender, RoutedEventArgs e)
    {
        BackgroundPickerPopup.Width = BackgroundPickerButton.ActualWidth;
        BackgroundPickerPopup.IsOpen = !BackgroundPickerPopup.IsOpen;
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

    private void OnBackgroundSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var tag = btn.Tag?.ToString();
            SelectedBackgroundText.Text = tag switch
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

            BackgroundPickerPopup.IsOpen = false;
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
}

private void UpdateBackgroundIcon(string? tag)
    {
        SelectedBackgroundIcon.Children.Clear();

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
            SelectedBackgroundIcon.Children.Add(line);
        }
    }
}
