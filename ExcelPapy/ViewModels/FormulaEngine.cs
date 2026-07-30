using System.Globalization;
using System.Text.RegularExpressions;

namespace ExcelPapy.ViewModels;

public static class FormulaEngine
{
    private static readonly Regex CellRefRegex = new(@"^([A-Z]+)(\d+)$", RegexOptions.Compiled);
    private static readonly Regex RangeRegex = new(@"^([A-Z]+\d+):([A-Z]+\d+)$", RegexOptions.Compiled);
    private static readonly Regex FormulaRegex = new(@"^([A-ZÀ-Ÿ]+)\((.*)\)$", RegexOptions.Compiled);

    private const string ErrorValue = "Erreur";

    
    
    // Indique si le texte saisi est une formule (commence par '=').
    public static bool IsFormula(string? text) => !string.IsNullOrEmpty(text) && text.StartsWith("=");

    
    // Évalue la formule contenue dans currentCell.Value et retourne le résultat sous forme de texte.
    // Retourne "Erreur" si la formule est invalide ou si la cellule se référence elle-même
    // (directement ou via une plage).
    
    public static string Evaluate(CellViewModel currentCell, MainViewModel vm)
    {
        var raw = currentCell.Value?.Trim();
        if (!IsFormula(raw)) return raw ?? string.Empty;

        var body = raw!.Substring(1).Trim().ToUpperInvariant();

        var match = FormulaRegex.Match(body);
        if (!match.Success) return ErrorValue;

        string functionName = match.Groups[1].Value;
        string argsText = match.Groups[2].Value;

        if (!TryResolveCells(argsText, vm, out var cells)) return ErrorValue;

        // Auto-référence : la cellule ne doit pas apparaître dans ses propres arguments
        if (cells.Contains(currentCell)) return ErrorValue;

        var values = new List<double>();
        foreach (var cell in cells)
        {
            if (TryGetNumericValue(cell, vm, out double val))
                values.Add(val);
        }

        double result;
        switch (functionName)
        {
            case "SOMME":
            case "SUM":
                result = values.Sum();
                break;

            case "MOYENNE":
            case "AVERAGE":
                if (values.Count == 0) return ErrorValue;
                result = values.Average();
                break;

            case "MIN":
                if (values.Count == 0) return ErrorValue;
                result = values.Min();
                break;

            case "MAX":
                if (values.Count == 0) return ErrorValue;
                result = values.Max();
                break;

            case "MEDIANE":
            case "MÉDIANE":
            case "MEDIAN":
                if (values.Count == 0) return ErrorValue;
                result = Median(values);
                break;

            default:
                return ErrorValue;
        }

        return result.ToString(CultureInfo.InvariantCulture);
    }

    private static double Median(List<double> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        int n = sorted.Count;
        return n % 2 == 1
            ? sorted[n / 2]
            : (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
    }

    
    // Résout les arguments texte (ex: "A1:A5,C2") en liste de CellViewModel, sans doublons.
    // Retourne false si un argument est invalide ou hors limites.
    private static bool TryResolveCells(string argsText, MainViewModel vm, out List<CellViewModel> cells)
    {
        cells = new List<CellViewModel>();
        if (string.IsNullOrWhiteSpace(argsText)) return true;

        var tokens = argsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var token in tokens)
        {
            var rangeMatch = RangeRegex.Match(token);
            if (rangeMatch.Success)
            {
                if (!TryGetCellIndex(rangeMatch.Groups[1].Value, out int col1, out int row1)) return false;
                if (!TryGetCellIndex(rangeMatch.Groups[2].Value, out int col2, out int row2)) return false;

                int minRow = Math.Min(row1, row2), maxRow = Math.Max(row1, row2);
                int minCol = Math.Min(col1, col2), maxCol = Math.Max(col1, col2);

                for (int r = minRow; r <= maxRow; r++)
                {
                    for (int c = minCol; c <= maxCol; c++)
                    {
                        var cell = GetCell(vm, r, c);
                        if (cell == null) return false;
                        if (!cells.Contains(cell)) cells.Add(cell);
                    }
                }
                continue;
            }

            var singleMatch = CellRefRegex.Match(token);
            if (singleMatch.Success)
            {
                if (!TryGetCellIndex(token, out int col, out int row)) return false;
                var cell = GetCell(vm, row, col);
                if (cell == null) return false;
                if (!cells.Contains(cell)) cells.Add(cell);
                continue;
            }

            // Argument non reconnu (ni référence simple "A1", ni plage "A1:A5")
            return false;
        }

        return true;
    }

    
    // Convertit une référence de cellule (ex: "A1", "AB12") en index colonne/ligne base 0.
    
    private static bool TryGetCellIndex(string reference, out int colIndex, out int rowIndex)
    {
        colIndex = -1;
        rowIndex = -1;

        var match = CellRefRegex.Match(reference);
        if (!match.Success) return false;

        colIndex = ColumnLettersToIndex(match.Groups[1].Value);

        if (!int.TryParse(match.Groups[2].Value, out int rowNumber)) return false;
        rowIndex = rowNumber - 1;

        return colIndex >= 0 && rowIndex >= 0;
    }

    
    // Convertit des lettres de colonne (A, B, ..., Z, AA, AB, ...) en index base 0.
    
    private static int ColumnLettersToIndex(string letters)
    {
        int result = 0;
        foreach (char c in letters)
            result = result * 26 + (c - 'A' + 1);
        return result - 1;
    }

    
    // Récupère la CellViewModel à la position (ligne, colonne), ou null si hors limites.
    // Adapte cette méthode si la structure de MainViewModel diffère (Rows[].Cells[]).
    
    private static CellViewModel? GetCell(MainViewModel vm, int rowIndex, int colIndex)
    {
        if (rowIndex < 0 || rowIndex >= vm.Rows.Count) return null;
        var row = vm.Rows[rowIndex];
        if (colIndex < 0 || colIndex >= row.Cells.Count) return null;
        return row.Cells[colIndex];
    }

    
    // Récupère la valeur numérique d'une cellule. Si la cellule contient elle-même une formule,
    // elle est évaluée récursivement.
    // Note : les références circulaires indirectes (A1 dépend de B1 qui dépend de A1) ne sont pas
    // détectées ici, seule l'auto-référence directe/via plage l'est.
    
    private static bool TryGetNumericValue(CellViewModel cell, MainViewModel vm, out double value)
    {
        value = 0;
        var raw = cell.Value;

        if (IsFormula(raw))
        {
            var evaluated = Evaluate(cell, vm);
            return double.TryParse(evaluated, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        return double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
    }
}
