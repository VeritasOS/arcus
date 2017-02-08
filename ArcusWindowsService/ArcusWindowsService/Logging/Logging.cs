/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using BEArcus.Agent;

[assembly: Obfuscation(Exclude = true, ApplyToMembers = true, Feature = "renaming")]

namespace Logging
{
    /// <summary>
    /// The Log object is used by a component to send information to the log output stream of the application.
    /// The ultimate output location depends on which listeners the log object currently has.
    /// </summary>
    public class Log : ILog
    {
        protected string _logname;
        protected string _truncatedname;
        protected string _eventLogSource = @"BE_ARCUS_LOG";

        //public string arcusLogFolder = @"C:\Arcus.Logs";
        public string arcusLogFolder = CommonSettings.LogFile;
        private const uint LOG_SIZE_LIMIT = 100000; // 100 KB

        /// <remark>
        /// Name of the log.
        /// </remark>
        public string Name
        {
            get
            {
                return _logname;
            }
            set
            {
                _logname = value;
                _truncatedname = (value.Length > 12) ? value.Substring(0, 12) : value;
            }
        }
        public string Owner
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string EventLogSource
        {
            get { return _eventLogSource; }
            set { _eventLogSource = value; }
        }

        //Constructor
        public Log()
        {
            // Create your file object here
            if (!Directory.Exists(arcusLogFolder))  // if it doesn't exist, create
                Directory.CreateDirectory(arcusLogFolder);
        }


        /// <summary>
        /// Log an informational text string to the logging stream.  The text will only be emitted if the INFO bit is set.
        /// </summary>
        public void LogInfo(string text, params object[] parms)
        {
            WriteLog(text);
        }

        /// <summary>
        /// Log informational text to the logging stream.  This is only performed if the user defined level matches what the stream is currently configured to emit.
        /// </summary>
        public void LogInfo(uint level, string text, params object[] parms)
        {
            WriteLog(text);
        }

        /// <summary>
        /// Log warning text to the logging stream. The text will only be emitted if the WARNING bit is set.
        /// </summary>
        public void LogWarning(string text, params object[] parms)
        {
            WriteLog(text);
        }

        /// <summary>
        /// Log warning text to the logging stream. The text will only be emitted if the WARNING bit is set.
        /// </summary>
        public void LogVerbose(string text, params object[] parms)
        {
            WriteLog(text);
        }

        /// <summary>
        /// Log error text to the logging stream. The text will only be emitted if the ERROR bit is set.
        /// </summary>
        public void LogError(string text, params object[] parms)
        {
            WriteLog(text);
        }


        /// <summary>
        /// Log error text to the logging stream. The text will only be emitted if the ERROR bit is set.
        /// Also Assert.
        /// </summary>
        public void LogErrorAndAssert(string text, params object[] parms)
        {
            WriteLog(text);
        }

        /// <summary>
        /// Log error and assert execution if condition=true, else do nothing.
        /// </summary>
        /// <param name="condition">if condition=true, log error and assert execution, else do nothing.</param>
        /// <param name="text">Message text to log in case of error\assert</param>
        /// <param name="parms"></param>
        public void LogErrorAndAssert(bool condition, string text, params object[] parms)
        {
            if (condition == true)
            {
                LogErrorAndAssert(text, parms);
            }
        }


        /// <summary>
        /// Log debug text to the logging stream.  The text will only be emitted if the DEBUG bit is set.
        /// </summary>
        private void LogDebug(string text, params object[] parms) //saurabhG: Michael Avalone says use LogVerbose, avoid LogDebug, hence made private
        {
            WriteLog(text);
        }

        /// <summary>
        /// Log exception text to the logging stream.  The text will only be emitted if the ERROR bit is set.
        /// </summary>
        public void LogException(string text, params object[] parms)
        {
            WriteLog(text);
        }

        /// <summary>
        /// Converts and exception to text and logs it to the logging stream.  The text will only be emitted if the ERROR bit is set.
        /// </summary>
        public void LogException(Exception excpt)
        {
            if (excpt == null)
            {
                throw new ArgumentNullException("excpt");
            }
            WriteLog(excpt.ToString());
        }

        /// <summary>
        /// Logs an event to the system. The event will always be logged.
        /// </summary>
        public void LogEvent(string text, EventLogEntryType type, int category, byte[] data, params object[] parms)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            string str = string.Format(text, parms);

            WriteLog(text);
            System.Diagnostics.EventLog.WriteEntry(_eventLogSource, str, type, 0, (short)category, data);

        }


        /// <summary>
        /// Logs an event to the system. The event will always be logged.
        /// </summary>
        public void LogEvent(string text, EventLogEntryType type)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            System.Diagnostics.EventLog.WriteEntry(_eventLogSource, text, type, 0);
            WriteLog(text);
        }

        /// <summary>
        /// Logs an event to the system. The event will always be logged.
        /// </summary>
        public void LogEvent(long messageID, EventLogEntryType type, int category, object[] parms, byte[] data)
        {
            System.Diagnostics.EventLog.WriteEvent(_eventLogSource, new EventInstance(messageID, category, type), data, parms);
        }



        public override string ToString()
        {
            return _logname;
        }

        protected void WriteLog(string message)
        {
            string filePath = Path.Combine(arcusLogFolder, "ArcusLog.txt");
            string file2Path = Path.Combine(arcusLogFolder, "ArcusLog1.txt");

            //Create a file if it does not exists.            
            StreamWriter w = File.AppendText(filePath);
            w.Close();

            // Check for file size, copy in file2 if exceeds limit
            long length = new System.IO.FileInfo(filePath).Length;
            if (length > LOG_SIZE_LIMIT)
            {
                File.Copy(filePath, file2Path, true);
                File.WriteAllText(filePath, String.Empty);
            }

            using (StreamWriter streamWriter = File.AppendText(filePath))
            {
                streamWriter.WriteLine("{0}:{1}:{2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), message);
                streamWriter.Close();
            }
        }
    }
}

