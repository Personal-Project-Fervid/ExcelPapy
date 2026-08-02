using System.Collections.ObjectModel;
using System.ComponentModel;
using ExcelPapy.Objects;

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
                    IsSelected = false,
                    Owner = this
                });
            Rows.Add(row);
        }

        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                cell.PropertyChanged += Cell_PropertyChanged;
    }
    private void Cell_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Quand une cellule change de valeur, les formules qui la référencent
        // ailleurs dans la grille doivent se rafraîchir.
        if (e.PropertyName == nameof(CellViewModel.Value))
            RefreshFormulas();
    }

    private void RefreshFormulas()
    {
        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (FormulaEngine.IsFormula(cell.Value))
                    cell.RaiseDisplayValueChanged();
    }

    private void Column_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ColumnHeaderViewModel.Width))
        {
            OnPropertyChanged(nameof(TotalColumnsWidth));

            // Rafraîchir la largeur des cellules
            foreach (var row in Rows)
            {
                foreach (var cell in row.Cells)
                {
                    // On force la mise à jour de la largeur pour que la grille s'adapte
                    cell.UpdateMergeUI();
                }
            }
        }
    }

    private void RowHeader_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RowHeaderViewModel.Height))
        {
            foreach (var row in Rows)
            {
                foreach (var cell in row.Cells)
                {
                    cell.UpdateMergeUI();
                }
            }
        }
    }

    public void SelectCell(CellViewModel cell, bool isShiftHeld)
    {
        // Arrêter l'édition de toutes les cellules
        foreach (var row in Rows)
            foreach (var c in row.Cells)
                c.IsEditing = false;

        CellViewModel effectiveCell = cell.IsMergedChild && cell.MasterCell != null ? cell.MasterCell : cell;

        if (!isShiftHeld)
        {
            ClearSelection();
            _selectionStart = effectiveCell;

            // Déterminer la plage complète de la cellule fusionnée (ou de la cellule simple)
            int rowMin = effectiveCell.Row;
            int rowMax = effectiveCell.Row + (effectiveCell.IsMergedMaster ? effectiveCell.MergeRowSpan - 1 : 0);
            int colMin = effectiveCell.Column;
            int colMax = effectiveCell.Column + (effectiveCell.IsMergedMaster ? effectiveCell.MergeColSpan - 1 : 0);

            foreach (var row in Rows)
                foreach (var c in row.Cells)
                    if (c.Row >= rowMin && c.Row <= rowMax &&
                        c.Column >= colMin && c.Column <= colMax)
                        c.IsSelected = true;

        }
        else if (_selectionStart != null)
        {
            // Ne PAS effacer _selectionStart, juste recalculer la zone
            ClearSelection();
            _selectionStart.IsSelected = true; // garder l'origine visible

            int startMasterRow = _selectionStart.IsMergedChild && _selectionStart.MasterCell != null ? _selectionStart.MasterCell.Row : _selectionStart.Row;
            int startMasterCol = _selectionStart.IsMergedChild && _selectionStart.MasterCell != null ? _selectionStart.MasterCell.Column : _selectionStart.Column;
            int startMasterRowMax = startMasterRow + (_selectionStart.IsMergedMaster ? _selectionStart.MergeRowSpan - 1 : 0);
            int startMasterColMax = startMasterCol + (_selectionStart.IsMergedMaster ? _selectionStart.MergeColSpan - 1 : 0);

            // Bornes de la cellule cliquée (en tenant compte de sa fusion éventuelle)
            int effectiveRowMax = effectiveCell.Row + (effectiveCell.IsMergedMaster ? effectiveCell.MergeRowSpan - 1 : 0);
            int effectiveColMax = effectiveCell.Column + (effectiveCell.IsMergedMaster ? effectiveCell.MergeColSpan - 1 : 0);

            int rowMin = Math.Min(_selectionStart.Row, cell.Row);
            int rowMax = Math.Max(_selectionStart.Row, cell.Row);
            int colMin = Math.Min(_selectionStart.Column, cell.Column);
            int colMax = Math.Max(_selectionStart.Column, cell.Column);

            foreach (var row in Rows)
                foreach (var c in row.Cells)
                {
                    int cRow = c.IsMergedChild && c.MasterCell != null ? c.MasterCell.Row : c.Row;
                    int cCol = c.IsMergedChild && c.MasterCell != null ? c.MasterCell.Column : c.Column;

                    if (c.Row >= rowMin && c.Row <= rowMax &&
                        c.Column >= colMin && c.Column <= colMax)
                        c.IsSelected = true;
                }
        }
        else
        {
            // Si _selectionStart est null et isShiftHeld, on traite comme un clic simple
            _selectionStart = effectiveCell;
            int rowMin = effectiveCell.Row;
            int rowMax = effectiveCell.Row + (effectiveCell.IsMergedMaster ? effectiveCell.MergeRowSpan - 1 : 0);
            int colMin = effectiveCell.Column;
            int colMax = effectiveCell.Column + (effectiveCell.IsMergedMaster ? effectiveCell.MergeColSpan - 1 : 0);

            foreach (var row in Rows)
                foreach (var c in row.Cells)
                    if (c.Row >= rowMin && c.Row <= rowMax &&
                        c.Column >= colMin && c.Column <= colMax)
                        c.IsSelected = true;
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

    private static readonly Windows.UI.Color CouleurGris = Microsoft.UI.Colors.FromARGB(0xFF, 0xBA, 0xBA, 0xBA);
    private static readonly Windows.UI.Color CouleurNoir = Microsoft.UI.Colors.Black;

    private static bool BrushHasColor(Brush brush, Windows.UI.Color color)
    {
        return brush is SolidColorBrush scb && scb.Color == color;
    }

    public void ApplyBackgroundToSelection(Brush? background)
    {
        if (background == null) 
            return;

        bool backgroundIsBlack = BrushHasColor(background, CouleurNoir);

        foreach (var row in Rows)
            foreach (var cell in row.Cells)
                if (cell.IsSelected)
                {
                    cell.Background = background;

                    if (!backgroundIsBlack)
                    {
                        if (!BrushHasColor(cell.BorderBrushTop, CouleurNoir))
                            cell.BorderBrushTop = background;
                        if (!BrushHasColor(cell.BorderBrushLeft, CouleurNoir))
                            cell.BorderBrushLeft = background;
                        if (!BrushHasColor(cell.BorderBrushRight, CouleurNoir))
                            cell.BorderBrushRight = background;
                        if (!BrushHasColor(cell.BorderBrushBottom, CouleurNoir))
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

    public void ApplyWeightAppToSelection(string weightKey)
    {
        AppView.WeightApp = weightKey;

        foreach (var col in ColumnHeaders)
            col.FontWeight = weightKey;


        foreach (var row in RowHeaders)
            row.FontWeight = weightKey;
    }

    private static readonly Brush BorderColorNoir = new SolidColorBrush(Microsoft.UI.Colors.Black);
    private static readonly Brush BorderColorGris = new SolidColorBrush(Microsoft.UI.Colors.FromARGB(0xFF, 0xBA, 0xBA, 0xBA));
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
                    SetTopBorder(c, EpaisseurBordure, BorderColorGris);
                    SetLeftBorder(c, EpaisseurBordure, BorderColorGris);
                    SetRightBorder(c, EpaisseurBordure, BorderColorGris);
                    SetBottomBorder(c, EpaisseurBordure, BorderColorGris);
                }
                break;

            case "All":
                foreach (var c in selectedCells)
                {
                    SetTopBorder(c, EpaisseurBordure, BorderColorNoir);
                    SetLeftBorder(c, EpaisseurBordure, BorderColorNoir);
                    SetRightBorder(c, EpaisseurBordure, BorderColorNoir);
                    SetBottomBorder(c, EpaisseurBordure, BorderColorNoir);
                }
                break;

            case "Top":
                int rMin = selectedCells.Min(c => c.Row);
                foreach (var c in selectedCells.Where(c => c.Row == rMin))
                    SetTopBorder(c, EpaisseurBordure, BorderColorNoir);
                break;

            case "Left":
                int cMin = selectedCells.Min(c => c.Column);
                foreach (var c in selectedCells.Where(c => c.Column == cMin))
                    SetLeftBorder(c, EpaisseurBordure, BorderColorNoir);
                break;

            case "Right":
                int cMax = selectedCells.Max(c => c.Column);
                foreach (var c in selectedCells.Where(c => c.Column == cMax))
                    SetRightBorder(c, EpaisseurBordure, BorderColorNoir);
                break;

            case "Bottom":
                int rMax = selectedCells.Max(c => c.Row);
                foreach (var c in selectedCells.Where(c => c.Row == rMax))
                    SetBottomBorder(c, EpaisseurBordure, BorderColorNoir);
                break;

            case "Outer":
                int rowMin = selectedCells.Min(c => c.Row);
                int rowMax = selectedCells.Max(c => c.Row);
                int colMin = selectedCells.Min(c => c.Column);
                int colMax = selectedCells.Max(c => c.Column);

                foreach (var c in selectedCells)
                {
                    if (c.Row == rowMin) SetTopBorder(c, EpaisseurBordure, BorderColorNoir);
                    if (c.Row == rowMax) SetBottomBorder(c, EpaisseurBordure, BorderColorNoir);
                    if (c.Column == colMin) SetLeftBorder(c, EpaisseurBordure, BorderColorNoir);
                    if (c.Column == colMax) SetRightBorder(c, EpaisseurBordure, BorderColorNoir);
                }
                break;
        }

        RecomputeAllCorners();
    }

    private void SetTopBorder(CellViewModel cell, double thickness, Brush ColorBorder)
    {
        int colspan = cell.IsMergedMaster ? cell.MergeColSpan : 1;

        for (int i = 0; i < colspan; i++)
        {
            int targetCol = cell.Column + i;

            if (cell.Row > 0)
            {
                // Le "haut" visuel appartient au "bas" de la cellule du dessus
                var above = Rows[cell.Row - 1].Cells[targetCol];
                above.BorderThicknessBottom = thickness;
                above.BorderBrushBottom = ColorBorder;
            }
            else
            {
                if(i==0)
                {
                    cell.BorderThicknessTop = thickness;
                    cell.BorderBrushTop = ColorBorder;
                }
            }
        }
    }

    private void SetLeftBorder(CellViewModel cell, double thickness, Brush ColorBorder)
    {
        int rowSpan = cell.IsMergedMaster ? cell.MergeRowSpan : 1;

        for (int i = 0; i < rowSpan; i++)
        {
            int targetRow = cell.Row + i;

            if (cell.Column > 0)
            {
                // Le "gauche" visuel appartient au "droit" de la cellule de gauche
                var left = Rows[targetRow].Cells[cell.Column - 1];
                left.BorderThicknessRight = thickness;
                left.BorderBrushRight = ColorBorder;
            }
            else
            {
                if (i == 0)
                {
                    cell.BorderThicknessLeft = thickness;
                    cell.BorderBrushLeft = ColorBorder;
                }
            }
        }
    }

    private void SetRightBorder(CellViewModel cell, double thickness, Brush ColorBorder)
    {
        if (cell.IsMergedChild && cell.MasterCell != null)
        {
            cell.MasterCell.BorderThicknessRight = thickness;
            cell.MasterCell.BorderBrushRight = ColorBorder;
        }
        else
        {
            cell.BorderThicknessRight = thickness;
            cell.BorderBrushRight = ColorBorder;
        }
    }

    private void SetBottomBorder(CellViewModel cell, double thickness, Brush ColorBorder)
    {
        if (cell.IsMergedChild && cell.MasterCell != null)
        {
            cell.MasterCell.BorderThicknessBottom = thickness;
            cell.MasterCell.BorderBrushBottom = ColorBorder;
        }
        else
        {
            cell.BorderThicknessBottom = thickness;
            cell.BorderBrushBottom = ColorBorder;
        }
    }

    private static readonly Brush TransparentBrush = new SolidColorBrush(Microsoft.UI.Colors.Transparent);

    // Recalcule les 4 coins de TOUTES les cellules en tenant compte
    // des bordures "empruntées" aux voisins (Top -> voisin du dessus, Left -> voisin de gauche)
    private void RecomputeAllCorners()
    {
        int rowCount = Rows.Count;
        int colCount = ColumnHeaders.Count;

        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < colCount; c++)
            {
                var cell = Rows[r].Cells[c];

                // Segment vertical au-dessus du point (propre bordure droite de la cellule)
                bool vertAbove = cell.BorderThicknessRight > 0 && IsBlack(cell.BorderBrushRight);

                // Segment vertical en dessous du point (bordure droite de la cellule du bas)
                bool vertBelow = false;
                if (r + 1 < rowCount)
                {
                    var below = Rows[r + 1].Cells[c];
                    vertBelow = below.BorderThicknessRight > 0 && IsBlack(below.BorderBrushRight);
                }

                // Segment horizontal à gauche du point (propre bordure basse de la cellule)
                bool horizLeft = cell.BorderThicknessBottom > 0 && IsBlack(cell.BorderBrushBottom);

                // Segment horizontal à droite du point (bordure basse de la cellule de droite)
                bool horizRight = false;
                if (c + 1 < colCount)
                {
                    var right = Rows[r].Cells[c + 1];
                    horizRight = right.BorderThicknessBottom > 0 && IsBlack(right.BorderBrushBottom);
                }

                bool hasVertical = vertAbove || vertBelow;
                bool hasHorizontal = horizLeft || horizRight;
                bool isIntersection = hasVertical && hasHorizontal;

                bool horizontalContinues = horizLeft && horizRight;
                bool verticalContinues = vertAbove && vertBelow;

                bool showCorner = isIntersection || horizontalContinues || verticalContinues;

                cell.CornerBottomRight = showCorner ? BorderColorNoir : TransparentBrush;
            }
        }
    }

    private static bool IsBlack(Brush brush)
    {
        return brush is SolidColorBrush scb &&
               scb.Color.A == 255 &&
               scb.Color.R == 0 &&
               scb.Color.G == 0 &&
               scb.Color.B == 0;
    }

    public void ToggleMergeSelection()
    {
        var selectedCells = Rows.SelectMany(r => r.Cells).Where(c => c.IsSelected).ToList();
        if (!selectedCells.Any()) return;

        int minRow = selectedCells.Min(c => c.Row);
        int maxRow = selectedCells.Max(c => c.Row);
        int minCol = selectedCells.Min(c => c.Column);
        int maxCol = selectedCells.Max(c => c.Column);

        var topLeftCell = Rows[minRow].Cells[minCol];
        int selectedRowCount = maxRow - minRow + 1;
        int selectedColCount = maxCol - minCol + 1;

        // 1. Vérifier si on doit DÉFUSIONNER (si la cellule maître correspond exactement à la sélection)
        if (topLeftCell.IsMergedMaster &&
            topLeftCell.MergeRowSpan == selectedRowCount &&
            topLeftCell.MergeColSpan == selectedColCount)
        {
            topLeftCell.IsMergedMaster = false;
            topLeftCell.MergeRowSpan = 1;
            topLeftCell.MergeColSpan = 1;

            foreach (var cell in selectedCells)
            {
                cell.IsMergedChild = false;
                cell.MasterCell = null;
                cell.UpdateMergeUI();
            }
            return;
        }

        // --- NOUVEAU CODE : NETTOYAGE D'UNE ANCIENNE FUSION DIFFÉRENTE ---
        // Si la cellule principale était déjà une cellule maîtresse mais avec des dimensions différentes
        if (topLeftCell.IsMergedMaster)
        {
            int oldRowSpan = topLeftCell.MergeRowSpan;
            int oldColSpan = topLeftCell.MergeColSpan;

            // On parcourt TOUTE l'ancienne zone fusionnée pour la réinitialiser
            for (int r = 0; r < oldRowSpan; r++)
            {
                for (int c = 0; c < oldColSpan; c++)
                {
                    int targetRow = topLeftCell.Row + r;
                    int targetCol = topLeftCell.Column + c;

                    // Vérification de sécurité pour ne pas déborder de la grille
                    if (targetRow < Rows.Count && targetCol < Rows[targetRow].Cells.Count)
                    {
                        var oldCell = Rows[targetRow].Cells[targetCol];

                        // On libère les anciennes cellules enfants (ex: C2:D3)
                        if (oldCell != topLeftCell)
                        {
                            oldCell.IsMergedChild = false;
                            oldCell.MasterCell = null;
                            oldCell.UpdateMergeUI(); // On force le rafraîchissement visuel pour qu'elles réapparaissent
                        }
                    }
                }
            }
        }

        // 2. FUSIONNER à la façon Microsoft Excel
        foreach (var cell in selectedCells)
        {
            cell.IsMergedMaster = false;
            cell.IsMergedChild = true;
            cell.MasterCell = topLeftCell;
            cell.MergeRowSpan = 1;
            cell.MergeColSpan = 1;

            // Excel vide les autres cellules et ne garde que la valeur de la cellule de référence
            if (cell != topLeftCell)
            {
                cell.Value = string.Empty;
            }
        }

        topLeftCell.IsMergedMaster = true;
        topLeftCell.IsMergedChild = false;
        topLeftCell.MasterCell = null;
        topLeftCell.MergeRowSpan = selectedRowCount;
        topLeftCell.MergeColSpan = selectedColCount;

        // Mettre à jour l'interface utilisateur pour toutes les cellules concernées
        foreach (var cell in selectedCells)
        {
            cell.UpdateMergeUI();
        }
    }

   

}
