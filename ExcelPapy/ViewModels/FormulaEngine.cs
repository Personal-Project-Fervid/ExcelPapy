using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExcelPapy.ViewModels;

public static class FormulaEngine
{
    // Utilisation de \b au lieu de ^ et $ pour matcher les références au sein d'une formule globale
    private static readonly Regex CellRefRegex = new(@"\b([A-Z]+)(\d+)\b", RegexOptions.Compiled);
    private static readonly Regex RangeRegex = new(@"\b([A-Z]+\d+):([A-Z]+\d+)\b", RegexOptions.Compiled);

    private static readonly Regex FunctionRegex = new(
        @"\b(SOMME|SUM|MOYENNE|AVERAGE|MIN|MINIMUM|MAX|MAXIMUM|MÉDIANE|MEDIANE|MEDIAN|AUJOURD'HUI|TODAY|MAINTENANT|NOW)\s*\(([^()]*)\)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string ErrorValue = "Erreur";

    /// <summary>
    /// Indique si le texte saisi est une formule (commence par '=').
    /// </summary>
    public static bool IsFormula(string? text) => !string.IsNullOrEmpty(text) && text.StartsWith("=");

    /// <summary>
    /// Évalue la formule contenue dans currentCell.Value et retourne le résultat sous forme de texte.
    /// </summary>
    public static string Evaluate(CellViewModel currentCell, MainViewModel vm)
    {
        var raw = currentCell.Value?.Trim();
        if (!IsFormula(raw)) return raw ?? string.Empty;

        // Extraction du contenu sans le '='
        string expression = raw!.Substring(1).Trim();

        try
        {
            // 1. Évaluation récursive des fonctions : ex. SOMME(A1:A5) -> "15"
            expression = ResolveFunctions(expression, currentCell, vm);
            if (expression == ErrorValue) return ErrorValue;

            string[] formatsDates = { "yyyy-MM-dd", "dd-MM-yyyy", "yyyy-MM-dd HH:mm", "dd-MM-yyyy HH:mm"};
            if (DateTime.TryParseExact(expression, formatsDates, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                return expression;
            }



            // 2. Remplacement des références de cellules isolées : ex. A1 + A2 -> 5 + 10
            expression = ResolveCellReferences(expression, currentCell, vm);
            if (expression == ErrorValue) return ErrorValue;

            // 3. Évaluation mathématique globale avec DataTable
            double numericResult = EvaluateMathExpression(expression);

            if (double.IsNaN(numericResult) || double.IsInfinity(numericResult))
                return ErrorValue;

            return numericResult.ToString(CultureInfo.InvariantCulture);
        }
        catch
        {
            return ErrorValue;
        }
    }

    private static string ResolveFunctions(string expr, CellViewModel currentCell, MainViewModel vm)
    {
        while (FunctionRegex.IsMatch(expr))
        {
            expr = FunctionRegex.Replace(expr, match =>
            {
                string funcName = match.Groups[1].Value.ToUpperInvariant();
                string argsText = match.Groups[2].Value;

                if (funcName is "TODAY" or "AUJOURD'HUI")
                {
                    string format = funcName == "TODAY" ? "yyyy-MM-dd" : "dd-MM-yyyy";
                    return DateTime.Now.ToString(format);
                }

                if (funcName is "NOW" or "MAINTENANT")
                {
                    string format = funcName == "NOW" ? "yyyy-MM-dd HH:mm" : "dd-MM-yyyy HH:mm";
                    return DateTime.Now.ToString(format);
                }


                if (!TryResolveCellsAndValues(argsText, vm, currentCell, out var values)) return ErrorValue;

                double res = funcName switch
                {
                    "SOMME" or "SUM" => values.Sum(),
                    "MOYENNE" or "AVERAGE" => values.Count == 0 ? double.NaN : values.Average(),
                    "MIN" or "MINIMUM" => values.Count == 0 ? double.NaN : values.Min(),
                    "MAX" or "MAXIMUM" => values.Count == 0 ? double.NaN : values.Max(),
                    "MEDIANE" or "MÉDIANE" or "MEDIAN" => values.Count == 0 ? double.NaN : Median(values),
                    _ => double.NaN
                };

                if (double.IsNaN(res)) return ErrorValue;

                return res.ToString(CultureInfo.InvariantCulture);
            });

            if (expr.Contains(ErrorValue)) return ErrorValue;
        }

        return expr;
    }

    private static string ResolveCellReferences(string expr, CellViewModel currentCell, MainViewModel vm)
    {
        return CellRefRegex.Replace(expr, match =>
        {
            string cellRef = match.Value;

            if (!TryGetCellIndex(cellRef, out int col, out int row))
                return ErrorValue;

            var cell = GetCell(vm, row, col);
            if (cell == null || cell == currentCell) return ErrorValue; // Auto-référence ou hors-limites

            if (TryGetNumericValue(cell, vm, out double val))
            {
                // Formatage sécurisé pour DataTable (évite l'ambiguïté des signes négatifs)
                return val < 0
                    ? $"({val.ToString(CultureInfo.InvariantCulture)})"
                    : val.ToString(CultureInfo.InvariantCulture);
            }

            // Si la cellule est vide, on la traite comme 0
            return "0";
        });
    }

    private static double EvaluateMathExpression(string mathExpr)
    {
        using var dt = new DataTable();
        var result = dt.Compute(mathExpr, null);
        return Convert.ToDouble(result, CultureInfo.InvariantCulture);
    }

    private static double Median(List<double> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        int n = sorted.Count;
        return n % 2 == 1
            ? sorted[n / 2]
            : (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
    }

    /// <summary>
    /// Résout les arguments d'une fonction (cellules, plages ou valeurs brutes).
    /// </summary>
    private static bool TryResolveCellsAndValues(string argsText, MainViewModel vm, CellViewModel currentCell, out List<double> values)
    {
        values = new List<double>();
        if (string.IsNullOrWhiteSpace(argsText)) return true;

        var tokens = argsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var token in tokens)
        {
            // 1. Plage de cellules (ex: A1:A5)
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
                        if (cell == null || cell == currentCell) return false;

                        if (TryGetNumericValue(cell, vm, out double val))
                            values.Add(val);
                    }
                }
                continue;
            }

            // 2. Cellule unique (ex: A1)
            var singleMatch = CellRefRegex.Match(token);
            if (singleMatch.Success)
            {
                if (!TryGetCellIndex(token, out int col, out int row)) return false;
                var cell = GetCell(vm, row, col);
                if (cell == null || cell == currentCell) return false;

                if (TryGetNumericValue(cell, vm, out double val))
                    values.Add(val);
                continue;
            }

            // 3. Valeur numérique directe (ex: 10 ou 12.5)
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double directVal))
            {
                values.Add(directVal);
                continue;
            }

            return false;
        }

        return true;
    }

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

    private static int ColumnLettersToIndex(string letters)
    {
        int result = 0;
        foreach (char c in letters)
            result = result * 26 + (char.ToUpper(c) - 'A' + 1);
        return result - 1;
    }

    private static CellViewModel? GetCell(MainViewModel vm, int rowIndex, int colIndex)
    {
        if (rowIndex < 0 || rowIndex >= vm.Rows.Count) return null;
        var row = vm.Rows[rowIndex];
        if (colIndex < 0 || colIndex >= row.Cells.Count) return null;
        return row.Cells[colIndex];
    }

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
