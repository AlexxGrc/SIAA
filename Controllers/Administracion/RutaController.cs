using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class RutaController : Controller
    {
        // GET: Ruta
        RutasContext db = new RutasContext();


       
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "";
          
         
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
            var elementos = from s in db.Rutas
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
              

                elementos = elementos.Where(s => s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.IDRuta);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Rutas.OrderBy(e => e.IDRuta).Count(); // Total number of elements

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
        //public ActionResult Index()
        //{
        //    var listaRutas = db.Rutas.ToList();
        //    return View(listaRutas);
        //}
       
        public ActionResult Create()
        {
            Ruta ruta = new Ruta();
            return View(ruta);
        }
       
        [HttpPost]
        public ActionResult Create(Ruta elementonuevo)
        {
            try
            {
                // TODO: Add insert logic here

                db.Rutas.Add(elementonuevo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
       
        public ActionResult Details(int id)
        {
            Ruta elemento = db.Rutas.Find(id);
            return View(elemento);
        }

       
        public ActionResult Edit(int id)
        {
            Ruta elemento = db.Rutas.Find(id);
            return View(elemento);
        }
      
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collectioo)
        {
            try
            {
                var elemento = db.Rutas.Single(m => m.IDRuta == id);
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
       
        public ActionResult Delete(int id)
        {
            try
            {
                var elemento = db.Rutas.Find(id);
                db.Rutas.Remove(elemento);
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