using System.Collections.ObjectModel;
using System.ComponentModel;
using ExcelPapy.Objects;
using System.Linq;


namespace ExcelPapy.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<RowViewModel> Rows { get; } = new();

    public ObservableCollection<ColumnHeaderViewModel> ColumnHeaders { get; } = new();
    public ObservableCollection<RowHeaderViewModel> RowHeaders { get; } = new();

    private CellViewModel? _selectionStart;
    public AppViewModel AppView { get; } = new();

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

    public void ApplyFontUnderlineToSelection()
    {
        bool anySelected = false;
        bool shouldUnderline = false;

        // Si au moins une cellule sélectionnée n'est pas soulignée, on souligne tout le groupe
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                {
                    anySelected = true;
                    if (!cell.FontUnderline)
                        shouldUnderline = true;
                }

        if (!anySelected) return;

        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                    cell.FontUnderline = shouldUnderline;
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

    public void ApplyBackgroundToSelection(Brush? background)
    {
        if (background == null) 
            return;

        Brush noir = new SolidColorBrush(Microsoft.UI.Colors.Black);

        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                {
                    cell.Background = background;

                    if (background != noir)
                    {
                        cell.BorderBrushTop = background;
                        cell.BorderBrushLeft = background;
                        cell.BorderBrushRight = background;
                        cell.BorderBrushBottom = background;
                    }
                }
    }

    public void ApplyAppPoliceToSelection(string policeKey)
    {
        AppView.FontApp = AppFonts.Resolve(policeKey);

        foreach (var col in ColumnHeaders)
            col.FontFamily = AppFonts.Resolve(policeKey);

        foreach (var row in RowHeaders)
            row.FontFamily = AppFonts.Resolve(policeKey);
    }

    private static readonly Brush BorderColorNoir = new SolidColorBrush(Microsoft.UI.Colors.Black);
    private const double EpaisseurBordure = 3;

    public void ApplyBorderToSelection(string borderType)
    {
        var selectedCells = Rows.SelectMany(r => r.Cells).Where(c => c.IsSelected).ToList();
        if (selectedCells.Count == 0)
            return;

        switch (borderType)
        {
            case "None":
                foreach (var c in selectedCells)
                {
                    SetTopBorder(c, 0);
                    SetLeftBorder(c, 0);
                    SetRightBorder(c, 0);
                    SetBottomBorder(c, 0);
                }
                break;

            case "All":
                foreach (var c in selectedCells)
                {
                    SetTopBorder(c, EpaisseurBordure);
                    SetLeftBorder(c, EpaisseurBordure);
                    SetRightBorder(c, EpaisseurBordure);
                    SetBottomBorder(c, EpaisseurBordure);
                }
                break;

            case "Top":
                foreach (var c in selectedCells)
                    SetTopBorder(c, EpaisseurBordure);
                break;

            case "Left":
                foreach (var c in selectedCells)
                    SetLeftBorder(c, EpaisseurBordure);
                break;

            case "Right":
                foreach (var c in selectedCells)
                    SetRightBorder(c, EpaisseurBordure);
                break;

            case "Bottom":
                foreach (var c in selectedCells)
                    SetBottomBorder(c, EpaisseurBordure);
                break;

            case "Outer":
                int rowMin = selectedCells.Min(c => c.Row);
                int rowMax = selectedCells.Max(c => c.Row);
                int colMin = selectedCells.Min(c => c.Column);
                int colMax = selectedCells.Max(c => c.Column);

                foreach (var c in selectedCells)
                {
                    if (c.Row == rowMin) SetTopBorder(c, EpaisseurBordure);
                    if (c.Row == rowMax) SetBottomBorder(c, EpaisseurBordure);
                    if (c.Column == colMin) SetLeftBorder(c, EpaisseurBordure);
                    if (c.Column == colMax) SetRightBorder(c, EpaisseurBordure);
                }
                break;
        }
    }

    private void SetTopBorder(CellViewModel cell, double thickness)
    {
        if (cell.Row > 0)
        {
            // Le "haut" visuel appartient au "bas" de la cellule du dessus
            var above = Rows[cell.Row - 1].Cells[cell.Column];
            above.BorderThicknessBottom = thickness;
            if (thickness > 0) above.BorderBrushBottom = BorderColorNoir;
        }
        else
        {
            cell.BorderThicknessTop = thickness;
            if (thickness > 0) cell.BorderBrushTop = BorderColorNoir;
        }
    }

    private void SetLeftBorder(CellViewModel cell, double thickness)
    {
        if (cell.Column > 0)
        {
            // Le "gauche" visuel appartient au "droit" de la cellule de gauche
            var left = Rows[cell.Row].Cells[cell.Column - 1];
            left.BorderThicknessRight = thickness;
            if (thickness > 0) left.BorderBrushRight = BorderColorNoir;
        }
        else
        {
            cell.BorderThicknessLeft = thickness;
            if (thickness > 0) cell.BorderBrushLeft = BorderColorNoir;
        }
    }

    private void SetRightBorder(CellViewModel cell, double thickness)
    {
        cell.BorderThicknessRight = thickness;
        if (thickness > 0) cell.BorderBrushRight = BorderColorNoir;
    }

    private void SetBottomBorder(CellViewModel cell, double thickness)
    {
        cell.BorderThicknessBottom = thickness;
        if (thickness > 0) cell.BorderBrushBottom = BorderColorNoir;
    }
    
}
