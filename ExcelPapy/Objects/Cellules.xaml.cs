using ExcelPapy.ViewModels;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;

namespace ExcelPapy.Objects;

public sealed partial class Cellules : UserControl
{
    private bool _isSyncing = false;
    private Point _marqueeStartPoint;
    private CellViewModel? _dragEndCell = null;

    public Cellules()
    {
        this.InitializeComponent();
        this.DataContext = new MainViewModel();

        CellScrollViewer.ViewChanged += OnCellScrollChanged;
        HeaderScrollViewer.ViewChanged += OnHeaderScrollChanged;
        RowHeaderScrollViewer.ViewChanged += OnRowHeaderScrollChanged;

        HorizontalSlider.ValueChanged += OnHorizontalSliderValueChanged;
        VerticalSlider.ValueChanged += OnVerticalSliderValueChanged;

        this.Loaded += (s, e) =>
        {
            AdjustScrollViewersAlignment();
            UpdateSlidersFromScroll();
        };

        this.SizeChanged += (s, e) =>
        {
            AdjustScrollViewersAlignment();
            UpdateSlidersFromScroll();
        };
    }

    public void SetViewModel(MainViewModel vm)
    {
        this.DataContext = vm;
    }

    private DispatcherTimer _resizeTimer;

    private void OnRowResize(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (sender is Rectangle rect && rect.DataContext is RowHeaderViewModel row)
        {
            row.Height = Math.Max(30, row.Height + e.Delta.Translation.Y);
        }
    }

    private void OnRowResizeCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        AdjustScrollViewersAlignment();
        UpdateSlidersFromScroll();
    }

    private void OnColumnResize(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (sender is Rectangle rect && rect.DataContext is ColumnHeaderViewModel column)
        {
            column.Width = Math.Max(50, column.Width + e.Delta.Translation.X);
        }
    }

    private void OnColumnResizeCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        AdjustScrollViewersAlignment();
        UpdateSlidersFromScroll();
    }


    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr SetCursor(IntPtr hCursor);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

    private const int IDC_SIZEWE = 32644;
    private const int IDC_SIZENS = 32645;
    private const int IDC_ARROW = 32512;

    private void OnColResizePointerEntered(object sender, PointerRoutedEventArgs e)
        => SetCursor(LoadCursor(IntPtr.Zero, IDC_SIZEWE));

    private void OnRowResizePointerEntered(object sender, PointerRoutedEventArgs e)
        => SetCursor(LoadCursor(IntPtr.Zero, IDC_SIZENS));

    private void OnResizePointerExited(object sender, PointerRoutedEventArgs e)
        => SetCursor(LoadCursor(IntPtr.Zero, IDC_ARROW));
    private void OnCellScrollChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (_isSyncing) return;
        _isSyncing = true;

        HeaderScrollViewer.ChangeView(CellScrollViewer.HorizontalOffset, null, null, true);
        RowHeaderScrollViewer.ChangeView(null, CellScrollViewer.VerticalOffset, null, true);

        UpdateSlidersFromScroll();

        _isSyncing = false;
    }

    // Quand l'en-tête colonne scrolle → sync les cellules
    private void OnHeaderScrollChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (_isSyncing) return;
        _isSyncing = true;

        CellScrollViewer.ChangeView(HeaderScrollViewer.HorizontalOffset, null, null, true);

        UpdateSlidersFromScroll();

        _isSyncing = false;
    }

    // Quand l'en-tête ligne scrolle → sync les cellules
    private void OnRowHeaderScrollChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (_isSyncing) return;
        _isSyncing = true;

        CellScrollViewer.ChangeView(null, RowHeaderScrollViewer.VerticalOffset, null, true);

        UpdateSlidersFromScroll();

        _isSyncing = false;
    }

    private void UpdateSlidersFromScroll()
    {
        if (HorizontalSlider != null)
        {
            HorizontalSlider.Maximum = Math.Max(0, CellScrollViewer.ScrollableWidth);
            HorizontalSlider.Value = CellScrollViewer.HorizontalOffset;
        }

        if (VerticalSlider != null)
        {
            VerticalSlider.Maximum = Math.Max(0, CellScrollViewer.ScrollableHeight);
            VerticalSlider.Value = CellScrollViewer.VerticalOffset;
        }
    }

    private void OnHorizontalSliderValueChanged(object sender, double value)
    {
        if (_isSyncing) return;
        _isSyncing = true;

        CellScrollViewer.ChangeView(value, null, null, true);
        HeaderScrollViewer.ChangeView(value, null, null, true);

        _isSyncing = false;
    }

    private void OnVerticalSliderValueChanged(object sender, double value)
    {
        if (_isSyncing) return;
        _isSyncing = true;

        CellScrollViewer.ChangeView(null, value, null, true);
        RowHeaderScrollViewer.ChangeView(null, value, null, true);

        _isSyncing = false;
    }

    private void AdjustScrollViewersAlignment()
    {
        // Réinitialiser les marges avant de mesurer pour éviter la boucle de rétroaction
        HeaderScrollViewer.Margin = new Thickness(0);
        RowHeaderScrollViewer.Margin = new Thickness(0);

        // Forcer une mesure à jour avant de lire les ViewportWidth/Height
        HeaderScrollViewer.UpdateLayout();
        RowHeaderScrollViewer.UpdateLayout();
        CellScrollViewer.UpdateLayout();

        // Largeur du viewport des cellules vs largeur du viewport des en-têtes
        double cellViewportWidth = CellScrollViewer.ViewportWidth;
        double headerViewportWidth = HeaderScrollViewer.ViewportWidth;
        double diffx = headerViewportWidth - cellViewportWidth;
        HeaderScrollViewer.Margin = new Thickness(0, 0, Math.Max(0, diffx), 0);

        double cellViewportHeight = CellScrollViewer.ViewportHeight;
        double rowHeaderViewportHeight = RowHeaderScrollViewer.ViewportHeight;
        double diffy = rowHeaderViewportHeight - cellViewportHeight;
        RowHeaderScrollViewer.Margin = new Thickness(0, 0, 0, Math.Max(0, diffy));
    }

    private bool _isDragging = false;
    private CellViewModel? _lastClickedCell = null;
    private DateTime _lastClickTime = DateTime.MinValue;
    private CellViewModel? _dragStartCell = null;
    private bool _hasDraggedToOtherCell = false;

    private void OnCellPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement el && el.DataContext is CellViewModel cell)
        {
            var vm = this.DataContext as MainViewModel;

            // Vérifier si c'est un double-clic (clic rapide sur la même cellule)
            bool isDoubleClick = false;
            if (_lastClickedCell == cell && (DateTime.Now - _lastClickTime).TotalMilliseconds < 300)
            {
                isDoubleClick = true;
            }

            _lastClickedCell = cell;
            _lastClickTime = DateTime.Now;

            if (isDoubleClick)
            {
                // Mode édition
                cell.IsEditing = true;
                // Enlever la sélection quand on rentre en mode édition
                cell.IsSelected = false;

                // Chercher la TextBox et la focus
                if (el is Border border)
                {
                    var grid = border.Child as Grid;
                    if (grid != null)
                    {
                        foreach (var child in grid.Children)
                        {
                            if (child is TextBox textBox)
                            {
                                // Activer la TextBox pour l'édition
                                textBox.IsReadOnly = false;
                                textBox.IsHitTestVisible = true;
                                textBox.Focus(FocusState.Programmatic);
                                textBox.SelectAll();
                                break;
                            }
                        }
                    }
                }
                e.Handled = true;
            }
            else
            {
                // Initialiser le drag
                _dragStartCell = cell;
                _hasDraggedToOtherCell = false;

                // Désactiver tous les TextBox en édition
                vm?.DisableAllEditing();

                // Mettre le point de départ de la sélection sur la cellule cliquée
                vm?.SetSelectionStart(cell);
                vm?.ClearSelection();
                //cell.IsSelected = true;

                // Capturer le pointeur pour recevoir PointerMoved hors de l'élément
                (sender as UIElement)?.CapturePointer(e.Pointer);
                _isDragging = true;

                this.Focus(FocusState.Programmatic);

                ShowMarquee(e.GetCurrentPoint(CellScrollViewer).Position);

                e.Handled = true;
            }
        }
    }

    private void OnCellPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging) return;

        var vm = this.DataContext as MainViewModel;
        if (vm == null || _dragStartCell == null) return;

        // Position relative au ScrollViewer des cellules
        var point = e.GetCurrentPoint(CellScrollViewer).Position;

        UpdateMarquee(point);

        // Position absolue dans le contenu scrollable
        double absX = point.X + CellScrollViewer.HorizontalOffset;
        double absY = point.Y + CellScrollViewer.VerticalOffset;

        // Trouver la colonne sous le curseur
        double cumX = 0;
        int targetCol = -1;
        for (int c = 0; c < vm.ColumnHeaders.Count; c++)
        {
            cumX += vm.ColumnHeaders[c].Width;
            if (absX < cumX)
            {
                targetCol = c;
                break;
            }
        }

        // Trouver la ligne sous le curseur
        double cumY = 0;
        int targetRow = -1;
        for (int r = 0; r < vm.RowHeaders.Count; r++)
        {
            cumY += vm.RowHeaders[r].Height;
            if (absY < cumY)
            {
                targetRow = r;
                break;
            }
        }

        if (targetRow >= 0 && targetCol >= 0)
        {
            var cell = vm.Rows[targetRow].Cells[targetCol];

            // Vérifier si on a glissé vers une cellule différente
            if (cell != _dragStartCell)
            {
                _hasDraggedToOtherCell = true;
            }

            // Appliquer la sélection par plage (du point de départ au curseur)
            _dragEndCell = cell;
        }
    }

    private void OnCellPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        HideMarquee();

        if (_isDragging && _dragStartCell != null)
        {
            var vm = this.DataContext as MainViewModel;

            // Si on n'a pas glissé vers une autre cellule, c'est un clic simple
            if (!_hasDraggedToOtherCell)
            {
                // Effacer la sélection précédente et sélectionner uniquement cette cellule
                var shiftState = Microsoft.UI.Input.InputKeyboardSource
                    .GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
                bool isShift = ((int)shiftState & 1) != 0;

                // Si pas de Shift, enlever toute sélection sauf la cellule cliquée
                if (!isShift)
                {
                    vm?.ClearSelection();
                    _dragStartCell.IsSelected = true;
                }
                else
                {
                    vm?.SelectCell(_dragStartCell, isShift);
                }
            }
            else if (_dragEndCell != null)
            {
                // Drag terminé : appliquer la sélection de la plage entière en une fois
                vm?.ClearSelection();
                vm?.SelectCell(_dragEndCell, isShiftHeld: true);
            }
        }

        _isDragging = false;
        _dragStartCell = null;
        _dragEndCell = null;
        _hasDraggedToOtherCell = false;
        (sender as UIElement)?.ReleasePointerCapture(e.Pointer);
    }

    private void ShowMarquee(Point start)
    {
        _marqueeStartPoint = start;
        MarqueeRect.Visibility = Visibility.Visible;
        UpdateMarquee(start);
    }

    private void UpdateMarquee(Point current)
    {
        double left = Math.Min(_marqueeStartPoint.X, current.X);
        double top = Math.Min(_marqueeStartPoint.Y, current.Y);
        double width = Math.Abs(current.X - _marqueeStartPoint.X);
        double height = Math.Abs(current.Y - _marqueeStartPoint.Y);

        Canvas.SetLeft(MarqueeRect, left);
        Canvas.SetTop(MarqueeRect, top);
        MarqueeRect.Width = width;
        MarqueeRect.Height = height;
    }

    private void HideMarquee()
    {
        MarqueeRect.Visibility = Visibility.Collapsed;
    }

    private void OnTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.DataContext is CellViewModel cell)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                // Désactiver la TextBox
                textBox.IsReadOnly = true;
                textBox.IsHitTestVisible = false;
                cell.IsEditing = false;
                // Retirer le focus de la TextBox
                this.Focus(FocusState.Programmatic);
                e.Handled = true;
            }
            else if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // Désactiver la TextBox
                textBox.IsReadOnly = true;
                textBox.IsHitTestVisible = false;
                cell.IsEditing = false;
                // Retirer le focus de la TextBox
                this.Focus(FocusState.Programmatic);
                e.Handled = true;
            }
        }
    }

    private DateTime _textBoxPointerPressedTime = DateTime.MinValue;
    private bool _textBoxDragDetected = false;

    private void OnTextBoxPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.DataContext is CellViewModel cell && cell.IsEditing)
        {
            // Mémoriser l'heure du clic sur la TextBox
            _textBoxPointerPressedTime = DateTime.Now;
            _textBoxDragDetected = false;
            // Capturer le pointeur pour détecter le mouvement
            textBox.CapturePointer(e.Pointer);
        }
    }

    private void OnTextBoxPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.DataContext is CellViewModel cell && cell.IsEditing)
        {
            // Si l'utilisateur commence à faire un drag (> 50ms depuis le PointerPressed)
            if (!_textBoxDragDetected && (DateTime.Now - _textBoxPointerPressedTime).TotalMilliseconds > 50)
            {
                _textBoxDragDetected = true;

                // Quitter le mode édition pour permettre le drag
                textBox.IsReadOnly = true;
                textBox.IsHitTestVisible = false;
                cell.IsEditing = false;

                // Initialiser le drag manuellement
                _dragStartCell = cell;
                _hasDraggedToOtherCell = false;

                var vm = this.DataContext as MainViewModel;
                vm?.DisableAllEditing();
                vm?.SetSelectionStart(cell);
                vm?.ClearSelection();
                //cell.IsSelected = true;

                // Obtenir la Border parent et capturer le pointeur dessus
                var grid = VisualTreeHelper.GetParent(textBox) as Grid;
                var border = VisualTreeHelper.GetParent(grid) as Border;

                if (border != null)
                {
                    textBox.ReleasePointerCapture(e.Pointer);
                    border.CapturePointer(e.Pointer);
                    _isDragging = true;

                    ShowMarquee(e.GetCurrentPoint(CellScrollViewer).Position);
                }

                e.Handled = true;
            }
        }
    }
}
