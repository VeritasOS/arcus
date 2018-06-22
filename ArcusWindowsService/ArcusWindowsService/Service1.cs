/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using BEArcus.Agent;
using Logging;
using System;
using System.Management.Automation;
using System.ServiceProcess;
using System.Threading;

namespace ArcusWindowsService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var thread = new Thread(ServiceStart.StartService);
            thread.Name = "ArcusWorker";
            thread.IsBackground = false;
            thread.Start();
        }

        protected override void OnStop()
        {
            LogUtility.LogInfoFunction("Windows Service stoped");
        }
    }
}
