using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Inventarios;
using PagedList;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;
using SIAAPI.ViewModels.Articulo;

namespace SIAAPI.Controllers.Inventarios
{
    public class VMovArticuloController : Controller
    {
        private VMovArticuloContext db = new VMovArticuloContext();

        // GET: VMovArticulo
        //public ActionResult Index()
        //{
        //return View(db.VMovArticulos.ToList());
        [Authorize(Roles = "Administrador")]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.AlmacenSortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.PresentacionSortParm = String.IsNullOrEmpty(sortOrder) ? "Presentacion" : "Presentacion";
            ViewBag.AccionSortParm = String.IsNullOrEmpty(sortOrder) ? "Accion" : "Accion";
            ViewBag.DocumentoSortParm = String.IsNullOrEmpty(sortOrder) ? "Documento" : "Documento";
            ViewBag.FechaSortParm = sortOrder == "Fecha" ? "date_desc" : "Date";
            ViewBag.TipoOperacionSortParm = String.IsNullOrEmpty(sortOrder) ? "TipoOperacion" : "TipoOperacion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;

            //Paginación
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //Paginación
            var elementos = from s in db.VMovArticulos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {

                elementos = elementos.Where(s => s.TipoOperacion.Contains(searchString) || s.Almacen.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Presentacion.Contains(searchString) || s.Accion.Contains(searchString) || s.Documento.Contains(searchString) || s.Fecha.ToString().Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Almacen":
                    elementos = elementos.OrderBy(s => s.Almacen);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "Presentacion":
                    elementos = elementos.OrderBy(s => s.Presentacion);
                    break;
                case "Accion":
                    elementos = elementos.OrderBy(s => s.Accion);
                    break;
                case "Documento":
                    elementos = elementos.OrderBy(s => s.Documento);
                    break;
                case "TipoOperacion":
                    elementos = elementos.OrderBy(s => s.TipoOperacion);
                    break;
                case "Date":
                    elementos = elementos.OrderBy(s => s.Fecha);
                    break;
                case "date_desc":
                    elementos = elementos.OrderByDescending(s => s.Fecha);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.IDMovimiento);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.VMovArticulos.OrderBy(e => e.IDMovimiento).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            //return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }



        // GET: VMovArticulo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VMovArticulo vMovArticulo = db.VMovArticulos.Find(id);
            if (vMovArticulo == null)
            {
                return HttpNotFound();
            }
            List<VMovArticulo> movimientoList = db.Database.SqlQuery<VMovArticulo>("select IDMovimiento,Fecha,Hora,Accion,Documento,NoDocumento,TipoOperacion,IDAlmacen,Almacen,IDArticulo,IDCaracteristica,Cref,Descripcion,Presentacion,Lote,Cantidad,Acumulado,observacion from dbo.VMovArticulo where [IdMovimiento]='" + id + "'").ToList();
            ViewBag.movimiento = movimientoList;
            return View(vMovArticulo);
        }

        // GET: VMovArticulo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VMovArticulo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDMovimiento,Fecha,Hora,Accion,Documento,NoDocumento,TipoOperacion,IDAlmacen,Almacen,IDArticulo,IDCaracteristica,Cref,Descripcion,Presentación,Lote,Cantidad,Acumulado,observacion")] VMovArticulo vMovArticulo)
        {
            if (ModelState.IsValid)
            {
                db.VMovArticulos.Add(vMovArticulo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vMovArticulo);
        }

      private ArticuloContext ar = new ArticuloContext();
        public JsonResult getmetodo(int id)
        {
            VCaracteristicaContext cara = new VCaracteristicaContext();
      var caracteristica = cara.VCaracteristica.Where(s=> s.Articulo_IDArticulo == id );
            List<SelectListItem> li = new List<SelectListItem>();

            foreach (var m in caracteristica)
            {
                li.Add(new SelectListItem { Text = m.Presentacion, Value = m.ID.ToString() });
              
            }
            return Json(new SelectList(li, "Value", "Text", JsonRequestBehavior.AllowGet));
        }


      }
}
