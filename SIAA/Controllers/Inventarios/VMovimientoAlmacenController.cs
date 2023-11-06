using PagedList;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Inventarios;
using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Inventarios
{
    public class VMovimientoAlmacenController : Controller
    {
        public VMovimientoAlmacenContext db = new VMovimientoAlmacenContext();

        // GET: VMovimientoAlmacen

        public ActionResult Index(string AlmacenES, string TipoOpe, string AccionES, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString, DateTime? FechaI, DateTime? FechaF , int? IDArticulo, int? IDCaracteristica)
        {

            //Buscar SearchString
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ReferenciaSortParm = String.IsNullOrEmpty(sortOrder) ? "Referencia" : "Referencia";
            ViewBag.ProductoSortParm = String.IsNullOrEmpty(sortOrder) ? "Producto" : "Producto";
            ViewBag.PresentacionSortParm = String.IsNullOrEmpty(sortOrder) ? "Presentacion" : "Presentacion";
            ViewBag.LoteSortParm = String.IsNullOrEmpty(sortOrder) ? "Lote" : "Lote";
            ViewBag.TipoOperacionSortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "Almacen";
            ViewBag.AccionSortParm = String.IsNullOrEmpty(sortOrder) ? "Accion" : "Accion";
            ViewBag.AlmacenSortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "Almacen";

            //Buscar por periodo de tiempo
                     

            var PeriodoTiempoQry = from s in db.VMovimientoAlmacenes
                                   where s.FechaMovimiento >= FechaI
                                   where s.FechaMovimiento <= FechaF
                             select s;
            ViewBag.PeriodoTiempo = PeriodoTiempoQry;

            //Buscar TipoOperacion
            var TipoOpeLst = new List<string>();
            var TipoOpeQry = from d in db.VMovimientoAlmacenes
                         orderby d.TipoOperacion
                             select d.TipoOperacion;

            TipoOpeLst.AddRange(TipoOpeQry.Distinct());
            ViewBag.TipoOpe = new SelectList(TipoOpeLst);


            //Buscar Accion
            var AccionESLst = new List<string>();
            var AccionESQry = from d in db.VMovimientoAlmacenes
                             orderby d.Accion
                              select d.Accion;

            AccionESLst.AddRange(AccionESQry.Distinct());
            ViewBag.AccionES = new SelectList(AccionESLst);

            //Buscar Almacen
            var AlmacenESLst = new List<string>();
            var AlmacenESQry = from d in db.VMovimientoAlmacenes
                              orderby d.Almacen
                              select d.Almacen;

            AlmacenESLst.AddRange(AlmacenESQry.Distinct());
            ViewBag.AlmacenES = new SelectList(AlmacenESLst);

            //if (searchString == null)
            //{
            //    searchString = currentFilter;
            //}

            var elementos = from s in db.VMovimientoAlmacenes
                            .OrderByDescending(s => s.FechaMovimiento).OrderByDescending(s => s.Hora)
                            select s;
           
            /////Busqueda
            ///tabla filtro: searchString

            if (!string.IsNullOrEmpty(searchString))
            {

                elementos = elementos.Where(s => s.Referencia.Contains(searchString) || s.Producto.Contains(searchString) || s.Presentacion.Contains(searchString) || s.Lote.Contains(searchString)).OrderByDescending(s => s.FechaMovimiento).OrderByDescending(s => s.Hora);

            }
            ///tabla filtro: Accion
            if (!String.IsNullOrEmpty(AccionES))
            {
                elementos = elementos.Where(s => s.Accion == AccionES).OrderByDescending(s => s.FechaMovimiento).OrderByDescending(s => s.Hora);

            }
            ///tabla filtro: Tipo de operación
            if (!String.IsNullOrEmpty(TipoOpe))
            {
                elementos = elementos.Where(s => s.TipoOperacion == TipoOpe).OrderByDescending(s => s.FechaMovimiento).OrderByDescending(s => s.Hora);

            }
            ///tabla filtro: Almacen
            if (!String.IsNullOrEmpty(AlmacenES))
            {
                elementos = elementos.Where(s => s.Almacen == AlmacenES).OrderByDescending(s => s.FechaMovimiento).OrderByDescending(s => s.Hora);

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
            //Ordenacion

            switch (sortOrder)
            {
                case "Referencia":
                    elementos = elementos.OrderBy(s => s.Referencia);
                    break;
                case "Producto":
                    elementos = elementos.OrderBy(s => s.Producto);
                    break;
                case "Presentacion":
                    elementos = elementos.OrderBy(s => s.Presentacion);
                    break;
                case "Lote":
                    elementos = elementos.OrderBy(s => s.Lote);
                    break;
                case "Accion":
                    elementos = elementos.OrderBy(s => s.Accion);
                    break;
                case "TipoOperacion":
                    elementos = elementos.OrderBy(s => s.TipoOperacion);
                    break;
                case "Almacen":
                    elementos = elementos.OrderBy(s => s.Almacen);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.FechaMovimiento.ToString());
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.VMovimientoAlmacenes.Count(); // Total number of elements

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


        public ActionResult Kardex(int IDAlmacen, int IDArticulo, int IDCaracteristica, DateTime? FechaI, DateTime? FechaF, int? page, int? PageSize)
        {
            List<VMovimientoAlmacen> elementoskardex;

            Articulo articulo = new ArticuloContext().Articulo.Find(IDArticulo);

            ViewBag.Articulo = articulo.Descripcion;

            Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + IDCaracteristica).FirstOrDefault();

            ViewBag.IDArticulo = caracteristica.Articulo_IDArticulo;
            ViewBag.IDCaracteristica = caracteristica.ID;
            ViewBag.IDAlmacen = IDAlmacen;
            ViewBag.Caracteristica = caracteristica.Presentacion;
            ViewBag.IDPresentacion = caracteristica.IDPresentacion;
            if (articulo.IDTipoArticulo == 6)
            {
                ViewBag.escinta = "SI";
            }
            else if (articulo.IDTipoArticulo == 7)
            {
                ViewBag.estinta = "SI";
            }
            else
            {
                ViewBag.escinta = "NO";
                ViewBag.estinta = "NO";
            }
            //if ((FechaI == null) || (FechaF == null))
            //    {
            string cadena1 = "select * from VMovimientoAlmacen where IDArticulo=" + IDArticulo + " and IDCaracteristica=" + IDCaracteristica + " and IDAlmacen=" + IDAlmacen + " order by fechaMovimiento desc,hora desc";
            elementoskardex = new VMovimientoAlmacenContext().Database.SqlQuery<VMovimientoAlmacen>(cadena1).ToList();
            //}
            //else
            //{
            //    string cadena2 = "select * from VMovimientoAlmacen where IDArticulo=" + IDArticulo + " and IDCaracteristica=" + IDCaracteristica + " and IDAlmacen=" + IDAlmacen + " and FechaMovimiento >='" + FechaI.Value.Year + "/" + FechaI.Value.Month + "/" + FechaI.Value.Day + "' and FechaMovimiento<='" + FechaF.Value.Year + "/" + FechaF.Value.Month + "/" + FechaF.Value.Day + "' order by fechamovimiento desc, hora desc";
            //     elementoskardex = new VMovimientoAlmacenContext().Database.SqlQuery<VMovimientoAlmacen>(cadena2).ToList();
            //}


            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = elementoskardex.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25", Selected = true },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;
            ViewBag.IDArticulo = IDArticulo;
            ViewBag.IDAlmacen = IDAlmacen;
            ViewBag.IDCaracteristica = IDCaracteristica;

            //return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementoskardex.ToPagedList(pageNumber, pageSize));
            //Paginación

        }


        public ActionResult Lotes( int IDCaracteristica, int IDArticulo, int IDAlmacen)
        {
            List<Clslotemp> lotes = new ClslotempContext().materiaP.Where(s => s.IDArticulo == IDArticulo && s.IDCaracteristica == IDCaracteristica && s.IDAlmacen == IDAlmacen && s.MetrosDisponibles>0).ToList();
            return View(lotes);
        }
        
        [HttpPost]
        public ActionResult DeleteLote(FormCollection colleccion)
        {
            int id = int.Parse(colleccion.Get("id").ToString());
            string motivo = colleccion.Get("Observacion").ToString();
          

            Clslotemp lote = new ClslotempContext().materiaP.Find(id);

            AjustesAlmacen elemento = new AjustesAlmacen();
            elemento.FechaAjuste = DateTime.Now;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            string fecha2 = DateTime.Now.ToString("yyyy/MM/dd");

            try
            {

                ClsDatoDecimal exiS = db.Database.SqlQuery<ClsDatoDecimal>("select (Existencia - " + lote.MetrosDisponibles + ") as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + lote.IDAlmacen + " and IDArticulo = " + lote.IDArticulo + " and IDCaracteristica = " + lote.IDCaracteristica + "").ToList()[0];

                Caracteristica cara1 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + lote.IDCaracteristica).FirstOrDefault();
                //Grava el movimiento de Entrada del Almacen
                string comandos = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + lote.IDCaracteristica + "," + cara1.IDPresentacion + "," + cara1.Articulo_IDArticulo + ",'Ajuste Inventario', " + lote.MetrosDisponibles + ", 'Ajuste Inventario',0,'" + lote.LoteInterno + "', " + lote.IDAlmacen + ", 'S', " + exiS.Dato + ", '" + motivo + "', SYSDATETIMEOFFSET ( ))";
                db.Database.ExecuteSqlCommand(comandos);

                //Actualiza la existencia del articulo que entro al almacen
                string comandos2 = "update dbo.InventarioAlmacen set Existencia= " + exiS.Dato + " where [IDAlmacen]=" + lote.IDAlmacen + "  and [IDCaracteristica]= " + lote.IDCaracteristica + "";
                db.Database.ExecuteSqlCommand(comandos2);
                //Actualiza articulos disponibles
                ClsDatoDecimal disp = db.Database.SqlQuery<ClsDatoDecimal>("select (Existencia-Apartado) as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + lote.IDAlmacen + " and IDArticulo = " + lote.IDArticulo + " and IDCaracteristica = " + lote.IDCaracteristica + "").FirstOrDefault();
                string comandos3 = "update dbo.InventarioAlmacen set Disponibilidad= " + disp.Dato + " where [IDAlmacen]=" + lote.IDAlmacen + "  and [IDCaracteristica]= " + lote.IDCaracteristica+ "";
                db.Database.ExecuteSqlCommand(comandos3);
                string comandos4 = "delete from clslotemp where id="+id;
                db.Database.ExecuteSqlCommand(comandos4);

            }
            catch(Exception err)
            {
                return RedirectToAction("InventarioAlmacen");
            }
            return RedirectToAction("kardex","VMovimientoAlmacen", new { IDCaracteristica = lote.IDCaracteristica, IDArticulo = lote.IDArticulo, IDAlmacen = lote.IDAlmacen });
        }

        public ActionResult LotesTinta(int IDCaracteristica, int IDArticulo, int IDAlmacen)
        {
            List<Clslotetinta> lotes = new ClslotetintaContext().Tintas.Where(s => s.idarticulo == IDArticulo && s.idcaracteristica == IDCaracteristica && s.IDAlmacen == IDAlmacen && s.Estado =="Existe").ToList();
            return View(lotes);
        }
        [HttpPost]
        public ActionResult DeleteLoteTinta(FormCollection colleccion)
        {
            int id = int.Parse(colleccion.Get("id").ToString());
            string motivo = colleccion.Get("Observacion").ToString();


            Clslotetinta lote = new ClslotetintaContext().Tintas.Find(id);

            AjustesAlmacen elemento = new AjustesAlmacen();
            elemento.FechaAjuste = DateTime.Now;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            string fecha2 = DateTime.Now.ToString("yyyy/MM/dd");

            try
            {

                ClsDatoDecimal exiS = db.Database.SqlQuery<ClsDatoDecimal>("select (Existencia - " + lote.cantidad + ") as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + lote.IDAlmacen + " and IDArticulo = " + lote.idarticulo + " and IDCaracteristica = " + lote.idcaracteristica + "").ToList().FirstOrDefault();

                Caracteristica cara1 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + lote.idcaracteristica).FirstOrDefault();
                //Grava el movimiento de Entrada del Almacen
                string comandos = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + lote.idcaracteristica + "," + cara1.IDPresentacion + "," + cara1.Articulo_IDArticulo + ",'Ajuste Inventario', " + lote.cantidad + ", 'Ajuste Inventario',0,'" + lote.lote + "', " + lote.IDAlmacen + ", 'S', " + exiS.Dato + ", '" + motivo + "', SYSDATETIMEOFFSET ( ))";
                db.Database.ExecuteSqlCommand(comandos);

                //Actualiza la existencia del articulo que entro al almacen
                string comandos2 = "update dbo.InventarioAlmacen set Existencia= " + exiS.Dato + " where [IDAlmacen]=" + lote.IDAlmacen + "  and [IDCaracteristica]= " + lote.idcaracteristica + "";
                db.Database.ExecuteSqlCommand(comandos2);
                //Actualiza articulos disponibles
                ClsDatoDecimal disp = db.Database.SqlQuery<ClsDatoDecimal>("select (Existencia-Apartado) as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + lote.IDAlmacen + " and IDArticulo = " + lote.idarticulo + " and IDCaracteristica = " + lote.idcaracteristica + "").FirstOrDefault();
                string comandos3 = "update dbo.InventarioAlmacen set Disponibilidad= " + disp.Dato + " where [IDAlmacen]=" + lote.IDAlmacen + "  and [IDCaracteristica]= " + lote.idcaracteristica + "";
                db.Database.ExecuteSqlCommand(comandos3);
                string comandos4 = "delete from clslotetinta where id=" + id;
                db.Database.ExecuteSqlCommand(comandos4);

            }
            catch (Exception err)
            {
                return RedirectToAction("InventarioAlmacen");
            }
            return RedirectToAction("kardex", "VMovimientoAlmacen", new { IDCaracteristica = lote.idcaracteristica, IDArticulo = lote.idarticulo, IDAlmacen = lote.IDAlmacen });
        }

    }
}
