using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using SIAAPI.Models.Produccion;

namespace SIAAPI.Controllers.Produccion
{
    public class Cambio_EstadosController : Controller
    {
        // GET: Cambio_Estados
        private CambioEstadoContext db = new CambioEstadoContext();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;

            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "Responsable" : "Responsable";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.CambioEstados
                            select s;
            ////Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.IDOrden.ToString().Contains(searchString) );
            }
            //Ordenacion

            switch (sortOrder)
            {
                //case "Orden":
                //    elementos = elementos.OrderBy(s => s.IDOrden);
                //    break;
                case "Fecha":
                    elementos = elementos.OrderByDescending(s => s.Fecha);
                    break;
                //case "Hora":
                //    elementos = elementos.OrderBy(s => s.Hora);
                //    break;
                case "Motivo":
                    elementos = elementos.OrderBy(s => s.motivo);
                    break;
                case "Usuario":
                    elementos = elementos.OrderBy(s => s.Usuario);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.Fecha);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.CambioEstados.OrderByDescending(e => e.Fecha).Count(); // Total number of elements

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
        }           
    }
}