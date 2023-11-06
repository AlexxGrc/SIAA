using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SIAAPI.Models.Inventarios;
using PagedList;
using SIAAPI.Models.Comercial;
using System.Data.SqlClient;
using System.Globalization;

namespace SIAAPI.Controllers.Inventarios
{
    public class VArticuloAlmacenController : Controller
    {
        public VArticuloAlmacenContext db = new VArticuloAlmacenContext();
        public AlmacenContext dba = new AlmacenContext();

        // GET: VArticuloAlmacen


        //[Authorize(Roles = "Administrador")]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //return View(db.VPArticuloAlmacenes.ToList());
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ReferenciaSortParm = String.IsNullOrEmpty(sortOrder) ? "Referencia" : "Referencia";
            ViewBag.ProductoSortParm = String.IsNullOrEmpty(sortOrder) ? "Producto" : "Producto";
            ViewBag.AlmacenSortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "Almacen";
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
            var elementos = from s in db.VArticuloAlmacenes
                            select s;

        //   var elementos = db.Database.SqlQuery<VArticuloAlmacen>("select * from VArticuloAlmacen").ToList().AsEnumerable();
            //espera 

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Referencia.Contains(searchString) || s.Producto.Contains(searchString) || s.Almacen.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Referencia":
                    elementos = elementos.OrderBy(s => s.Referencia);

                    break;
                case "Producto":
                    elementos = elementos.OrderBy(s => s.Producto);
                    break;
                case "Almacen":
                    elementos = elementos.OrderBy(s => s.Almacen);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Referencia);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE

            //int count = db.VPArticuloAlmacenes.Count();
            int count = elementos.Count();
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
        }

        public ActionResult Index2()
        {
            try
            {
                ////var elementos = db.VArticuloAlmacenes.ToList();
                ////ViewBag.listEle = elementos;
                var elementos = db.Database.SqlQuery<VArticuloAlmacen>("select * from VArticuloAlmacen");
                return View(elementos);
            }
             
            catch (Exception e)
            {
                ViewData["msg"] = e.Message;
                return View("Error");
    }
          

}

        // GET: TraspasoOrder 
        public ActionResult NuevoTraspaso(String id)
    {
            VArticuloAlmacen x = db.VArticuloAlmacenes.Find(id);

            // se me hace que la llave primaria esta mal haber la base muestrame el sqlmanagament

            TraspasoOrder ordentraspaso = new TraspasoOrder();

            ordentraspaso.IDCaracteristica = x.IDCaracteristica;
            ordentraspaso.Referencia = x.Referencia;

            ordentraspaso.Producto = x.Producto;

            ordentraspaso.Presentacion = x.Presentacion;
            ordentraspaso.Existencia = x.Existencia;

            ordentraspaso.IDAlmacenSalida = x.IDAlmacenSalida;  
            ordentraspaso.Almacen = x.Almacen;
            ordentraspaso.FechaOrden = DateTime.Now;
            //ViewData["FechaOrden"] = DateTime.Now.ToString();

            var list = new AlmacenRepository().GetAlmacenes();
            ViewBag.AlmacenDestino = list;
            //if (list == null)
            //{

            //}


            // ViewBag.AlmacenDestino = new SelectList(list, "IDAlmacen", "Descripcion");

            ViewBag.Mensaje = "";
            return View(ordentraspaso);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult NuevoTraspaso(string id, TraspasoOrder ordentraspaso)
    {
            VArticuloAlmacen x = db.VArticuloAlmacenes.Find(id);

            //TraspasoOrder ordentraspaso = new TraspasoOrder();
            ordentraspaso.IDCaracteristica = x.IDCaracteristica;
            ordentraspaso.Referencia = x.Referencia;

            ordentraspaso.Producto = x.Producto;

            ordentraspaso.Presentacion = x.Presentacion;
            ordentraspaso.Existencia = x.Existencia;

            ordentraspaso.IDAlmacenSalida = x.IDAlmacenSalida;
            ordentraspaso.Almacen = x.Almacen;
            DateTime fecha = ordentraspaso.FechaOrden;
            string fecha1 = fecha.ToString("yyyy/MM/dd");

            var list1 = new AlmacenRepository().GetAlmacenes();
            ViewBag.AlmacenDestino = list1;

            if (x.Existencia >= ordentraspaso.CantidadTraspaso)
            {

                try
                {
                    db.Database.ExecuteSqlCommand("exec MovEntAlm '" +  fecha1 + "', " + ordentraspaso.IDCaracteristica + ", 'MovEntAlm', " + ordentraspaso.CantidadTraspaso +" , '" + ordentraspaso.Lote +"', "+ ordentraspaso.IDAlmacenSalida+", "+ ordentraspaso.AlmacenDestino +", '" + ordentraspaso.Observacion +"' ");
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (SqlException err)
                {
                    var list = new AlmacenRepository().GetAlmacenes();
                    ViewBag.AlmacenDestino = list;
                    return View(ordentraspaso);
                }
            }
           else
            {
                TempData["message"] = "Verifique la cantidad del traspaso sea MENOR a la existencia de este almacen";
                var list = new AlmacenRepository().GetAlmacenes();
                ViewBag.AlmacenDestino = list;
                return View(ordentraspaso);
            }
        }


      
    }
}

