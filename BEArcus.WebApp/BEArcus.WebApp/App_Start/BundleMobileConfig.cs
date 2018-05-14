using System.Web;
using System.Web.Optimization;

namespace BEArcus.WebApp {
    public class BundleMobileConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquerymobile")
                .Include("~/Scripts/jquery.mobile-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/Mobilejs").Include(
                "~/Scripts/jquery.mobile-*",
                "~/Scripts/jquery-*"));

            bundles.Add(new StyleBundle("~/Content/Mobile/css")
                .Include("~/Content/Site.Mobile.css"));
            
            bundles.Add(new StyleBundle("~/Content/jquerymobile/css")
                .Include("~/Content/jquery.mobile-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.*",
                     "~/Content/site.*"));

            bundles.Add(new StyleBundle("~/Content/Mobilecss").Include(
                "~/Content/jquery.mobile.structure-*",
                "~/Content/jquery.mobile-*"));
        }
    }
}