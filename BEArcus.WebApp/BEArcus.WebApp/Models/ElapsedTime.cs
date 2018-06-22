/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEArcus.WebApp.Models
{
    public class ElapsedTime
    {
        public string Ticks { get; set; }
        public string Days { get; set; }
        public string Hours { get; set; }
        public string Milliseconds { get; set; }
        public string Minutes { get; set; }
        public string Seconds { get; set; }
        public string TotalDays { get; set; }
        public string TotalHours { get; set; }
        public string TotalMilliseconds { get; set; }
        public string TotalMinutes { get; set; }
        public string TotalSeconds { get; set; }
    }
}