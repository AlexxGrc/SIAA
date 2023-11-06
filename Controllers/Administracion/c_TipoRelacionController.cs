using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using PagedList;

using System.IO;



namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_TipoRelacionController : Controller
    {
        private c_TipoRelacionContext db = new c_TipoRelacionContext();

        // GET: c_tiporelacion
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.C1SortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveTipoRelacion" : "ClaveTipoRelacion";
            ViewBag.C2SortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";

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
            var elementos = from s in db.c_TipoRelaciones
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ClaveTipoRelacion.Contains(searchString) || s.Descripcion.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClavePeriocidad":
                    elementos = elementos.OrderBy(s => s.ClaveTipoRelacion);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveTipoRelacion);

                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_TipoRelaciones.OrderBy(e => e.IDTipoRelacion).Count(); // Total number of elements

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

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        // GET: c_tiporelacion/Details/5
        public ActionResult Details(int id)
        {
           
            c_TipoRelacion c_TipoRelacion = db.c_TipoRelaciones.Find(id);
            if (c_TipoRelacion == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoRelacion);
        }

        // GET: c_tiporelacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_tiporelacion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdTipoRelacion,ClaveTipoRelacion,Descripcion")] c_TipoRelacion elemento)
        {
            if (ModelState.IsValid)
            {
                db.c_TipoRelaciones.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(elemento);
        }

        // GET: c_tiporelacion/Edit/5
        public ActionResult Edit(int id)
        {
           
            c_TipoRelacion c_tiporelacion = db.c_TipoRelaciones.Find(id);
            
            return View(c_tiporelacion);
        }

        // POST: c_tiporelacion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdTipoRelacion,ClaveTipoRelacion,Descripcion")] c_TipoRelacion c_tiporelacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_tiporelacion).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_tiporelacion);
        }

        // GET: c_tiporelacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoRelacion c_tiporelacion = db.c_TipoRelaciones.Find(id);
            if (c_tiporelacion == null)
            {
                return HttpNotFound();
            }
            return View(c_tiporelacion);
        }

        // POST: c_tiporelacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_TipoRelacion c_tiporelacion = db.c_TipoRelaciones.Find(id);
            db.c_TipoRelaciones.Remove(c_tiporelacion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
       
    }
}
