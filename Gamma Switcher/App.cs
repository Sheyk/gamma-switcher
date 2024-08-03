using EftSettings;
using System.Runtime.InteropServices;
using Timer = System.Timers.Timer;
namespace Gamma_Switcher;

public partial class App : Form
{
    private readonly ColorConfigs _configFile;
    private readonly NotifyIcon _notifyIcon;
    private const string TargetWindowTitle = "EscapeFromTarkov";
    private bool _isTargetWindowActive;
    private bool _oneSecondBuffer;

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

    public App(ColorConfigs configFile)
    {
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        Visible = false;
        
        _configFile = configFile;
        _notifyIcon = new NotifyIcon();
        _notifyIcon.Icon = SystemIcons.Application;
        _notifyIcon.Icon = new Icon("icon.ico");
        _notifyIcon.Visible = true;
        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);

        var timer = new Timer();
        timer.Interval = 1000;
        timer.Elapsed += SwitchConfigOnActiveWindow;
        timer.Start();
    }

    private void SwitchConfigOnActiveWindow(object? _, EventArgs e)
    {
        var activeWindowTitle = GetActiveWindowTitle();
        var isTargetWindowTitle = activeWindowTitle?.StartsWith(TargetWindowTitle) == true;
        
        if (isTargetWindowTitle && !_isTargetWindowActive)
        {
            if(_oneSecondBuffer)
            {
                SwitchToTargetConfig();
                _isTargetWindowActive = true;
                _oneSecondBuffer = false;
            }
            else _oneSecondBuffer = true;
            
        }
        else if (isTargetWindowTitle && _isTargetWindowActive)
        {
            SwitchToDefaultConfig();
            _isTargetWindowActive = false;
            _oneSecondBuffer = false;
        }
    }

    private static string? GetActiveWindowTitle()
    {
        const int nChars = 256;
        var buffer = new System.Text.StringBuilder(nChars);
        var handle = GetForegroundWindow();
        return GetWindowText(handle, buffer, nChars) > 0 ? buffer.ToString() : null;
    }

    private void SwitchToTargetConfig()
    {
        NvidiaSettings.SetDigitalVibrance(_configFile.Target!.Vibrance);
        WindowsSettings.SetGammaRamp(_configFile.Target.GammaRamp);
    }

    private void SwitchToDefaultConfig()
    {
        NvidiaSettings.SetDigitalVibrance(_configFile.Default!.Vibrance);
        WindowsSettings.SetGammaRamp(_configFile.Default.GammaRamp);
    }

    private void Exit(object? sender, EventArgs e)
    {
        _notifyIcon.Visible = false;
        Application.Exit();
    }
}

