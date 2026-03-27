namespace MyCrosshair;

public class CrosshairSettings
{
    public int Length { get; set; } = 10;
    public int Thickness { get; set; } = 2;
    public int Gap { get; set; } = 3;
    public bool ShowOutline { get; set; } = true;
    public int OutlineThickness { get; set; } = 1;
    public byte R { get; set; } = 0;
    public byte G { get; set; } = 255;
    public byte B { get; set; } = 100;
    public double Opacity { get; set; } = 1.0;
    public bool ShowDot { get; set; } = false;
    public int DotSize { get; set; } = 3;
    public bool TShape { get; set; } = false;
    public string Language { get; set; } = "zh";
}
