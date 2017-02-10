/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BEArcus.Agent
{
    public class UserSettingsHelper
    {
        public static XmlNode node;
        public static XmlAttribute user;
        public static XmlAttribute start;
        public static XmlAttribute end;
        public static XmlAttribute lastUpdate;
        public static XmlNode setting;
        string path = Directory.GetCurrentDirectory();

        public static void CreateFileSystemSettings(string fileSystemName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']", fileSystemName);
            XmlNode fileSystem = xmlDoc.SelectSingleNode(query);

            if (fileSystem == null)
            {
                XmlNode FileSystemNode = xmlDoc.CreateElement("FileSystem");

                XmlAttribute fileSystemId = xmlDoc.CreateAttribute("Name");
                FileSystemNode.Attributes.Append(fileSystemId);
                fileSystemId.Value = fileSystemName;

                XmlNode node = xmlDoc.CreateElement("Alert");
                XmlAttribute user = xmlDoc.CreateAttribute("User");
                XmlAttribute start = xmlDoc.CreateAttribute("FetchStartTime");
                XmlAttribute end = xmlDoc.CreateAttribute("FetchEndTime");
                XmlAttribute lastUpdate = xmlDoc.CreateAttribute("LastUpdatedTime");
                node.Attributes.Append(user);
                node.Attributes.Append(start);
                node.Attributes.Append(end);
                node.Attributes.Append(lastUpdate);
                user.Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                start.Value = DateTime.Now.ToString();
                end.Value = "2016-02-25";
                FileSystemNode.AppendChild(node);

                node = xmlDoc.CreateElement("Job");
                user = xmlDoc.CreateAttribute("User");
                start = xmlDoc.CreateAttribute("FetchStartTime");
                end = xmlDoc.CreateAttribute("FetchEndTime");
                lastUpdate = xmlDoc.CreateAttribute("LastUpdatedTime");
                node.Attributes.Append(user);
                node.Attributes.Append(start);
                node.Attributes.Append(end);
                node.Attributes.Append(lastUpdate);
                user.Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                start.Value = DateTime.Now.ToString();
                end.Value = "2016-02-25";
                FileSystemNode.AppendChild(node);

                node = xmlDoc.CreateElement("JobHistory");
                user = xmlDoc.CreateAttribute("User");
                start = xmlDoc.CreateAttribute("FetchStartTime");
                end = xmlDoc.CreateAttribute("FetchEndTime");
                lastUpdate = xmlDoc.CreateAttribute("LastUpdatedTime");
                node.Attributes.Append(user);
                node.Attributes.Append(start);
                node.Attributes.Append(end);
                node.Attributes.Append(lastUpdate);
                user.Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                start.Value = DateTime.Now.ToString();
                end.Value = "2016-02-25";
                FileSystemNode.AppendChild(node);

                string query2 = string.Format("//TimeSetting");
                setting = xmlDoc.SelectSingleNode(query2);
                setting.AppendChild(FileSystemNode);

                xmlDoc.Save(@".\UserSettings.xml");
            }

            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static void FileSetAlertLastUpdateTime(string fileSystemName, DateTime lastUpdateTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']//Alert", fileSystemName);
            XmlNode alert = xmlDoc.SelectSingleNode(query);
            alert.Attributes["LastUpdatedTime"].Value = lastUpdateTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static string FileGetAlertFetchEndTime(string fileSystemName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']//Alert", fileSystemName);
            XmlNode alert = xmlDoc.SelectSingleNode(query);
            return alert.Attributes["FetchEndTime"].Value;
        }

        public static void FileSetAlertFetchEndTime(string fileSystemName, DateTime lastUpdateTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']//Alert", fileSystemName);
            XmlNode alert = xmlDoc.SelectSingleNode(query);
            alert.Attributes["FetchEndTime"].Value = lastUpdateTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }
        public static void FileSetJobHistoryLastUpdateTime(string fileSystemName, DateTime lastUpdateTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']//JobHistory", fileSystemName);
            XmlNode jobHistory = xmlDoc.SelectSingleNode(query);
            jobHistory.Attributes["LastUpdatedTime"].Value = lastUpdateTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static void FileSetJobLastUpdateTime(string fileSystemName, DateTime lastUpdateTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']//Job", fileSystemName);
            XmlNode job = xmlDoc.SelectSingleNode(query);
            job.Attributes["LastUpdatedTime"].Value = lastUpdateTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static void FileSetAlertFetchStartTime(string fileSystemName, DateTime startTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']//Alert", fileSystemName);
            XmlNode alert = xmlDoc.SelectSingleNode(query);
            alert.Attributes["FetchStartTime"].Value = startTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }
        public static void FileSetJobHistoryFetchStartTime(string fileSystemName, DateTime startTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']//JobHistory", fileSystemName);
            XmlNode jobHistory = xmlDoc.SelectSingleNode(query);
            jobHistory.Attributes["FetchStartTime"].Value = startTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static void DocumetDBSetAlertFetchStartTime(string documentDBName, DateTime startTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']//Alert", documentDBName);
            XmlNode alert = xmlDoc.SelectSingleNode(query);
            alert.Attributes["FetchStartTime"].Value = startTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static void DocumentDBSetJobHistoryFetchStartTime(string documentDBName, DateTime startTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']//JobHistory", documentDBName);
            XmlNode jobHistory = xmlDoc.SelectSingleNode(query);
            jobHistory.Attributes["FetchStartTime"].Value = startTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static string FileGetJobHistoryFetchEndTime(string fileSystemName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']//JobHistory", fileSystemName);
            XmlNode jobHistory = xmlDoc.SelectSingleNode(query);
            return jobHistory.Attributes["FetchEndTime"].Value;
        }

        public static void FileSetJobHistoryFetchEndTime(string fileSystemName, DateTime lastUpdateTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//FileSystem[@Name='{0}']//JobHistory", fileSystemName);
            XmlNode jobHistory = xmlDoc.SelectSingleNode(query);
            jobHistory.Attributes["FetchEndTime"].Value = lastUpdateTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static void CreateDocumentDBSettings(string documentDBName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']", documentDBName);
            XmlNode documentDB = xmlDoc.SelectSingleNode(query);

            if (documentDB == null)
            {
                XmlNode documentDBNode = xmlDoc.CreateElement("DocumentDB");

                XmlAttribute documentDBId = xmlDoc.CreateAttribute("Name");
                documentDBNode.Attributes.Append(documentDBId);
                documentDBId.Value = documentDBName;

                node = xmlDoc.CreateElement("Alert");
                user = xmlDoc.CreateAttribute("User");
                start = xmlDoc.CreateAttribute("FetchStartTime");
                end = xmlDoc.CreateAttribute("FetchEndTime");
                lastUpdate = xmlDoc.CreateAttribute("LastUpdatedTime");
                node.Attributes.Append(user);
                node.Attributes.Append(start);
                node.Attributes.Append(end);
                node.Attributes.Append(lastUpdate);
                user.Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                start.Value = DateTime.Now.ToString();
                end.Value = "2016-02-25";
                documentDBNode.AppendChild(node);

                node = xmlDoc.CreateElement("Job");
                user = xmlDoc.CreateAttribute("User");
                start = xmlDoc.CreateAttribute("FetchStartTime");
                end = xmlDoc.CreateAttribute("FetchEndTime");
                lastUpdate = xmlDoc.CreateAttribute("LastUpdatedTime");
                node.Attributes.Append(user);
                node.Attributes.Append(start);
                node.Attributes.Append(end);
                node.Attributes.Append(lastUpdate);
                user.Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                start.Value = DateTime.Now.ToString();
                end.Value = "2016-02-25";
                documentDBNode.AppendChild(node);

                node = xmlDoc.CreateElement("JobHistory");
                user = xmlDoc.CreateAttribute("User");
                start = xmlDoc.CreateAttribute("FetchStartTime");
                end = xmlDoc.CreateAttribute("FetchEndTime");
                lastUpdate = xmlDoc.CreateAttribute("LastUpdatedTime");
                node.Attributes.Append(user);
                node.Attributes.Append(start);
                node.Attributes.Append(end);
                node.Attributes.Append(lastUpdate);
                user.Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                start.Value = DateTime.Now.ToString();
                end.Value = "2016-02-25";
                documentDBNode.AppendChild(node);

                string query2 = string.Format("//TimeSetting");
                setting = xmlDoc.SelectSingleNode(query2);
                setting.AppendChild(documentDBNode);
                xmlDoc.Save(@".\UserSettings.xml");
            }

            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static string GetLastUpdatedTime(string documentDBName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']//Alert", documentDBName);
            XmlNode alert = xmlDoc.SelectSingleNode(query);
            return alert.Attributes["LastUpdatedTime"].Value;
        }

        public static void SetLastUpdateTime(string documentDBName, DateTime lastUpdateTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']//Alert", documentDBName);
            XmlNode alert = xmlDoc.SelectSingleNode(query);
            alert.Attributes["LastUpdatedTime"].Value = lastUpdateTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static string GetFetchEndTime(string documentDBName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']//Alert", documentDBName);
            XmlNode alert = xmlDoc.SelectSingleNode(query);
            return alert.Attributes["FetchEndTime"].Value;
        }

        public static void SetFetchEndTime(string documentDBName, DateTime lastUpdateTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']//Alert", documentDBName);
            XmlNode alert = xmlDoc.SelectSingleNode(query);
            alert.Attributes["FetchEndTime"].Value = lastUpdateTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }
        public static void SetJobHistoryLastUpdateTime(string documentDBName, DateTime lastUpdateTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']//JobHistory", documentDBName);
            XmlNode jobHistory = xmlDoc.SelectSingleNode(query);
            jobHistory.Attributes["LastUpdatedTime"].Value = lastUpdateTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static string GetJobHistoryFetchEndTime(string documentDBName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']//JobHistory", documentDBName);
            XmlNode jobHistory = xmlDoc.SelectSingleNode(query);
            return jobHistory.Attributes["FetchEndTime"].Value;
        }

        public static void SetJobHistoryFetchEndTime(string documentDBName, DateTime lastUpdateTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']//JobHistory", documentDBName);
            XmlNode jobHistory = (XmlElement)xmlDoc.SelectSingleNode(query);
            jobHistory.Attributes["FetchEndTime"].Value = lastUpdateTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }
        public static void CreateSettingsFile()
        {
            XmlDocument xmlDoc = new XmlDocument();          
            XmlNode root = xmlDoc.CreateElement("TimeSetting");
            XmlAttribute globalRunTime = xmlDoc.CreateAttribute("GlobalRunTime");
            root.Attributes.Append(globalRunTime);          
            xmlDoc.AppendChild(root);
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static void SetRunTime(DateTime runTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//TimeSetting");
            XmlNode timeSetting = xmlDoc.SelectSingleNode(query);
            timeSetting.Attributes["GlobalRunTime"].Value = runTime.ToString();
            xmlDoc.Save(@".\UserSettings.xml");
        }

        public static void CreateAlertNode(string documentDBName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']", documentDBName);
            XmlNode documentDBNode = xmlDoc.SelectSingleNode(query);

            node = xmlDoc.CreateElement("Alert");
            user = xmlDoc.CreateAttribute("User");
            start = xmlDoc.CreateAttribute("FetchStartTime");
            end = xmlDoc.CreateAttribute("FetchEndTime");
            lastUpdate = xmlDoc.CreateAttribute("LastUpdatedTime");
            node.Attributes.Append(user);
            node.Attributes.Append(start);
            node.Attributes.Append(end);
            node.Attributes.Append(lastUpdate);
            user.Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            start.Value = DateTime.Now.ToString();
            end.Value = "2016-02-25";
            documentDBNode.AppendChild(node);
        }
        public static void CreateJobNode(string documentDBName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            string query = string.Format("//DocumentDB[@Name='{0}']", documentDBName);
            XmlNode documentDBNode = xmlDoc.SelectSingleNode(query);

            XmlNode jobNode = xmlDoc.CreateElement("Job");

            XmlAttribute user = xmlDoc.CreateAttribute("User");
            XmlAttribute start = xmlDoc.CreateAttribute("StartTime");
            XmlAttribute lastUpdate = xmlDoc.CreateAttribute("LastUpdateTime");
            jobNode.Attributes.Append(user);
            jobNode.Attributes.Append(start);
            jobNode.Attributes.Append(lastUpdate);
            user.Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            start.Value = DateTime.Now.ToString();
            lastUpdate.Value = "2016-02-25";

            documentDBNode.AppendChild(jobNode);
        }

        public static void CreateJobHistoryNode(XmlNode documentDBNode)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@".\UserSettings.xml");

            XmlNode jobHistoryNode = xmlDoc.CreateElement("JobHistory");

            XmlAttribute user = xmlDoc.CreateAttribute("User");
            XmlAttribute start = xmlDoc.CreateAttribute("FetchStartTime");
            XmlAttribute end = xmlDoc.CreateAttribute("FetchEndTime");
            XmlAttribute lastUpdate = xmlDoc.CreateAttribute("LastUpdatedTime");
            jobHistoryNode.Attributes.Append(user);
            jobHistoryNode.Attributes.Append(start);
            jobHistoryNode.Attributes.Append(end);
            jobHistoryNode.Attributes.Append(lastUpdate);
            user.Value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            start.Value = DateTime.Now.ToString();
            end.Value = "2016-02-25";

            documentDBNode.AppendChild(jobHistoryNode);

            xmlDoc.Save(@".\UserSettings.xml");

        }
    }
}
