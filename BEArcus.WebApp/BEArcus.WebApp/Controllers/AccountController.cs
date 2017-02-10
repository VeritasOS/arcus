/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BEArcus.WebApp.Models;
using System.Web.Security;
using System.IO.Compression;
using System.Diagnostics;
using System.Web.Configuration;
using System.Configuration;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Cookies;

namespace BEArcus.WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public static bool login;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        /// <summary>
        /// Checks if Endpoint Url and Authorization key is present in the Web.config file and redirects the user accordingly
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult LoginHandler()
        {
            Trace.WriteLine("Entering LoginHandler method");

            //Gets the endpoint url and authorization value from web.config
            string EndpointUrl = WebConfigurationManager.AppSettings["EndpointUrl"];
            string AuthorizationKey = WebConfigurationManager.AppSettings["AuthorizationKey"];

            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Check if EndPoint Url and Authorization Key is present in Web.config");
            if (EndpointUrl.Equals("EndpointUrl"))
            {
                //Endpoint url is not present in web.config (Display DocumentDB Credentials page)
                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Accept Endpoint Url and Authorization key from user");
                return RedirectToAction("DocumentDBCredentials", "Account");
            }

            //endpoint url is present in web.cofig (Display Home Page)
            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Display Home Page");
            return RedirectToAction("HomePage", "MediaServer");
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // Send an OpenID Connect sign-out request.
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            //return RedirectToAction("LoginHandler", "Account");
            return Redirect("https://login.windows.net/{0}/oauth2/logout?post_logout_redirect_uri={}");
        }

        //
        // GET: /Account/DocumentDBCredentials
        //[Authorize]
        [AllowAnonymous]
        public ActionResult DocumentDBCredentials()
        {
            Trace.WriteLine("Entering DocumentDBCredentials GET method");
            return View();
        }

        //
        // POST: /Account/DocumentDBCredentials
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> DocumentDBCredentials(DocumentDBCredentialsViewModel model)
        {
            Trace.WriteLine("Entering DocumentDBCredentials POST method");
            if (ModelState.IsValid)
            {
                //Check if Database is accessible
                Task<bool> flag = DocumentDBDataController.IsDatabaseAccessible(model.EndPointUrl, model.AuthorizationKey);
                if (flag.Result)
                {
                    //Helps to open the Root level web.config file.
                    Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                    //Modifying the AppKey from AppValue to AppValue1
                    webConfigApp.AppSettings.Settings["EndpointUrl"].Value = model.EndPointUrl;
                    webConfigApp.AppSettings.Settings["AuthorizationKey"].Value = model.AuthorizationKey;
                    //Save the Modified settings of AppSettings.                   
                    webConfigApp.Save();

                    //Update the configuration.xml file in Agent Application with Endpoint Url and Authorization Key.
                    AgentController agent = new AgentController();
                    Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Calling CreateSetting method");
                    agent.UpdateAgentConfiguration(model.EndPointUrl, model.AuthorizationKey);
                    return RedirectToAction("AgentDownload", "Account");
                }
                else
                {
                    ViewBag.ErrorMessage = "DocumentDB database is not accessible";
                    return View("Error");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult AgentDownload()
        {
            Trace.WriteLine("Entering AgentDownload method");
            return View();
        }

        /// <summary>
        /// Download Agent Application
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public FileResult Download(string FileName)
        {
            Trace.WriteLine("Entering Download method");

            //Delete Existing AgentApp.zip
            if (System.IO.File.Exists(Server.MapPath
                      ("~/Agent/AgentApp.zip")))
            {
                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Deleting existing AgentApp.zip");
                System.IO.File.Delete(Server.MapPath
                              ("~/Agent/AgentApp.zip"));
            }

            //Creating AgentApp.zip
            Trace.TraceInformation(DateTime.Now.ToLongTimeString() + "Creating Agent.zip");
            ZipFile.CreateFromDirectory(Server.MapPath("~/Agent/AgentApp"), Server.MapPath("~/Agent/AgentApp.zip"));

            return File("~/Agent/AgentApp.zip", "application/zip", "Agent.zip");
        }

        [AllowAnonymous]
        public ActionResult DocumentDBDetails()
        {
            Trace.WriteLine("Entering DocumentDBDetails method");
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Main", "MediaServer");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}