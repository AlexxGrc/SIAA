using SIAAPI.Models.Comercial;
using SIAAPI.Models.PlaneacionProduccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.PlaneacionProduccion
{
    public class RangoCostoController : Controller
    {
        private RangoPlaneacionContext db = new RangoPlaneacionContext();

        public ActionResult Agrega(int? id)
        {
            
            ViewBag.id = id;
            HEspecificacionEContext hoja = new HEspecificacionEContext();
            HEspecificacionE hespecificacion = hoja.HEspecificacionesE.Find(id);
            ViewBag.version = hespecificacion.Version;

            var elementos = from s in db.Rangos
                            where s.IDHE==id && s.Version==hespecificacion.Version
                            select s;

           
            ViewBag.rangoinferior=.01;
            ViewBag.rangosuperior = 999.99;

            ClsDatoEntero countRango = db.Database.SqlQuery<ClsDatoEntero>("select count(IDHE) as Dato from RangoPlaneacionCosto where IDHE='" + id + "' and Version='" + hespecificacion.Version + "'").ToList()[0];
            if (countRango.Dato != 0)
            {

                List<RangoPlaneacionCosto> rangoplaneacion = db.Database.SqlQuery<RangoPlaneacionCosto>("select * from [dbo].[RangoPlaneacionCosto] where IDRP=(SELECT MAX(IDRP) from RangoPlaneacionCosto) and IDHE='" + id + "' and Version='" + hespecificacion.Version + "'").ToList();
                decimal num = rangoplaneacion.Select(s => s.RangoSup).FirstOrDefault();

                decimal rangocorrecto = num + .01M;
                ViewBag.rangoinferior = rangocorrecto;

            }
            return View(elementos);
        }



        [HttpPost]
        public ActionResult Insertar(int idhe, int version,decimal rangi, decimal rangs)
        {

            try
            {
              db.Database.ExecuteSqlCommand("insert into [dbo].[RangoPlaneacionCosto]([IDHE],[RangoInf],[RangoSup],[Costo],[Version]) values ('" + idhe + "','" + rangi + "','" + rangs + "',0,'" + version + "')");
              return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        [HttpPost]
        public JsonResult Edititem(int id, decimal rangi, decimal rangs)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update [dbo].[RangoPlaneacionCosto] set [RangoInf]=" + rangi + ", [RangoSup]='" + rangs + "' where [IDRP]=" + id);

          
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        [HttpPost]
        public JsonResult Deleteitem(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[RangoPlaneacionCosto] where [IDRP]='" + id + "'");

         
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


    }
}