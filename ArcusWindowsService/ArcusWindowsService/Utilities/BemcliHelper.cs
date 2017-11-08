/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace BEArcus.Agent
{
    //System.Management.Automation

    ///<summary>
    ///Contains method:
    ///To Load the powershell script Main.ps1.
    /// </summary>
    class BemcliHelper
    {
        static string bemcliScriptPath = CommonSettings.BemcliPath;
        static string str = System.IO.File.ReadAllText(bemcliScriptPath);

        public static PowerShell LoadPowerShellScript()
        {
            try
            {
                LogUtility.LogInfoFunction("Entered LoadPowerShellScript.");
                PowerShell powershell = PowerShell.Create();

                powershell.AddScript(str, false);

                LogUtility.LogInfoFunction("Loading powershell scripts.");
                Collection<PSObject> obj = powershell.Invoke();

                powershell.Commands.Clear();

                LogUtility.LogInfoFunctionFinished();
                return powershell;
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                LogUtility.LogInfoFunction("Error:" + e.Message + "Message:" + baseException.Message);
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
                return null;
            }
        }
    }
}
