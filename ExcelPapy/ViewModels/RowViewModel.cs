using System.Collections.ObjectModel;
namespace ExcelPapy.ViewModels;

public class RowViewModel
{
    public ObservableCollection<CellViewModel> Cells { get; } = new();
}
