using System.Web.Optimization;

namespace CLS_SLE
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));
            //Allows for live searching drop down list
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-select").Include(
                "~/Scripts/bootstrap-select.min.js",
                "~/Scripts/mapping.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/fontawesome/css/all.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/images").Include(
                     "~/Content/images/tasks-solid.png",
                     "~/Content/images/users-solid.png"
                     ));
            //Styling for bootstrap-select. Allows for live searching drop down list
            bundles.Add(new StyleBundle("~/Content/bootstrap-select").Include(
                "~/Content/bootstrap-select.min.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryUI").Include(
                      "~/Content/jquery-ui*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include(
                 "~/Scripts/jquery-ui*"));

        }
    }
}
