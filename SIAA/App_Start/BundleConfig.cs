using System.Web;
using System.Web.Optimization;

namespace SIAA
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                    "~/Estilos/plugins/fontawesome-free/css/all.min.css",
                    "~/Estilos/Plugins/fontawesome-free/css/fontawesome.css",

                    //"~/Estilos/Bootstrap/bootstrap.css",
                    //"~/Estilos/assets/css/bootstrap.min.css",
                    //"~/Estilos/plugins/tempusdominus-bootstrap-4/css/tempusdominus-bootstrap-4.min.css",
                    //"~/Estilos/plugins/icheck-bootstrap/icheck-bootstrap.min.css",
                    //"~/Estilos/plugins/jqvmap/jqvmap.min.css",
                    "~/Estilos/Menu/css/adminlte.css",
                    "~/Estilos/plugins/overlayScrollbars/css/OverlayScrollbars.min.css",
                    "~/Estilos/plugins/daterangepicker/daterangepicker.css",
                    "~/Estilos/plugins/summernote/summernote-bs4.min.css"
                    ));
            //   "~/Estilos/libs/bootstrap.css",
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
             "~/scripts/libs/jquery_1.10.2/jquery-1.10.2.min.js",
                        "~/Estilos/plugins/jquery/jquery.js",
                        "~/Estilos/plugins/jquery-ui/jquery-ui.min.js",
                        "~/Estilos/bootstrapjs/locales/bootstrap.bundle.min.js",
                        "~/Estilos/plugins/chart.js/Chart.min.js",
                        //"~/Estilos/plugins/sparklines/sparkline.js",
                        //"~/Estilos/plugins/jqvmap/jquery.vmap.min.js",
                        //"~/Estilos/plugins/jqvmap/maps/jquery.vmap.usa.js",
                        //"~/Estilos/plugins/jquery-knob/jquery.knob.min.js",
                        "~/Estilos/plugins/moment/moment.min.js",
                        "~/Estilos/plugins/daterangepicker/daterangepicker.js",
                        //"~/Estilos/plugins/tempusdominus-bootstrap-4/js/tempusdominus-bootstrap-4.min.js",
                        "~/Estilos/plugins/summernote/summernote-bs4.min.js",
                        "~/Estilos/plugins/overlayScrollbars/js/jquery.overlayScrollbars.min.js",
                        "~/Estilos/Menu/js/adminlte.js",
                        "~/Estilos/Menu/js/demo.js",
                        //"~/Estilos/Menu/js/pages/dashboard.js",
                        "~/scripts/libs/alertifyjs/alertify.min.js"
                       /* "~/Content/vendor/bootstrap/js/bootstrap.min.js"*/));


        }
    }
}
