using PagedList;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;

using System.IO;
using SIAAPI.ViewModels.Comercial;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion")]
    public class MovimientosController : Controller
    {
        // GET: MovAutorizaciones
        private MovimientosContext db = new MovimientosContext();
        // GET: Almacen
        public ActionResult MovComercial(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "IDDestino" : "IDDestino";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "IDOrigen" : "IDOrigen";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.MovComerciales.Include(c => c.User)
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.DocumentoDestino.Contains(searchString)|| s.DocumentoOrigen.Contains(searchString) || s.Fecha.ToString().Contains(searchString) || s.User.Username.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "IDDestino":
                    elementos = elementos.OrderBy(s => s.IDDestino);
                    break;
                case "IDOrigen":
                    elementos = elementos.OrderBy(s => s.IDOrigen);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDMovimiento);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.MovComerciales.OrderByDescending(e => e.IDMovimiento).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        public ActionResult MovAutorizacion(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Documento" : "Documento";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDocumento" : "IDDocumento";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.MovAutorizaciones.Include(c => c.User)
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Documento.Contains(searchString) || s.IDDocumento.ToString().Contains(searchString) || s.Fecha.ToString().Contains(searchString) || s.User.Username.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Documento":
                    elementos = elementos.OrderBy(s => s.Documento);
                    break;
                case "IDDocumento":
                    elementos = elementos.OrderBy(s => s.IDDocumento);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDMovimientoA);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.MovAutorizaciones.OrderByDescending(e => e.IDMovimientoA).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }
     

      

       

    }
}