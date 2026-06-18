using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ExcelPapy.Objects;

public sealed partial class TextPersonalization : UserControl
{
    public TextPersonalization()
    {
        this.InitializeComponent();

        SelectedPoliceText.Text = "Segoe UI";
        SelectedPoliceText.FontFamily = new FontFamily("Segoe UI");
        SelectedFontSizeText.Text = "12";
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
                "12" => "12",
                "14" => "14",
                "16" => "16",
                "18" => "18",
                _ => "Taille..."
            };

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
            SelectedPoliceColorText.Text = colorname switch
            {
                "White" => "A",
                "Black" => "A",
                "Green" => "A",
                "Purple" => "A",
                "Pink" => "A",
                "Blue" => "A",
                "Red" => "A",
                "Yellow" => "A",
                _ => "A"
            };

            SelectedPoliceColorIcon.Background = colorname switch
            {
                "White" => new SolidColorBrush(Microsoft.UI.Colors.White),
                "Black" => new SolidColorBrush(Microsoft.UI.Colors.Black),
                "Red" => new SolidColorBrush(Microsoft.UI.Colors.Red),
                "Blue" => new SolidColorBrush(Microsoft.UI.Colors.Blue),
                "Yellow" => new SolidColorBrush(Microsoft.UI.Colors.Yellow),
                "Green" => new SolidColorBrush(Microsoft.UI.Colors.Green),
                "Purple" => new SolidColorBrush(Microsoft.UI.Colors.Purple),
                "Pink" => new SolidColorBrush(Microsoft.UI.Colors.Pink),
                _ => new SolidColorBrush(Microsoft.UI.Colors.White)
            };

            PoliceColorPickerPopup.IsOpen = false;
        }
    }

    private void UpdatePoliceColorIcon(string? colorname)
    {

    }
}
