using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelPapy.ViewModels;

public partial class ColumnHeaderViewModel : ObservableObject
{
    [ObservableProperty]
    private double _width = 200;

    public string Label { get; init; } = string.Empty;
}
