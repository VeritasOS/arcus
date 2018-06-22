/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using Newtonsoft.Json;
using System;

namespace BEArcus.Agent
{
    public class Job
    {
        public enum StatusString
        {
            Unknown,
            Canceled,
            Completed,
            SucceededWithExceptions,
            OnHold,
            Error,
            Missed,
            Recovered,
            Resumed,
            Succeeded,
            ThresholdAbort,
            Dispatched,
            DispatchFailed,
            InvalidSchedule,
            InvalidTimeWindow,
            NotInTimeWindow,
            Queued,
            Disabled,
            Active,
            Ready,
            Scheduled,
            Superseded,
            ToBeScheduled,
            Linked,
            RuleBlocked
        };

        public enum JobTypeString
        {
            Unknown,
            Backup,
            Restore,
            Verify,
            Catalog,
            Utility,
            Report,
            Duplicate,
            TestRun,
            ResourceDiscovery,
            CopyJob,
            Archive,
            RestoreFromArchive,
            DeleteFromArchive,
            Install,
            ConvertToVirtual
        };

        public enum PriorityString
        {
            Normal,
            Lowest,
            Low,
            High,
            Highest
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


        public string TaskName { get; set; }

        private string jobType;
        public string JobType
        {
            get
            {
                return jobType;
            }
            set
            {
                jobType = value;
                jobType = GetJobTypeString(Int32.Parse(JobType));
            }
        }

        public Storage Storage { get; set; }

        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                status = GetStatusString(Int32.Parse(Status));
            }
        }

        public Schedule Schedule { get; set; }

        public string SelectionSummary { get; set; }

        private string priority;
        public string Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
                priority = GetPriorityString(Int32.Parse(Priority));
            }
        }

        public string MediaServerName { get; set; }

        public string AgentVersion = CommonSettings.Agent;

        public static string GetStatusString(int id)
        {
            return Enum.GetName(typeof(StatusString), id);
        }
        public static string GetJobTypeString(int id)
        {
            return Enum.GetName(typeof(JobTypeString), id);
        }

        public static string GetPriorityString(int id)
        {
            return Enum.GetName(typeof(PriorityString), id);
        }

    }
}

