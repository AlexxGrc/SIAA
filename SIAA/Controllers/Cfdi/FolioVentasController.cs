using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Cfdi;
using PagedList;

namespace SIAAPI.Controllers.Cfdi
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion")]
    public class FolioVentasController : Controller
    {
     
            private FolioVentasContext db = new FolioVentasContext();

            public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
            {
                //var lista = from e in db.c_Grupos
                //            orderby e.IDGrupo
                //            select e;
                //return View(lista);
                ViewBag.CurrentSort = sortOrder;
                ViewBag.UnoSortParm = String.IsNullOrEmpty(sortOrder) ? "Folio" : "Folio";
                ViewBag.DosSortParm = String.IsNullOrEmpty(sortOrder) ? "Serie" : "Serie";
                ViewBag.TresSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";

                if (searchString == null)
                {
                    searchString = currentFilter;
                }

                // Pass filtering string to view in order to maintain filtering when paging
                ViewBag.SearchString = searchString;
                //Paginación
                var elementos = from s in db.FoliosV.Include(f => f.c_TipoComprobante)
                                select s;
                //Busqueda
                if (!string.IsNullOrEmpty(searchString))
                {
                    elementos = elementos.Where(s => s.Serie.Contains(searchString) || s.c_TipoComprobante.Descripcion.Contains(searchString) || s.Numero.ToString().Contains(searchString));
                }

                //Ordenacion

                switch (sortOrder)
                {
                    case "Folio":
                        elementos = elementos.OrderBy(s => s.Numero);
                        break;
                    case "Serie":
                        elementos = elementos.OrderBy(s => s.Serie);
                        break;
                    case "Descripcion":
                        elementos = elementos.OrderBy(s => s.IDTipoComprobante);
                        break;
                    default:
                        elementos = elementos.OrderBy(s => s.IDFolioVentas);
                        break;
                }

                //Paginación
                // DROPDOWNLIST FOR UPDATING PAGE SIZE
                int count = db.FoliosV.OrderBy(e => e.IDFolioVentas).Count(); // Total number of elements

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
            // GET: Folio
            //public ActionResult Index()
            //{
            //    var folios = db.Folios.Include(f => f.c_TipoComprobante);
            //    return View(folios.ToList());
            //}

            // GET: Folio/Details/5
            public ActionResult Details(int? id)
            {
                
                FolioVentas folio = db.FoliosV.Find(id);
               
                return View(folio);
            }

            // GET: Folio/Create
            public ActionResult Create()
            {
                ViewBag.IDTipoComprobante = new SelectList(db.c_TipoComprobantes, "IDTipoComprobante", "Descripcion");
                return View();
            }

            // POST: Folio/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create(FolioVentas folio)
            {
                try
                {

                    db.FoliosV.Add(folio);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception err)
                {

                    ViewBag.IDTipoComprobante = new SelectList(db.c_TipoComprobantes, "IDTipoComprobante", "Descripcion", folio.IDTipoComprobante);

                    return View();
                }


            }

            // GET: Folio/Edit/5
            public ActionResult Edit(int? id)
            {
              
            FolioVentas folio = db.FoliosV.Find(id);
             
                ViewBag.IDTipoComprobante = new SelectList(db.c_TipoComprobantes, "IDTipoComprobante", "Descripcion", folio.IDTipoComprobante);
                return View(folio);
            }

            // POST: Folio/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit(FolioVentas folio)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(folio).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.IDTipoComprobante = new SelectList(db.c_TipoComprobantes, "IDTipoComprobante", "Descripcion", folio.IDTipoComprobante);
                return View(folio);
            }

            // GET: Folio/Delete/5
            public ActionResult Delete(int? id)
            {

            FolioVentas folio = db.FoliosV.Find(id);
            
            
                return View(folio);
            }

            // POST: Folio/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
            FolioVentas folio = db.FoliosV.Find(id);
                db.FoliosV.Remove(folio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
}