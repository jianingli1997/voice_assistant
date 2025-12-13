namespace VoiceAssistant.Log;
using NLog.Config;
using NLog.Targets;
using NLog;
using NLog.Layouts;

public static class LogHelper
{
    private static readonly object LockObj = new object();

    public static void WriteErrorLog(string message, string loggerName)
    {
        LogHelper.WriteLog(message, loggerName, LogType.Error);
    }

    public static void WriteInfoLog(string message, string loggerName)
    {
        LogHelper.WriteLog(message, loggerName, LogType.Info);
    }

    public static void WriteWarnLog(string message, string loggerName)
    {
        LogHelper.WriteLog(message, loggerName, LogType.Warn);
    }


    public static void WriteDebugLog(string message, string loggerName)
    {
        LogHelper.WriteLog(message, loggerName, LogType.Debug);
    }

    public static void WriteFatalLog(string message, string loggerName)
    {
        LogHelper.WriteLog(message, loggerName, LogType.Fatal);
    }


    private static void WriteLog(string message, string loggerName, LogType logType)
    {
        LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
        FileTarget fileTarget1 = new FileTarget("logfile");
        fileTarget1.FileName = (Layout)(loggerName == "alarm"
            ? string.Format("logs/{0:yyyy-MM-dd}/{1}.txt", (object)DateTime.Now, (object)loggerName)
            : string.Format("logs/{0:yyyy-MM-dd}/{1}/{2}.txt", (object)DateTime.Now, (object)logType,
                (object)loggerName));
        fileTarget1.Layout = (Layout)(loggerName == "alarm"
            ? "${longdate}${message}"
            : "${longdate}|${level}|${logger}|${message}");
        fileTarget1.ArchiveAboveSize = 10485760L;
        FileTarget fileTarget2 = fileTarget1;
        loggingConfiguration.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, (Target)fileTarget2);
        LogManager.Configuration = loggingConfiguration;
        Logger logger = LogManager.GetLogger(loggerName);
        switch (logType)
        {
            case LogType.Debug:
                logger.Debug("\n" + message);
                break;
            case LogType.Info:
            case LogType.Communication:
                logger.Info("\n" + message);
                break;
            case LogType.Warn:
                logger.Warn("\n" + message);
                break;
            case LogType.Error:
                logger.Error("\n" + message);
                break;
            case LogType.Fatal:
                logger.Fatal("\n" + message);
                break;
        }
    }
}