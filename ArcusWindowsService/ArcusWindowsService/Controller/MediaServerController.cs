/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Data;
using System.Xml;
using System.IO;
using System.Text;


namespace BEArcus.Agent
{
    class MediaServerController
    {

        public static void SaveMediaServerData(IStorageService storageType, string storageId)
        {
            LogUtility.LogInfoFunction("Entered SaveMediaServerData.");
            MediaServer mediaServerObject = GetMediaSeverData();
            double capacity = 0;

            //We are calculating Estimated Capacity Usage using job log files.
            //Whenever BEMCLI commands are available, we will use the interface to calculate Estimated Capacity Usage.
            //Arcus is calculating approximate Estimated Capacity Usage and might not exactly match with BE.

            try
            {
                DateTime fromDate = DateTime.Now.AddDays(-30);
                DateTime toDate = DateTime.Now;
                int? number = null;

                LogUtility.LogInfoFunction("Entered GetSuccessfullJobHistoryByDate.");
                //For Loading the powershell scripts ( Main.ps1 ) in memory.
                LogUtility.LogInfoFunction("Calling BemcliHelper function LoadPowerShellScript(); ");
                BemcliHelper bemcliObject = new BemcliHelper();
                PowerShell powershell = BemcliHelper.LoadPowerShellScript();

                //Calling the powershell function GetSuccessfullJobHistoryByDate(FromTime,ToTime,Number) which is present in BEMCLIScripts\MediaServer.psm1
                LogUtility.LogInfoFunction("Invoking PowerShell Command GetSuccessfullJobHistoryByDate");
                powershell.AddCommand("GetSuccessfullJobHistoryByDate");
                powershell.AddParameter("FromTime", fromDate);
                powershell.AddParameter("ToTime", toDate);
                powershell.AddParameter("Number", number);
                powershell.AddCommand("Out-String");
                var results = powershell.Invoke<string>();

                //check if the results string is empty
                if (String.IsNullOrWhiteSpace(results[0]))
                {
                    LogUtility.LogInfoFunction("Successfull Job History List is empty");
                    //If the string is empty, there were no records found. Threfore return null.               
                    LogUtility.LogInfoFunctionFinished();
                }

                else
                {
                    //Converting json document into object format
                    List<JobHistory> jobHistoryObjects = JobHistoryController.ConvertFromJson(results[0]);
                    string resourcePath = "";
                    Dictionary<string, double> resourcesKeyValue = new Dictionary<string, double>();

                    foreach (JobHistory jobHistory in jobHistoryObjects)
                    {
                        string pathnew = jobHistory.JobLogFilePath;
                        XmlDocument Docs = new XmlDocument();

                        //Opening Log file
                        Docs.Load(pathnew);
                        XmlElement root = Docs.DocumentElement;
                        XmlNodeList nodes = root.SelectNodes("//backup/machine/set");

                        foreach (XmlNode node in nodes)
                        {
                            //Get the resource name
                            resourcePath = node["set_resource_name"].InnerText.ToString();

                            XmlNode consumedDataNode = root.SelectSingleNode("//backup/machine/set/summary");
                            //Get the consumed data
                            string consumedDataSummary = consumedDataNode["new_processed_bytes"].InnerText.ToString();

                            //Convert consumed data into appropriate format
                            consumedDataSummary = consumedDataSummary.Replace(",", "");
                            string consumedResourceData = new string(consumedDataSummary.SkipWhile(c => !char.IsDigit(c)).TakeWhile(c => char.IsDigit(c)).ToArray());
                            double consumedResourceDataSize = double.Parse(consumedResourceData);

                            //Convert consumed data into GB
                            //We will always display Estimated Capacity usage in GB
                            consumedResourceDataSize = consumedResourceDataSize / 1024 / 1024 / 1024;

                            resourcePath = resourcePath.Trim().ToLower();
                            if (resourcesKeyValue.ContainsKey(resourcePath)) // Check if path exist in dictionary, update the data consumed value  
                            {
                                double value = resourcesKeyValue[resourcePath];
                                if (consumedResourceDataSize > value)
                                {
                                    resourcesKeyValue[resourcePath] = consumedResourceDataSize;
                                }
                            }
                            else
                            {
                                resourcesKeyValue.Add(resourcePath, consumedResourceDataSize);  // if a path not added in dictionary, here we are adding   
                            }
                        }
                    }

                    foreach (KeyValuePair<string, double> kvp in resourcesKeyValue)
                    {
                        // Calculate Estimated Capacity Usage
                        capacity = capacity + kvp.Value;
                    }

                }
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                LogUtility.LogInfoFunction("Error:" + e.Message + "Message:" + baseException.Message);
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }

            if (mediaServerObject != null)
            {
                string usedData = string.Format("{0:N3}", capacity);
                //We will always display Estimated Capacity usage in GB
                usedData = usedData + " GB";
                mediaServerObject.EstimatedUsedCapacity = usedData;
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
