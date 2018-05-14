/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System.Web.Mvc;
using System.Xml;
using System.Diagnostics;
using System;

namespace BEArcus.WebApp.Controllers
{
    public class AgentController : Controller
    {
        /// <summary>
        /// Updates the Agents Configuration.xml file with endpoint url and authorization key.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        public void UpdateAgentConfiguration(string endpointUrl, string authorizationKey)
        {
            try
            {
                Trace.WriteLine("Entering CreateSetting method");
                XmlDocument xmlDoc = new XmlDocument();

                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Loading Configuration.xml");
                xmlDoc.Load(System.Web.HttpContext.Current.Server.MapPath("~/Agent/AgentApp/Settings/Configuration.xml"));
                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("ns", "http://schemas.arcus.com/configuration");
                //string query = string.Format("//ns:DataStores/ns:DocumentDB",nsmgr);
                XmlNode endpoint = xmlDoc.SelectSingleNode("//ns:DataStores/ns:DocumentDB", nsmgr);
                endpoint.Attributes["EndPointUrl"].Value = endpointUrl;
                endpoint.Attributes["AuthorizationKey"].Value = authorizationKey;

                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Saving Configuration.xml");
                xmlDoc.Save(System.Web.HttpContext.Current.Server.MapPath("~/Agent/AgentApp/Settings/Configuration.xml"));
            }
            catch (Exception e)
            {
                Trace.WriteLine("Unable to update Agent's Configuration.xml" + e.Message);
            }
        }
    }
}