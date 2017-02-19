/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace BEArcus.Agent
{
    class MediaServerController
    {

        public static void SaveMediaServerData(IStorageService storageType, string storageId)
        {
            LogUtility.LogInfoFunction("Entered SaveMediaServerData.");
            MediaServer mediaServerObject = GetMediaSeverData();

            if (mediaServerObject != null)
            {
                storageType.SaveMediaServerData(mediaServerObject, storageId);
            }
            else
            {
                LogUtility.LogInfoFunction("No media server record fetched.");
            }
            LogUtility.LogInfoFunctionFinished();
        }


        /// <summary>
        /// Gets the Media Server Name by invoking the PowerShell command.
        /// </summary>
        /// <returns>Media server Name.</returns>
        public static string GetMediaSever()
        {
            LogUtility.LogInfoFunction("Entered GetMediaServer.");
            LogUtility.LogInfoFunction("Calling BemcliHelper function LoadPowerShellScript(); ");
            PowerShell powershell = BemcliHelper.LoadPowerShellScript();

            LogUtility.LogInfoFunction("Invoking the PowerShell command GetLocalMediaServer()");
            powershell.AddCommand("GetLocalMediaServer");
            powershell.AddCommand("Out-String");
            var results = powershell.Invoke<string>();

            //check if the results string is empty
            if (String.IsNullOrWhiteSpace(results[0]))
            {
                //If the string is empty, there were no records found. Threfore return an empty Alert List.
                LogUtility.LogInfoFunction("Media Server Name not fetched");
                LogUtility.LogInfoFunctionFinished();
                return null;
            }

            //If records are found pass it to ConvertFromJson.
            var mediaServer = ConvertFromJson(results[0]);
            LogUtility.LogInfoFunctionFinished();
            return mediaServer.Name;
        }

        public static MediaServer GetMediaSeverData()
        {
            LogUtility.LogInfoFunction("Entered GetMediaServerData.");
            LogUtility.LogInfoFunction("Calling BemcliHelper function LoadPowerShellScript(); ");
            PowerShell powershell = BemcliHelper.LoadPowerShellScript();

            LogUtility.LogInfoFunction("Invoking the PowerShell command GetLocalMediaServer()");
            powershell.AddCommand("GetLocalMediaServer");
            powershell.AddCommand("Out-String");
            var results = powershell.Invoke<string>();

            //check if the results string is empty
            if (String.IsNullOrWhiteSpace(results[0]))
            {
                //If the string is empty, there were no records found. Threfore return an empty Alert List.
                LogUtility.LogInfoFunction("Media Server Data not fetched");
                LogUtility.LogInfoFunctionFinished();
                return null;
            }

            //If records are found pass it to ConvertFromJson.
            MediaServer mediaServer = ConvertFromJson(results[0]);
            LogUtility.LogInfoFunctionFinished();
            mediaServer.LastUpdate = DateTime.UtcNow;
            return mediaServer;
        }



        public static MediaServer ConvertFromJson(string jsonString)
        {
            LogUtility.LogInfoFunction("Entered ConvertFromJson.");
            LogUtility.LogInfoFunction("Calling JsonHelper function  JsonHelper.JsonDeserialize<MediaServer>(jsonString); ");
            var mediaServerObject = JsonHelper.JsonDeserialize<MediaServer>(jsonString);
            LogUtility.LogInfoFunctionFinished();
            return mediaServerObject;
        }
    }
}
