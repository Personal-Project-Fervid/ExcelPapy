using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelPapy.ViewModels;

public partial class RowHeaderViewModel : ObservableObject
{
    [ObservableProperty]
    private double _height = 100;
    public string Label { get; init; } = string.Empty;
}
