/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEArcus.WebApp.Models
{
    public class Alert
    {
        public string Name { get; set; }
        public string be_id { get; set; }
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Severity { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public Storage Storage { get; set; }
        public string BackupExecServerName { get; set; }
        public string Source { get; set; }
        public string MediaServerName { get; set; }
        public string Umi { get; set; }
    }
}