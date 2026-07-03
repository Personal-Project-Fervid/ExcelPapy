namespace ExcelPapy.Objects;

public sealed partial class AppPersonalization : UserControl
{

    public AppPersonalization()
    {
        this.InitializeComponent();
    }

    private void OnBoldAppPickerClick(object sender, RoutedEventArgs e)
    {
        BoldAppPickerBorder.Width = BoldAppPickerButton.ActualWidth;
        BoldAppPickerPopup.IsOpen = !BoldAppPickerPopup.IsOpen;
    }

    private void OnBoldAppSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var boldApp = btn.Tag?.ToString();
            SelectedBoldAppText.Text = boldApp switch
            {
                "Light" => "G",
                "Normal" => "G",
                "DemiBold" => "G",
                "Bold" => "G",
                "ExtraBold" => "G",
                "Black" => "G",
                "UltraBlack" => "G",
                _ => "G"
            };

            SelectedBoldAppText.FontWeight = boldApp switch
            {
                "Light" => Microsoft.UI.Text.FontWeights.Light,
                "Normal" => Microsoft.UI.Text.FontWeights.Normal,
                "DemiBold" => Microsoft.UI.Text.FontWeights.DemiBold,
                "Bold" => Microsoft.UI.Text.FontWeights.Bold,
                "ExtraBold" => Microsoft.UI.Text.FontWeights.ExtraBold,
                "Black" => Microsoft.UI.Text.FontWeights.Black,
                "UltraBlack" => Microsoft.UI.Text.FontWeights.UltraBlack,
                _ => Microsoft.UI.Text.FontWeights.DemiBold
            };

            BoldAppPickerPopup.IsOpen = false;
        }
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
                "Segoe UI" => "Police",
                "Arial" => "Police",
                "Calibri" => "Police",
                "Times new roman" => "Police",
                "Verdana" => "Police",
                _ => "Police"
            };

            SelectedPoliceText.FontFamily = police switch
            {
                "Segoe UI" => new FontFamily("Segoe UI"),
                "Arial" => new FontFamily("Arial"),
                "Calibri" => new FontFamily("Calibri"),
                "Times new roman" => new FontFamily("Times New Roman"),
                "Verdana" => new FontFamily("Verdana"),
                _ => new FontFamily("Arial")
            };

            PolicePickerPopup.IsOpen = false;
        }
    }
}
