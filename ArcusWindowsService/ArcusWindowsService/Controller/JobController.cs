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
    /// To fetch Jobs from bemcli by properties like Name, Job Type, Task Type, Number      
    /// To save the fectched data to any storage media like file, database etc.
    /// </summary>      
    class JobController
    {
        ///<summary>
        ///Calls SaveJobData() to save the data to a file , database or any other storage media based on  persistanceStorageDependency
        ///</summary>
        ///<param name="storageType">
        ///Storage type like file, DocumentDB database instance etc.
        /// </param>
        public static void SaveJobData(IStorageService storageType, string storageId)
        {
            LogUtility.LogInfoFunction("Entered SaveJobData.");
            List<Job> jobObjects = GetJob();
            if (jobObjects != null)
            {
                storageType.SaveJobData(jobObjects, storageId);
            }
            else
            {
                LogUtility.LogInfoFunction("No records found.");
            }
            LogUtility.LogInfoFunctionFinished();
        }

        ///<Summary>
        ///Fetches Jobs based on the parameters passed.
        ///If no parameters are specified returns all the Jobs.
        ///</Summary>
        ///<param name="name">
        ///The name of the Job
        ///</param>
        ///<param name="jobType">
        ///The JobType of the Job
        ///</param>
        ///<param name="taskType">
        ///The Task type of the Job
        ///</param>
        ///<param name="number">
        ///The number of jobs to be fetched
        ///</param>
        public static List<Job> GetJob(string name = "", string jobType = "", string taskType = "", int? number = null)
        {
            LogUtility.LogInfoFunction("Entered GetJob.");
            //For Loading the powershell scripts ( Main.ps1 ) in memory.
            LogUtility.LogInfoFunction("Calling BemcliHelper function LoadPowerShellScript(); ");
            PowerShell powershell = BemcliHelper.LoadPowerShellScript();

            //Calling the powershell function GetJObs(Name,JobType,TaskType,Number) which is present in BEMCLIScripts\Jobs.psm1
            LogUtility.LogInfoFunction("Invoking PowerShell Command GetJobs(Name,JobType,TaskType,Number) ");
            powershell.AddCommand("GetJobs");
            powershell.AddParameter("Name", name);
            powershell.AddParameter("JobType", jobType);
            powershell.AddParameter("TaskType", taskType);
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
            var jobObjects = ConvertFromJson(results[0]);
            foreach (Job job in jobObjects)
            {
                job.MediaServerName = mediaServerName;
            }
            LogUtility.LogInfoFunctionFinished();
            return jobObjects;
        }

        ///<Summary>
        ///To display the Jobs on console
        ///</Summary>  
        public static void ViewJobs(List<Job> jobObjects)
        {
            foreach (Job jobObject in jobObjects)
            {
                Console.WriteLine("\nNamg:\t {0} \n Id:\t {1} \n TaskNamg:\t {2}\n\n",
                                  jobObject.Name, jobObject.Id, jobObject.TaskName);
            }
        }

        //Converts the Json string into List of Job Objects
        public static List<Job> ConvertFromJson(string jsonString)
        {
            LogUtility.LogInfoFunction("Entered ConvertFromJson.");
            //To check if the JsonString contains an array of objects or a single object.
            var token = JToken.Parse(jsonString);
            if (token is JArray)
            {
                LogUtility.LogInfoFunction("Calling JsonHelper function JsonDeserialize<List<Job>>(jsonString); ");
                var jobObjects = JsonHelper.JsonDeserialize<List<Job>>(jsonString);
                LogUtility.LogInfoFunctionFinished();
                return jobObjects;
            }
            else
            {
                LogUtility.LogInfoFunction("Calling JsonHelper function JsonDeserialize<Job>(jsonString); ");
                var jobObjects = JsonHelper.JsonDeserialize<Job>(jsonString);
                List<Job> result = new List<Job>();
                result.Add(jobObjects);
                LogUtility.LogInfoFunctionFinished();
                return result;
            }
        }
    }
}
