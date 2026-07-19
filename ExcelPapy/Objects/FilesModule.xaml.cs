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
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using SkiaSharp;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ExcelPapy.Objects;

public sealed partial class FilesModule : UserControl
{
    public FilesModule()
    {
        this.InitializeComponent();
    }

    private void OnSavePickerClick(object sender, RoutedEventArgs e)
    {
        SavePickerBorder.Width = SavePickerButton.ActualWidth;
        SavePickerPopup.IsOpen = !SavePickerPopup.IsOpen;
    }

    public FrameworkElement CaptureRoot { get; set; }

    private byte[] _cachedBlurredFullImage;
    private int _cachedWidth;
    private int _cachedHeight;
    private bool _isBlurCacheReady = false;

    private CancellationTokenSource _saveBlurTransitionCts;

    private async void SavePickerPopup_Opened(object sender, object e)
    {
        _saveBlurTransitionCts?.Cancel();
        _saveBlurTransitionCts = new CancellationTokenSource();
        var token = _saveBlurTransitionCts.Token;

        BlurredSaveContainer.Visibility = Visibility.Collapsed;
        BlurredSaveContainertransition.Visibility = Visibility.Visible;
        BlurredSaveContainertransition.Opacity = 1;
        BlurredSaveContainer.Opacity = 0;

        SavePickerBorder.UpdateLayout();
        await Task.Yield();

        await EnsureBlurCacheAsync();
        ApplyCachedBlurToTarget(SavePickerBorder, BlurredSaveBackgroundBrush);

        try
        {
            _ = BlurTransition(BlurredSaveContainer, BlurredSaveContainertransition, token);
        }
        catch (OperationCanceledException) { }
    }

    private void SavePickerPopup_Closed(object sender, object e)
    {
        _saveBlurTransitionCts?.Cancel();

        BlurredSaveBackgroundBrush.ImageSource = null;
        BlurredSaveContainer.Opacity = 0;
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
