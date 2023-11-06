using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Cfdi;

namespace SIAAPI.Controllers.Cfdi
{
    public class CertificadosController : Controller
    {
        private CertificadosContext db = new CertificadosContext();

        // GET: Certificados
        public ActionResult Index()
        {
            return View(db.certificados.ToList());
        }

        // GET: Certificados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Certificados certificados = db.certificados.Find(id);
            if (certificados == null)
            {
                return HttpNotFound();
            }
            return View(certificados);
        }

        // GET: Certificados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Certificados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCertificado,PassCertificado,Nombredelcertificado,Nombredelkey,UsuarioMultifacturas,PassMultifacturas,FechaVigenciaCertificado")] Certificados certificados)
        {
            if (ModelState.IsValid)
            {
                db.certificados.Add(certificados);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(certificados);
        }

        // GET: Certificados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Certificados certificados = db.certificados.Find(id);
            if (certificados == null)
            {
                return HttpNotFound();
            }
            return View(certificados);
        }

        // POST: Certificados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCertificado,PassCertificado,Nombredelcertificado,Nombredelkey,UsuarioMultifacturas,PassMultifacturas,FechaVigenciaCertificado")] Certificados certificados)
        {
            if (ModelState.IsValid)
            {
                db.Entry(certificados).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(certificados);
        }

        // GET: Certificados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Certificados certificados = db.certificados.Find(id);
            if (certificados == null)
            {
                return HttpNotFound();
            }
            return View(certificados);
        }

        // POST: Certificados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Certificados certificados = db.certificados.Find(id);
            db.certificados.Remove(certificados);
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
