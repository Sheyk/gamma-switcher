namespace Gamma_Switcher;

public partial class App : Form
{
    private readonly NotifyIcon _notifyIcon;
    private readonly WindowChangeTracker _windowChangeTracker;

    public App(WindowChangeTracker windowChangeTracker)
    {
        _windowChangeTracker = windowChangeTracker;

        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        Visible = false;

        _notifyIcon = new NotifyIcon();
        _notifyIcon.Icon = SystemIcons.Application;
        _notifyIcon.Icon = new Icon("icon.ico");
        _notifyIcon.Visible = true;
        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        _notifyIcon.ContextMenuStrip.Items.Add("Pause", null, TogglePause);
        _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);

        _windowChangeTracker.Start();
    }

    private void Exit(object? sender, EventArgs e)
    {
        _notifyIcon.Visible = false;
        Application.Exit();
    }

    private void TogglePause(object? sender, EventArgs e)
    {
        var item = _notifyIcon.ContextMenuStrip!.Items[0];
        item.Text = item.Text == "Pause" ? "Resume" : "Pause";
        _windowChangeTracker.TogglePause();
    }
    
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Hide();
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW
            return cp;
        }
    }
}
