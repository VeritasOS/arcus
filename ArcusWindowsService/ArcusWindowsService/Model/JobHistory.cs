/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using Newtonsoft.Json;
using System;
using System.Globalization;



namespace BEArcus.Agent
{
    public class JobHistory
    {
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
        public string Name { get; set; }

        private string status;
        public string JobStatus
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                status = Job.GetStatusString(Int32.Parse(JobStatus));
            }
        }

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
                jobType = Job.GetJobTypeString(Int32.Parse(JobType));
            }
        }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string BackupExecServerName { get; set; }

        public string StorageName { get; set; }

        public string PercentComplete { get; set; }

        public ElapsedTime ElapsedTime { get; set; }

        private string totalDataSizeBytes;
        public string TotalDataSizeBytes
        {
            get
            {
                return totalDataSizeBytes;
            }
            set
            {
                totalDataSizeBytes = value;
                totalDataSizeBytes = ConvertFromBytes(Convert.ToInt64(TotalDataSizeBytes));
            }
        }

        private string jobRateMBPerMinute;
        public string JobRateMBPerMinute
        {
            get
            {
                return jobRateMBPerMinute;
            }
            set
            {
                jobRateMBPerMinute = value;
                if (JobRateMBPerMinute.Equals("-1"))
                {
                    jobRateMBPerMinute = " ";
                }              
            }
        }

        private string errorCode;
        public string ErrorCode
        {
            get { return errorCode; }
            set
            {
                errorCode = value;
                errorCode = Int32.Parse(errorCode).ToString("X");
            }
        }

        public string MediaServerName { get; set; }

        public string AgentVersion = CommonSettings.Agent;

        static string ConvertFromBytes(long bytes)
        {
            if (bytes < 1024)
            {
                string dataSize = bytes.ToString("0.00");
                return dataSize + " Bytes";
            }
            else if(bytes>=1024 && bytes < 1048576)
            {                
                return (bytes / 1024f).ToString("0.00")+ " KB";
            }
            else if (bytes >= 1048576 && bytes < 1073741824 )
            {
                return ((bytes / 1024f) / 1024f).ToString("0.00")+ " MB";
            }
            else if(bytes>= 1073741824)
            {
                return (((bytes / 1024f) / 1024f) /1024f).ToString("0.00") + " GB";
            }
            return bytes.ToString("0.00") + "Bytes";
        }
    }
}
