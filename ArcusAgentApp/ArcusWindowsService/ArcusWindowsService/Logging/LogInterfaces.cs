/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Logging
{

    /// <summary>
    /// This static class contains the constant values for the various well known log masks.
    /// A flags enumeration was not used in order to provide a mechanism for components to define
    /// additional custom flags specific to the component.
    /// </summary>
    public static class LogLevel
    {
        /// <remark>
        /// The master switch controlled by a log configuration object.
        /// </remark>
        public const uint Enabled = 0x00000001;

        /// <remark>
        /// Informational statements and will be displayed when the component is enabled.
        /// </remark>
        public const uint Information = 0x00000002;

        /// <remark>
        /// Additonal informational stamtements and will be displayed when verbose logging is activated.
        /// </remark>
        public const uint Verbose = 0x00000004;

        /// <remark>
        /// Warning statements.  A warning prefix is prepended to the statement and is highlighted.
        /// </remark>
        public const uint Warning = 0x00000008;

        /// <remark>
        /// Error statements.  An error prefix is prepended to the statement and is highlighted.
        /// </remark>
        public const uint Error = 0x00000010;

        /// <remark>
        /// Debug statements. A debug prefix is prepended to the statement and is highlighted.
        /// </remark>
        public const uint Debug = 0x00000020;

        /// <remark>
        /// EventLog statements. EventLog statements are always displayed but only actually written to
        /// the eventlog when the event log level is enabled.
        /// </remark>
        public const uint Event = 0x00000040;

        /// <remark>
        /// This is a mask that indicates the valid region for user defined logging flags.
        /// These flags could be used to log a specific set of behaviors within a component.
        /// To make use of custom flags the component would use the LogInfo(int, string, params) method.
        /// </remark>
        public const uint User = 0xFFFFFF00;

        /// <remarks>
        /// The default setting that components will use if configuration information is not found.
        /// </remarks>
        public const uint Default = Enabled | Information | Verbose | Warning | Error | Debug | Event;
    }
    /// <summary>
    /// The log interface that is used by components to log information.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Name of this log object. This name will show up in the statement being logged.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Name of the owner of the log object.
        /// </summary>
        string Owner { get; set; }

        /// <summary>
        /// Description of the owner using this log object.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The name of the event source when an event is written to the event log.
        /// </summary>
        string EventLogSource { get; set; }

        /// <summary>
        /// Logs an informational statement.
        /// </summary>
        void LogInfo(string text, params object[] parms);

        /// <summary>
        /// Logs an informational statement if the given custom component level is active.
        /// </summary>
        void LogInfo(uint level, string text, params object[] parms);

        /// <summary>
        /// Logs a verbose informational statement.
        /// </summary>
        void LogVerbose(string text, params object[] parms);

        /// <summary>
        /// Logs a warning statement.
        /// </summary>
        void LogWarning(string text, params object[] parms);

        /// <summary>
        /// Logs an error statement.
        /// </summary>
        void LogError(string text, params object[] parms);

        /// <summary>
        /// Logs an error statement and assert.
        /// </summary>
        void LogErrorAndAssert(string text, params object[] parms);

        /// <summary>
        /// If condition is true, error will be logged and execution will be asserted.
        /// Logs an error statement and assert bases on provided contition.
        /// </summary>
        void LogErrorAndAssert(bool condition, string text, params object[] parms);


        /// <summary>
        /// Logs a statement indicating an exception was encountered. Uses the 'Error' flag to decide if it should be logged.
        /// </summary>
        void LogException(string text, params object[] parms);

        /// <summary>
        /// Logs the given exception. Uses the 'Error' flag to decide if it should be logged.
        /// </summary>
        void LogException(Exception excpt);

        /// <summary>
        /// Logs Text to the event log. Events are always logged to the system.
        /// </summary>
        /// <param name="text">Format string to use for output</param>
        /// <param name="type">The type of event (information, error, etc.)</param>
        void LogEvent(string text, EventLogEntryType type);

        /// <summary>
        /// Formats text and logs it to the event log. Events are always logged to the system.
        /// </summary>
        /// <param name="text">Format string to use for output</param>
        /// <param name="type">The type of event (information, error, etc.)</param>
        /// <param name="category">The category of the event.</param>
        /// <param name="parms">Array of objects inserted into the format string to produce text</param>
        void LogEvent(string text, EventLogEntryType type, int category, byte[] data, params object[] parms);

        /// <summary>
        /// Merges any supplied strings to the specified messageID and logs it to the event log. Events are always logged to the system.
        /// </summary>
        /// <param name="messageID">The id of the message to log.</param>
        /// <param name="type">The type of event (information, error, ect.)</param>
        /// <param name="category">The category of the event.</param>
        /// <param name="parms">Array of objects inserted into the format string to produce text</param>
        /// <param name="data">Binary data to log with the event.</param>
        void LogEvent(long messageID, EventLogEntryType type, int category, object[] parms, byte[] data);
    }
  
}
