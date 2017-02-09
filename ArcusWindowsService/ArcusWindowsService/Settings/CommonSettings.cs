/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Linq;
using ArcusWindowsService;

namespace BEArcus.Agent
{
    public class CommonSettings
    {
        #region Properties

        private static string BemcliScriptPath;
        private static string DatabaseName;
        private static string AlertCollectionName;
        private static string JobCollectionName;
        private static string JobHistoryCollectionName;
        private static string TimeInterval;
        private static string LastUpdatedTime;
        private static string LogFilePath;
        private static string MediaServerCollectionName;
        private static string CustomerName;
        private static string AgentVersion;
        #endregion


        public static string BemcliPath
        {
            get
            {
                if (string.IsNullOrEmpty(BemcliScriptPath))
                {
                    BemcliScriptPath = GetSetting("BemcliScripts");
                }
                return BemcliScriptPath;
            }
        }

        public static string Database
        {
            get
            {
                if (string.IsNullOrEmpty(DatabaseName))
                {
                    DatabaseName = GetSetting("DatabaseName");
                }
                return DatabaseName;
            }
        }

        public static string AlertCollection
        {
            get
            {
                if (string.IsNullOrEmpty(AlertCollectionName))
                {
                    AlertCollectionName = GetSetting("AlertCollectionName");
                }
                return AlertCollectionName;
            }
        }

        public static string JobCollection
        {
            get
            {
                if (string.IsNullOrEmpty(JobCollectionName))
                {
                    JobCollectionName = GetSetting("JobCollectionName");
                }
                return JobCollectionName;
            }
        }

        public static string JobHistoryCollection
        {
            get
            {
                if (string.IsNullOrEmpty(JobHistoryCollectionName))
                {
                    JobHistoryCollectionName = GetSetting("JobHistoryCollectionName");
                }
                return JobHistoryCollectionName;
            }
        }

        public static string MediaServerCollection
        {
            get
            {
                if (string.IsNullOrEmpty(MediaServerCollectionName))
                {
                    MediaServerCollectionName = GetSetting("MediaServerCollectionName");
                }
                return MediaServerCollectionName;
            }
        }

        public static string Interval
        {
            get
            {
                if (string.IsNullOrEmpty(TimeInterval))
                {
                    TimeInterval = GetSetting("TimeInterval");
                }
                return TimeInterval;
            }
        }

        public static string LastUpdate
        {
            get
            {
                if (string.IsNullOrEmpty(LastUpdatedTime))
                {
                    LastUpdatedTime = GetSetting("LastUpdatedTime");
                }
                return LastUpdatedTime;
            }
        }

        public static string LogFile
        {
            get
            {
                if (string.IsNullOrEmpty(LogFilePath))
                {
                    LogFilePath = GetSetting("LogFilePath");
                }
                return LogFilePath;
            }
        }
        public static string Customer
        {
            get
            {
                if (string.IsNullOrEmpty(CustomerName))
                {
                    CustomerName = GetSetting("CustomerName");
                }
                return CustomerName;
            }
        }

        public static string Agent
        {
            get
            {
                if (string.IsNullOrEmpty(AgentVersion))
                {
                    AgentVersion = GetSetting("AgentVersion");
                }
                return AgentVersion;
            }
        }


        #region Helper function

        public static string GetSetting(string settingName)
        {
            return Configuration.Instance.CommonSettings.Where(s => s.Name.Equals(settingName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
        }

        #endregion Helper function
    }
}
