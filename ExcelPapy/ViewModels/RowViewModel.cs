using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ExcelPapy.ViewModels;

public class RowViewModel
{
    public ObservableCollection<CellViewModel> Cells { get; } = new();
}
