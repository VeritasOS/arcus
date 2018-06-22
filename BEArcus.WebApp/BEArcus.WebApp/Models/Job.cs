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
    public class Job
    {
        public string Name { get; set; }             
        public string be_id { get; set; }
        [Display(Name = "Backup Type")]
        public string Id { get; set; }
        public string TaskName { get; set; }      
        public string JobType { get; set; }        
        public Storage Storage { get; set; }      
        public string Status { get; set; }       
        public Schedule Schedule { get; set; }
        public string SelectionSummary { get; set; }
        public string Priority { get; set; }
        public string MediaServerName { get; set; }
    }
}