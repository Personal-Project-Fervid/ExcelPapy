using ExcelPapy.ViewModels;
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace ExcelPapy.Objects;

public sealed partial class TextPersonalization : UserControl
{
    public FrameworkElement CaptureRoot {  get; set; }

    private byte[] _cachedBlurredFullImage;
    private int _cachedWidth;
    private int _cachedHeight;
    private bool _isBlurCacheReady = false;

    private bool _isKeyDown = false;
    public void setKeyDown(bool value)
    {
        _isKeyDown = value;
        if (_isKeyDown)
        {
            PolicePickerPopup.IsOpen = false;
            PoliceColorPickerPopup.IsOpen = false;
            FontSizePickerPopup.IsOpen = false;
        }
    }

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
        ApplyPlaceholderBackground();
    }

    private void ApplyPlaceholderBackground()
    {
        var placeholder = new BitmapImage();
        // Une image 1x1 blanche suffit, étirée par Stretch="Fill"
        BlurredBackgroundBrush.ImageSource = placeholder;
        BlurredFontSizeBackgroundBrush.ImageSource = placeholder;
        BlurredPoliceColorBackgroundBrush.ImageSource = placeholder;
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
                "#B41A09" => new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xB4, 0x1A, 0x09)),
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

    private async Task BlurTransition(FrameworkElement x, FrameworkElement y, CancellationToken token)
    {
        x.Visibility = Visibility.Visible;
        for (double i = 0; i < 1; i+= 0.05)
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

    private CancellationTokenSource _policeBlurTransitionCts;
    private CancellationTokenSource _fontSizeBlurTransitionCts;
    private CancellationTokenSource _policeColorBlurTransitionCts;
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
        await RefreshBlurCacheAsync();
        ApplyCachedBlurToTarget(PolicePickerBorder, BlurredBackgroundBrush);

        try
        {
            await BlurTransition(BlurredPoliceContainer, BlurredPoliceContainertransition, token);
        }catch (OperationCanceledException) { }
    }

    private void PolicePickerPopup_Closed(object sender, object e)
    {
        _policeBlurTransitionCts?.Cancel();

        BlurredBackgroundBrush.ImageSource = null;
        BlurredPoliceContainer.Opacity = 0;
    }

    private async void FontSizePickerPopup_Opened(object sender, object e)
    {
        _fontSizeBlurTransitionCts?.Cancel();
        _fontSizeBlurTransitionCts = new CancellationTokenSource();
        var token = _fontSizeBlurTransitionCts.Token;

        BlurredFontSizeContainer.Visibility = Visibility.Collapsed;
        BlurredFontSizeContainertransition.Visibility = Visibility.Visible;
        BlurredFontSizeContainertransition.Opacity = 1;
        BlurredFontSizeContainer.Opacity = 0;

        FontSizePickerBorder.UpdateLayout();
        await Task.Yield();

        await RefreshBlurCacheAsync();
        ApplyCachedBlurToTarget(FontSizePickerBorder, BlurredFontSizeBackgroundBrush);

        try
        {
            _ = BlurTransition(BlurredFontSizeContainer, BlurredFontSizeContainertransition, token);
        }
        catch (OperationCanceledException) { }
    }

    private void FontSizePickerPopup_Closed(object sender, object e)
    {
        _fontSizeBlurTransitionCts?.Cancel();

        BlurredFontSizeBackgroundBrush.ImageSource = null;
        BlurredFontSizeContainer.Opacity = 0;
    }

    private async void PoliceColorPickerPopup_Opened(object sender, object e)
    {
        _policeColorBlurTransitionCts?.Cancel();
        _policeColorBlurTransitionCts = new CancellationTokenSource();
        var token = _policeColorBlurTransitionCts.Token;

        BlurredPoliceColorContainer.Visibility = Visibility.Collapsed;
        BlurredPoliceColorContainertransition.Visibility = Visibility.Visible;
        BlurredPoliceColorContainertransition.Opacity = 1;
        BlurredPoliceColorContainer.Opacity = 0;

        PoliceColorPickerBorder.UpdateLayout();
        await Task.Yield();
         
        await RefreshBlurCacheAsync();
        ApplyCachedBlurToTarget(PoliceColorPickerBorder, BlurredPoliceColorBackgroundBrush);

        try 
        { 
            _ = BlurTransition(BlurredPoliceColorContainer, BlurredPoliceColorContainertransition, token);
        }
        catch (OperationCanceledException) { }
    }

    private void PoliceColorPickerPopup_Closed(object sender, object e)
    {
        _policeColorBlurTransitionCts?.Cancel();

        BlurredPoliceColorBackgroundBrush.ImageSource = null;
        BlurredPoliceColorContainer.Opacity = 0;
    }


    private const float blurRadius = 2f;

    private async Task RefreshBlurCacheAsync()
    {
        
        try
        {
            var elementToCapture = CaptureRoot
                ?? Window.Current.Content as FrameworkElement;

            System.Diagnostics.Debug.WriteLine($"[BLUR] CaptureRoot = {CaptureRoot?.GetType().Name ?? "NULL"}");
            System.Diagnostics.Debug.WriteLine($"[BLUR] elementToCapture = {elementToCapture?.GetType().Name ?? "NULL"}");

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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur application flou popup : {ex.Message}");
        }
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
}
