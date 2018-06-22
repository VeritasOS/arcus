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
    public class MediaServer
    {
        public string be_id { get; set; }
        [Display(Name = "Media Server")]
        public string Name { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        public DateTime LastUpdate { get; set; }

        public string Version { get; set; }
        public string EstimatedUsedCapacity { get; set; }

    }
}