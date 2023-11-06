using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Produccion;

namespace SIAAPI.Controllers.Produccion
{
    public class EstadoOrdenController : Controller
    {
        private EstadoOrdenContext db = new EstadoOrdenContext();

        // GET: EstadoOrden
        public ActionResult Index()
        {
            return View(db.EstadoOrdenes.ToList());
        }

        // GET: EstadoOrden/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstadoOrden estadoOrden = db.EstadoOrdenes.Find(id);
            if (estadoOrden == null)
            {
                return HttpNotFound();
            }
            return View(estadoOrden);
        }

        // GET: EstadoOrden/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstadoOrden/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDEstadoOrden,Descripcion")] EstadoOrden estadoOrden)
        {
            if (ModelState.IsValid)
            {
                db.EstadoOrdenes.Add(estadoOrden);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(estadoOrden);
        }

        // GET: EstadoOrden/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstadoOrden estadoOrden = db.EstadoOrdenes.Find(id);
            if (estadoOrden == null)
            {
                return HttpNotFound();
            }
            return View(estadoOrden);
        }

        // POST: EstadoOrden/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDEstadoOrden,Descripcion")] EstadoOrden estadoOrden)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estadoOrden).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estadoOrden);
        }

        // GET: EstadoOrden/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstadoOrden estadoOrden = db.EstadoOrdenes.Find(id);
            if (estadoOrden == null)
            {
                return HttpNotFound();
            }
            return View(estadoOrden);
        }

        // POST: EstadoOrden/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EstadoOrden estadoOrden = db.EstadoOrdenes.Find(id);
            db.EstadoOrdenes.Remove(estadoOrden);
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
