using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ExcelPapy.ViewModels;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ExcelPapy.Objects;

public sealed partial class Cellules : UserControl
{
    private bool _isSyncing = false;

    public Cellules()
    {
        this.InitializeComponent();
        this.DataContext = new MainViewModel();

        CellScrollViewer.ViewChanged += OnCellScrollChanged;
        HeaderScrollViewer.ViewChanged += OnHeaderScrollChanged;
        RowHeaderScrollViewer.ViewChanged += OnRowHeaderScrollChanged;
    }

    private void OnColumnResize(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (sender is Rectangle rect &&
            rect.DataContext is ColumnHeaderViewModel col)
        {
            col.Width = Math.Max(50, col.Width + e.Delta.Translation.X);
        }
    }

    private DispatcherTimer _resizeTimer;

    private void OnRowResize(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (sender is Rectangle rect &&
            rect.DataContext is RowHeaderViewModel row)
        {
            row.Height = Math.Max(30, row.Height + e.Delta.Translation.Y);

            // Initialiser ou réinitialiser le timer pour mettre à jour le layout après la fin du redimensionnement
            if (_resizeTimer == null)
            {
                _resizeTimer = new DispatcherTimer();
                _resizeTimer.Interval = TimeSpan.FromMilliseconds(150);
                _resizeTimer.Tick += async (s, args) =>
                {
                    _resizeTimer.Stop();

                    // Forcer une recréation complète en vidant et repeuplan la collection
                    var vm = this.DataContext as MainViewModel;
                    if (vm != null)
                    {
                        // Sauvegarder les données
                        var rowsData = vm.Rows.ToList();

                        // Vider complètement les collections
                        vm.Rows.Clear();

                        // Attendre que le layout se vide
                        await Task.Delay(50);

                        // Re-ajouter tous les items
                        foreach (var row in rowsData)
                        {
                            vm.Rows.Add(row);
                        }

                        // Réinitialiser le scroll à la position 0,0
                        _isSyncing = true;
                        CellScrollViewer.ChangeView(0, 0, null, false);
                        RowHeaderScrollViewer.ChangeView(null, 0, null, false);
                        HeaderScrollViewer.ChangeView(0, null, null, false);
                        _isSyncing = false;
                    }
                };
            }
            else
            {
                _resizeTimer.Stop();
            }

            _resizeTimer.Start();
        }
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

        _isSyncing = false;
    }

    // Quand l'en-tête colonne scrolle → sync les cellules
    private void OnHeaderScrollChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (_isSyncing) return;
        _isSyncing = true;

        CellScrollViewer.ChangeView(HeaderScrollViewer.HorizontalOffset, null, null, true);

        _isSyncing = false;
    }

    // Quand l'en-tête ligne scrolle → sync les cellules
    private void OnRowHeaderScrollChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (_isSyncing) return;
        _isSyncing = true;

        CellScrollViewer.ChangeView(null, RowHeaderScrollViewer.VerticalOffset, null, true);

        _isSyncing = false;
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
                cell.IsSelected = true;

                // Capturer le pointeur pour recevoir PointerMoved hors de l'élément
                (sender as UIElement)?.CapturePointer(e.Pointer);
                _isDragging = true;

                e.Handled = true;
            }
        }
    }

    private void OnCellPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging) return;

        var vm = this.DataContext as MainViewModel;
        if (vm == null || _dragStartCell == null) return;

        // Retirer le focus du TextBox actif
        this.Focus(FocusState.Programmatic);

        // Position relative au ScrollViewer des cellules
        var point = e.GetCurrentPoint(CellScrollViewer).Position;

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
            vm.SelectCell(cell, isShiftHeld: true);
        }
    }

    private void OnCellPointerReleased(object sender, PointerRoutedEventArgs e)
    {
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
        }

        _isDragging = false;
        _dragStartCell = null;
        _hasDraggedToOtherCell = false;
        (sender as UIElement)?.ReleasePointerCapture(e.Pointer);
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
                cell.IsSelected = true;

                // Obtenir la Border parent et capturer le pointeur dessus
                var grid = VisualTreeHelper.GetParent(textBox) as Grid;
                var border = VisualTreeHelper.GetParent(grid) as Border;

                if (border != null)
                {
                    textBox.ReleasePointerCapture(e.Pointer);
                    border.CapturePointer(e.Pointer);
                    _isDragging = true;
                }

                e.Handled = true;
            }
        }
    }
}
