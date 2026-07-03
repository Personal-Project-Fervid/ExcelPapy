using System.Diagnostics;
using System.Runtime.InteropServices;
using ExcelPapy.Objects;
using ExcelPapy.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace ExcelPapy.Presentation;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new();
    public MainPage()
    {
        this.InitializeComponent();
        Cellules.SetViewModel(ViewModel);
        TextPersonalization.SetViewModel(ViewModel);
        CellPersonalization.SetViewModel(ViewModel);

        TextPersonalization.CaptureRoot = RootGrid;
        CellPersonalization.CaptureRoot = RootGrid;

        RootGrid.PointerMoved += RootGrid_PointerMoved;

        _timer.Interval = TimeSpan.FromMilliseconds(100); // fréquence de vérification
        _timer.Tick += CheckKeyState;
        _timer.Start();
    }

    [DllImport("user32.dll")]
    private static extern int ShowCursor(bool bShow);

    private readonly DispatcherTimer _timer = new DispatcherTimer();
    private Stopwatch _stopwatch = new Stopwatch();
    private bool _isKeyDown = false;
    private bool _isCursorHidden = false;
    private const int LongPressThreshold = 500;

    private void CheckKeyState(object? sender, object e)
    {
        var state = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.A);
        bool isDown = state.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        if (isDown && !_isKeyDown)
        {
            // Vient d'être enfoncée
            _isKeyDown = true;
            _stopwatch.Restart();
        }
        else if (isDown && _isKeyDown)
        {
            // Maintenue : vérifier si le seuil est atteint
            if (_stopwatch.ElapsedMilliseconds >= LongPressThreshold)
            {
                TextPersonalization.setKeyDown(true);
                CellPersonalization.setKeyDown(true);

                MagnifyingGlass.Visibility = Visibility.Visible;
                UpdateMagnifyingGlassPosition();
                HideCursor();
            }
        }
        else if (!isDown && _isKeyDown)
        {
            // Vient d'être relâchée
            TextPersonalization.setKeyDown(false);
            CellPersonalization.setKeyDown(false);

            _isKeyDown = false;
            _stopwatch.Stop();
            MagnifyingGlass.Visibility = Visibility.Collapsed;
            ShowCursorBack();
        }
    }

    private void HideCursor()
    {
        if (!_isCursorHidden)
        {
            ShowCursor(false);
            _isCursorHidden = true;
        }
    }

    private void ShowCursorBack()
    {
        if (_isCursorHidden)
        {
            ShowCursor(true);
            _isCursorHidden = false;
        }
    }

    private Windows.Foundation.Point _pointerPosition;

    private void RootGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        _pointerPosition = e.GetCurrentPoint(RootGrid).Position;

        if (MagnifyingGlass.Visibility == Visibility.Visible)
        {
            UpdateMagnifyingGlassPosition();
        }
    }

    private void UpdateMagnifyingGlassPosition()
    {
        // Centrer l'objet sur le curseur (ajustez l'offset selon vos besoins)
        MagnifyingGlassTransform.X = _pointerPosition.X - (MagnifyingGlass.Width / 2);
        MagnifyingGlassTransform.Y = _pointerPosition.Y - (MagnifyingGlass.Height / 2);
    }
}
