using System.Web.Optimization;

namespace englishProject
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            /////////////////////////////////////JAVASCRIPT///////////////////////////////////////////////////
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/knockout-{version}.js",
                         "~/Scripts/knockout-3.4.0.debug.js"

                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                            "~/Scripts/bootstrap.js"
                          ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            /////////////////////////////////////JAVASCRIPT///////////////////////////////////////////////////

            ////////////////////////////CSS//////////////////////////////////////////////////////////////

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/navbar.css",
                      "~/Content/font-awesome-4.5.0/css/font-awesome.min.css"
                      ));

            ////////////////////////////CSS//////////////////////////////////////////////////////////////
        }
    }
}