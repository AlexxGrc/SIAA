using SIAAPI.Models.Comercial;
using SIAAPI.Models.PlaneacionProduccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Produccion
{
    public class HojaEspecController : Controller
    {
        // GET: HojaEspec
        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// Crear la hoja de especificacion en base a la familia
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult CrearHE(int id, int IdEspec)
        {
            ViewBag.Espec = IdEspec;
            List<AtributosdeHE> atributos ;
            atributos = new AtributosdeHEContext().Database.SqlQuery<AtributosdeHE>("Select * from AtributosdeHE where idfamilia="+id).ToList();
            return View(atributos);

        }

        /// <summary>
        /// Crear la hoja de especificacion en base a la familia
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult CrearHEpec(int id,String Presentacion)
        {
            HEspecificacionEContext bd = new HEspecificacionEContext();
            try
            {
                bd.Database.ExecuteSqlCommand("update [dbo].[HEspecificacionE] set ESPECIFICACION='" + Presentacion + "' where  IDHE=" + id);
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
            }
            HEspecificacionE especificacion = bd.HEspecificacionesE.Find(id);
            
          
                return RedirectToAction("IndexHE", "PlanPlaneacionProduccionE", new { @sortOrder = "", @currentFilter = "", @searchString = especificacion.Descripcion, @page = 1, @PageSize = 15 });
            

        }

    }
}