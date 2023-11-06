using PagedList;
using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Administracion
{
    public class C_MatPrimaController : Controller
    {
        private MateriaPrimaContext db = new MateriaPrimaContext();
        // GET: C_MatPrima
        public ActionResult IndexMateria(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            var elementos = from s in db.Pesos.OrderBy(s=> s.IDmatpri)
                            select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.IDmatpri.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ID":
                    elementos = elementos.OrderBy(s => s.idpeso);
                    break;
                case "IDMATPRI":
                    elementos = elementos.OrderBy(s => s.IDmatpri);
                    break;
                case "PESOXM":
                    elementos = elementos.OrderBy(s => s.PesoxMt);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.idpeso);
                    break;
            }
        
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Pesos.OrderBy(e => e.idpeso).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25", Selected = true },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 25);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
            
        }
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: Almacen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Peso elemento)
        {
            try
            {
                // TODO: Add insert logic here

                var db = new MateriaPrimaContext();
                db.Pesos.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("IndexMateria");
            }
            catch
            {
               
                return View();
            }
        }
        public ActionResult Edit(int id)
        {
            var elemento = db.Pesos.Single(m => m.idpeso == id);
            return View(elemento);
        }

        // POST: Almacen/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.Pesos.Single(m => m.idpeso == id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("IndexMateria");
                }
               
                return View();
            }
            catch (Exception er)
            {
             
                return View();
            }
        }
        public ActionResult Delete(int id)
        {
            var elemento = db.Pesos.Single(m => m.idpeso == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }

        // POST: Almacen/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.Pesos.Single(m => m.idpeso == id);
                db.Pesos.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("IndexMateria");

            }
            catch
            {
                return View();
            }
        }
        public ActionResult Details(int id)
        {
            var elemento = db.Pesos.Single(m => m.idpeso == id);
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

        // POST: 
        [HttpPost]
        public ActionResult Details(int id, Peso collection)
        {
            var elemento = db.Pesos.Single(m => m.idpeso == id);
            return View(elemento);
        }
    }
}