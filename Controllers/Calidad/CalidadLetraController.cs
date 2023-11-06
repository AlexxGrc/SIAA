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
    public class CalidadLetraController : Controller
    {
        private CalidadLetraContext db = new CalidadLetraContext();

        // GET: CalidadLetra
        public ActionResult Index()
        {
            return View(db.CalidadLetras.ToList());
        }

        // GET: CalidadLetra/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalidadLetra calidadLetra = db.CalidadLetras.Find(id);
            if (calidadLetra == null)
            {
                return HttpNotFound();
            }
            return View(calidadLetra);
        }

        // GET: CalidadLetra/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CalidadLetra/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDLetra,LI_Lote_mill,LS_Lote_mill,NGI1,NGI2,NGI3")] CalidadLetra calidadLetra)
        {
            if (ModelState.IsValid)
            {
                db.CalidadLetras.Add(calidadLetra);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(calidadLetra);
        }

        // GET: CalidadLetra/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalidadLetra calidadLetra = db.CalidadLetras.Find(id);
            if (calidadLetra == null)
            {
                return HttpNotFound();
            }
            return View(calidadLetra);
        }

        // POST: CalidadLetra/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDLetra,LI_Lote_mill,LS_Lote_mill,NGI1,NGI2,NGI3")] CalidadLetra calidadLetra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calidadLetra).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(calidadLetra);
        }

        // GET: CalidadLetra/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalidadLetra calidadLetra = db.CalidadLetras.Find(id);
            if (calidadLetra == null)
            {
                return HttpNotFound();
            }
            return View(calidadLetra);
        }

        // POST: CalidadLetra/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CalidadLetra calidadLetra = db.CalidadLetras.Find(id);
            db.CalidadLetras.Remove(calidadLetra);
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
