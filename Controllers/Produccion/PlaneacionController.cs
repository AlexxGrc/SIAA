using MathParserTK;
using Newtonsoft.Json.Linq;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Produccion
{
    public class PlaneacionController : Controller
    {
        // GET: Planeacion
        
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult getModelosvsProcesos()
        {
            modelAddEditPlaneacion da = new modelAddEditPlaneacion();
            PlenacionContext db = new PlenacionContext();
            try
            {
                da.Modelos = db.ModelosDeProduccions.ToList();
                da.VistaModeloProceso = db.VistaModeloProcesos.ToList();
              
                return Json(da);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        public ActionResult getOrdendeproduccion()
        {
            return Json(500, "");
        }


        /// <summary>
        /// /obtiene el articulo basado en el id con la idea de tener los datos para la porden
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Articulo getArticulo( int id)
        {
            try
            {
                ArticuloContext db = new ArticuloContext();
                Articulo vanessa = db.Articulo.Find(id);
                return vanessa;
            }
            catch(Exception err)
            {
                string error = err.Message;

                return new Articulo();
            }
        }

        public Caracteristica getPresentacion(int id)
        {
            Caracteristica zu = new Caracteristica();
            try
            {

                ArticuloContext ar = new ArticuloContext();
                zu = ar.Caracteristica.Find(id);
                return zu;
            }
            catch (Exception err)
            {
                string error = err.Message;
                return zu;
            }
        }
        /// <summary>
        ///  metodo que evaluar la formula y redondea a factorcierre
        /// </summary>
     
        /// <param name="formula"></param> es la formula para calcular la cantidad de articulo a consumir
        /// <param name="factorcierre"></param> es el redondeo que va tomar para el costo
        /// <returns></returns>

    }
}