/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using ArcusWindowsService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BEArcus.Agent
{
    /// <summary>
    /// Contains methods:
    /// To save Alert, Job and JobHistory Objects to Json files.
    /// </summary>       

    public class FileDataController : IStorageService
    {
        ///<Summary> 
        ///Saves the Alerts to a Json file.
        ///</Summary>       
        public void SaveAlertData(List<Alert> alertObject, string fileSystemName)
        {
            string jsonstr = JsonHelper.JsonSerializer<List<Alert>>(alertObject);
            string jsonFilePath = GetFileSystemType(fileSystemName).AlertPath;
            LogUtility.LogInfoFunction("Alert:Writing to Json file.");
            System.IO.File.WriteAllText(jsonFilePath, jsonstr);
            UserSettingsHelper.FileSetAlertLastUpdateTime(fileSystemName, DateTime.Now);
            LogUtility.LogInfoFunction(string.Format("Alerts in {0}.", jsonFilePath));
            LogUtility.LogInfoFunctionFinished();
        }

        ///<Summary>
        ///Saves Jobs to a Json file.
        ///</Summary>  
        public void SaveJobData(List<Job> jobObject, string fileSystemName)
        {
            string jsonstr = JsonHelper.JsonSerializer<List<Job>>(jobObject);
            string jsonFilePath = GetFileSystemType(fileSystemName).JobPath;
            LogUtility.LogInfoFunction("Deleting the existing file contents.");
            System.IO.File.WriteAllText(jsonFilePath, string.Empty);
            LogUtility.LogInfoFunction("Job:Writing to Json file.");
            System.IO.File.WriteAllText(jsonFilePath, jsonstr);
            UserSettingsHelper.FileSetJobHistoryLastUpdateTime(fileSystemName, DateTime.Now);
            LogUtility.LogInfoFunction(string.Format("Jobs in {0}.", jsonFilePath));
            LogUtility.LogInfoFunctionFinished();
        }

        ///<Summary>
        ///Saves JobHistory to a Json file.
        ///</Summary>  
        public void SaveJobHistoryData(List<JobHistory> jobHistoryObject, string fileSystemName)
        {
            string jsonstr = JsonHelper.JsonSerializer<List<JobHistory>>(jobHistoryObject);
            string jsonFilePath = GetFileSystemType(fileSystemName).JobHistoryPath;
            LogUtility.LogInfoFunction("JobHistory:Writing to Json file.");
            System.IO.File.WriteAllText(jsonFilePath, jsonstr);
            UserSettingsHelper.FileSetJobHistoryLastUpdateTime(fileSystemName, DateTime.Now);

            LogUtility.LogInfoFunction(string.Format("JobHistories in {0}.", jsonFilePath));
            LogUtility.LogInfoFunctionFinished();
        }

        public void SaveMediaServerData(MediaServer mediaServerObject, string fileSystemName)
        {
            string jsonstr = JsonHelper.JsonSerializer<MediaServer>(mediaServerObject);
            string jsonFilePath = GetFileSystemType(fileSystemName).MediaServerPath;
            LogUtility.LogInfoFunction("MediaServer:Writing to Json file.");
            System.IO.File.WriteAllText(jsonFilePath, jsonstr);
            LogUtility.LogInfoFunction(string.Format("JobHistories in {0}.", jsonFilePath));
            LogUtility.LogInfoFunctionFinished();
        }

        /// <summary>
        /// Gets the file system type object for the given file system name.
        /// </summary>
        /// <param name="fileSystemName">Id for the file system</param>
        /// <returns></returns>
        public FileSystemType GetFileSystemType(string fileSystemName)
        {
            LogUtility.LogInfoFunction("Getting FileSystemType");
            LogUtility.LogInfoFunctionFinished();
            return Configuration.Instance.DataStores.FileSystem.
                                  Where(s => s.Name.Equals(fileSystemName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

        }
    }
}
