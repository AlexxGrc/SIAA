using PagedList;
using SIAAPI.Models.Calidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Calidad
{
    public class MetodoController : Controller
    {
        private MetodoContext db = new MetodoContext();

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
            var elementos = from s in db.Metodos
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
            int count = db.Metodos.OrderBy(e => e.IDMetodo).Count(); // Total number of elements

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
        // GET: Inspeccions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Metodo metodo = db.Metodos.Find(id);
            if (metodo == null)
            {
                return HttpNotFound();
            }
            return View(metodo);
        }

        // GET: Inspeccions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inspeccions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDMetodo,Descripcion")] Metodo metodo)
        {
            if (ModelState.IsValid)
            {
                db.Metodos.Add(metodo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(metodo);
        }

        // GET: Inspeccions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Metodo metodo = db.Metodos.Find(id);
            if (metodo == null)
            {
                return HttpNotFound();
            }
            return View(metodo);
        }

        // POST: Inspeccions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDMetodo,Descripcion")] Metodo metodo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(metodo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(metodo);
        }

        // GET: Inspeccions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Metodo metodo = db.Metodos.Find(id);
            if (metodo == null)
            {
                return HttpNotFound();
            }
            return View(metodo);
        }

        // POST: Inspeccions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Metodo metodo = db.Metodos.Find(id);
            db.Metodos .Remove(metodo);
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