using CrystalDecisions.CrystalReports.Engine;
using PagedList;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_PeriocidadPagoController : Controller
    {
        private c_PeriodicidadPagoContext db = new c_PeriodicidadPagoContext();
        
        // GET: c_PeriocidadPago
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.C1SortParm = String.IsNullOrEmpty(sortOrder) ? "ClavePeriocidad" : "ClavePeriocidad";
            ViewBag.C2SortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";

            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;

            //Paginación
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //Paginación
            var elementos = from s in db.c_PeriocidadPagos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                 elementos = elementos.Where(s => s.ClavePeriocidad.Contains(searchString) || s.Descripcion.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClavePeriocidad":
                    elementos = elementos.OrderBy(s => s.ClavePeriocidad);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClavePeriocidad);

                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_PeriocidadPagos.OrderBy(e => e.IDPeriocidadPago).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        // GET: c_PeriocidadPago/Details/5
        public ActionResult Details(int id)
        {
            var elemento = db.c_PeriocidadPagos.Single(m => m.IDPeriocidadPago == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

   

    // GET: c_PeriocidadPago/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_PeriocidadPago/Create
        [HttpPost]
        public ActionResult Create(c_PeriodicidadPago elemento)
        {
        try
        {
                // TODO: Add insert logic here

            var db = new c_PeriodicidadPagoContext();
            db.c_PeriocidadPagos.Add(elemento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        catch
        {
            return View();
        }
    }

        // GET: c_PeriocidadPago/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.c_PeriocidadPagos.Single(m => m.IDPeriocidadPago == id);
            return View(elemento);
        }

        // POST: c_PeriocidadPago/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                var elemento = db.c_PeriocidadPagos.Single(m => m.IDPeriocidadPago == id);
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

        // GET: c_PeriocidadPago/Delete/5
        public ActionResult Delete(int id)
        {
            var elemento = db.c_PeriocidadPagos.Single(m => m.IDPeriocidadPago == id);
            return View(elemento);
        }

        // POST: c_PeriocidadPago/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var db = new c_PeriodicidadPagoContext();
                var elemento = db.c_PeriocidadPagos.Single(m => m.IDPeriocidadPago == id);
                db.c_PeriocidadPagos.Remove(elemento);
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