
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using Reportes;
using System;
using System.IO;
using System.Text;

using SIAAPI.Models.Inventarios;
using PagedList;
using SIAAPI.Models.Administracion;

namespace SIAAPI.Controllers.Inventarios
{
    [Authorize(Roles = "Administrador,Facturacion,Ventas,Sistemas,Almacenista,Comercial,Compras")]
    public class InventarioController : Controller
    {
        private InventarioContext db = new InventarioContext();
        private VInventarioInicialContext vdb = new VInventarioInicialContext();
        private AlmacenContext dba = new AlmacenContext();
        private c_ClaveUnidadContext dbu = new c_ClaveUnidadContext();
        private VArtCaracteristicaContext dbc = new VArtCaracteristicaContext();
        private ArticuloContext dbar = new ArticuloContext();

        public ActionResult Index(string sortOrder, string searchString, string currentFilter, string Almacen, string Producto, int? page, int? PageSize, Inventario inventario)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.AlmacenSortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "Almacen";
            ViewBag.ProductoSortParm = String.IsNullOrEmpty(sortOrder) ? "Producto" : "Producto";
            ViewBag.PresentacionSortParm = String.IsNullOrEmpty(sortOrder) ? "Presentacion" : "Presentacion";

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
            var elementos = from s in vdb.VInvIni
                            select s;

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Presentacion.Contains(searchString) || s.Producto.Contains(searchString) || s.Almacen.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Almacen":
                    elementos = elementos.OrderBy(s => s.Almacen);

                    break;
                case "Producto":
                    elementos = elementos.OrderBy(s => s.Producto);
                    break;
                case "Presentacion":
                    elementos = elementos.OrderBy(s => s.Presentacion);
                    break;

                default:
                    elementos = elementos.OrderBy(s => s.Almacen );
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE

            int count = vdb.VInvIni.OrderBy(e => e.IDInventario).Count();
            //int count = elementos.Count();
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


        // GET: Inventario/Details/5
        public ActionResult Details(int id)
        {
            var elemento = vdb.VInvIni.Single(m => m.IDInventario == id);
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

        // POST: Inventario/Details/5
        [HttpPost]
       
        public ActionResult Details(int id, FormCollection collection)
        {
            var elemento = vdb.VInvIni.Single(m => m.IDInventario == id);
            return View(elemento);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////7

        public ActionResult getJsonCaracteristicaArticulo(int id)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticuloconidpres(id);
            return Json(presentacion, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getPresentacionPorArticulo(int ida)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(ida);
            return presentacion;

        } 


        public ActionResult Create()
        {
            Inventario elemento = new Inventario();
             elemento = new Inventario();
            string FechInv = DateTime.Now.ToShortDateString();

            //Almacen
            var IDAlmacen = dba.Almacenes.OrderBy(i => i.Descripcion).ToList();
            List<SelectListItem> liaE = new List<SelectListItem>();
            liaE.Add(new SelectListItem { Text = "--Selecciona un Almacen--", Value = "0" });
            foreach (var a in IDAlmacen)
            {
                liaE.Add(new SelectListItem { Text = a.IDAlmacen + " | " + a.CodAlm + " | " + a.Descripcion, Value = a.IDAlmacen.ToString() });

            }
            ViewBag.IDAlmacen = liaE;

            //Articulos
            var datosArticulo = dbar.Articulo.OrderBy(i => i.Cref).ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "--Selecciona un Articulo--", Value = "0" });
            foreach (var a in datosArticulo)
            {
                liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion , Value = a.IDArticulo.ToString() });

            }
            ViewBag.IDArticulo = liAC;
            ViewBag.PresentacionList = getPresentacionPorArticulo(0);


           
            

            return View();
        }

        // POST: Inventario/Create
        [HttpPost]
   
        public ActionResult Create(Inventario elemento)
        {
            int idalm = elemento.IDAlmacen;
            int idinv = elemento.IDCaracteristica;
            decimal cant = elemento.Cantidad;
            int uni = int.Parse( elemento.IDClaveUnidad.ToString());

            //datos del alrticulo elegido
            Articulo articulo = new ArticuloContext().Articulo.Find(elemento.IDArticulo);
            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + elemento.IDCaracteristica).ToList().FirstOrDefault();
            //VArtCaracteristica artcaracList = dbc.Database.SqlQuery<VArtCaracteristica>("select ID,IDArticulo, IDCaracteristica, Cref, Descripcion, Presentacion, IDClaveUnidad, Unidad FROM dbo.VArtCaracteristica where IDCaracteristica = " + idinv + "").ToList().FirstOrDefault();
            //ViewBag.artcarac = artcaracList;
            //int idart = artcaracList.IDArticulo;
            //int idcar = artcaracList.IDCaracteristica;
            string fechaact = DateTime.Now.ToShortDateString();
            try
            {
                // TODO: Add insert logic here
                InventarioAlmacen invenalma = new InventarioAlmacenContext().InventarioAlmacenes.Where(s => s.IDCaracteristica == idinv && s.IDAlmacen == idalm).FirstOrDefault();

                if (invenalma == null)
                {
                    db.Database.ExecuteSqlCommand("Insert Into [InventarioAlmacen] (IDAlmacen,IDArticulo,IDCaracteristica,Existencia,porllegar,apartado,disponibilidad ) values(" + idalm + ", " + articulo.IDArticulo + ", " + cara.ID + ", " + cant + ",0,0," + cant + ")");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("update  [InventarioAlmacen] set Existencia=" +elemento.Cantidad +", Disponibilidad=" +elemento.Cantidad +", porllegar=0, apartado=0 where idCaracteristica="+ elemento.IDCaracteristica+ " and idalmacen="+elemento.IDAlmacen );

                }
                //  db.Database.ExecuteSqlCommand(" exec dbo.MovArt '" + convertirfechaamericana( fechaact.ToString())+"', " + elemento.IDCaracteristica + ", 'InvIni', " + cant + ", 'inventario Inicial', 0,'', "+ idalm + ", ' ',0");2
                try
                {
                    db.Database.ExecuteSqlCommand(" INSERT INTO dbo.MovimientoArticulo (Fecha, Id, IDPresentacion, Articulo_IDArticulo, Accion, cantidad, Documento, NoDocumento, Lote, IDAlmacen, TipoOperacion, acumulado, observacion, Hora) values (GETDATE(), " + elemento.IDCaracteristica + " , " + cara.IDPresentacion + "," + cara.Articulo_IDArticulo + ", 'Inventario Inicial'," + elemento.Cantidad + " ,'Inicial',0,''," + elemento.IDAlmacen + ", 'E'," + elemento.Cantidad + ",'', SYSDATETIMEOFFSET())");
                }
                catch(Exception err)
                {
                    string mensajeerror = err.Message;
                }
                return RedirectToAction("Index","InventarioAlmacen", new { searchString = articulo.Cref});
            }
            catch (Exception ex)
            {
                return View("Error", ex);

            }
        }
        /// <summary>
        /// funcion que convierte una fecha que esta en string dd/mm/YYYY en un string de tipo americano YYYY/mm/dd
        /// </summary>
        /// <param name="data">Es la fecha dd/mm/YYYY</param>
        /// <returns></returns>
        public string convertirfechaamericana(string data)
        {
            DateTime fecha = DateTime.Parse(data);
            string nuevafecha = fecha.Year + "/" + fecha.Month + "/" + fecha.Day;
            return nuevafecha;
        }

        // GET: /Delete/5

        public ActionResult Delete(int id)
        {
            var elemento = vdb.VInvIni.Single(m => m.IDInventario == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }


        // POST: a/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                string fechaact = DateTime.Now.ToShortDateString();
                var elemento = db.Inventarios.Single(m => m.IDInventario == id);
                int idalm = elemento.IDAlmacen;
                int idcar = elemento.IDCaracteristica;
                decimal cant = elemento.Cantidad;
                int uni = int.Parse(elemento.IDClaveUnidad.ToString());


                //db.Inventarios.Remove(elemento);
                //db.SaveChanges();
                db.Database.ExecuteSqlCommand("delete from dbo.inventario where IDInventario =" + id + "");
                db.Database.ExecuteSqlCommand(" exec dbo.MovArt '" + convertirfechaamericana(fechaact.ToString()) + "', " + idcar + ", 'AjuSal', " + cant + ", 'inventario Inicial', 0,'', " + idalm + ", 'Eliminado del Inventario Inicial ',0");
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
        /// <summary>
        /// Checa la unidad de medida del articulo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public JsonResult GetUnidad(int id) //checar la unidad del articulo
        {
           
            using (ArticuloContext db = new ArticuloContext())
            {
                string unidad = string.Empty;
                try
                {
                    unidad = db.Articulo.Find(id).c_ClaveUnidad.Nombre;
                    return Json(new { result = unidad }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                    return Json(new { result = "" }, JsonRequestBehavior.AllowGet);
                }
                   
               
            }
        }

        public JsonResult getarticulosblando(string buscar)
        {
            buscar = buscar.Remove(buscar.Length - 1);
            var Articulos = new ArticuloContext().Articulo.Where(s => s.Cref.Contains(buscar) ).OrderBy(S=>S.Cref);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Articulo art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Cref +" "+ art.Descripcion;
                elemento.Value = art.IDArticulo.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }

    }
}

