/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace BEArcus.Agent
{
    /// <summary>
    /// Contains methods:
    /// To fetch Alerts from bemcli by properties like Name,Severity,Category,Number,To and From dates       
    /// To save the fectched data to any storage media like file, database etc.
    /// </summary>
    class AlertController
    {
        public static DateTime currentTime;
        public static DateTime fetchEndTime;

        ///<summary>
        /// Calls SaveAlertData() to save the data to a file , database or any other storage media based on  
        /// storageType
        ///</summary>
        ///<param name="storageType">
        ///Storage type like file, DocumentDB database instance etc.
        /// </param>
        public static void SaveAlertData(IStorageService storageType, string storageId)
        {
            LogUtility.LogInfoFunction("Entered SaveAlertData.");
            if (storageType.GetType() == typeof(FileDataController))
            {
                fetchEndTime = Convert.ToDateTime(UserSettingsHelper.FileGetAlertFetchEndTime(storageId));
                UserSettingsHelper.FileSetAlertFetchStartTime(storageId, fetchEndTime);
                currentTime = DateTime.Now;
                UserSettingsHelper.FileSetAlertFetchEndTime(storageId, currentTime);
            }
            else
            {
                fetchEndTime = Convert.ToDateTime(UserSettingsHelper.GetFetchEndTime(storageId));
                UserSettingsHelper.DocumetDBSetAlertFetchStartTime(storageId, fetchEndTime);
                currentTime = DateTime.Now;
                UserSettingsHelper.SetFetchEndTime(storageId, currentTime);
            }

            List<Alert> alertObjects = GetAlertByDate(fetchEndTime, currentTime);

            if (alertObjects != null)
            {
                storageType.SaveAlertData(alertObjects, storageId);
            }
            else
            {
                LogUtility.LogInfoFunction("No Alert records found.");
            }
            LogUtility.LogInfoFunctionFinished();
        }

        ///<Summary>
        ///Fetches Alerts based on the parameters passed.
        ///If no parameters are specified returns all the Alerts.
        /// </Summary>
        ///<param name="name">
        ///The name of the Alert
        ///</param>
        ///<param name="severity">
        ///The Severity of the Alert
        ///</param>
        ///<param name="category">
        ///The Category of the Alert
        ///</param>
        ///<param name="number">
        ///The number of alerts to be fetched
        ///</param>
        public static List<Alert> GetAlert(string name = "", string severity = "", string category = "", int? number = null)
        {
            LogUtility.LogInfoFunction("Entered GetAlert.");
            //For Loading the powershell scripts ( Main.ps1 ) in memory.
            LogUtility.LogInfoFunction("Calling BemcliHelper function LoadPowerShellScript(); ");
            PowerShell powershell = BemcliHelper.LoadPowerShellScript();

            //Calling the powershell function GetAlerts(Name,Severity,Category,Number) which is present in BEMCLIScripts\Alerts.psm1
            LogUtility.LogInfoFunction("Invoking PowerShell Command GetAlerts(Name,Severity,Category,Number) ");
            powershell.AddCommand("GetAlerts");
            powershell.AddParameter("Name", name);
            powershell.AddParameter("Severity", severity);
            powershell.AddParameter("Category", category);
            powershell.AddParameter("Number", number);
            powershell.AddCommand("Out-String");
            var results = powershell.Invoke<string>();

            //check if the results string is empty
            if (String.IsNullOrWhiteSpace(results[0]))
            {
                //If the string is empty, there were no records found. Threfore return null.              
                LogUtility.LogInfoFunctionFinished();
                return null;
            }
            //If records are found pass it to ConvertFromJson.
            string mediaServerName = MediaServerController.GetMediaSever();
            var alertObjects = ConvertFromJson(results[0]);
            foreach (Alert alert in alertObjects)
            {
                alert.MediaServerName = mediaServerName;
            }
            LogUtility.LogInfoFunctionFinished();
            return alertObjects;
        }

        ///<Summary>
        ///Fetches Alerts based on the FromTime and ToTime and the number of Alerts to be fetched.        
        ///</Summary>     
        public static List<Alert> GetAlertByDate(DateTime fromDate, DateTime toDate, int? number = null)
        {
            LogUtility.LogInfoFunction("Entered GetAlertByDate.");
            //For Loading the powershell scripts ( Main.ps1 )in memory.
            LogUtility.LogInfoFunction("Call to BemcliHelper.LoadPowerShellScript();");
            PowerShell powershell = BemcliHelper.LoadPowerShellScript();

            LogUtility.LogInfoFunction("Invoking PowerShell Command GetAlertsByDate");
            powershell.AddCommand("GetAlertsByDate");
            powershell.AddParameter("FromTime", fromDate);
            powershell.AddParameter("ToTime", toDate);
            powershell.AddParameter("Number", number);
            powershell.AddCommand("Out-String");
            var results = powershell.Invoke<string>();
            //check if the results string is empty
            if (String.IsNullOrWhiteSpace(results[0]))
            {
                LogUtility.LogInfoFunctionFinished();
                //If the string is empty, there were no records found. Threfore return null.               
                return null;
            }
            string mediaServerName = MediaServerController.GetMediaSever();
            //If records are found pass it to ConvertFromJson.               
            var alertObjects = ConvertFromJson(results[0]);
            //To det the Media server name.
            foreach (Alert alert in alertObjects)
            {
                alert.MediaServerName = mediaServerName;
            }
            LogUtility.LogInfoFunctionFinished();
            return alertObjects;
        }

        ///<Summary>
        ///To display the Alerts on console
        ///</Summary>  
        public static void ViewAlerts(List<Alert> alertObjects)
        {
            LogUtility.LogInfoFunction("Entered ViewAlerts.");
            foreach (Alert alertObject in alertObjects)
            {
                Console.WriteLine("\nNamg:\t {0} \n Id:\t {1} \n Datg:\t {2}\n\n",
                                 alertObject.Name, alertObject.Id, alertObject.Date);
            }
            LogUtility.LogInfoFunctionFinished();
        }

        ///<Summary> 
        ///Checks if the Json string contains an array or a single object and returns a List of Alert Object/s
        ///</Summary>  
        public static List<Alert> ConvertFromJson(string jsonString)
        {
            LogUtility.LogInfoFunction("Entered ConvertFromJson.");
            //To check if the JsonString contains an array of objects / a single object.            
            var token = JToken.Parse(jsonString);
            if (token is JArray)
            {
                LogUtility.LogInfoFunction("Calling JsonHelper function JsonDeserialize<List<Alert>>(JsonString); ");
                var alertObjects = JsonHelper.JsonDeserialize<List<Alert>>(jsonString);
                LogUtility.LogInfoFunctionFinished();
                return alertObjects;
            }
            else
            {
                LogUtility.LogInfoFunction("Calling JsonHelper function JsonDeserialize<List<Alert>>(JsonString); ");
                var alertObjects = JsonHelper.JsonDeserialize<Alert>(jsonString);
                List<Alert> result = new List<Alert>();
                result.Add(alertObjects);
                LogUtility.LogInfoFunctionFinished();
                return result;
            }
        }
    }
}
