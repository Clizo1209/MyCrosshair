namespace MyCrosshair;

public static class Loc
{
    private static bool IsEn => SettingsManager.Current.Language == "en";

    public static string Title            => IsEn ? "MyCrosshair Config"   : "MyCrosshair 配置";
    public static string SecColor         => IsEn ? "COLOR"                : "颜色";
    public static string SecShape         => IsEn ? "SHAPE"                : "形状";
    public static string SecOptions       => IsEn ? "OPTIONS"              : "选项";
    public static string CurrentColor     => IsEn ? "Current Color"        : "当前颜色";
    public static string Red              => IsEn ? "Red R"                : "红 R";
    public static string Green            => IsEn ? "Green G"              : "绿 G";
    public static string Blue             => IsEn ? "Blue B"               : "蓝 B";
    public static string Opacity          => IsEn ? "Opacity"              : "透明度";
    public static string Length           => IsEn ? "Length"               : "长度";
    public static string Thickness        => IsEn ? "Thickness"            : "粗细";
    public static string Gap              => IsEn ? "Gap"                  : "间隙";
    public static string ShowOutline      => IsEn ? "Show Outline"         : "显示轮廓";
    public static string OutlineThickness => IsEn ? "Outline Width"        : "轮廓粗细";
    public static string ShowDot          => IsEn ? "Show Center Dot"      : "显示中心点";
    public static string DotSize          => IsEn ? "Dot Size"             : "点大小";
    public static string TShape           => IsEn ? "T-Shape (no top arm)" : "T 形准星（无上臂）";
    public static string LangLabel        => IsEn ? "Language"             : "语言";
    public static string SaveConfig       => IsEn ? "Save Config"          : "保存配置";
    public static string Saved            => IsEn ? "Saved ✓"             : "已保存 ✓";
}
