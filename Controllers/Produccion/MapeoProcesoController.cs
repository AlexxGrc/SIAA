using SIAAPI.Models.Comercial;
using SIAAPI.Models.Produccion;
using SIAAPI.ViewModels;
using SIAAPI.ViewModels.produccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SIAAPI.Controllers.Produccion
{
    //[Authorize(Roles = "Administrador")]
    public class MapeoProcesoController : Controller
    {
        private ProcesoContext db = new ProcesoContext();
        //private VOrdenProduccionContext dbo = new VOrdenProduccionContext();
        private VMaquinaProcesoContext dbm = new VMaquinaProcesoContext();


        // GET: MapeoProceso
        public ActionResult Index()
        {
            var datosProceso = db.Database.SqlQuery<Proceso>("select * from dbo.Proceso order by IDProceso asc").ToList();
            ViewBag.datosProceso = datosProceso;
            return View(datosProceso);
        }

        // GET: MapeoProceso/verOrden/5
        public ActionResult verOrden(int? idproceso)
        {
            //var datosOrden = dbo.Database.SqlQuery<OrdenProduccionDetalle>("select * from  dbo.OrdenProduccionDetalle where IDProceso = " + idproceso + " and IDEStadoOrden <>(Select IDEstadoOrden from dbo.EstadoOrden where Descripcion = 'Terminado'").ToList();
            //ViewBag.datosOrden = datosOrden;
            var datosOrden = db.Database.SqlQuery<VOrdenProduccion>("select * from  [dbo].[VOrdenProduccion] where IDProceso ="+ idproceso + " and IDEStadoOrden <>(Select IDEstadoOrden from dbo.EstadoOrden where Descripcion = 'Terminado')").ToList();
            ViewBag.datosOrden = datosOrden;
            var datosProceso = db.Database.SqlQuery<Proceso>("select * from dbo.Proceso where IDProceso = " + idproceso + "").ToList();
            ViewBag.datosProceso = datosProceso;
            return View(datosOrden);
        }

        // GET: MapeoProceso/verOrden/5
        public ActionResult verMaquina(int? idproceso)
        {

            var datosMaquina = dbm.Database.SqlQuery<VMaquinaProceso>("select * from VMaquinaProceso where IDProceso = " + idproceso + "").ToList();
            ViewBag.datosMaquina = datosMaquina;
            var datosProceso = db.Database.SqlQuery<Proceso>("select * from dbo.Proceso where IDProceso = " + idproceso + "").ToList();
            ViewBag.datosProceso = datosProceso;
            ViewBag.idProceso = idproceso;


            return View(datosMaquina);
            
        }

        // GET: MapeoProceso/verBitacora/5

    }
}
