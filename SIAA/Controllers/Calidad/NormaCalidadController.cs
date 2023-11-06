using SIAAPI.Models.Calidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Calidad
{
    public class NormaCalidadController : Controller
    {
        // GET: NormaCalidad
        public NormaCalidadContext db = new NormaCalidadContext();
        public ActionResult Index()
        { 
          
            var lista =  from e in db.NormasCalidad
                        orderby e.IDNorma
                         select e;
           return View(lista);
        }

        // GET: NormaCalidad/Details/5
        public ActionResult Details(int id)
        {
            var elemento = db.NormasCalidad.Single(m => m.IDNorma == id);
            if (elemento == null)
            {
                return NotFound();
            }
            return View(elemento);
        }

        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        // GET: NormaCalidad/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NormaCalidad/Create
        [HttpPost]

        public ActionResult Create(NormaCalidad elemento)
        {
            try
            {
                var db = new NormaCalidadContext();
                db.NormasCalidad.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: NormaCalidad/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.NormasCalidad.Single(m => m.IDNorma == id);
            return View(elemento);
        }

        // POST: NormaCalidad/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.NormasCalidad.Single(m => m.IDNorma == id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(elemento);
            }
            catch
            {
                return View();
            }
        }

        // GET: NormaCalidad/Delete/5
        public ActionResult Delete(int id)
        {
            var elemento = db.NormasCalidad.Single(m => m.IDNorma == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }

        // POST: NormaCalidad/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                var elemento = db.NormasCalidad.Single(m => m.IDNorma == id);
                db.NormasCalidad.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

