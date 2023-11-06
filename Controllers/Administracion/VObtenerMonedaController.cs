using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using PagedList;
using System.Data.SqlClient;

using System.IO;
using SIAAPI.ViewModels.Comercial;

namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion")]
    public class VObtenerMonedaController : Controller
    {
            private VObtenerMonedaContext db = new VObtenerMonedaContext();
            private TipoCambioContext dbg = new TipoCambioContext();
     
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.FechaTCSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
                ViewBag.MonedaOrigenSortParm = String.IsNullOrEmpty(sortOrder) ? "MonedaOrigen" : "";
                ViewBag.MonedaDestinoSortParm = String.IsNullOrEmpty(sortOrder) ? "MonedaDestino" : "";
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
                var elementos = from s in db.VObtenerMonedas
                                select s;
                //Busqueda
                if (!string.IsNullOrEmpty(searchString))
                {

                    elementos = elementos.Where(s => s.FechaTC.ToString().Contains(searchString) || s.MonedaOrigen.Contains(searchString) || s.MonedaDestino.Contains(searchString)).OrderByDescending(s => s.FechaTC);

                }

                //Ordenacion

                switch (sortOrder)
                {
                    case "Date_desc":
                        elementos = elementos.OrderByDescending(s => s.FechaTC);
                        break;
                    case "MonedaOrigen":
                        elementos = elementos.OrderBy(s => s.MonedaOrigen);
                        break;
                    case "MonedaDestino":
                        elementos = elementos.OrderBy(s => s.MonedaDestino);
                        break;
                    default:
                        elementos = elementos.OrderByDescending(s => s.FechaTC);
                        break;
                }

                //Paginación
                // DROPDOWNLIST FOR UPDATING PAGE SIZE
                int count = db.VObtenerMonedas.OrderBy(e => e.IDTipoCambio).Count(); // Total number of elements

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


        public ActionResult Details(int id)
            {
                var elemento = db.VObtenerMonedas.Single(m => m.IDTipoCambio == id);
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



        // GET: VObtenerMonedas/Edit/5
      
        public ActionResult Edit(int? id)
            {
                var monedas = new MonedaRepository().GetMoneda();
                ViewBag.Moneda = monedas;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                VObtenerMoneda vObtenerMoneda = db.VObtenerMonedas.Find(id);
                if (vObtenerMoneda == null)
                {
                    return HttpNotFound();
                }
                return View(vObtenerMoneda);
            }

        // POST: VObtenerMonedas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
     
        [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "IDTipoCambio,FechaTC,IDMonedaOrigen,MonedaOrigen,IDMonedaDestino,MonedaDestino,TC")] VObtenerMoneda vObtenerMoneda)
            {
                    //db.Entry(vObtenerMoneda).State = System.Data.Entity.EntityState.Modified;
                    try
                    {


                        dbg.Database.ExecuteSqlCommand("update TipoCambio set FechaTC= '" + vObtenerMoneda.FechaTC.Value.Year + "-" + vObtenerMoneda.FechaTC.Value.Month + "-" + vObtenerMoneda.FechaTC.Value.Day + "', IDMonedaOrigen = '" + vObtenerMoneda.IDMonedaOrigen + "', IDMonedaDestino = '" + vObtenerMoneda.IDMonedaDestino + "', TC= " + vObtenerMoneda.TC + " where IDTipoCambio = " + vObtenerMoneda.IDTipoCambio + "");
                        //dbg.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (SqlException err)
                    {
                        var monedas = new MonedaRepository().GetMoneda();
                        ViewBag.Mensaje = err.Message;
                        ViewBag.Moneda = monedas;
                        return View();
                    }
                    catch (Exception err)
                    {
                        var monedas = new MonedaRepository().GetMoneda();
                        ViewBag.Mensaje = err.Message;
                        ViewBag.Moneda = monedas;
                        return View();
                    }
                

                return View(vObtenerMoneda);
            }

        // GET: VObtenerMonedas/Delete/5
       
        public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                VObtenerMoneda vObtenerMoneda = db.VObtenerMonedas.Find(id);
                if (vObtenerMoneda == null)
                {
                    return HttpNotFound();
                }
                return View(vObtenerMoneda);
            }

        // POST: VObtenerMonedas/Delete/5
    
        [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                //VObtenerMoneda vObtenerMoneda = db.VObtenerMonedas.Find(id);
                //db.VObtenerMonedas.Remove(vObtenerMoneda);
                try
                {
                    dbg.Database.ExecuteSqlCommand("delete from TipoCambio where IDTipoCambio = " + id + "");
                    //dbg.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (SqlException err)
                {
                    var monedas = new MonedaRepository().GetMoneda();
                    ViewBag.Mensaje = err.Message;
                    ViewBag.Moneda = monedas;
                    return View();
                }
                catch (Exception err)
                {
                    var monedas = new MonedaRepository().GetMoneda();
                    ViewBag.Mensaje = err.Message;
                    ViewBag.Moneda = monedas;
                    return View();
                }
            }
       
        protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }


       

        public ActionResult CreaReporteporfecha()
        {
            return View();
        }
       
     




    }
}