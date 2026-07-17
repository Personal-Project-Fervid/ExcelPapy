using ExcelPapy.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ExcelPapy.Objects;

public sealed partial class Tutorial : UserControl
{
    public Tutorial()
    {
        this.InitializeComponent();
    }
    public void SetViewModel(MainViewModel vm)
    {
        this.DataContext = vm;
    }

    public void ShowTutorial(string tutorialName)
    {
        switch (tutorialName)
        {
            case "Loupe":
                loupe.Visibility = Visibility.Visible;
                break;
            case "Copier":
                copier.Visibility = Visibility.Visible;
                break;
            case "Coller":
                coller.Visibility = Visibility.Visible;
                break;
            case "Couper":
                couper.Visibility = Visibility.Visible;
                break;
            case "Retour":
                retour.Visibility = Visibility.Visible;
                break;
            case "Zoom":
                zoom.Visibility = Visibility.Visible;
                break;
            default:
                break;
        };
    }


    public void HideTutorial(string tutorialName)
    {
        switch(tutorialName)
        {
            case "Loupe":
                loupe.Visibility = Visibility.Collapsed;
                break;
            case "Copier":
                copier.Visibility = Visibility.Collapsed;
                break;
            case "Coller":
                coller.Visibility = Visibility.Collapsed;
                break;
            case "Couper":
                couper.Visibility = Visibility.Collapsed;
                break;
            case "Retour":
                retour.Visibility = Visibility.Collapsed;
                break;
            case "Zoom":
                zoom.Visibility = Visibility.Collapsed;
                break;
            default:
                break;
        };
    }

}
