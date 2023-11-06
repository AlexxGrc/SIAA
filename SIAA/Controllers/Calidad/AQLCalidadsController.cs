
using PagedList;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.Calidad
{
    public class AQLCalidadsController : Controller
    {
        private ArticuloContext db = new ArticuloContext();

        // GET: AQLCalidads
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
            var elementos = from s in db.AQLCalidad
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
            int count = db.AQLCalidad.OrderBy(e => e.IDAQL).Count(); // Total number of elements

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
        //    return View(db.AQLCalidad.ToList());
        //}

        // GET: AQLCalidads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AQLCalidad aQLCalidad = db.AQLCalidad.Find(id);
            if (aQLCalidad == null)
            {
                return HttpNotFound();
            }
            return View(aQLCalidad);
        }

        // GET: AQLCalidads/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AQLCalidads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDAQL,Descripcion")] AQLCalidad aQLCalidad)
        {
            if (ModelState.IsValid)
            {
                db.AQLCalidad.Add(aQLCalidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aQLCalidad);
        }

        // GET: AQLCalidads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AQLCalidad aQLCalidad = db.AQLCalidad.Find(id);
            if (aQLCalidad == null)
            {
                return HttpNotFound();
            }
            return View(aQLCalidad);
        }

        // POST: AQLCalidads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAQL,Descripcion")] AQLCalidad aQLCalidad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aQLCalidad).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aQLCalidad);
        }

        // GET: AQLCalidads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AQLCalidad aQLCalidad = db.AQLCalidad.Find(id);
            if (aQLCalidad == null)
            {
                return HttpNotFound();
            }
            return View(aQLCalidad);
        }

        // POST: AQLCalidads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AQLCalidad aQLCalidad = db.AQLCalidad.Find(id);
            db.AQLCalidad.Remove(aQLCalidad);
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
