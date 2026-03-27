using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MyCrosshair;

public partial class ConfigWindow : Window
{
    private bool _loading = true;
    private readonly System.Windows.Threading.DispatcherTimer _saveTimer;

    public ConfigWindow()
    {
        InitializeComponent();

        _saveTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(800)
        };
        _saveTimer.Tick += (_, _) => { _saveTimer.Stop(); SettingsManager.Save(); };

        LoadSettings();
    }

    private void LoadSettings()
    {
        _loading = true;
        var s = SettingsManager.Current;

        SliderR.Value = s.R;
        SliderG.Value = s.G;
        SliderB.Value = s.B;
        SliderOpacity.Value = (int)(s.Opacity * 100);
        SliderLength.Value = s.Length;
        SliderThickness.Value = s.Thickness;
        SliderGap.Value = s.Gap;
        SliderOutline.Value = s.OutlineThickness;
        ChkOutline.IsChecked = s.ShowOutline;
        ChkDot.IsChecked = s.ShowDot;
        SliderDotSize.Value = s.DotSize;
        ChkTShape.IsChecked = s.TShape;

        UpdateLabels();
        UpdateColorPreview();
        UpdateOutlineRow();

        _loading = false;
    }

    private void UpdateLabels()
    {
        ValR.Text = ((int)SliderR.Value).ToString();
        ValG.Text = ((int)SliderG.Value).ToString();
        ValB.Text = ((int)SliderB.Value).ToString();
        ValOpacity.Text = $"{(int)SliderOpacity.Value}%";
        ValLength.Text = ((int)SliderLength.Value).ToString();
        ValThickness.Text = ((int)SliderThickness.Value).ToString();
        ValGap.Text = ((int)SliderGap.Value).ToString();
        ValOutline.Text = ((int)SliderOutline.Value).ToString();
        ValDotSize.Text = ((int)SliderDotSize.Value).ToString();
    }

    private void UpdateColorPreview()
    {
        var color = Color.FromRgb(
            (byte)(int)SliderR.Value,
            (byte)(int)SliderG.Value,
            (byte)(int)SliderB.Value);
        ColorPreview.Background = new SolidColorBrush(color);
    }

    private void UpdateOutlineRow()
    {
        bool enabled = ChkOutline.IsChecked == true;
        OutlineThicknessRow.IsEnabled = enabled;
        OutlineThicknessRow.Opacity = enabled ? 1.0 : 0.4;

        bool dotEnabled = ChkDot.IsChecked == true;
        DotSizeRow.IsEnabled = dotEnabled;
        DotSizeRow.Opacity = dotEnabled ? 1.0 : 0.4;
    }

    private void ApplySettings()
    {
        if (_loading) return;

        var s = SettingsManager.Current;
        s.R = (byte)(int)SliderR.Value;
        s.G = (byte)(int)SliderG.Value;
        s.B = (byte)(int)SliderB.Value;
        s.Opacity = SliderOpacity.Value / 100.0;
        s.Length = (int)SliderLength.Value;
        s.Thickness = (int)SliderThickness.Value;
        s.Gap = (int)SliderGap.Value;
        s.OutlineThickness = (int)SliderOutline.Value;
        s.ShowOutline = ChkOutline.IsChecked == true;
        s.ShowDot = ChkDot.IsChecked == true;
        s.DotSize = (int)SliderDotSize.Value;
        s.TShape = ChkTShape.IsChecked == true;

        UpdateLabels();
        UpdateColorPreview();
        UpdateOutlineRow();
        SettingsManager.NotifyChanged();

        _saveTimer.Stop();
        _saveTimer.Start();
    }

    private void Color_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => ApplySettings();
    private void Shape_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => ApplySettings();
    private void Option_Changed(object sender, RoutedEventArgs e) => ApplySettings();

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();

    private void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        _saveTimer.Stop();
        SettingsManager.Save();

        SaveBtn.Content = "已保存 v";
        var t = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        t.Tick += (_, _) => { t.Stop(); SaveBtn.Content = "保存配置"; };
        t.Start();
    }
}
