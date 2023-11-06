using PagedList;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Produccion
{
    public class ModelosDeProduccionController : Controller
    {
        
        private ModelosDeProduccionContext db = new ModelosDeProduccionContext();
        // GET: ModelosDeProduccion
        [Authorize(Roles = "Administrador,AdminProduccion,Gerencia,Sistemas")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.ModelosDeProducciones
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.ModelosDeProducciones.OrderBy(e => e.IDModeloProduccion).Count(); // Total number of elements

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
        //    var lista = from e in db.ModelosDeProducciones
        //                orderby e.IDModeloProduccion
        //                select e;
        //    return View(lista);
        //}

        // GET: ModelosDeProduccion/Details/5
        public ActionResult Details(int id)
        {

            var elemento = db.ModelosDeProducciones.Single(m => m.IDModeloProduccion == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

        // GET: ModelosDeProduccion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ModelosDeProduccion/Create
        [HttpPost]
        public ActionResult Create(ModelosDeProduccion elemento)
        {
            try
            {
                var db = new ModelosDeProduccionContext();
                db.ModelosDeProducciones.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ModelosDeProduccion/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.ModelosDeProducciones.Single(m => m.IDModeloProduccion == id);
            return View(elemento);
        }

        // POST: ModelosDeProduccion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.ModelosDeProducciones.Single(m => m.IDModeloProduccion == id);
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

        // GET: ModelosDeProduccion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ModelosDeProduccion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, ModelosDeProduccion collection)
        {
            try
            {
                // TODO: Add delete logic here

                var elemento = (from e in db.ModelosDeProducciones
                                where e.IDModeloProduccion == id
                                select e).FirstOrDefault();
                db.ModelosDeProducciones.Remove(elemento);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
