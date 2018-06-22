/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using BEArcus.Agent;
using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace ArcusWindowsService
{
    public class ServiceStart
    {
        public static void StartService()
        {

            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            ILog Log = Logger.Instance;
            try
            {

                LogUtility.LogInfoFunction("Windows Service started");
                if (!File.Exists(@".\UserSettings.xml"))
                {
                    //Create User settings file
                    UserSettingsHelper.CreateSettingsFile();
                }
                LogUtility.CheckLogFileEnabled();

                //For Loading the powershell scripts ( Main.ps1 )in memory to check if supported version of BE is installed.
                LogUtility.LogInfoFunction("Call to BemcliHelper.LoadPowerShellScript(); (To check if supported version of BE is installed.)");
                PowerShell powershell = BemcliHelper.LoadPowerShellScript();
                if (powershell.HadErrors)
                {
                    LogUtility.LogInfoFunction("Check the BE Version installed. Supported versions are 14.0, 14.1, 14.2 and 16.0");
                    Console.ReadLine();
                }
                else
                {
                    Start.Init();
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                LogUtility.LogInfoFunction("Error:" + e.Message + "Message:" + baseException.Message);
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
                Log.LogException(e);
                Console.ReadLine();
            }
        }


    }
}
