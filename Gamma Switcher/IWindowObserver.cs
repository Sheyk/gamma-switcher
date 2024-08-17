namespace Gamma_Switcher;

public interface IWindowObserver
{
    public string TargetWindowTitle { get; }
    public bool Enabled { get; }
    public void OnFocus();
    public void OnLostFocus();
}

public static class WindowObserverExtensions
{
    public static void RunIfEnabled(this IWindowObserver observer, Action action)
    {
        if (observer.Enabled)
            action();
    }

    public static void RunIfEnabled(this IWindowObserver observer, Func<Task> action)
    {
        if (observer.Enabled)
            Task.Run(action);
    }
}
