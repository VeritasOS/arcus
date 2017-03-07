/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using ArcusWindowsService;



namespace BEArcus.Agent
{
    /// <summary>
    /// Contains methods:
    /// To save Alert, Job and JobHistory Objects to DocumentDB database.
    /// </summary>
    public class DocumentDBDataController : IStorageService
    {
        private static readonly string databaseId = CommonSettings.Database;
        private static readonly string alertCollectionId = CommonSettings.AlertCollection;
        private static readonly string jobCollectionId = CommonSettings.JobCollection;
        private static readonly string jobHistoryCollectionId = CommonSettings.JobHistoryCollection;
        private static readonly string mediaServerCollectionId = CommonSettings.MediaServerCollection;
        private int purgeDBInterval = Int32.Parse(CommonSettings.PurgeDB);
        private static DocumentClient client;
        private string EndpointUrl;
        private string AuthorizationKey;
        public string currentMediaServer = MediaServerController.GetMediaSever();


        /// <summary>
        /// Gets the database if exists or creates a new database with the given Id.
        /// </summary>
        /// <param name="databaseId">
        /// Id for the Database.
        /// </param>
        /// <returns>Database Instance.</returns>
        public static async Task<Database> GetOrCreateDatabase(string databaseId)
        {
            try
            {
                LogUtility.LogInfoFunction("Entered GetOrCreateDatabase.");

                // Check to verify a database with the id=ArcusRegistry does not exist
                Database database = client.CreateDatabaseQuery().
                                    Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();

                // If the database does not exist, create a new database
                if (database == null)
                {
                    LogUtility.LogInfoFunction("Creating Database.");
                    database = await client.CreateDatabaseAsync(
                        new Database
                        {
                            Id = databaseId
                        });
                }
                LogUtility.LogInfoFunctionFinished();
                return database;
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                LogUtility.LogInfoFunction("Error:" + e.Message + "Message:" + baseException.Message);
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets the collection if exists or creates a new collection with the given Id.
        /// </summary>
        /// <param name="databaseId">Id for the Database.</param>
        /// <param name="collectionId">Id for the collecion.</param>
        /// <returns>Collecion Instance.</returns>
        public static async Task<DocumentCollection> GetOrCreateCollection(string databaseId, string collectionId)
        {
            try
            {
                LogUtility.LogInfoFunction("Entered GetOrCreateCollection.");

                // Check to verify a document collection with the id=collectionId does not exist
                DocumentCollection documentCollection = client.CreateDocumentCollectionQuery("dbs/" + databaseId).
                                                        Where(c => c.Id == collectionId).AsEnumerable().FirstOrDefault();

                // If the document collection does not exist, create a new collection
                if (documentCollection == null)
                {
                    LogUtility.LogInfoFunction("Creating Collecion");
                    documentCollection = await client.CreateDocumentCollectionAsync("dbs/" + databaseId,
                        new DocumentCollection
                        {
                            Id = collectionId
                        });

                    if (!collectionId.Equals("MediaServerCollection"))
                    {
                        documentCollection.PartitionKey.Paths.Add("/MediaServerName");
                    }
                }
                LogUtility.LogInfoFunctionFinished();
                return documentCollection;
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                LogUtility.LogInfoFunction("Error:" + e.Message + "Message:" + baseException.Message);
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
                return null;
            }

        }

        /// <summary>
        /// Saves the Alerts into the DocumentDB Collection with Id: "AlertCollection"
        /// </summary>
        /// <param name="alertObject">
        /// Contains the Alert data.
        /// </param>
        /// <param name="documentDBName">
        /// The documentDB name to determine to which EndPointUrl the data is to be stored.
        /// </param>
        public void SaveAlertData(List<Alert> alertObject, string documentDBName)
        {
            try
            {
                LogUtility.LogInfoFunction("Entered SaveAlertData.");

                List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
                EndpointUrl = list[0];
                AuthorizationKey = list[1];

                using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
                {
                    //To get or create the documentDB database and Alert collection.
                    Init(databaseId, alertCollectionId);
                    var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, alertCollectionId);
                    LogUtility.LogInfoFunction("Pushing Alerts into DocumentDB.");
                    foreach (Alert alert in alertObject)
                    {
                        //Push alert to AlertCollection. 
                        client.CreateDocumentAsync(collectionLink, alert).Wait();
                    }
                    LogUtility.LogInfoFunction("Alerts successfully stored in DocumentDB");
                }
                LogUtility.LogInfoFunction("Setting the Last Update time");
                UserSettingsHelper.SetLastUpdateTime(documentDBName, DateTime.Now);
                LogUtility.LogInfoFunctionFinished();
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                LogUtility.LogInfoFunction("Error:" + e.Message + "Message:" + baseException.Message);
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
        }

        /// <summary>
        /// Saves the Jobs into the DocumentDB Collection with Id: "JobCollection"
        /// </summary>
        /// <param name="jobObject">
        ///Contains the Job Data        
        /// </param>
        /// <param name="documentDBName">
        /// The documentDB name to determine to which EndPointUrl the data is to be stored.
        /// </param>
        public async void SaveJobData(List<Job> jobObject, string documentDBName)
        {
            try
            {
                LogUtility.LogInfoFunction("Entered SaveJobData.");
                List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
                EndpointUrl = list[0];
                AuthorizationKey = list[1];

                await DeleteJob(documentDBName);

                using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
                {
                    //To get or create the documentDB database and Job collection.
                    Init(databaseId, jobCollectionId);
                    var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, jobCollectionId);
                    LogUtility.LogInfoFunction("Pushing Jobs into DocumentDB.");
                    foreach (Job job in jobObject)
                    {
                        //Push job to JobCollection. 
                        await client.CreateDocumentAsync(collectionLink, job);
                    }
                    LogUtility.LogInfoFunction("Jobs successfully stored in DocumentDB");
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

        /// <summary>
        ///  Saves the Job Histories into the DocumentDB Collection with Id: "JobHistoryCollection"
        /// </summary>
        /// <param name="jobHistoryObject">
        /// Contains the JobHistory Data
        /// </param>
        /// <param name="documentDBName">
        /// The documentDB name to determine to which EndPointUrl the data is to be stored.
        /// </param>
        public void SaveJobHistoryData(List<JobHistory> jobHistoryObject, string documentDBName)
        {
            LogUtility.LogInfoFunction("Entered SaveJobHistoryData.");
            SaveJobHistory(jobHistoryObject, documentDBName).Wait();
            LogUtility.LogInfoFunctionFinished();
        }

        public async Task SaveJobHistory(List<JobHistory> jobHistoryObject, string documentDBName)
        {
            try
            {
                LogUtility.LogInfoFunction("Entered SaveJobHistory.");
                List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
                EndpointUrl = list[0];
                AuthorizationKey = list[1];

                using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
                {
                    //To get or create the documentDB database and JobHistory collection.
                    Init(databaseId, jobHistoryCollectionId);
                    var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, jobHistoryCollectionId);
                    LogUtility.LogInfoFunction("Pushing Job Histories into DocumentDB.");
                    foreach (JobHistory jobHistory in jobHistoryObject)
                    {
                        //Push job to JobHistoryCollection. 
                        await client.CreateDocumentAsync(collectionLink, jobHistory);
                    }
                    LogUtility.LogInfoFunction("Job Histories successfully stored in DocumentDB");
                }
                LogUtility.LogInfoFunction("Setting the Last Update time");
                UserSettingsHelper.SetJobHistoryLastUpdateTime(documentDBName, DateTime.Now);

                LogUtility.LogInfoFunctionFinished();
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                LogUtility.LogInfoFunction("Error:" + e.Message + "Message:" + baseException.Message);
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
        }

        /// <summary>
        /// Saves the Media Server details into the DocumentDB Collection with Id: "MediaServerCollection"
        /// </summary>
        /// <param name="mediaServerObject"></param>
        /// <param name="documentDBName"></param>
        public async void SaveMediaServerData(MediaServer mediaServerObject, string documentDBName)
        {
            try
            {
                LogUtility.LogInfoFunction("Entered SaveMediaServerData.");
                List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
                EndpointUrl = list[0];
                AuthorizationKey = list[1];

                DeleteMediaServer(documentDBName).Wait();

                using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
                {
                    //To get or create the documentDB database and MediaServer collection.
                    Init(databaseId, mediaServerCollectionId);
                    var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, mediaServerCollectionId);
                    LogUtility.LogInfoFunction("Pushing Job Histories into DocumentDB.");

                    //Push MediaServers to MediaServerCollection. 
                    await client.CreateDocumentAsync(collectionLink, mediaServerObject);

                    LogUtility.LogInfoFunction("Media Server Data successfully stored in DocumentDB");
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

        /// <summary>
        /// Delete the existing Jobs within JobCollection.
        /// </summary>
        /// <param name="documentDBName"></param>
        /// <returns></returns>
        public async Task DeleteJob(string documentDBName)
        {
            LogUtility.LogInfoFunction("Entered DeleteJob.");
            List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
            EndpointUrl = list[0];
            AuthorizationKey = list[1];

            using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
            {
                try
                {
                    Database db = client.CreateDatabaseQuery().
                              Where(o => o.Id == databaseId).AsEnumerable().FirstOrDefault();
                    var coll = client.CreateDocumentCollectionQuery(db.CollectionsLink).Where(c => c.Id == jobCollectionId).ToList().First();

                    var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, jobCollectionId);

                    var docs = client.CreateDocumentQuery(
                  collectionLink,
                  new SqlQuerySpec()
                  {
                      QueryText = "SELECT * FROM JobCollection c WHERE c.MediaServerName = @MediaServerName",
                      Parameters = new SqlParameterCollection()
                      {
                           new SqlParameter("@MediaServerName", MediaServerController.GetMediaSever() )
                      }
                  });

                    LogUtility.LogInfoFunction("Deleting Existing Jobs.");
                    foreach (Document doc in docs)
                    {
                        await client.DeleteDocumentAsync(doc.SelfLink);
                    }
                    LogUtility.LogInfoFunctionFinished();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    LogUtility.LogExceptionFunction(e);
                }
            }
            LogUtility.LogInfoFunctionFinished();
        }

        /// <summary>
        /// Delete the existing Media server records from MediaServerCollection. 
        /// </summary>
        /// <param name="documentDBName"></param>
        /// <returns></returns>
        public async Task DeleteMediaServer(string documentDBName)
        {
            LogUtility.LogInfoFunction("Entered DeleteMediaServer.");

            List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
            EndpointUrl = list[0];
            AuthorizationKey = list[1];

            using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
            {
                try
                {
                    Database db = client.CreateDatabaseQuery().
                              Where(o => o.Id == databaseId).AsEnumerable().FirstOrDefault();
                    var coll = client.CreateDocumentCollectionQuery(db.CollectionsLink).Where(c => c.Id == mediaServerCollectionId).ToList().First();

                    var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, mediaServerCollectionId);

                    var docs = client.CreateDocumentQuery(
                  collectionLink,
                  new SqlQuerySpec()
                  {
                      QueryText = "SELECT * FROM MediaServerCollection c WHERE c.Name = @MediaServerName",
                      Parameters = new SqlParameterCollection()
                      {
                           new SqlParameter("@MediaServerName", MediaServerController.GetMediaSever() )
                      }
                  });

                    LogUtility.LogInfoFunction("Deleting Existing Media Server.");
                    foreach (Document doc in docs)
                    {
                        await client.DeleteDocumentAsync(doc.SelfLink);
                    }
                    LogUtility.LogInfoFunctionFinished();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    LogUtility.LogExceptionFunction(e);
                }
            }
            LogUtility.LogInfoFunctionFinished();
        }

        /// <summary>
        /// Creates stored procedure to delete Alerts.
        /// </summary>
        /// <param name="documentDBName"></param>
        /// <returns></returns>
        public async Task<StoredProcedure> CreateAlertStoredProcedure(string documentDBName)
        {
            LogUtility.LogInfoFunction("Entered CreateAlertStoredProcedure.");
            List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
            EndpointUrl = list[0];
            AuthorizationKey = list[1];

            using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
            {
                // Get database
                //Database database = client.CreateDatabaseQuery().
                //                Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();

                // Get collection (Alert)
                DocumentCollection collection = client.CreateDocumentCollectionQuery("dbs/" + databaseId).
                                                    Where(c => c.Id == alertCollectionId).AsEnumerable().FirstOrDefault();

                //Check if the Stored Procedure exists.
                StoredProcedure storedProcedure = client.CreateStoredProcedureQuery(collection.SelfLink).Where(c => c.Id == "spDeleteAlert").AsEnumerable().FirstOrDefault();
                if (storedProcedure == null)
                {
                    // Create stored procedure
                    var sprocBody = File.ReadAllText(@".\Server\PurgeData.js");

                    var sprocDefinition = new StoredProcedure
                    {
                        Id = "spDeleteAlert",
                        Body = sprocBody
                    };

                    StoredProcedure sproc = await client.CreateStoredProcedureAsync(collection.SelfLink, sprocDefinition);
                    return sproc;
                }
                LogUtility.LogInfoFunctionFinished();
                return storedProcedure;
            }
        }

        /// <summary>
        /// Creates stored procedure to delete Job Histories.
        /// </summary>
        /// <param name="documentDBName"></param>
        /// <returns></returns>
        public async Task<StoredProcedure> CreateJobHistoryStoredProcedure(string documentDBName)
        {
            LogUtility.LogInfoFunction("Entered CreateJobHistoryStoredProcedure.");
            List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
            EndpointUrl = list[0];
            AuthorizationKey = list[1];

            using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
            {
                // Get database
                //Database database = client.CreateDatabaseQuery().
                //                Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();

                // Get collection (Job History)
                DocumentCollection collection = client.CreateDocumentCollectionQuery("dbs/" + databaseId).
                                                    Where(c => c.Id == jobHistoryCollectionId).AsEnumerable().FirstOrDefault();

                //Check if Stored Procedure Exists.
                StoredProcedure storedProcedure = client.CreateStoredProcedureQuery(collection.SelfLink).Where(c => c.Id == "spDeleteJobHistory").AsEnumerable().FirstOrDefault();
                if (storedProcedure == null)
                {
                    // Create stored procedure
                    var sprocBody = File.ReadAllText(@".\Server\PurgeData.js");
                    var sprocDefinition = new StoredProcedure
                    {
                        Id = "spDeleteJobHistory",
                        Body = sprocBody
                    };
                    StoredProcedure sproc = await client.CreateStoredProcedureAsync(collection.SelfLink, sprocDefinition);
                    LogUtility.LogInfoFunctionFinished();
                    return sproc;
                }
                LogUtility.LogInfoFunctionFinished();
                return storedProcedure;

            }
        }

        /// <summary>
        /// Excutes the stored procedure for deleting alerts.
        /// </summary>
        /// <param name="documentDBName"></param>
        /// <returns></returns>
        public async Task Execute_spBulkDeleteAlerts(string documentDBName)
        {
            LogUtility.LogInfoFunction("Entered Execute_spBulkDeleteAlerts.");
            List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
            EndpointUrl = list[0];
            AuthorizationKey = list[1];

            using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
            {
                //Change Indexing policy of AlertCollection.
                DocumentCollection collection = client.CreateDocumentCollectionQuery("dbs/" + databaseId).
                                                 Where(c => c.Id == alertCollectionId).AsEnumerable().FirstOrDefault();
                collection.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });
                await client.ReplaceDocumentCollectionAsync(collection);

                LogUtility.LogInfoFunction("Execute BulkDeleteAlerts");
                DateTime now = DateTime.Now;
                //To get one month before Date
                DateTime date = now.AddHours(-730);
                var thresholdDate = date.ToString("yyyy-MM-dd");

                // delete all documents that satisfy filter
                var sql = String.Format("Select * from AlertCollection where AlertCollection.Date<'{0}' and AlertCollection.MediaServerName='{1}'", thresholdDate, currentMediaServer);

                var count = await Execute_spBulkDelete(client, sql, "spDeleteAlert", collection);
                LogUtility.LogInfoFunction(String.Format("Deleted documents; count: {0}", count));
                LogUtility.LogInfoFunctionFinished();
            }
        }

        /// <summary>
        /// Executes the stored procedure to delete Job Histories.
        /// </summary>
        /// <param name="documentDBName"></param>
        /// <returns></returns>
        public async Task Execute_spBulkDeleteJobHistories(string documentDBName)
        {
            LogUtility.LogInfoFunction("Entered Execute_spBulkDeleteJobHistories.");
            List<string> list = GetEndpointUrlAndAuthorizationKey(documentDBName);
            EndpointUrl = list[0];
            AuthorizationKey = list[1];

            using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
            {
                //Change Indexing policy of JobHistoryCollection.
                DocumentCollection collection = client.CreateDocumentCollectionQuery("dbs/" + databaseId).
                                                 Where(c => c.Id == jobHistoryCollectionId).AsEnumerable().FirstOrDefault();
                collection.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });
                await client.ReplaceDocumentCollectionAsync(collection);

                LogUtility.LogInfoFunction("Execute BulkDeleteJobHistories");
                DateTime now = DateTime.Now;
                //To get one month before Date
                DateTime date = now.AddHours(-purgeDBInterval);
                var thresholdDate = date.ToString("yyyy-MM-dd");

                // delete all documents that satisfy filter
                var sql = String.Format("Select * from JobHistoryCollection where JobHistoryCollection.EndTime<'{0}' and JobHistoryCollection.MediaServerName='{1}'", thresholdDate, currentMediaServer);

                var count = await Execute_spBulkDelete(client, sql, "spDeleteJobHistory", collection);
                LogUtility.LogInfoFunction(String.Format("Deleted documents; count: {0}", count));
                LogUtility.LogInfoFunctionFinished();
            }
        }

        private async static Task<int> Execute_spBulkDelete(DocumentClient client, string sql, string sprocId, DocumentCollection collection)
        {
            LogUtility.LogInfoFunction("Entered Execute_spBulkDelete.");
            var continuationFlag = true;
            var totalDeletedCount = 0;
            while (continuationFlag)
            {
                var result = await ExecuteStoredProcedure<spBulkDeleteResponse>(client, sprocId, collection, sql);
                continuationFlag = result.ContinuationFlag;
                var deletedCount = result.Count;
                totalDeletedCount += deletedCount;

                LogUtility.LogInfoFunction(String.Format("Deleted {0} documents ({1} total, more: {2})", deletedCount, totalDeletedCount, continuationFlag));
            }
            LogUtility.LogInfoFunctionFinished();
            return totalDeletedCount;
        }

        private async static Task<T> ExecuteStoredProcedure<T>(DocumentClient client, string sprocId, DocumentCollection collection, params dynamic[] sprocParams)
        {
            LogUtility.LogInfoFunction("Entered ExecuteStoredProcedure.");
            var query = new SqlQuerySpec
            {
                QueryText = "SELECT * FROM c WHERE c.id = @id",
                Parameters = new SqlParameterCollection { new SqlParameter { Name = "@id", Value = sprocId } }
            };


            StoredProcedure sproc = client
                .CreateStoredProcedureQuery(collection.StoredProceduresLink, query)
                .AsEnumerable()
                .First();

            while (true)
            {
                try
                {
                    var result = await client.ExecuteStoredProcedureAsync<T>(sproc.SelfLink, sprocParams);

                    LogUtility.LogInfoFunction(String.Format("Executed stored procedure: {0}", sprocId));
                    LogUtility.LogInfoFunctionFinished();
                    return result;
                }
                catch (DocumentClientException ex)
                {
                    if ((int)ex.StatusCode == 429)
                    {
                        LogUtility.LogInfoFunction(String.Format("  ...retry in {0}", ex.RetryAfter));
                        Thread.Sleep(ex.RetryAfter);
                        continue;
                    }
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Calls the GetOrCreateDatabase() method and GetOrCreateCollection().
        /// </summary>
        /// <param name="databaseId">Database Id.</param>
        /// <param name="collectionId">Collection Id.</param>
        public static void Init(string databaseId, string collectionId)
        {
            LogUtility.LogInfoFunction("Entered Init.");
            GetOrCreateDatabase(databaseId).Wait();
            GetOrCreateCollection(databaseId, collectionId).Wait();
            LogUtility.LogInfoFunctionFinished();
        }

        /// <summary>
        /// Gets the DocumentDBType object for the provided documentDB Name.
        /// </summary>
        /// <param name="documentDBName">Unique Id for the documentDB.</param>
        /// <returns></returns>
        public DocumentDBType GetDocumentDBType(string documentDBName)
        {
            LogUtility.LogInfoFunction("Entered GetDocumentDBType.");
            LogUtility.LogInfoFunction("Getting DocumentDBType");
            LogUtility.LogInfoFunctionFinished();
            return Configuration.Instance.DataStores.DocumentDB.
                                  Where(d => d.Name.Equals(documentDBName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the decrypted Endpoint url and Authorization Key.
        /// </summary>
        /// <param name="documentDBName"></param>
        /// <returns></returns>
        public List<string> GetEndpointUrlAndAuthorizationKey(string documentDBName)
        {
            LogUtility.LogInfoFunction("Entered GetEndpointUrlAndAuthorizationKey.");
            List<string> list = new List<string>();
            string encryptedEndpointUrl = GetDocumentDBType(documentDBName).EndPointUrl;
            string decryptedEndpointUrl = SecurityController.Decrypt(encryptedEndpointUrl);
            string encryptedAuthorizationKey = GetDocumentDBType(documentDBName).AuthorizationKey;
            string decryptedAuthorizationKey = SecurityController.Decrypt(encryptedAuthorizationKey);

            if (string.IsNullOrEmpty(decryptedEndpointUrl))
            {
                list.Add(encryptedEndpointUrl);
                list.Add(encryptedAuthorizationKey);
                return list;
            }
            list.Add(decryptedEndpointUrl);
            list.Add(decryptedAuthorizationKey);
            LogUtility.LogInfoFunctionFinished();
            return list;
        }

        public class spBulkDeleteResponse
        {
            [JsonProperty(PropertyName = "count")]
            public int Count { get; set; }
            [JsonProperty(PropertyName = "continuationFlag")]
            public bool ContinuationFlag { get; set; }
        }

    }
}
