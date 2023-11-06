using PagedList;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Produccion
{
    public class ProcesoController : Controller
    {
        private ProcesoContext db = new ProcesoContext();
        // GET: Procesos
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NombreProcesoSortParm = String.IsNullOrEmpty(sortOrder) ? "NombreProceso" : "NombreProceso";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.Procesos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.NombreProceso.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "NombreProceso":
                    elementos = elementos.OrderBy(s => s.NombreProceso);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.NombreProceso);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Procesos.OrderBy(e => e.IDProceso).Count(); // Total number of elements

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

        //    var lista =  from e in db.Procesos
        //                orderby e.IDProceso
        //                select e;
        //    return View(lista);
        //}

        // GET: Procesos/Details/5
        public ActionResult Details(int id)
        {
              
            var elemento = db.Procesos.Single(m => m.IDProceso == id);
            ViewBag.IdProceso = new SelectList(db.Procesos, "IDProceso", "Procesos");
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

        // GET: Procesos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Procesos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Proceso elemento)
        {
           try
            {
                var db = new ProcesoContext();
                db.Procesos.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Procesos/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.Procesos.Single(m => m.IDProceso == id);
            return View(elemento);
        }

        // POST: Procesos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
               var elemento = db.Procesos.Single(m => m.IDProceso == id);
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

        // GET: Procesos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Procesos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Proceso collection)
        {
            try
            {
                // TODO: Add delete logic here
                //var elemento = db.Procesos.Single(m => m.IDProceso == id);
                //db.Procesos.Remove(elemento);
                //db.SaveChanges();
                //return RedirectToAction("Index");

                var elemento= (from e in db.Procesos
                    where e.IDProceso == id
                       select e).FirstOrDefault();
                db.Procesos.Remove(elemento);
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
