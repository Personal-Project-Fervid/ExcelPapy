using ExcelPapy.ViewModels;

namespace ExcelPapy.Objects;

public sealed partial class AppPersonalization : UserControl
{

    public AppPersonalization()
    {
        this.InitializeComponent();
    }
    public void SetViewModel(MainViewModel vm)
    {
        this.DataContext = vm;
    }

    private Tutorial _tutorial;
    public void SetTutorial(Tutorial tutorial)
    {
        _tutorial = tutorial;
    }

    private bool _isKeyDown = false;
    public void setKeyDown(bool value)
    {
        _isKeyDown = value;
        if (_isKeyDown)
        {
            BoldAppPickerPopup.IsOpen = false;
            PolicePickerPopup.IsOpen = false;
            MagnifyingGlassPickerPopup.IsOpen = false;
            TutorialPickerPopup.IsOpen = false;
        }
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

    private void OnMagnifyingGlassPickerClick(object sender, RoutedEventArgs e)
    {
        MagnifyingGlassPickerBorder.Width = MagnifyingGlassPickerButton.ActualWidth;
        MagnifyingGlassPickerPopup.IsOpen = !MagnifyingGlassPickerPopup.IsOpen;
    }

    private void OnMagnifyingGlassSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var police = btn.Tag?.ToString();
            SelectedMagnifyingGlassText.Text = police switch
            {
                "Segoe UI" => "Police",
                "Arial" => "Police",
                "Calibri" => "Police",
                "Times new roman" => "Police",
                "Verdana" => "Police",
                _ => "Police"
            };

            SelectedMagnifyingGlassText.FontFamily = police switch
            {
                "Segoe UI" => new FontFamily("Segoe UI"),
                "Arial" => new FontFamily("Arial"),
                "Calibri" => new FontFamily("Calibri"),
                "Times new roman" => new FontFamily("Times New Roman"),
                "Verdana" => new FontFamily("Verdana"),
                _ => new FontFamily("Arial")
            };

            MagnifyingGlassPickerPopup.IsOpen = false;
        }
    }

    private void OnTutorialPickerClick(object sender, RoutedEventArgs e)
    {
        TutorialPickerBorder.Width = TutorialPickerButton.ActualWidth;
        TutorialPickerPopup.IsOpen = !TutorialPickerPopup.IsOpen;
    }

    bool isAllChecked = false;
    private void AllCheck(object sender, RoutedEventArgs e)
    {
        if(!isAllChecked)
        {
            Loupe.IsChecked = true;
            Copier.IsChecked = true;
            Coller.IsChecked = true;
            Couper.IsChecked = true;
            Retour.IsChecked = true;
            Zoom.IsChecked = true;

            All.Content = "Tout décocher";
        }
        if(isAllChecked)
        {
            Loupe.IsChecked = false;
            Copier.IsChecked = false;
            Coller.IsChecked = false;
            Couper.IsChecked = false;
            Retour.IsChecked = false;
            Zoom.IsChecked = false;
            All.Content = "Tout cocher";
        }

        isAllChecked = !isAllChecked;
    }

    

    private void TutorialCheck(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb)
        {
            var Tuto = cb.Tag?.ToString();

            switch(Tuto)
            {
                case "Loupe": _tutorial.ShowTutorial(Tuto); break;
                case "Copier": _tutorial.ShowTutorial(Tuto); break;
                case "Coller": _tutorial.ShowTutorial(Tuto); break;
                case "Couper": _tutorial.ShowTutorial(Tuto); break;
                case "Retour": _tutorial.ShowTutorial(Tuto); break;
                case "Zoom": _tutorial.ShowTutorial(Tuto); break;
                default: break;
            };
        }
    }

    private void TutorialUncheck(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb)
        {
            var Tuto = cb.Tag?.ToString();

            switch (Tuto)
            {
                case "Loupe": _tutorial.HideTutorial(Tuto); break;
                case "Copier": _tutorial.HideTutorial(Tuto); break;
                case "Coller": _tutorial.HideTutorial(Tuto); break;
                case "Couper": _tutorial.HideTutorial(Tuto); break;
                case "Retour": _tutorial.HideTutorial(Tuto); break;
                case "Zoom": _tutorial.HideTutorial(Tuto); break;
                default: break;
            }
            ;
        }
    }

}
