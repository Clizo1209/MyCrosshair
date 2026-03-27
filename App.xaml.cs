using System.Windows;
using WinForms = System.Windows.Forms;

namespace MyCrosshair;

public partial class App
{
    private WinForms.NotifyIcon _notifyIcon = null!;
    private OverlayWindow _overlay = null!;
    private ConfigWindow? _configWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SettingsManager.Load();

        _overlay = new OverlayWindow();
        _overlay.Show();

        SetupTrayIcon();
    }

    private void SetupTrayIcon()
    {
        var bmp = new System.Drawing.Bitmap(16, 16);
        using (var g = System.Drawing.Graphics.FromImage(bmp))
        {
            g.Clear(System.Drawing.Color.Transparent);
            using var pen = new System.Drawing.Pen(System.Drawing.Color.LimeGreen, 1.5f);
            g.DrawLine(pen, 8, 2, 8, 14);
            g.DrawLine(pen, 2, 8, 14, 8);
        }

        _notifyIcon = new WinForms.NotifyIcon
        {
            Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon()),
            Visible = true,
            Text = "MyCrosshair"
        };

        var menu = new WinForms.ContextMenuStrip();
        menu.Items.Add("配置", null, (_, _) => Dispatcher.Invoke(OpenConfig));
        menu.Items.Add(new WinForms.ToolStripSeparator());
        menu.Items.Add("退出", null, (_, _) => Dispatcher.Invoke(Shutdown));

        _notifyIcon.ContextMenuStrip = menu;
        _notifyIcon.DoubleClick += (_, _) => Dispatcher.Invoke(OpenConfig);
    }

    private void OpenConfig()
    {
        if (_configWindow == null)
        {
            _configWindow = new ConfigWindow();
            _configWindow.Closing += (_, e) =>
            {
                e.Cancel = true;
                _configWindow.Hide();
            };
        }
        _configWindow.Show();
        _configWindow.Activate();
        if (_configWindow.WindowState == WindowState.Minimized)
            _configWindow.WindowState = WindowState.Normal;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _notifyIcon?.Dispose();
        base.OnExit(e);
    }
}
