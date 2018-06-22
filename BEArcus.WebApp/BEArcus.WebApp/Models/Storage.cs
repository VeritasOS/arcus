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
    public class Storage
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string StorageType { get; set; }
        public string MemberDevices { get; set; }
        public string Description { get; set; }
        public string IsSystemDefined { get; set; }
        public string SelectionMethod { get; set; }
        public string UsesDataLifecycleManagement { get; set; }
    }
}