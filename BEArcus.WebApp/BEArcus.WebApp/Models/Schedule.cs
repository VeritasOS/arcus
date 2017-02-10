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
    public class Schedule
    {
        public string RecurrenceType { get; set; }
        public string LocalizedScheduleString { get; set; }
        public string DaysOfWeek { get; set; }
        public string Every { get; set; }
        public DateTime StartDate { get; set; }
        public string DatesToExclude { get; set; }
        public string DatesToInclude { get; set; }
    }
}