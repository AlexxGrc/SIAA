﻿using PagedList;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.Reportes;
using System.IO;

using SIAAPI.Models.Cfdi;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Globalization;
using System.Web.Helpers;
using System.Collections;

namespace SIAAPI.Controllers.Comercial
{
    public class EncOrdenCompraController : Controller
    {
        private OrdenCompraContext db = new OrdenCompraContext();
        private ProveedorContext prov = new ProveedorContext();
        [Authorize(Roles = "Administrador,Facturacion,Gerencia,Sistemas,Compras,Comercial,Almacenista,Diseno, Logistica,Proveedor,Calidad, GestionCalidad")]
        public ActionResult Index(string Divisa, string Status, string sortOrder, string Fechainicio, string Fechafinal, string currentFilter, string searchString, string Proveedor, int? page, int? PageSize)
        {
            //string Consultaselect = "SELECT EncOrdenCompra.* ";
            string consultasql = "  select top 1000* from VOrdenCompra  ";

            string filtro = string.Empty; // comienza sin ningun fitro
            string orden = string.Empty;
            string SumaSql = "select ClaveMoneda as MonedaOrigen, Sum(Subtotal) as Subtotal, Sum(IVA) as IVA, Sum(Total) as Total, sum(TotalPesos) as TotalenPesos from dbo.VOrdenCompra";
            string FiltroSuma = "and EstadoOC != 'Cancelado'";
            string GroupSql = "group by ClaveMoneda";
            string CadenaSql = string.Empty;
            string CadenaResumenSql = string.Empty;

            VOrdenCompraContext dbv = new VOrdenCompraContext();
            if (Session["Proveedor"] != null)
            {
                RedirectToAction("Indexp");
            }

            //var proveedores = new ProveedorAllRepository().GetProveedor();

            ///filtro: Proveedor
            var ProLst = new List<string>();
            var ProQry = from d in prov.Proveedores
                         orderby d.IDProveedor
                         select d.Empresa;
            ProLst.Add(" ");
            ProLst.AddRange(ProQry.Distinct());
            ViewBag.Proveedor = new SelectList(ProLst);
            ViewBag.ProveedorSeleccionado = Proveedor;



            var SerLst = new List<string>();
            var SerQry = from d in db.c_Monedas
                         orderby d.IDMoneda
                         select d.ClaveMoneda;
            SerLst.Add(" ");
            SerLst.AddRange(SerQry.Distinct());
            ViewBag.Divisa = new SelectList(SerLst);
            ViewBag.DivisaSeleccionado = Divisa;


            var StaLst = new List<string>();
            var StaQry = from d in db.EncOrdenCompras
                         orderby d.IDOrdenCompra
                         select d.Status;
            StaLst.Add(" ");
            StaLst.AddRange(StaQry.Distinct());
            ViewBag.Status = new SelectList(StaLst);
            ViewBag.StatusSeleccionado = Status;




            if (searchString == " ")
            {
                searchString = string.Empty;
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = " where IDOrdenCompra = " + searchString;
                }
                else
                {
                    filtro += " and IDOrdenCompra = " + searchString;
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
                    filtro = " where EstadoOC = '" + Status + "'";
                    FiltroSuma = "";
                }
                else
                {
                    filtro += " and EstadoOC = '" + Status + "'";
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
                    orden = " order by IDOrdenCompra Desc";
                    break;
                case "Proveedor":
                    orden = " order by IDProveedor ";
                    break;
                default:
                    orden = " order by IDOrdenCompra Desc";
                    break;
            }

            ///tabla filtro: FechaInicial

            //if (filtro == string.Empty)
            //{
            //    filtro = "where (Fecha between  convert(varchar,DATEADD(mm, -4, getdate()), 23) and convert(varchar, getdate(), 23)) ";
            //}


            string cadena = consultasql + " " + filtro + " " + orden;
            dbv.Database.CommandTimeout = 300;

            List<VOrdenCompra> elementos = dbv.Database.SqlQuery<VOrdenCompra>(cadena).ToList();



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
            ViewBag.OrdenCompraSortParm = String.IsNullOrEmpty(sortOrder) ? "OrdenCompra" : "OrdenCompra";
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
            int count = dbv.VOrdenCompras.Count(); // Total number of elements


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
            List<VDetOrdenCompra> orden = db.Database.SqlQuery<VDetOrdenCompra>("select DetOrdenCompra.IDDetExterna,Articulo.MinimoCompra,DetOrdenCompra.IDOrdenCompra,DetOrdenCompra.Suministro,Articulo.Descripcion as Articulo,DetOrdenCompra.Cantidad,DetOrdenCompra.Costo,DetOrdenCompra.CantidadPedida,DetOrdenCompra.Descuento,DetOrdenCompra.Importe,DetOrdenCompra.IVA,DetOrdenCompra.ImporteIva,DetOrdenCompra.ImporteTotal, DetOrdenCompra.Nota,DetOrdenCompra.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetOrdenCompra.Status, DetOrdenCompra.Caracteristica_ID as Caracteristica_ID from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncOrdenCompra.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncOrdenCompra inner join Proveedores on Proveedores.IDProveedor=EncOrdenCompra.IDProveedor  where EncOrdenCompra.IDOrdenCompra='" + id + "' group by EncOrdenCompra.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(id);
            if (encOrdenCompra == null)
            {
                return HttpNotFound();
            }
            return View(encOrdenCompra);
        }
        public ActionResult AutorizarC(int? id)
        {

            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(id);
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            List<User> userid;
            userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            List<DetOrdenCompra> orden = db.Database.SqlQuery<DetOrdenCompra>("select DetOrdenCompra.IDDetOrdenCompra, DetOrdenCompra.IDDetExterna,DetOrdenCompra.IDOrdenCompra,DetOrdenCompra.IDArticulo,DetOrdenCompra.Caracteristica_ID,DetOrdenCompra.Costo,DetOrdenCompra.CantidadPedida,DetOrdenCompra.Cantidad,DetOrdenCompra.Descuento,DetOrdenCompra.IDAlmacen,DetOrdenCompra.Importe,DetOrdenCompra.IVA,DetOrdenCompra.ImporteIva,DetOrdenCompra.ImporteTotal,DetOrdenCompra.Suministro,DetOrdenCompra.Nota,DetOrdenCompra.Status,DetOrdenCompra.Ordenado,Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "'").ToList();
            ViewBag.ordenes = orden;




            try
            {
                string insertar = "Insert into AutorizacionesOC (IDOrdenC,TipoAutorizacion,Usuario,Fecha) values(" + id + ",'Calidad'," + UserID + ",sysdatetime())";
                db.Database.ExecuteSqlCommand(insertar);
            }
            catch (Exception err)
            {

            }

            return RedirectToAction("Index");
        }

        public ActionResult Autorizar(int? id)
        {

            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(id);
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            List<User> userid;
            userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            List<DetOrdenCompra> orden = db.Database.SqlQuery<DetOrdenCompra>("select DetOrdenCompra.IDDetOrdenCompra, DetOrdenCompra.IDDetExterna,DetOrdenCompra.IDOrdenCompra,DetOrdenCompra.IDArticulo,DetOrdenCompra.Caracteristica_ID,DetOrdenCompra.Costo,DetOrdenCompra.CantidadPedida,DetOrdenCompra.Cantidad,DetOrdenCompra.Descuento,DetOrdenCompra.IDAlmacen,DetOrdenCompra.Importe,DetOrdenCompra.IVA,DetOrdenCompra.ImporteIva,DetOrdenCompra.ImporteTotal,DetOrdenCompra.Suministro,DetOrdenCompra.Nota,DetOrdenCompra.Status,DetOrdenCompra.Ordenado,Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "'").ToList();
            ViewBag.ordenes = orden;


            for (int i = 0; i < orden.Count(); i++)
            {
                int idc = ViewBag.ordenes[i].Caracteristica_ID;
                decimal cantidad = ViewBag.ordenes[i].Cantidad;
                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.Where(s => s.IDAlmacen == encOrdenCompra.IDAlmacen && s.IDCaracteristica == idc).FirstOrDefault();
                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + idc).FirstOrDefault();
                if (inv == null)
                {

                    db.Database.ExecuteSqlCommand("INSERT INTO dbo.InventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) VALUES(" + encOrdenCompra.IDAlmacen + "," + cara.Articulo_IDArticulo + "," + cara.ID + ", 0, " + cantidad + ", 0, 0); ");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen set porllegar=(porllegar+" + cantidad + ") WHERE IDAlmacen =" + encOrdenCompra.IDAlmacen + " and IDCaracteristica = " + idc);
                }

                 decimal existencia = 0;
                try
                {
                    existencia = inv.Existencia;
                }
                catch (Exception err)
                {


                }
                InventarioAlmacen inv1 = new InventarioAlmacenContext().InventarioAlmacenes.Where(s => s.IDAlmacen == encOrdenCompra.IDAlmacen && s.IDCaracteristica == idc).FirstOrDefault();

                db.Database.ExecuteSqlCommand("INSERT INTO dbo.MovimientoArticulo(Fecha, Id, IDPresentacion, Articulo_IDArticulo, Accion, cantidad, Documento, NoDocumento, Lote, IDAlmacen, TipoOperacion, acumulado, observacion, Hora) values (getdate(), " + idc + "," + cara.IDPresentacion + "," + cara.Articulo_IDArticulo + ", 'Orden de Compra'," + cantidad + ", 'Orden de Compra'," + id + ",''," + encOrdenCompra.IDAlmacen + ", 'NA'," + inv1.Existencia + ",'', SYSDATETIMEOFFSET()); ");

            }



            db.Database.ExecuteSqlCommand("update [dbo].[EncOrdenCompra] set [Status]='Activo' where [IDOrdenCompra]='" + id + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetOrdenCompra] set [Status]='Activo' where [IDOrdenCompra]='" + id + "'");
            db.Database.ExecuteSqlCommand("insert into [dbo].[MovAutorizacion] ([Documento],[IDDocumento],[Fecha],[UserID]) values('OrdenCompra','" + encOrdenCompra.IDOrdenCompra + "','" + fecha + "','" + UserID + "')");


            try
            {
                string insertar = "Insert into AutorizacionesOC (IDOrdenC,TipoAutorizacion,Usuario,Fecha) values(" + id + ",'OC'," + UserID + ",sysdatetime())";
                db.Database.ExecuteSqlCommand(insertar);
            }
            catch (Exception err)
            {

            }
            return RedirectToAction("Index");
        }
        //public ActionResult Autorizar(int? id)
        //{

        //    EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(id);
        //    string fecha = DateTime.Now.ToString("yyyyMMdd");
        //    List<User> userid;
        //    userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
        //    int UserID = userid.Select(s => s.UserID).FirstOrDefault();
        //    List<DetOrdenCompra> orden = db.Database.SqlQuery<DetOrdenCompra>("select DetOrdenCompra.IDDetOrdenCompra, DetOrdenCompra.IDDetExterna,DetOrdenCompra.IDOrdenCompra,DetOrdenCompra.IDArticulo,DetOrdenCompra.Caracteristica_ID,DetOrdenCompra.Costo,DetOrdenCompra.CantidadPedida,DetOrdenCompra.Cantidad,DetOrdenCompra.Descuento,DetOrdenCompra.IDAlmacen,DetOrdenCompra.Importe,DetOrdenCompra.IVA,DetOrdenCompra.ImporteIva,DetOrdenCompra.ImporteTotal,DetOrdenCompra.Suministro,DetOrdenCompra.Nota,DetOrdenCompra.Status,DetOrdenCompra.Ordenado,Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "'").ToList();
        //    ViewBag.ordenes = orden;


        //    for (int i = 0; i < orden.Count(); i++)
        //    {
        //        int idc = ViewBag.ordenes[i].Caracteristica_ID;
        //        decimal cantidad = ViewBag.ordenes[i].Cantidad;
        //        InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.Where(s => s.IDAlmacen == encOrdenCompra.IDAlmacen && s.IDCaracteristica == idc).FirstOrDefault();
        //        Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + idc).FirstOrDefault();
        //        if (inv == null)
        //        {

        //            db.Database.ExecuteSqlCommand("INSERT INTO dbo.InventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) VALUES(" + encOrdenCompra.IDAlmacen + "," + cara.Articulo_IDArticulo + "," + cara.ID + ", 0, " + cantidad + ", 0, 0); ");
        //        }
        //        else
        //        {
        //            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen set porllegar=porllegar+" + cantidad + " WHERE IDAlmacen =" + encOrdenCompra.IDAlmacen + " and IDCaracteristica = " + idc);
        //        }

        //        //  db.Database.ExecuteSqlCommand("exec dbo.MovArt'" + fecha + "'," + ViewBag.ordenes[i].Caracteristica_ID + ",'OrdCom'," + + ",'OrdenCompra'," + id + ",0,'" + encOrdenCompra.IDAlmacen + "','" + ViewBag.ordenes[i].Nota + "',1");
        //        decimal existencia = 0;
        //        try
        //        {
        //            existencia = inv.Existencia;
        //        }
        //        catch(Exception err)
        //        {

        //        }

        //        db.Database.ExecuteSqlCommand("INSERT INTO dbo.MovimientoArticulo(Fecha, Id, IDPresentacion, Articulo_IDArticulo, Accion, cantidad, Documento, NoDocumento, Lote, IDAlmacen, TipoOperacion, acumulado, observacion, Hora) values (getdate(), " + idc + "," + cara.IDPresentacion + "," + cara.Articulo_IDArticulo + ", 'Orden de Compra'," + cantidad + ", 'Orden de Compra'," + id + ",''," + encOrdenCompra.IDAlmacen + ", 'NA'," + (existencia + cantidad) + ",'', SYSDATETIMEOFFSET()); ");

        //    }



        //    db.Database.ExecuteSqlCommand("update [dbo].[EncOrdenCompra] set [Status]='Activo' where [IDOrdenCompra]='" + id + "'");
        //    db.Database.ExecuteSqlCommand("update [dbo].[DetOrdenCompra] set [Status]='Activo' where [IDOrdenCompra]='" + id + "'");
        //    db.Database.ExecuteSqlCommand("insert into [dbo].[MovAutorizacion] ([Documento],[IDDocumento],[Fecha],[UserID]) values('OrdenCompra','" + encOrdenCompra.IDOrdenCompra + "','" + fecha + "','" + UserID + "')");
        //    return RedirectToAction("Index");
        //}
        public ActionResult FinalizarOrden(int? id)
        {

            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(id);
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            List<User> userid;
            userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            List<DetOrdenCompra> orden = db.Database.SqlQuery<DetOrdenCompra>("select DetOrdenCompra.* from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "' and Status='Activo'").ToList();
            ViewBag.ordenes = orden;
            for (int i = 0; i < orden.Count(); i++)
            {
                DetOrdenCompra detorden = db.DetOrdenCompras.Find(ViewBag.ordenes[i].IDDetOrdenCompra);


                int IDArticylo = ViewBag.ordenes[i].IDArticulo;
                int IDCaraterystuca = ViewBag.ordenes[i].Caracteristica_ID;
                ClsDatoEntero existe = new InventarioAlmacenContext().Database.SqlQuery<ClsDatoEntero>("select count(IDInventarioAlmacen) as Dato from dbo.InventarioAlmacen WHERE IDAlmacen=" + detorden.IDAlmacen + " and IDCaracteristica=" + IDCaraterystuca + " and IDArticulo=" + IDArticylo).FirstOrDefault();
                if (existe.Dato != 0)
                {
                    ClsDatoDecimal porllegar = db.Database.SqlQuery<ClsDatoDecimal>("select PorLlegar as Dato from dbo.InventarioAlmacen WHERE IDAlmacen=" + detorden.IDAlmacen + " and IDCaracteristica=" + IDCaraterystuca + " and IDArticulo=" + IDArticylo + "").FirstOrDefault();

                    decimal cantidadf = (detorden.Cantidad - detorden.Suministro);
                    decimal PorLlegar = porllegar.Dato - cantidadf;
                    if (PorLlegar < 0)
                    { PorLlegar = 0; }
                    db.Database.ExecuteSqlCommand("update  dbo.InventarioAlmacen set Porllegar=" + PorLlegar + "  WHERE IDAlmacen=" + detorden.IDAlmacen + " and IDCaracteristica=" + IDCaraterystuca + " and IDArticulo=" + IDArticylo + "");


                    try
                    {
                        InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == detorden.IDAlmacen && s.IDCaracteristica == IDCaraterystuca).FirstOrDefault();

                        Caracteristica carateristica = new InventarioAlmacenContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + IDCaraterystuca).ToList().FirstOrDefault();

                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Finalización Órden de Compra'," + cantidadf + ",'Órden de Compra'," + detorden.IDOrdenCompra + ",''," + detorden.IDAlmacen + ",'NA'," + ia.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {

                    }
                    //db.Database.ExecuteSqlCommand("update [dbo].[DetOrdenCompra] set [Status]='Finalizado' where [IDDetOrdenCompra]=" + ViewBag.ordenes[i].IDDetOrdenCompra + "");
                }
                db.Database.ExecuteSqlCommand("update [dbo].[DetOrdenCompra] set [Status]='Finalizado' where [IDOrdenCompra]=" + id + "");
            }

            try
            {
                string cadenam = "INSERT INTO [dbo].[ReporteFinalizaciones] ([IDDocumento], [Documento], [IDUsuario], [Fecha]) VALUES ";
                cadenam += " (" + id + ", 'OrdenCompra', " + UserID + ", GETDATE())";
                db.Database.ExecuteSqlCommand(cadenam);
            }
            catch (Exception err)
            {
                string mensajee = err.Message;
            }

            db.Database.ExecuteSqlCommand("update [dbo].[EncOrdenCompra] set [Status]='Finalizado' where [IDOrdenCompra]='" + id + "'");
            return RedirectToAction("Index");
        }
        [HttpPost]

        public ActionResult MonedaC(int? idmoneda, int? idprov)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            var orden = db.Database.SqlQuery<VCarrito>("select (CarritoC.Precio * (select dbo.GetTipocambio(GETDATE(),CarritoC.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * CarritoC.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoC.IDCarrito,CarritoC.usuario,CarritoC.IDCaracteristica,CarritoC.Precio,CarritoC.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoC.Precio * CarritoC.Cantidad as Importe, CarritoC.Nota from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            ViewBag.carrito = orden;


            string fecha = DateTime.Now.ToString("yyyyMMdd");
            for (int i = 0; i < orden.Count(); i++)
            {

                db.Database.ExecuteSqlCommand("update [dbo].[CarritoC] set  [IDMoneda]='" + idmoneda + "', [Precio]='" + ViewBag.carrito[i].Precio + "' * dbo.GetTipocambio('" + fecha + "'," + ViewBag.carrito[i].IDMoneda + "," + idmoneda + ") where IDCarrito ='" + ViewBag.carrito[i].IDCarrito + "' and usuario='" + usuario + "'");

            }
            //return RedirectToAction("Create", new { idprov = idprov });
            return Json(true);

        }
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
            List<SelectListItem> moneda = new List<SelectListItem>();

            ClsDatoEntero monedacarrito = db.Database.SqlQuery<ClsDatoEntero>("select distinct IDMoneda as Dato from CarritoC where usuario=" + usuario + "").ToList()[0];
            ViewBag.IDMoneda = new SelectList(prov.c_Monedas, "IDMoneda", "Descripcion", monedacarrito.Dato);

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

            ViewBag.moneda = ViewBag.IDMoneda;

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
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS, "IDUsoCFDI", "Descripcion", 3);
            List<VCarrito> orden = db.Database.SqlQuery<VCarrito>("select (CarritoC.Precio * (select dbo.GetTipocambio(GETDATE(),CarritoC.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * CarritoC.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,Articulo.Cref, Caracteristica.IDPresentacion as IDPresentacion,c_ClaveUnidad.Nombre as Unidad,CarritoC.IDCarrito,CarritoC.usuario,CarritoC.IDCaracteristica,CarritoC.Precio,CarritoC.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoC.Precio * CarritoC.Cantidad as Importe, CarritoC.Nota from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=CarritoC.IDMoneda) as MonedaOrigen, (select SUM(CarritoC.Precio * CarritoC.Cantidad)) as Subtotal, SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16 as IVA, (SUM(CarritoC.Precio * CarritoC.Cantidad)) + (SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16) as Total ,0 as TotalPesos from CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "' group by CarritoC.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;

            ViewBag.carrito = orden;

            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarrito) as Dato from CarritoC where  usuario='" + usuario + "'").ToList()[0];
            int x = c.Dato;
            ViewBag.dato = x;

            ClsDatoEntero cantidad = db.Database.SqlQuery<ClsDatoEntero>("select count(Cantidad) as Dato from CarritoC where Cantidad=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datocantidad = cantidad.Dato;

            ClsDatoEntero precio = db.Database.SqlQuery<ClsDatoEntero>("select count(Precio) as Dato from CarritoC where Precio=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datoprecio = precio.Dato;
            //Termina la consulta del carrito
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EncOrdenCompra encOrdenCompra)
        {
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0, Precio = 0;

            Proveedor proveedor = new ProveedorContext().Proveedores.Find(encOrdenCompra.IDProveedor);

            decimal montoiva = proveedor.c_TiposIVA.Tasa;  // new c_TipoIVAContext().c_TiposIVA.Find(proveedor.)

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<VCarrito> orden = db.Database.SqlQuery<VCarrito>("select (CarritoC.Precio * (select dbo.GetTipocambio(GETDATE(),CarritoC.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * CarritoC.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoC.IDCarrito,CarritoC.usuario,CarritoC.IDCaracteristica,CarritoC.Precio,CarritoC.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoC.Precio * CarritoC.Cantidad as Importe, CarritoC.Nota from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=CarritoC.IDMoneda) as MonedaOrigen, (select SUM(CarritoC.Precio * CarritoC.Cantidad)) as Subtotal, SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16 as IVA, (SUM(CarritoC.Precio * CarritoC.Cantidad)) + (SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16) as Total ,0 as TotalPesos from CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "' group by CarritoC.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;

            ViewBag.carrito = orden;

            if (ModelState.IsValid)
            {
                string fechahoy = DateTime.Now.ToShortDateString();
                int num = 0;
                DateTime fecha = encOrdenCompra.Fecha;
                string fecha1 = fecha.ToString("yyyy/MM/dd");

                DateTime fechareq = encOrdenCompra.FechaRequiere;
                string fecha2 = fechareq.ToString("yyyy/MM/dd");

                List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();

                VCambio tcambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + encOrdenCompra.IDMoneda + "," + origen + ") as TC").ToList()[0];
                decimal tCambio = tcambio.TC;





                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncOrdenCompra]([Fecha],[FechaRequiere],[IDProveedor],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[TipoCambio],[UserID],[IDUsoCFDI]) values ('" + fecha1 + "','" + fecha2 + "','" + encOrdenCompra.IDProveedor + "','" + encOrdenCompra.IDFormapago + "','" + encOrdenCompra.IDMoneda + "','" + encOrdenCompra.Observacion + "','" + subtotal + "','" + iva + "','" + total + "','" + encOrdenCompra.IDMetodoPago + "','" + encOrdenCompra.IDCondicionesPago + "','" + encOrdenCompra.IDAlmacen + "','Inactivo','" + tCambio + "','" + usuario + "','" + encOrdenCompra.IDUsoCFDI + "')");
                db.SaveChanges();

                List<EncOrdenCompra> numero;
                numero = db.Database.SqlQuery<EncOrdenCompra>("SELECT * FROM [dbo].[EncOrdenCompra] WHERE IDOrdenCompra = (SELECT MAX(IDOrdenCompra) from EncOrdenCompra)").ToList();
                num = numero.Select(s => s.IDOrdenCompra).FirstOrDefault();


                for (int i = 0; i < orden.Count(); i++)
                {

                    Precio = ViewBag.carrito[i].Precio;
                    importe = ViewBag.carrito[i].Cantidad * Precio;
                    importeiva = Math.Round(importe * montoiva, 2);
                    importetotal = importeiva + importe;
                    db.Database.ExecuteSqlCommand("INSERT INTO DetOrdenCompra([IDOrdenCompra],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna]) values ('" + num + "','" + ViewBag.carrito[i].IDArticulo + "','" + ViewBag.carrito[i].Cantidad + "','" + Precio + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encOrdenCompra.IDMoneda + "),'" + ViewBag.carrito[i].Cantidad + "','0','" + importe + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encOrdenCompra.IDMoneda + "),'true','" + importeiva + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encOrdenCompra.IDMoneda + "),'" + importetotal + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encOrdenCompra.IDMoneda + "),'" + ViewBag.carrito[i].Nota + "','0','" + ViewBag.carrito[i].IDCaracteristica + "','" + encOrdenCompra.IDAlmacen + "','0','Inactivo','" + ViewBag.carrito[i].Presentacion + "','" + ViewBag.carrito[i].jsonPresentacion + "','0')");
                    db.Database.ExecuteSqlCommand("delete from CarritoC where IDCarrito='" + ViewBag.carrito[i].IDCarrito + "'");
                    db.SaveChanges();



                }
                List<DetOrdenCompra> datostotales = db.Database.SqlQuery<DetOrdenCompra>("select * from dbo.DetOrdenCompra where IDOrdenCompra='" + num + "'").ToList();
                subtotalr = datostotales.Sum(s => s.Importe);
                ivar = Math.Round(subtotalr * montoiva, 2);
                totalr = subtotalr + ivar;
                db.Database.ExecuteSqlCommand("update [dbo].[EncOrdenCompra] set [Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "' where [IDOrdenCompra]='" + num + "'");

                return RedirectToAction("Index");

            }


            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", encOrdenCompra.IDFormapago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", encOrdenCompra.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", encOrdenCompra.IDMoneda);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", encOrdenCompra.IDCondicionesPago);
            ViewBag.IDProveedor = new SelectList(db.Proveedor, "IDProveedor", "Empresa", encOrdenCompra.IDProveedor);
            ViewBag.IDAlmacen = new SelectList(db.Almacenes, "IDAlmacen", "Descripcion", encOrdenCompra.IDProveedor);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS, "IDUsoCFDI", "Descripcion");
            return View(encOrdenCompra);

        }

        public ActionResult Cancelar(int? id)
        {
            List<User> userid;
            userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            string fecha = DateTime.Now.ToString("yyyyMMdd");

            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(id);
            List<DetOrdenCompra> orden = db.Database.SqlQuery<DetOrdenCompra>("select DetOrdenCompra.* from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "'  and Status<>'Finalizado'").ToList();
            ViewBag.ordenes = orden;
            try
            {
                List<DetRecepcion> detalles = new PrefacturaContext().Database.SqlQuery<DetRecepcion>("Select * from detRecepcion where IDExterna=" + id + " and status!='Cancelado'").ToList();

                if (detalles.Count > 0)
                {
                    throw new Exception("Hay Recepciones activas, por lo que no es posible cancelar la OC");
                }
            }
            catch (Exception err)
            {

            }



            foreach (var details in orden)
            {
                Caracteristica carateristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + details.Caracteristica_ID).ToList().FirstOrDefault();
                Articulo articulodetalle = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);
                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();

                if (inv != null)
                {

                    if (details.Status != "Inactivo")
                    {
                        if (articulodetalle.CtrlStock)
                        {
                            string c = "UPDATE dbo.InventarioAlmacen SET PorLlegar =(Porllegar-" + details.Cantidad + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID;
                            db.Database.ExecuteSqlCommand(c);
                            db.Database.ExecuteSqlCommand("Update InventarioAlmacen set porllegar=0 where porllegar<0 and IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);

                        }
                    }


                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Orden de Compra'," + details.Cantidad + ",'Orden Compra '," + details.IDOrdenCompra + ",''," + details.IDAlmacen + ",'N/A',0,'',CONVERT (time, SYSDATETIMEOFFSET()), " + UserID + ")";
                    db.Database.ExecuteSqlCommand(cadenam);

                }
                else
                {
                    if (details.Status != "Inactivo")
                    {
                        if (articulodetalle.CtrlStock)
                        {
                            db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                    + details.IDAlmacen + "," + articulodetalle.IDArticulo + "," + carateristica.ID + ",0,0,0,0)");
                        }
                    }

                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Orden de Compra'," + details.Cantidad + ",'Orden Compra '," + details.IDOrdenCompra + ",''," + details.IDAlmacen + ",'N/A',0,'',CONVERT (time, SYSDATETIMEOFFSET()), " + UserID + ")";
                    db.Database.ExecuteSqlCommand(cadenam);
                }



            }

            db.Database.ExecuteSqlCommand("update EncOrdenCompra set [Status]='Cancelado' where IDOrdenCompra='" + id + "'");
            db.Database.ExecuteSqlCommand("update DetOrdenCompra set [Status]='Cancelado'  where IDOrdenCompra='" + id + "'");
            try
            {
                List<DetRequisiciones> detailRequi = db.Database.SqlQuery<DetRequisiciones>("select d.*from detrequisiciones as d inner join elementosOrdenCompra as e on d.iddetrequisiciones=e.iddetdocumento where e.idordencompra=" + id).ToList();

                foreach (var item in detailRequi)
                {

                    db.Database.ExecuteSqlCommand("update DetRequisiciones set Status='Activo',suministro=0  where IDDetRequisiciones=" + item.IDDetRequisiciones);
                    db.Database.ExecuteSqlCommand("update EncRequisiciones set Status='Activo'  where idrequisicion=" + item.IDRequisicion);

                }

            }
            catch (Exception err)
            {
                String mensajerror = err.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var elemento = db.EncOrdenCompras.Single(m => m.IDOrdenCompra == id);

            return View(elemento);
        }

        // POST: Almacen/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from EncOrdenCompra where IDOrdenCompra='" + id + "'");
                db.Database.ExecuteSqlCommand("delete from DetOrdenCompra where IDOrdenCompra='" + id + "'");
                return RedirectToAction("Index");

            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(id);

            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", encOrdenCompra.IDFormapago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", encOrdenCompra.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", encOrdenCompra.IDMoneda);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", encOrdenCompra.IDCondicionesPago);
            ViewBag.IDProveedor = new SelectList(db.Proveedor, "IDProveedor", "Empresa", encOrdenCompra.IDProveedor);
            return View(encOrdenCompra);
        }

        // POST: EncRequisiones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EncOrdenCompra encOrdenCompra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(encOrdenCompra).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", encOrdenCompra.IDFormapago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", encOrdenCompra.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", encOrdenCompra.IDMoneda);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", encOrdenCompra.IDCondicionesPago);
            ViewBag.IDProveedor = new SelectList(db.Proveedor, "IDProveedor", "Empresa", encOrdenCompra.IDProveedor);
            return View(encOrdenCompra);
        }



        public ActionResult Requisiciones(int? id)
        {

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            if (id == null)
            {
                ViewBag.idrequi = 0;
                new ContactosClieContext().Database.ExecuteSqlCommand("delete from CarritoRequisicion  where userID=" + UserID);

            }
            else
            {

                List<CarritoRequisicion> car = db.Database.SqlQuery<CarritoRequisicion>("select * from [dbo].[CarritoRequisicion] where userID=" + UserID + "").ToList();

                foreach (var ite in car)
                {
                    if (ite.IDRequisicion == id)
                    {
                        ViewBag.idrequi = id;
                        new ContactosClieContext().Database.ExecuteSqlCommand("delete from CarritoRequisicion  where userID=" + UserID);

                    }
                    else
                    {
                        ViewBag.idrequi = id;

                    }
                }
                if (car.Count() == 0)
                {
                    ViewBag.idrequi = id;
                }





            }


            EncOrdenCompra nuevac = new EncOrdenCompra();
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0, Precio = 0;
            //Datos de Encabezado de Orden de Compra


            List<VEncRequisicion> orden = db.Database.SqlQuery<VEncRequisicion>("select EncRequisiciones.IDRequisicion,EncRequisiciones.Fecha, EncRequisiciones.FechaRequiere, Proveedores.Empresa as Proveedor, c_FormaPago.Descripcion as FormaPago, c_Moneda.Descripcion as Divisa, EncRequisiciones.Observacion, EncRequisiciones.Subtotal, EncRequisiciones.IVA, EncRequisiciones.Total,c_MetodoPago.Descripcion as MetodoPago, CondicionesPago.Descripcion as CondicionesPago, Almacen.Descripcion as Almacen, EncRequisiciones.Status,EncRequisiciones.UserID, EncRequisiciones.TipoCambio, c_UsoCFDI.Descripcion as UsoCFDI from EncRequisiciones inner join  Proveedores on Proveedores.IDProveedor=EncRequisiciones.IDProveedor inner join c_FormaPago on c_FormaPago.IDFormaPago=EncRequisiciones.IDFormapago inner join c_Moneda on c_Moneda.IDMoneda=EncRequisiciones.IDMoneda inner join c_MetodoPago on c_MetodoPago.IDMetodoPago=EncRequisiciones.IDMetodoPago inner join CondicionesPago on CondicionesPago.IDCondicionesPago=EncRequisiciones.IDCondicionesPago inner join Almacen on Almacen.IDAlmacen=EncRequisiciones.IDAlmacen inner join c_UsoCFDI on c_UsoCFDI.IDUsoCFDI=EncRequisiciones.IDUsoCFDI where EncRequisiciones.IDRequisicion='" + id + "' and EncRequisiciones.Status='Activo'").ToList();
            ViewBag.data = orden;

            List<VDetRequisiciones> elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
            ViewBag.datos = elementos;

            List<VCarritoRequisicion> lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.Cref,Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna,CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
            ViewBag.carritor = lista;

            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRequisiciones.TipoCambio)) as TotalenPesos from CarritoRequisicion inner join EncRequisiciones on EncRequisiciones.IDRequisicion=CarritoRequisicion.IDRequisicion group by EncRequisiciones.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;


            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
            int x = c.Dato;
            ViewBag.dato = x;

            ClsDatoEntero w = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
            if (id == null && w.Dato != 0)
            {
                ClsDatoEntero maxidreq = db.Database.SqlQuery<ClsDatoEntero>("select max(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
                ClsDatoEntero idreq = db.Database.SqlQuery<ClsDatoEntero>("select IDRequisicion as Dato from CarritoRequisicion where IDCarritoRequisicion='" + maxidreq.Dato + "'").ToList()[0];
                RequisicionesContext bd = new RequisicionesContext();
                ProveedorContext pr = new ProveedorContext();
                EncRequisiciones encRequisiciones = bd.EncRequisicioness.Find(idreq.Dato);
                nuevac.Fecha = encRequisiciones.Fecha;
                nuevac.FechaRequiere = encRequisiciones.FechaRequiere;

                elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
                ViewBag.datos = elementos;

                lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo, CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
                ViewBag.carritor = lista;

                try
                {
                    ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", encRequisiciones.IDUsoCFDI);
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion");
                }

                try
                {
                    ViewBag.IDAlmacen = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion", encRequisiciones.IDAlmacen);
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDUsoCFDI = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion");
                }


                try
                {
                    ViewBag.IDMoneda = new SelectList(pr.c_Monedas.ToList().OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion", encRequisiciones.IDMoneda); ;
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDMoneda = new SelectList(bd.c_Monedas.OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion");
                }



                try
                {
                    ViewBag.IDMetodoPago = new SelectList(pr.c_MetodoPagos.ToList().OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion", encRequisiciones.IDMetodoPago);
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDMetodoPago = new SelectList(bd.c_MetodoPagos.OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion");
                }


                try
                {
                    ViewBag.IDFormaPago = new SelectList(pr.c_FormaPagos.ToList().OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion", encRequisiciones.IDFormapago); ;

                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDFormaPago = new SelectList(bd.c_FormaPagos.OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion");
                }



                try
                {
                    ViewBag.IDCondicionesPago = new SelectList(pr.CondicionesPagos.ToList().OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion", encRequisiciones.IDCondicionesPago);

                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDCondicionesPago = new SelectList(bd.CondicionesPagos.OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion");
                }
                try
                {
                    ViewBag.IDProveedor = new SelectList(prov.Proveedores.ToList().OrderBy(s => s.Empresa), "IDProveedor", "Empresa", encRequisiciones.IDProveedor); ;
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDProveedor = new SelectList(prov.Proveedores.OrderBy(s => s.Empresa), "IDProveedor", "Empresa");
                }

            }

            if (id != null)
            {

                int idaux = 0;
                ClsDatoEntero existe = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRequisicion) as Dato from EncRequisiciones where IDRequisicion= '" + id + "'").ToList()[0];
                if (existe.Dato == 0)
                {

                    ViewBag.mensaje = "Requisición inexistente";

                    ProveedorContext pr = new ProveedorContext();
                    ClsDatoEntero countcc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
                    if (countcc.Dato != 0)
                    {
                        ClsDatoEntero maxidreq = db.Database.SqlQuery<ClsDatoEntero>("select max(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
                        ClsDatoEntero idreq = db.Database.SqlQuery<ClsDatoEntero>("select IDRequisicion as Dato from CarritoRequisicion where IDCarritoRequisicion='" + maxidreq.Dato + "'").ToList()[0];
                        RequisicionesContext bd = new RequisicionesContext();

                        EncRequisiciones encRequisiciones = bd.EncRequisicioness.Find(idreq.Dato);
                        nuevac.Fecha = encRequisiciones.Fecha;
                        nuevac.FechaRequiere = encRequisiciones.FechaRequiere;
                        try
                        {
                            ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", encRequisiciones.IDUsoCFDI);
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion");
                        }

                        try
                        {
                            ViewBag.IDAlmacen = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion", encRequisiciones.IDAlmacen);
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDUsoCFDI = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion");
                        }


                        try
                        {
                            ViewBag.IDMoneda = new SelectList(pr.c_Monedas.ToList().OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion", encRequisiciones.IDMoneda); ;
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDMoneda = new SelectList(bd.c_Monedas.OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion");
                        }



                        try
                        {
                            ViewBag.IDMetodoPago = new SelectList(pr.c_MetodoPagos.ToList().OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion", encRequisiciones.IDMetodoPago);
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDMetodoPago = new SelectList(bd.c_MetodoPagos.OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion");
                        }


                        try
                        {
                            ViewBag.IDFormaPago = new SelectList(pr.c_FormaPagos.ToList().OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion", encRequisiciones.IDFormapago); ;

                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDFormaPago = new SelectList(bd.c_FormaPagos.OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion");
                        }



                        try
                        {
                            ViewBag.IDCondicionesPago = new SelectList(pr.CondicionesPagos.ToList().OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion", encRequisiciones.IDCondicionesPago);

                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDCondicionesPago = new SelectList(bd.CondicionesPagos.OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion");
                        }
                        try
                        {
                            ViewBag.IDProveedor = new SelectList(prov.Proveedores, "IDProveedor", "Empresa", encRequisiciones.IDProveedor);
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDProveedor = new SelectList(prov.Proveedores.OrderBy(s => s.Empresa), "IDProveedor", "Empresa");
                        }

                        elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
                        ViewBag.datos = elementos;

                        lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
                        ViewBag.carritor = lista;

                    }
                    else
                    {
                        ViewBag.data = null;

                        ViewBag.otro = 0;

                        ViewBag.datos = null;
                        ViewBag.dato = 0;
                    }
                    if (nuevac.IDProveedor == 0 && idaux > 0)
                    {
                        nuevac.IDProveedor = idaux;
                    }
                    elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
                    ViewBag.datos = elementos;

                    lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
                    ViewBag.carritor = lista;

                    return View(nuevac);
                }
                else
                {
                    RequisicionesContext bd = new RequisicionesContext();
                    ProveedorContext pr = new ProveedorContext();
                    EncRequisiciones encRequisiciones = bd.EncRequisicioness.Find(id);
                    nuevac.Fecha = encRequisiciones.Fecha;
                    nuevac.FechaRequiere = encRequisiciones.FechaRequiere;
                    nuevac.IDProveedor = encRequisiciones.IDProveedor;
                    try
                    {
                        ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", encRequisiciones.IDUsoCFDI);
                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion");
                    }

                    try
                    {
                        ViewBag.IDAlmacen = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion", encRequisiciones.IDAlmacen);
                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDUsoCFDI = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion");
                    }


                    try
                    {
                        ViewBag.IDMoneda = new SelectList(pr.c_Monedas.ToList().OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion", encRequisiciones.IDMoneda); ;
                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDMoneda = new SelectList(bd.c_Monedas.OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion");
                    }



                    try
                    {
                        ViewBag.IDMetodoPago = new SelectList(pr.c_MetodoPagos.ToList().OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion", encRequisiciones.IDMetodoPago);
                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDMetodoPago = new SelectList(bd.c_MetodoPagos.OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion");
                    }


                    try
                    {
                        ViewBag.IDFormaPago = new SelectList(pr.c_FormaPagos.ToList().OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion", encRequisiciones.IDFormapago); ;

                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDFormaPago = new SelectList(bd.c_FormaPagos.OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion");
                    }



                    try
                    {
                        ViewBag.IDCondicionesPago = new SelectList(pr.CondicionesPagos.ToList().OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion", encRequisiciones.IDCondicionesPago);

                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDCondicionesPago = new SelectList(bd.CondicionesPagos.OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion");
                    }
                    try
                    {
                        ViewBag.IDProveedor = new SelectList(prov.Proveedores.OrderBy(s=> s.Empresa), "IDProveedor", "Empresa", encRequisiciones.IDProveedor);

                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDProveedor = new SelectList(prov.Proveedores.OrderBy(s => s.Empresa), "IDProveedor", "Empresa");
                    }
                    elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
                    ViewBag.datos = elementos;

                    lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
                    ViewBag.carritor = lista;

                    //   return View(nuevac);
                }
            }



            ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRequisicion) as Dato from EncRequisiciones where IDRequisicion='" + id + "' and Status='Activo'").ToList()[0];

            ViewBag.otro = denc.Dato;



            ClsDatoEntero dcompra = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRequisicion) as Dato from DetRequisiciones where IDRequisicion='" + id + "' and Status='Activo'").ToList()[0];



            if (id != null && denc.Dato > 0 && dcompra.Dato > 0)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO CarritoRequisicion([IDDetExterna],[IDRequisicion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion]) SELECT [IDDetRequisiciones],[IDRequisicion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion] FROM DetRequisiciones where IDRequisicion='" + id + "' and Status='Activo'");
                db.Database.ExecuteSqlCommand("update [dbo].[CarritoRequisicion] set [UserID]='" + UserID + "' where [IDRequisicion]='" + id + "'");


            }

            orden = db.Database.SqlQuery<VEncRequisicion>("select EncRequisiciones.IDRequisicion,EncRequisiciones.Fecha, EncRequisiciones.FechaRequiere, Proveedores.Empresa as Proveedor, c_FormaPago.Descripcion as FormaPago, c_Moneda.Descripcion as Divisa, EncRequisiciones.Observacion, EncRequisiciones.Subtotal, EncRequisiciones.IVA, EncRequisiciones.Total,c_MetodoPago.Descripcion as MetodoPago, CondicionesPago.Descripcion as CondicionesPago, Almacen.Descripcion as Almacen, EncRequisiciones.Status,EncRequisiciones.UserID, EncRequisiciones.TipoCambio, c_UsoCFDI.Descripcion as UsoCFDI from EncRequisiciones inner join  Proveedores on Proveedores.IDProveedor=EncRequisiciones.IDProveedor inner join c_FormaPago on c_FormaPago.IDFormaPago=EncRequisiciones.IDFormapago inner join c_Moneda on c_Moneda.IDMoneda=EncRequisiciones.IDMoneda inner join c_MetodoPago on c_MetodoPago.IDMetodoPago=EncRequisiciones.IDMetodoPago inner join CondicionesPago on CondicionesPago.IDCondicionesPago=EncRequisiciones.IDCondicionesPago inner join Almacen on Almacen.IDAlmacen=EncRequisiciones.IDAlmacen inner join c_UsoCFDI on c_UsoCFDI.IDUsoCFDI=EncRequisiciones.IDUsoCFDI where EncRequisiciones.IDRequisicion='" + id + "' and EncRequisiciones.Status='Activo'").ToList();
            ViewBag.data = orden;

            elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
            ViewBag.datos = elementos;

            lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, CarritoRequisicion.Caracteristica_ID as IDCaracteristica, CarritoRequisicion.IDArticulo as IDArticulo  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
            ViewBag.carritor = lista;

            resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRequisiciones.TipoCambio)) as TotalenPesos from CarritoRequisicion inner join EncRequisiciones on EncRequisiciones.IDRequisicion=CarritoRequisicion.IDRequisicion group by EncRequisiciones.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;


            c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRequisicion) as Dato from CarritoRequisicion where  UserID='" + UserID + "'").ToList()[0];
            x = c.Dato;
            ViewBag.dato = x;


            Session["UserID"] = UserID;
            return View(nuevac);
        }

        [HttpPost]
        public ActionResult Requisiciones(EncOrdenCompra encOrdenCompra, FormCollection elementos)
        {
            SIAAPI.ViewModels.Comercial.VCambio cambiodedia = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<SIAAPI.ViewModels.Comercial.VCambio>
                                 ("select dbo.GetTipocambioCadena(GETDATE(),'USD','MXN') as TC").ToList()[0];
            decimal Cambiodeldia = cambiodedia.TC;

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            //if (encOrdenCompra.IDFormapago != 0)
            //{
            int contador = int.Parse(elementos["contador"]);

            Proveedor pro = new ProveedorContext().Proveedores.Find(encOrdenCompra.IDProveedor);

            decimal ivapro = pro.c_TiposIVA.Tasa;
            decimal subtotalr = 0;
            decimal ivar = 0;
            decimal totalr = 0;
            string tieneiva = "'1'";
            if (ivapro == 0)
            {
                tieneiva = "'0'";
            }




            List<c_Moneda> clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + encOrdenCompra.IDMoneda + "'").ToList();
            string clave = clavemoneda.Select(s => s.ClaveMoneda).FirstOrDefault();



            for (int i = 1; i <= contador; i++)
            {

                string IDCarrito = elementos["IDCarrito" + i];
                string IDArticulo = elementos["IDArticulo" + i];
                string IDCaracteristica = elementos["IDCarasteristica" + i];
                string Nota = elementos["Nota" + i];


                RequisicionesContext dbreq = new RequisicionesContext();
                int norequisicion = int.Parse(elementos["Requisicion" + i]);
                EncRequisiciones encRequisiciones = dbreq.EncRequisicioness.Find(norequisicion);

                Articulo articulo = new ArticuloContext().Articulo.Find(int.Parse(IDArticulo));
                //Datos para tipo de cambio
                c_Moneda monedaoriginal = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + articulo.IDMoneda + "'").ToList().FirstOrDefault();
                c_Moneda monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + encOrdenCompra.IDMoneda + "'").ToList().FirstOrDefault();

                VCambio cambiodet = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio(GETDATE()," + monedaoriginal.IDMoneda + "," + monedadestino.IDMoneda + ") as TC").ToList()[0];
                decimal Cambiodet = cambiodet.TC;


                decimal Cantidad = decimal.Parse(elementos["Cantidad" + i]);
                decimal Costo = decimal.Parse(elementos["Costo" + i]);
                Costo = Costo * Cambiodet;
                decimal importe = Costo * Cantidad;
                decimal ivac = importe * ivapro;
                decimal costot = importe + ivac;

                ClsDatoDecimal cantidadreal = db.Database.SqlQuery<ClsDatoDecimal>("select cantidadpedida as Dato from dbo.[DetRequisiciones] WHERE idrequisicion=" + norequisicion + "").ToList()[0];
                ClsDatoDecimal sum = db.Database.SqlQuery<ClsDatoDecimal>("select suministro as Dato from dbo.[DetRequisiciones] WHERE idrequisicion=" + norequisicion + "").ToList()[0];

                decimal suministro = 0;
                if (sum.Dato == 0)
                {
                    db.Database.ExecuteSqlCommand("update [EncRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                    db.Database.ExecuteSqlCommand("update [DetRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                }


                if (Cantidad > cantidadreal.Dato || Cantidad == cantidadreal.Dato)
                {
                    db.Database.ExecuteSqlCommand("update [EncRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                    db.Database.ExecuteSqlCommand("update [DetRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                }
                if (sum.Dato < cantidadreal.Dato || sum.Dato == cantidadreal.Dato)
                {
                    suministro = sum.Dato - Cantidad;
                    db.Database.ExecuteSqlCommand("update DetRequisiciones set nota='" + Nota + "', suministro=" + suministro + ", Costo =" + Costo + ",Importe=" + importe + ", IVA  =" + tieneiva + ",ImporteIVA=" + ivac + ", ImporteTotal=" + costot + ", status='Activo' where idrequisicion=" + norequisicion);
                    if (suministro <= 0)
                    {
                        db.Database.ExecuteSqlCommand("update [EncRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                        db.Database.ExecuteSqlCommand("update [DetRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                    }

                }





                db.Database.ExecuteSqlCommand("update CarritoRequisicion set nota='" + Nota + "', Cantidad=" + Cantidad + ", Costo =" + Costo + ",Importe=" + importe + ", IVA  =" + tieneiva + ",ImporteIVA=" + ivac + ", ImporteTotal=" + costot + " where IDCarritoRequisicion=" + IDCarrito);
                subtotalr += importe;
                ivar += ivac;
                totalr += costot;

                //db.Database.ExecuteSqlCommand("update [EncRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);

            }

            List<VCarritoRequisicion> carritor = db.Database.SqlQuery<VCarritoRequisicion>("select Caracteristica.ID as IDCaracteristica,Articulo.IDArticulo,Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro, CarritoRequisicion.Nota,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();

            int num = 0;
            DateTime fecha = encOrdenCompra.Fecha;
            string fecha1 = fecha.ToString("yyyy/MM/dd");

            DateTime fechareq = encOrdenCompra.FechaRequiere;
            string fecha2 = fechareq.ToString("yyyy/MM/dd");


            string fechaatual = DateTime.Now.ToString("yyyy/MM/dd");

            db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncOrdenCompra]([Fecha],[FechaRequiere],[IDProveedor],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[TipoCambio],[UserID],[IDUsoCFDI], SujetoCalidad) values ('" + fecha1 + "','" + fecha2 + "','" + encOrdenCompra.IDProveedor + "','" + encOrdenCompra.IDFormapago + "'," + encOrdenCompra.IDMoneda + ",'" + encOrdenCompra.Observacion + "'," + subtotalr + "," + ivar + "," + totalr + "," + encOrdenCompra.IDMetodoPago + "," + encOrdenCompra.IDCondicionesPago + "," + encOrdenCompra.IDAlmacen + ",'Inactivo', " + Cambiodeldia + ",'" + UserID + "','" + encOrdenCompra.IDUsoCFDI + "','" + encOrdenCompra.SujetoCalidad + "')");



            List<EncOrdenCompra> numero;
            numero = db.Database.SqlQuery<EncOrdenCompra>("SELECT * FROM [dbo].[EncOrdenCompra] WHERE IDOrdenCompra = (SELECT MAX(IDOrdenCompra) from EncOrdenCompra)").ToList();
            num = numero.Select(s => s.IDOrdenCompra).FirstOrDefault();


            foreach (VCarritoRequisicion itemcarritor in carritor)
            {


                //Insercion de Detalle de Orden de Compra
                db.Database.ExecuteSqlCommand("INSERT INTO DetOrdenCompra([IDOrdenCompra],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna]) values ('" + num + "','" + itemcarritor.IDArticulo + "','" + itemcarritor.Cantidad + "','" + itemcarritor.Costo + "','" + itemcarritor.Cantidad + "','0','" + itemcarritor.Importe + "','true','" + itemcarritor.ImporteIva + "','" + itemcarritor.ImporteTotal + "','" + itemcarritor.Nota + "','0','" + itemcarritor.IDCaracteristica + "','" + encOrdenCompra.IDAlmacen + "','0','Inactivo','" + itemcarritor.Presentacion + "','" + itemcarritor.jsonPresentacion + "','" + itemcarritor.IDRequisicion + "')");
                List<DetOrdenCompra> numero2 = db.Database.SqlQuery<DetOrdenCompra>("SELECT * FROM [dbo].[DetOrdenCompra] WHERE IDDetOrdenCompra = (SELECT MAX(IDDetOrdenCompra) from DetOrdenCompra)").ToList();
                int num2 = numero2.Select(s => s.IDDetOrdenCompra).FirstOrDefault();
                db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('OrdenCompra'," + num + "," + itemcarritor.Cantidad + ",'" + fechaatual + "'," + itemcarritor.IDRequisicion + ",'Requisición'," + UserID + "," + itemcarritor.IDDetExterna + "," + num2 + ")");

                db.Database.ExecuteSqlCommand("delete from CarritoRequisicion where IDCarritoRequisicion='" + itemcarritor.IDCarritoRequisicion + "'");





            }




            return RedirectToAction("Index");
        }

        public ActionResult Carritocpartial(int IDProveedor, int IDMoneda)
        {
            string iduser = Session["UserID"].ToString();

          
            List<VCarritoRequisicion> carritor = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.Cref,Caracteristica.ID as IDCaracteristica,Articulo.IDArticulo,Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro, CarritoRequisicion.Nota,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + iduser + "'").ToList();

            foreach (VCarritoRequisicion item in carritor )
            {
                string cadena= "select* from MatrizPrecioProv where ranginf <= " + item.Cantidad + " and rangSup>= " + item.Cantidad + " and idproveedor = "+IDProveedor+" and IDArticulo = "+item.IDArticulo;
                MatrizPrecioProv mpp = new MatrizPrecioProveedorContext().Database.SqlQuery<MatrizPrecioProv>( cadena).FirstOrDefault();

                if (mpp== null)
                    {

                }
                else
                {
                    decimal IVa = decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                    if (mpp.IDMoneda==IDMoneda)
                    {
                       
                        decimal costo = mpp.Precio;
                        new MatrizPrecioProvContext().Database.ExecuteSqlCommand("update CarritoRequisicion set costo=" +costo + ", importe = cantidad *"+costo +" where idCarritoRequisicion =" + item.IDCarritoRequisicion);
                        new MatrizPrecioProvContext().Database.ExecuteSqlCommand("update CarritoRequisicion set importeiva=importe*" + IVa + ", importetotal = importe + (importe*" + IVa  + ") where idCarritoRequisicion =" + item.IDCarritoRequisicion);

                    }
                    else
                    {
                        string cadenatc = "select  dbo. GetTipocambio(GetDate(), " + mpp.IDMoneda + ", " + IDMoneda + ") as Dato";
                        decimal Tc = new MatrizPrecioProvContext().Database.SqlQuery<ClsDatoDecimal>(cadenatc).FirstOrDefault().Dato;
                        decimal costo = mpp.Precio * Tc;
                        new MatrizPrecioProvContext().Database.ExecuteSqlCommand("update CarritoRequisicion set costo=" + costo + ", importe = cantidad *" + costo + " where idCarritoRequisicion =" + item.IDCarritoRequisicion);
                        new MatrizPrecioProvContext().Database.ExecuteSqlCommand("update CarritoRequisicion set importeiva=importe*" + IVa + ", importetotal = importe + (importe*" + IVa + ") where idCarritoRequisicion =" + item.IDCarritoRequisicion);


                    }
                }


               
            }

          carritor = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.Cref,Caracteristica.ID as IDCaracteristica,Articulo.IDArticulo,Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro, CarritoRequisicion.Nota,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + iduser + "'").ToList();

            return PartialView("Carritocpartial", carritor);
        }
        public ActionResult VRequisiciones(int? id)
        {

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            if (id == null)
            {
                ViewBag.idrequi = 0;
                new ContactosClieContext().Database.ExecuteSqlCommand("delete from CarritoRequisicion  where userID=" + UserID);

            }
            else
            {

                List<CarritoRequisicion> car = db.Database.SqlQuery<CarritoRequisicion>("select * from [dbo].[CarritoRequisicion] where userID=" + UserID + "").ToList();

                foreach (var ite in car)
                {
                    if (ite.IDRequisicion == id)
                    {
                        ViewBag.idrequi = id;
                        new ContactosClieContext().Database.ExecuteSqlCommand("delete from CarritoRequisicion  where userID=" + UserID);

                    }
                    else
                    {
                        ViewBag.idrequi = id;

                    }
                }
                if (car.Count() == 0)
                {
                    ViewBag.idrequi = id;
                }





            }


            EncOrdenCompra nuevac = new EncOrdenCompra();
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0, Precio = 0;
            //Datos de Encabezado de Orden de Compra


            List<VEncRequisicion> orden = db.Database.SqlQuery<VEncRequisicion>("select EncRequisiciones.IDRequisicion,EncRequisiciones.Fecha, EncRequisiciones.FechaRequiere, Proveedores.Empresa as Proveedor, c_FormaPago.Descripcion as FormaPago, c_Moneda.Descripcion as Divisa, EncRequisiciones.Observacion, EncRequisiciones.Subtotal, EncRequisiciones.IVA, EncRequisiciones.Total,c_MetodoPago.Descripcion as MetodoPago, CondicionesPago.Descripcion as CondicionesPago, Almacen.Descripcion as Almacen, EncRequisiciones.Status,EncRequisiciones.UserID, EncRequisiciones.TipoCambio, c_UsoCFDI.Descripcion as UsoCFDI from EncRequisiciones inner join  Proveedores on Proveedores.IDProveedor=EncRequisiciones.IDProveedor inner join c_FormaPago on c_FormaPago.IDFormaPago=EncRequisiciones.IDFormapago inner join c_Moneda on c_Moneda.IDMoneda=EncRequisiciones.IDMoneda inner join c_MetodoPago on c_MetodoPago.IDMetodoPago=EncRequisiciones.IDMetodoPago inner join CondicionesPago on CondicionesPago.IDCondicionesPago=EncRequisiciones.IDCondicionesPago inner join Almacen on Almacen.IDAlmacen=EncRequisiciones.IDAlmacen inner join c_UsoCFDI on c_UsoCFDI.IDUsoCFDI=EncRequisiciones.IDUsoCFDI where EncRequisiciones.IDRequisicion='" + id + "' and EncRequisiciones.Status='Activo'").ToList();
            ViewBag.data = orden;

            List<VDetRequisiciones> elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
            ViewBag.datos = elementos;

            List<VCarritoRequisicion> lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.Cref,Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna,CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
            ViewBag.carritor = lista;

            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRequisiciones.TipoCambio)) as TotalenPesos from CarritoRequisicion inner join EncRequisiciones on EncRequisiciones.IDRequisicion=CarritoRequisicion.IDRequisicion group by EncRequisiciones.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;


            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
            int x = c.Dato;
            ViewBag.dato = x;

            ClsDatoEntero w = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
            if (id == null && w.Dato != 0)
            {
                ClsDatoEntero maxidreq = db.Database.SqlQuery<ClsDatoEntero>("select max(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
                ClsDatoEntero idreq = db.Database.SqlQuery<ClsDatoEntero>("select IDRequisicion as Dato from CarritoRequisicion where IDCarritoRequisicion='" + maxidreq.Dato + "'").ToList()[0];
                RequisicionesContext bd = new RequisicionesContext();
                ProveedorContext pr = new ProveedorContext();
                EncRequisiciones encRequisiciones = bd.EncRequisicioness.Find(idreq.Dato);
                nuevac.Fecha = encRequisiciones.Fecha;
                nuevac.FechaRequiere = encRequisiciones.FechaRequiere;

                elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
                ViewBag.datos = elementos;

                lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo, CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
                ViewBag.carritor = lista;

                try
                {
                    ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", encRequisiciones.IDUsoCFDI);
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion");
                }

                try
                {
                    ViewBag.IDAlmacen = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion", encRequisiciones.IDAlmacen);
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDUsoCFDI = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion");
                }


                try
                {
                    ViewBag.IDMoneda = new SelectList(pr.c_Monedas.ToList().OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion", encRequisiciones.IDMoneda); ;
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDMoneda = new SelectList(bd.c_Monedas.OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion");
                }



                try
                {
                    ViewBag.IDMetodoPago = new SelectList(pr.c_MetodoPagos.ToList().OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion", encRequisiciones.IDMetodoPago);
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDMetodoPago = new SelectList(bd.c_MetodoPagos.OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion");
                }


                try
                {
                    ViewBag.IDFormaPago = new SelectList(pr.c_FormaPagos.ToList().OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion", encRequisiciones.IDFormapago); ;

                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDFormaPago = new SelectList(bd.c_FormaPagos.OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion");
                }



                try
                {
                    ViewBag.IDCondicionesPago = new SelectList(pr.CondicionesPagos.ToList().OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion", encRequisiciones.IDCondicionesPago);

                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDCondicionesPago = new SelectList(bd.CondicionesPagos.OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion");
                }
                try
                {
                    ViewBag.IDProveedor = new SelectList(prov.Proveedores.ToList().OrderBy(s => s.Empresa), "IDProveedor", "Empresa", encRequisiciones.IDProveedor); ;
                }
                catch (Exception err1)
                {
                    string mensaje = err1.Message;
                    ViewBag.IDProveedor = new SelectList(prov.Proveedores.OrderBy(s => s.Empresa), "IDProveedor", "Empresa");
                }

            }

            if (id != null)
            {

                int idaux = 0;
                ClsDatoEntero existe = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRequisicion) as Dato from EncRequisiciones where IDRequisicion= '" + id + "'").ToList()[0];
                if (existe.Dato == 0)
                {

                    ViewBag.mensaje = "Requisición inexistente";

                    ProveedorContext pr = new ProveedorContext();
                    ClsDatoEntero countcc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
                    if (countcc.Dato != 0)
                    {
                        ClsDatoEntero maxidreq = db.Database.SqlQuery<ClsDatoEntero>("select max(IDCarritoRequisicion) as Dato from CarritoRequisicion ").ToList()[0];
                        ClsDatoEntero idreq = db.Database.SqlQuery<ClsDatoEntero>("select IDRequisicion as Dato from CarritoRequisicion where IDCarritoRequisicion='" + maxidreq.Dato + "'").ToList()[0];
                        RequisicionesContext bd = new RequisicionesContext();

                        EncRequisiciones encRequisiciones = bd.EncRequisicioness.Find(idreq.Dato);
                        nuevac.Fecha = encRequisiciones.Fecha;
                        nuevac.FechaRequiere = encRequisiciones.FechaRequiere;
                        try
                        {
                            ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", encRequisiciones.IDUsoCFDI);
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion");
                        }

                        try
                        {
                            ViewBag.IDAlmacen = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion", encRequisiciones.IDAlmacen);
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDUsoCFDI = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion");
                        }


                        try
                        {
                            ViewBag.IDMoneda = new SelectList(pr.c_Monedas.ToList().OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion", encRequisiciones.IDMoneda); ;
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDMoneda = new SelectList(bd.c_Monedas.OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion");
                        }



                        try
                        {
                            ViewBag.IDMetodoPago = new SelectList(pr.c_MetodoPagos.ToList().OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion", encRequisiciones.IDMetodoPago);
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDMetodoPago = new SelectList(bd.c_MetodoPagos.OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion");
                        }


                        try
                        {
                            ViewBag.IDFormaPago = new SelectList(pr.c_FormaPagos.ToList().OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion", encRequisiciones.IDFormapago); ;

                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDFormaPago = new SelectList(bd.c_FormaPagos.OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion");
                        }



                        try
                        {
                            ViewBag.IDCondicionesPago = new SelectList(pr.CondicionesPagos.ToList().OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion", encRequisiciones.IDCondicionesPago);

                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDCondicionesPago = new SelectList(bd.CondicionesPagos.OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion");
                        }
                        try
                        {
                            ViewBag.IDProveedor = new SelectList(prov.Proveedores, "IDProveedor", "Empresa", encRequisiciones.IDProveedor);
                        }
                        catch (Exception err1)
                        {
                            string mensaje = err1.Message;
                            ViewBag.IDProveedor = new SelectList(prov.Proveedores.OrderBy(s => s.Empresa), "IDProveedor", "Empresa");
                        }

                        elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
                        ViewBag.datos = elementos;

                        lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
                        ViewBag.carritor = lista;

                    }
                    else
                    {
                        ViewBag.data = null;

                        ViewBag.otro = 0;

                        ViewBag.datos = null;
                        ViewBag.dato = 0;
                    }
                    if (nuevac.IDProveedor == 0 && idaux > 0)
                    {
                        nuevac.IDProveedor = idaux;
                    }
                    elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
                    ViewBag.datos = elementos;

                    lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
                    ViewBag.carritor = lista;

                    return View(nuevac);
                }
                else
                {
                    RequisicionesContext bd = new RequisicionesContext();
                    ProveedorContext pr = new ProveedorContext();
                    EncRequisiciones encRequisiciones = bd.EncRequisicioness.Find(id);
                    nuevac.Fecha = encRequisiciones.Fecha;
                    nuevac.FechaRequiere = encRequisiciones.FechaRequiere;
                    nuevac.IDProveedor = encRequisiciones.IDProveedor;
                    try
                    {
                        ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", encRequisiciones.IDUsoCFDI);
                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion");
                    }

                    try
                    {
                        ViewBag.IDAlmacen = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion", encRequisiciones.IDAlmacen);
                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDUsoCFDI = new SelectList(bd.Almacenes.OrderBy(s => s.Descripcion), "IDAlmacen", "Descripcion");
                    }


                    try
                    {
                        ViewBag.IDMoneda = new SelectList(pr.c_Monedas.ToList().OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion", encRequisiciones.IDMoneda); ;
                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDMoneda = new SelectList(bd.c_Monedas.OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion");
                    }



                    try
                    {
                        ViewBag.IDMetodoPago = new SelectList(pr.c_MetodoPagos.ToList().OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion", encRequisiciones.IDMetodoPago);
                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDMetodoPago = new SelectList(bd.c_MetodoPagos.OrderBy(s => s.Descripcion), "IDMetodoPago", "Descripcion");
                    }


                    try
                    {
                        ViewBag.IDFormaPago = new SelectList(pr.c_FormaPagos.ToList().OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion", encRequisiciones.IDFormapago); ;

                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDFormaPago = new SelectList(bd.c_FormaPagos.OrderBy(s => s.Descripcion), "IDFormaPago", "Descripcion");
                    }



                    try
                    {
                        ViewBag.IDCondicionesPago = new SelectList(pr.CondicionesPagos.ToList().OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion", encRequisiciones.IDCondicionesPago);

                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDCondicionesPago = new SelectList(bd.CondicionesPagos.OrderBy(s => s.Descripcion), "IDCondicionesPago", "Descripcion");
                    }
                    try
                    {
                        ViewBag.IDProveedor = new SelectList(prov.Proveedores.OrderBy(s => s.Empresa), "IDProveedor", "Empresa", encRequisiciones.IDProveedor);

                    }
                    catch (Exception err1)
                    {
                        string mensaje = err1.Message;
                        ViewBag.IDProveedor = new SelectList(prov.Proveedores.OrderBy(s => s.Empresa), "IDProveedor", "Empresa");
                    }
                    elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
                    ViewBag.datos = elementos;

                    lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
                    ViewBag.carritor = lista;

                    //   return View(nuevac);
                }
            }



            ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRequisicion) as Dato from EncRequisiciones where IDRequisicion='" + id + "' and Status='Activo'").ToList()[0];

            ViewBag.otro = denc.Dato;



            ClsDatoEntero dcompra = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRequisicion) as Dato from DetRequisiciones where IDRequisicion='" + id + "' and Status='Activo'").ToList()[0];



            if (id != null && denc.Dato > 0 && dcompra.Dato > 0)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO CarritoRequisicion([IDDetExterna],[IDRequisicion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion]) SELECT [IDDetRequisiciones],[IDRequisicion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion] FROM DetRequisiciones where IDRequisicion='" + id + "' and Status='Activo'");
                db.Database.ExecuteSqlCommand("update [dbo].[CarritoRequisicion] set [UserID]='" + UserID + "' where [IDRequisicion]='" + id + "'");


            }

            orden = db.Database.SqlQuery<VEncRequisicion>("select EncRequisiciones.IDRequisicion,EncRequisiciones.Fecha, EncRequisiciones.FechaRequiere, Proveedores.Empresa as Proveedor, c_FormaPago.Descripcion as FormaPago, c_Moneda.Descripcion as Divisa, EncRequisiciones.Observacion, EncRequisiciones.Subtotal, EncRequisiciones.IVA, EncRequisiciones.Total,c_MetodoPago.Descripcion as MetodoPago, CondicionesPago.Descripcion as CondicionesPago, Almacen.Descripcion as Almacen, EncRequisiciones.Status,EncRequisiciones.UserID, EncRequisiciones.TipoCambio, c_UsoCFDI.Descripcion as UsoCFDI from EncRequisiciones inner join  Proveedores on Proveedores.IDProveedor=EncRequisiciones.IDProveedor inner join c_FormaPago on c_FormaPago.IDFormaPago=EncRequisiciones.IDFormapago inner join c_Moneda on c_Moneda.IDMoneda=EncRequisiciones.IDMoneda inner join c_MetodoPago on c_MetodoPago.IDMetodoPago=EncRequisiciones.IDMetodoPago inner join CondicionesPago on CondicionesPago.IDCondicionesPago=EncRequisiciones.IDCondicionesPago inner join Almacen on Almacen.IDAlmacen=EncRequisiciones.IDAlmacen inner join c_UsoCFDI on c_UsoCFDI.IDUsoCFDI=EncRequisiciones.IDUsoCFDI where EncRequisiciones.IDRequisicion='" + id + "' and EncRequisiciones.Status='Activo'").ToList();
            ViewBag.data = orden;

            elementos = db.Database.SqlQuery<VDetRequisiciones>("select Articulo.MinimoCompra,DetRequisiciones.IDDetRequisiciones, DetRequisiciones.IDRequisicion,DetRequisiciones.Suministro,Articulo.Descripcion as Articulo,DetRequisiciones.Cantidad,DetRequisiciones.Costo,DetRequisiciones.CantidadPedida,DetRequisiciones.Descuento,DetRequisiciones.Importe,DetRequisiciones.IVA,DetRequisiciones.ImporteIva,DetRequisiciones.ImporteTotal,DetRequisiciones.Nota,DetRequisiciones.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRequisiciones.Status from  DetRequisiciones INNER JOIN Caracteristica ON DetRequisiciones.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRequisicion='" + id + "' and Status='Activo'").ToList();
            ViewBag.datos = elementos;

            lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, CarritoRequisicion.Caracteristica_ID as IDCaracteristica, CarritoRequisicion.IDArticulo as IDArticulo  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
            ViewBag.carritor = lista;

            resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRequisiciones.TipoCambio)) as TotalenPesos from CarritoRequisicion inner join EncRequisiciones on EncRequisiciones.IDRequisicion=CarritoRequisicion.IDRequisicion group by EncRequisiciones.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;


            c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRequisicion) as Dato from CarritoRequisicion where  UserID='" + UserID + "'").ToList()[0];
            x = c.Dato;
            ViewBag.dato = x;


            Session["UserID"] = UserID;
            return View(nuevac);
        }

        [HttpPost]
        public ActionResult VRequisiciones(EncOrdenCompra encOrdenCompra, FormCollection elementos)
        {
            SIAAPI.ViewModels.Comercial.VCambio cambiodedia = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<SIAAPI.ViewModels.Comercial.VCambio>
                                 ("select dbo.GetTipocambioCadena(GETDATE(),'USD','MXN') as TC").ToList()[0];
            decimal Cambiodeldia = cambiodedia.TC;

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            //if (encOrdenCompra.IDFormapago != 0)
            //{
            int contador = int.Parse(elementos["contador"]);

            Proveedor pro = new ProveedorContext().Proveedores.Find(encOrdenCompra.IDProveedor);

            decimal ivapro = pro.c_TiposIVA.Tasa;
            decimal subtotalr = 0;
            decimal ivar = 0;
            decimal totalr = 0;
            string tieneiva = "'1'";
            if (ivapro == 0)
            {
                tieneiva = "'0'";
            }




            List<c_Moneda> clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + encOrdenCompra.IDMoneda + "'").ToList();
            string clave = clavemoneda.Select(s => s.ClaveMoneda).FirstOrDefault();



            for (int i = 1; i <= contador; i++)
            {

                string IDCarrito = elementos["IDCarrito" + i];
                string IDArticulo = elementos["IDArticulo" + i];
                string IDCaracteristica = elementos["IDCarasteristica" + i];
                string Nota = elementos["Nota" + i];


                RequisicionesContext dbreq = new RequisicionesContext();
                int norequisicion = int.Parse(elementos["Requisicion" + i]);
                EncRequisiciones encRequisiciones = dbreq.EncRequisicioness.Find(norequisicion);

                Articulo articulo = new ArticuloContext().Articulo.Find(int.Parse(IDArticulo));
                //Datos para tipo de cambio
                c_Moneda monedaoriginal = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + articulo.IDMoneda + "'").ToList().FirstOrDefault();
                c_Moneda monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + encOrdenCompra.IDMoneda + "'").ToList().FirstOrDefault();

                VCambio cambiodet = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio(GETDATE()," + monedaoriginal.IDMoneda + "," + monedadestino.IDMoneda + ") as TC").ToList()[0];
                decimal Cambiodet = cambiodet.TC;


                decimal Cantidad = decimal.Parse(elementos["Cantidad" + i]);
                decimal Costo = decimal.Parse(elementos["Costo" + i]);
                Costo = Costo * Cambiodet;
                decimal importe = Costo * Cantidad;
                decimal ivac = importe * ivapro;
                decimal costot = importe + ivac;

                ClsDatoDecimal cantidadreal = db.Database.SqlQuery<ClsDatoDecimal>("select cantidadpedida as Dato from dbo.[DetRequisiciones] WHERE idrequisicion=" + norequisicion + "").ToList()[0];
                ClsDatoDecimal sum = db.Database.SqlQuery<ClsDatoDecimal>("select suministro as Dato from dbo.[DetRequisiciones] WHERE idrequisicion=" + norequisicion + "").ToList()[0];

                decimal suministro = 0;
                if (sum.Dato == 0)
                {
                    db.Database.ExecuteSqlCommand("update [EncRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                    db.Database.ExecuteSqlCommand("update [DetRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                }


                if (Cantidad > cantidadreal.Dato || Cantidad == cantidadreal.Dato)
                {
                    db.Database.ExecuteSqlCommand("update [EncRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                    db.Database.ExecuteSqlCommand("update [DetRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                }
                if (sum.Dato < cantidadreal.Dato || sum.Dato == cantidadreal.Dato)
                {
                    suministro = sum.Dato - Cantidad;
                    db.Database.ExecuteSqlCommand("update DetRequisiciones set nota='" + Nota + "', suministro=" + suministro + ", Costo =" + Costo + ",Importe=" + importe + ", IVA  =" + tieneiva + ",ImporteIVA=" + ivac + ", ImporteTotal=" + costot + ", status='Activo' where idrequisicion=" + norequisicion);
                    if (suministro <= 0)
                    {
                        db.Database.ExecuteSqlCommand("update [EncRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                        db.Database.ExecuteSqlCommand("update [DetRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);
                    }

                }





                db.Database.ExecuteSqlCommand("update CarritoRequisicion set nota='" + Nota + "', Cantidad=" + Cantidad + ", Costo =" + Costo + ",Importe=" + importe + ", IVA  =" + tieneiva + ",ImporteIVA=" + ivac + ", ImporteTotal=" + costot + " where IDCarritoRequisicion=" + IDCarrito);
                subtotalr += importe;
                ivar += ivac;
                totalr += costot;

                //db.Database.ExecuteSqlCommand("update [EncRequisiciones] set status='Finalizado' where idrequisicion=" + norequisicion);

            }

            List<VCarritoRequisicion> carritor = db.Database.SqlQuery<VCarritoRequisicion>("select Caracteristica.ID as IDCaracteristica,Articulo.IDArticulo,Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro, CarritoRequisicion.Nota,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();

            int num = 0;
            DateTime fecha = encOrdenCompra.Fecha;
            string fecha1 = fecha.ToString("yyyy/MM/dd");

            DateTime fechareq = encOrdenCompra.FechaRequiere;
            string fecha2 = fechareq.ToString("yyyy/MM/dd");


            string fechaatual = DateTime.Now.ToString("yyyy/MM/dd");

            db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncOrdenCompra]([Fecha],[FechaRequiere],[IDProveedor],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[TipoCambio],[UserID],[IDUsoCFDI], SujetoCalidad) values ('" + fecha1 + "','" + fecha2 + "','" + encOrdenCompra.IDProveedor + "','" + encOrdenCompra.IDFormapago + "'," + encOrdenCompra.IDMoneda + ",'" + encOrdenCompra.Observacion + "'," + subtotalr + "," + ivar + "," + totalr + "," + encOrdenCompra.IDMetodoPago + "," + encOrdenCompra.IDCondicionesPago + "," + encOrdenCompra.IDAlmacen + ",'Inactivo', " + Cambiodeldia + ",'" + UserID + "','" + encOrdenCompra.IDUsoCFDI + "','" + encOrdenCompra.SujetoCalidad + "')");



            List<EncOrdenCompra> numero;
            numero = db.Database.SqlQuery<EncOrdenCompra>("SELECT * FROM [dbo].[EncOrdenCompra] WHERE IDOrdenCompra = (SELECT MAX(IDOrdenCompra) from EncOrdenCompra)").ToList();
            num = numero.Select(s => s.IDOrdenCompra).FirstOrDefault();


            //foreach (VCarritoRequisicion itemcarritor in carritor)
            //{


            //    //Insercion de Detalle de Orden de Compra
            //    db.Database.ExecuteSqlCommand("INSERT INTO DetOrdenCompra([IDOrdenCompra],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna]) values ('" + num + "','" + itemcarritor.IDArticulo + "','" + itemcarritor.Cantidad + "','" + itemcarritor.Costo + "','" + itemcarritor.Cantidad + "','0','" + itemcarritor.Importe + "','true','" + itemcarritor.ImporteIva + "','" + itemcarritor.ImporteTotal + "','" + itemcarritor.Nota + "','0','" + itemcarritor.IDCaracteristica + "','" + encOrdenCompra.IDAlmacen + "','0','Inactivo','" + itemcarritor.Presentacion + "','" + itemcarritor.jsonPresentacion + "','" + itemcarritor.IDRequisicion + "')");
            //    List<DetOrdenCompra> numero2 = db.Database.SqlQuery<DetOrdenCompra>("SELECT * FROM [dbo].[DetOrdenCompra] WHERE IDDetOrdenCompra = (SELECT MAX(IDDetOrdenCompra) from DetOrdenCompra)").ToList();
            //    int num2 = numero2.Select(s => s.IDDetOrdenCompra).FirstOrDefault();
            //    db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('OrdenCompra'," + num + "," + itemcarritor.Cantidad + ",'" + fechaatual + "'," + itemcarritor.IDRequisicion + ",'Requisición'," + UserID + "," + itemcarritor.IDDetExterna + "," + num2 + ")");

            //    db.Database.ExecuteSqlCommand("delete from CarritoRequisicion where IDCarritoRequisicion='" + itemcarritor.IDCarritoRequisicion + "'");





            //}
            Hashtable tabla = new Hashtable();
            foreach (VCarritoRequisicion itemcarritor in carritor)
            {
                DetRequisiciones detalleR = db.Database.SqlQuery<DetRequisiciones>("select * from dbo.DetRequisiciones where idDetRequisiciones=" + itemcarritor.IDDetExterna + "").ToList().FirstOrDefault();

                string llave = detalleR.IDArticulo + "," + detalleR.Caracteristica_ID;

                if (!tabla.ContainsKey(llave))
                {
                    tabla.Add(llave, detalleR);
                    db.Database.ExecuteSqlCommand("INSERT INTO DetOrdenCompra([IDOrdenCompra],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna]) values ('" + num + "','" + itemcarritor.IDArticulo + "','" + itemcarritor.Cantidad + "','" + itemcarritor.Costo + "','" + itemcarritor.Cantidad + "','0','" + itemcarritor.Importe + "','true','" + itemcarritor.ImporteIva + "','" + itemcarritor.ImporteTotal + "','" + itemcarritor.Nota + "','0','" + itemcarritor.IDCaracteristica + "','" + encOrdenCompra.IDAlmacen + "','0','Inactivo','" + itemcarritor.Presentacion + "','" + itemcarritor.jsonPresentacion + "','" + detalleR.IDDetRequisiciones + "')");
                    db.Database.ExecuteSqlCommand("update detrequisiciones set suministro=" + itemcarritor.Cantidad + "where iddetrequisiciones=" + itemcarritor.IDDetExterna);

                    List<DetOrdenCompra> numero2 = db.Database.SqlQuery<DetOrdenCompra>("SELECT * FROM [dbo].[DetOrdenCompra] WHERE IDDetOrdenCompra = (SELECT MAX(IDDetOrdenCompra) from DetOrdenCompra)").ToList();
                    int num2 = numero2.Select(s => s.IDDetOrdenCompra).FirstOrDefault();
                    string cadenaMov = "insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Orden Compra','" + num + "','" + itemcarritor.Cantidad + "',SYSDATETIME(),'" + detalleR.IDRequisicion + "','Requisición','" + UserID + "','" + detalleR.IDDetRequisiciones + "','" + num2 + "')";
                    db.Database.ExecuteSqlCommand(cadenaMov);
                    db.Database.ExecuteSqlCommand("insert into [dbo].[elementosOrdenCompra] ([documento],[iddocumento],[iddetdocumento],cantidad,iddetOrdenCompra,idOrdenCompra ) values('Requisición'," + detalleR.IDRequisicion + "," + detalleR.IDDetRequisiciones + "," + itemcarritor.Cantidad + "," + num2 + "," + num + ")");
                }
                else
                {



                    List<DetOrdenCompra> numero2 = db.Database.SqlQuery<DetOrdenCompra>("select * from [dbo].[DetOrdenCompra] WHERE IDArticulo = " + detalleR.IDArticulo + " and Caracteristica_ID=" + detalleR.Caracteristica_ID + " and idOrdenCompra=" + num + "").ToList();
                    int num2 = numero2.Select(s => s.IDDetOrdenCompra).FirstOrDefault();
                    decimal cantidadDet = numero2.Select(s => s.Cantidad).FirstOrDefault();

                    DetRequisiciones anterior = (DetRequisiciones)tabla[llave];
                    DetRequisiciones nuevo = anterior;

                    nuevo.Cantidad = cantidadDet + itemcarritor.Cantidad;
                    nuevo.CantidadPedida = nuevo.CantidadPedida + itemcarritor.CantidadPedida;
                    nuevo.Nota = nuevo.Nota + " " + itemcarritor.Nota;
                    nuevo.Importe = Math.Round(nuevo.Cantidad * nuevo.Costo, 2);
                    nuevo.ImporteIva = Math.Round(nuevo.Importe * (decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA)), 2);
                    nuevo.ImporteTotal = nuevo.Importe + nuevo.ImporteIva;

                    tabla.Remove(llave);
                    tabla.Add(llave, nuevo);


                    db.Database.ExecuteSqlCommand("update DetOrdenCompra set nota='" + nuevo.Nota + "', cantidad=" + nuevo.Cantidad + ",cantidadpedida=" + nuevo.CantidadPedida + ",importe=" + nuevo.Importe + ",importeiva=" + nuevo.ImporteIva + ", importetotal=" + nuevo.ImporteTotal + " where iddetOrdenCompra=" + num2);
                    db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Orden Compra','" + num + "','" + detalleR.CantidadPedida + "',SYSDATETIME(),'" + detalleR.IDRequisicion + "','Requisición','" + UserID + "','" + detalleR.IDDetRequisiciones + "'," + num2 + ")");
                    db.Database.ExecuteSqlCommand("insert into [dbo].[elementosOrdenCompra] ([documento],[iddocumento],[iddetdocumento],cantidad,iddetOrdenCompra,idOrdenCompra ) values('Requisición'," + detalleR.IDRequisicion + "," + detalleR.IDDetRequisiciones + "," + itemcarritor.Cantidad + "," + num2 + "," + num + ")");

                }



                //  db.Database.ExecuteSqlCommand("update [dbo].[EncRequisiciones] set [Status]='Finalizado' where [IDRemision]='" + detalle.IDRemision + "'");
                db.Database.ExecuteSqlCommand("delete from CarritoRequisicion where IDCarritoRequisicion='" + itemcarritor.IDCarritoRequisicion + "'");

            }






            return RedirectToAction("Index");
        }

        public ActionResult VCarritocpartial(int IDProveedor, int IDMoneda)
        {
            string iduser = Session["UserID"].ToString();


            List<VCarritoRequisicion> carritor = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.Cref,Caracteristica.ID as IDCaracteristica,Articulo.IDArticulo,Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro, CarritoRequisicion.Nota,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + iduser + "'").ToList();

            foreach (VCarritoRequisicion item in carritor)
            {
                string cadena = "select* from MatrizPrecioProv where ranginf <= " + item.Cantidad + " and rangSup>= " + item.Cantidad + " and idproveedor = " + IDProveedor + " and IDArticulo = " + item.IDArticulo;
                MatrizPrecioProv mpp = new MatrizPrecioProveedorContext().Database.SqlQuery<MatrizPrecioProv>(cadena).FirstOrDefault();

                if (mpp == null)
                {

                }
                else
                {
                    decimal IVa = decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                    if (mpp.IDMoneda == IDMoneda)
                    {

                        decimal costo = mpp.Precio;
                        new MatrizPrecioProvContext().Database.ExecuteSqlCommand("update CarritoRequisicion set nota='" + mpp.Observacion + "', costo=" + costo + ", importe = cantidad *" + costo + " where idCarritoRequisicion =" + item.IDCarritoRequisicion);
                        new MatrizPrecioProvContext().Database.ExecuteSqlCommand("update CarritoRequisicion set importeiva=importe*" + IVa + ", importetotal = importe + (importe*" + IVa + ") where idCarritoRequisicion =" + item.IDCarritoRequisicion);

                    }
                    else
                    {
                        string cadenatc = "select  dbo. GetTipocambio(GetDate(), " + mpp.IDMoneda + ", " + IDMoneda + ") as Dato";
                        decimal Tc = new MatrizPrecioProvContext().Database.SqlQuery<ClsDatoDecimal>(cadenatc).FirstOrDefault().Dato;
                        decimal costo = mpp.Precio * Tc;
                        new MatrizPrecioProvContext().Database.ExecuteSqlCommand("update CarritoRequisicion set nota='" + mpp.Observacion + "',  costo=" + costo + ", importe = cantidad *" + costo + " where idCarritoRequisicion =" + item.IDCarritoRequisicion);
                        new MatrizPrecioProvContext().Database.ExecuteSqlCommand("update CarritoRequisicion set importeiva=importe*" + IVa + ", importetotal = importe + (importe*" + IVa + ") where idCarritoRequisicion =" + item.IDCarritoRequisicion);


                    }
                }



            }

            carritor = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.Cref,Caracteristica.ID as IDCaracteristica,Articulo.IDArticulo,Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro, CarritoRequisicion.Nota,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + iduser + "'").ToList();

            return PartialView("VCarritocpartial", carritor);
        }

        [HttpPost]
        public JsonResult DeleteitemCarrito(int id)
        {
            try
            {
                CarritoContext cr = new CarritoContext();
                CarritoRequisicion carritoRequisicion = cr.CarritoRequisiciones.Find(id);

                db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRequisicion] where [IDCarritoRequisicion]='" + id + "'");
                db.Database.ExecuteSqlCommand("update [dbo].[DetRequisiciones] set [Status]='Activo' where [IDDetRequisiciones]='" + carritoRequisicion.IDDetExterna + "'");
                db.Database.ExecuteSqlCommand("update [dbo].[EncRequisiciones] set [Status]='Activo' where [IDRequisicion]='" + carritoRequisicion.IDRequisicion + "'");

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        //////////////////delete carrito

        public ActionResult deletecarrito(int? idr)
        {
            CarritoContext cr = new CarritoContext();
            CarritoRequisicion carritoRequisicion = cr.CarritoRequisiciones.Find(idr);
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRequisicion] where [IDCarritoRequisicion]='" + idr + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetRequisiciones] set [Status]='Activo' where [IDDetRequisiciones]='" + carritoRequisicion.IDDetExterna + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[EncRequisiciones] set [Status]='Activo' where [IDRequisicion]='" + carritoRequisicion.IDRequisicion + "'");


            ViewBag.id = carritoRequisicion.IDRequisicion;



            ProveedorContext pr = new ProveedorContext();
            RequisicionesContext bd = new RequisicionesContext();
            EncRequisiciones encRequisicion = bd.EncRequisicioness.Find(carritoRequisicion.IDRequisicion);

            ViewBag.IDUsoCFDI = new SelectList(bd.c_UsoCFDIS.Where(s => s.IDUsoCFDI.Equals(encRequisicion.IDUsoCFDI)), "IDUsoCFDI", "Descripcion");
            ViewBag.IDAlmacen = new SelectList(bd.Almacenes.Where(s => s.IDAlmacen.Equals(encRequisicion.IDAlmacen)), "IDAlmacen", "Descripcion");

            //var proveedor = prov.Proveedores.ToList();

            List<SelectListItem> moneda = new List<SelectListItem>();
            c_Moneda mm1 = pr.c_Monedas.Find(encRequisicion.IDMoneda);
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
            c_MetodoPago metodop = pr.c_MetodoPagos.Find(encRequisicion.IDMetodoPago);
            metodo.Add(new SelectListItem { Text = metodop.Descripcion, Value = metodop.IDMetodoPago.ToString() });
            ViewBag.metodo = metodo;

            List<SelectListItem> forma = new List<SelectListItem>();
            c_FormaPago formap = pr.c_FormaPagos.Find(encRequisicion.IDFormapago);
            forma.Add(new SelectListItem { Text = formap.Descripcion, Value = formap.IDFormaPago.ToString() });
            ViewBag.forma = forma;

            List<SelectListItem> condiciones = new List<SelectListItem>();
            CondicionesPago condicionesp = pr.CondicionesPagos.Find(encRequisicion.IDCondicionesPago);
            condiciones.Add(new SelectListItem { Text = condicionesp.Descripcion, Value = condicionesp.IDCondicionesPago.ToString() });
            ViewBag.condiciones = condiciones;
            List<SelectListItem> li = new List<SelectListItem>();
            Proveedor mm = pr.Proveedores.Find(encRequisicion.IDProveedor);
            li.Add(new SelectListItem { Text = mm.Empresa, Value = mm.IDProveedor.ToString() });
            ViewBag.proveedor = li;

            ViewBag.data = null;

            ViewBag.otro = 0;

            ViewBag.datos = null;


            List<VCarritoRequisicion> lista = db.Database.SqlQuery<VCarritoRequisicion>("select Articulo.MinimoCompra,CarritoRequisicion.IDDetExterna, CarritoRequisicion.IDArticulo,CarritoRequisicion.IDCarritoRequisicion,CarritoRequisicion.IDRequisicion,CarritoRequisicion.Suministro,Articulo.Descripcion as Articulo,CarritoRequisicion.Cantidad,CarritoRequisicion.Costo,CarritoRequisicion.CantidadPedida,CarritoRequisicion.Descuento,CarritoRequisicion.Importe,CarritoRequisicion.IVA,CarritoRequisicion.ImporteIva,CarritoRequisicion.ImporteTotal,CarritoRequisicion.Nota,CarritoRequisicion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRequisicion INNER JOIN Caracteristica ON CarritoRequisicion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo ").ToList();
            ViewBag.carritor = lista;

            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRequisiciones.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRequisiciones.TipoCambio)) as TotalenPesos from CarritoRequisicion inner join EncRequisiciones on EncRequisiciones.IDRequisicion=CarritoRequisicion.IDRequisicion  group by EncRequisiciones.IDMoneda ").ToList();
            ViewBag.sumatoria = resumen;

            //return RedirectToAction("Requisiciones");
            return RedirectToAction("Requisiciones");


        }



        public ActionResult borracarrito()
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRequisicion] where [UserID]='" + UserID + "'");
            return RedirectToAction("Index");
        }

        public ActionResult PdfOrdenCompra(int id)
        {

            EncOrdenCompra Ordencompra = db.EncOrdenCompras.Find(id);
            DocumentoOrdenCompra x = new DocumentoOrdenCompra();

            x.claveMoneda = Ordencompra.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = Ordencompra.Fecha.ToShortDateString();
            x.fechaRequerida = Ordencompra.FechaRequiere.ToShortDateString();
            x.Proveedor = Ordencompra.Proveedor.Empresa;
            x.formaPago = Ordencompra.c_FormaPago.ClaveFormaPago;
            x.metodoPago = Ordencompra.c_MetodoPago.ClaveMetodoPago;
            x.RFCproveedor = Ordencompra.Proveedor.Telefonouno;
            x.total = float.Parse(Ordencompra.Total.ToString());
            x.subtotal = float.Parse(Ordencompra.Subtotal.ToString());
            x.Entregaren = Ordencompra.Entregaren;
            //  x.tipo_cambio = Ordencompra.TipoCambio.ToString();
            x.tipo_cambio =
              x.serie = "";
            x.folio = Ordencompra.IDOrdenCompra.ToString();
            x.UsodelCFDI = Ordencompra.c_UsoCFDI.Descripcion;
            x.IDALmacen = Ordencompra.Almacen.IDAlmacen;
            x.Telefonoproveedor = Ordencompra.Proveedor.Telefonouno;
            x.condicionesdepago = Ordencompra.CondicionesPago.Descripcion;
            x.Observacion = Ordencompra.Observacion;
            x.Autorizado = Ordencompra.Status;
            x.DireccionProveedor = x.DireccionProveedor = Ordencompra.Proveedor.Calle + " " + Ordencompra.Proveedor.NoExt + " " + Ordencompra.Proveedor.NoInt + "," + Ordencompra.Proveedor.Colonia + "," + Ordencompra.Proveedor.Municipio + "," + Ordencompra.Proveedor.Estados.Estado + "," + Ordencompra.Proveedor.paises.Pais;
            x.Confianza = Ordencompra.Proveedor.Confianza;
            ImpuestoOC iva = new ImpuestoOC();
            iva.impuesto = "IVA";
            iva.tasa = 16;
            iva.importe = float.Parse(Ordencompra.IVA.ToString());


            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
            x.firmadefinanzas = empresa.Director_finanzas;
            x.firmadecompras = empresa.Persona_de_Compras + "";

            List<DetOrdenCompra> detalles = db.Database.SqlQuery<DetOrdenCompra>("select * from [dbo].[DetOrdenCompra] where [IDOrdenCompra]= " + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoOC producto = new ProductoOC();
                Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);

                c_ClaveProductoServicio claveprodsat = db.Database.SqlQuery<c_ClaveProductoServicio>("select c_ClaveProductoServicio.* from (Articulo inner join Familia on articulo.IDFamilia= Familia.IDFamilia) inner join c_ClaveProductoServicio on c_ClaveProductoServicio.IDProdServ= Familia.IDProdServ where Articulo.IDArticulo= " + item.IDArticulo).ToList()[0];
                //  producto.ClaveProducto = claveprodsat.ClaveProdServ;
                producto.ClaveProducto = arti.Cref;
                producto.c_unidad = arti.c_ClaveUnidad.Nombre;
                producto.cantidad = item.Cantidad.ToString();
                producto.descripcion = arti.Descripcion;
                producto.valorUnitario = float.Parse(item.Costo.ToString());
                producto.v_unitario = float.Parse(item.Costo.ToString());
                producto.importe = float.Parse(item.Importe.ToString());
                producto.Tipo = new ArticuloContext().TipoArticulo.Find(arti.IDTipoArticulo).Descripcion.ToString();
                ///

                Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("SELECT * FROM Caracteristica where ID=" + item.Caracteristica_ID).ToList().FirstOrDefault();

                producto.Presentacion = item.Presentacion; //item.presentacion;
                producto.Observacion = item.Nota;
                ///
                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }



            //CreaOrdenCompraPDF documento = new CreaOrdenCompraPDF(logoempresa, x);
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            try
            {
                CreaOrdenCompraPDF documento = new CreaOrdenCompraPDF(logoempresa, x);
                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            return RedirectToAction("Index");
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


        public ActionResult CreaReporteporfecha()
        {
            return View();
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


        /// //////////Ordenes por Proveedor//////////
        public ActionResult IndexP(string Divisa, string Status, string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            VOrdenCompra3MContext dbv = new VOrdenCompra3MContext();
            SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select [IDProveedor] as Dato from [dbo].[Proveedores] where[RFC] ='" + User.Identity.Name + "'").ToList().FirstOrDefault();
            int p = c.Dato;

            Session["Proveedor"] = p;


            List<Proveedor> proveedor = db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where [IDProveedor]=" + p + " ").ToList();
            ViewBag.Proveedor = proveedor;
            var SerLst = new List<string>();
            var SerQry = from d in db.c_Monedas
                         orderby d.IDMoneda
                         select d.ClaveMoneda;
            SerLst.Add(" ");
            SerLst.AddRange(SerQry.Distinct());
            ViewBag.Divisa = new SelectList(SerLst);


            //var StaLst = new List<string>();
            //var StaQry = from d in dbv.VOrdenCompra3M
            //             orderby d.IDOrdenCompra
            //             select d.Status;
            //StaLst.Add(" ");
            //StaLst.AddRange(StaQry.Distinct());
            //ViewBag.Status = new SelectList(StaLst);

            //ViewBag.StatusSeleccionado = Status;
            ViewBag.DivisaSeleccionado = Divisa;

            string cadenaSQl = "select * from [dbo].[VOrdenCompra3M] ";
            string Filtro = string.Empty;
            string Filtro1 = string.Empty;
            string Filtro2 = string.Empty;
            string orden = "order by IDOrdenCompra desc";
            string cadena2 = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from VOrdenCompra3M";
            string grupo2 = "group by Moneda";



            //Filtro Divisa

            if (!String.IsNullOrEmpty(Divisa))
            {

                if (Filtro == string.Empty)
                {
                    Filtro = "where Moneda = '" + Divisa + "'";
                }
                else
                {
                    Filtro += " and Moneda = '" + Divisa + "'";
                }
            }

            //Filtro Status
            if (!String.IsNullOrEmpty(Status))
            {

                if (Filtro == string.Empty)
                {
                    Filtro = "where Status = '" + Status + "'";
                }
                else
                {
                    Filtro += "and Status = '" + Status + "'";
                }
            }

            //Ordenacion

            switch (sortOrder)
            {

                case "ClaveMoneda":
                    orden = " order by  Moneda asc ";
                    break;
                case "Status":
                    orden = " order by Status desc ";
                    break;
                default:
                    orden = " order by IDOrdenCompra desc ";
                    break;
            }


            ViewBag.CurrentSort = sortOrder;
            ViewBag.OrdenCompraSortParm = String.IsNullOrEmpty(sortOrder) ? "OrdenCompra" : "OrdenCompra";
            ViewBag.ProveedorSortParm = String.IsNullOrEmpty(sortOrder) ? "Proveedor" : "Proveedor";


            ViewBag.CurrentFilter = searchString;
            if (Filtro == string.Empty)
            {
                Filtro = "where [IDProveedor] = " + p + "";
            }
            else
            {
                Filtro += " and [IDProveedor] = " + p + "";
            }

            string ConsultaSql = cadenaSQl + " " + Filtro + " " + orden;
            var elementos = dbv.Database.SqlQuery<VOrdenCompra3M>(ConsultaSql).ToList();

            //Filtro2 = "where IDProveedor = " + p + " " + Filtro;
            string cadenaResumen = cadena2 + " " + Filtro + " " + grupo2;
            var divisa = db.Database.SqlQuery<ResumenFac>(cadenaResumen).ToList();
            ViewBag.sumatoria = divisa;
            ViewBag.Status = Status;
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbv.VOrdenCompra3M.Count(); // Total number of elements

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
        ////
        public ActionResult DetailsP(int? id)
        {
            List<VDetOrdenCompra> orden = db.Database.SqlQuery<VDetOrdenCompra>("select DetOrdenCompra.IDDetExterna,Articulo.MinimoCompra,DetOrdenCompra.IDOrdenCompra,DetOrdenCompra.Suministro,Articulo.Descripcion as Articulo,DetOrdenCompra.Cantidad,DetOrdenCompra.Costo,DetOrdenCompra.CantidadPedida,DetOrdenCompra.Descuento,DetOrdenCompra.Importe,DetOrdenCompra.IVA,DetOrdenCompra.ImporteIva,DetOrdenCompra.ImporteTotal, DetOrdenCompra.Nota,DetOrdenCompra.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetOrdenCompra.Status from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncOrdenCompra.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncOrdenCompra inner join Proveedores on Proveedores.IDProveedor=EncOrdenCompra.IDProveedor  where EncOrdenCompra.IDOrdenCompra='" + id + "' group by EncOrdenCompra.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EncOrdenCompra encOrdenCompra = db.EncOrdenCompras.Find(id);
            if (encOrdenCompra == null)
            {
                return HttpNotFound();
            }
            return View(encOrdenCompra);
        }
        ///  Cargar Factura por proveedor ////
        ///  

        public ActionResult CreatedesdeArchivoF(string returnUrl, int? id)
        {
            SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].[ContactosProv] where [Email] ='" + User.Identity.Name + "'").ToList()[0];
            int p = c.Dato;
            List<Proveedor> proveedor = db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where [IDProveedor]=" + p + "").ToList();
            ViewBag.Proveedor = proveedor;
            ViewBag.IDOrden = id;
            return View();

        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreatedesdeArchivoF(FormCollection collection, string returnUrl, int id)
        {
            EncfacturaProv elemento = new EncfacturaProv();

            try
            {
                /*Aqui voy intentar guardar la imagen que me pase la vista*/
                try
                {
                    HttpPostedFileBase archivo = Request.Files["Imag1"];
                    //archivo.SaveAs(Path.Combine(directory, Path.GetFileName(archivo.FileName)));
                    // Generador.CreaPDF(Path.Combine(@".\facturas", Path.GetFileName(archivo.FileName)));
                    StreamReader reader = new StreamReader(archivo.InputStream);
                    String contenidoxml = reader.ReadToEnd();


                    Generador.CreaPDF temp = new Generador.CreaPDF(contenidoxml);

                    elemento.Fecha = System.DateTime.Parse(temp._templatePDF.fechaEmisionCFDI);




                    try
                    {
                        elemento.Numero = temp._templatePDF.folio;
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                        elemento.Numero = "0";
                    }
                    elemento.Nombre_Proveedor = temp._templatePDF.emisor.Nombre;

                    // verifica si el proveedor existe


                    try
                    {
                        ProveedorContext dbp = new ProveedorContext();
                        Proveedor proveedorenlabase = dbp.Database.SqlQuery<Proveedor>("select * from proveedores where Empresa='" + temp._templatePDF.emisor.Nombre + "'").ToList()[0];
                        int IDpbase = proveedorenlabase.IDProveedor; /// si la consulta no devolvio fila lanazara una excepcion 
                        elemento.IDProveedor = IDpbase;
                    }
                    catch (Exception err)
                    {
                        //string error = err.Message;
                        //string mensajealusuario = "EL PROVEEDOR NO SE ENCUENTRA REGISTRADO O SU NOMBRE NO CONSIDE CON EXACTITUD A TU REGISTRO VERIFICA PUNTOS, COMAS Y ESPACIOS ";
                        //ViewBag.Mensajeerror = mensajealusuario;
                        //return View();
                        elemento.IDProveedor = 0;
                    }

                    try
                    {
                        EncfacturaProvContext dbp = new EncfacturaProvContext();
                        EncfacturaProv proveedorenlabase = dbp.Database.SqlQuery<EncfacturaProv>("select * from EncfacturaProv where UUID='" + temp._templatePDF.folioFiscalUUID + "'").ToList()[0];
                        int IDpbase = proveedorenlabase.ID; /// si la consulta no devolvio fila lanazara una excepcion 
                        string mensajealusuario = "LA FACTURA YA SE ENCUENTRA EN EL SISTEMA ";
                        ViewBag.Mensajeerror = mensajealusuario;
                        //return View();
                        return RedirectToAction("IndexP");
                    }
                    catch (Exception err)
                    {
                        //string error = err.Message;
                    }

                    elemento.Subtotal = decimal.Parse(temp._templatePDF.subtotal.ToString());
                    elemento.Total = decimal.Parse(temp._templatePDF.total.ToString());
                    elemento.IVA = elemento.Total - elemento.Subtotal;

                    c_MonedaContext db = new c_MonedaContext();
                    List<c_Moneda> clavemoneda;
                    clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE claveMoneda='" + temp._templatePDF.claveMoneda + "'").ToList();
                    int clave = clavemoneda.Select(s => s.IDMoneda).FirstOrDefault();
                    elemento.IDMoneda = clave;
                    elemento.Moneda = temp._templatePDF.claveMoneda;
                    elemento.Estado = "A";
                    if (temp._templatePDF.tipo_cambio != "")
                    {
                        elemento.TC = decimal.Parse(temp._templatePDF.tipo_cambio);
                    }
                    else
                    {
                        elemento.TC = 1;
                    }

                    elemento.RutaXML = contenidoxml;

                    elemento.UUID = temp._templatePDF.folioFiscalUUID;

                    elemento.Metododepago = temp._templatePDF.metodoPago;
                    elemento.Formadepago = temp._templatePDF.formaPago;
                    elemento.ConPagos = false;
                    EncfacturaProvContext dbe = new EncfacturaProvContext();
                    dbe.EncfacturaProveedores.Add(elemento);
                    dbe.SaveChanges();
                    var idFac = dbe.Database.SqlQuery<EncfacturaProv>("select ID from dbo.EncFacturaProv where UUID =" + elemento.UUID + "").ToList();
                    ViewBag.idFac = idFac;

                    //Relacionar la orden y la factura
                    db.Database.ExecuteSqlCommand("Insert into dbo.EncOrdenFact(IDOrdenCompra, ID ) Values(" + id + "," + idFac + ")");
                    //return RedirectToAction("Index");
                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("IndexP");
                }
                catch (Exception ERR)
                {
                    ViewBag.Mensajeerror = ERR.InnerException.Message;
                    return View();
                }


            }
            catch (Exception ERR2)
            {
                ViewBag.Mensajeerror = "Este archivo Xml no contiene una factura valida";
                return View();
            }
        }

        public JsonResult getmetodo(int id)
        {
            Proveedor proveedor = prov.Proveedores.Find(id);
            int a = 0;
            a = proveedor.IDMetodoPago;

            return Json(a, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getforma(int id)
        {
            Proveedor proveedor = prov.Proveedores.Find(id);
            int a = 0;
            a = proveedor.IDFormaPago;

            return Json(a, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getmoneda(int id)
        {
            Proveedor proveedor = prov.Proveedores.Find(id);
            int a = 0;
            a = proveedor.IDMoneda;

            return Json(a, JsonRequestBehavior.AllowGet);
        }
        ////////////////


        public JsonResult getcondiciones(int id)
        {
            Proveedor proveedor = prov.Proveedores.Find(id);
            int a = 0;
            a = proveedor.IDCondicionesPago;

            return Json(a, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Edititem(int id, decimal cantidad, string nota, decimal Precio)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Cantidad a cambiar " + cantidad);
                CarritoContext car = new CarritoContext();
                db.Database.ExecuteSqlCommand("update [dbo].[CarritoC] set nota='" + nota + "',[Cantidad]=" + cantidad + ",  Precio=" + Precio + "  where IDCarrito=" + id);
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        public ActionResult RutaPDF(int id)
        {

            EncOrdenCompra Ordencompra = db.EncOrdenCompras.Find(id);
            DocumentoRutaOrdenCompra x = new DocumentoRutaOrdenCompra();

            x.claveMoneda = Ordencompra.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = Ordencompra.Fecha.ToShortDateString();
            x.fechaRequerida = Ordencompra.FechaRequiere.ToShortDateString();
            x.Proveedor = Ordencompra.Proveedor.Empresa;
            x.formaPago = Ordencompra.c_FormaPago.ClaveFormaPago;
            x.metodoPago = Ordencompra.c_MetodoPago.ClaveMetodoPago;
            x.RFCproveedor = Ordencompra.Proveedor.Telefonouno;
            x.total = float.Parse(Ordencompra.Total.ToString());
            x.subtotal = float.Parse(Ordencompra.Subtotal.ToString());
            x.Entregaren = Ordencompra.Entregaren;
            //  x.tipo_cambio = Ordencompra.TipoCambio.ToString();
            x.tipo_cambio =
              x.serie = "";
            x.folio = Ordencompra.IDOrdenCompra.ToString();
            x.UsodelCFDI = Ordencompra.c_UsoCFDI.Descripcion;
            x.IDALmacen = Ordencompra.Almacen.IDAlmacen;
            x.Telefonoproveedor = Ordencompra.Proveedor.Telefonouno;
            x.condicionesdepago = Ordencompra.CondicionesPago.Descripcion;
            x.Observacion = Ordencompra.Observacion;
            x.Autorizado = Ordencompra.Status;
            x.DireccionProveedor = x.DireccionProveedor = Ordencompra.Proveedor.Calle + " " + Ordencompra.Proveedor.NoExt + " " + Ordencompra.Proveedor.NoInt + "," + Ordencompra.Proveedor.Colonia + "," + Ordencompra.Proveedor.Municipio + "," + Ordencompra.Proveedor.Estados.Estado + "," + Ordencompra.Proveedor.paises.Pais;

            ImpuestoRutaOC iva = new ImpuestoRutaOC();
            iva.impuesto = "IVA";
            iva.tasa = 16;
            iva.importe = float.Parse(Ordencompra.IVA.ToString());


            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
            x.firmadefinanzas = empresa.Director_finanzas;
            x.firmadecompras = empresa.Persona_de_Compras + "";

            List<DetOrdenCompra> detalles = db.Database.SqlQuery<DetOrdenCompra>("select * from [dbo].[DetOrdenCompra] where [IDOrdenCompra]= " + id).ToList();

            int contador = 1;
            foreach (DetOrdenCompra item in detalles)
            {
                ProductoRutaOC producto = new ProductoRutaOC();
                Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);

                c_ClaveProductoServicio claveprodsat = db.Database.SqlQuery<c_ClaveProductoServicio>("select c_ClaveProductoServicio.* from (Articulo inner join Familia on articulo.IDFamilia= Familia.IDFamilia) inner join c_ClaveProductoServicio on c_ClaveProductoServicio.IDProdServ= Familia.IDProdServ where Articulo.IDArticulo= " + item.IDArticulo).ToList()[0];
                //  producto.ClaveProducto = claveprodsat.ClaveProdServ;
                producto.ClaveProducto = arti.Cref;
                producto.c_unidad = arti.c_ClaveUnidad.Nombre;
                producto.cantidad = item.Cantidad.ToString();
                producto.iddetordencompra = item.IDDetOrdenCompra;
                producto.descripcion = arti.Descripcion;
                producto.valorUnitario = float.Parse(item.Costo.ToString());
                producto.v_unitario = float.Parse(item.Costo.ToString());
                producto.importe = float.Parse(item.Importe.ToString());
                producto.Tipo = new ArticuloContext().TipoArticulo.Find(arti.IDTipoArticulo).Descripcion.ToString();
                ///
                producto.suministro = item.Suministro;
                Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("SELECT * FROM Caracteristica where ID=" + item.Caracteristica_ID).ToList().FirstOrDefault();

                producto.Presentacion = item.Presentacion; //item.presentacion;
                producto.Observacion = item.Nota;
                ///
                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }



            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            try
            {


                CreaRutaOrdenCompraPDF documento = new CreaRutaOrdenCompraPDF(logoempresa, x);

            }
            catch (Exception err)
            {
                String mensaje = err.Message;
            }
            return RedirectToAction("Index");

        }



        public ActionResult EntreFechasOC()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasOC(EFecha modelo, FormCollection coleccion)
        {
            VRemisionClieContext dbr = new VRemisionClieContext();
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

                cadena = "select * from [dbo].[VOrdenCompra] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' ";
                var datos = dbr.Database.SqlQuery<VOrdenCompra>(cadena).ToList();
                ViewBag.datos = datos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Ordenes de Compra");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:U1"].Style.Font.Size = 20;
                Sheet.Cells["A1:U1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:U3"].Style.Font.Bold = true;
                Sheet.Cells["A1:U1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:U1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Ordenes de Compra");

                row = 2;
                Sheet.Cells["A1:U1"].Style.Font.Size = 12;
                Sheet.Cells["A1:U1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:U1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:U2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3:U3"].Style.Font.Bold = true;
                Sheet.Cells["A3:U3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:U3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Orden Compra");
                Sheet.Cells["B3"].RichText.Add("Fecha"); ;
                Sheet.Cells["C3"].RichText.Add("Fecha Requerida");
                Sheet.Cells["D3"].RichText.Add("RFC");
                Sheet.Cells["E3"].RichText.Add("Empresa");
                Sheet.Cells["F3"].RichText.Add("Clave Metodo de Pago");
                Sheet.Cells["G3"].RichText.Add("Metodo de Pago");
                Sheet.Cells["H3"].RichText.Add("Clave Forma de Pago");
                Sheet.Cells["I3"].RichText.Add("Forma de Pago");
                Sheet.Cells["J3"].RichText.Add("Clave Condiciones de Pago");
                Sheet.Cells["K3"].RichText.Add("Condiciones de Pago");
                Sheet.Cells["L3"].RichText.Add("Subtotal");
                Sheet.Cells["M3"].RichText.Add("Iva");
                Sheet.Cells["N3"].RichText.Add("Total");
                Sheet.Cells["O3"].RichText.Add("Moneda");
                Sheet.Cells["P3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["Q3"].RichText.Add("Total en Pesos");
                Sheet.Cells["R3"].RichText.Add("Status");
                Sheet.Cells["S3"].RichText.Add("Almacen");
                Sheet.Cells["T3"].RichText.Add("Observación");
                Sheet.Cells["U3"].RichText.Add("Genero");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:U3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VOrdenCompra item in ViewBag.datos)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDOrdenCompra;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.FechaRequiere;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.RFC;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Empresa;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.ClaveMetodoPago;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.MetodoPago;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.ClaveFormaPago;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.FormaPago;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.ClaveCondicionesPago;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.CondicionesPago;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.TotalPesos;
                    Sheet.Cells[string.Format("R{0}", row)].Value = item.EstadoOC;
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.Almacen;
                    Sheet.Cells[string.Format("T{0}", row)].Value = item.Observacion;
                    Sheet.Cells[string.Format("U{0}", row)].Value = item.Username;
                    
                    row++;
                }


                //DETALLE DE LA OC
                try
                {
                    cadena = "select * from [dbo].[VDetOC] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' ";
                      var datosD = dbr.Database.SqlQuery<VDetOC>(cadena).ToList();

                    Sheet = Ep.Workbook.Worksheets.Add("Detalle Ordenes de Compra");

                    // en la fila1 formateo las celdas y coloco el título de la hoja
                    // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                    row = 1;
                    //Fijar la fuente para A1:Q1
                    Sheet.Cells["A1:U1"].Style.Font.Size = 20;
                    Sheet.Cells["A1:U1"].Style.Font.Name = "Calibri";
                    Sheet.Cells["A1:U3"].Style.Font.Bold = true;
                    Sheet.Cells["A1:U1"].Style.Font.Color.SetColor(Color.DarkBlue);
                    Sheet.Cells["A1:U1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    Sheet.Cells["A1"].RichText.Add("Listado de Detalles Ordenes de Compra");

                    row = 2;
                    Sheet.Cells["A1:U1"].Style.Font.Size = 12;
                    Sheet.Cells["A1:U1"].Style.Font.Name = "Calibri";
                    Sheet.Cells["A1:U1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                    Sheet.Cells["A2:U2"].Style.Font.Bold = true;
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
                    Sheet.Cells["A3:L3"].Style.Font.Bold = true;
                    Sheet.Cells["A3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["A3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    // Se establece el nombre que identifica a cada una de las columnas de datos.
                    Sheet.Cells["A3"].RichText.Add("Orden Compra");
                    Sheet.Cells["B3"].RichText.Add("Fecha"); ;
                    Sheet.Cells["C3"].RichText.Add("Clave");
                    Sheet.Cells["D3"].RichText.Add("Descripción");
                    Sheet.Cells["E3"].RichText.Add("Presentación");
                    Sheet.Cells["F3"].RichText.Add("Nota");
                    Sheet.Cells["G3"].RichText.Add("Cantidad");
                    Sheet.Cells["H3"].RichText.Add("Costo");
                    Sheet.Cells["I3"].RichText.Add("Importe");
                    Sheet.Cells["J3"].RichText.Add("Status");
                    Sheet.Cells["K3"].RichText.Add("Almacén");
                    
                    //Aplicar borde doble al rango de celdas A3:Q3
                    Sheet.Cells["A3:L3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                    // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                    // Se establecen los formatos para las celdas: Fecha, Moneda
                    row = 4;
                    Sheet.Cells.Style.Font.Bold = false;
                    foreach (VDetOC item in datosD)
                    {

                        Sheet.Cells[string.Format("A{0}", row)].Value = item.IDOrdenCompra;
                        Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                        Sheet.Cells[string.Format("C{0}", row)].Value = item.Cref;
                        Sheet.Cells[string.Format("D{0}", row)].Value = item.Descripcion;
                        Sheet.Cells[string.Format("E{0}", row)].Value = "NP: "+item.IDPresentacion + " "+ item.Presentacion;
                        Sheet.Cells[string.Format("F{0}", row)].Value = item.Nota;
                        Sheet.Cells[string.Format("G{0}", row)].Value = item.Cantidad;
                        Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("H{0}", row)].Value = item.Costo;
                        Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("I{0}", row)].Value = item.Importe;
                        Sheet.Cells[string.Format("J{0}", row)].Value = item.Status;
                        Sheet.Cells[string.Format("K{0}", row)].Value = item.Almacen;
                        
                        
                        row++;
                    }


                }
                catch (Exception err)
                {

                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "OrdenCompra.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }


        public JsonResult JsonProveedor(int id)
        {
            string USERID = Session["UserID"].ToString();
            Proveedor prov = new ProveedorContext().Proveedores.Find(id);
            if (prov != null)
            {
                datosprov nuevo = new datosprov();
                nuevo.IDCondicionesPago = prov.IDCondicionesPago;
                nuevo.IDFormapago = prov.IDFormaPago;
                nuevo.IDMetodoPago = prov.IDMetodoPago;
                nuevo.IDMoneda = prov.IDMoneda;

                //////////////aqui el codigo actualizando el precio segun el prov////////////////

              //  new CarritoPContext().Database.ExecuteSqlCommand("update dbo.CarritoRequisicion set costo=1 where  userID=" + USERID);

                return   Json(nuevo, JsonRequestBehavior.AllowGet);
            }
        
            else
            {
                var errorModel = new { error = "No se encontro el Proveedor" };
                return new JsonHttpStatusResult(errorModel, HttpStatusCode.InternalServerError);
            }
        }




        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// REPORTE DE OC VS RECEPCION DIAS DIFERENCIA
        /// </summary>
        public ActionResult EntreFechasOCvsREC()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasOCvsREC(EFecha modelo, FormCollection coleccion)
        {
            VRemisionClieContext dbr = new VRemisionClieContext();
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

                cadena = "	select distinct(oc.idordencompra), oc.fecha, oc.fecharequiere, oc.empresa, oc.total, oc.clavemoneda, oc.tipoCambio, oc.TotalPesos, oc.EstadoOC, er.idrecepcion, er.fecha as FechaRecepcion, DATEDIFF(day, oc.fecha, er.fecha) as diasDiferencia from [VOrdenCompra] as oc inner join detordencompra as doc on oc.idordencompra=doc.idordencompra inner join detrecepcion as dr on dr.idexterna=oc.idordencompra inner join encrecepcion as er on er.idrecepcion=dr.idrecepcion where er.status<>'Cancelado' and  oc.Fecha >= '" + FI + "' and oc.Fecha <='" + FF + "' ";
                var datos = dbr.Database.SqlQuery<VOrdenCompraRecepcion>(cadena).ToList();
                ViewBag.datos = datos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Ordenes de Compra");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:L1"].Style.Font.Size = 20;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L3"].Style.Font.Bold = true;
                Sheet.Cells["A1:L1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Ordenes de Compra vs Recepción");

                row = 2;
                Sheet.Cells["A1:L1"].Style.Font.Size = 12;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:L2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3:L3"].Style.Font.Bold = true;
                Sheet.Cells["A3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("No. Orden Compra");
                Sheet.Cells["B3"].RichText.Add("Fecha"); ;
                Sheet.Cells["C3"].RichText.Add("Fecha Requerida");
               
                Sheet.Cells["D3"].RichText.Add("Empresa");
                
                Sheet.Cells["E3"].RichText.Add("Total");
                Sheet.Cells["F3"].RichText.Add("Moneda");
                Sheet.Cells["G3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["H3"].RichText.Add("Total en Pesos");
                Sheet.Cells["I3"].RichText.Add("Status");
                Sheet.Cells["J3"].RichText.Add("No. Recepción");
                Sheet.Cells["K3"].RichText.Add("Fecha Recepción");
                Sheet.Cells["L3"].RichText.Add("Días Diferencia");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:L3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VOrdenCompraRecepcion item in ViewBag.datos)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDOrdenCompra;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.FechaRequiere;
                    
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Empresa;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.TotalPesos;
                    string EstadoOrdenC = "";
                    if (item.EstadoOC == "Activo")
                    {
                        EstadoOrdenC = "Por Recepcionar";
                    }
                    if (item.EstadoOC == "Finalizado")
                    {
                        EstadoOrdenC = "Recepcionada";
                    }
                    if (item.EstadoOC == "Inactivo")
                    {
                        EstadoOrdenC = "Inactivo";
                    }
                    if (item.EstadoOC == "Cancelado")
                    {
                        EstadoOrdenC = "Cancelada";
                    }
                    Sheet.Cells[string.Format("I{0}", row)].Value = EstadoOrdenC;


                  
                            Sheet.Cells[string.Format("J{0}", row)].Value = item.IDRecepcion;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.FechaRecepcion;

                    //string fechaOrdenC = item.Fecha.ToString("yyyy-MM-dd");
                    //string fechaORecepcion = item.FechaRecepcion.ToString("yyyy-MM-dd");
                    //string fechaOrdenC = item.Fecha.Year.ToString() + "-" + item.Fecha.Month.ToString() + "-" + item.Fecha.Day.ToString();
                    //string fechaORecepcion = item.FechaRecepcion.Year.ToString() + "-" + item.FechaRecepcion.Month.ToString() + "-" + item.FechaRecepcion.Day.ToString();





                    Sheet.Cells[string.Format("L{0}", row)].Value = DiasHabiles(item.Fecha, item.FechaRecepcion) ;

                           

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "OrdenCompravsRecepcion.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }
        public int DiasHabiles(DateTime firstDay, DateTime lastDay)
        {
            List<DateTime> bankHolidays = new List<DateTime>();
            DateTime uno = DateTime.Parse(DateTime.Now.Year + "/05/01");
            DateTime dos = DateTime.Parse(DateTime.Now.Year + "/05/05");
            DateTime tres = DateTime.Parse(DateTime.Now.Year + "/12/24");
            DateTime cuatro = DateTime.Parse(DateTime.Now.Year + "/12/25");
            DateTime cinco = DateTime.Parse(DateTime.Now.Year + "/12/12");
            DateTime seis = DateTime.Parse(DateTime.Now.Year + "/12/31");
            DateTime siete = DateTime.Parse(DateTime.Now.Year + "/01/01");
            DateTime ocho = DateTime.Parse(DateTime.Now.Year + "/12/24");
            DateTime nueve = DateTime.Parse(DateTime.Now.Year + "/09/16");
            bankHolidays.Add(uno);
            bankHolidays.Add(dos);
            bankHolidays.Add(tres);
            bankHolidays.Add(cuatro);
            bankHolidays.Add(cinco);
            bankHolidays.Add(seis);
            bankHolidays.Add(siete);
            bankHolidays.Add(ocho);
            bankHolidays.Add(nueve);

            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            //if (firstDay > lastDay)
            //    throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days;// + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }


        public class JsonHttpStatusResult : JsonResult
        {
            private readonly HttpStatusCode _httpStatus;

            public JsonHttpStatusResult(object data, HttpStatusCode httpStatus)
            {
                Data = data;
                _httpStatus = httpStatus;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                context.RequestContext.HttpContext.Response.StatusCode = (int)_httpStatus;
                base.ExecuteResult(context);


            }
        }



        //////SUAJES
        ///
        //public ActionResult HistoriaSuajes( string Clave, string Status, string Fechainicio, string Fechafinal, int? page, int? PageSize, int IDFamilia = 0)
        //{
        //    int pageNumber = 0;
        //    int pageSize = 0;
        //    int count = 0;
        //    if (Clave == null)
        //    {
        //        Clave = "";
        //    }

        //    ViewBag.IDFamilia = new FamAlmRepository().GetAlmacenesF(IDFamilia);

        //    var StaLst = new List<string>();
        //    var StaQry = from d in db.EncOrdenCompras
        //                 orderby d.IDOrdenCompra
        //                 select d.Status;
        //    StaLst.Add(" ");
        //    StaLst.AddRange(StaQry.Distinct());
        //    ViewBag.Status = new SelectList(StaLst);
        //    ViewBag.StatusSeleccionado = Status;

        //    List<VHistoriaSuajes> almacen = new List<VHistoriaSuajes>();
        //    string cadena1 = "";
        //    if (Fechainicio == "")
        //    {
        //        Fechainicio = string.Empty;
        //    }
        //    if (Fechafinal == "")
        //    {
        //        Fechafinal = string.Empty;
        //    }
        //    if (Status == "")
        //    {
        //        Status = string.Empty;
        //    }
        //    if (Fechafinal == string.Empty)
        //    {
        //        Fechafinal = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
        //    }
        //    if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
        //    {
        //        cadena1 = "select d.iddetordencompra,d.Nota, o.idordencompra, o.fecha, o.fecharequiere, d.cantidad, a.cref, c.presentacion, c.idpresentacion, d.importe, d.status from encordencompra as o inner join detordencompra as d on o.idordencompra=d.idordencompra inner join articulo as a on a.idarticulo=d.idarticulo inner join caracteristica as c on c.id=d.caracteristica_id where  BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";


        //    }
        //    if (IDFamilia != 0)
        //    {
        //        if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
        //        {
        //            cadena1 = "select d.iddetordencompra, d.Nota, o.idordencompra, o.fecha, o.fecharequiere, d.cantidad, a.cref, c.presentacion, c.idpresentacion, d.importe, d.status from encordencompra as o inner join detordencompra as d on o.idordencompra=d.idordencompra inner join articulo as a on a.idarticulo=d.idarticulo inner join caracteristica as c on c.id=d.caracteristica_id where a.idfamilia=" + IDFamilia + "and Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'"; 
        //        }
        //        else
        //        {
        //            cadena1 = "select d.iddetordencompra, d.Nota, o.idordencompra, o.fecha, o.fecharequiere, d.cantidad, a.cref, c.presentacion, c.idpresentacion, d.importe, d.status from encordencompra as o inner join detordencompra as d on o.idordencompra=d.idordencompra inner join articulo as a on a.idarticulo=d.idarticulo inner join caracteristica as c on c.id=d.caracteristica_id where a.idfamilia=" + IDFamilia;

        //        }

        //    }
        //    else
        //    {
        //        cadena1 = "select d.iddetordencompra, d.Nota, o.idordencompra, o.fecha, o.fecharequiere, d.cantidad, a.cref, c.presentacion, c.idpresentacion, d.importe, d.status from encordencompra as o inner join detordencompra as d on o.idordencompra=d.idordencompra inner join articulo as a on a.idarticulo=d.idarticulo inner join caracteristica as c on c.id=d.caracteristica_id order by o.idordencompra";

        //    }
        //    if (Clave != "")
        //    {
        //        if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
        //        {
        //            cadena1 = "select d.iddetordencompra, d.Nota, o.idordencompra, o.fecha, o.fecharequiere, d.cantidad, a.cref, c.presentacion, c.idpresentacion, d.importe, d.status from encordencompra as o inner join detordencompra as d on o.idordencompra=d.idordencompra inner join articulo as a on a.idarticulo=d.idarticulo inner join caracteristica as c on c.id=d.caracteristica_id where (a.cref like '%" + Clave + "%' or d.Nota like '%"+Clave+"%') and Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
        //        }
        //        else
        //        {
        //            cadena1 = "select d.iddetordencompra, d.Nota, o.idordencompra, o.fecha, o.fecharequiere, d.cantidad, a.cref, c.presentacion, c.idpresentacion, d.importe, d.status from encordencompra as o inner join detordencompra as d on o.idordencompra=d.idordencompra inner join articulo as a on a.idarticulo=d.idarticulo inner join caracteristica as c on c.id=d.caracteristica_id where (a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%')";

        //        }

        //    }
        //    if (Clave != "" && IDFamilia!=0)
        //    {
        //        if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
        //        {
        //            cadena1 = "select d.iddetordencompra,d.Nota, o.idordencompra, o.fecha, o.fecharequiere, d.cantidad, a.cref, c.presentacion, c.idpresentacion, d.importe, d.status from encordencompra as o inner join detordencompra as d on o.idordencompra=d.idordencompra inner join articulo as a on a.idarticulo=d.idarticulo inner join caracteristica as c on c.id=d.caracteristica_id where (a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%') and a.idfamilia=" + IDFamilia +" and Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'"; 

                    

        //        }
        //        else
        //        {
        //            cadena1 = "select d.iddetordencompra, d.Nota,o.idordencompra, o.fecha, o.fecharequiere, d.cantidad, a.cref, c.presentacion, c.idpresentacion, d.importe, d.status from encordencompra as o inner join detordencompra as d on o.idordencompra=d.idordencompra inner join articulo as a on a.idarticulo=d.idarticulo inner join caracteristica as c on c.id=d.caracteristica_id where (a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%') and a.idfamilia=" + IDFamilia;

        //        }

        //    }
           


        //    if (!String.IsNullOrEmpty(Status))
        //    {
        //        if (IDFamilia != 0)
        //        {
        //            cadena1 = cadena1 + " and o.status='" + Status + "'";
        //        }
        //        else if (Clave != "")
        //        {
        //            cadena1 = cadena1 + " and o.status='" + Status + "'";
        //        }
        //        else if (Clave != "" && IDFamilia != 0)
        //        {
        //            cadena1 = cadena1 + " and o.status='" + Status + "'";
        //        }
        //        if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
        //        {
        //            cadena1 = cadena1 + " and o.status='" + Status + "'";
        //        }
        //        else
        //        {
        //            cadena1 = cadena1 + "  where o.status='" + Status + "'";
        //        }
        //    }


          

            
               
                


        //    almacen = db.Database.SqlQuery<VHistoriaSuajes>(cadena1).OrderByDescending(s=> s.IDOrdenCompra).ToList();

        //    //Paginación
        //    // DROPDOWNLIST FOR UPDATING PAGE SIZE
        //    count = almacen.Count(); // Total number of elements

        //    // Populate DropDownList
        //    ViewBag.PageSize = new List<SelectListItem>()
        //    {
        //        new SelectListItem { Text = "10", Value = "10" },
        //        new SelectListItem { Text = "25", Value = "25", Selected = true },
        //        new SelectListItem { Text = "50", Value = "50" },
        //        new SelectListItem { Text = "100", Value = "100" },
        //        new SelectListItem { Text = "Todo", Value = count.ToString() }
        //     };
        //    ViewBag.CurrentFilter = Clave;
        //    ViewBag.FamiliaSeleccionada = IDFamilia;
        //    pageNumber = (page ?? 1);
        //    pageSize = (PageSize ?? 25);
        //    ViewBag.psize = pageSize;
       
        //    return View(almacen.ToPagedList(pageNumber, pageSize));
        //}



        public ActionResult HistoriaSuajes(FormCollection collection, string Clave, string Status, string Fechainicio, string Fechafinal, int? page, int? PageSize, int IDFamilia = 0)
        {
            int pageNumber = 0;
            int pageSize = 0;
            int count = 0;
            if (Clave == null)
            {
                Clave = "";
            }

            ViewBag.IDFamilia = new FamAlmRepository().GetAlmacenesF(IDFamilia);

            var StaLst = new List<string>();
            var StaQry = from d in db.EncOrdenCompras
                         orderby d.IDOrdenCompra
                         select d.Status;
            StaLst.Add(" ");
            StaLst.AddRange(StaQry.Distinct());
            ViewBag.Status = new SelectList(StaLst);
            ViewBag.StatusSeleccionado = Status;

            List<VHistoriaSuaje> almacen = new List<VHistoriaSuaje>();
            string cadena1 = "";
            string orden = "order by o.IDOrdenCompra desc";
            if (Fechainicio == "")
            {
                Fechainicio = string.Empty;
            }
            if (Fechafinal == "")
            {
                Fechafinal = string.Empty;
            }
            if (Status == "")
            {
                Status = string.Empty;
            }
            if (Fechafinal == string.Empty)
            {
                Fechafinal = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            }
            if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
            {
                cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where o.Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "' and d.importe > 100 and o.status != 'Cancelado'";


            }
            if (IDFamilia != 0)
            {
                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where a.IDFamilia = " + IDFamilia + " and o.Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "' and d.importe > 100 and o.status != 'Cancelado'";
                }
                else
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where a.IDFamilia = " + IDFamilia + " and d.importe > 100 and o.status != 'Cancelado'";

                }

            }
            else
            {
                //cadena sin filtros
                cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda from((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where d.importe > 100 and o.status != 'Cancelado'";

            }
            if (Clave != "")
            {
                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where(a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%') and o.Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "' and d.importe > 100 and o.status != 'Cancelado'";
                }
                else
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where(a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%')";

                }

            }
            if (Clave != "" && IDFamilia != 0)
            {
                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where (a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%') and a.idfamilia=" + IDFamilia + " and o.Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "' and d.importe > 100 and o.status != 'Cancelado'";



                }
                else
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where (a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%') and a.idfamilia=" + IDFamilia + " and d.importe > 100 and o.status != 'Cancelado'";

                }

            }



            if (!String.IsNullOrEmpty(Status))
            {
                if (IDFamilia != 0)
                {
                    cadena1 = cadena1 + " and o.status='" + Status + "'";
                }
                else if (Clave != "")
                {
                    cadena1 = cadena1 + " and o.status='" + Status + "'";
                }
                else if (Clave != "" && IDFamilia != 0)
                {
                    cadena1 = cadena1 + " and o.status='" + Status + "'";
                }
                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    cadena1 = cadena1 + " and o.status='" + Status + "'";
                }
                else
                {
                    cadena1 = cadena1 + "  and o.status='" + Status + "'";
                }
            }

            cadena1 = cadena1 + orden;
            almacen = db.Database.SqlQuery<VHistoriaSuaje>(cadena1).OrderByDescending(s => s.Orden_Compra).ToList();

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            count = almacen.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25", Selected = true },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };
            ViewBag.CurrentFilter = Clave;
            ViewBag.FamiliaSeleccionada = IDFamilia;
            pageNumber = (page ?? 1);
            pageSize = (PageSize ?? 25);
            ViewBag.psize = pageSize;

            return View(almacen.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ExcelHistoriaSuajes(string Clave, string Status, string Fechainicio, string Fechafinal, int IDFamilia = 0)
        {

            var StaLst = new List<string>();
            var StaQry = from d in db.EncOrdenCompras
                         orderby d.IDOrdenCompra
                         select d.Status;
            StaLst.Add(" ");
            StaLst.AddRange(StaQry.Distinct());
            ViewBag.Status = new SelectList(StaLst);
            ViewBag.StatusSeleccionado = Status;

            if (Status == "  ")
            {
                Status = "";
            }
            if (Fechainicio == " ")
            {
                Fechainicio = "";
            }

            List<VHistoriaSuaje> almacen = new List<VHistoriaSuaje>();
            string cadena1 = "";
            string orden = "order by o.IDOrdenCompra desc";
            if (Fechainicio == "")
            {
                Fechainicio = string.Empty;
            }
            if (Fechafinal == "")
            {
                Fechafinal = string.Empty;
            }
            if (Status == "")
            {
                Status = string.Empty;
            }
            if (Fechafinal == string.Empty)
            {
                Fechafinal = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            }
            if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
            {
                cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where o.Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "' and d.importe > 100 and o.status != 'Cancelado'";


            }
            if (IDFamilia != 0)
            {
                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where a.IDFamilia = " + IDFamilia + " and o.Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "' and d.importe > 100 and o.status != 'Cancelado'";
                }
                else
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where a.IDFamilia = " + IDFamilia + " and d.importe > 100 and o.status != 'Cancelado'";

                }

            }
            else
            {
                //cadena sin filtros
                cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda from((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where d.importe > 100 and o.status != 'Cancelado'";

            }
            if (Clave != "")
            {
                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where(a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%') and o.Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "' and d.importe > 100 and o.status != 'Cancelado'";
                }
                else
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where(a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%')";

                }

            }
            if (Clave != "" && IDFamilia != 0)
            {
                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where (a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%') and a.idfamilia=" + IDFamilia + " and o.Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "' and d.importe > 100 and o.status != 'Cancelado'";



                }
                else
                {
                    cadena1 = "select top 500 O.IDOrdenCompra AS Orden_compra, O.Fecha,O.FechaRequiere, a.Familia, o.status,  a.cref, a.Descripcion, c.presentacion, c.idpresentacion,d.cantidad, cu.Nombre, ( d.importe) as monto ,M.ClaveMoneda, d.Nota from ((encordencompra as o inner join detordencompra as d on o.idordencompra = d.idordencompra inner join Varticulo as a on a.idarticulo = d.idarticulo inner join caracteristica as c on c.id = d.caracteristica_id) inner join c_ClaveUnidad cu on a.IDClaveUnidad = cu.IDClaveUnidad ) inner join c_Moneda M on o.IdMoneda = M.IdMoneda where (a.cref like '%" + Clave + "%' or d.Nota like '%" + Clave + "%') and a.idfamilia=" + IDFamilia + " and d.importe > 100 and o.status != 'Cancelado'";

                }

            }


            if (!String.IsNullOrEmpty(Status))
            {
                if (IDFamilia != 0)
                {
                    cadena1 = cadena1 + " and o.status='" + Status + "'";
                }
                else if (Clave != "")
                {
                    cadena1 = cadena1 + " and o.status='" + Status + "'";
                }
                else if (Clave != "" && IDFamilia != 0)
                {
                    cadena1 = cadena1 + " and o.status='" + Status + "'";
                }
                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    cadena1 = cadena1 + " and o.status='" + Status + "'";
                }
                else
                {
                    cadena1 = cadena1 + "  and o.status='" + Status + "'";
                }
            }

            cadena1 = cadena1 + orden;
            almacen = db.Database.SqlQuery<VHistoriaSuaje>(cadena1).OrderByDescending(s => s.Orden_Compra).ToList();

            ExcelPackage Ep = new ExcelPackage();


            var Sheet = Ep.Workbook.Worksheets.Add("HistoriaOP");
            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para EL RANGO DE CELDAS A1:B1
            Sheet.Cells["A1:K1"].Style.Font.Size = 20;
            Sheet.Cells["A1:K1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:K1"].Style.Font.Bold = true;
            Sheet.Cells["A1:K1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1:K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A1"].RichText.Add("Historia de Ordenes de Compra");

            row = 2;
            //Fijar la fuente para EL RANGO DE CELDAS A2:B2
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 12;
            Sheet.Cells["A2:K2"].Style.Font.Bold = true;
            Sheet.Cells["A2:K2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:K2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            //Sheet.Cells["A1"].Value = "Clave";
            //Sheet.Cells["B1"].Value = "Descripción";
            Sheet.Cells["A2"].RichText.Add("Orden Compra");
            Sheet.Cells["B2"].RichText.Add("Fecha");
            Sheet.Cells["C2"].RichText.Add("Fecha Requerida");
            Sheet.Cells["D2"].RichText.Add("Clave Articulo");
            Sheet.Cells["E2"].RichText.Add("Presentación");
            Sheet.Cells["F2"].RichText.Add("Cantidad");
            Sheet.Cells["G2"].RichText.Add("Unidad");
            Sheet.Cells["H2"].RichText.Add("Nota");
            Sheet.Cells["I2"].RichText.Add("Subtotal");
            Sheet.Cells["J2"].RichText.Add("Moneda");
            Sheet.Cells["K2"].RichText.Add("Estado");
            Sheet.Cells["A2:K2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            row = 3;
            foreach (var item in almacen)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.Orden_Compra;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.FechaRequiere;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Cref;
                Sheet.Cells[string.Format("E{0}", row)].Value = "NP: " + item.IDPresentacion + " " + item.Presentacion;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Cantidad;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Nombre;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Nota;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.Monto;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.ClaveMoneda;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.Status;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteHistoriaOP.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();

            return View();
        }
    }
    public class FamAlmRepository
    {
        public IEnumerable<SelectListItem> GetAlmacenesF(int IDAlmacen)
        {

            List<SelectListItem> lista;
            using (var context = new FamiliaContext())
            {
                string cadenasql = "select*from Familia ";
                lista = context.Database.SqlQuery<Familia>(cadenasql).ToList().OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDFamilia.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una familia ---"
                };
                lista.Insert(0, descripciontip);
                return new SelectList(lista, "Value", "Text");
            }
        }

    }
    public class datosprov
    {
           public     int IDFormapago { get; set; }
        public int IDMoneda { get; set; }
        public int IDCondicionesPago { get; set; }

        public int IDMetodoPago { get; set; }
    }

    
}