using System.Diagnostics;
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
        MagnifyingGlass.SetViewModel(ViewModel);
        AppPersonalization.SetViewModel(ViewModel);
        AppPersonalization.SetTutorial(tutorial);
        tutorial.SetViewModel(ViewModel);

        TextPersonalization.CaptureRoot = RootGrid;
        CellPersonalization.CaptureRoot = RootGrid;
        MagnifyingGlass.CaptureRoot = RootGrid;

        RootGrid.PointerMoved += RootGrid_PointerMoved;

        _timer.Interval = TimeSpan.FromMilliseconds(100); // fréquence de vérification
        _timer.Tick += CheckKeyState;
        _timer.Start();
    }

    

    private readonly DispatcherTimer _timer = new DispatcherTimer();
    private Stopwatch _stopwatch = new Stopwatch();
    private bool _isKeyDown = false;
    private bool _isCursorHidden = false;
    private const int LongPressThreshold = 500;
    private bool _oneShot = false;

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
                AppPersonalization.setKeyDown(true);

                if (!_oneShot)
                _ = MagnifyingGlass.CaptureAppAsync();

                MagnifyingGlass.Visibility = Visibility.Visible;
                MagnifyingGlassRectangle.Visibility = Visibility.Visible;
                MagnifyingGlassRectangle.IsHitTestVisible = true;
                _oneShot = true;

                if (!_isCursorHidden)
                {
                    MagnifyingGlass.HideCursor();
                    _isCursorHidden = true;
                }
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
            MagnifyingGlassRectangle.Visibility = Visibility.Collapsed;
            MagnifyingGlassRectangle.IsHitTestVisible = false;
            _oneShot = false;

            if (_isCursorHidden)
            {
                MagnifyingGlass.ShowCursorBack();
                _isCursorHidden = false;
            }
        }
    }

    private void RootGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        MagnifyingGlass.UpdatePointer(
            e.GetCurrentPoint(RootGrid).Position);
    }
}
