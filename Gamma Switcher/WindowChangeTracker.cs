using System.Runtime.InteropServices;
using System.Text;
using Timer = System.Timers.Timer;
namespace Gamma_Switcher;

public class WindowChangeTracker(IWindowObserver[] windowObservers)
{
    private readonly Timer _timer = new ();
    private readonly WindowEvents _windowEvents = new ();
    private readonly IWindowObserver[] _windowObservers = windowObservers.Where(x => x.Enabled).ToArray();
    private bool _pause;

    public void Start()
    {
        _timer.Interval = 1000;
        _timer.Elapsed += SwitchConfigOnActiveWindow;
        _timer.Start();
    }

    private void SwitchConfigOnActiveWindow(object? _, EventArgs e)
    {
        if (_pause) return;

        _windowEvents.Add(GetActiveWindowTitle());

        if (_windowEvents.AreAllEqual() || !_windowEvents.HappenedTwice()) return;

        foreach (var windowObserver in _windowObservers)
            if (_windowEvents.TitleN!.StartsWith(windowObserver.TargetWindowTitle))
                windowObserver.OnFocus();
            else
                windowObserver.OnLostFocus();
    }

    public void TogglePause()
    {
        _pause = !_pause;
        if (_pause)
            _timer.Stop();
        if (!_pause)
            _timer.Start();

        _windowEvents.Clear();
    }

    private static string? GetActiveWindowTitle()
    {
        const int nChars = 256;
        var buffer = new StringBuilder(nChars);
        var handle = GetForegroundWindow();
        return GetWindowText(handle, buffer, nChars) > 0 ? buffer.ToString() : null;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    private class WindowEvents
    {
        private string? TitleNMinus2 { get; set; }
        private string? TitleNMinus1 { get; set; }
        public string? TitleN { get; private set; }

        public void Add(string? title)
        {
            TitleNMinus2 = TitleNMinus1;
            TitleNMinus1 = TitleN;
            TitleN = title;
        }

        public bool AreAllEqual() => TitleN == TitleNMinus1 && TitleNMinus1 == TitleNMinus2;

        public bool HappenedTwice() =>
            TitleN != null && TitleN == TitleNMinus1;

        public void Clear()
        {
            TitleNMinus2 = null;
            TitleNMinus1 = null;
            TitleN = null;
        }
    }
}
