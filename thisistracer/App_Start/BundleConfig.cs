using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace thisistracer
{
    class BundleConfig
    {
        public static void RegisterBundle(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js/jquery").Include(
                "~/Content/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/js/modernizr").Include(
                "~/Content/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/js/bootstrap").Include(
                "~/Content/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Content/Styles/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/site").Include(
                "~/Content/Styles/Site.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
