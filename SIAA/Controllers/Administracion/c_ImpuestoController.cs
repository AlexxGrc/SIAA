using System.Data;
using System.Linq;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using PagedList;

using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Controllers
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_ImpuestoController : Controller
    {
        private c_ImpuestoContext db = new c_ImpuestoContext();

        // GET: c_impuesto
     
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {

            //var Impuestos = from e in db.c_Impuestos
            //                orderby e.IDImpuesto
            //                select e;
            //return View(Impuestos);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveFormaPago" : "ClaveFormaPago";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.c_Impuestos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ClaveImpuesto.Contains(searchString) || s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveImpuesto":
                    elementos = elementos.OrderBy(s => s.ClaveImpuesto);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveImpuesto);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_Impuestos.OrderBy(e => e.IDImpuesto).Count(); // Total number of elements

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



        // GET: c_impuesto/Create
       public ActionResult Create()
        {

            var listatipo = new tipoimpuestoRepository().GetTipos();
            ViewBag.listatipo = listatipo;
            return View();
        }


        // POST: c_impuesto/Create
       
        [HttpPost]

        public ActionResult Create(c_Impuesto elemento)
        {
            try
            {
                db.c_Impuestos.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception er)
            {
                string mensajederror = er.Message;
                var listatipo = new tipoimpuestoRepository().GetTipos();
                ViewBag.listatipo = listatipo;
                return View();
            }
        }

        // GET: c_impuesto/Edit/5
       
        public ActionResult Edit(int id)
        {
            var listatipo = new tipoimpuestoRepository().GetTipos();
            ViewBag.listatipo = listatipo;
            var impuesto = db.c_Impuestos.Single(m => m.IDImpuesto == id);
            return View(impuesto);
        }

        // POST: c_impuesto/Edit/5
      
        [HttpPost]
        
        public ActionResult Edit(int id, FormCollection collection)
        {
           try
            {
                // TODO: Add update logic here
                var elemento = db.c_Impuestos.Single(m => m.IDImpuesto == id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                var listatipo = new tipoimpuestoRepository().GetTipos();
                ViewBag.listatipo = listatipo;
                return View(elemento);
            }
            catch(Exception er)
            {
              
                return View();
            }
        }

        // GET: c_impuesto/Details/5
       public ActionResult Details(int id)
        {
            var impuesto = db.c_Impuestos.Single(m => m.IDImpuesto == id);
            return View(impuesto);
        }

      
        [HttpPost]
        public ActionResult Details(int id, FormCollection collection)
        {
            var impuesto = db.c_Impuestos.Single(m => m.IDImpuesto == id);
            return View(impuesto);
        }
        // GET: c_Impuesto/Delete/5
       
        public ActionResult Delete(int id)
        {
            var impuesto = db.c_Impuestos.Single(m => m.IDImpuesto == id);
            return View(impuesto);
         
        }

        // POST: c_Impuesto/Delete/5
       
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                var elemento = (from e in db.c_Impuestos
                                where e.IDImpuesto == id
                                select e).FirstOrDefault();
                db.c_Impuestos.Remove(elemento);
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

