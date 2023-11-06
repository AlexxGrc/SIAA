
using SIAAPI.Facturas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers
{
    public class EjemploFacturaController : Controller
    {
        // GET: EjemploFactura
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VerEjemploFactura()
        {
            EjemploFactura x = new EjemploFactura();
           var respuesta= Server.HtmlEncode( x.timbrarfacturaejemplo());
            return Content(respuesta);



        }

        public ActionResult VerEjemplopagos()
        {

            EjemploFacturaPagos x = new EjemploFacturaPagos();

            string respuesta = x.timbrarfacturaejemplopagos();


            var reshtml = Server.HtmlEncode("->" + respuesta);

            return Content(reshtml);
        }
    }
}