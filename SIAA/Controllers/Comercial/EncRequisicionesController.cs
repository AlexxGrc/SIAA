using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.ViewModels.Comercial;
using System.Data.SqlClient;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.Reportes;
using System.IO;
using System.Net.Mail;
using System.Text;

using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Globalization;
using SIAAPI.Models.Produccion;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Sistemas,Almacenista,AdminProduccion, Produccion, Facturacion, Gerencia,Compras")]
    public class EncRequisicionesController : Controller
    {
        private RequisicionesContext db = new RequisicionesContext();
        private ProveedorContext prov = new ProveedorContext();
        private VRequisicionesContext dbv = new VRequisicionesContext();

        public ActionResult Index(string Empresa, string Divisa, string Status, string sortOrder, string currentFilter, string searchString, string Fechainicio, string Fechafinal, int? page, int? PageSize)
        {

            string ConsultaSql = "select * from [dbo].[VRequisicion]";
            string FiltroSql = string.Empty;
            string OrdenSql = "order by IDRequisicion desc";
            string SumaSql = "select ClaveMoneda as MonedaOrigen, sum(Subtotal) as Subtotal, sum(IVA) as IVA, sum(Total) As Total from[dbo].[VRequisicion]";
            string GroupSql = "group by ClaveMoneda";
            string CadenaSql = string.Empty;
            string CadenaResumenSql = string.Empty;

            try
            {


                ViewBag.IDproveedorDesconocido = SIAAPI.Properties.Settings.Default.IDProveedorDesconocido;


                var SerLst = new List<string>();
                var SerQry = from d in db.c_Monedas
                             orderby d.IDMoneda
                             select d.ClaveMoneda;
                SerLst.Add(" ");
                SerLst.AddRange(SerQry.Distinct());
                ViewBag.Divisa = new SelectList(SerLst);
                ViewBag.DivisaSeleccionada = Divisa;

                var StaLst = new List<string>();
                var StaQry = from d in db.EncRequisicioness
                             orderby d.IDRequisicion
                             select d.Status;
                StaLst.Add(" ");
                StaLst.AddRange(StaQry.Distinct());
                ViewBag.Status = new SelectList(StaLst);

                ///tabla filtro: Divisa
                if (Divisa == " ")
                {
                    Divisa = string.Empty;
                }

                if (!String.IsNullOrEmpty(Divisa))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where ClaveMoneda = '" + Divisa + "'";
                    }
                    else
                    {
                        FiltroSql += " and  ClaveMoneda = '" + Divisa + "'";
                    }
                }

                ///tabla filtro: Status
                if (Status == " ")
                {
                    Status = string.Empty;
                }

                if (!String.IsNullOrEmpty(Status))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where Status = '" + Status + "'";
                    }
                    else
                    {
                        FiltroSql += " and  Status = '" + Status + "'";
                    }
                }

                ///tabla filtro: searchString
                if (searchString == "")
                {
                    searchString = string.Empty;
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where IDRequisicion = " + int.Parse(searchString.ToString()) + "";
                    }
                    else
                    {
                        FiltroSql += " and  IDRequisicion = " + int.Parse(searchString.ToString()) + "";
                    }

                }

                ///tabla filtro: Proveedor
                if (Empresa == "")
                {
                    Empresa = string.Empty;
                }

                if (!String.IsNullOrEmpty(Empresa))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where Empresa like '" + Empresa + "%'";
                    }
                    else
                    {
                        FiltroSql += " and  Empresa like '" + Empresa + "%'";
                    }

                }

                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    if (Fechafinal == string.Empty)
                    {
                        Fechafinal = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                    }
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = " where  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                    }
                    else
                    {
                        FiltroSql += " and  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                    }
                }


                ViewBag.CurrentSort = sortOrder;
                ViewBag.DivisaSortParm = String.IsNullOrEmpty(sortOrder) ? "Divisa" : "";
                ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) ? "Status" : "";
                ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
                ViewBag.EmpresaSortParm = String.IsNullOrEmpty(sortOrder) ? "Empresa" : "";
                ViewBag.AlmacenSortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "";
                // Pass filtering string to view in order to maintain filtering when paging
                ViewBag.Fechainicio = Fechainicio;
                ViewBag.Fechafinal = Fechafinal;

                switch (sortOrder)
                {
                    case "Requisición":
                        OrdenSql = " order by  IDRequisicion, IDRequisicion asc ";
                        break;

                    case "Empresa":
                        OrdenSql = " order by  Empresa, IDRequisicion desc ";
                        break;

                    default:
                        OrdenSql = "order by IDRequisicion desc ";
                        break;
                }



                ///tabla filtro: FechaInicial

                if (FiltroSql == string.Empty)
                {
                    FiltroSql = "where (Fecha between  convert(varchar,DATEADD(mm, -4, getdate()), 23) and convert(varchar, getdate(), 23)) ";
                }

                CadenaSql = ConsultaSql + " " + FiltroSql + " " + OrdenSql;

                var elementos = dbv.Database.SqlQuery<VRequisiciones>(CadenaSql).ToList();


                ViewBag.sumatoria = "";
                try
                {

                    var SumaLst = new List<string>();
                    var SumaQry = SumaSql + " " + FiltroSql + " " + GroupSql;
                    List<ResumenReq> data = db.Database.SqlQuery<ResumenReq>(SumaQry).ToList();
                    ViewBag.sumatoria = data;

                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                }

                //Paginación
                int count = elementos.Count();// Total number of elements
                // Populate DropDownList
                ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" , Selected = true},
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

                int pageNumber = (page ?? 1);
                int pageSize = (PageSize ?? 25);
                ViewBag.psize = pageSize;


                return View(elementos.ToPagedList(pageNumber, pageSize));
                //Paginación

            }

            catch (Exception err)
            {
                string mensaje = err.Message;

                var reshtml = Server.HtmlEncode(CadenaSql);

                return Content(reshtml);
            }
        }
        public ActionResult IndexAntes(string Divisa, string Status, string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.IDproveedorDesconocido = SIAAPI.Properties.Settings.Default.IDProveedorDesconocido;
            var SerLst = new List<string>();
            var SerQry = from d in db.c_Monedas
                         orderby d.IDMoneda
                         select d.ClaveMoneda;
            SerLst.Add(" ");
            SerLst.AddRange(SerQry.Distinct());
            ViewBag.Divisa = new SelectList(SerLst);

            var StaLst = new List<string>();
            var StaQry = from d in db.EncRequisicioness
                         orderby d.IDRequisicion
                         select d.Status;
            StaLst.Add(" ");
            StaLst.AddRange(StaQry.Distinct());
            ViewBag.Status = new SelectList(StaLst);



            var elementos = (from s in db.EncRequisicioness select s).OrderByDescending(s => s.IDRequisicion);


            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncRequisiciones where [Status]<>'Cancelado' group by EncRequisiciones.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;


            
            if (!String.IsNullOrEmpty(searchString))
            {
                elementos = (from s in db.EncRequisicioness select s).OrderByDescending(s => s.IDRequisicion);

                elementos = elementos.Where(s => s.IDRequisicion.ToString().Contains(searchString) || s.Proveedor.Empresa.Contains(searchString)).OrderByDescending(s => s.IDRequisicion);

                var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncRequisiciones inner join Proveedores on Proveedores.IDProveedor=EncRequisiciones.IDProveedor  where (CAST(EncRequisiciones.IDRequisicion AS nvarchar(max))='" + searchString + "' or Proveedores.Empresa='" + searchString + "') and [Status]<>'Cancelado' group by EncRequisiciones.IDMoneda ").ToList();
                ViewBag.sumatoria = filtro;

            }
            
            if (!String.IsNullOrEmpty(Divisa))
            {
                elementos = (from s in db.EncRequisicioness select s).OrderByDescending(s => s.IDRequisicion);
                elementos = elementos.Where(s => s.c_Moneda.ClaveMoneda == Divisa).OrderByDescending(s => s.IDRequisicion);
                var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncRequisiciones inner join c_Moneda on c_Moneda.IDMoneda=EncRequisiciones.IDMoneda  where c_Moneda.ClaveMoneda='"+Divisa+ "' and [Status]<>'Cancelado'group by EncRequisiciones.IDMoneda ").ToList();
                ViewBag.sumatoria =divisa;
                ViewBag.Divisa = Divisa;
            }

            //Filtro Status
            if (!String.IsNullOrEmpty(Status))
            {
                elementos = (from s in db.EncRequisicioness
                             select s).OrderByDescending(s => s.IDRequisicion);
                elementos = elementos.Where(s => s.Status.Equals(Status)).OrderByDescending(s => s.IDRequisicion);

                var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncRequisiciones inner join c_Moneda on c_Moneda.IDMoneda=EncRequisiciones.IDMoneda  where Status='" + Status + "' group by EncRequisiciones.IDMoneda").ToList();
                ViewBag.sumatoria = divisa;
                ViewBag.Status = Status;

            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.RequisicionSortParm = String.IsNullOrEmpty(sortOrder) ? "Requisicion" : "Requisicion";
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
                case "Requisicion":
                    elementos = elementos.OrderByDescending(s => s.IDRequisicion);
                    break;
                case "Proveedor":
                    elementos = elementos.OrderByDescending(s => s.Proveedor.Empresa);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDRequisicion);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.EncRequisicioness.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = " ", Value = " ", Selected = true },
                new SelectListItem { Text = "10", Value = "10" },
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
        public ActionResult Cambio(int? id)
        {
            decimal subtotal = 0, iva = 0, total = 0, precio = 0, importe = 0, importetotal = 0,importeiva=0 ;
            EncRequisiciones encRequisicion = db.EncRequisicioness.Find(id);

            string fecha = DateTime.Now.ToString("yyyyMMdd");

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();

            decimal Cambio = 0;
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + origen + "," + encRequisicion.IDMoneda + ") as TC").ToList()[0];
            Cambio = cambio.TC;
            
            db.Database.ExecuteSqlCommand("INSERT INTO EncOrdenCompra([Fecha],[FechaRequiere],[IDProveedor],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[UserID],[TipoCambio],[IDUsoCFDI]) SELECT [Fecha],[FechaRequiere],[IDProveedor],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[UserID],[TipoCambio],[IDUsoCFDI] FROM EncRequisiciones where IDRequisicion='" + id+"'");
            List<EncOrdenCompra> numero = db.Database.SqlQuery<EncOrdenCompra>("SELECT * FROM [dbo].[EncOrdenCompra] WHERE IDOrdenCompra = (SELECT MAX(IDOrdenCompra) from EncOrdenCompra)").ToList();
            int num = numero.Select(s => s.IDOrdenCompra).FirstOrDefault();
            //Insertar Detalle de Orden Compra
            List<DetRequisiciones> orden = db.Database.SqlQuery<DetRequisiciones>("select * from dbo.DetRequisiciones where IDRequisicion='" + id + "' and Status='Activo'").ToList();
            ViewBag.ordenc = orden;

            for (int i = 0; i < orden.Count(); i++)
            {
              
                precio = ViewBag.ordenc[i].Costo;
                importe = precio * ViewBag.ordenc[i].Cantidad;
                    importeiva = importe * (decimal)0.16;
                    importetotal = importeiva + importe;
                    db.Database.ExecuteSqlCommand("INSERT INTO DetOrdenCompra([IDOrdenCompra],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna]) values ('" + num + "','" + ViewBag.ordenc[i].IDArticulo + "','" + ViewBag.ordenc[i].Cantidad + "','" + precio + "','" + ViewBag.ordenc[i].Cantidad + "','0','" + importe + "','true','" + importeiva + "','" + importetotal + "','" + ViewBag.ordenc[i].Nota + "','0','" + ViewBag.ordenc[i].Caracteristica_ID + "','" + encRequisicion.IDAlmacen + "','0','Inactivo','" + ViewBag.ordenc[i].Presentacion + "','" + ViewBag.ordenc[i].jsonPresentacion + "','"+ ViewBag.ordenc[i].IDDetRequisiciones + "')");
                // db.Database.ExecuteSqlCommand("exec dbo.MovArt'" + fecha + "'," + ViewBag.ordenc[i].Caracteristica_ID + ",'OrdCom'," + ViewBag.ordenc[i].Cantidad + ",'OrdenCompra'," + num + ",0,'" + encRequisicion.IDAlmacen + "','" + ViewBag.ordenc[i].Nota + "',0");
                List<DetOrdenCompra> numero2 = db.Database.SqlQuery<DetOrdenCompra>("SELECT * FROM [dbo].[DetOrdenCompra] WHERE IDDetOrdenCompra = (SELECT MAX(IDDetOrdenCompra) from DetOrdenCompra)").ToList();
                int num2 = numero2.Select(s => s.IDDetOrdenCompra).FirstOrDefault();
                db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('OrdenCompra','" + num + "','" + ViewBag.ordenc[i].Cantidad + "','" + fecha + "','" + id + "','Requisición','" + UserID + "','" + ViewBag.ordenc[i].IDDetRequisiciones + "','" + num2 + "')");

            }
            //Insertar Detalle de Orden Compra
            List<DetOrdenCompra> datostotales = db.Database.SqlQuery<DetOrdenCompra>("select * from dbo.DetOrdenCompra where IDOrdenCompra='" + num + "'").ToList();
            subtotal = datostotales.Sum(s => s.Importe);
            iva = subtotal * (decimal)0.16;
            total = subtotal + iva;


            db.Database.ExecuteSqlCommand("update [dbo].[EncOrdenCompra] set [Status]='Inactivo', Fecha='"+fecha+"', [Subtotal]='" + subtotal + "',[IVA]='" + iva + "',[Total]='" + total + "' where [IDOrdenCompra]='" + num + "'");

            db.Database.ExecuteSqlCommand("update [dbo].[EncRequisiciones] set [Status]='Finalizado' where [IDRequisicion]='" +id+ "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetRequisiciones] set [Status]='Finalizado' where [IDRequisicion]='" + id + "'");

            // db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('OrdenCompra','" + num+ "','0','" + fecha + "','" + encRequisicion.IDRequisicion + "','Requisición','" + UserID + "','0','0')");


            return RedirectToAction("Index", "EncOrdenCompra");
        }

        public ActionResult Cancelar(int? id)
        {

           
            string fecha = DateTime.Now.ToString("yyyyMMdd");


            EncRequisiciones encRequisiciones = db.EncRequisicioness.Find(id);
            List<VDetRequisiciones> requisicion = db.Database.SqlQuery<VDetRequisiciones>("select DetRequisiciones.Caracteristica_ID,Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion,DetRequisiciones.Status  from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();

            //  ViewBag.req = requisicion;
            foreach (VDetRequisiciones elemrequi in requisicion)
            {
                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + elemrequi.Caracteristica_ID).ToList().FirstOrDefault();
                Articulo ar = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);

                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + ar.IDArticulo + ",'Requisición',";
                cadenam += elemrequi.Cantidad + ",'Cancelación Requisición de compra'," + id + ",'',";
                cadenam += encRequisiciones.IDAlmacen + ",'N/A',0,'',CONVERT (time, SYSDATETIMEOFFSET()))";

                db.Database.ExecuteSqlCommand(cadenam);

            }
            db.Database.ExecuteSqlCommand("update detsolicitud set [Status]='Cancelado' where NumeroDR=" + id + " and DocumentoR='Requisicion'");
            db.Database.ExecuteSqlCommand("update detrequisiciones set [Status]='Cancelado' where IDRequisicion=" + id + "");
            db.Database.ExecuteSqlCommand("update EncRequisiciones set [Status]='Cancelado' where IDRequisicion=" + id + "");

            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            //List<VDetRequisiciones> requisicion = db.Database.SqlQuery<VDetRequisiciones>("select DetRequisiciones.IDRequisicion,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal, DetRequisiciones.Nota,DetRequisiciones.Ordenado,Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "'").ToList();
            List<VDetRequisiciones> requisicion = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.Cref, Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion,DetRequisiciones.Status, Caracteristica.ID as Caracteristica_ID  from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "'").ToList();

            ViewBag.req = requisicion;


            var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncRequisiciones inner join Proveedores on Proveedores.IDProveedor=EncRequisiciones.IDProveedor  where EncRequisiciones.IDRequisicion='" + id+ "' group by EncRequisiciones.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EncRequisiciones encRequisiciones = db.EncRequisicioness.Find(id);
            if (encRequisiciones == null)
            {
                return HttpNotFound();
            }
            return View(encRequisiciones);
        }

   

        [HttpPost]

        public ActionResult MonedaC(int? idmoneda, int? idprov)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            var requisicion = db.Database.SqlQuery<VCarrito>("select (CarritoC.Precio * (select dbo.GetTipocambio(GETDATE(),CarritoC.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * CarritoC.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoC.IDCarrito,CarritoC.usuario,CarritoC.IDCaracteristica,CarritoC.Precio,CarritoC.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoC.Precio * CarritoC.Cantidad as Importe, CarritoC.Nota from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            ViewBag.carrito = requisicion;


            string fecha = DateTime.Now.ToString("yyyyMMdd");
            for (int i = 0; i < requisicion.Count(); i++)
            {

                db.Database.ExecuteSqlCommand("update [dbo].[CarritoC] set  [IDMoneda]='" + idmoneda + "', [Precio]='" + ViewBag.carrito[i].Precio + "' * dbo.GetTipocambio('" + fecha + "'," + ViewBag.carrito[i].IDMoneda + "," + idmoneda + ") where IDCarrito ='" + ViewBag.carrito[i].IDCarrito + "' and usuario='" + usuario + "'");

            }
            //return RedirectToAction("Create", new { idprov = idprov });
            return Json(true);

        }
        // GET: EncRequisiones/Create
        public ActionResult Create()
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            CarritoContext car = new CarritoContext();
            ClsDatoEntero cambio = db.Database.SqlQuery<ClsDatoEntero>("select distinct IDProveedor as Dato from CarritoC where usuario=" + usuario + "").ToList()[0];

            List<SelectListItem> li = new List<SelectListItem>();
            Proveedor mm = prov.Proveedores.Find(cambio.Dato);
            li.Add(new SelectListItem { Text = mm.Empresa, Value = mm.IDProveedor.ToString() });
            ViewBag.proveedor = li;

            Proveedor proveedor = prov.Proveedores.Find(cambio.Dato);
            ClsDatoEntero monedacarrito = db.Database.SqlQuery<ClsDatoEntero>("select distinct IDMoneda as Dato from CarritoC where usuario=" + usuario + "").ToList()[0];

            ViewBag.IDMoneda = new SelectList(prov.c_Monedas, "IDMoneda", "Descripcion", monedacarrito.Dato);




            //Proveedor proveedor = prov.Proveedores.Find(cambio.Dato);
            //List<SelectListItem> moneda = new List<SelectListItem>();

            //ClsDatoEntero monedacarrito = db.Database.SqlQuery<ClsDatoEntero>("select distinct IDMoneda as Dato from CarritoC where usuario=" + usuario + "").ToList()[0];
            //c_Moneda monedap = prov.c_Monedas.Find(monedacarrito.Dato);
            //moneda.Add(new SelectListItem { Text = monedap.Descripcion, Value = monedap.IDMoneda.ToString() });
            //moneda.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            //var todosmoneda = prov.c_Monedas.ToList();
            //if (todosmoneda != null)
            //{
            //    foreach (var y in todosmoneda)
            //    {
            //        moneda.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDMoneda.ToString() });
            //    }
            //}

            //ViewBag.moneda = moneda;

            List<SelectListItem> metodo = new List<SelectListItem>();
            c_MetodoPago metodop = prov.c_MetodoPagos.Find(proveedor.IDMetodoPago);
            metodo.Add(new SelectListItem { Text = metodop.Descripcion, Value = metodop.IDMetodoPago.ToString() });
            metodo.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosmetodo = prov.c_MetodoPagos.ToList();
            if (todosmetodo != null)
            {
                foreach (var y in todosmetodo)
                {
                    metodo.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDMetodoPago.ToString() });
                }
            }

            ViewBag.metodo = metodo;

            List<SelectListItem> forma = new List<SelectListItem>();
            c_FormaPago formap = prov.c_FormaPagos.Find(proveedor.IDFormaPago);
            forma.Add(new SelectListItem { Text = formap.Descripcion, Value = formap.IDFormaPago.ToString() });
            forma.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosforma = prov.c_FormaPagos.ToList();
            if (todosforma != null)
            {
                foreach (var y in todosforma)
                {
                    forma.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDFormaPago.ToString() });
                }
            }

            ViewBag.forma = forma;

            List<SelectListItem> condiciones = new List<SelectListItem>();
            CondicionesPago condicionesp = prov.CondicionesPagos.Find(proveedor.IDCondicionesPago);
            condiciones.Add(new SelectListItem { Text = condicionesp.Descripcion, Value = condicionesp.IDCondicionesPago.ToString() });
            condiciones.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todoscondiciones = prov.CondicionesPagos.ToList();
            if (todoscondiciones != null)
            {
                foreach (var y in todoscondiciones)
                {
                    condiciones.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDCondicionesPago.ToString() });
                }
            }

            ViewBag.condiciones = condiciones;


            ViewBag.IDAlmacen = new SelectList(db.Almacenes, "IDAlmacen", "Descripcion");
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS, " IDUsoCFDI", "Descripcion");


            List<VCarrito> requisicion = db.Database.SqlQuery<VCarrito>("select (CarritoC.Precio * (select dbo.GetTipocambio(GETDATE(),CarritoC.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * CarritoC.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoC.IDCarrito,CarritoC.usuario,CarritoC.IDCaracteristica,CarritoC.Precio,CarritoC.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoC.Precio * CarritoC.Cantidad as Importe, CarritoC.Nota from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=CarritoC.IDMoneda) as MonedaOrigen, (select SUM(CarritoC.Precio * CarritoC.Cantidad)) as Subtotal, SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16 as IVA, (SUM(CarritoC.Precio * CarritoC.Cantidad)) + (SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16) as Total ,0 as TotalPesos from CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "' group by CarritoC.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;

            ViewBag.carrito = requisicion;
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarrito) as Dato from CarritoC where  usuario='" + usuario + "'").ToList()[0];
            int x = c.Dato;
            ViewBag.dato = x;

            ClsDatoEntero cantidad = db.Database.SqlQuery<ClsDatoEntero>("select count(Cantidad) as Dato from CarritoC where Cantidad=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datocantidad = cantidad.Dato;

            ClsDatoEntero preciocontar = db.Database.SqlQuery<ClsDatoEntero>("select count(Precio) as Dato from CarritoC where Precio=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datoprecio = preciocontar.Dato;
            //Termina la consulta del carrito
            return View();
        }

        // POST: EncRequisiones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EncRequisiciones encRequisiciones)
        {
            //Datos para ingresar en Requisicion 
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0, Cambio = 0, Precio = 0, subtotalaux = 0;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<VCarrito> requisicion = db.Database.SqlQuery<VCarrito>("select (CarritoC.Precio * (select dbo.GetTipocambio(GETDATE(),CarritoC.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * CarritoC.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoC.IDCarrito,CarritoC.usuario,CarritoC.IDCaracteristica,CarritoC.Precio,CarritoC.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoC.Precio * CarritoC.Cantidad as Importe, CarritoC.Nota from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=CarritoC.IDMoneda) as MonedaOrigen, (select SUM(CarritoC.Precio * CarritoC.Cantidad)) as Subtotal, SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16 as IVA, (SUM(CarritoC.Precio * CarritoC.Cantidad)) + (SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16) as Total ,0 as TotalPesos from CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "' group by CarritoC.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;
            //try
            //{
            //    subtotal = requisicion.Sum(s => s.Importe);
            //    iva = subtotal * (decimal)0.16;
            //    total = subtotal + iva;
            //}
            //catch (Exception e)
            //{

            //}
            //ViewBag.Subtotal = subtotal.ToString("C");
            //ViewBag.IVA = iva.ToString("C");
            //ViewBag.Total = total.ToString("C");
            ViewBag.carrito = requisicion;
            //Termina 

            if (ModelState.IsValid)
            {
                int num = 0;
                DateTime fecha = encRequisiciones.Fecha;
                string fecha1 = fecha.ToString("yyyy/MM/dd");

                DateTime fechareq = encRequisiciones.FechaRequiere;
                string fecha2 = fechareq.ToString("yyyy/MM/dd");

                List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();

                //VCambio cambio= db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + origen + "," + encRequisiciones.IDMoneda + ") as TC").ToList()[0];
                //Cambio = cambio.TC;

                VCambio tcambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + encRequisiciones.IDMoneda + "," + origen + ") as TC").ToList()[0];
                decimal tCambio = tcambio.TC;


                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncRequisiciones]([Fecha],[FechaRequiere],[IDProveedor],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[TipoCambio],[UserID],[IDUsoCFDI]) values ('" + fecha1 + "','" + fecha2 + "','" + encRequisiciones.IDProveedor + "','" + encRequisiciones.IDFormapago + "','" + encRequisiciones.IDMoneda + "','" + encRequisiciones.Observacion + "','" + subtotal + "','" + iva + "','" + total + "','" + encRequisiciones.IDMetodoPago + "','" + encRequisiciones.IDCondicionesPago + "','" + encRequisiciones.IDAlmacen + "','Activo','" + tCambio + "','" + usuario + "','" + encRequisiciones.IDUsoCFDI + "')");
                db.SaveChanges();

                List<EncRequisiciones> numero;
                numero = db.Database.SqlQuery<EncRequisiciones>("SELECT * FROM [dbo].[EncRequisiciones] WHERE IDRequisicion = (SELECT MAX(IDRequisicion) from EncRequisiciones)").ToList();
                num = numero.Select(s => s.IDRequisicion).FirstOrDefault();

                for (int i = 0; i < requisicion.Count(); i++)
                {
                    //VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encRequisiciones.IDMoneda + ") as TC").ToList()[0];
                    //Cambio = cambio.TC;
                    Precio = ViewBag.carrito[i].Precio;
                    importe = ViewBag.carrito[i].Cantidad * Precio;
                    importeiva = importe * (decimal)0.16;
                    importetotal = importeiva + importe;
                    db.Database.ExecuteSqlCommand("INSERT INTO DetRequisiciones([IDRequisicion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion]) values ('" + num + "','" + ViewBag.carrito[i].IDArticulo + "','" + ViewBag.carrito[i].Cantidad + "','" + Precio + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encRequisiciones.IDMoneda + "),'" + ViewBag.carrito[i].Cantidad + "','0','" + importe + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encRequisiciones.IDMoneda + "),'true','" + importeiva + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encRequisiciones.IDMoneda + "),'" + importetotal + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encRequisiciones.IDMoneda + "),'" + ViewBag.carrito[i].Nota + "','0','" + ViewBag.carrito[i].IDCaracteristica + "','" + encRequisiciones.IDAlmacen + "','0','Activo','" + ViewBag.carrito[i].Presentacion + "','" + ViewBag.carrito[i].jsonPresentacion + "')");
                    db.Database.ExecuteSqlCommand("exec dbo.MovArt'" + fecha1 + "'," + ViewBag.carrito[i].IDCaracteristica + ",'ReqCom'," + ViewBag.carrito[i].Cantidad + ",'Requisicion'," + num + ",0,'" + encRequisiciones.IDAlmacen + "','" + ViewBag.carrito[i].Nota + "',0");
                    db.Database.ExecuteSqlCommand("delete from CarritoC where IDCarrito='" + ViewBag.carrito[i].IDCarrito + "'");
                    db.SaveChanges();

                }
                List<DetRequisiciones> datostotales = db.Database.SqlQuery<DetRequisiciones>("select * from dbo.DetRequisiciones where IDRequisicion='" + num + "'").ToList();
                subtotalr = datostotales.Sum(s => s.Importe);
                ivar = subtotalr * (decimal)0.16;
                totalr = subtotalr + ivar;
                db.Database.ExecuteSqlCommand("update [dbo].[EncRequisiciones] set [Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "' where [IDRequisicion]='" + num + "'");

                return RedirectToAction("Index");

            }


            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", encRequisiciones.IDFormapago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", encRequisiciones.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", encRequisiciones.IDMoneda);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", encRequisiciones.IDCondicionesPago);
            ViewBag.IDProveedor = new SelectList(db.Proveedor, "IDProveedor", "Empresa", encRequisiciones.IDProveedor);
            ViewBag.IDAlmacen = new SelectList(db.Almacenes, "IDAlmacen", "Descripcion", encRequisiciones.IDProveedor);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS, "IDUsoCFDI", "Descripcion");

            return View(encRequisiciones);

        }

        public void PdfRequisicion(int id)
        {
            EncRequisiciones requisicion = db.EncRequisicioness.Find(id);
            DocumentoRequisicion x = new DocumentoRequisicion();

            x.claveMoneda = requisicion.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = requisicion.Fecha.ToShortDateString();
            x.fechaRequerida = requisicion.FechaRequiere.ToShortDateString();
            x.Proveedor = requisicion.Proveedor.Empresa;
            x.formaPago = requisicion.c_FormaPago.ClaveFormaPago;
            x.metodoPago = requisicion.c_MetodoPago.ClaveMetodoPago;
            x.RFCproveedor = requisicion.Proveedor.Telefonouno;
            x.total = float.Parse(requisicion.Total.ToString());
            x.subtotal = float.Parse(requisicion.Subtotal.ToString());
            x.tipo_cambio = requisicion.TipoCambio.ToString();
            x.serie = "";
            x.folio = requisicion.IDRequisicion.ToString();
            x.UsodelCFDI = requisicion.c_UsoCFDI.Descripcion;
            x.IDALmacen = requisicion.Almacen.IDAlmacen + " " + requisicion.Almacen.Descripcion;
            x.Telefonoproveedor = requisicion.Proveedor.Telefonouno;
            ImpuestoRequisicion iva = new ImpuestoRequisicion();
            iva.impuesto = "IVA";
            iva.tasa = 16;
            iva.importe = float.Parse(requisicion.IVA.ToString());


            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;


            List<DetRequisiciones> detalles = db.Database.SqlQuery<DetRequisiciones>("select * from DetRequisiciones where IDRequisicion= " + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoRequisicion producto = new ProductoRequisicion();
                Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);

                c_ClaveProductoServicio claveprodsat = db.Database.SqlQuery<c_ClaveProductoServicio>("select c_ClaveProductoServicio.* from (Articulo inner join Familia on articulo.IDFamilia= Familia.IDFamilia) inner join c_ClaveProductoServicio on c_ClaveProductoServicio.IDProdServ= Familia.IDProdServ where Articulo.IDArticulo= " + item.IDArticulo).ToList()[0];
                producto.ClaveProducto = claveprodsat.ClaveProdServ;

                producto.c_unidad = arti.c_ClaveUnidad.ClaveUnidad;
                producto.cantidad = item.Cantidad.ToString();
                producto.descripcion = arti.Descripcion;
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

            //try
            //{


            CreaRequisicionPDF documento = new CreaRequisicionPDF(logoempresa, x);
           


            //}
            //catch (Exception err)
            //{

            //}

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
      
     

        public ActionResult EntreFechasReq()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasReq(EFecha modelo, FormCollection coleccion)
        {
            VRequisicionesContext dbr = new VRequisicionesContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");

            string cadena = "";
            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {

                cadena = "select * from [dbo].[VRequisicion] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' ";
                var datos = dbr.Database.SqlQuery<VRequisiciones>(cadena).ToList();
                ViewBag.datos = datos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Requisición");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:P1"].Style.Font.Size = 20;
                Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:P3"].Style.Font.Bold = true;
                Sheet.Cells["A1:P1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:P1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Requisiciones");

                row = 2;
                Sheet.Cells["A1:O1"].Style.Font.Size = 12;
                Sheet.Cells["A1:O1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:O1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:O2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3:O3"].Style.Font.Bold = true;
                Sheet.Cells["A3:O3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:O3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID Requisición");
                Sheet.Cells["B3"].RichText.Add("Fecha"); ;
                Sheet.Cells["C3"].RichText.Add("Fecha Requerida");
                Sheet.Cells["D3"].RichText.Add("RFC");
                Sheet.Cells["E3"].RichText.Add("Empresa");
                Sheet.Cells["F3"].RichText.Add("Subtotal");
                Sheet.Cells["G3"].RichText.Add("Iva");
                Sheet.Cells["H3"].RichText.Add("Total");
                Sheet.Cells["I3"].RichText.Add("Moneda");
                Sheet.Cells["J3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["K3"].RichText.Add("Total en Pesos");
                Sheet.Cells["L3"].RichText.Add("Status");
                Sheet.Cells["M3"].RichText.Add("Almacen");
                Sheet.Cells["N3"].RichText.Add("Observación");
                Sheet.Cells["O3"].RichText.Add("Genero");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:O3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VRequisiciones item in ViewBag.datos)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDRequisicion;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.FechaRequiere;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.RFC;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Empresa;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.TotalPesos;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Status;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Almacen;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.Observacion;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.Username;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Requisiciones.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }

    }
}