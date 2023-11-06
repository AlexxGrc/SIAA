using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.CartaPorte;

namespace SIAAPI.Controllers.CartaPorte
{
    public class c_ClaveProdServCPController : Controller
    {
        private c_ClaveProdServCPContext db = new c_ClaveProdServCPContext();

        // GET: c_ClaveProdServCP
        public ActionResult Index()
        {
            return View(db.clave.ToList());
        }

        // GET: c_ClaveProdServCP/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ClaveProdServCP c_ClaveProdServCP = db.clave.Find(id);
            if (c_ClaveProdServCP == null)
            {
                return HttpNotFound();
            }
            return View(c_ClaveProdServCP);
        }

        // GET: c_ClaveProdServCP/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_ClaveProdServCP/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClaveP,Descripcion,PalabrasS,MaterialP,FIVigencia,FFVigencia")] c_ClaveProdServCP c_ClaveProdServCP)
        {
            if (ModelState.IsValid)
            {
                db.clave.Add(c_ClaveProdServCP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_ClaveProdServCP);
        }

        // GET: c_ClaveProdServCP/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ClaveProdServCP c_ClaveProdServCP = db.clave.Find(id);
            if (c_ClaveProdServCP == null)
            {
                return HttpNotFound();
            }
            return View(c_ClaveProdServCP);
        }

        // POST: c_ClaveProdServCP/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClaveP,Descripcion,PalabrasS,MaterialP,FIVigencia,FFVigencia")] c_ClaveProdServCP c_ClaveProdServCP)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_ClaveProdServCP).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_ClaveProdServCP);
        }

        // GET: c_ClaveProdServCP/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ClaveProdServCP c_ClaveProdServCP = db.clave.Find(id);
            if (c_ClaveProdServCP == null)
            {
                return HttpNotFound();
            }
            return View(c_ClaveProdServCP);
        }

        // POST: c_ClaveProdServCP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_ClaveProdServCP c_ClaveProdServCP = db.clave.Find(id);
            db.clave.Remove(c_ClaveProdServCP);
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
