using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ExcelPapy.ViewModels;

public partial class MainViewModel
{
    public ObservableCollection<RowViewModel> Rows { get; } = new();
    public ObservableCollection<ColumnHeaderViewModel> ColumnHeaders { get; } = new();
    public ObservableCollection<RowHeaderViewModel> RowHeaders { get; } = new();

    public MainViewModel()
    {
        for (int c = 0; c < 26; c++)
            ColumnHeaders.Add(new ColumnHeaderViewModel{Label=((char)('A' + c)).ToString()});

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
                    RowHeader = rowHeader
                });
            Rows.Add(row);
        }
            
    }
}
