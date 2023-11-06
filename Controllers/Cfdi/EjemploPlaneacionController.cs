using SIAAPI.ClasesProduccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Cfdi
{
    public class EjemploPlaneacionController : Controller
    {
        // GET: EjemploPlaneacion
        public ActionResult Index()
        {
            EjemplodePlaneacion x = new EjemplodePlaneacion();
          
            return View();
        }
    }
}