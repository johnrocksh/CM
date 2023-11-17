namespace CastManager.Logger
{
    using System.Runtime.CompilerServices;
    using System.Diagnostics;

    using NLogLogger = NLog.Logger;
    using NLog;

    public static class Logger
    {
        public static void WriteLine(string message, [CallerMemberName] string propertyName = null)
        {
            var msgPrompt = $"{propertyName}: {message}";
            Debug.WriteLine(msgPrompt);
            LogManager.GetCurrentClassLogger().Info(msgPrompt);
        }
        public static void WriteException(string message, [CallerMemberName] string propertyName = null)
        {
            var msgPrompt = $"{propertyName}: {message}";
            Debug.WriteLine(msgPrompt);
            LogManager.GetCurrentClassLogger().Error(msgPrompt);
        }
    }
}
