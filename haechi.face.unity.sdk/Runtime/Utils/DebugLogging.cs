
public static class DebugLogging
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public static LogLevel logLevel = LogLevel.Info;

    public static void DebugLog(string message)
    {
        UnityEngine.Debug.Log(message);
    }
}
