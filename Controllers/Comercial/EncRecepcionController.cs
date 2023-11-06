using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.ViewModels.Comercial;
using SIAAPI.ViewModels.Articulo;
using System.Web;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Cfdi;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using SIAAPI.Models.Inventarios;
using SIAAPI.Reportes;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Globalization;
using SIAAPI.Models.Produccion;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Almacenista,Administrador,Compras,Sistemas,Facturacion, Logistica,Calidad, GestionCalidad")]
    public class EncRecepcionController : Controller
    {
        private OrdenCompraContext db = new OrdenCompraContext();
        RecepcionContext BD = new RecepcionContext();
        ProveedorContext prov = new ProveedorContext();
        private RecepcionContext dbr = new RecepcionContext();
        public ActionResult Index(string Divisa, string Status, string sortOrder, string Fechainicio, string Fechafinal, string currentFilter, string searchString, string Proveedor, string Almacen, int? page, int? PageSize)
        {
            //--IDRecepcion, Fecha, Empresa, Almacen, ClaveMoneda, EstadoRec

            VRecepcionContext dbv = new VRecepcionContext();
            AlmacenContext dba = new AlmacenContext();

            string consultasql = "select * from [dbo].[VRecepcion] ";

            string filtro = string.Empty; // comienza sin ningun fitro
            string orden = string.Empty;
            string SumaSql = "select ClaveMoneda as MonedaOrigen, Sum(Subtotal) as Subtotal, Sum(IVA) as IVA, Sum(Total) as Total, sum(TotalPesos) as TotalenPesos from dbo.VRecepcion";
            string FiltroSuma = "and EstadoRec != 'Cancelado'";
            string GroupSql = "group by ClaveMoneda";
            string CadenaSql = string.Empty;
            string CadenaResumenSql = string.Empty;


            ///filtro: Proveedor
            var ProLst = new List<string>();
            var ProQry = from d in prov.Proveedores
                         orderby d.IDProveedor
                         select d.Empresa;
            ProLst.Add(" ");
            ProLst.AddRange(ProQry.Distinct());
            ViewBag.Proveedor = new SelectList(ProLst);
            ViewBag.ProveedorSeleccionado = Proveedor;


            ///filtro: Divisa
            var SerLst = new List<string>();
            var SerQry = from d in db.c_Monedas
                         orderby d.IDMoneda
                         select d.ClaveMoneda;
            SerLst.Add(" ");
            SerLst.AddRange(SerQry.Distinct());
            ViewBag.Divisa = new SelectList(SerLst);
            ViewBag.DivisaSeleccionado = Divisa;

            ///filtro: idRecepcion
            var StaLst = new List<string>();
            var StaQry = from d in BD.EncRecepciones
                         orderby d.IDRecepcion
                         select d.Status;
            StaLst.Add(" ");
            StaLst.AddRange(StaQry.Distinct());
            ViewBag.Status = new SelectList(StaLst);
            ViewBag.StatusSeleccionado = Status;

            ///filtro: Almacen
            var AlmLst = new List<string>();
            var AlmQry = from d in dba.Almacenes
                         orderby d.IDAlmacen
                         select d.Descripcion;
            AlmLst.Add(" ");
            AlmLst.AddRange(AlmQry.Distinct());
            ViewBag.Almacen = new SelectList(AlmLst);
            ViewBag.AlmacenSeleccionado = Almacen;



            if (searchString == " ")
            {
                searchString = string.Empty;
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = " where IDRecepcion = " + searchString;
                }
                else
                {
                    filtro += " and IDRecepcion= " + searchString;
                }
            }


            if (Divisa == " ")
            {
                Divisa = string.Empty;
            }
            if (!String.IsNullOrEmpty(Divisa))
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = " where ClaveMoneda = '" + Divisa + "'";
                }
                else
                {
                    filtro += " and ClaveMoneda = '" + Divisa + "'";
                }
                ViewBag.DivisaSeleccionado = Divisa;
            }

            if (Status == " ")
            {
                Status = string.Empty;
            }
            if (!String.IsNullOrEmpty(Status))
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = " where EstadoRec = '" + Status + "'";
                    FiltroSuma = "";
                }
                else
                {
                    filtro += " and EstadoRec = '" + Status + "'";
                    FiltroSuma = "";
                }
                ViewBag.StatusSeleccionado = Status;
            }

            if (Proveedor == " ")
            {
                Proveedor = string.Empty;
            }
            if (!String.IsNullOrEmpty(Proveedor))
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = " where Empresa = '" + Proveedor + "'";
                }
                else
                {
                    filtro += " and Empresa = '" + Proveedor + "'";
                }
            }
            ViewBag.ProveedorSeleccionado = Proveedor;

            if (Almacen == " ")
            {
                Almacen = string.Empty;
            }
            if (!String.IsNullOrEmpty(Almacen))
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = " where Almacen = '" + Almacen + "'";
                }
                else
                {
                    filtro += " and Almacen = '" + Almacen + "'";
                }
            }
            ViewBag.AlmacenSeleccionado = Almacen;

            if (Fechainicio == "")
            {
                Fechainicio = string.Empty;
            }
            if (Fechafinal == "")
            {
                Fechafinal = string.Empty;
            }

            if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
            {
                if (Fechafinal == string.Empty)
                {
                    Fechafinal = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                }
                if (filtro == string.Empty)
                {
                    filtro = " where  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                }
                else
                {
                    filtro += " and  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                }
            }



            switch (sortOrder)
            {
                case "OrdenCompra":
                    orden = " order by IDRecepcion Desc";
                    break;
                case "Proveedor":
                    orden = " order by IDProveedor ";
                    break;
                default:
                    orden = " order by IDRecepcion Desc";
                    break;
            }

            ///tabla filtro: FechaInicial

            if (filtro == string.Empty)
            {
                filtro = "where (Fecha between  convert(varchar,DATEADD(mm, -4, getdate()), 23) and convert(varchar, getdate(), 23)) ";
            }


            string cadena = consultasql + " " + filtro + " " + orden;

            List<VRecepcion> elementos = dbv.Database.SqlQuery<VRecepcion>(cadena).ToList();



            ViewBag.sumatoria = "";
            try
            {

                var SumaLst = new List<string>();
                var SumaQry = string.Empty;

                FiltroSuma = filtro + " " + FiltroSuma;
                SumaQry = SumaSql + " " + FiltroSuma + " " + GroupSql;

                List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
                ViewBag.sumatoria = data;

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.OrdenCompraSortParm = String.IsNullOrEmpty(sortOrder) ? "Recepcion" : "Recepcion";
            ViewBag.ProveedorSortParm = String.IsNullOrEmpty(sortOrder) ? "Proveedor" : "Proveedor";

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

            //Ordenacion


            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbv.VRecepciones.Count(); // Total number of elements


            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10 ", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(int? id)
        {
            List<VDetRecepcion> orden = BD.Database.SqlQuery<VDetRecepcion>("select DetRecepcion.IDDetRecepcion, DetRecepcion.IDAlmacen, DetRecepcion.IDExterna,DetRecepcion.Devolucion,DetRecepcion.Lote,Articulo.MinimoCompra,DetRecepcion.IDRecepcion,DetRecepcion.Suministro,Articulo.Descripcion as Articulo,DetRecepcion.Cantidad,DetRecepcion.Costo,DetRecepcion.CantidadPedida,DetRecepcion.Descuento,DetRecepcion.Importe,DetRecepcion.IVA,DetRecepcion.ImporteIva,DetRecepcion.ImporteTotal, DetRecepcion.Nota,DetRecepcion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRecepcion.Status, Caracteristica.ID as Caracteristica_ID from  DetRecepcion INNER JOIN Caracteristica ON DetRecepcion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRecepcion='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = BD.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRecepcion.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncRecepcion inner join Proveedores on Proveedores.IDProveedor=EncRecepcion.IDProveedor  where EncRecepcion.IDRecepcion='" + id + "' group by EncRecepcion.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;


            EncRecepcion encRecepcion = BD.EncRecepciones.Find(id);

            return View(encRecepcion);
        }

        public ActionResult DetailsRecepcionando(int? id)
        {
            List<VDetRecepcion> orden = BD.Database.SqlQuery<VDetRecepcion>("select DetRecepcion.IDDetRecepcion, DetRecepcion.IDExterna,DetRecepcion.Devolucion,DetRecepcion.Lote,Articulo.MinimoCompra,DetRecepcion.IDRecepcion,DetRecepcion.Suministro,Articulo.Descripcion as Articulo,DetRecepcion.Cantidad,DetRecepcion.Costo,DetRecepcion.CantidadPedida,DetRecepcion.Descuento,DetRecepcion.Importe,DetRecepcion.IVA,DetRecepcion.ImporteIva,DetRecepcion.ImporteTotal, DetRecepcion.Nota,DetRecepcion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRecepcion.Status, Caracteristica.ID as Caracteristica_ID from  DetRecepcion INNER JOIN Caracteristica ON DetRecepcion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRecepcion='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = BD.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRecepcion.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncRecepcion inner join Proveedores on Proveedores.IDProveedor=EncRecepcion.IDProveedor  where EncRecepcion.IDRecepcion='" + id + "' group by EncRecepcion.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;


            EncRecepcion encRecepcion = BD.EncRecepciones.Find(id);

            return View(encRecepcion);
        }

        public ActionResult Cancelar(int? id)
        {


            List<EncRecepcion> detalles = new PrefacturaContext().Database.SqlQuery<EncRecepcion>("Select * from  [EncRecepcion] where idrecepcion=" + id + " and status!='Cancelado'").ToList();
            foreach (EncRecepcion detalle in detalles)
            {

                if (detalle.DocumentoFactura != "")
                {
                    throw new Exception("La recepción ya cuenta con factura");
                }

            }

            RecepcionContext bd = new RecepcionContext();
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            bool tienecintas = false;
            bool tienetintas = false;

            List<DetRecepcion> recepcionD = BD.Database.SqlQuery<DetRecepcion>("select * from  DetRecepcion  where IDRecepcion=" + id).ToList();
            int OrdenCompra = 0;
            foreach (var details in recepcionD)
            {

                db.Database.ExecuteSqlCommand("update EncOrdenCompra set [Status]='Activo' where [IDOrdenCompra]=" + details.IDExterna);
                db.Database.ExecuteSqlCommand("update [DetOrdenCompra] set [Status]='Activo', suministro=0   where iddetOrdenCompra=" + details.IDDetExterna);
                OrdenCompra = details.IDExterna;
            }

            EncRecepcion encRecepcion = bd.EncRecepciones.Find(id);
            string consulta = "select * from  DetRecepcion INNER JOIN Caracteristica ON DetRecepcion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRecepcion=" + id;
            List<DetRecepcion> recepcion = BD.Database.SqlQuery<DetRecepcion>(consulta).ToList();



            foreach (var details in recepcion)
            {
                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + details.Caracteristica_ID).ToList().FirstOrDefault();
                Articulo articulodetalle = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);
                //InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();

                DetOrdenCompra orden = new OrdenCompraContext().DetOrdenCompras.ToList().Where(s => s.IDDetOrdenCompra == details.IDDetExterna).FirstOrDefault();
                int ALMACEN = 0;
                if (details.IDAlmacen == orden.IDAlmacen)
                {
                    ALMACEN = details.IDAlmacen; // es el mismo almacen
                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();


                    Articulo a = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);
                    if (inv != null)
                    {


                        if (a.CtrlStock)
                        {
                            string c = "UPDATE InventarioAlmacen SET Existencia =(Existencia-" + details.Cantidad + "),  Disponibilidad = (Disponibilidad-" + details.Cantidad + "), PorLlegar =(Porllegar+ " + details.Cantidad + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID;
                            db.Database.ExecuteSqlCommand(c);
                            db.Database.ExecuteSqlCommand("Update inventarioAlmacen set porllegar=0 where porllegar<0 and IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);

                            db.Database.ExecuteSqlCommand("Update inventarioAlmacen set existencia=0 where existencia<0 and IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);

                        }
                        InventarioAlmacen inv1 = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();


                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Recepción'," + details.Cantidad + ",'Recepción '," + details.IDRecepcion + ",''," + details.IDAlmacen + ",'S'," + inv1.Existencia + ",'Cancelación recepción',CONVERT (time, SYSDATETIMEOFFSET()))";
                        db.Database.ExecuteSqlCommand(cadenam);

                    }
                    else
                    {
                        if (a.CtrlStock)
                        {
                            db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                    + details.IDAlmacen + "," + articulodetalle.IDArticulo + "," + carateristica.ID + ",0," + orden.Cantidad + ",0,0)");
                        }



                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Recepción'," + details.Cantidad + ",'Recepción '," + details.IDRecepcion + ",''," + details.IDAlmacen + ",'S',0,'Cancelación recepción',CONVERT (time, SYSDATETIMEOFFSET()))";
                        db.Database.ExecuteSqlCommand(cadenam);

                    }
                }
                else
                {
                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();

                    ALMACEN = orden.IDAlmacen; /// cambio de almacen

                    Articulo a = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);
                    if (inv != null)
                    {


                        if (a.CtrlStock)
                        {
                            db.Database.ExecuteSqlCommand("Update inventarioAlmacen set Porllegar=0 where porllegar<0 and  IDAlmacen = " + orden.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);

                            string c = "UPDATE InventarioAlmacen SET Existencia =(Existencia-" + details.Cantidad + "),  Disponibilidad = (Disponibilidad-" + details.Cantidad + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID;
                            db.Database.ExecuteSqlCommand(c);
                            db.Database.ExecuteSqlCommand("Update inventarioAlmacen set existencia=0 where existencia<0 and IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);
                            db.Database.ExecuteSqlCommand("Update inventarioAlmacen set Porllegar=(Porllegar+" + details.Cantidad + ") where IDAlmacen = " + orden.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);

                        }
                        InventarioAlmacen inv1 = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();


                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Recepción'," + details.Cantidad + ",'Recepción '," + details.IDRecepcion + ",''," + details.IDAlmacen + ",'S'," + inv1.Existencia + ",'Cancelación recepción',CONVERT (time, SYSDATETIMEOFFSET()))";
                        db.Database.ExecuteSqlCommand(cadenam);

                    }
                    else
                    {
                        if (a.CtrlStock)
                        {
                            db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                    + details.IDAlmacen + "," + articulodetalle.IDArticulo + "," + carateristica.ID + ",0," + details.CantidadPedida + ",0,0)");
                        }



                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Recepción'," + details.Cantidad + ",'Recepción '," + details.IDRecepcion + ",''," + details.IDAlmacen + ",'S',0,'Cancelación recepción',CONVERT (time, SYSDATETIMEOFFSET()))";
                        db.Database.ExecuteSqlCommand(cadenam);

                    }
                }




                if (articulodetalle.IDTipoArticulo == 6)
                {
                    tienecintas = true;
                }
                if (articulodetalle.IDTipoArticulo == 7)
                {
                    tienetintas = true;
                }


                if (tienecintas == true)
                {
                    db.Database.ExecuteSqlCommand("delete from dbo.clslotemp where IDArticulo=" + carateristica.Articulo_IDArticulo + " and IDcaracteristica=" + carateristica.ID + " and IDRecepcion=" + details.IDRecepcion);

                }
                if (tienetintas == true)
                {
                    db.Database.ExecuteSqlCommand("delete from dbo.clslotetinta where IDArticulo=" + carateristica.Articulo_IDArticulo + " and IDcaracteristica=" + carateristica.ID + " and IDRecepcion=" + details.IDRecepcion);

                }



            }


            db.Database.ExecuteSqlCommand("update EncRecepcion set [Status]='Cancelado' where IDRecepcion='" + id + "'");
            db.Database.ExecuteSqlCommand("update DetRecepcion set [Status]='Cancelado'  where IDRecepcion='" + id + "'");
            return RedirectToAction("Index");
        }
       

        [HttpPost]
        public JsonResult ActualizarMoneda(int id)
        {

            // return RedirectToAction("OrdenCompra");
            var moneda = prov.Proveedores.Where(x => x.IDProveedor == id).ToList();
            List<SelectListItem> listmoneda = new List<SelectListItem>();
            if (moneda != null)
            {
                foreach (var x in moneda)
                {
                    listmoneda.Add(new SelectListItem { Text = x.Moneda.Descripcion, Value = x.IDMoneda.ToString() });
                }
            }
            listmoneda.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosmoneda = prov.c_Monedas.ToList();
            if (todosmoneda != null)
            {
                foreach (var x in todosmoneda)
                {
                    listmoneda.Add(new SelectListItem { Text = x.Descripcion, Value = x.IDMoneda.ToString() });
                }
            }
            return Json(new SelectList(listmoneda, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        [HttpPost]

        public ActionResult MonedaC(int? id, int? idorden, int? idmoneda)
        {
            CarritoContext dbcr = new CarritoContext();
            CarritoRecepcion carritorecepcion = dbcr.CarritoRecepciones.Find(id);

            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0, Cambio = 0, Precio = 0, subtotalaux = 0;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            List<VCarritoRecepcion> orden = db.Database.SqlQuery<VCarritoRecepcion>("select CarritoRecepcion.Lote,Articulo.Cref,CarritoRecepcion.IDDetExterna,CarritoRecepcion.IDCarritoRecepcion,CarritoRecepcion.IDOrdenCompra,CarritoRecepcion.Suministro,Articulo.Descripcion as Articulo,CarritoRecepcion.Cantidad,CarritoRecepcion.Costo,CarritoRecepcion.CantidadPedida,CarritoRecepcion.Descuento,CarritoRecepcion.Importe,CarritoRecepcion.IVA,CarritoRecepcion.ImporteIva,CarritoRecepcion.ImporteTotal,CarritoRecepcion.Nota,CarritoRecepcion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as IDCaracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRecepcion INNER JOIN Caracteristica ON CarritoRecepcion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + carritorecepcion.UserID + "' and IDOrdenCompra='" + carritorecepcion.IDOrdenCompra + "'").ToList();

            subtotal = orden.Sum(s => s.Importe);
            iva = subtotal * (decimal)0.16;
            total = subtotal + iva;


            ViewBag.Subtotal = subtotal.ToString("C");
            ViewBag.IVA = iva.ToString("C");
            ViewBag.Total = total.ToString("C");
            ViewBag.carrito = orden;
            //Termina 

            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(carritorecepcion.IDOrdenCompra);
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            List<c_Moneda> monedaorigen;
            monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();

            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + encOrdenCompra.IDMoneda + "," /*Falta de crear el destino+ DESTINO*/+ ") as TC").ToList()[0];


            for (int i = 0; i < orden.Count(); i++)
            {
                Precio = ViewBag.carrito[i].Precio * Cambio;
                importe = ViewBag.carrito[i].Cantidad * Precio;
                importeiva = importe * (decimal)0.16;
                importetotal = importeiva + importe;
                db.Database.ExecuteSqlCommand("Update [dbo].[CarritoRecepcion] set [Subtotal]='" + importe + "',[IVA]='" + importeiva + "',[Total]='" + importetotal + " where IDCarrito ='" + ViewBag.carrito[i].IDCarrito + "'");

                db.Database.ExecuteSqlCommand("delete from CarritoC where IDCarrito='" + ViewBag.carrito[i].IDCarrito + "'");
                db.SaveChanges();



            }
            VCambio cambioaux = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + encOrdenCompra.IDMoneda + "," + origen + ") as TC").ToList()[0];
            decimal TipoCambio = cambioaux.TC;
            List<DetOrdenCompra> datostotales = db.Database.SqlQuery<DetOrdenCompra>("select * from dbo.DetOrdenCompra where IDOrdenCompra='" + "'").ToList();
            subtotalr = datostotales.Sum(s => s.Importe);
            ivar = subtotalr * (decimal)0.16;
            totalr = subtotalr + ivar;
            db.Database.ExecuteSqlCommand("update [dbo].[EncOrdenCompra] set [Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "',[TipoCambio]='" + TipoCambio + "' where [IDOrdenCompra]='" + "'");


            return View();

        }
        public ActionResult OrdenCompra(int? id)
        {


            ViewBag.OrdeCompra = id;
            ViewBag.moneda = null;
            ViewBag.metodo = null;
            ViewBag.forma = null;
            ViewBag.condiciones = null;
            ViewBag.proveedor = null;
            ViewBag.id = id;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            //  db.Database.ExecuteSqlCommand("update [dbo].[CarritoRecepcion] set [UserID]='" + UserID + "' where [IDOrdenCompra]='" + id + "'");
            db.Database.ExecuteSqlCommand("delete [dbo].[CarritoRecepcion] where [UserID]='" + UserID + "' and [IDOrdenCompra]='" + id + "'");

            List<VEncOrdenC> orden = db.Database.SqlQuery<VEncOrdenC>("select EncOrdenCompra.IDOrdenCompra, CONVERT(VARCHAR(10),EncOrdenCompra.Fecha,103) AS Fecha,CONVERT(VARCHAR(10),EncOrdenCompra.FechaRequiere,103) AS FechaRequiere,Proveedores.Empresa as Proveedor from EncOrdenCompra INNER JOIN Proveedores ON EncOrdenCompra.IDProveedor= Proveedores.IDProveedor where  EncOrdenCompra.IDOrdenCompra='" + id + "' and EncOrdenCompra.Status='Activo'").ToList();
            ViewBag.data = orden;

            ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDOrdenCompra) as Dato from EncOrdenCompra where IDOrdenCompra='" + id + "' and Status='Activo'").ToList()[0];
            ViewBag.otro = denc.Dato;

            List<VDetOrdenCompra> elementos = db.Database.SqlQuery<VDetOrdenCompra>("select DetOrdenCompra.IDDetOrdenCompra, DetOrdenCompra.IDAlmacen,DetOrdenCompra.IDOrdenCompra,DetOrdenCompra.Suministro,Articulo.Descripcion as Articulo,DetOrdenCompra.Cantidad,DetOrdenCompra.Costo,DetOrdenCompra.Cantidad- DetordenCompra.suministro as CantidadPedida,DetOrdenCompra.Descuento,DetOrdenCompra.Importe,DetOrdenCompra.IVA,DetOrdenCompra.ImporteIva,DetOrdenCompra.ImporteTotal, DetOrdenCompra.Nota,DetOrdenCompra.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "' and DetOrdenCompra.Status='Activo'").ToList();
            ViewBag.datos = elementos;

            ClsDatoEntero dcompra = db.Database.SqlQuery<ClsDatoEntero>("select count(IDOrdenCompra) as Dato from DetOrdenCompra where IDOrdenCompra='" + id + "' and Status='Activo'").ToList()[0];

            if (id != null && denc.Dato > 0 && dcompra.Dato > 0 && UserID != 0)
            {
                for (int i = 0; i < elementos.Count(); i++)
                {
                    OrdenCompraContext dboc = new OrdenCompraContext();
                    DetOrdenCompra detOrdenCompra = dboc.DetOrdenCompras.Find(ViewBag.datos[i].IDDetOrdenCompra);

                    decimal importe = ViewBag.datos[i].CantidadPedida * ViewBag.datos[i].Costo;
                    decimal importeiva = importe * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                    decimal importetotal = importeiva + importe;
                    string cadenasql = "INSERT INTO CarritoRecepcion([IDDetExterna],[IDOrdenCompra],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],userID) values ('" + ViewBag.datos[i].IDDetOrdenCompra + "','" + ViewBag.datos[i].IDOrdenCompra + "','" + detOrdenCompra.IDArticulo + "','" + detOrdenCompra.Cantidad + "','" + detOrdenCompra.Costo + "','" + (detOrdenCompra.CantidadPedida - detOrdenCompra.Suministro) + "','" + detOrdenCompra.Descuento + "','" + importe + "','" + detOrdenCompra.IVA + "','" + importeiva + "','" + importetotal + "','" + detOrdenCompra.Nota + "','" + detOrdenCompra.Ordenado + "','" + detOrdenCompra.Caracteristica_ID + "','" + detOrdenCompra.IDAlmacen + "','" + detOrdenCompra.Suministro + "','Activo','" + ViewBag.datos[i].Presentacion + "', '" + ViewBag.datos[i].jsonPresentacion + "'," + UserID + ")";
                    db.Database.ExecuteSqlCommand(cadenasql);


                }

            }

            ProveedorContext pr = new ProveedorContext();
            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(id);


            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.Where(s => s.IDUsoCFDI.Equals(encOrdenCompra.IDUsoCFDI)), "IDUsoCFDI", "Descripcion");
            // ViewBag.IDAlmacen = new SelectList(db.Almacenes.Where(s => s.IDAlmacen.Equals(encOrdenCompra.IDAlmacen)), "IDAlmacen", "Descripcion");
            ViewBag.IDAlmacen = new SelectList(db.Almacenes, "IDAlmacen", "Descripcion", encOrdenCompra.IDAlmacen);
            var proveedor = prov.Proveedores.ToList();

            List<SelectListItem> moneda = new List<SelectListItem>();
            c_Moneda monedap = pr.c_Monedas.Find(encOrdenCompra.IDMoneda);
            moneda.Add(new SelectListItem { Text = monedap.Descripcion, Value = monedap.IDMoneda.ToString() });
            moneda.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosmoneda = prov.c_Monedas.ToList();
            if (todosmoneda != null)
            {
                foreach (var x in todosmoneda)
                {
                    moneda.Add(new SelectListItem { Text = x.Descripcion, Value = x.IDMoneda.ToString() });
                }
            }

            ViewBag.moneda = moneda;

            List<SelectListItem> metodo = new List<SelectListItem>();
            c_MetodoPago metodop = pr.c_MetodoPagos.Find(encOrdenCompra.IDMetodoPago);
            metodo.Add(new SelectListItem { Text = metodop.Descripcion, Value = metodop.IDMetodoPago.ToString() });
            ViewBag.metodo = metodo;

            List<SelectListItem> forma = new List<SelectListItem>();
            c_FormaPago formap = pr.c_FormaPagos.Find(encOrdenCompra.IDFormapago);
            forma.Add(new SelectListItem { Text = formap.Descripcion, Value = formap.IDFormaPago.ToString() });
            ViewBag.forma = forma;

            List<SelectListItem> condiciones = new List<SelectListItem>();
            CondicionesPago condicionesp = pr.CondicionesPagos.Find(encOrdenCompra.IDCondicionesPago);
            condiciones.Add(new SelectListItem { Text = condicionesp.Descripcion, Value = condicionesp.IDCondicionesPago.ToString() });
            ViewBag.condiciones = condiciones;

            List<SelectListItem> li = new List<SelectListItem>();
            Proveedor mm = pr.Proveedores.Find(encOrdenCompra.IDProveedor);
            li.Add(new SelectListItem { Text = mm.Empresa, Value = mm.IDProveedor.ToString() });
            ViewBag.proveedor = li;

            // ViewBag.IDAlmacenEspe = new SelectList(db.Almacenes, "IDAlmacen", "CodAlm", encOrdenCompra.IDAlmacen);
            ViewBag.IDAlmacenP = db.Almacenes.ToList();


            List<VCarritoRecepcion> lista = db.Database.SqlQuery<VCarritoRecepcion>("select CarritoRecepcion.Lote, CarritoRecepcion.IDAlmacen, Articulo.Cref,CarritoRecepcion.IDDetExterna,CarritoRecepcion.IDCarritoRecepcion,CarritoRecepcion.IDOrdenCompra,CarritoRecepcion.Suministro,Articulo.Descripcion as Articulo,CarritoRecepcion.Cantidad,CarritoRecepcion.Costo,CarritoRecepcion.CantidadPedida,CarritoRecepcion.Descuento,CarritoRecepcion.Importe,CarritoRecepcion.IVA,CarritoRecepcion.ImporteIva,CarritoRecepcion.ImporteTotal,CarritoRecepcion.Nota,CarritoRecepcion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as IDCaracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRecepcion INNER JOIN Caracteristica ON CarritoRecepcion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "' and IDOrdenCompra='" + id + "'").ToList();
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRecepcion) as Dato from CarritoRecepcion where UserID='" + UserID + "' and IDOrdenCompra='" + id + "'").ToList()[0];
            ViewBag.dato = c.Dato;


            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncOrdenCompra.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncOrdenCompra.TipoCambio)) as TotalenPesos from CarritoRecepcion inner join EncOrdenCompra on EncOrdenCompra.IDOrdenCompra=CarritoRecepcion.IDOrdenCompra where  CarritoRecepcion.UserID='" + UserID + "' and CarritoRecepcion.IDOrdenCompra='" + id + "' group by EncOrdenCompra.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;
            return View(lista);
        }

 		public ActionResult EditTicketR(int id)
        {

            //RecepcionContext db1 = new RecepcionContext();

            EncRecepcion elemento = new RecepcionContext().Database.SqlQuery<EncRecepcion>("select * from EncRecepcion where IDRecepcion=" + id).ToList().FirstOrDefault();



            return View(elemento);
        }

        [HttpPost]
        public JsonResult EditarTicketRecepcion(int id, string DocumentoFactura,string Ticket)
        {
            try
            {
              
                db.Database.ExecuteSqlCommand("update [dbo].[EncRecepcion] set  [DocumentoFactura]='" + DocumentoFactura + "', Ticket="+Ticket+" where idrecepcion=" + id);


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }



        [HttpPost]
        public ActionResult update(int idorden, string Fecha, string Observacion, string Proviene, string DocumentoFactura,int IDMoneda, int IDProveedor, int? IDAlmacen, int IDFormaPago, int IDCondicionesPago, int IDMetodoPago, int IDUsoCFDI, string Ticket,List<VCarritoRecepcion> cr)
        {
            bool tienecintas = false;
            bool tienetintas = false;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            EncOrdenCompra ordencompra = new OrdenCompraContext().EncOrdenCompras.Find(idorden);


            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, diferencia = 0, Cambio = 0, Precio = 0;
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            if (Ticket==string.Empty)
            {
                Ticket = "0";
            }
            db.Database.ExecuteSqlCommand("insert into [dbo].[EncRecepcion] ([Fecha],[Observacion],[DocumentoFactura],[IDProveedor],[IDFormaPago],[IDMoneda],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[TipoCambio],[UserID],[IDUsoCFDI],[Subtotal],[IVA],[Total],[Status],ticket) values('" + Fecha + "','" + Observacion + "','" + DocumentoFactura + "','" + IDProveedor + "','" + IDFormaPago + "','" + IDMoneda + "','" + IDMetodoPago + "','" + IDCondicionesPago + "','" + ordencompra.IDAlmacen + "','" + ordencompra.TipoCambio + "','" + UserID + "','" + IDUsoCFDI + "','0','0','0','Activo',"+ Ticket + ")");

            List<EncRecepcion> numero = db.Database.SqlQuery<EncRecepcion>("select * from [dbo].[EncRecepcion] WHERE IDRecepcion = (SELECT MAX(IDRecepcion) from EncRecepcion)").ToList();
            int num = numero.Select(s => s.IDRecepcion).FirstOrDefault();

            List<c_Moneda> clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + IDMoneda + "'").ToList();
            string clave = clavemoneda.Select(s => s.ClaveMoneda).FirstOrDefault();

            foreach (var i in cr)
            {
                OrdenCompraContext oc = new OrdenCompraContext();
                DetOrdenCompra detOrdenCompra = oc.DetOrdenCompras.Find(i.IDDetExterna);
                EncOrdenCompra encOrdenCompra = oc.EncOrdenCompras.Find(i.IDOrdenCompra);
                //Datos para tipo de cambio
                List<c_Moneda> claved = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + encOrdenCompra.IDMoneda + "'").ToList();
                //string clavedet = claved.Select(s => s.ClaveMoneda).FirstOrDefault();
                int dosc = claved.Select(s => s.IDMoneda).FirstOrDefault();

                List<c_Moneda> uno = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='" + clave + "'").ToList();
                int unoc = uno.Select(s => s.IDMoneda).FirstOrDefault();


                VCambio cambiodet = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + dosc + "," + unoc + ") as TC").ToList()[0];
                Cambio = cambiodet.TC;

                Articulo articulo = new ArticuloContext().Articulo.Find(detOrdenCompra.IDArticulo);
                if (articulo.IDTipoArticulo == 6)
                {
                    tienecintas = true;
                }

                if (articulo.IDTipoArticulo == 7)
                {
                    tienetintas = true;
                }

                Precio = i.Costo * Cambio;
                decimal suministro = detOrdenCompra.Suministro + i.Suministro;

                if (i.Cantidad <= suministro)
                {
                    subtotal = i.Suministro * Precio;
                    iva = subtotal * (decimal)0.16;
                    total = subtotal + iva;


                    db.Database.ExecuteSqlCommand("update [dbo].[DetOrdenCompra] set [Status]='Finalizado', [Suministro]='" + suministro + "' where [IDDetOrdenCompra]='" + i.IDDetExterna + "'");

                    decimal promedioPesos = 0;
                    decimal promediodls = 0;
                    decimal conversion = 0;
                    try
                    {
                        Promedio promedio = new PromedioContext().Database.SqlQuery<Promedio>("select * from Promedio where idArticulo=" + detOrdenCompra.IDArticulo).ToList().FirstOrDefault();

                        if (promedio != null)
                        {
                            try
                            {
                                decimal tCambio = 0;
                                VCambio cambioprodls = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "',181, 180 ) as TC").ToList().FirstOrDefault();
                                tCambio = cambioprodls.TC;


                                if (IDMoneda == 180)
                                {
                                    promedioPesos = (promedio.PromedioenPesos + Precio) / 2;

                                    conversion = Precio / tCambio;
                                    promediodls = (promedio.Promedioendls + conversion) / 2;


                                }
                                if (IDMoneda == 181)
                                {

                                    conversion = Precio * tCambio;
                                    promedioPesos = (promedio.PromedioenPesos + conversion) / 2;

                                    promediodls = (promedio.Promedioendls + Precio) / 2;

                                }

                            }
                            catch (Exception err)
                            {

                            }

                            try
                            {

                                
                                    db.Database.ExecuteSqlCommand("update [dbo].[Promedio] set [FechaUltimaCompra]=SYSDATETIME(), [UltimoCosto]=" + Precio + ", IDUltimaMoneda=" + IDMoneda + ", PromedioenPesos=" + promedioPesos + ", Promedioendls=" + promediodls + " where [IDArticulo]=" + detOrdenCompra.IDArticulo + "");
                                
                            }
                            catch(Exception err)
                            {

                            }
                            


                        }
                        else
                        {
                            decimal tCambio = 0;
                            VCambio cambioprodls = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "',181, 180 ) as TC").ToList().FirstOrDefault();
                            tCambio = cambioprodls.TC;


                            if (IDMoneda == 180)
                            {
                                promedioPesos = Precio;

                                conversion = Precio / tCambio;
                                promediodls =  conversion;


                            }
                            if (IDMoneda == 181)
                            {

                                conversion = Precio * tCambio;
                                promedioPesos =  conversion;

                                promediodls = Precio;

                            }
                            string cadenaprom = "insert into  [dbo].[Promedio] (idarticulo, IDultimaMoneda, [FechaUltimaCompra],ultimocosto,promedioenpesos,promedioendls) values (" + detOrdenCompra.IDArticulo + "," + IDMoneda + ",SYSDATETIME(), " + Precio + ", " + promedioPesos + ", " + Math.Round(promediodls,2) + ")";
                            db.Database.ExecuteSqlCommand(cadenaprom);
                        }
                    }
                    catch (Exception err)
                    {

                    }



                    db.Database.ExecuteSqlCommand("insert into DetRecepcion([IDRecepcion],[IDExterna],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna],[Lote],[Devolucion]) values ('" +
                                                                           num + "','" + detOrdenCompra.IDOrdenCompra + "','" + detOrdenCompra.IDArticulo + "','" + i.Suministro + "','" + Precio + "','" + i.Cantidad + "','0','" + subtotal + "','true','" + iva + "','" + total + "','" + detOrdenCompra.Nota + "','0','" + detOrdenCompra.Caracteristica_ID + "','" + i.IDAlmacen + "',0,'Finalizado','" + detOrdenCompra.Presentacion + "','" + detOrdenCompra.jsonPresentacion + "','" + i.IDDetExterna + "','" + i.Lote + "','0')");

                    List<DetRecepcion> numero2 = db.Database.SqlQuery<DetRecepcion>("select * from [dbo].[DetRecepcion] WHERE IDDetRecepcion = (SELECT MAX(IDDetRecepcion) from DetRecepcion)").ToList();
                    int num2 = numero2.Select(s => s.IDDetRecepcion).FirstOrDefault();
                    int numdoc = numero2.Select(s => s.IDRecepcion).FirstOrDefault();


                    db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Recepción','" + num + "','" + i.Suministro + "','" + fecha + "','" + i.IDOrdenCompra + "','OrdenCompra','" + UserID + "','" + i.IDDetExterna + "','" + num2 + "')");

                    Articulo a = new ArticuloContext().Articulo.Find(detOrdenCompra.IDArticulo);
                    if (a.CtrlStock)
                    {
                        // SI LLEGA MAS DE LO QUE PEDIMOS EL POR LLEGAR DEBE BAJAR A LA CANTUDAD DEL SUMINISTO 

                        decimal porllegar = i.Cantidad;
                        if (porllegar<0)
                        {
                            porllegar = 0;
                        }

                        // aqui nunca el suministro va ser menor a la cantudad por que ya lo tenemos en el primer if

                        if (i.IDAlmacen == detOrdenCompra.IDAlmacen)
                        {


                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia =(Existencia+" + i.Suministro + "),  Disponibilidad =(disponibilidad+" + i.Suministro + "), PorLlegar = (Porllegar-" + porllegar + ") WHERE IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");
                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET PorLlegar = 0 WHERE porllegar<0 and IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");


                            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                            try
                            {
                                OrdenCompraContext db = new OrdenCompraContext();
                                int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Recepción de Compra'," + i.Suministro + ",'Recepcion '," + numdoc + ",'" + i.Lote + "'," + i.IDAlmacen + ",'E'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()),"+usuario+")";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }




                        }
                        else
                        {
                           db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia =(Existencia+" + i.Suministro + "),  Disponibilidad =(disponibilidad+" + i.Suministro + ") WHERE IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");
                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad =(Existencia-Apartado) WHERE IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");

                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET PorLlegar = (PorLlegar-" +i.Suministro + ") WHERE IDAlmacen =" + detOrdenCompra.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");
                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET PorLlegar =0 where PorLlegar <0 and IDAlmacen =" + detOrdenCompra.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");


                            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                            try
                            {
                                OrdenCompraContext db = new OrdenCompraContext();
                                int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Recepción de Compra'," + i.Suministro + ",'Recepcion '," + numdoc + ",'" + i.Lote + "'," + i.IDAlmacen + ",'E'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()),"+usuario+")";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }



                        }

                    }
                    db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRecepcion] where [IDCarritoRecepcion]='" + i.IDCarritoRecepcion + "'");
                }
                else if (i.Cantidad > suministro) // quiere decir que debe seguir esperando
                {
                    diferencia = i.Cantidad - i.Suministro;
                    subtotal = i.Suministro * Precio;
                    iva = subtotal * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                    total = subtotal + iva;
                    decimal restorecepcion = 0;
                    restorecepcion = detOrdenCompra.CantidadPedida - detOrdenCompra.Suministro;




                    if (i.Suministro >= restorecepcion)
                    {
                        db.Database.ExecuteSqlCommand("update [dbo].[DetOrdenCompra] set [Status]='Finalizado', [Suministro]='" + suministro + "', [CantidadPedida]='" + i.Cantidad + "' where [IDDetOrdenCompra]='" + i.IDDetExterna + "'");

                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("update [dbo].[DetOrdenCompra] set [Status]='Activo', [Suministro]='" + suministro + "', [CantidadPedida]='" + i.Cantidad + "' where [IDDetOrdenCompra]='" + i.IDDetExterna + "'");

                    }

                    decimal promedioPesos = 0;
                    decimal promediodls = 0;
                    decimal conversion = 0;
                    try
                    {
                        Promedio promedio = new PromedioContext().Database.SqlQuery<Promedio>("select * from Promedio where idArticulo=" + detOrdenCompra.IDArticulo).ToList().FirstOrDefault();

                        if (promedio != null)
                        {
                            try
                            {
                                decimal tCambio = 0;
                                VCambio cambioprodls = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "',181, 180 ) as TC").ToList().FirstOrDefault();
                                tCambio = cambioprodls.TC;


                                if (IDMoneda == 180)
                                {
                                    promedioPesos = (promedio.PromedioenPesos + Precio) / 2;

                                    conversion = Precio / tCambio;
                                    promediodls = (promedio.Promedioendls + conversion) / 2;


                                }
                                if (IDMoneda == 181)
                                {

                                    conversion = Precio * tCambio;
                                    promedioPesos = (promedio.PromedioenPesos + conversion) / 2;

                                    promediodls = (promedio.Promedioendls + Precio) / 2;

                                }

                            }
                            catch (Exception err)
                            {

                            }
                       
                                    db.Database.ExecuteSqlCommand("insert into  [dbo].[Promedio] (idarticulo, IDultimamMoneda, [FechaUltimaCompra],ultimocosto,promedioenpesos,promedioendls) values (" + detOrdenCompra.IDArticulo + "," + IDMoneda + ",SYSDATETIME(), " + Precio + ", " + promedioPesos + ", " + promediodls + "");
                          
                          


                        }
                        else
                        {
                            decimal tCambio = 0;
                            VCambio cambioprodls = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "',181, 180 ) as TC").ToList().FirstOrDefault();
                            tCambio = cambioprodls.TC;


                            if (IDMoneda == 180)
                            {
                                promedioPesos = Precio;

                                conversion = Precio / tCambio;
                                promediodls = conversion;


                            }
                            if (IDMoneda == 181)
                            {

                                conversion = Precio * tCambio;
                                promedioPesos = conversion;

                                promediodls = Precio;

                            }
                            string cadenaprom = "insert into  [dbo].[Promedio] (idarticulo, IDultimaMoneda, [FechaUltimaCompra],ultimocosto,promedioenpesos,promedioendls) values (" + detOrdenCompra.IDArticulo + "," + IDMoneda + ",SYSDATETIME(), " + Precio + ", " + promedioPesos + ", " + Math.Round(promediodls, 2) + ")";
                            db.Database.ExecuteSqlCommand(cadenaprom);

                        }
                    }
                    catch (Exception err)
                    {

                    }


                    db.Database.ExecuteSqlCommand("insert into DetRecepcion([IDRecepcion],[IDExterna],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna],[Lote],[Devolucion]) values ('" +
                                                                           num + "','" + detOrdenCompra.IDOrdenCompra + "','" + detOrdenCompra.IDArticulo + "','" + i.Suministro + "','" + Precio + "','" + i.Cantidad + "','0','" + subtotal + "','true','" + iva + "','" + total + "','" + detOrdenCompra.Nota + "','0','" + detOrdenCompra.Caracteristica_ID + "','" + i.IDAlmacen + "'," + suministro + ",'Activo','" + detOrdenCompra.Presentacion + "','" + detOrdenCompra.jsonPresentacion + "','" + i.IDDetExterna + "','" + i.Lote + "','0')");


                    List<DetRecepcion> numero2 = db.Database.SqlQuery<DetRecepcion>("select * from [dbo].[DetRecepcion] WHERE IDDetRecepcion = (SELECT MAX(IDDetRecepcion) from DetRecepcion)").ToList();
                    int num2 = numero2.Select(s => s.IDDetRecepcion).FirstOrDefault();
                    int numdoc = numero2.Select(s => s.IDRecepcion).FirstOrDefault();

                    db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Recepción','" + num + "','" + i.Suministro + "','" + fecha + "','" + i.IDOrdenCompra + "','OrdenCompra','" + UserID + "','" + i.IDDetExterna + "','" + num2 + "')");
                    Articulo a = new ArticuloContext().Articulo.Find(detOrdenCompra.IDArticulo);
                    if (a.CtrlStock)
                    {
                        decimal porllegar = i.Suministro;
                        if (i.IDAlmacen == detOrdenCompra.IDAlmacen)
                        {


                          
                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia =(Existencia+" + i.Suministro + ") WHERE IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");
                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad =(Existencia-Apartado) WHERE IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");

                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET PorLlegar =(PorLlegar-" + i.Suministro + ") WHERE IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");

                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET PorLlegar =0  WHERE PorLlegar<0 and IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");


                            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                            try
                            {
                                OrdenCompraContext db = new OrdenCompraContext();
                                int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora],usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Recepción de Compra'," + i.Suministro + ",'Recepción'," + numdoc + ",'" + i.Lote + "'," + i.IDAlmacen + ",'E'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }


                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia =(Existencia+" + i.Suministro + "),  Disponibilidad =(disponibilidad+" + i.Suministro + ") WHERE IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");
                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad =(Existencia-Apartado) WHERE IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");

                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET PorLlegar = (PorLlegar-" + porllegar + ") WHERE IDAlmacen =" + detOrdenCompra.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");
                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET PorLlegar =0 where PorLlegar <0 and IDAlmacen =" + detOrdenCompra.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");



                            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                            try
                            {
                                OrdenCompraContext db = new OrdenCompraContext();
                                int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora],usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Recepción de Compra'," + i.Suministro + ",'Recepcion '," + numdoc + ",'" + i.Lote + "'," + i.IDAlmacen + ",'E'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }




                        }

                    }



                    // db.Database.ExecuteSqlCommand("exec dbo.MovArt'" + fecha + "'," + i.IDCaracteristica + ",'RecCom'," + i.Suministro + ",'Recepcion','" + numdoc + "','" + i.Lote + "','" + i.IDAlmacen + "','" + i.Nota + "',1");

                    db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRecepcion] where [IDCarritoRecepcion]='" + i.IDCarritoRecepcion + "'");
                }


                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDOrdenCompra) as Dato from DetOrdenCompra where IDOrdenCompra ='" + i.IDOrdenCompra + "'").ToList().FirstOrDefault();
                int x = c.Dato;

                ClsDatoEntero b = db.Database.SqlQuery<ClsDatoEntero>("select count(IDOrdenCompra) as Dato from DetOrdenCompra where IDOrdenCompra ='" + i.IDOrdenCompra + "' and Status = 'Finalizado'").ToList().FirstOrDefault();
                int y = b.Dato;

                if (x == y)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncOrdenCompra] set [Status]='Finalizado' where [IDOrdenCompra]='" + i.IDOrdenCompra + "'");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncOrdenCompra] set [Status]='Activo' where [IDOrdenCompra]='" + i.IDOrdenCompra + "'");
                }



            }

            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
            VCambio cambioaux = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + IDMoneda + "," + origen + ") as TC").ToList()[0];
            decimal TipoCambio = cambioaux.TC;
            List<DetRecepcion> datostotales = db.Database.SqlQuery<DetRecepcion>("select * from dbo.DetRecepcion where IDRecepcion='" + num + "'").ToList();
            subtotalr = datostotales.Sum(s => s.Importe);
            ivar = subtotalr * (decimal)0.16;
            totalr = subtotalr + ivar;
            // db.Database.ExecuteSqlCommand("update [dbo].[EncRecepcion] set [Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "',[TipoCambio]='" + TipoCambio + "' where [IDRecepcion]='" + num + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[EncRecepcion] set [Subtotal]=" + subtotalr + ",[IVA]=" + ivar + ",[Total]=" + totalr + " where [IDRecepcion]=" + num + "");
           


            
           
            if (tienecintas  ||  tienetintas )
            {
                return RedirectToAction("DetailsRecepcionando", new { id = num });
            }
            return RedirectToAction("Index");
        }


        public ActionResult delete(int? id)
        {
            CarritoContext cr = new CarritoContext();
            CarritoRecepcion carritoRecepcion = cr.CarritoRecepciones.Find(id);
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRecepcion] where [IDCarritoRecepcion]='" + id + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetOrdenCompra] set [Status]='Activo' where [IDDetOrdenCompra]='" + carritoRecepcion.IDDetExterna + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[EncOrdenCompra] set [Status]='Activo' where [IDOrdenCompra]='" + carritoRecepcion.IDOrdenCompra + "'");
            ClsDatoEntero dato = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRecepcion) as Dato from CarritoRecepcion where UserID ='" + UserID + "' and IDOrdenCompra='" + carritoRecepcion.IDOrdenCompra + "'").ToList()[0];
            if (dato.Dato == 0)
            {
                return RedirectToAction("Index", "EncOrdenCompra");
            }
            ViewBag.id = carritoRecepcion.IDOrdenCompra;



            ProveedorContext pr = new ProveedorContext();
            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(carritoRecepcion.IDOrdenCompra);

            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.Where(s => s.IDUsoCFDI.Equals(encOrdenCompra.IDUsoCFDI)), "IDUsoCFDI", "Descripcion");
            ViewBag.IDAlmacen = new SelectList(db.Almacenes.Where(s => s.IDAlmacen.Equals(encOrdenCompra.IDAlmacen)), "IDAlmacen", "Descripcion");

            var proveedor = prov.Proveedores.ToList();

            List<SelectListItem> moneda = new List<SelectListItem>();
            c_Moneda mm1 = pr.c_Monedas.Find(encOrdenCompra.IDMoneda);
            moneda.Add(new SelectListItem { Text = mm1.Descripcion, Value = mm1.IDMoneda.ToString() });
            moneda.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosmoneda = prov.c_Monedas.ToList();
            if (todosmoneda != null)
            {
                foreach (var x in todosmoneda)
                {
                    moneda.Add(new SelectListItem { Text = x.Descripcion, Value = x.IDMoneda.ToString() });
                }
            }

            ViewBag.moneda = moneda;

            List<SelectListItem> metodo = new List<SelectListItem>();
            c_MetodoPago metodop = pr.c_MetodoPagos.Find(encOrdenCompra.IDMetodoPago);
            metodo.Add(new SelectListItem { Text = metodop.Descripcion, Value = metodop.IDMetodoPago.ToString() });
            ViewBag.metodo = metodo;

            List<SelectListItem> forma = new List<SelectListItem>();
            c_FormaPago formap = pr.c_FormaPagos.Find(encOrdenCompra.IDFormapago);
            forma.Add(new SelectListItem { Text = formap.Descripcion, Value = formap.IDFormaPago.ToString() });
            ViewBag.forma = forma;

            List<SelectListItem> condiciones = new List<SelectListItem>();
            CondicionesPago condicionesp = pr.CondicionesPagos.Find(encOrdenCompra.IDCondicionesPago);
            condiciones.Add(new SelectListItem { Text = condicionesp.Descripcion, Value = condicionesp.IDCondicionesPago.ToString() });
            ViewBag.condiciones = condiciones;
            List<SelectListItem> li = new List<SelectListItem>();
            Proveedor mm = pr.Proveedores.Find(encOrdenCompra.IDProveedor);
            li.Add(new SelectListItem { Text = mm.Empresa, Value = mm.IDProveedor.ToString() });
            ViewBag.proveedor = li;

            //List<VEncOrdenC> orden = db.Database.SqlQuery<VEncOrdenC>("select EncOrdenCompra.IDOrdenCompra, CONVERT(VARCHAR(10),EncOrdenCompra.Fecha,103) AS Fecha,CONVERT(VARCHAR(10),EncOrdenCompra.FechaRequiere,103) AS FechaRequiere,Proveedores.Empresa as Proveedor from EncOrdenCompra INNER JOIN Proveedores ON EncOrdenCompra.IDProveedor= Proveedores.IDProveedor where  EncOrdenCompra.IDOrdenCompra='" + id + "' and EncOrdenCompra.Status='Activo'").ToList();

            ViewBag.data = null;

            //ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDOrdenCompra) as Dato from EncOrdenCompra where IDOrdenCompra='" + id + "' and Status='Activo'").ToList()[0];

            ViewBag.otro = 0;

            //List<VDetOrdenCompra> elementos = db.Database.SqlQuery<VDetOrdenCompra>("select DetOrdenCompra.IDDetOrdenCompra,DetOrdenCompra.IDOrdenCompra,DetOrdenCompra.Suministro,Articulo.Descripcion as Articulo,DetOrdenCompra.Cantidad,DetOrdenCompra.Costo,DetOrdenCompra.CantidadPedida,DetOrdenCompra.Descuento,DetOrdenCompra.Importe,DetOrdenCompra.IVA,DetOrdenCompra.ImporteIva,DetOrdenCompra.ImporteTotal, DetOrdenCompra.Nota,DetOrdenCompra.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "' and Status='Activo'").ToList();

            ViewBag.datos = null;
            ViewBag.IDAlmacenP = db.Almacenes.ToList();


            List<VCarritoRecepcion> lista = db.Database.SqlQuery<VCarritoRecepcion>("select CarritoRecepcion.Lote, CarritoRecepcion.IDAlmacen, Articulo.Cref,CarritoRecepcion.IDDetExterna,CarritoRecepcion.IDCarritoRecepcion,CarritoRecepcion.IDOrdenCompra,CarritoRecepcion.Suministro,Articulo.Descripcion as Articulo,CarritoRecepcion.Cantidad,CarritoRecepcion.Costo,CarritoRecepcion.CantidadPedida,CarritoRecepcion.Descuento,CarritoRecepcion.Importe,CarritoRecepcion.IVA,CarritoRecepcion.ImporteIva,CarritoRecepcion.ImporteTotal,CarritoRecepcion.Nota,CarritoRecepcion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as IDCaracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRecepcion INNER JOIN Caracteristica ON CarritoRecepcion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "' and IDOrdenCompra='" + carritoRecepcion.IDOrdenCompra + "'").ToList();
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRecepcion) as Dato from CarritoRecepcion where UserID='" + UserID + "' and IDOrdenCompra='" + id + "'").ToList()[0];
            ViewBag.dato = c.Dato;


            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncOrdenCompra.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncOrdenCompra.TipoCambio)) as TotalenPesos from CarritoRecepcion inner join EncOrdenCompra on EncOrdenCompra.IDOrdenCompra=CarritoRecepcion.IDOrdenCompra where  CarritoRecepcion.UserID='" + UserID + "' and CarritoRecepcion.IDOrdenCompra='" + carritoRecepcion.IDOrdenCompra + "'group by EncOrdenCompra.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;



            return View("OrdenCompra", lista);

        }
        //////////////////////////////////////////////////////////////Devolución///////////////////////////////////////////////////////////////////////////////////
        public ActionResult Devolucion(int? id)
        {
            List<VEncOrdenC> orden = db.Database.SqlQuery<VEncOrdenC>("select EncRecepcion.IDRecepcion as IDOrdenCompra, CONVERT(VARCHAR(10),EncRecepcion.Fecha,103) AS Fecha,EncRecepcion.Observacion AS FechaRequiere,Proveedores.Empresa as Proveedor from EncRecepcion INNER JOIN Proveedores ON EncRecepcion.IDProveedor= Proveedores.IDProveedor where EncRecepcion.IDRecepcion=" + id + "").ToList();
            ViewBag.data = orden;

            ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRecepcion) as Dato from EncRecepcion where IDRecepcion=" + id ).ToList().FirstOrDefault();
            int w = denc.Dato;
            ViewBag.otro = w;

            List<VDetRecepcion> elementos = db.Database.SqlQuery<VDetRecepcion>("select DetRecepcion.IDDetRecepcion, DetRecepcion.IDDetRecepcion,DetRecepcion.IDRecepcion,DetRecepcion.Suministro,Articulo.Descripcion as Articulo,DetRecepcion.Cantidad,DetRecepcion.Costo,DetRecepcion.CantidadPedida,DetRecepcion.Descuento,DetRecepcion.Importe,DetRecepcion.IVA,DetRecepcion.ImporteIva,DetRecepcion.ImporteTotal,DetRecepcion.Nota,DetRecepcion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRecepcion.Caracteristica_ID  from  DetRecepcion INNER JOIN Caracteristica ON DetRecepcion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRecepcion=" + id ).ToList();
            ViewBag.datos = elementos;


            ClsDatoEntero dcompra = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRecepcion) as Dato from DetRecepcion where IDRecepcion=" + id  ).ToList().FirstOrDefault();

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();


            if (id != null && denc.Dato > 0 && dcompra.Dato > 0)
            {
                db.Database.ExecuteSqlCommand("delete from CarritoDevolucion");

                for (int i = 0; i < elementos.Count(); i++)
                {
                    
                    RecepcionContext dboc = new RecepcionContext();
                    DetRecepcion detRecepcion = dboc.DetRecepciones.Find(ViewBag.datos[i].IDDetRecepcion);


                    db.Database.ExecuteSqlCommand("INSERT INTO CarritoDevolucion([IDDetExterna],[IDRecepcion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[Lote]) values ('" + ViewBag.datos[i].IDDetRecepcion + "','" + ViewBag.datos[i].IDRecepcion + "','" + detRecepcion.IDArticulo + "','" + detRecepcion.Cantidad + "','" + detRecepcion.Costo + "','" + detRecepcion.Devolucion + "','" + detRecepcion.Descuento + "','" + detRecepcion.Importe + "','" + detRecepcion.IVA + "','" + detRecepcion.ImporteIva + "','" + detRecepcion.ImporteTotal + "','" + detRecepcion.Nota + "','" + detRecepcion.Ordenado + "','" + detRecepcion.Caracteristica_ID + "','" + detRecepcion.IDAlmacen + "','" + detRecepcion.Suministro + "','Activo','" + ViewBag.datos[i].Presentacion + "', '" + ViewBag.datos[i].jsonPresentacion + "','" + detRecepcion.Lote + "')");


                }
                db.Database.ExecuteSqlCommand("update [dbo].[DetRecepcion] set [Status]='Devuelto' where [IDRecepcion]=" + id );
                db.Database.ExecuteSqlCommand("update [dbo].[EncRecepcion] set [Status]='Devuelto' where [IDRecepcion]=" + id );
                db.Database.ExecuteSqlCommand("update [dbo].[CarritoDevolucion] set [UserID]='" + UserID + "' where [IDRecepcion]='" + id + "'");
            }
            bool tienecintas = false;
            foreach (var d in elementos)
            {
                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + d.Caracteristica_ID).ToList().FirstOrDefault();
                Articulo articulodetalle = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);
                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == d.IDAlmacen && s.IDCaracteristica == d.Caracteristica_ID).FirstOrDefault();



                if (articulodetalle.IDTipoArticulo == 6)
                {
                    tienecintas = true;
                }



            }
            List<VCarritoDevolucion> lista = db.Database.SqlQuery<VCarritoDevolucion>("select CarritoDevolucion.Lote,CarritoDevolucion.IDDetExterna,CarritoDevolucion.IDCarritoDevolucion,CarritoDevolucion.IDRecepcion,CarritoDevolucion.Suministro,Articulo.Descripcion as Articulo,CarritoDevolucion.Cantidad,CarritoDevolucion.Costo,CarritoDevolucion.CantidadPedida,CarritoDevolucion.Descuento,CarritoDevolucion.Importe,CarritoDevolucion.IVA,CarritoDevolucion.ImporteIva,CarritoDevolucion.ImporteTotal,CarritoDevolucion.Nota,CarritoDevolucion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as ID_Caracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoDevolucion INNER JOIN Caracteristica ON CarritoDevolucion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRecepcion='" + id + "'").ToList();
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoDevolucion) as Dato from CarritoDevolucion where  IDRecepcion='" + id + "'").ToList()[0];
            int x = c.Dato;
            ViewBag.dato = x;
            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRecepcion.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRecepcion.TipoCambio)) as TotalenPesos from CarritoDevolucion inner join EncRecepcion on EncRecepcion.IDRecepcion=CarritoDevolucion.IDRecepcion where CarritoDevolucion.UserID='" + UserID + "' and CarritoDevolucion.IDRecepcion='" + id + "' group by EncRecepcion.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;

            //if (tienecintas == false)
            //{
                return View(lista);
            //}
            //else
            //{
            //    return RedirectToAction("DevolucionCintasRecepcion", new { idrecepcion = id });
            //}




        }

        public ActionResult PdfDevolucion(int id)
        {
            EncDevolucion recepcion = dbr.EncDevoluciones.Find(id);
            DocumentoRecepcionD x = new DocumentoRecepcionD();

            x.claveMoneda = recepcion.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = recepcion.Fecha.ToShortDateString();
            //x.fechaRequerida = recepcion.FechaRequiere.ToShortDateString();
            x.Proveedor = recepcion.Proveedor.Empresa;
            x.formaPago = recepcion.c_FormaPago.ClaveFormaPago;
            x.metodoPago = recepcion.c_MetodoPago.ClaveMetodoPago;
            x.RFCproveedor = recepcion.Proveedor.Telefonouno;
            x.total = float.Parse(recepcion.Total.ToString());
            x.subtotal = float.Parse(recepcion.Subtotal.ToString());
            x.tipo_cambio = recepcion.TipoCambio.ToString();
            x.Observacion = recepcion.Observacion;
            x.DocumentoFactura = recepcion.DocumentoFactura.ToString();
            x.serie = "";
            x.folio = recepcion.IDDevolucion.ToString();
            x.UsodelCFDI = recepcion.c_UsoCFDI.Descripcion;
            x.IDALmacen = recepcion.Almacen.IDAlmacen + " " + recepcion.Almacen.Descripcion;
            x.Telefonoproveedor = recepcion.Proveedor.Telefonouno;
            ImpuestoRecepcionD iva = new ImpuestoRecepcionD();
            iva.impuesto = "IVA";
            iva.tasa = 16;
            iva.importe = float.Parse(recepcion.IVA.ToString());


            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;


            List<DetDevolucion> detalles = db.Database.SqlQuery<DetDevolucion>("select * from DetDevolucion where IDDevolucion= " + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoRecepcionD producto = new ProductoRecepcionD();
                Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);
                SIAAPI.Models.Comercial.Caracteristica caracteristica = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + item.Caracteristica_ID).FirstOrDefault();

                c_ClaveProductoServicio claveprodsat = db.Database.SqlQuery<c_ClaveProductoServicio>("select c_ClaveProductoServicio.* from (Articulo inner join Familia on articulo.IDFamilia= Familia.IDFamilia) inner join c_ClaveProductoServicio on c_ClaveProductoServicio.IDProdServ= Familia.IDProdServ where Articulo.IDArticulo= " + item.IDArticulo).ToList()[0];
                producto.ClaveProducto = claveprodsat.ClaveProdServ;

                producto.c_unidad = arti.c_ClaveUnidad.ClaveUnidad;
                producto.cantidad = item.Cantidad.ToString();
                producto.descripcion = arti.Descripcion;
                producto.cref = arti.Cref;
                producto.valorUnitario = float.Parse(item.Costo.ToString());
                producto.v_unitario = float.Parse(item.Costo.ToString());
                producto.importe = float.Parse(item.Importe.ToString());
                ///
                producto.Presentacion = caracteristica.Presentacion; //item.presentacion;
                ///
                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }
            //CreaDevolucionRecepcionnPDF documento = new CreaDevolucionRecepcionnPDF(logoempresa, x);
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            try
            {
                //CreaCOPDF documento = new CreaCOPDF(logoempresa, x);
                CreaDevolucionRecepcionnPDF documento = new CreaDevolucionRecepcionnPDF(logoempresa, x);
                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            return RedirectToAction("Index");
        }

        public ActionResult DevolucionCintasRecepcion(int idrecepcion)
        
        {
            ViewBag.recepcion = idrecepcion;
            List<DetRecepcion> recepcion = db.Database.SqlQuery<DetRecepcion>(" select d.* from articulo as a inner join detrecepcion as d on a.idarticulo=d.idarticulo where a.idtipoarticulo=6 and d.idrecepcion=" + idrecepcion).ToList();

            var listado = new RecepcionContext().DetRecepciones.Where(s => s.IDRecepcion == idrecepcion).ToList();

            ViewBag.listado = listado;



            return View(recepcion);
        }





        public ActionResult deletedev(int? id)
        {
            CarritoContext cr = new CarritoContext();
            CarritoDevolucion carritoDevolucion = cr.CarritoDevoluciones.Find(id);
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoDevolucion] where [IDCarritoDevolucion]='" + id + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetRecepcion] set [Status]='Activo' where [IDDetRecepcion]='" + carritoDevolucion.IDDetExterna + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[EncRecepcion] set [Status]='Activo' where [IDRecepcion]='" + carritoDevolucion.IDRecepcion + "'");

            ClsDatoEntero dato = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoDevolucion) as Dato from CarritoDevolucion where UserID ='" + UserID + "' and IDRecepcion='" + carritoDevolucion.IDRecepcion + "'").ToList()[0];
            if (dato.Dato == 0)
            {
                return RedirectToAction("Index");
            }
            ViewBag.id = carritoDevolucion.IDRecepcion;
            ViewBag.datos = null;
            ViewBag.data = null;
            ViewBag.otro = 0;
            List<VCarritoDevolucion> lista = db.Database.SqlQuery<VCarritoDevolucion>("select CarritoDevolucion.Lote,CarritoDevolucion.IDDetExterna,CarritoDevolucion.IDCarritoDevolucion,CarritoDevolucion.IDRecepcion,CarritoDevolucion.Suministro,Articulo.Descripcion as Articulo,CarritoDevolucion.Cantidad,CarritoDevolucion.Costo,CarritoDevolucion.CantidadPedida,CarritoDevolucion.Descuento,CarritoDevolucion.Importe,CarritoDevolucion.IVA,CarritoDevolucion.ImporteIva,CarritoDevolucion.ImporteTotal,CarritoDevolucion.Nota,CarritoDevolucion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as ID_Caracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoDevolucion INNER JOIN Caracteristica ON CarritoDevolucion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "' and IDRecepcion='" + carritoDevolucion.IDRecepcion + "'").ToList();
            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRecepcion.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRecepcion.TipoCambio)) as TotalenPesos from CarritoDevolucion inner join EncRecepcion on EncRecepcion.IDRecepcion=CarritoDevolucion.IDRecepcion where CarritoDevolucion.UserID='" + UserID + "' and CarritoDevolucion.IDRecepcion='" + carritoDevolucion.IDRecepcion + "' group by EncRecepcion.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;
            return View("Devolucion", lista);
            //return RedirectToAction("Devolucion", new {id=id });


        }


        [HttpPost]
        public ActionResult updateD(List<VCarritoDevolucion> cr, FormCollection coleccion)
        {

            string ObservacionDevolucion = "";
            try
            {
                ObservacionDevolucion=coleccion.Get("Observacion");
            }
            catch (Exception err)
            {

            }
            int id = 0;
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, cantidad = 0, Precio = 0, devuelve = 0, CantidadTotal = 0;
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            foreach (var i in cr)
            {
                id = i.IDRecepcion;
            }
            db.Database.ExecuteSqlCommand("insert into [dbo].[EncDevolucion] ([Fecha],[Observacion],[DocumentoFactura],[IDProveedor],[IDFormaPago],[IDMoneda],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[TipoCambio],[UserID],[IDUsoCFDI],[Subtotal],[IVA],[Total],[Status]) select [Fecha],[Observacion],[DocumentoFactura],[IDProveedor],[IDFormaPago],[IDMoneda],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[TipoCambio],[UserID],[IDUsoCFDI],[Subtotal],[IVA],[Total],[Status] from [dbo].[EncRecepcion]  where IDRecepcion='" + id + "' ");

            List<EncDevolucion> numero = db.Database.SqlQuery<EncDevolucion>("select * from [dbo].[EncDevolucion] WHERE IDDevolucion = (SELECT MAX(IDDevolucion) from EncDevolucion)").ToList();
            int num = numero.Select(s => s.IDDevolucion).FirstOrDefault();


            db.Database.ExecuteSqlCommand("update [dbo].[EncDevolucion] set Observacion='"+ObservacionDevolucion+"',[Status]='Activo',[Fecha]='" + fecha + "',[UserID]='" + UserID + "' where [IDDevolucion]='" + num + "'");
            bool tienecintas = false;
            foreach (var i in cr)
            {
               
                RecepcionContext oc = new RecepcionContext();
                DetRecepcion detRecepcion = oc.DetRecepciones.Find(i.IDDetExterna);
                Articulo articulodetalle = new ArticuloContext().Articulo.Find(detRecepcion.IDArticulo);
                cantidad = detRecepcion.Cantidad - detRecepcion.Devolucion;
                devuelve = detRecepcion.Devolucion + i.Cantidad;
                Precio = detRecepcion.Costo;
                if (cantidad == i.Cantidad)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[DetRecepcion] set [CantidadPedida]='" + i.Cantidad + "',[Devolucion]='" + devuelve + "' where [IDDetRecepcion]='" + i.IDDetExterna + "'");
                    CantidadTotal = i.Cantidad;
                }

                else if (cantidad > i.Cantidad)
                {
                    decimal cantidadpedida = cantidad - i.Cantidad;

                    //ViewBag.mensaje = "La cantidad devuelta es mayor a la cantidad en existencia, fue devuelta la cantidad total en existencia";
                    db.Database.ExecuteSqlCommand("update [dbo].[DetRecepcion] set [CantidadPedida]='" + cantidadpedida + "',[Status]='Activo',[Devolucion]='" + devuelve + "' where [IDDetRecepcion]='" + i.IDDetExterna + "'");
                    CantidadTotal = i.Cantidad;
                }
                else if (cantidad < i.Cantidad)
                {
                    devuelve = detRecepcion.Devolucion + cantidad;
                    db.Database.ExecuteSqlCommand("update [dbo].[DetRecepcion] set [CantidadPedida]='" + cantidad + "',[Status]='Devuelto',[Devolucion]='" + devuelve + "' where [IDDetRecepcion]='" + i.IDDetExterna + "'");
                    CantidadTotal = cantidad;

                }

                subtotal = i.Cantidad * Precio;
                iva = subtotal * (decimal)0.16;
                total = subtotal + iva;

                db.Database.ExecuteSqlCommand("insert into DetDevolucion([IDDevolucion],[IDRecepcion],[IDDetRecepcion],[IDArticulo],[Caracteristica_ID],[Cantidad],[Costo],[Importe],[ImporteIva],[ImporteTotal],[Nota],[Lote],[IDAlmacen],[Status]) values ('" + num + "','" + detRecepcion.IDRecepcion + "', '" + i.IDDetExterna + "', '" + detRecepcion.IDArticulo + "', '" + detRecepcion.Caracteristica_ID + "', '" + CantidadTotal + "','" + Precio + "','" + subtotal + "','" + iva + "','" + total + "','" + detRecepcion.Nota + "','" + detRecepcion.Lote + "','" + detRecepcion.IDAlmacen + "','Activo')");

                List<DetDevolucion> numero2 = db.Database.SqlQuery<DetDevolucion>("select * from [dbo].[DetDevolucion] WHERE IDDetDevolucion = (SELECT MAX(IDDetDevolucion) from DetDevolucion)").ToList();
                int num2 = numero2.Select(s => s.IDDetDevolucion).FirstOrDefault();

                db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Devolución','" + num + "','" + CantidadTotal + "','" + fecha + "','" + i.IDRecepcion + "','Recepción','" + UserID + "','" + i.IDDetExterna + "','" + num2 + "')");
                db.Database.ExecuteSqlCommand("exec dbo.MovArt'" + fecha + "'," + detRecepcion.Caracteristica_ID + ",'DevCom'," + CantidadTotal + ",'Devolucion'," + num2 + ",'" + detRecepcion.Lote + "','" + detRecepcion.IDAlmacen + "','" + detRecepcion.Nota + "',0");
                db.Database.ExecuteSqlCommand("update dbo.InventarioAlmacen set apartado=0 where apartado<0 and IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");

                db.Database.ExecuteSqlCommand("update dbo.InventarioAlmacen set disponibilidad=(existencia-apartado) where  IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");

                //try
                //{
                //    if (articulodetalle.CtrlStock)
                //    {
                //        // SI LLEGA MAS DE LO QUE PEDIMOS EL POR LLEGAR DEBE BAJAR A LA CANTUDAD DEL SUMINISTO 


                //        // aqui nunca el suministro va ser menor a la cantudad por que ya lo tenemos en el primer if




                //            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia =(Existencia+" + i.Suministro + "),  Disponibilidad =(disponibilidad+" + i.Suministro + ") WHERE IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");
                //            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET PorLlegar = 0 WHERE porllegar<0 and IDAlmacen =" + i.IDAlmacen + " and IDCaracteristica =" + i.IDCaracteristica + "");


                //            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                //            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                //            try
                //            {
                //                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                //                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Devolución Recepción'," + i.Suministro + ",'Devolución '," + i.IDRecepcion + ",'" + i.Lote + "'," + i.IDAlmacen + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                //                db.Database.ExecuteSqlCommand(cadenam);
                //            }
                //            catch (Exception err)
                //            {
                //                string mensajee = err.Message;
                //            }







                //    }
                //}
                //catch (Exception err)
                //{

                //}



                db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoDevolucion] where [IDCarritoDevolucion]='" + i.IDCarritoDevolucion + "'");


                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRecepcion) as Dato from DetRecepcion where IDRecepcion ='" + i.IDRecepcion + "'").ToList()[0];
                int x = c.Dato;

                ClsDatoEntero b = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRecepcion) as Dato from DetRecepcion where IDRecepcion ='" + i.IDRecepcion + "' and Status = 'Devuelto'").ToList()[0];
                int y = b.Dato;

                if (x == y)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncRecepcion] set [Status]='Finalizado' where [IDRecepcion]='" + i.IDRecepcion + "'");
                    db.Database.ExecuteSqlCommand("update [dbo].[DetRecepcion] set [Status]='Finalizado' where [IDRecepcion]='" + i.IDRecepcion + "'");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncRecepcion] set [Status]='Activo' where [IDRecepcion]='" + i.IDRecepcion + "'");
                }

                if (articulodetalle.IDTipoArticulo == 6)
                {
                    tienecintas = true;
                }


            }


            List<DetDevolucion> datostotales = db.Database.SqlQuery<DetDevolucion>("select * from dbo.DetDevolucion where IDDevolucion='" + num + "'").ToList();
            subtotalr = datostotales.Sum(s => s.Importe);
            ivar = subtotalr * (decimal)0.16;
            totalr = subtotalr + ivar;
            db.Database.ExecuteSqlCommand("update [dbo].[EncDevolucion] set [Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "' where [IDDevolucion]='" + num + "'");

            if (tienecintas == false)
            {
                return RedirectToAction("IndexDevolucion");
            }
            else
            {
                return RedirectToAction("DevolucionCintasRecepcion", new { idrecepcion = id });
            }

        }

        public ActionResult IndexDevolucion(string Divisa, string Status, string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {


            var SerLst = new List<string>();
            var SerQry = from d in BD.c_Monedas
                         orderby d.IDMoneda
                         select d.ClaveMoneda;
            SerLst.Add(" ");
            SerLst.AddRange(SerQry.Distinct());
            ViewBag.Divisa = new SelectList(SerLst);

            var StaLst = new List<string>();
            var StaQry = from d in BD.EncDevoluciones
                         orderby d.IDDevolucion
                         select d.Status;
            StaLst.Add(" ");
            StaLst.AddRange(StaQry.Distinct());
            ViewBag.Status = new SelectList(StaLst);


            var elementos = (from s in BD.EncDevoluciones
                             select s).OrderByDescending(s => s.IDDevolucion);


            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucion.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucion where [Status]<>'Cancelado' group by EncDevolucion.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;


            if (!String.IsNullOrEmpty(searchString))
            {
                elementos = (from s in BD.EncDevoluciones
                             select s).OrderByDescending(s => s.IDDevolucion);

                elementos = elementos.Where(s => s.IDDevolucion.ToString().Contains(searchString) || s.Proveedor.Empresa.Contains(searchString)).OrderByDescending(s => s.IDDevolucion);

                var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucion.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucion inner join Proveedores on Proveedores.IDProveedor=EncDevolucion.IDProveedor where (CAST(EncDevolucion.IDDevolucion AS nvarchar(max))='" + searchString + "' or Proveedores.Empresa='" + searchString + "') and [Status]<>'Cancelado' group by EncDevolucion.IDMoneda ").ToList();
                ViewBag.sumatoria = filtro;


            }
            //Filtro Divisa
            if (!String.IsNullOrEmpty(Divisa))
            {
                elementos = (from s in BD.EncDevoluciones
                             select s).OrderByDescending(s => s.IDDevolucion);
                elementos = elementos.Where(s => s.c_Moneda.ClaveMoneda == Divisa).OrderByDescending(s => s.IDDevolucion);

                var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucion.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucion inner join c_Moneda on c_Moneda.IDMoneda=EncDevolucion.IDMoneda  where c_Moneda.ClaveMoneda='" + Divisa + "' and [Status]<>'Cancelado' group by EncDevolucion.IDMoneda").ToList();
                ViewBag.sumatoria = divisa;


            }

            //Filtro Status
            if (!String.IsNullOrEmpty(Status))
            {
                elementos = (from s in BD.EncDevoluciones
                             select s).OrderByDescending(s => s.IDDevolucion);
                elementos = elementos.Where(s => s.Status.Equals(Status)).OrderByDescending(s => s.IDDevolucion);

                var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucion.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucion inner join c_Moneda on c_Moneda.IDMoneda=EncDevolucion.IDMoneda  where Status='" + Status + "' group by EncDevolucion.IDMoneda").ToList();
                ViewBag.sumatoria = divisa;


            }


            ViewBag.CurrentSort = sortOrder;
            ViewBag.DevolucionSortParm = String.IsNullOrEmpty(sortOrder) ? "Devolucion" : "Devolucion";
            ViewBag.ProveedorSortParm = String.IsNullOrEmpty(sortOrder) ? "Proveedor" : "Proveedor";

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

            //Ordenacion

            switch (sortOrder)
            {
                case "Devolucion":
                    elementos = elementos.OrderByDescending(s => s.IDDevolucion);
                    break;
                case "Proveedor":
                    elementos = elementos.OrderByDescending(s => s.Proveedor.Empresa);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDDevolucion);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = BD.EncDevoluciones.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10 ", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));



        }


        public ActionResult DetailsDevolucion(int? id)
        {
            List<VDetDevolucion> orden = BD.Database.SqlQuery<VDetDevolucion>("select Articulo.Cref,DetDevolucion.IDDetDevolucion,DetDevolucion.IDDevolucion,DetDevolucion.IDRecepcion,DetDevolucion.IDDetRecepcion,Articulo.Descripcion as Articulo,DetDevolucion.Cantidad,DetDevolucion.Costo,DetDevolucion.Importe,DetDevolucion.ImporteIva,DetDevolucion.ImporteTotal,DetDevolucion.Nota,Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion,DetDevolucion.Status,DetDevolucion.Lote from  DetDevolucion INNER JOIN Caracteristica ON DetDevolucion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDDevolucion='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = BD.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucion.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucion inner join Proveedores on Proveedores.IDProveedor=EncDevolucion.IDProveedor  where EncDevolucion.IDDevolucion='" + id + "' group by EncDevolucion.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;


            EncDevolucion encDevolucion = BD.EncDevoluciones.Find(id);

            return View(encDevolucion);
        }


       
        public ActionResult CreaReporteporfechanombre()
        {
            //Buscar Cliente
            ProveedorContext dbc = new ProveedorContext();
            var proveedor = dbc.Proveedores.OrderBy(m => m.Empresa).ToList();
            List<SelectListItem> listaPro = new List<SelectListItem>();
            listaPro.Add(new SelectListItem { Text = "--Selecciona Proveedor--", Value = "0" });

            foreach (var m in proveedor)
            {
                listaPro.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });
            }
            ViewBag.proveedor = listaPro;
            return View();
        }

      

        public ActionResult Cargarcintas(int ID)
        {
            //  VCarritoRecepcion carrito = new CarritoContext().Database.SqlQuery<VCarritoRecepcion>("select CarritoRecepcion.Lote,Articulo.Cref,CarritoRecepcion.IDDetExterna,CarritoRecepcion.IDCarritoRecepcion,CarritoRecepcion.IDOrdenCompra,CarritoRecepcion.Suministro,Articulo.Descripcion as Articulo,CarritoRecepcion.Cantidad,CarritoRecepcion.Costo,CarritoRecepcion.CantidadPedida,CarritoRecepcion.Descuento,CarritoRecepcion.Importe,CarritoRecepcion.IVA,CarritoRecepcion.ImporteIva,CarritoRecepcion.ImporteTotal,CarritoRecepcion.Nota,CarritoRecepcion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as IDCaracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRecepcion INNER JOIN Caracteristica ON CarritoRecepcion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where  CarritoRecepcion.IDCarritoRecepcion=" + ID + "").ToList().FirstOrDefault();
            //   
            DetRecepcion detrecepcion = new RecepcionContext().DetRecepciones.Find(ID);
            Articulo articulo = new ArticuloContext().Articulo.Find(detrecepcion.IDArticulo);
            DetOrdenCompra detordencompra = new OrdenCompraContext().DetOrdenCompras.Find(detrecepcion.IDDetExterna);
            FormulaSiaapi.Formulas formula = new FormulaSiaapi.Formulas();

            int ANCHO = int.Parse(formula.getvalor("ANCHO", detordencompra.Presentacion).ToString());

            int LARGO = int.Parse(formula.getvalor("LARGO", detordencompra.Presentacion).ToString());


            Clslotempcreate elemento = new Clslotempcreate();

            elemento.Cref = articulo.Cref;

            elemento.IDArticulo = articulo.IDArticulo;

            elemento.IDCaracteristica = detrecepcion.Caracteristica_ID;
            elemento.IDRecepcion = detrecepcion.IDRecepcion;


            elemento.Ancho = ANCHO;

            elemento.Largo = LARGO;

            elemento.OrdenCompra = detrecepcion.IDExterna;
            elemento.IDDetOrdenCompra = detrecepcion.IDDetRecepcion;

            elemento.IDCarrito = ID;
            elemento.MetrosCuadrados = decimal.Parse(elemento.Largo.ToString()) * (decimal.Parse(elemento.Ancho.ToString()) / 1000);

            elemento.NoCintas = 1;

            elemento.Lote = "";

            elemento.LoteInterno = articulo.Cref + "/" + ANCHO + "/" + LARGO + "/" + elemento.OrdenCompra + "/" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + "/";


            return View(elemento);
        }

        [HttpPost]
        public ActionResult Cargarcintas(Clslotempcreate elemento)
        {
            OrdenCompraContext db = new OrdenCompraContext();
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

           
            try
            {
                if (elemento.Largo == 0)
                {
                    throw new Exception("El Largo no puede ser 0");
                }

                if (elemento.MetrosCuadrados == 0)
                {
                    throw new Exception("Los metros cuadrado no pueden ser 0");
                }
                int contador = 1;


                try
                {
                    int registros = new OrdenCompraContext().Database.SqlQuery<ClsDatoEntero>("select Max(NoCinta) as Dato from Clslotemp where OrdenCompra =" + elemento.OrdenCompra + " and IDDetOrdenCompra=" + elemento.IDDetOrdenCompra).ToList().FirstOrDefault().Dato;
                    contador = registros + 1;
                }
                catch (Exception err2)
                {
                    string mensaje2 = err2.Message;
                    contador = 1;
                }

                EncOrdenCompra orden = new OrdenCompraContext().EncOrdenCompras.Find(elemento.OrdenCompra);


                for (int i = 1; i <= elemento.NoCintas; i++)
                {


                    Clslotemp nuevo = new Clslotemp();
                    nuevo.Ancho = elemento.Ancho;
                    nuevo.Largo = elemento.Largo;
                    nuevo.NoCinta = contador;
                    nuevo.IDDetOrdenCompra = elemento.IDDetOrdenCompra;
                    nuevo.MetrosCuadrados = elemento.MetrosCuadrados;
                    nuevo.Fecha = DateTime.Now;
                    nuevo.MetrosDisponibles = elemento.MetrosCuadrados;
                    nuevo.Metrosutilizados = 0;
                    nuevo.OrdenCompra = elemento.OrdenCompra;
                    nuevo.Lote = elemento.Lote;
                    nuevo.LoteInterno = elemento.LoteInterno + contador;
                    nuevo.IDProveedor = orden.IDProveedor;
                    nuevo.IDAlmacen = orden.IDAlmacen;
                    nuevo.IDArticulo = elemento.IDArticulo;
                    nuevo.IDCaracteristica = elemento.IDCaracteristica;
                    nuevo.IDRecepcion = elemento.IDRecepcion;
                    contador++;
                    db.Clslotesmp.Add(nuevo);
                    db.SaveChanges();

                    Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + elemento.IDCaracteristica).ToList().FirstOrDefault();
                    DetRecepcion det = new RecepcionContext().Database.SqlQuery<DetRecepcion>("select*from DetRecepcion where caracteristica_id=" + elemento.IDCaracteristica + " and idrecepcion=" + elemento.IDRecepcion).ToList().FirstOrDefault();

                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == det.IDAlmacen && s.IDCaracteristica == elemento.IDCaracteristica).FirstOrDefault();
                    try
                    {
                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                        cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Recepción de Compra'," + elemento.MetrosCuadrados + ",'Recepción - Lote interno'," + elemento.IDRecepcion + ",'" + nuevo.LoteInterno + "'," + det.IDAlmacen + ",'N/A'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";
                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {
                        string mensajee = err.Message;
                    }
                }
                

                return View(elemento);
            }
            catch (Exception err)
            {
                ViewBag.MensajeError = err.Message;
                return View(elemento);
            }

         


        }

        public ActionResult Cargartintas(int ID)
        {
            //  VCarritoRecepcion carrito = new CarritoContext().Database.SqlQuery<VCarritoRecepcion>("select CarritoRecepcion.Lote,Articulo.Cref,CarritoRecepcion.IDDetExterna,CarritoRecepcion.IDCarritoRecepcion,CarritoRecepcion.IDOrdenCompra,CarritoRecepcion.Suministro,Articulo.Descripcion as Articulo,CarritoRecepcion.Cantidad,CarritoRecepcion.Costo,CarritoRecepcion.CantidadPedida,CarritoRecepcion.Descuento,CarritoRecepcion.Importe,CarritoRecepcion.IVA,CarritoRecepcion.ImporteIva,CarritoRecepcion.ImporteTotal,CarritoRecepcion.Nota,CarritoRecepcion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as IDCaracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRecepcion INNER JOIN Caracteristica ON CarritoRecepcion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where  CarritoRecepcion.IDCarritoRecepcion=" + ID + "").ToList().FirstOrDefault();
            //   
            DetRecepcion detrecepcion = new RecepcionContext().DetRecepciones.Find(ID);
            EncRecepcion recepcion = new RecepcionContext().EncRecepciones.Find(detrecepcion.IDRecepcion);
            Articulo articulo = new ArticuloContext().Articulo.Find(detrecepcion.IDArticulo);
            DetOrdenCompra detordencompra = new OrdenCompraContext().DetOrdenCompras.Find(detrecepcion.IDDetExterna);
           

            Clslotetintacreate elemento = new Clslotetintacreate();

            ViewBag.Articulo = articulo;
            elemento.IDRecepcion = detrecepcion.IDRecepcion;
            elemento.iddetrecepcion = detrecepcion.IDDetRecepcion;
            elemento.NoEnvases = int.Parse(Math.Round((detrecepcion.Cantidad*0.25M),0).ToString());
            elemento.cantidad = 4;


            elemento.unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad).ClaveUnidad;

            elemento.ccodalm = new AlmacenContext().Almacenes.Find(detrecepcion.IDAlmacen).CodAlm;

            

           

           


            return View(elemento);
        }


        [HttpPost]
        public ActionResult Cargartintas(Clslotetintacreate elemento)
        {
            OrdenCompraContext db = new OrdenCompraContext();
            try
            {
                if (elemento.NoEnvases == 0)
                {
                    throw new Exception("El numero de envases no puede ser 0");
                }

                if (elemento.cantidad == 0)
                {
                    throw new Exception("La cantidad no pueden ser 0");
                }
                int contador = 1;


                try
                {
                    int registros = new OrdenCompraContext().Database.SqlQuery<ClsDatoEntero>("select Max(consecutivo) as Dato from Clslotetinta where iddetrecepcion =" + elemento.iddetrecepcion ).ToList().FirstOrDefault().Dato;
                    contador = registros + 1;
                }
                catch (Exception err2)
                {
                    string mensaje2 = err2.Message;
                    contador = 1;
                }

                DetRecepcion detrecepcion = new RecepcionContext().DetRecepciones.Find(elemento.iddetrecepcion);
                EncRecepcion recepcion = new RecepcionContext().EncRecepciones.Find(detrecepcion.IDRecepcion);
                Articulo articulo = new ArticuloContext().Articulo.Find(detrecepcion.IDArticulo);
                DetOrdenCompra detordencompra = new OrdenCompraContext().DetOrdenCompras.Find(detrecepcion.IDDetExterna);

                for (int i = 1; i <= elemento.NoEnvases; i++)
                {


                    Clslotetinta nuevo = new Clslotetinta();
                    //nuevo.Ancho = elemento.Ancho;
                    //nuevo.Largo = elemento.Largo;
                    nuevo.consecutivo = contador;
                    //nuevo.IDDetOrdenCompra = elemento.IDDetOrdenCompra;
                    nuevo.unidad = elemento.unidad;
                    nuevo.OrdenCompra = detordencompra.IDOrdenCompra;
                    nuevo.IDFamilia = articulo.IDFamilia;
                    nuevo.cantidad = elemento.cantidad;
                    nuevo.fecha = DateTime.Now;
                    Almacen almacen = new AlmacenContext().Almacenes.Find(detrecepcion.IDAlmacen);
                    nuevo.ccodalm = almacen.CodAlm;
                    nuevo.lote = almacen.CodAlm + "&" + articulo.Cref +"&" + elemento.cantidad + "&" + elemento.unidad + "&" + recepcion.DocumentoFactura + "&" + contador;
                    nuevo.Cref = articulo.Cref;
                    nuevo.factura = recepcion.DocumentoFactura;
                    nuevo.IDAlmacen = detrecepcion.IDAlmacen;
                    nuevo.idarticulo = detrecepcion.IDArticulo;
                    nuevo.idcaracteristica = detrecepcion.Caracteristica_ID;
                    nuevo.IDRecepcion = detrecepcion.IDRecepcion;
                    nuevo.iddetrecepcion = detrecepcion.IDDetRecepcion;
                    nuevo.Estado = "Existe";
                    contador++;
                    ClslotetintaContext db2 = new ClslotetintaContext();
                    db2.Tintas.Add(nuevo);
                    db2.SaveChanges();

                }
                return View(elemento);
            }
            catch (Exception err)
            {
                DetRecepcion detrecepcion = new RecepcionContext().DetRecepciones.Find(elemento.iddetrecepcion);
                EncRecepcion recepcion = new RecepcionContext().EncRecepciones.Find(detrecepcion.IDRecepcion);
                Articulo articulo = new ArticuloContext().Articulo.Find(detrecepcion.IDArticulo);
                DetOrdenCompra detordencompra = new OrdenCompraContext().DetOrdenCompras.Find(detrecepcion.IDDetExterna);
                ViewBag.Articulo = articulo;

                ViewBag.MensajeError = err.Message;


                Clslotetintacreate elemento2 = new Clslotetintacreate();

                ViewBag.Articulo = articulo;
                elemento2.IDRecepcion = detrecepcion.IDRecepcion;
                elemento2.iddetrecepcion = detrecepcion.IDDetRecepcion;



                elemento2.unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad).ClaveUnidad;

                elemento2.ccodalm = new AlmacenContext().Almacenes.Find(detrecepcion.IDAlmacen).CodAlm;


                return View(elemento2);

            }

           


        }

        public ActionResult Detallecintas(int IDDetRecepcion)
        {
            var elementos = new RecepcionContext().lotes.ToList().Where(S => S.IDDetOrdenCompra == IDDetRecepcion); // en realidad es detalle de la recepcion
            return View(elementos);
        }

        public ActionResult Detalletintas(int IDDetRecepcion)
        {
            var elementos = new ClslotetintaContext().Tintas.ToList().Where(S => S.iddetrecepcion == IDDetRecepcion); // en realidad es detalle de la recepcion
            return View(elementos);
        }

        public ActionResult Eliminarcinta(int ID, int IDDetRecepcion)
        {
            new RecepcionContext().Database.ExecuteSqlCommand("delete from clslotemp where id=" + ID);

            return RedirectToAction("Cargarcintas", new { ID = IDDetRecepcion });
        }
        public ActionResult EliminarcintaDev(int ID, int IDDetRecepcion)
        {
            DetRecepcion detRecepcion = new RecepcionContext().DetRecepciones.Find(IDDetRecepcion);

            new RecepcionContext().Database.ExecuteSqlCommand("delete from clslotemp where id=" + ID);

            return RedirectToAction("DevolucionCintasRecepcion", new { idrecepcion = detRecepcion.IDRecepcion });
        }

        public ActionResult Eliminartinta(int ID, int IDDetRecepcion)
        {
            new RecepcionContext().Database.ExecuteSqlCommand("delete from clslotetinta where id=" + ID);

            return RedirectToAction("Cargartintas", new { ID = IDDetRecepcion });
        }

        public ActionResult ImprimirEtiquetas(int IDDetRecepcion)
        {
            Empresa empresa = new EmpresaContext().empresas.Find(2);
            List<Clslotemp> elementos = new RecepcionContext().Database.SqlQuery<Clslotemp>("select * from clslotemp where IDDetOrdenCompra=" + IDDetRecepcion).ToList();
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            DetRecepcion recepcion = new RecepcionContext().DetRecepciones.Find(IDDetRecepcion);
            if (elementos.Count > 0)
            {
                string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                Reportes.CreaEtiquetaMP documento = new Reportes.CreaEtiquetaMP(logoempresa, elementos, recepcion.IDRecepcion);
                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            else
            {
                return Content("No hay lotes");
            }




        }


        public ActionResult ImprimirEtiqtint(int IDDetRecepcion)
        {
            Empresa empresa = new EmpresaContext().empresas.Find(2);
            List<Clslotetinta> elementos = new RecepcionContext().Database.SqlQuery<Clslotetinta>("select * from clslotetinta where iddetrecepcion=" + IDDetRecepcion).ToList();
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            DetRecepcion recepcion = new RecepcionContext().DetRecepciones.Find(IDDetRecepcion);
            if (elementos.Count > 0)
            {
                string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                Reportes.GeneradorEtiqTin documento = new Reportes.GeneradorEtiqTin(logoempresa, elementos, recepcion.IDRecepcion);
                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            else
            {
                return Content("No hay lotes");
            }




        }

        public void PdfRecepcion(int id)
        {
            EncRecepcion recepcion = dbr.EncRecepciones.Find(id);
            DocumentoRecepcion x = new DocumentoRecepcion();

            x.claveMoneda = recepcion.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = recepcion.Fecha.ToShortDateString();
            //x.fechaRequerida = recepcion.FechaRequiere.ToShortDateString();
            x.Proveedor = recepcion.Proveedor.Empresa;
            x.formaPago = recepcion.c_FormaPago.ClaveFormaPago;
            x.metodoPago = recepcion.c_MetodoPago.ClaveMetodoPago;
            x.RFCproveedor = recepcion.Proveedor.Telefonouno;
            x.total = float.Parse(recepcion.Total.ToString());
            x.subtotal = float.Parse(recepcion.Subtotal.ToString());
            x.tipo_cambio = recepcion.TipoCambio.ToString();
            x.Observacion = recepcion.Observacion;
            x.DocumentoFactura = recepcion.DocumentoFactura.ToString();
            x.serie = "";
            x.folio = recepcion.IDRecepcion.ToString();
            x.UsodelCFDI = recepcion.c_UsoCFDI.Descripcion;
            x.IDALmacen = recepcion.Almacen.IDAlmacen + " " + recepcion.Almacen.Descripcion;
            x.Telefonoproveedor = recepcion.Proveedor.Telefonouno;
            ImpuestoRecepcion iva = new ImpuestoRecepcion();
            iva.impuesto = "IVA";
            iva.tasa = 16;
            iva.importe = float.Parse(recepcion.IVA.ToString());


            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;


            List<DetRecepcion> detalles = db.Database.SqlQuery<DetRecepcion>("select * from DetRecepcion where IDRecepcion= " + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoRecepcion producto = new ProductoRecepcion();
                Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);

                c_ClaveProductoServicio claveprodsat = db.Database.SqlQuery<c_ClaveProductoServicio>("select c_ClaveProductoServicio.* from (Articulo inner join Familia on articulo.IDFamilia= Familia.IDFamilia) inner join c_ClaveProductoServicio on c_ClaveProductoServicio.IDProdServ= Familia.IDProdServ where Articulo.IDArticulo= " + item.IDArticulo).ToList()[0];
                producto.ClaveProducto = claveprodsat.ClaveProdServ;

                producto.c_unidad = arti.c_ClaveUnidad.ClaveUnidad;
                producto.cantidad = item.Cantidad.ToString();
                producto.descripcion = arti.Descripcion;
                producto.cref = arti.Cref;
                producto.valorUnitario = float.Parse(item.Costo.ToString());
                producto.v_unitario = float.Parse(item.Costo.ToString());
                producto.importe = float.Parse(item.Importe.ToString());
                ///
                producto.Presentacion = item.Presentacion; //item.presentacion;
                ///
                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);
            CreaRecepcionnPDF documento = new CreaRecepcionnPDF(logoempresa, x);

            RedirectToAction("Index");

        }
        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            System.Drawing.Image returnImage = null;
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                returnImage = System.Drawing.Image.FromStream(ms, true);//Exception occurs here
            }
            catch { }
            return returnImage;

        }


        public ActionResult EntreFechasRec()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasRec(EFecha modelo, FormCollection coleccion)
        {
            VRecepcionContext dbr = new VRecepcionContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");

            string cadena = "";
            string cadenaDet = "";
            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {

                cadena = "select * from [dbo].[VRecepcion] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' ";
                var datos = dbr.Database.SqlQuery<VRecepcion>(cadena).ToList();
                ViewBag.datos = datos;

                cadenaDet = "select * from [dbo].[VRecepcionDet] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' order by [Empresa],[DocumentoFactura],[IDRecepcion] ";
                var datosDet = dbr.Database.SqlQuery<VRecepcionDet>(cadenaDet).ToList();
                ViewBag.datosDet = datosDet;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Recepción");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:Q1"].Style.Font.Size = 20;
                Sheet.Cells["A1:Q1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:Q3"].Style.Font.Bold = true;
                Sheet.Cells["A1:Q1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:Q1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de recepciones");

                row = 2;
                Sheet.Cells["A1:Q1"].Style.Font.Size = 12;
                Sheet.Cells["A1:Q1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:Q1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:Q2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:Q3"].Style.Font.Bold = true;
                Sheet.Cells["A3:Q3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:Q3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID Recepción");
                Sheet.Cells["B3"].RichText.Add("Fecha Recepción");
                Sheet.Cells["C3"].RichText.Add("O.C.");
                Sheet.Cells["D3"].RichText.Add("Fecha O.C.");
                Sheet.Cells["E3"].RichText.Add("RFC");
                Sheet.Cells["F3"].RichText.Add("Empresa");
                Sheet.Cells["G3"].RichText.Add("Factura");
                Sheet.Cells["H3"].RichText.Add("Subtotal");
                Sheet.Cells["I3"].RichText.Add("Iva");
                Sheet.Cells["J3"].RichText.Add("Total");
                Sheet.Cells["K3"].RichText.Add("Moneda");
                Sheet.Cells["L3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["M3"].RichText.Add("Total en Pesos");
                Sheet.Cells["N3"].RichText.Add("Status");
                Sheet.Cells["O3"].RichText.Add("Almacen");
                Sheet.Cells["P3"].RichText.Add("Observación");
                Sheet.Cells["Q3"].RichText.Add("Generada por");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:Q3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VRecepcion item in ViewBag.datos)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDRecepcion;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                    SIAAPI.clasescfdi.ClsRastreaDA rastrea = new SIAAPI.clasescfdi.ClsRastreaDA();
                    List<SIAAPI.clasescfdi.NodoTrazo> nodos = rastrea.getDocumentoAnterior("OCompra", item.IDRecepcion, "Encabezado");
                    string oc = "";
                    string fechaoc = "";
                    try
                    {
                        foreach (SIAAPI.clasescfdi.NodoTrazo nodo in nodos)
                        {
                            EncOrdenCompra aocante = new OrdenCompraContext().EncOrdenCompras.Find(nodo.ID);
                            fechaoc += aocante.Fecha.ToShortDateString() + " ";
                            oc = oc + nodo.Descripcion;
                        }

                    }
                    catch (Exception)
                    {

                    }
                    Sheet.Cells[string.Format("C{0}", row)].Value = oc;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = fechaoc;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.RFC;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Empresa;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.DocumentoFactura;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.ClaveMoneda;

                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "##0.00";
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.TotalPesos;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.EstadoRec;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.Almacen;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.Observacion;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.Username;

                    row++;
                }

                //Hoja No. 2
                Sheet = Ep.Workbook.Worksheets.Add("Detalles");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:T1"].Style.Font.Size = 20;
                Sheet.Cells["A1:T1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:T3"].Style.Font.Bold = true;
                Sheet.Cells["A1:T1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:T1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Detalle de recepciones");

                row = 2;
                Sheet.Cells["A1:T1"].Style.Font.Size = 12;
                Sheet.Cells["A1:T1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:T1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:T2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:T3"].Style.Font.Bold = true;
                Sheet.Cells["A3:T3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:T3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID Detalle");
                Sheet.Cells["B3"].RichText.Add("ID Recepción");
                Sheet.Cells["C3"].RichText.Add("Fecha Recepcion");
                Sheet.Cells["D3"].RichText.Add("Factura");
                Sheet.Cells["E3"].RichText.Add("O.C.");
                Sheet.Cells["F3"].RichText.Add("Fecha O.C.");
                Sheet.Cells["G3"].RichText.Add("Empresa");
                Sheet.Cells["H3"].RichText.Add("Clave");
                Sheet.Cells["I3"].RichText.Add("Artículo");
                Sheet.Cells["J3"].RichText.Add("Presentación");
                Sheet.Cells["K3"].RichText.Add("Lote");
                Sheet.Cells["L3"].RichText.Add("Cantidad Pedida");
                Sheet.Cells["M3"].RichText.Add("Suministro");
                Sheet.Cells["N3"].RichText.Add("Devolución");
                Sheet.Cells["O3"].RichText.Add("Costo");
                Sheet.Cells["P3"].RichText.Add("Importe");
                Sheet.Cells["Q3"].RichText.Add("IVA");
                Sheet.Cells["R3"].RichText.Add("Total");
                Sheet.Cells["S3"].RichText.Add("Almacén");
                Sheet.Cells["T3"].RichText.Add("Nota");
                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:T3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VRecepcionDet itemD in ViewBag.datosDet)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.IDDetRecepcion;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.IDRecepcion;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemD.DocumentoFactura;
                    SIAAPI.clasescfdi.ClsRastreaDA rastrea = new SIAAPI.clasescfdi.ClsRastreaDA();
                    List<SIAAPI.clasescfdi.NodoTrazo> nodos = rastrea.getDocumentoAnterior("OCompra", itemD.IDRecepcion, "Encabezado");
                    string oc = "";
                    string fechaoc = "";
                    try
                    {
                        foreach (SIAAPI.clasescfdi.NodoTrazo nodo in nodos)
                        {
                            EncOrdenCompra aocante = new OrdenCompraContext().EncOrdenCompras.Find(nodo.ID);
                            fechaoc += aocante.Fecha.ToShortDateString() + " ";
                            oc = oc + nodo.Descripcion;
                        }

                    }
                    catch (Exception)
                    {

                    }
                    Sheet.Cells[string.Format("E{0}", row)].Value = oc;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("F{0}", row)].Value = fechaoc;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.Empresa;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.Cref;
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.Descripcion;
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.Presentacion;
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.Lote;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.CantidadPedida;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("M{0}", row)].Value = itemD.Suministro;
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("N{0}", row)].Value = itemD.Devolucion;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("O{0}", row)].Value = itemD.Costo;
                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("P{0}", row)].Value = itemD.Importe;
                    Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("Q{0}", row)].Value = itemD.ImporteIVA;
                    Sheet.Cells[string.Format("R{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("R{0}", row)].Value = itemD.ImporteTotal;
                    Sheet.Cells[string.Format("S{0}", row)].Value = itemD.Almacen;
                    Sheet.Cells[string.Format("T{0}", row)].Value = itemD.Nota;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Recepcion.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }

        public ActionResult Recepciones()
        {
            ReporteRecepciones elemento = new ReporteRecepciones();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult Recepciones(ReporteRecepciones modelo, FormCollection coleccion)
        {
            VRecepcionContext dbr = new VRecepcionContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            ReporteRecepciones report = new ReporteRecepciones();
            //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"),DateTime.Parse( "2019-07-30"));
            byte[] abytes = report.PrepareReport(modelo.fechaini, modelo.fechafin);
            return File(abytes, "application/pdf", "ReporteRecepciones.pdf");

        }
        
        public ActionResult CancelarDevolucion(int? id)

        {

            int IDRecepcion = 0;
          

            RecepcionContext bd = new RecepcionContext();
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            bool tienecintas = false;
            bool tienetintas = false;


            EncDevolucion encRecepcion = bd.EncDevoluciones.Find(id);
            string consulta = "select * from  DetDevolucion INNER JOIN Caracteristica ON DetDevolucion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where iddevolucion=" + id;
            List<DetDevolucion> recepcion = BD.Database.SqlQuery<DetDevolucion>(consulta).ToList();



            foreach (var details in recepcion)
            {
                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + details.Caracteristica_ID).ToList().FirstOrDefault();
                Articulo articulodetalle = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);
                //InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();

               
                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();

                    
                    Articulo a = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);

                try
                {
                    if (a.CtrlStock)
                    {
                        string c = "UPDATE InventarioAlmacen SET Existencia =(Existencia+" + details.Cantidad + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID;
                        db.Database.ExecuteSqlCommand(c);

                        db.Database.ExecuteSqlCommand("UPDATE InventarioAlmacen SET Disponibilidad = (Existencia -apartado ) WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica = " + details.Caracteristica_ID);

                    }
                    InventarioAlmacen inv1 = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();


                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación de Devolución'," + details.Cantidad + ",'Devolución Recepción '," + details.IDDevolucion + ",''," + details.IDAlmacen + ",'E'," + inv1.Existencia + ",'Cancelación recepción',CONVERT (time, SYSDATETIMEOFFSET()))";
                    db.Database.ExecuteSqlCommand(cadenam);
                }
                catch (Exception err)
                {

                }                



                if (articulodetalle.IDTipoArticulo == 6)
                {
                    tienecintas = true;
                }
                if (articulodetalle.IDTipoArticulo == 7)
                {
                    tienetintas = true;
                }


              
                IDRecepcion = details.IDRecepcion;

            }


            db.Database.ExecuteSqlCommand("update EncDevolucion set [Status]='Cancelado' where IDDevolucion='" + id + "'");
            db.Database.ExecuteSqlCommand("update detdevolucion set [Status]='Cancelado'  where IDDevolucion='" + id + "'");
            if (tienecintas || tienetintas)
            {
                return RedirectToAction("DetailsRecepcionando", new { id = IDRecepcion });
            }
            return RedirectToAction("IndexDevolucion");
        }

        public ActionResult ReporteMaterialRecepcionado(/*EFecha modelo, FormCollection coleccion*/)
        {
            //VReporteMacrosurtidoAlmacenContext dbe = new VReporteMacrosurtidoAlmacenContext();
            //string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            //string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            //string cual = coleccion.Get("Enviar");
            string cadena = "";

            //if (cual == "Generar reporte")
            //{
            //    return View();
            //}
            //if (cual == "Generar excel")
            //{

            cadena = "select * from ReporteMateriaP2Meses order by fecha";
            var datos = new ReporteMateriaP2MesesContext().Database.SqlQuery<ReporteMateriaP2Meses>(cadena).ToList();
            ViewBag.datos = datos;

            ExcelPackage Ep = new ExcelPackage();
            //Crear la hoja y poner el nombre de la pestaña del libro
            var Sheet = Ep.Workbook.Worksheets.Add("Materia P Recepcionada últimos 2 meses");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:M1"].Style.Font.Size = 20;
            Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:M3"].Style.Font.Bold = true;
            Sheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Recepciones Materia Prima");

            row = 2;
            Sheet.Cells["A1:M1"].Style.Font.Size = 12;
            Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:M1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:M2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado del periodo de datos
            //row = 2;
            //Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
            //Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //Sheet.Cells[string.Format("B2", row)].Value = FI;
            //Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
            //Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //Sheet.Cells[string.Format("E2", row)].Value = FF;
            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:M3"].Style.Font.Bold = true;
            Sheet.Cells["A3:M3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:M3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A3"].RichText.Add("No. Recepción");
            Sheet.Cells["B3"].RichText.Add("Fecha");
            Sheet.Cells["C3"].RichText.Add("Clave");
            Sheet.Cells["D3"].RichText.Add("Descripción");
            Sheet.Cells["E3"].RichText.Add("Presentación");
            Sheet.Cells["F3"].RichText.Add("Cantidad");


            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A3:M3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 4;
           
            foreach (ReporteMateriaP2Meses item in datos)
            {
                
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDRecepcion;
                Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

                Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Cref;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Presentacion;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Cantidad;

                row++;
            }


            //Se genera el archivo y se descarga

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteMacroSurtidoAlmacen.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
            return Redirect("/blah");
            //}

        }

    }

}
