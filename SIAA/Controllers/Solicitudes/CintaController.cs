using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Solicitudes
{
    public class CintaController : Controller
    {
        // GET: Cinta
        [HttpPost]
        public JsonResult DameDatos(int id)
        {
            Articulo articulo = new ArticuloContext().Articulo.Find(id);
            return Json(articulo, JsonRequestBehavior.AllowGet);
        }
    }
}