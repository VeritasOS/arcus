/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using BEArcus.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Diagnostics;


namespace BEArcus.WebApp.Controllers
{
    public class MediaServerController : Controller
    {
        /// <summary>
        /// Gets the Alerts and returns the Alerts view.
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public ActionResult Alert(string mediaServer)
        {
            Trace.WriteLine("Entering Alert method");
            try
            {
                ViewBag.DateSortParm = "date_desc";

                IEnumerable<Alert> alerts = DocumentDBDataController.GetAlerts(mediaServer);
                ViewBag.Mediaserver = mediaServer;
                ViewBag.Alerts = alerts;
                return View(alerts);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error:" + ex.Message);
                return View("Error");
            }
        }

        /// <summary>
        /// Gets the Alerts in ascending or decending order and returns Alert partial view.
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public ActionResult GetSortedDate(string sortOrder, string media)
        {
            Trace.WriteLine("Entering GetSortedDate method");
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            switch (sortOrder)
            {
                case "Date":
                    IEnumerable<Alert> alerts1 = DocumentDBDataController.GetAlerts(media);
                    ViewBag.Mediaserver = media;
                    ViewBag.Alerts = alerts1;
                    return PartialView("Alert", alerts1);

                case "date_desc":
                    List<Alert> alerts2 = DocumentDBDataController.GetAlertsInDescOrder(media);
                    ViewBag.Mediaserver = media;
                    ViewBag.Alerts = alerts2;
                    return PartialView("Alert", alerts2);
                default:
                    List<Alert> alerts = DocumentDBDataController.GetAlertsInDescOrder(media);
                    ViewBag.Mediaserver = media;
                    ViewBag.Alerts = alerts;
                    return PartialView("Alert", alerts);
            }

        }

        /// <summary>
        /// Gets the Job Histories in ascending or decending order ( of Start Date) and returns Job History partial view.
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public ActionResult GetSortedStartDate(string sortOrder, string media)
        {
            Trace.WriteLine("Entering GetSortedStartDate method");
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            switch (sortOrder)
            {

                case "Date":
                    var jobHistories1 = DocumentDBDataController.GetJobHistories(media);
                    ViewBag.Mediaserver = media;
                    return PartialView("JobHistory", jobHistories1);

                case "date_desc":
                    var jobHistories2 = DocumentDBDataController.GetStartDateInDescOrder(media);
                    ViewBag.Mediaserver = media;
                    return PartialView("JobHistory", jobHistories2);

                default:
                    var jobHistories = DocumentDBDataController.GetStartDateInDescOrder(media);
                    ViewBag.Mediaserver = media;
                    return PartialView("JobHistory", jobHistories);
            }
        }

        /// <summary>
        /// Gets the Job Histories in ascending or decending order (of End Date) and returns Job History partial view.
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public ActionResult GetSortedEndDate(string sortOrder, string media)
        {
            Trace.WriteLine("Entering GetSortedEndDate method");
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            switch (sortOrder)
            {
                case "Date":
                    var jobHistories1 = DocumentDBDataController.GetJobHistories(media);
                    ViewBag.Mediaserver = media;
                    return PartialView("JobHistory", jobHistories1);

                case "date_desc":
                    var jobHistories2 = DocumentDBDataController.GetEndDateInDescOrder(media);
                    ViewBag.Mediaserver = media;
                    return PartialView("JobHistory", jobHistories2);
                default:
                    var jobHistories = DocumentDBDataController.GetEndDateInDescOrder(media);
                    ViewBag.Mediaserver = media;
                    return PartialView("JobHistory", jobHistories);
            }
        }

        /// <summary>
        /// Gets the Alerts by name and returns the the Alert partial view.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="media"></param>
        /// <param name="alerts"></param>
        /// <returns></returns>
        public ActionResult GetAlertName(string Name, string media, IEnumerable<Alert> alerts)
        {
            Trace.WriteLine("Entering GetAlertName method");
            ModelState.Remove("Name");
            if (Name.Equals("All"))
            {
                IEnumerable<Alert> alerts2 = DocumentDBDataController.GetAlerts(media);
                ViewBag.Mediaserver = media;
                return PartialView("Alert", alerts2);
            }
            var alerts1 = DocumentDBDataController.GetAlertsByName(Name, media);
            ViewBag.Mediaserver = media;
            return PartialView("Alert", alerts1);
        }

        /// <summary>
        ///  Gets the Alerts by severity and returns the the Alert partial view.
        /// </summary>
        /// <param name="Severity"></param>
        /// <param name="media"></param>
        /// <param name="alerts"></param>
        /// <returns></returns>
        public ActionResult GetSeverity(string Severity, string media, IEnumerable<Alert> alerts)
        {
            Trace.WriteLine("Entering GetSeverity method");
            ModelState.Clear();
            if (Severity.Equals("All"))
            {
                IEnumerable<Alert> alerts2 = DocumentDBDataController.GetAlerts(media);
                ViewBag.Mediaserver = media;
                return PartialView("Alert", alerts2);
            }
            var alerts1 = DocumentDBDataController.GetAlertsBySeverity(Severity, media);
            ViewBag.Mediaserver = media;
            return PartialView("Alert", alerts1);
        }

        /// <summary>
        ///  Gets the Alerts by Category and returns the the Alert partial view.
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="media"></param>
        /// <param name="alerts"></param>
        /// <returns></returns>
        public ActionResult GetCategory(string Category, string media, IEnumerable<Alert> alerts)
        {
            Trace.WriteLine("Entering GetCategory method");
            ModelState.Clear();
            if (Category.Equals("All"))
            {
                IEnumerable<Alert> alerts2 = DocumentDBDataController.GetAlerts(media);
                ViewBag.Mediaserver = media;
                return PartialView("Alert", alerts2);
            }

            var alerts1 = DocumentDBDataController.GetAlertsByCategory(Category, media);
            ViewBag.Mediaserver = media;
            return PartialView("Alert", alerts1);
        }

        /// <summary>
        /// Gets Jobs for the given media server and returns Job View.
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public ActionResult Job(string mediaServer)
        {
            Trace.WriteLine("Entering Job method");
            ViewBag.DateSortParm = "date_desc";
            var jobs = DocumentDBDataController.GetJobs(mediaServer);
            ViewBag.Mediaserver = mediaServer;
            ViewBag.Jobs = jobs;
            return View(jobs);
        }

        /// <summary>
        /// Gets Jobs by name for given media server and returns Job partial view.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="media"></param>
        /// <param name="jobs"></param>
        /// <returns></returns>
        public ActionResult GetJobName(string Name, string media, IEnumerable<Job> jobs)
        {
            Trace.WriteLine("Entering GetJobName method");
            ModelState.Clear();
            if (Name.Equals("All"))
            {
                var jobs2 = DocumentDBDataController.GetJobs(media);
                ViewBag.Mediaserver = media;
                return PartialView("Job", jobs2);
            }
            var jobs1 = DocumentDBDataController.GetJobsByName(Name, media);
            ViewBag.Mediaserver = media;
            return PartialView("Job", jobs1);

        }

        /// <summary>
        ///  Gets Jobs by Task for given media server and returns Job partial view.
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="media"></param>
        /// <param name="jobs"></param>
        /// <returns></returns>
        public ActionResult GetTaskName(string TaskName, string media, IEnumerable<Job> jobs)
        {
            Trace.WriteLine("Entering GetTaskName method");
            ModelState.Clear();
            if (TaskName.Equals("All"))
            {
                var jobs2 = DocumentDBDataController.GetJobs(media);
                ViewBag.Mediaserver = media;
                return PartialView("Job", jobs2);
            }
            var jobs1 = DocumentDBDataController.GetJobsByTaskName(TaskName, media);
            ViewBag.Mediaserver = media;
            return PartialView("Job", jobs1);
        }

        /// <summary>
        ///  Gets Jobs by Job Type for given media server and returns Job partial view.
        /// </summary>
        /// <param name="JobType"></param>
        /// <param name="media"></param>
        /// <param name="jobs"></param>
        /// <returns></returns>
        public ActionResult GetJobType(string JobType, string media, IEnumerable<Job> jobs)
        {
            Trace.WriteLine("Entering GetJobType method");
            ModelState.Clear();
            if (JobType.Equals("All"))
            {
                var jobs2 = DocumentDBDataController.GetJobs(media);
                ViewBag.Mediaserver = media;
                return PartialView("Job", jobs2);
            }
            var jobs1 = DocumentDBDataController.GetJobsByJobType(JobType, media);
            ViewBag.Mediaserver = media;
            return PartialView("Job", jobs1);
        }

        /// <summary>
        ///  Gets Jobs by status for given media server and returns Job partial view.
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="media"></param>
        /// <param name="jobs"></param>
        /// <returns></returns>
        public ActionResult GetStatus(string Status, string media, IEnumerable<Job> jobs)
        {
            Trace.WriteLine("Entering GetStatus method");
            ModelState.Clear();
            if (Status.Equals("All"))
            {
                var jobs2 = DocumentDBDataController.GetJobs(media);
                ViewBag.Mediaserver = media;
                return PartialView("Job", jobs2);
            }

            var jobs1 = DocumentDBDataController.GetJobsByStatus(Status, media);
            ViewBag.Mediaserver = media;
            return PartialView("Job", jobs1);
        }

        /// <summary>
        /// Gets the Job Histories for the given media server and returns JobHistory View.
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public ActionResult JobHistory(string mediaServer)
        {
            Trace.WriteLine("Entering JobHistory method");
            ViewBag.DateSortParm = "date_desc";

            var jobHistories = DocumentDBDataController.GetJobHistories(mediaServer);
            ViewBag.Mediaserver = mediaServer;
            return View(jobHistories);
        }

        /// <summary>
        ///  Gets Job Histories by name for given media server and returns JobHistory partial view.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public ActionResult GetJobHistoryName(string Name, string media)
        {
            Trace.WriteLine("Entering GetJobHistoryName method");
            ModelState.Clear();
            if (Name.Equals("All"))
            {
                var jobHistories2 = DocumentDBDataController.GetJobHistories(media);
                ViewBag.Mediaserver = media;
                return PartialView("JobHistory", jobHistories2);
            }

            var jobsHistories1 = DocumentDBDataController.GetJobHistoryByName(Name, media);
            ViewBag.Mediaserver = media;
            return PartialView("JobHistory", jobsHistories1);
        }

        /// <summary>
        /// Gets Job Histories by Status for given media server and returns JobHistory partial view.
        /// </summary>
        /// <param name="JobStatus"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public ActionResult GetJobHistoryStatus(string JobStatus, string media)
        {
            Trace.WriteLine("Entering GetJobHistoryStatus method");
            ModelState.Clear();
            if (JobStatus.Equals("All"))
            {
                var jobHistories2 = DocumentDBDataController.GetJobHistories(media);
                ViewBag.Mediaserver = media;
                return PartialView("JobHistory", jobHistories2);
            }
            var jobsHistories1 = DocumentDBDataController.GetJobHistoryByJobStatus(JobStatus, media);
            ViewBag.Mediaserver = media;
            return PartialView("JobHistory", jobsHistories1);
        }

        /// <summary>
        /// Gets Job Histories by Type for given media server and returns JobHistory partial view.
        /// </summary>
        /// <param name="JobType"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public ActionResult GetJobHistoryType(string JobType, string media)
        {
            Trace.WriteLine("Entering GetJobHistoryType method");
            ModelState.Clear();
            if (JobType.Equals("All"))
            {
                var jobHistories2 = DocumentDBDataController.GetJobHistories(media);
                ViewBag.Mediaserver = media;
                return PartialView("JobHistory", jobHistories2);
            }
            var jobsHistories1 = DocumentDBDataController.GetJobHistoryByJobType(JobType, media);
            ViewBag.Mediaserver = media;
            return PartialView("JobHistory", jobsHistories1);
        }


        /// <summary>
        /// Gets the media server details and returns the FilterPanel view.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mediaServer"></param>
        /// <param name="befilter"></param>
        /// <returns></returns>
        public ActionResult FilterPanel(string id, string[] mediaServer, string[] befilter)
        {
            Trace.WriteLine("Entering FilterPanel method");
            ViewBag.Customer = id;
            ViewBag.MediaServer = mediaServer;
            ViewBag.BEFilter = befilter;

            var mediaServer1 = DocumentDBDataController.GetMediaServers();
            return View(mediaServer1);
        }

        /// <summary>
        /// Gets the Media server, Critical alerts and Job status information and returns HomePage View
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePage()
        {
            Trace.WriteLine("Entering HomePage method");
            Trace.WriteLine("Entering HomePage method");
            try
            {
                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Fetching Media servers, critical alerts, failed Jobs, Missed Jobs, Succeeded jobs and Job completed with exception");
                var mediaServer = DocumentDBDataController.GetMediaServers();

                ViewBag.Alerts = DocumentDBDataController.GetCrititcalAlerts();
                ViewBag.FailedJobs = DocumentDBDataController.GetFailedJobHistories();
                ViewBag.MissedJobs = DocumentDBDataController.GetMissedJobHistories();
                ViewBag.SucceededJobs = DocumentDBDataController.GetSucceededJobHistories();
                ViewBag.SucceededWithExceptions = DocumentDBDataController.GetExceptionJobHistories();
                return View(mediaServer);
            }
            catch (Exception ex)
            {
                Exception baseException = ex.GetBaseException();
                Trace.TraceError("Error:" + ex.Message + "Message:" + baseException.Message);
                return View("Error");
            }
        }

        /// <summary>
        /// Gets the Media server information and returns the Group view
        /// </summary>
        /// <returns></returns>
        public ActionResult Group()
        {
            Trace.WriteLine("Entering Group method");
            try
            {
                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Fetch the Media Server details");
                var mediaServer = DocumentDBDataController.GetMediaServers();
                return View(mediaServer);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error:" + ex.Message);
                return View("Error");
            }

        }

        /// <summary>
        /// Returns the Create Group Partial view
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateGroup()
        {
            Trace.WriteLine("Entering CreateGroup method");
            try
            {
                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Returning Partial View CreateGroup");
                return PartialView("CreateGroup");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error:" + ex.Message);
                return View("Error");
            }
        }

        /// <summary>
        /// Save Group data and return Group View
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task<ActionResult> SaveGroupData(string[] mediaServer, string customer)
        {
            Trace.WriteLine("Entering GetGroupData method");
            IEnumerable<string> mediServer = mediaServer.AsEnumerable();

            await DocumentDBDataController.SaveGroupData("", "", mediaServer, customer);
            return RedirectToAction("Group", "MediaServer");
        }

        /// <summary>
        /// Returns the filtered data to Home Page view
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <param name="alerts"></param>
        /// <param name="failedJobs"></param>
        /// <param name="missedJobs"></param>
        /// <param name="succeededJobs"></param>
        /// <param name="succeededWithExceptionsJobs"></param>
        /// <param name="TimeFilter"></param>
        /// <returns></returns>
        public ActionResult FilteredImportantEvents(IEnumerable<MediaServer> mediaServer, List<Alert> alerts, List<JobHistory> failedJobs, List<JobHistory> missedJobs, List<JobHistory> succeededJobs, List<JobHistory> succeededWithExceptionsJobs, string TimeFilter)
        {
            Trace.WriteLine("Entering FileterImportatntEvents method");
            var media = DocumentDBDataController.GetMediaServers();
            List<Alert> alert = DocumentDBDataController.GetCrititcalAlerts();
            List<JobHistory> failedJob = DocumentDBDataController.GetFailedJobHistories();
            List<JobHistory> missedJob = DocumentDBDataController.GetMissedJobHistories();
            List<JobHistory> succeededJob = DocumentDBDataController.GetSucceededJobHistories();
            List<JobHistory> succeededWithExceptionsJob = DocumentDBDataController.GetExceptionJobHistories();
            if (TimeFilter.Equals("All"))
            {
                ViewBag.Alerts = alert;
                ViewBag.FailedJobs = failedJob;
                ViewBag.MissedJobs = missedJob;
                ViewBag.SucceededJobs = succeededJob;
                ViewBag.SucceededWithExceptions = succeededWithExceptionsJob;
                return PartialView("HomePage", media);
            }
            int hrs = Convert.ToInt32(TimeFilter);
            DateTime now = DateTime.Now;
            ViewBag.Alerts = alert.Where(o => o.Date > now.AddHours(hrs) && o.Date <= now).ToList();
            ViewBag.FailedJobs = failedJob.Where(o => o.StartTime > now.AddHours(hrs) && o.StartTime <= now).ToList();
            ViewBag.MissedJobs = missedJob.Where(o => o.StartTime > now.AddHours(hrs) && o.StartTime <= now).ToList();
            ViewBag.SucceededJobs = succeededJob.Where(o => o.StartTime > now.AddHours(hrs) && o.StartTime <= now).ToList();
            ViewBag.SucceededWithExceptions = succeededWithExceptionsJob.Where(o => o.StartTime > now.AddHours(hrs) && o.StartTime <= now).ToList();
            return PartialView("HomePage", media);
        }

        /// <summary>
        /// Gets the critical alerts for given media server and returns Alert view
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public ActionResult CriticalAlert(string mediaServer)
        {
            Trace.WriteLine("Entering CriticalAlert method");
            ViewBag.DateSortParm = "date_desc";
            IEnumerable<Alert> alerts = DocumentDBDataController.GetAlertsBySeverity("Error", mediaServer);
            ViewBag.Mediaserver = mediaServer;
            ViewBag.Alerts = alerts;
            return View("Alert", alerts);
        }

        /// <summary>
        /// Gets the failed jobs for given media server and returns JobHistory View
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public ActionResult FailedJob(string mediaServer)
        {
            Trace.WriteLine("Entering FailedJob method");
            ViewBag.DateSortParm = "date_desc";

            var jobHistories = DocumentDBDataController.GetJobHistoryByJobStatus("Error", mediaServer);
            ViewBag.Mediaserver = mediaServer;
            return View("JobHistory", jobHistories);
        }

        /// <summary>
        /// Gets the Missed jobs for given media server and returns JobHistory View
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public ActionResult MissedJob(string mediaServer)
        {
            Trace.WriteLine("Entering MissedJob method");
            ViewBag.DateSortParm = "date_desc";

            var jobHistories = DocumentDBDataController.GetJobHistoryByJobStatus("Missed", mediaServer);
            ViewBag.Mediaserver = mediaServer;
            return View("JobHistory", jobHistories);
        }

        /// <summary>
        /// Gets the successful jobs for given media server and returns JobHistory View
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public ActionResult SuccessfulJob(string mediaServer)
        {
            Trace.WriteLine("Entering SuccessfulJob method");
            ViewBag.DateSortParm = "date_desc";

            var jobHistories = DocumentDBDataController.GetJobHistoryByJobStatus("Succeeded", mediaServer);
            ViewBag.Mediaserver = mediaServer;
            return View("JobHistory", jobHistories);
        }

        /// <summary>
        /// Gets the Jobs completed with exception for given media server and returns JobHistory View
        /// </summary>
        /// <param name="mediaServer"></param>
        /// <returns></returns>
        public ActionResult CompletedWithExceptionJob(string mediaServer)
        {
            Trace.WriteLine("Entering CompleteWithExceptionJob method");
            ViewBag.DateSortParm = "date_desc";

            var jobHistories = DocumentDBDataController.GetJobHistoryByJobStatus("SucceededWithExceptions", mediaServer);
            ViewBag.Mediaserver = mediaServer;
            return View("JobHistory", jobHistories);
        }

        /// <summary>
        /// Creating pie chart
        /// </summary>
        /// <param name="failedcnt"></param>
        /// <param name="missedcnt"></param>
        /// <param name="succeededcnt"></param>
        /// <param name="exceptioncnt"></param>
        /// <returns></returns>
        public ActionResult CreatePie(int failedcnt, int missedcnt, int succeededcnt, int exceptioncnt)
        {
            Trace.WriteLine("Entering CreatePie method");
            //Create pie chart
            var chart = new Chart(width: 300, height: 200, theme: ChartTheme.Vanilla3D)
            .AddSeries(chartType: "pie",
                            xValue: new[] { "", "", "", "" },
                            yValues: new[] { succeededcnt.ToString(), failedcnt.ToString(), missedcnt.ToString(), exceptioncnt.ToString() })
                            .GetBytes("png");

            return File(chart, "image/bytes");
        }

    }
}