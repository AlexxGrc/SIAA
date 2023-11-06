using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using PagedList;

using System.IO;

namespace SIAAPI.Controllers.Produccion
{
    public class TrabajadorProcesoController : Controller
    {
        private TrabajadorContext db = new TrabajadorContext();

        // GET: TrabajadorProceso
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TrabajadorSortParm = String.IsNullOrEmpty(sortOrder) ? "Trabajador" : "Trabajador";
            ViewBag.ProcesoSortParm = String.IsNullOrEmpty(sortOrder) ? "Proceso" : "Proceso";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.TrabajadorProcesoes.Include(t => t.Proceso).Include(t => t.Trabajador)
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Trabajador.Nombre.Contains(searchString) || s.Proceso.NombreProceso.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Trabajador":
                    elementos = elementos.OrderBy(s => s.Trabajador.Nombre);
                    break;
                case "Proceso":
                    elementos = elementos.OrderBy(s => s.Proceso.NombreProceso);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Proceso.NombreProceso);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.TrabajadorProcesoes.OrderBy(e => e.IDTrabajadorProceso).Count(); // Total number of elements

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
        //    var trabajadorProcesoes = db.TrabajadorProcesoes.Include(t => t.Proceso).Include(t => t.Trabajador);
        //    return View(trabajadorProcesoes.ToList());
        //}

        // GET: TrabajadorProceso/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrabajadorProceso trabajadorProceso = db.TrabajadorProcesoes.Find(id);
            if (trabajadorProceso == null)
            {
                return HttpNotFound();
            }
            return View(trabajadorProceso);
        }

        // GET: TrabajadorProceso/Create
        public ActionResult Create()
        {
            ViewBag.IDProceso = new SelectList(db.Procesos , "IDProceso", "NombreProceso");
            ViewBag.IDTrabajador = new SelectList(db.Trabajadores.OrderBy(s=> s.Nombre), "IDTrabajador", "Nombre");
            return View();
        }

        // POST: TrabajadorProceso/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTrabajadorProceso,IDTrabajador,IDProceso")] TrabajadorProceso trabajadorProceso)
        {
            if (ModelState.IsValid)
            {
                db.TrabajadorProcesoes.Add(trabajadorProceso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDProceso = new SelectList(db.Procesos, "IDProceso", "NombreProceso", trabajadorProceso.IDProceso);
            ViewBag.IDTrabajador = new SelectList(db.Trabajadores, "IDTrabajador", "Nombre", trabajadorProceso.IDTrabajador);
            return View(trabajadorProceso);
        }

        // GET: TrabajadorProceso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrabajadorProceso trabajadorProceso = db.TrabajadorProcesoes.Find(id);
            if (trabajadorProceso == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDProceso = new SelectList(db.Procesos, "IDProceso", "NombreProceso", trabajadorProceso.IDProceso);
            ViewBag.IDTrabajador = new SelectList(db.Trabajadores, "IDTrabajador", "Nombre", trabajadorProceso.IDTrabajador);
            return View(trabajadorProceso);
        }

        // POST: TrabajadorProceso/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDTrabajadorProceso,IDTrabajador,IDProceso")] TrabajadorProceso trabajadorProceso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trabajadorProceso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDProceso = new SelectList(db.Procesos, "IDProceso", "NombreProceso", trabajadorProceso.IDProceso);
            ViewBag.IDTrabajador = new SelectList(db.Trabajadores, "IDTrabajador", "RFC", trabajadorProceso.IDTrabajador);
            return View(trabajadorProceso);
        }

        // GET: TrabajadorProceso/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrabajadorProceso trabajadorProceso = db.TrabajadorProcesoes.Find(id);
            if (trabajadorProceso == null)
            {
                return HttpNotFound();
            }
            return View(trabajadorProceso);
        }

        // POST: TrabajadorProceso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrabajadorProceso trabajadorProceso = db.TrabajadorProcesoes.Find(id);
            db.TrabajadorProcesoes.Remove(trabajadorProceso);
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
