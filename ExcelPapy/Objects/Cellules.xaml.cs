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

    private void OnRowResize(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (sender is Rectangle rect &&
            rect.DataContext is RowHeaderViewModel row)
        {
            row.Height = Math.Max(30, row.Height + e.Delta.Translation.Y);
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
}
