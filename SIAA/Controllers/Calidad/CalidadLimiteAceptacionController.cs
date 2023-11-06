using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Calidad;

namespace SIAAPI.Controllers.Calidad
{
    public class CalidadLimiteAceptacionController : Controller
    {
        private CalidadLimiteAceptacionContext db = new CalidadLimiteAceptacionContext();

        // GET: CalidadLimiteAceptacion
        public ActionResult Index()
        {
            return View(db.CalidadLimitesAceptacion.ToList());
        }

        // GET: CalidadLimiteAceptacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalidadLimiteAceptacion calidadLimiteAceptacion = db.CalidadLimitesAceptacion.Find(id);
            if (calidadLimiteAceptacion == null)
            {
                return HttpNotFound();
            }
            return View(calidadLimiteAceptacion);
        }

        // GET: CalidadLimiteAceptacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CalidadLimiteAceptacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDAceptacion,CodigoLetra,TamanoMuestra,AC1,RE1,AC15,RE15,AC25,RE25,AC4,RE4,AC65,RE65,AC10,RE10")] CalidadLimiteAceptacion calidadLimiteAceptacion)
        {
            if (ModelState.IsValid)
            {
                db.CalidadLimitesAceptacion.Add(calidadLimiteAceptacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(calidadLimiteAceptacion);
        }

        // GET: CalidadLimiteAceptacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalidadLimiteAceptacion calidadLimiteAceptacion = db.CalidadLimitesAceptacion.Find(id);
            if (calidadLimiteAceptacion == null)
            {
                return HttpNotFound();
            }
            return View(calidadLimiteAceptacion);
        }

        // POST: CalidadLimiteAceptacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAceptacion,CodigoLetra,TamanoMuestra,AC1,RE1,AC15,RE15,AC25,RE25,AC4,RE4,AC65,RE65,AC10,RE10")] CalidadLimiteAceptacion calidadLimiteAceptacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calidadLimiteAceptacion).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(calidadLimiteAceptacion);
        }

        // GET: CalidadLimiteAceptacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalidadLimiteAceptacion calidadLimiteAceptacion = db.CalidadLimitesAceptacion.Find(id);
            if (calidadLimiteAceptacion == null)
            {
                return HttpNotFound();
            }
            return View(calidadLimiteAceptacion);
        }

        // POST: CalidadLimiteAceptacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CalidadLimiteAceptacion calidadLimiteAceptacion = db.CalidadLimitesAceptacion.Find(id);
            db.CalidadLimitesAceptacion.Remove(calidadLimiteAceptacion);
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
