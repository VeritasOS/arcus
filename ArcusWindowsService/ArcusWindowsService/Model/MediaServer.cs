/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace BEArcus.Agent
{
    public class MediaServer
    {
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

        public string CustomerName = CommonSettings.Customer;

       
        public DateTime LastUpdate
        {
            get;
            set;
        }

        public string Version { get; set; }

        public string AgentVersion = CommonSettings.Agent;

    }
}
