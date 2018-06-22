/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEArcus.WebApp.Models
{
    public class JobHistory
    {
        public string be_id { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string JobStatus { get; set; }
        public string JobType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string BackupExecServerName { get; set; }
        public string StorageName { get; set; }
        public string PercentComplete { get; set; }
        public ElapsedTime ElapsedTime { get; set; }
        [Display(Name = "Backup Data Size (Bytes)")]
        public string TotalDataSizeBytes { get; set; }
        [Display(Name = "Job Rate (MB/Min)")]
        public string JobRateMBPerMinute { get; set; }
        public string ErrorCode { get; set; }
        public string MediaServerName { get; set; }
    }
}