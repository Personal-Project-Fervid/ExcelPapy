using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelPapy.ViewModels;

public partial class CellViewModel : ObservableObject
{
    [ObservableProperty]
    private string _value = string.Empty;

    public int Row { get; init; }
    public int Column { get; init; }

    public ColumnHeaderViewModel? ColumnHeader { get; init; }
    public RowHeaderViewModel? RowHeader { get; init; }


}
