using System;
using System.Globalization;
namespace LDAPSyncTool 
{
    
    public class Log
    {
        public static int LogLevel = 1;

        private static void logEntry(int level,string message, params object[] args)
        {
            if (level >= LogLevel)
            {
                var formattedMessage = string.Format(message, args);

                var logLevelFormatted = level switch 
                {
                    0 => "DEBUG",
                    1 => "INFO",
                    2 => "WARN",
                    3 => "ERROR",
                    4 => "FATAL",
                    _ => "UNKNOWN"
                };

                var logEntry = $"{DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture)} [{logLevelFormatted}] {formattedMessage}";

                Console.WriteLine(logEntry);
            }
        }
        public static void Debug(string message, params object[] args)
        {
            logEntry(0, message, args);
        }
        public static void Info(string message, params object[] args)
        {
            logEntry(1, message, args);
        }
        public static void Warn(string message, params object[] args)
        {
            logEntry(2, message, args);
        }
        public static void Error(string message, params object[] args)
        {
            logEntry(3, message, args);
        }
        public static void Fatal(string message, params object[] args)
        {
            logEntry(4, message, args);
        }
        public static void Exception(Exception ex)
        {
            var messageBuilder = new System.Text.StringBuilder();
            

            var exception = ex;
            while(exception != null)
            {
                messageBuilder.AppendLine(exception.Message);
                var trace = exception.StackTrace;
                if (trace != null)
                {
                    messageBuilder.AppendLine(trace);
                }
                exception = exception.InnerException;
            }
            Error(messageBuilder.ToString());
        }


    }
}