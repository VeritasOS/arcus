/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BEArcus.Agent
{
    class Main
    {
        /// <summary>
        /// Checks if the File system type and DocumentDB type is enabled 
        /// and calls the respective handler methods.
        /// </summary>
        public static void Init()
        {
            try
            {
                LogUtility.CheckLogFileEnabled();

                //Get the file Systems that are enabled.            
                IEnumerable<BEArcus.Agent.FileSystemType> fileSystems = Configuration.Instance.DataStores.FileSystem.
                                                                                          Where(f => f.Enabled);
                LogUtility.LogInfoFunction("Storing data to enbled File Systems");
                IStorageService fileservice = new FileDataController();
                foreach (FileSystemType fileSystem in fileSystems)
                {
                    UserSettingsHelper.CreateFileSystemSettings(fileSystem.Name);
                    AlertController.SaveAlertData(fileservice, fileSystem.Name);
                    JobController.SaveJobData(fileservice, fileSystem.Name);
                    JobHistoryController.SaveJobHistoryData(fileservice, fileSystem.Name);
                    MediaServerController.SaveMediaServerData(fileservice, fileSystem.Name);
                }

                //Get the DocumentDB data store that are enabled.
                IEnumerable<BEArcus.Agent.DocumentDBType> documentDBStreams = Configuration.Instance.DataStores.DocumentDB.
                                                                                        Where(d => d.Enabled);
                LogUtility.LogInfoFunction("Storing data to enbled DocumentDB accounts.");
                //IStorageService documentDBService = new DocumentDBDataController();
                foreach (DocumentDBType documentDB in documentDBStreams)
                {
                    UserSettingsHelper.CreateDocumentDBSettings(documentDB.Name);

                    //Decrypt the Authorization Key to check if it was encryped earlier
                    string decrypedAuthorizationKey = SecurityController.Decrypt(documentDB.AuthorizationKey);

                    //Decrypt method returns null if the Authorization key was not Encrypted earlier
                    if (string.IsNullOrEmpty(decrypedAuthorizationKey))
                    {
                        //Encrypt Authorization Key
                        string encryptedAuthorizationKey = SecurityController.Encrypt(documentDB.AuthorizationKey);
                        //Encrypt EndpointUrl
                        string encryptedEndpointUrl = SecurityController.Encrypt(documentDB.EndPointUrl);
                        //Save changes to Configuration.xml
                        SecurityController.UpdateConfiguration(encryptedEndpointUrl, encryptedAuthorizationKey);
                    }
                    IStorageService documentDBService = new DocumentDBDataController();
                    AlertController.SaveAlertData(documentDBService, documentDB.Name);
                    JobController.SaveJobData(documentDBService, documentDB.Name);
                    JobHistoryController.SaveJobHistoryData(documentDBService, documentDB.Name);
                    MediaServerController.SaveMediaServerData(documentDBService, documentDB.Name);
                    PurgeDataController.PurgeAlerts(documentDB.Name);
                    PurgeDataController.PurgeJobHistories(documentDB.Name);
                }
                LogUtility.LogInfoFunctionFinished();
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                LogUtility.LogInfoFunction("Error:" + e.Message + "Message:" + baseException.Message);
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
        }
    }
}