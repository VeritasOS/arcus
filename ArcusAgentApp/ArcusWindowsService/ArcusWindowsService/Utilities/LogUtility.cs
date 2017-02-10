/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Runtime.CompilerServices;
using System.Text;
using Logging;

namespace BEArcus.Agent
{

    #region Enumerations

    public enum LogSite
    {
        File = 0,
        Console = 1,
        FileAndConsole = 2
    }

    #endregion Enumerations  

    class LogUtility
    {
        #region Fields

        public static ILog Log = Logger.Instance;
        public static LogSite logSite;
        public static Log log = new Log();
        #endregion Fields     

        #region Methods
        public static void CheckLogFileEnabled()
        {
            if (Configuration.Instance.LogSetting.Enabled)
            {
                logSite = LogSite.FileAndConsole;
                return;
            }
            logSite = LogSite.Console;
        }


        public static void LogExceptionFunction(Exception exception, string typeName = null, string extraInfo = null,
            [CallerMemberName] string methodName = null, [CallerFilePath] string sourceFile = null, [CallerLineNumber] int lineNumber = 0)
        {
            log.LogException(exception);
        }

        public static void LogInfoFunction(string message, string typeName = null,
            [CallerMemberName] string methodName = null, [CallerFilePath] string sourceFile = null, [CallerLineNumber] int lineNumber = 0)
        {
            string logString = GetLogString(message, typeName, methodName);
            log.LogInfo(logString);
        }

        public static void LogInfoFunctionFinished(string typeName = null, string extraInfo = null,
            [CallerMemberName] string methodName = null, [CallerFilePath] string sourceFile = null, [CallerLineNumber] int lineNumber = 0)
        {
            LogInfoFunction("Finished", typeName, methodName, sourceFile, lineNumber);
        }

        private static string GetLogString(string message, string typeName = null, string methodName = null)
        {
            StringBuilder sb = new StringBuilder();

            if (!typeName.IsNullEmptyOrBlank()) sb.AppendFormat("{0}.", typeName);
            sb.AppendFormat("{0}", methodName);
            string logString = string.Format("[{0}] - {1}", sb.ToString(), message);

            return logString;
        }

        #endregion
    }
}
