using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyCrosshair;

public partial class ConfigWindow : Window
{
    private bool _loading = true;
    private bool _updatingControls = false;
    private readonly System.Windows.Threading.DispatcherTimer _saveTimer;

    // Maps each value TextBox → (its Slider, min, max)
    private Dictionary<TextBox, (Slider Slider, int Min, int Max)> _boxMap = new();

    public ConfigWindow()
    {
        InitializeComponent();

        _saveTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(800)
        };
        _saveTimer.Tick += (_, _) => { _saveTimer.Stop(); SettingsManager.Save(); };

        _boxMap = new()
        {
            { ValR,         (SliderR,         0,   255) },
            { ValG,         (SliderG,         0,   255) },
            { ValB,         (SliderB,         0,   255) },
            { ValOpacity,   (SliderOpacity,   10,  100) },
            { ValLength,    (SliderLength,    1,   60)  },
            { ValThickness, (SliderThickness, 1,   10)  },
            { ValGap,       (SliderGap,       0,   30)  },
            { ValOutline,   (SliderOutline,   1,   5)   },
            { ValDotSize,   (SliderDotSize,   1,   20)  },
        };

        LoadSettings();
    }

    private void LoadSettings()
    {
        _loading = true;
        var s = SettingsManager.Current;

        SliderR.Value         = s.R;
        SliderG.Value         = s.G;
        SliderB.Value         = s.B;
        SliderOpacity.Value   = (int)(s.Opacity * 100);
        SliderLength.Value    = s.Length;
        SliderThickness.Value = s.Thickness;
        SliderGap.Value       = s.Gap;
        SliderOutline.Value   = s.OutlineThickness;
        ChkOutline.IsChecked  = s.ShowOutline;
        ChkDot.IsChecked      = s.ShowDot;
        SliderDotSize.Value   = s.DotSize;
        ChkTShape.IsChecked   = s.TShape;

        foreach (ComboBoxItem item in LangCombo.Items)
            if ((string)item.Tag == s.Language) { LangCombo.SelectedItem = item; break; }

        UpdateLabels();
        UpdateColorPreview();
        UpdateOutlineRow();
        ApplyLanguage();

        _loading = false;
    }

    private void ApplyLanguage()
    {
        TitleText.Text       = Loc.Title;
        SecColor.Text        = Loc.SecColor;
        SecShape.Text        = Loc.SecShape;
        SecOptions.Text      = Loc.SecOptions;
        LblCurrentColor.Text = Loc.CurrentColor;
        LblR.Text            = Loc.Red;
        LblG.Text            = Loc.Green;
        LblB.Text            = Loc.Blue;
        LblOpacity.Text      = Loc.Opacity;
        LblLength.Text       = Loc.Length;
        LblThickness.Text    = Loc.Thickness;
        LblGap.Text          = Loc.Gap;
        LblOutlineThick.Text = Loc.OutlineThickness;
        LblDotSize.Text      = Loc.DotSize;
        LblLanguage.Text     = Loc.LangLabel;
        ChkOutline.Content   = Loc.ShowOutline;
        ChkDot.Content       = Loc.ShowDot;
        ChkTShape.Content    = Loc.TShape;
        SaveBtn.Content      = Loc.SaveConfig;
    }

    private void UpdateLabels()
    {
        _updatingControls = true;
        ValR.Text         = ((int)SliderR.Value).ToString();
        ValG.Text         = ((int)SliderG.Value).ToString();
        ValB.Text         = ((int)SliderB.Value).ToString();
        ValOpacity.Text   = ((int)SliderOpacity.Value).ToString();
        ValLength.Text    = ((int)SliderLength.Value).ToString();
        ValThickness.Text = ((int)SliderThickness.Value).ToString();
        ValGap.Text       = ((int)SliderGap.Value).ToString();
        ValOutline.Text   = ((int)SliderOutline.Value).ToString();
        ValDotSize.Text   = ((int)SliderDotSize.Value).ToString();
        _updatingControls = false;
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
        bool outlineOn = ChkOutline.IsChecked == true;
        OutlineThicknessRow.IsEnabled = outlineOn;
        OutlineThicknessRow.Opacity   = outlineOn ? 1.0 : 0.4;

        bool dotOn = ChkDot.IsChecked == true;
        DotSizeRow.IsEnabled = dotOn;
        DotSizeRow.Opacity   = dotOn ? 1.0 : 0.4;
    }

    private void ApplySettings()
    {
        if (_loading) return;

        var s = SettingsManager.Current;
        s.R              = (byte)(int)SliderR.Value;
        s.G              = (byte)(int)SliderG.Value;
        s.B              = (byte)(int)SliderB.Value;
        s.Opacity        = SliderOpacity.Value / 100.0;
        s.Length         = (int)SliderLength.Value;
        s.Thickness      = (int)SliderThickness.Value;
        s.Gap            = (int)SliderGap.Value;
        s.OutlineThickness = (int)SliderOutline.Value;
        s.ShowOutline    = ChkOutline.IsChecked == true;
        s.ShowDot        = ChkDot.IsChecked == true;
        s.DotSize        = (int)SliderDotSize.Value;
        s.TShape         = ChkTShape.IsChecked == true;

        UpdateLabels();
        UpdateColorPreview();
        UpdateOutlineRow();
        SettingsManager.NotifyChanged();

        _saveTimer.Stop();
        _saveTimer.Start();
    }

    // TextBox → Slider sync (user typed a number)
    private void ValBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_updatingControls || _loading) return;
        if (sender is not TextBox box) return;
        if (!_boxMap.TryGetValue(box, out var info)) return;
        if (!int.TryParse(box.Text, out int val)) return;

        val = Math.Clamp(val, info.Min, info.Max);
        if ((int)info.Slider.Value != val)
            info.Slider.Value = val;   // triggers ValueChanged → ApplySettings
    }

    private void Color_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => ApplySettings();
    private void Shape_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => ApplySettings();
    private void Option_Changed(object sender, RoutedEventArgs e) => ApplySettings();

    private void Lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_loading) return;
        if (LangCombo.SelectedItem is ComboBoxItem item)
        {
            SettingsManager.Current.Language = (string)item.Tag;
            ApplyLanguage();
            _saveTimer.Stop();
            _saveTimer.Start();
        }
    }

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

        SaveBtn.Content = Loc.Saved;
        var t = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        t.Tick += (_, _) => { t.Stop(); SaveBtn.Content = Loc.SaveConfig; };
        t.Start();
    }
}
