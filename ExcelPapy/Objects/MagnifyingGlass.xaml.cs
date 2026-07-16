using System.Runtime.InteropServices;
using ExcelPapy.ViewModels;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Hosting;

namespace ExcelPapy.Objects;

public sealed partial class MagnifyingGlass : UserControl
{
    private double _rasterizationScale = 1.0;
    private double _magnification = 2.0;
    private RenderTargetBitmap? _snapshot;
    private MainViewModel? _mainViewModel;

    public FrameworkElement? CaptureRoot { get; set; }

    public MagnifyingGlass()
    {
        this.InitializeComponent();
        this.Loaded += MagnifyingGlass_Loaded;
        this.Unloaded += MagnifyingGlass_Unloaded;
    }

    public void SetViewModel(MainViewModel vm)
    {
        _mainViewModel = vm;
    }

    public void UpdatePointer(Point position)
    {
        _pointerPosition = position;
        UpdateMagnifyingGlassPosition();
    }

    /*private void MagnifyingGlass_Loaded(object sender, RoutedEventArgs e)
    {
        if (CaptureRoot?.XamlRoot != null)
        {
            _rasterizationScale = CaptureRoot.XamlRoot.RasterizationScale;
            CaptureRoot.XamlRoot.Changed += XamlRoot_Changed;
        }
        else if (Window.Current?.Content is FrameworkElement fe && fe.XamlRoot != null)
        {
            _rasterizationScale = fe.XamlRoot.RasterizationScale;
            fe.XamlRoot.Changed += XamlRoot_Changed;
        }
    }*/

    private void MagnifyingGlass_Unloaded(object sender, RoutedEventArgs e)
    {
        if (CaptureRoot?.XamlRoot != null)
        {
            CaptureRoot.XamlRoot.Changed -= XamlRoot_Changed;
        }
        else if (Window.Current?.Content is FrameworkElement fe && fe.XamlRoot != null)
        {
            fe.XamlRoot.Changed -= XamlRoot_Changed;
        }
    }

    private async void XamlRoot_Changed(XamlRoot sender, XamlRootChangedEventArgs args)
    {
        // Si le RasterizationScale a changé, on met à jour et on recapture
        var newScale = sender.RasterizationScale;
        if (!DoubleUtil.AreClose(newScale, _rasterizationScale))
        {
            _rasterizationScale = newScale;
            if (this.Visibility == Visibility.Visible)
            {
                await CaptureAppAsync();
            }
        }
    }

    public async Task CaptureAppAsync()
    {
        var elementToCapture = CaptureRoot ?? Window.Current.Content as FrameworkElement;
        if (elementToCapture == null) return;

        // Récupérer la rasterization scale actuelle
        _rasterizationScale = elementToCapture.XamlRoot?.RasterizationScale ?? 1.0;

        // Calculer la taille en pixels réels
        var width = elementToCapture.ActualWidth;
        var height = elementToCapture.ActualHeight;
        if (width <= 0 || height <= 0) return;

        int pxW = Math.Max(1, (int)Math.Round(width * _rasterizationScale));
        int pxH = Math.Max(1, (int)Math.Round(height * _rasterizationScale));

        _snapshot = new RenderTargetBitmap();
        try
        {
            await _snapshot.RenderAsync(elementToCapture, pxW, pxH);
        }
        catch
        {
            // En cas d'échec, tenter une capture sans dimensions
            _snapshot = new RenderTargetBitmap();
            await _snapshot.RenderAsync(elementToCapture);
        }

        SetImage(_snapshot);
    }

    public void SetImage(RenderTargetBitmap bitmap)
    {
        if (bitmap == null) return;
        ViewContent.Source = bitmap;
        ApplyZoom();
    }

    private void ApplyZoom()
    {
        double effectiveScale = _magnification / _rasterizationScale;
        ZoomTransform.ScaleX = effectiveScale;
        ZoomTransform.ScaleY = effectiveScale;
    }

    [DllImport("user32.dll")]
    private static extern int ShowCursor(bool bShow);

    

    // Petit utilitaire pour comparer doubles
    private static class DoubleUtil
    {
        private const double Epsilon = 1e-6;
        public static bool AreClose(double a, double b) => Math.Abs(a - b) < Epsilon;
    }

    public void HideCursor()
    {
        ShowCursor(false);
    }

    public void ShowCursorBack()
    {
        ShowCursor(true);
    }

    private Windows.Foundation.Point _pointerPosition;

    private void RootGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        _pointerPosition = e.GetCurrentPoint(CaptureRoot).Position;

            UpdateMagnifyingGlassPosition();
        
    }

    private void UpdateMagnifyingGlassPosition()
    {
        //Centrer l'objet sur le curseur (ajustez l'offset selon vos besoins)
        GlassRootTransform.X = _pointerPosition.X - 125;
        GlassRootTransform.Y = _pointerPosition.Y - 125;
        //EllipseTransform.X = _pointerPosition.X - (EllipseGlass.Width / 2);
        //EllipseTransform.Y = _pointerPosition.Y - (EllipseGlass.Height / 2);

        UpdatePan(_pointerPosition);
    }

    public void UpdatePan(Point pointerPositionInCaptureRoot)
    {
        double effectiveScale = _magnification / _rasterizationScale;

        //PanTransform.X = -(pointerPositionInCaptureRoot.X * effectiveScale)+ EllipseGlass.Width / 2;
        //PanTransform.Y = -(pointerPositionInCaptureRoot.Y * effectiveScale)+ EllipseGlass.Height / 2;

        PanTransform.X = -(pointerPositionInCaptureRoot.X * effectiveScale) + GlassRoot.Width / 2;
        PanTransform.Y = -(pointerPositionInCaptureRoot.Y * effectiveScale) + GlassRoot.Height / 2;
    }

    private void MagnifyingGlass_Loaded(object sender, RoutedEventArgs e)
    {
        // RasterizationScale
        if (CaptureRoot?.XamlRoot != null)
        {
            _rasterizationScale = CaptureRoot.XamlRoot.RasterizationScale;
            CaptureRoot.XamlRoot.Changed += XamlRoot_Changed;
        }

        // Clip circulaire
        var visual = ElementCompositionPreview.GetElementVisual(GlassRoot);
        var compositor = visual.Compositor;

        var ellipse = compositor.CreateEllipseGeometry();

        ellipse.Center = new System.Numerics.Vector2(125, 125);
        ellipse.Radius = new System.Numerics.Vector2(125, 125);

        var clip = compositor.CreateGeometricClip();

        clip.Geometry = ellipse;

        visual.Clip = clip;
   }

}
