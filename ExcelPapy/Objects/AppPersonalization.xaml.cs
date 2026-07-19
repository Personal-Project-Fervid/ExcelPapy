using System.Runtime.InteropServices.WindowsRuntime;
using ExcelPapy.ViewModels;
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using Windows.Storage.Streams;

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
            LanguagePickerPopup.IsOpen = false;
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

    private void OnLanguagePickerClick(object sender, RoutedEventArgs e)
    {
        LanguagePickerBorder.Width = LanguagePickerButton.ActualWidth;
        LanguagePickerPopup.IsOpen = !LanguagePickerPopup.IsOpen;
    }

    private void OnLanguageSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var language = btn.Tag?.ToString();
            SelectedLanguageText.Text = language switch
            {
                "Français" => "Français",
                "English" => "English",
                _ => "Français"
            };

            LanguagePickerPopup.IsOpen = false;
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

            All.Text = "Tout décocher";
        }
        if(isAllChecked)
        {
            Loupe.IsChecked = false;
            Copier.IsChecked = false;
            Coller.IsChecked = false;
            Couper.IsChecked = false;
            Retour.IsChecked = false;
            Zoom.IsChecked = false;
            All.Text = "Tout cocher";
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




    public FrameworkElement CaptureRoot { get; set; }

    private byte[] _cachedBlurredFullImage;
    private int _cachedWidth;
    private int _cachedHeight;
    private bool _isBlurCacheReady = false;

    private CancellationTokenSource _boldAppBlurTransitionCts;
    private CancellationTokenSource _policeBlurTransitionCts;
    private CancellationTokenSource _languageBlurTransitionCts;
    private CancellationTokenSource _tutorialBlurTransitionCts;

    private async void BoldAppPickerPopup_Opened(object sender, object e)
    {
        _boldAppBlurTransitionCts?.Cancel();
        _boldAppBlurTransitionCts = new CancellationTokenSource();
        var token = _boldAppBlurTransitionCts.Token;

        BlurredBoldAppContainer.Visibility = Visibility.Collapsed;
        BlurredBoldAppContainertransition.Visibility = Visibility.Visible;
        BlurredBoldAppContainertransition.Opacity = 1;
        BlurredBoldAppContainer.Opacity = 0;

        BoldAppPickerBorder.UpdateLayout();
        await Task.Yield();

        await EnsureBlurCacheAsync();
        ApplyCachedBlurToTarget(BoldAppPickerBorder, BlurredBoldAppBackgroundBrush);

        try
        {
            _ = BlurTransition(BlurredBoldAppContainer, BlurredBoldAppContainertransition, token);
        }
        catch (OperationCanceledException) { }
    }

    private void BoldAppPickerPopup_Closed(object sender, object e)
    {
        _boldAppBlurTransitionCts?.Cancel();

        BlurredBoldAppBackgroundBrush.ImageSource = null;
        BlurredBoldAppContainer.Opacity = 0;
    }

    private async void PolicePickerPopup_Opened(object sender, object e)
    {
        _policeBlurTransitionCts?.Cancel();
        _policeBlurTransitionCts = new CancellationTokenSource();
        var token = _policeBlurTransitionCts.Token;

        BlurredPoliceContainer.Visibility = Visibility.Collapsed;
        BlurredPoliceContainertransition.Visibility = Visibility.Visible;
        BlurredPoliceContainertransition.Opacity = 1;
        BlurredPoliceContainer.Opacity = 0;

        PolicePickerBorder.UpdateLayout();
        await Task.Yield();

        await EnsureBlurCacheAsync();
        ApplyCachedBlurToTarget(PolicePickerBorder, BlurredPoliceBackgroundBrush);
        try
        {
            _ = BlurTransition(BlurredPoliceContainer, BlurredPoliceContainertransition, token);
        }
        catch (OperationCanceledException) { }
    }

    private void PolicePickerPopup_Closed(object sender, object e)
    {
        _policeBlurTransitionCts?.Cancel();
        BlurredPoliceBackgroundBrush.ImageSource = null;
        BlurredPoliceContainer.Opacity = 0;
    }

    private async void LanguagePickerPopup_Opened(object sender, object e)
    {
        _languageBlurTransitionCts?.Cancel();
        _languageBlurTransitionCts = new CancellationTokenSource();
        var token = _languageBlurTransitionCts.Token;

        BlurredLanguageContainer.Visibility = Visibility.Collapsed;
        BlurredLanguageContainertransition.Visibility = Visibility.Visible;
        BlurredLanguageContainertransition.Opacity = 1;
        BlurredLanguageContainer.Opacity = 0;

        LanguagePickerBorder.UpdateLayout();
        await Task.Yield();

        await EnsureBlurCacheAsync();
        ApplyCachedBlurToTarget(LanguagePickerBorder, BlurredLanguageBackgroundBrush);

        try
        {
            _ = BlurTransition(BlurredLanguageContainer, BlurredLanguageContainertransition, token);
        }
        catch (OperationCanceledException) { }
    }

    private void LanguagePickerPopup_Closed(object sender, object e)
    {
        _languageBlurTransitionCts?.Cancel();

        BlurredLanguageBackgroundBrush.ImageSource = null;
        BlurredLanguageContainer.Opacity = 0;
    }

    private async void TutorialPickerPopup_Opened(object sender, object e)
    {
        _tutorialBlurTransitionCts?.Cancel();
        _tutorialBlurTransitionCts = new CancellationTokenSource();
        var token = _tutorialBlurTransitionCts.Token;

        BlurredTutorialContainer.Visibility = Visibility.Collapsed;
        BlurredTutorialContainertransition.Visibility = Visibility.Visible;
        BlurredTutorialContainertransition.Opacity = 1;
        BlurredTutorialContainer.Opacity = 0;

        TutorialPickerBorder.UpdateLayout();
        await Task.Yield();

        await EnsureBlurCacheAsync();
        ApplyCachedBlurToTarget(TutorialPickerBorder, BlurredTutorialBackgroundBrush);

        try
        {
            _ = BlurTransition(BlurredTutorialContainer, BlurredTutorialContainertransition, token);
        }
        catch (OperationCanceledException) { }
    }

    private void TutorialPickerPopup_Closed(object sender, object e)
    {
        _tutorialBlurTransitionCts?.Cancel();

        BlurredTutorialBackgroundBrush.ImageSource = null;
        BlurredTutorialContainer.Opacity = 0;
    }


    private const float blurRadius = 2f;

    private async Task EnsureBlurCacheAsync()
    {
        try
        {
            var elementToCapture = CaptureRoot
                ?? Window.Current.Content as FrameworkElement;

            if (elementToCapture == null)
            {
                System.Diagnostics.Debug.WriteLine("Aucune racine de capture disponible.");
                return;
            }

            var renderTarget = new RenderTargetBitmap();
            await renderTarget.RenderAsync(elementToCapture);

            var pixelBuffer = await renderTarget.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();

            int width = renderTarget.PixelWidth;
            int height = renderTarget.PixelHeight;

            _cachedBlurredFullImage = ApplyGaussianBlur(pixels, width, height, blurRadius);
            _cachedWidth = width;
            _cachedHeight = height;
            _isBlurCacheReady = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur précalcul flou : {ex.Message}");
        }
    }

    private async void ApplyCachedBlurToTarget(FrameworkElement target, ImageBrush destinationBrush)
    {

        try
        {
            var elementToCapture = CaptureRoot
                ?? Window.Current.Content as FrameworkElement;

            var croppedBytes = CropToElement(_cachedBlurredFullImage, _cachedWidth, _cachedHeight, target, elementToCapture);

            var bitmapImage = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(croppedBytes.AsBuffer());
                stream.Seek(0);
                await bitmapImage.SetSourceAsync(stream);
            }

            destinationBrush.ImageSource = bitmapImage;

            // Restaure l'opacité normale une fois le vrai flou appliqué
            // (au cas où le conteneur parent avait été ajusté pour le placeholder)
        }
        catch (Exception) { }
    }

    private byte[] ApplyGaussianBlur(byte[] bgraPixels, int width, int height, float blurRadius)
    {
        // SkiaSharp attend du BGRA8888 — RenderTargetBitmap produit déjà ce format
        using var bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888));
        System.Runtime.InteropServices.Marshal.Copy(bgraPixels, 0, bitmap.GetPixels(), bgraPixels.Length);

        using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888));
        var canvas = surface.Canvas;

        using var blurFilter = SKImageFilter.CreateBlur(blurRadius, blurRadius);
        using var paint = new SKPaint { ImageFilter = blurFilter };

        canvas.DrawBitmap(bitmap, 0, 0, paint);
        canvas.Flush();

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private byte[] CropToElement(byte[] pngBytes, int sourceWidth, int sourceHeight, FrameworkElement target, FrameworkElement captureRoot)
    {
        // Calcule la position de l'élément cible par rapport à la racine capturée
        var transform = target.TransformToVisual(captureRoot);
        var bounds = transform.TransformBounds(new Windows.Foundation.Rect(0, 0, target.ActualWidth, target.ActualHeight));

        using var original = SKBitmap.Decode(pngBytes);

        var cropRect = SKRectI.Round(new SKRect(
            (float)bounds.X, (float)bounds.Y,
            (float)(bounds.X + bounds.Width), (float)(bounds.Y + bounds.Height)));

        // Clamp pour éviter de sortir des limites du bitmap source
        cropRect.Left = Math.Max(0, cropRect.Left);
        cropRect.Top = Math.Max(0, cropRect.Top);
        cropRect.Right = Math.Min(sourceWidth, cropRect.Right);
        cropRect.Bottom = Math.Min(sourceHeight, cropRect.Bottom);

        using var cropped = new SKBitmap(cropRect.Width, cropRect.Height);
        using var canvas = new SKCanvas(cropped);
        canvas.DrawBitmap(original, cropRect, new SKRect(0, 0, cropRect.Width, cropRect.Height));

        using var image = SKImage.FromBitmap(cropped);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private async Task BlurTransition(FrameworkElement x, FrameworkElement y, CancellationToken token)
    {
        x.Visibility = Visibility.Visible;
        for (double i = 0; i < 1; i += 0.05)
        {
            token.ThrowIfCancellationRequested();
            y.Opacity = 1 - i;
            x.Opacity = i;
            await Task.Delay(50);
        }
        y.Opacity = 0;
        x.Opacity = 1;
        y.Visibility = Visibility.Collapsed;
    }

}
