using System.Collections.ObjectModel;
using System.ComponentModel;


namespace ExcelPapy.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<RowViewModel> Rows { get; } = new();

    public ObservableCollection<ColumnHeaderViewModel> ColumnHeaders { get; } = new();
    public ObservableCollection<RowHeaderViewModel> RowHeaders { get; } = new();

    private CellViewModel? _selectionStart;

    public double TotalColumnsWidth
    {
        get
        {
            double total = 0;
            foreach (var col in ColumnHeaders)
            {
                total += col.Width;
            }
            return total;
        }
    }

    public MainViewModel()
    {
        for (int c = 0; c < 26; c++)
        {
            var column = new ColumnHeaderViewModel
            {
                Label = ((char)('A' + c)).ToString()
            };

            // On écoute les changements de largeur
            column.PropertyChanged += Column_PropertyChanged;

            ColumnHeaders.Add(column);
        }

        // Créer une grille 100 x 26 (colonnes A-Z)
        for (int r = 0; r < 100; r++)
        {
            var rowHeader = new RowHeaderViewModel { Label = (r + 1).ToString() };
            RowHeaders.Add(rowHeader);
            var row = new RowViewModel();
            for (int c = 0; c < 26; c++)
                row.Cells.Add(new CellViewModel
                {
                    Row = r,
                    Column = c,
                    ColumnHeader = ColumnHeaders[c],
                    RowHeader = rowHeader,
                    IsEditing = false,
                    IsSelected = false
                });
            Rows.Add(row);
        }
    }

    private void Column_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ColumnHeaderViewModel.Width))
        {
            OnPropertyChanged(nameof(TotalColumnsWidth));
        }
    }

    public void SelectCell(CellViewModel cell, bool isShiftHeld)
    {
        // Arrêter l'édition de toutes les cellules
        foreach (var row in Rows)
            foreach (var c in row.Cells)
                c.IsEditing = false;

        if (!isShiftHeld)
        {
            ClearSelection();
            _selectionStart = cell;
            cell.IsSelected = true;
        }
        else if (_selectionStart != null)
        {
            // Ne PAS effacer _selectionStart, juste recalculer la zone
            ClearSelection();
            _selectionStart.IsSelected = true; // garder l'origine visible

            int rowMin = Math.Min(_selectionStart.Row, cell.Row);
            int rowMax = Math.Max(_selectionStart.Row, cell.Row);
            int colMin = Math.Min(_selectionStart.Column, cell.Column);
            int colMax = Math.Max(_selectionStart.Column, cell.Column);

            foreach (var row in Rows)
                foreach (var c in row.Cells)
                    if (c.Row >= rowMin && c.Row <= rowMax &&
                        c.Column >= colMin && c.Column <= colMax)
                        c.IsSelected = true;
        }
        else
        {
            // Si _selectionStart est null et isShiftHeld, on traite comme un clic simple
            _selectionStart = cell;
            cell.IsSelected = true;
        }

        // Recalculer les bordures de sélection
        UpdateSelectionBorders();
    }

    public void ClearSelection()
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                cell.IsSelected = false;

        // Réinitialiser aussi les bordures
        UpdateSelectionBorders();
    }

    public void SetSelectionStart(CellViewModel cell)
    {
        _selectionStart = cell;
    }

    public void DisableAllEditing()
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                cell.IsEditing = false;
    }

    private void UpdateSelectionBorders()
    {
        // Réinitialiser toutes les bordures
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                cell.SelectionBorderThickness = new Microsoft.UI.Xaml.Thickness(0);

        // Calculer les bordures pour les cellules sélectionnées
        foreach (var row in Rows)
        {
            foreach (var cell in row.Cells)
            {
                if (cell.IsSelected)
                {
                    double top = 3;
                    double left = 3;
                    double right = 3;
                    double bottom = 3;

                    // Vérifier les cellules adjacentes
                    // Haut
                    if (cell.Row > 0 && Rows[cell.Row - 1].Cells[cell.Column].IsSelected)
                        top = 0;

                    // Bas
                    if (cell.Row < Rows.Count - 1 && Rows[cell.Row + 1].Cells[cell.Column].IsSelected)
                        bottom = 0;

                    // Gauche
                    if (cell.Column > 0 && Rows[cell.Row].Cells[cell.Column - 1].IsSelected)
                        left = 0;

                    // Droite
                    if (cell.Column < ColumnHeaders.Count - 1 && Rows[cell.Row].Cells[cell.Column + 1].IsSelected)
                        right = 0;

                    cell.SelectionBorderThickness = new Microsoft.UI.Xaml.Thickness(left, top, right, bottom);
                }
            }
        }
    }

    public void ApplyFontFamilyToSelection(string fontFamily)
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                    cell.FontFamily = fontFamily;
    }

    public void ApplyFontSizeToSelection(double fontSize)
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                    cell.FontSize = fontSize;
    }

    public void ApplyForegroundToSelection(Brush brush)
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                    cell.Foreground = brush;
    }

    public void ApplyFontWeightToSelection()
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                {
                    if(cell.IsBold)
                        cell.FontWeight = "Normal";
                    else
                        cell.FontWeight = "Bold";

                    cell.IsBold = !cell.IsBold;
                }
    }

    public void ApplyFontStyleToSelection()
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                {
                    if(cell.IsItalic)
                        cell.FontStyle = "Normal";
                    else
                        cell.FontStyle = "Italic";

                    cell.IsItalic = !cell.IsItalic;
                }
    }

    public void ApplyVerticalAlignmentToSelection(string VerticalAlignment)
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                    cell.VerticalAlignment = VerticalAlignment;
    }

    public void ApplyHorizontalAlignmentToSelection(string HorizontalAlignment)
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                    cell.HorizontalAlignment = HorizontalAlignment;
    }

    public void ApplyBackgroundToSelection(Brush background)
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                    cell.Background = background;
    }
}
