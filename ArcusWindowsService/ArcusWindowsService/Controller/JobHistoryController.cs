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
    /// To fetch JobHistories from bemcli by properties like Name, Job Status, Job Type, Number, To and From dates       
    /// To save the fectched data to any storage media like file, database etc.
    /// </summary>    
    class JobHistoryController
    {
        public static DateTime currentTime;
        public static DateTime fetchEndTime;

        ///<summary>
        /// Calls SaveJobHistoryData() to save the data to a file , database or any other storage media based on  
        /// StorageType
        ///</summary>
        ///<param name="storageType">
        ///Storage type like file, DocumentDB database instance etc.
        /// </param>
        public static void SaveJobHistoryData(IStorageService storageType, string storageId)
        {
            LogUtility.LogInfoFunction("Entered SaveJobHistoryData.");
            //check the storage type to get the specific data from UserSettings.xml file
            if (storageType.GetType() == typeof(FileDataController))
            {
                LogUtility.LogInfoFunction("The storage type is FileSystem.");
                fetchEndTime = Convert.ToDateTime(UserSettingsHelper.FileGetJobHistoryFetchEndTime(storageId));
                UserSettingsHelper.FileSetJobHistoryFetchStartTime(storageId, fetchEndTime);
                currentTime = DateTime.Now;
                UserSettingsHelper.FileSetJobHistoryFetchEndTime(storageId, currentTime);
            }
            else
            {
                LogUtility.LogInfoFunction("The storage type is DocumentDB.");
                fetchEndTime = Convert.ToDateTime(UserSettingsHelper.GetJobHistoryFetchEndTime(storageId));
                UserSettingsHelper.DocumentDBSetJobHistoryFetchStartTime(storageId, fetchEndTime);
                currentTime = DateTime.Now;
                UserSettingsHelper.SetJobHistoryFetchEndTime(storageId, currentTime);
            }

            //List<JobHistory> jobHistoryObjects = GetJobHistory();
            List<JobHistory> jobHistoryObjects = GetJobHistoryByDate(fetchEndTime, currentTime);

            if (jobHistoryObjects != null)
            {
                LogUtility.LogInfoFunction("Calling  storageType.SaveJobHistoryData(jobHistoryObjects, storageId);");
                storageType.SaveJobHistoryData(jobHistoryObjects, storageId);
            }
            else
            {
                LogUtility.LogInfoFunction("No JobHistory records found.");
            }
            LogUtility.LogInfoFunctionFinished();
        }

        ///<Summary>
        ///Fetches JobHistories based on the parameters passed.
        ///If no parameters are specified returns all the Job Histories.
        /// </Summary>
        ///<param name="name">
        ///The name of the JobHistory
        ///</param>
        ///<param name="jobStatus">
        ///Status of the job
        ///</param>
        ///<param name="jobType">
        ///Type of the Job
        ///</param>
        ///<param name="number">
        ///The number of Job Histories to be fetched
        ///</param>
        public static List<JobHistory> GetJobHistory(string name = "", string jobStatus = "", string jobType = "", int? number = null)
        {
            LogUtility.LogInfoFunction("Entered GetJobHistory.");
            //For Loading the powershell scripts ( Main.ps1 ) in memory.
            LogUtility.LogInfoFunction("Calling BemcliHelper function LoadPowerShellScript(); ");
            PowerShell powershell = BemcliHelper.LoadPowerShellScript();

            //Calling the powershell function GetJobHistory(Name,JobStatus,JobType,Number) which is present in BEMCLIScripts\JobHistory.psm1
            LogUtility.LogInfoFunction("Invoking PowerShell Command GetJobHistory");
            powershell.AddCommand("GetJobHistory");
            powershell.AddParameter("Name", name);
            powershell.AddParameter("JobStatus", jobStatus);
            powershell.AddParameter("JobType", jobType);
            powershell.AddParameter("Number", number);
            powershell.AddCommand("Out-String");
            var results = powershell.Invoke<string>();

            //check if the results string is empty
            if (String.IsNullOrWhiteSpace(results[0]))
            {
                //If the string is empty, there were no records found. Threfore return null.
                LogUtility.LogInfoFunction("Job History List is empty");
                LogUtility.LogInfoFunctionFinished();
                return null;
            }

            //If records are found pass it to ConvertFromJson.
            string mediaServerName = MediaServerController.GetMediaSever();
            var jobHistoryObjects = ConvertFromJson(results[0]);
            foreach (JobHistory jobHistory in jobHistoryObjects)
            {
                jobHistory.MediaServerName = mediaServerName;
            }
            LogUtility.LogInfoFunctionFinished();
            return jobHistoryObjects;
        }

        ///<Summary>
        ///Fetches Job Histories based on the FromTime and ToTime and the number of Job Histories to be fetched.        
        ///</Summary>     
        public static List<JobHistory> GetJobHistoryByDate(DateTime fromDate, DateTime toDate, int? number = null)
        {
            LogUtility.LogInfoFunction("Entered GetJobHistoryByDate.");
            //For Loading the powershell scripts ( Main.ps1 ) in memory.
            LogUtility.LogInfoFunction("Calling BemcliHelper function LoadPowerShellScript(); ");
            BemcliHelper bemcliObject = new BemcliHelper();
            PowerShell powershell = BemcliHelper.LoadPowerShellScript();

            //Calling the powershell function GetJobHistory(FromTime,ToTime,Number) which is present in BEMCLIScripts\JobHistory.psm1
            LogUtility.LogInfoFunction("Invoking PowerShell Command GetJobHistoryByDate");
            powershell.AddCommand("GetJobHistoryByDate");
            powershell.AddParameter("FromTime", fromDate);
            powershell.AddParameter("ToTime", toDate);
            powershell.AddParameter("Number", number);
            powershell.AddCommand("Out-String");
            var results = powershell.Invoke<string>();

            //check if the results string is empty
            if (String.IsNullOrWhiteSpace(results[0]))
            {
                LogUtility.LogInfoFunction("Job History List is empty");
                //If the string is empty, there were no records found. Threfore return null.               
                LogUtility.LogInfoFunctionFinished();
                return null;
            }
            //If records are found pass it to ConvertFromJson.
            string mediaServerName = MediaServerController.GetMediaSever();
            var jobHistoryObjects = ConvertFromJson(results[0]);
            foreach (JobHistory jobHistory in jobHistoryObjects)
            {
                jobHistory.MediaServerName = mediaServerName;
            }
            LogUtility.LogInfoFunctionFinished();
            return jobHistoryObjects;
        }


        public static void ViewJobHistory(List<JobHistory> jobHistoryObjects)
        {
            foreach (JobHistory jobHistoryObject in jobHistoryObjects)
            {
                Console.WriteLine("\nNamg:\t {0} \n JobStatus:\t {1} \n JobTypg:\t {2}\n\n",
                                   jobHistoryObject.Name, jobHistoryObject.JobStatus, jobHistoryObject.JobType);
            }
        }

        //Converts the Json string into List of Alert Objects
        public static List<JobHistory> ConvertFromJson(string jsonString)
        {
            LogUtility.LogInfoFunction("Entered ConvertFromJson.");
            //To check if the JsonString contains an array of objects or a single object.
            var token = JToken.Parse(jsonString);
            if (token is JArray)
            {
                LogUtility.LogInfoFunction("Calling JsonHelper function JsonDeserialize<List<JobHistory>>(JsonString); ");
                var jobHistoryObjects = JsonHelper.JsonDeserialize<List<JobHistory>>(jsonString);
                LogUtility.LogInfoFunctionFinished();
                return jobHistoryObjects;
            }
            else
            {
                LogUtility.LogInfoFunction("Calling JsonHelper function JsonDeserialize<JobHistory>(JsonString); ");
                var jobHistoryObjects = JsonHelper.JsonDeserialize<JobHistory>(jsonString);
                List<JobHistory> result = new List<JobHistory>();
                result.Add(jobHistoryObjects);
                LogUtility.LogInfoFunctionFinished();
                return result;
            }
        }
    }
}
