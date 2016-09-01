using System.Web;
using System.Web.Optimization;

namespace WebUI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region StyleBundle

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //   "~/Content/Site.css"
            //));

            bundles.Add(new StyleBundle("~/Content/css").Include(
               "~/Content/MySmartAdmin.css"
            ));

            bundles.Add(new StyleBundle("~/Content/printcss").Include(
               "~/Content/Print.css"
            ));

            bundles.Add(new StyleBundle("~/Content/jqueryui/css").Include(
                "~/Content/jqueryui/jquery-ui-1.10.4.custom.min.css"
            ));

            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
                "~/Content/kendo/kendo.common-bootstrap.min.css",
                "~/Content/kendo/kendo.bootstrap.min.css"
            ));

            bundles.Add(new StyleBundle("~/Content/bootstrapcss/css").Include(
                "~/Content/bootstrap/css/bootstrap.min.css",
                "~/Content/bootstrap-tour/bootstrap-tour.min.css"));

            //ckeditor
            bundles.Add(new ScriptBundle("~/Scripts/wcw/ckeditor").Include(
                "~/Scripts/wcw/ckeditor.js"
            ));

            bundles.Add(new StyleBundle("~/Content/sweetalert").Include(
                "~/Content/sweet-alert/sweet-alert.css"));

            #endregion

            #region ScriptBundle

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-1.10.2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.10.4.custom.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //    "~/Scripts/jquery.unobtrusive*",
            //    "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
               "~/Scripts/jquery-1.7.1.min.js",
               "~/Scripts/jquery.unobtrusive*",
               "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Content/bootstrap/js/bootstrap.min.js",
                "~/Scripts/bootstrap-tour/bootstrap-tour.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/kendo/kendo.all.min.js",
                "~/Scripts/kendo/cultures/kendo.culture.id-ID.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/sweet-alert").Include(
                "~/Scripts/sweet-alert/sweet-alert.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/buzz").Include(
                "~/Scripts/buzz/buzz.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/webapp").Include(
                "~/Scripts/webapp.js"));

            //library
            bundles.Add(new ScriptBundle("~/bundles/assignment").Include(
                "~/Scripts/lib/jquery.assignment-chart.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //angular js
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-route.js"));

            var scriptBundle = new ScriptBundle("~/bundles/angularApp")
                .IncludeDirectory("~/Scripts/app/", "*.js", searchSubdirectories: true);

            bundles.Add(scriptBundle);

            #endregion           

            #region smart admin

            bundles.Add(new StyleBundle("~/Content/smart-admin/css/bundle").IncludeDirectory("~/Content/smart-admin/css", "*.min.css"));

            bundles.Add(new ScriptBundle("~/Scripts/smartadmin").Include(
                "~/Scripts/smart-admin/app.config.js",
                "~/Scripts/smart-admin/plugin/jquery-touch/jquery.ui.touch-punch.min.js",
                //"~/Content/bootstrap/js/bootstrap.min.js",
                //"~/Scripts/smart-admin/notification/SmartNotification.min.js",
                "~/Scripts/smart-admin/smartwidgets/jarvis.widget.min.js",
                //"~/Scripts/smart-admin/plugin/jquery-validate/jquery.validate.min.js",
                //"~/Scripts/smart-admin/plugin/masked-input/jquery.maskedinput.min.js",
                //"~/Scripts/smart-admin/plugin/select2/select2.min.js",
                //"~/Scripts/smart-admin/plugin/bootstrap-slider/bootstrap-slider.min.js",
                //"~/Scripts/smart-admin/plugin/bootstrap-progressbar/bootstrap-progressbar.min.js",
                "~/Scripts/smart-admin/plugin/msie-fix/jquery.mb.browser.min.js",
                //"~/Scripts/smart-admin/plugin/fastclick/fastclick.min.js"
                "~/Scripts/smart-admin/app.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/full-calendar").Include(
                "~/Scripts/smart-admin/plugin/moment/moment.min.js",
                "~/Scripts/smart-admin/plugin/fullcalendar/jquery.fullcalendar.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/charts").Include(
                "~/Scripts/smart-admin/plugin/easy-pie-chart/jquery.easy-pie-chart.min.js",
                "~/Scripts/smart-admin/plugin/sparkline/jquery.sparkline.min.js",
                "~/Scripts/smart-admin/plugin/morris/morris.min.js",
                "~/Scripts/smart-admin/plugin/morris/raphael.min.js",
                "~/Scripts/smart-admin/plugin/flot/jquery.flot.cust.min.js",
                "~/Scripts/smart-admin/plugin/flot/jquery.flot.resize.min.js",
                "~/Scripts/smart-admin/plugin/flot/jquery.flot.time.min.js",
                "~/Scripts/smart-admin/plugin/flot/jquery.flot.fillbetween.min.js",
                "~/Scripts/smart-admin/plugin/flot/jquery.flot.orderBar.min.js",
                "~/Scripts/smart-admin/plugin/flot/jquery.flot.pie.min.js",
                "~/Scripts/smart-admin/plugin/flot/jquery.flot.tooltip.min.js",
                "~/Scripts/smart-admin/plugin/dygraphs/dygraph-combined.min.js",
                "~/Scripts/smart-admin/plugin/chartjs/chart.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/datatables").Include(
                "~/Scripts/smart-admin/plugin/datatables/jquery.dataTables.min.js",
                "~/Scripts/smart-admin/plugin/datatables/dataTables.colVis.min.js",
                "~/Scripts/smart-admin/plugin/datatables/dataTables.tableTools.min.js",
                "~/Scripts/smart-admin/plugin/datatables/dataTables.bootstrap.min.js",
                "~/Scripts/smart-admin/plugin/datatable-responsive/datatables.responsive.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/jq-grid").Include(
                "~/Scripts/smart-admin/plugin/jqgrid/jquery.jqGrid.min.js",
                "~/Scripts/smart-admin/plugin/jqgrid/grid.locale-en.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/forms").Include(
                "~/Scripts/smart-admin/plugin/jquery-form/jquery-form.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/smart-chat").Include(
                "~/Scripts/smart-admin/smart-chat-ui/smart.chat.ui.min.js",
                "~/Scripts/smart-admin/smart-chat-ui/smart.chat.manager.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/vector-map").Include(
                "~/Scripts/smart-admin/plugin/vectormap/jquery-jvectormap-1.2.2.min.js",
                "~/Scripts/smart-admin/plugin/vectormap/jquery-jvectormap-world-mill-en.js"
            ));

            #endregion

            bundles.IgnoreList.Clear();
            BundleTable.EnableOptimizations = true;
        }
    }
}