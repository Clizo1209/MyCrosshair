using System.Windows;
using System.Windows.Media;

namespace MyCrosshair;

public class CrosshairRenderer : FrameworkElement
{
    public CrosshairRenderer()
    {
        SettingsManager.Changed += OnSettingsChanged;
        Unloaded += (_, _) => SettingsManager.Changed -= OnSettingsChanged;
    }

    private void OnSettingsChanged() => Dispatcher.Invoke(InvalidateVisual);

    protected override void OnRender(DrawingContext dc)
    {
        var s = SettingsManager.Current;
        var cx = ActualWidth / 2;
        var cy = ActualHeight / 2;
        var center = new Point(cx, cy);

        byte alpha = (byte)(s.Opacity * 255);
        var mainBrush = new SolidColorBrush(Color.FromArgb(alpha, s.R, s.G, s.B));
        mainBrush.Freeze();
        var mainPen = new Pen(mainBrush, s.Thickness);
        mainPen.Freeze();

        Pen? outlinePen = null;
        SolidColorBrush? outlineBrush = null;
        if (s.ShowOutline)
        {
            outlineBrush = new SolidColorBrush(Color.FromArgb(alpha, 0, 0, 0));
            outlineBrush.Freeze();
            outlinePen = new Pen(outlineBrush, s.Thickness + s.OutlineThickness * 2.0);
            outlinePen.Freeze();
        }

        var lines = new List<(Point from, Point to)>
        {
            (new Point(cx + s.Gap, cy), new Point(cx + s.Gap + s.Length, cy)),
            (new Point(cx - s.Gap, cy), new Point(cx - s.Gap - s.Length, cy)),
            (new Point(cx, cy + s.Gap), new Point(cx, cy + s.Gap + s.Length)),
        };

        if (!s.TShape)
            lines.Add((new Point(cx, cy - s.Gap), new Point(cx, cy - s.Gap - s.Length)));

        if (outlinePen != null)
            foreach (var (f, t) in lines)
                dc.DrawLine(outlinePen, f, t);

        foreach (var (f, t) in lines)
            dc.DrawLine(mainPen, f, t);

        if (s.ShowDot)
        {
            double r = s.DotSize;
            if (outlineBrush != null)
                dc.DrawEllipse(outlineBrush, null, center, r + s.OutlineThickness, r + s.OutlineThickness);
            dc.DrawEllipse(mainBrush, null, center, r, r);
        }
    }
}
