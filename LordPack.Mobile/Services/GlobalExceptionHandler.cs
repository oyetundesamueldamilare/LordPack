namespace LordPack.Mobile.Services;

public static class GlobalExceptionHandler
{
    public static void Initialize()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var exception = args.ExceptionObject as Exception;
            LogException(exception, "AppDomain UnhandledException");
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            LogException(args.Exception, "TaskScheduler UnobservedTaskException");
            args.SetObserved();
        };
    }

    private static void LogException(Exception? ex, string source)
    {
        if (ex == null) return;

        System.Diagnostics.Debug.WriteLine($"[GLOBAL ERROR] {source}: {ex.Message}");

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Unexpected Error", "An unexpected error occurred. If the issue persists, please restart the app.", "OK");
            }
        });
    }
}