using Microsoft.UI.Xaml.Media;

namespace ExcelPapy.Objects;

public static class AppFonts
{
    public static readonly Dictionary<string, string> Paths = new()
    {
        ["Segoe UI"] = "Segoe UI",
        ["Fraunces"] = "ms-appx:///Assets/Fonts/Fraunces-VariableFont_SOFT,WONK,opsz,wght.ttf#Fraunces",
        ["Inter"] = "ms-appx:///Assets/Fonts/Inter-VariableFont_opsz,wght.ttf#Inter",
        ["LexendDeca"] = "ms-appx:///Assets/Fonts/LexendDeca-VariableFont_wght.ttf#LexendDeca",
        ["Nabla"] = "ms-appx:///Assets/Fonts/Nabla-Regular-VariableFont_EDPT,EHLT.ttf#Nabla",
        ["NotoSans"] = "ms-appx:///Assets/Fonts/NotoSans-VariableFont_wdht,wght.ttf#NotoSans",
        ["Rubik"] = "ms-appx:///Assets/Fonts/Rubik-VariableFont_wght.ttf#Rubik",
        ["ScienceGothic"] = "ms-appx:///Assets/Fonts/ScienceGothic-VariableFont_CTRS,slnt,wdht,wght.ttf#ScienceGothic",
        ["TiltWarp"] = "ms-appx:///Assets/Fonts/TiltWarp-Regular-VariableFont_XROT,YROT.ttf#TiltWarp",
    };

    public static FontFamily Resolve(string? key) =>
        key is not null && Paths.TryGetValue(key, out var path)
            ? new FontFamily(path)
            : new FontFamily("Segoe UI");
}
