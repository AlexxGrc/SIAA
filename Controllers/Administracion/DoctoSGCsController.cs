using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.miempresa;
using System.IO;
using System.Text;
using SIAAPI.Controllers.Cfdi;

namespace SIAAPI.Controllers.Administracion
{
    public class DoctoSGCsController : Controller
    {
        private DoctoSGCContext db = new DoctoSGCContext();

        // GET: DoctoSGCs
        public ActionResult Index()
        {
            return View(db.DoctoSGC.ToList());
        }

        // GET: DoctoSGCs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctoSGC doctoSGC = db.DoctoSGC.Find(id);
            if (doctoSGC == null)
            {
                return HttpNotFound();
            }
            return View(doctoSGC);
        }

        // GET: DoctoSGCs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DoctoSGCs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDDocto,Clave,Documento")] DoctoSGC doctoSGC)
        {
            if (ModelState.IsValid)
            {
                db.DoctoSGC.Add(doctoSGC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctoSGC);
        }

        // GET: DoctoSGCs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctoSGC doctoSGC = db.DoctoSGC.Find(id);
            if (doctoSGC == null)
            {
                return HttpNotFound();
            }
            return View(doctoSGC);
        }

        // POST: DoctoSGCs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDDocto,Clave,Documento")] DoctoSGC doctoSGC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctoSGC).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctoSGC);
        }

        // GET: DoctoSGCs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctoSGC doctoSGC = db.DoctoSGC.Find(id);
            if (doctoSGC == null)
            {
                return HttpNotFound();
            }
            return View(doctoSGC);
        }

        // POST: DoctoSGCs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DoctoSGC doctoSGC = db.DoctoSGC.Find(id);
            db.DoctoSGC.Remove(doctoSGC);
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
