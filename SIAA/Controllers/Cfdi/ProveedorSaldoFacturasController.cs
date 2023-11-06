using PagedList;
using SIAAPI.Models.Cfdi;
using SIAAPI.ViewModels.Cfdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Cfdi
{
    public class ProveedorSaldoFacturasController : Controller
    {
        // GET: ClienteSaldoFacturas
        private ProveedorSaldoFacturasContext db = new ProveedorSaldoFacturasContext();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Proveedor" : "Proveedor";
            string ConsultaSql = "select S.ID, S.Proveedor, sum(TotalFacturado) as TotalFacturado,  sum(ImportePagado) as ImportePagado, sum(ImporteSaldoInsoluto) as ImporteSaldoInsoluto, Moneda from [dbo].[ProveedorSaldoFacturas] as S inner join Proveedores as P on S.ID = P.IDProveedor ";
            string Filtro = string.Empty;
            string Agrupar = "group by S.ID, S.Proveedor, S.Moneda";
            string Orden = " order by S.Proveedor asc";

            //Buscar por nombre
            if (!String.IsNullOrEmpty(searchString))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = " where S.Proveedor like '%" + searchString + "%'";
                }
                else
                {
                    Filtro += " and S.Proveedor like '%" + searchString + "%'";
                }

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
            //var elementos = from s in db.ClienteSaldoFacturas
            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Agrupar + " " + Orden;

            var elementos = db.Database.SqlQuery<ProveedorSaldoFacturas>(cadenaSQl).ToList();
            string ConsultaSqlResumen = "select Moneda, sum(TotalFacturado) as TotalFacturado,  sum(ImportePagado) as ImportePagado, sum(ImporteSaldoInsoluto) as ImporteSaldoInsoluto from [dbo].[ProveedorSaldoFacturas] as S inner join Proveedores as P on S.ID = P.IDProveedor ";
            string ConsultaAgrupado = "group by Moneda order by Moneda ";
            ViewBag.sumatoria = "";
            try
            {

                var SumaLst = new List<string>();
                var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
                List<ResumenSaldos> data = db.Database.SqlQuery<ResumenSaldos>(SumaQry).ToList();
                ViewBag.sumatoria = data;

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {

                //elementos = elementos.Where(s => s.NombreCliente.ToUpper().Contains(searchString.ToUpper()));

            }

            //Ordenacion
           
            switch (sortOrder)
            {
                case "Proveedor":
                    Orden = " order by Proveedor asc";
                    break;
                
                default:
                    Orden = " order by Proveedor asc";
                    break;
            }

            //Paginación
            
            int count = db.ProveedorSaldoFacturas.OrderBy(e => e.Proveedor).Count(); // Total number of elements

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

            //return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }  
    }
}
