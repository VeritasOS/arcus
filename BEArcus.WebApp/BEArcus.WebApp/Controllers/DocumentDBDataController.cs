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
using BEArcus.WebApp.Models;
using System.Web.Configuration;
using System.Diagnostics;

namespace BEArcus.WebApp.Controllers
{
    /// <summary>
    /// Contains methods:
    /// To save Alert, Job and JobHistory Objects to DocumentDB database.
    /// </summary>
    public class DocumentDBDataController
    {

        private static readonly string beDatabaseId = WebConfigurationManager.AppSettings["beDatabaseId"];
        private static readonly string alertCollectionId = WebConfigurationManager.AppSettings["alertCollectionId"];
        private static readonly string jobCollectionId = WebConfigurationManager.AppSettings["jobCollectionId"];
        private static readonly string jobHistoryCollectionId = WebConfigurationManager.AppSettings["jobHistoryCollectionId"];
        private static readonly string mediaServerCollectionId = WebConfigurationManager.AppSettings["mediaServerCollectionId"];

        private static DocumentClient client;
        private static string EndpointUrl = WebConfigurationManager.AppSettings["EndpointUrl"];
        private static string AuthorizationKey = WebConfigurationManager.AppSettings["AuthorizationKey"];

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
                Trace.WriteLine("Entering GetOrCreateDatabase");

                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Checking if the Database" + databaseId + "Exists");
                // Check to verify a database with the id does not exist
                Database database = client.CreateDatabaseQuery().
                                    Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();

                // If the database does not exist, create a new database
                if (database == null)
                {
                    Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Creating Database" + databaseId);
                    database = await client.CreateDatabaseAsync(
                        new Database
                        {
                            Id = databaseId
                        });
                }

                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + databaseId + "is created or present");
                return database;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e.Message);
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
                Trace.WriteLine("Entering GetOrCreateCollection");
                // Check to verify a document collection with the id does not exist
                DocumentCollection documentCollection = client.CreateDocumentCollectionQuery("dbs/" + databaseId).
                                                        Where(c => c.Id == collectionId).AsEnumerable().FirstOrDefault();

                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Checking if the Collection" + collectionId + "Exists");
                // If the document collection does not exist, create a new collection
                if (documentCollection == null)
                {
                    Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Creating Collection" + databaseId);
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
                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + collectionId + "is created or present");
                return documentCollection;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e.Message);
                return null;
            }
        }


        /// <summary>
        /// Gets the Media Server data form the given Endpoint Url and Authorization Key.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static IEnumerable<MediaServer> GetMediaServers()
        {
            Trace.WriteLine("Entering GetMediaServer");

            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            Init(beDatabaseId, mediaServerCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, mediaServerCollectionId);

            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Querying for Media Servers");
            return client.CreateDocumentQuery<MediaServer>(collectionLink, "SELECT * FROM MediaServerCollection").AsEnumerable();
        }


        /// <summary>
        /// Gets the Alerts data for the provided Media Sever form the given Endpoint Url and Authorization Key. 
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Alert> GetAlerts(string mediaServer)
        {
            Trace.WriteLine("Entering GetAlerts method");

            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);

            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Querying for Alerts for " + mediaServer);
            return client.CreateDocumentQuery<Alert>(
                collectionLink)
                .Where(a => a.MediaServerName == mediaServer)
                .AsEnumerable();
        }

        /// <summary>
        /// Gets the Jobs data for the provided Media Sever form the given Endpoint Url and Authorization Key. 
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Job> GetJobs(string mediaServer)
        {
            Trace.WriteLine("Entering GetJobs method");

            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobCollectionId);

            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Querying for Jobs for" + mediaServer);
            return client.CreateDocumentQuery<Job>(
               collectionLink)
               .Where(a => a.MediaServerName == mediaServer)
               .AsEnumerable();
        }

        /// <summary>
        /// Gets the Job Histories data for the provided Media Sever form the given Endpoint Url and Authorization Key. 
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<JobHistory> GetJobHistories(string mediaServer)
        {
            Trace.WriteLine("Entering GetJobHistories method");

            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Querying for Job Histories for" + mediaServer);
            return client.CreateDocumentQuery<JobHistory>(
               collectionLink)
               .Where(a => a.MediaServerName == mediaServer)
               .AsEnumerable();
        }


        /// <summary>
        /// Gets All the Alerts data from the given Endpoint Url and Authorization Key.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static IEnumerable<Alert> GetAllAlerts(string endpointUrl, string authorizationKey)
        {
            //client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);

            return client.CreateDocumentQuery<Alert>(
                collectionLink)
                .AsEnumerable();
        }

        /// <summary>
        ///  Gets All the Jobs data from the given Endpoint Url and Authorization Key.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static IEnumerable<Job> GetAllJobs(string endpointUrl, string authorizationKey)
        {
            //client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobCollectionId);

            return client.CreateDocumentQuery<Job>(
               collectionLink)
               .AsEnumerable();
        }

        /// <summary>
        ///  Gets All the Job Histories data from the given Endpoint Url and Authorization Key.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static IEnumerable<JobHistory> GetAllJobHistories(string endpointUrl, string authorizationKey)
        {
            //client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
               collectionLink)
               .AsEnumerable();
        }

        /// <summary>
        /// Gets List of Alerts for a media server by severity 
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="severity"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Alert> GetAlertsBySeverity(string severity, string mediaServer)
        {
            Trace.WriteLine("Entering GetAlertsBySeverity method");

            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);

            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Querying for Alerts with severity" + severity + "for media server" + mediaServer);
            return client.CreateDocumentQuery<Alert>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM AlertCollection a WHERE a.Severity = @Severity AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@Severity", severity),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets list of Alerts for a media server by Category
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="category"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Alert> GetAlertsByCategory(string category, string mediaServer)
        {
            Trace.WriteLine("Entering GetAlertsByCategoy method");

            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);

            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Querying for Alerts with category" + category + "for media server" + mediaServer);
            return client.CreateDocumentQuery<Alert>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM AlertCollection a WHERE a.Category = @Category AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@Category", category),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets list of alerts for a media server by alert name
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="name"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Alert> GetAlertsByName(string name, string mediaServer)
        {
            Trace.WriteLine("Entering GetAlertsByName");

            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);

            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Querying for Alerts with name" + name + "for media server" + mediaServer);
            return client.CreateDocumentQuery<Alert>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM AlertCollection a WHERE a.Name = @Name AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@Name", name),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets list of alerts for a media server by BE id
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="be_id"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Alert> GetAlertsByBE_id(string endpointUrl, string authorizationKey, string be_id, string mediaServer)
        {
            Trace.WriteLine("Entering GetAlertsByBE_id");
            //client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);

            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Querying for Alerts with BE_id" + be_id + "for media server" + mediaServer);
            return client.CreateDocumentQuery<Alert>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM AlertCollection a WHERE a.be_id = @BE_id AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@BE_id", be_id),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets the Alerts for the given media server by date specified.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="date"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Alert> GetAlertsByDate(string date, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);

            return client.CreateDocumentQuery<Alert>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM AlertCollection a WHERE a.Date = @Date AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@Date",date),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets list of alerts for a media server by its BackupExecServerName
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="backupExec"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Alert> GetAlertsByBackupExec(string endpointUrl, string authorizationKey, string backupExec, string mediaServer)
        {
            //client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);

            return client.CreateDocumentQuery<Alert>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM AlertCollection a WHERE a.BackupExecServerName = @Name AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@Name", backupExec),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets Alerts in Decending order
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static List<Alert> GetAlertsInDescOrder(string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);
            List<Alert> alerts = client.CreateDocumentQuery<Alert>(
                collectionLink)
                .Where(a => a.MediaServerName == mediaServer)
                .ToList();
            alerts.Sort((a, b) => b.Date.CompareTo(a.Date));
            return alerts;

        }

        /// <summary>
        /// Gets JobHistories in Decending order (Based on Start date)
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static List<JobHistory> GetStartDateInDescOrder(string mediaServer)
        {

            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);


            List<JobHistory> jobs = client.CreateDocumentQuery<JobHistory>(
               collectionLink)
               .Where(a => a.MediaServerName == mediaServer)
               .ToList();
            jobs.Sort((a, b) => b.StartTime.CompareTo(a.StartTime));
            return jobs;
        }


        /// <summary>
        /// Gets Job Histories in Descending order (Based on End Date)
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static List<JobHistory> GetEndDateInDescOrder(string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            List<JobHistory> jobs = client.CreateDocumentQuery<JobHistory>(
               collectionLink)
               .Where(a => a.MediaServerName == mediaServer)
               .ToList();
            jobs.Sort((a, b) => b.EndTime.CompareTo(a.EndTime));
            return jobs;
        }

        /// <summary>
        /// Gets the Jobs for the given media server by its name
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="name"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Job> GetJobsByName(string name, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobCollectionId);

            return client.CreateDocumentQuery<Job>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM JobCollection a WHERE a.Name = @Name AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@Name",name),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets Jobs for the given media server by its job type
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="jobType"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Job> GetJobsByJobType(string jobType, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobCollectionId);
            return client.CreateDocumentQuery<Job>(
                   collectionLink,
                   new SqlQuerySpec()
                   {
                       QueryText = "SELECT * FROM JobCollection a WHERE a.JobType = @JobType AND a.MediaServerName = @MediaServer",
                       Parameters = new SqlParameterCollection()
                       {
                        new SqlParameter("@JobType", jobType),
                        new SqlParameter("@MediaServer", mediaServer)
                       }
                   });

        }

        /// <summary>
        /// Gets Jobs for the given media server by its BE id.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="be_id"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Job> GetJobsByBE_id(string endpointUrl, string authorizationKey, string be_id, string mediaServer)
        {
            //client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobCollectionId);
            return client.CreateDocumentQuery<Job>(
                   collectionLink,
                   new SqlQuerySpec()
                   {
                       QueryText = "SELECT * FROM JobCollection a WHERE a.be_id = @BE_id AND a.MediaServerName = @MediaServer",
                       Parameters = new SqlParameterCollection()
                       {
                        new SqlParameter("@BE_id", be_id),
                        new SqlParameter("@MediaServer", mediaServer)
                       }
                   });

        }

        /// <summary>
        /// Gets Jobs for the given media server by its Task name.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="TaskName"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Job> GetJobsByTaskName(string TaskName, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobCollectionId);
            return client.CreateDocumentQuery<Job>(
                   collectionLink,
                   new SqlQuerySpec()
                   {
                       QueryText = "SELECT * FROM JobCollection a WHERE a.TaskName = @TaskName AND a.MediaServerName = @MediaServer",
                       Parameters = new SqlParameterCollection()
                       {
                        new SqlParameter("@TaskName", TaskName),
                        new SqlParameter("@MediaServer", mediaServer)
                       }
                   });
        }

        /// <summary>
        /// Gets the Jobs for the given media server by its Status.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="Status"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<Job> GetJobsByStatus(string Status, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobCollectionId);
            return client.CreateDocumentQuery<Job>(
                   collectionLink,
                   new SqlQuerySpec()
                   {
                       QueryText = "SELECT * FROM JobCollection a WHERE a.Status = @Status AND a.MediaServerName = @MediaServer",
                       Parameters = new SqlParameterCollection()
                       {
                        new SqlParameter("@Status",Status),
                        new SqlParameter("@MediaServer", mediaServer)
                       }
                   });
        }

        /// <summary>
        /// Gets the Jobs Histories for the given media server by its Name.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="name"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<JobHistory> GetJobHistoryByName(string name, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM JobHistoryCollection a WHERE a.Name = @Name AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@Name",name),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets the Jobs Histories for the given media server by its BE id.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="be_id"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<JobHistory> GetJobHistoryByBE_id(string endpointUrl, string authorizationKey, string be_id, string mediaServer)
        {
            //client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM JobHistoryCollection a WHERE a.be_id = @BE_id AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@BE_id",be_id),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets the Jobs Histories for the given media server by its Job Status.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="jobStatus"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<JobHistory> GetJobHistoryByJobStatus(string jobStatus, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM JobHistoryCollection a WHERE a.JobStatus = @JobStatus AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@JobStatus",jobStatus),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets the Jobs Histories for the given media server by its Job Type.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="jobType"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<JobHistory> GetJobHistoryByJobType(string jobType, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM JobHistoryCollection a WHERE a.JobType = @JobType AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@JobType",jobType),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets the Jobs Histories for the given media server by its Start Time.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="startTime"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<JobHistory> GetJobHistoryByStartTime(string startTime, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);
            DateTime dt = Convert.ToDateTime(startTime);
            return client.CreateDocumentQuery<JobHistory>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM JobHistoryCollection a WHERE a.StartTime = @StartTime AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@StartTime",startTime),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets the Jobs Histories for the given media server by its End Time.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="endTime"></param>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public static IEnumerable<JobHistory> GetJobHistoryByEndTime(string endTime, string mediaServer)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobHistoryCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
                     collectionLink,
                     new SqlQuerySpec()
                     {
                         QueryText = "SELECT * FROM JobHistoryCollection a WHERE a.EndTime = @EndTime AND a.MediaServerName = @MediaServer",
                         Parameters = new SqlParameterCollection()
                         {
                        new SqlParameter("@EndTime",endTime),
                        new SqlParameter("@MediaServer", mediaServer)
                         }
                     });
        }

        /// <summary>
        /// Gets the Media Server details for the given customer.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static IEnumerable<MediaServer> GetMediaServerByCustomer(string endpointUrl, string authorizationKey, string customer)
        {
            //client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, mediaServerCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, mediaServerCollectionId);
            return client.CreateDocumentQuery<MediaServer>(collectionLink).
                                 Where(d => d.CustomerName == customer).AsEnumerable();
        }

        /// <summary>
        /// Gets the critical alerts.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static List<Alert> GetCrititcalAlerts()
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, alertCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, alertCollectionId);

            return client.CreateDocumentQuery<Alert>(
                collectionLink).Where(o => o.Severity.Equals("Error")).ToList();

        }

        /// <summary>
        /// Gets the Failed job histories.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static List<JobHistory> GetFailedJobHistories()
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
               collectionLink).Where(o => o.JobStatus.Equals("Error"))
               .ToList();
        }

        /// <summary>
        /// Gets the Missed Job Histories.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static List<JobHistory> GetMissedJobHistories()
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
               collectionLink).Where(o => o.JobStatus.Equals("Missed"))
               .ToList();
        }

        /// <summary>
        /// Gets the Succeeded Job Histories.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static List<JobHistory> GetSucceededJobHistories()
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
               collectionLink).Where(o => o.JobStatus.Equals("Succeeded"))
               .ToList();
        }

        /// <summary>
        /// Gets the Job Histories completed with exception.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static List<JobHistory> GetExceptionJobHistories()
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Init(beDatabaseId, jobCollectionId);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, jobHistoryCollectionId);

            return client.CreateDocumentQuery<JobHistory>(
               collectionLink).Where(o => o.JobStatus.Equals("SucceededWithExceptions"))
               .ToList();
        }

        /// <summary>
        /// Saves the media server to the provided group name (customer). 
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <param name="mediaServer"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static async Task<bool> SaveGroupData(string endpointUrl, string authorizationKey, IEnumerable<string> mediaServer, string customer)
        {
            using (client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
            {
                //To get or create the documentDB database and collection.
                Init(beDatabaseId, mediaServerCollectionId);
                var collectionLink = UriFactory.CreateDocumentCollectionUri(beDatabaseId, mediaServerCollectionId);

                var users = client.CreateDocumentQuery(
                collectionLink,
                new SqlQuerySpec()
                {
                    QueryText = "SELECT * FROM MediaServerCollection c WHERE ARRAY_CONTAINS(@mediaServer,c.Name)",
                    Parameters = new SqlParameterCollection()
                    {
                           new SqlParameter("@mediaServer", mediaServer)
                    }
                });

                foreach (Document user in users)
                {
                    user.SetPropertyValue("CustomerName", customer);
                    await client.ReplaceDocumentAsync(user);
                }
                return true;
            }
        }

        /// <summary>
        /// Checks if the database is accessible.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        /// <returns></returns>
        public static async Task<bool> IsDatabaseAccessible(string endpointUrl, string authorizationKey)
        {
            client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            Database database = await GetOrCreateDatabase(beDatabaseId);
            if (database == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the GetOrCreateDatabase() method and GetOrCreateCollection().
        /// </summary>
        /// <param name="databaseId">Database Id.</param>
        /// <param name="collectionId">Collection Id.</param>
        public static void Init(string databaseId, string collectionId)
        {
            try
            {
                GetOrCreateDatabase(databaseId).Wait();
                GetOrCreateCollection(databaseId, collectionId).Wait();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error: " + e.Message);
            }
        }
    }
}
