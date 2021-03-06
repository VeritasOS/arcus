﻿/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using Newtonsoft.Json;
using System;

namespace BEArcus.Agent
{
    public class Alert
    {
        public enum CategoryString
        {
            None,
            JobWarning,
            JobSuccess,
            JobFailure,
            JobCancellation,
            CatalogError,
            SoftwareUpdateInformation,
            SoftwareUpdateWarning,
            SoftwareUpdateError,
            InstallInformation,
            InstallWarning,
            GeneralInformation,
            DatabaseMaintenanceInformation,
            DatabaseMaintenanceFailure,
            BackupExecRetrieveUrlUpdateInformation,
            BackupExecRetrieveUrlUpdateFailure,
            ArchivingOptionOperationInformation,
            ArchivingOptionOperationFailure,
            IdrCopyFailed,
            IdrFullBackupSuccess,
            BackupJobContainsNoData,
            JobCompletedWithExceptions,
            JobStart,
            ServiceStart,
            ServiceStop,
            DeviceError,
            DeviceWarning,
            DeviceInformation,
            DeviceIntervention,
            MediaError,
            MediaWarning,
            MediaInformation,
            MediaIntervention,
            MediaInsert,
            MediaOverwrite,
            MediaRemove,
            LibraryInsert,
            TapeAlertInformation,
            TapeAlertWarning,
            TapeAlertError
        };

        public enum SourceString
        {
            System,
            Storage,
            Media,
            Job
        };

        public string Name { get; set; }


        public string be_id;
        public string Id
        {
            get
            {
                return be_id;
            }
            set
            {
                be_id = value;
            }
        }

        public DateTime Date { get; set; }

        private string severity;
        public string Severity
        {
            get
            {
                return severity;
            }
            set
            {
                severity = value;
                severity = GetSeverityString(Severity);
            }
        }
        // public enum SeverityString {None,Information,Question,Warning,Error}

        private string category;
        public string Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
                category = GetCategoryString(Int32.Parse(Category));
            }
        }

        public string Message { get; set; }

        public Storage Storage { get; set; }

        public string BackupExecServerName { get; set; }

        private string source;
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                source = GetSourceString(Int32.Parse(Source));
            }
        }

        public string MediaServerName { get; set; }

        public static string GetSeverityString(string id)
        {
            switch (id)
            {
                case "0":
                    return "None";
                case "1":
                    return "Information";
                case "2":
                    return "Question";
                case "3":
                    return "Warning";
                case "4":
                    return "Error";
                default:
                    return null;
            }
        }

        public string Umi { get; set; }

        public string AgentVersion = CommonSettings.Agent;

        public static string GetCategoryString(int id)
        {
            return Enum.GetName(typeof(CategoryString), id);
        }

        public static string GetSourceString(int id)
        {
            return Enum.GetName(typeof(SourceString), id);
        }
    }
}

