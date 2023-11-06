using CrystalDecisions.CrystalReports.Engine;
using PagedList;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;


namespace SIAAPI.Controllers.Produccion
{
    public class ModeloProcesoController : Controller
    {
        private ModeloProcesoContext db = new ModeloProcesoContext();
        private VistaModeloProcesoContext vista = new VistaModeloProcesoContext();
        private ProcesoContext p = new ProcesoContext();
        private ModelosDeProduccionContext mp = new ModelosDeProduccionContext();
        // GET: ModeloProceso
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.NombreProcesoSortParm = String.IsNullOrEmpty(sortOrder) ? "NombreProceso" : "NombreProceso";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in vista.VistaModeloProcesos
                                        select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Descripcion.Contains(searchString) || s.NombreProceso.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "NombreProceso":
                    elementos = elementos.OrderBy(s => s.NombreProceso);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = vista.VistaModeloProcesos.OrderBy(e => e.IDModeloProceso).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }
        //public ActionResult Index()
        //{
        //    //Se construyo a partir de la VistaModeloProceso,  no desde aquí
        //    var lista =  from e in vista.VistaModeloProcesos
        //                orderby e.Descripcion
        //                select e;
        //    return View(lista);
        //}


        public ActionResult Details(int id)
        {
          var elemento = vista.VistaModeloProcesos.Single(m => m.IDModeloProceso == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

        // GET: ModeloProceso/Create
        public ActionResult Create()
        {
            var ListaDescripcion = new ModelosDeProduccionRepository().GetDescripcion();
            var ListaNombreProduccion = new ProcesoRepository().GetNombreProceso();
            ViewBag.ListaDescripcion = ListaDescripcion;
            ViewBag.ListaNombreProduccion = ListaNombreProduccion;
            return View();
        }

        // POST: ModeloProceso/Create
        [HttpPost]
        public ActionResult Create(ModeloProceso elemento)
        {
            try
            {
                var db = new ModeloProcesoContext();

                db.ModeloProceso.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ModeloProceso/Edit/5
        public ActionResult Edit(int id)
        {
            //var elemento = db.ModeloProcesos.Single(m => m.IDModeloProceso == id);
            //return View(elemento);

            var elementooriginal = db.ModeloProceso.Single(m => m.IDModeloProceso == id);
            var ListaDescripcion = new ModelosDeProduccionRepository().GetDescripcion();
            var ListaNombreProduccion = new ProcesoRepository().GetNombreProceso();
            ViewBag.ListaDescripcion = ListaDescripcion;
            ViewBag.ListaNombreProduccion = ListaNombreProduccion;
            return View(elementooriginal);


        }

        // POST: ModeloProceso/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
               var elemento = db.ModeloProceso.Single(m => m.IDModeloProceso == id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(elemento);
            }
            catch
            {
                return View();
            }
        }

        // GET: ModeloProceso/Delete/5
        public ActionResult Delete(int id)
        {
            var elemento = vista.VistaModeloProcesos.Single(m => m.IDModeloProceso == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

        // POST: ModeloProceso/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, ModeloProceso collection)
        {
            try
            {
                // TODO: Add delete logic here
                var db = new ModeloProcesoContext();
                var elemento = db.ModeloProceso.Single(m => m.IDModeloProceso == id);
                db.ModeloProceso.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
       }
        public ActionResult Reporte()
        {
            ReportDocument reporte = new ReportDocument();

            reporte.Load(Path.Combine(Server.MapPath("~/reportes/Crystal/Administracion"), "MODELOPROCESO.rpt"));

            string servidor = Conexion.Darvalordelaconexion("data source", "DefaultConnection");
            string basededatos = Conexion.Darvalordelaconexion("initial catalog", "DefaultConnection");
            string usuario = Conexion.Darvalordelaconexion("user id", "DefaultConnection");
            string pass = Conexion.Darvalordelaconexion("password", "DefaultConnection");


            reporte.DataSourceConnections[0].SetConnection(@servidor, basededatos, false);
            reporte.DataSourceConnections[0].SetLogon(usuario, pass);


            //reporte.SetParameterValue("fechaini", "2018/08/01");
            //reporte.SetParameterValue("fechafin", "2018/08/31");
            Response.Buffer = false;

            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = reporte.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Catálogo de Modelo de Producción .pdf");

            }
            catch (Exception err)
            {
                string error = err.Message;
            }
            return Redirect("index");
        }
    }
}

