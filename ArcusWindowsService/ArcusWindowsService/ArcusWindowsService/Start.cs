/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Timers;

namespace BEArcus.Agent
{
    public class Start
    {
        private static Timer aTimer;

        public static void Init()
        {
            SetTimer();
        }

        /// <summary>
        /// Sets the timer for the time interval specified by the user in Configuration.xml file
        /// </summary>
        private static void SetTimer()
        {
            //Execute the Init method first time the user runs the Agent
            Main.Init();

            //Setting the timer
            int timeInterval = Int32.Parse(CommonSettings.Interval);
            aTimer = new Timer(timeInterval);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        /// <summary>
        /// Executes the Init method after the elapsed time.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            LogUtility.LogInfoFunction(string.Format("The Elapsed event was raised at {0}",
                              e.SignalTime));
            UserSettingsHelper.SetRunTime(DateTime.Now);
            Main.Init();
        }
    }
}
