using PagedList;
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

using System.IO;
using SIAAPI.Reportes;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Globalization;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,GerenteVentas, Ventas,Compras")]
    public class EncCotizacionController : Controller
    {
        private CotizacionContext db = new CotizacionContext();
        private ClientesContext prov = new ClientesContext();
       

        public ActionResult Index(string Cliente, string Vendedor, string Oficina, string Divisa, string Status, string sortOrder, string currentFilter, string searchString, string Fechainicio, string Fechafinal, int? page, int? PageSize)
        {

            VCotizacionesContext dbc = new VCotizacionesContext();
            VendedorContext dbv = new VendedorContext();

          
            string ConsultaSql = "select * from dbo.VCotizaciones";
            string FiltroSql = string.Empty;
            string OrdenSql = "order by IDCotizacion desc";
            string SumaSql = "select ClaveMoneda as MonedaOrigen, Sum(Subtotal) as Subtotal, Sum(IVA) as IVA, Sum(Total) as Total, sum(Total * TipoCambio) as TotalenPesos from dbo.VCotizaciones";
            string FiltroSuma = "and Status != 'Cancelado'";
            string GroupSql = "group by ClaveMoneda";
            string CadenaSql = string.Empty;
            string CadenaResumenSql = string.Empty;

            try
            {

                ///filtro: Divisa
                var SerLst = new List<string>();
                var SerQry = from d in db.c_Monedas
                             orderby d.IDMoneda
                             select d.ClaveMoneda;
                SerLst.Add(" ");
                SerLst.AddRange(SerQry.Distinct());
                ViewBag.Divisa = new SelectList(SerLst);
                ViewBag.DivisaSeleccionada = Divisa;

                ///filtro: Vendedor
                var VenLst = new List<string>();
                var VenQry = from d in dbv.Vendedores
                             orderby d.IDVendedor
                             select d.Nombre;
                VenLst.Add(" ");
                VenLst.AddRange(VenQry.Distinct());
                ViewBag.Vendedor = new SelectList(VenLst);
                ViewBag.VendedorSeleccionado = Vendedor;

                OficinaContext dbof = new OficinaContext();

                ///filtro: Oficina
                var OfLst = new List<string>();
                var OfQry = from d in dbof.Oficinas
                            orderby d.NombreOficina
                            select d.NombreOficina;
                OfLst.Add(" ");
                OfLst.AddRange(OfQry.Distinct());
                ViewBag.Oficina = new SelectList(OfLst);
                ViewBag.OficinaSeleccionada = Oficina;

                ///filtro: Status
                var StaLst = new List<string>();
                var StaQry = from d in dbc.VCotizaciones
                             orderby d.IDCotizacion
                             select d.Status;
                StaLst.Add(" ");
                StaLst.AddRange(StaQry.Distinct());
                ViewBag.Status = new SelectList(StaLst);
                ViewBag.StatusSeleccionado = Status;

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
                        FiltroSql = "where IDCotizacion = " + int.Parse(searchString.ToString()) + "";
                    }
                    else
                    {
                        FiltroSql += " and  IDCotizacion = " + int.Parse(searchString.ToString()) + "";
                    }

                }


                ///tabla filtro: Cliente
                if (Cliente == "")
                {
                    Cliente = string.Empty;
                }

                if (!String.IsNullOrEmpty(Cliente))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where Cliente like '" + Cliente + "%'";
                    }
                    else
                    {
                        FiltroSql += " and  Cliente like '" + Cliente + "%'";
                    }

                }
                ///tabla filtro: Oficina
                if (Oficina == "")
                {
                    Oficina = string.Empty;
                }

                if (!String.IsNullOrEmpty(Oficina))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where NombreOficina = '" + Oficina + "'";
                    }
                    else
                    {
                        FiltroSql += " and NombreOficina = '" + Oficina + "'";
                    }

                }

                ///tabla filtro: Vendedor
                if (Vendedor == "")
                {
                    Vendedor = string.Empty;
                }

                if (!String.IsNullOrEmpty(Vendedor))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where Nombre = '" + Vendedor + "'";
                    }
                    else
                    {
                        FiltroSql += " and Nombre = '" + Vendedor + "'";
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
                ViewBag.EmpresaSortParm = String.IsNullOrEmpty(sortOrder) ? "Cliente" : "";
                // Pass filtering string to view in order to maintain filtering when paging
                ViewBag.Fechainicio = Fechainicio;
                ViewBag.Fechafinal = Fechafinal;

                switch (sortOrder)
                {
                    case "Cotizacion":
                        OrdenSql = " order by  IDCotizacion asc ";
                        break;

                    case "Cliente":
                        OrdenSql = " order by  Cliente, IDPedido desc ";
                        break;

                    default:
                        OrdenSql = "order by IDCotizacion desc ";
                        break;
                }



               

                CadenaSql = ConsultaSql + " " + FiltroSql + " " + OrdenSql;

                var elementos = dbv.Database.SqlQuery<VCotizaciones>(CadenaSql).ToList();


                ViewBag.sumatoria = "";
                try
                {

                    var SumaLst = new List<string>();


                    FiltroSuma = FiltroSql + " " + FiltroSuma;
                    var SumaQry = SumaSql + " " + FiltroSuma + " " + GroupSql;
                    List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
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
        public ActionResult Cambio(int? id, string DireccionEntrega, List<VDetCotizacion> cr, string OCompra)
        {

            EncCotizacion encCotizacionaux = db.EncCotizaciones.Find(id);
            List<SelectListItem> entrega = new List<SelectListItem>();
            List<VEntrega> entregasall = db.Database.SqlQuery<VEntrega>("select en.IDEntrega,en.IDCliente,en.CalleEntrega,en.NumExtEntrega,ISNULL(en.NumIntEntrega,0),en.ColoniaEntrega,en.MunicipioEntrega,en.CPEntrega,en.ObservacionEntrega,es.Estado from dbo.Entrega as en inner join Estados as es on es.IDEstado=en.IDEstado where IDCliente='" + encCotizacionaux.IDCliente + "'").ToList();
            ViewBag.entregaa = entregasall;
            entrega.Add(new SelectListItem { Text = "El Cliente Recoge", Value = "El Cliente Recoge" });
            for (int i = 0; i < entregasall.Count(); i++)
            {
                entrega.Add(new SelectListItem { Text = "Calle: " + ViewBag.entregaa[i].CalleEntrega + " " + "No. Exterior: " + ViewBag.entregaa[i].NumExtEntrega + " " + "No. Interior: " + ViewBag.entregaa[i].NumIntentrega + " " + "Colonia: " + ViewBag.entregaa[i].ColoniaEntrega + " " + "Municipio: " + ViewBag.entregaa[i].MunicipioEntrega + " " + "C.P.: " + ViewBag.entregaa[i].CPEntrega + " " + "Estado: " + ViewBag.entregaa[i].Estado + " ", Value = "Calle: " + ViewBag.entregaa[i].CalleEntrega + " " + "No. Exterior: " + ViewBag.entregaa[i].NumExtEntrega + " " + "No. Interior: " + ViewBag.entregaa[i].NumIntentrega + " " + "Colonia: " + ViewBag.entregaa[i].ColoniaEntrega + " " + "Municipio: " + ViewBag.entregaa[i].MunicipioEntrega + " " + "C.P.: " + ViewBag.entregaa[i].CPEntrega + " " + "Estado: " + ViewBag.entregaa[i].Estado + " " });
                entrega.Add(new SelectListItem { Text = "Transportista Calle: " + ViewBag.entregaa[i].CalleEntrega + " " + "No. Exterior: " + ViewBag.entregaa[i].NumExtEntrega + " " + "No. Interior: " + ViewBag.entregaa[i].NumIntentrega + " " + "Colonia: " + ViewBag.entregaa[i].ColoniaEntrega + "Municipio: " + ViewBag.entregaa[i].MunicipioEntrega + " " + " " + "C.P.: " + ViewBag.entregaa[i].CPEntrega + " " + "Estado: " + ViewBag.entregaa[i].Estado + " ", Value = "Transportista Calle: " + ViewBag.entregaa[i].CalleEntrega + " " + "No. Exterior: " + ViewBag.entregaa[i].NumExtEntrega + " " + "No. Interior: " + ViewBag.entregaa[i].NumIntentrega + " " + "Colonia: " + ViewBag.entregaa[i].ColoniaEntrega + " " + "Municipio: " + ViewBag.entregaa[i].MunicipioEntrega + " " + "C.P.: " + ViewBag.entregaa[i].CPEntrega + " " + "Estado: " + ViewBag.entregaa[i].Estado + " " });
                entrega.Add(new SelectListItem { Text = "Mensajeria Calle: " + ViewBag.entregaa[i].CalleEntrega + " " + "No. Exterior: " + ViewBag.entregaa[i].NumExtEntrega + " " + "No. Interior: " + ViewBag.entregaa[i].NumIntentrega + " " + "Colonia: " + ViewBag.entregaa[i].ColoniaEntrega + " " + "Municipio: " + ViewBag.entregaa[i].MunicipioEntrega + " " + "C.P.: " + ViewBag.entregaa[i].CPEntrega + " " + "Estado: " + ViewBag.entregaa[i].Estado + " ", Value = "Mensajeria Calle: " + ViewBag.entregaa[i].CalleEntrega + " " + "No. Exterior: " + ViewBag.entregaa[i].NumExtEntrega + " " + "No. Interior: " + ViewBag.entregaa[i].NumIntentrega + " " + "Colonia: " + ViewBag.entregaa[i].ColoniaEntrega + " " + "Municipio: " + ViewBag.entregaa[i].MunicipioEntrega + " " + "C.P.: " + ViewBag.entregaa[i].CPEntrega + " " + "Estado: " + ViewBag.entregaa[i].Estado + " " });
                entrega.Add(new SelectListItem { Text = "Ocurre Colonia: " + ViewBag.entregaa[i].ColoniaEntrega + " " + "Municipio: " + ViewBag.entregaa[i].MunicipioEntrega + " " + "C.P.: " + ViewBag.entregaa[i].CPEntrega + " " + "Estado: " + ViewBag.entregaa[i].Estado + " ", Value = "Ocurre Colonia: " + ViewBag.entregaa[i].ColoniaEntrega + " " + "Municipio: " + ViewBag.entregaa[i].MunicipioEntrega + " " + "C.P.: " + ViewBag.entregaa[i].CPEntrega + " " + "Estado: " + ViewBag.entregaa[i].Estado + " " });
            }
            ViewBag.entrega = entrega;
            if (DireccionEntrega != null)
            {
                decimal subtotal = 0, iva = 0, total = 0, precio = 0, importe = 0, importetotal = 0, importeiva = 0, Cambio = 0;
                EncCotizacion encCotizacion = db.EncCotizaciones.Find(id);
                string fecha = DateTime.Now.ToString("yyyy/MM/dd");
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int UserID = userid.Select(s => s.UserID).FirstOrDefault();
                List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
                VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + origen + "," + encCotizacion.IDMoneda + ") as TC").ToList()[0];
                Cambio = cambio.TC;


                db.Database.ExecuteSqlCommand("INSERT INTO EncPedido([Fecha],[FechaRequiere],[IDCliente],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[UserID],[TipoCambio],[IDUsoCFDI],[IDVendedor]) SELECT [Fecha],[FechaRequiere],[IDCliente],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[UserID],[TipoCambio],[IDUsoCFDI],[IDVendedor] FROM EncCotizacion where IDCotizacion='" + id + "'");
                List<EncPedido> numero = db.Database.SqlQuery<EncPedido>("SELECT * FROM [dbo].[EncPedido] WHERE IDPedido = (SELECT MAX(IDPedido) from EncPedido)").ToList();
                int num = numero.Select(s => s.IDPedido).FirstOrDefault();
                foreach (var i in cr)
                {
                    //List<DetCotizacion> orden = db.Database.SqlQuery<DetCotizacion>("select * from dbo.DetCotizacion where IDCotizacion='" + id + "' and Status='Activo'").ToList();
                    //ViewBag.ordenc = orden;
                    //for (int i = 0; i < orden.Count(); i++)
                    //{

                    DetCotizacion detcotizacion = db.DetCotizaciones.Find(i.IDDetCotizacion);
                    Articulo articulo = new ArticuloContext().Articulo.Find(detcotizacion.IDArticulo);
                    precio = detcotizacion.Costo * Cambio;
                    importe = precio * i.Cantidad;
                    importeiva = importe * (decimal)0.16;
                    importetotal = importeiva + importe;
                    db.Database.ExecuteSqlCommand("INSERT INTO DetPedido([IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna],[GeneraOrdenP],[IDRemision],[IDPrefactura]) values ('" + num + "','" + detcotizacion.IDArticulo + "','" + i.Cantidad + "','" + precio + "','" + i.Cantidad + "','0','" + importe + "','true','" + importeiva + "','" + importetotal + "','" + detcotizacion.Nota + "','0','" + detcotizacion.Caracteristica_ID + "','" + detcotizacion.IDAlmacen + "','0','Inactivo','" + detcotizacion.Presentacion + "','" + detcotizacion.jsonPresentacion + "','" + i.IDDetCotizacion + "','" + articulo.GeneraOrden + "','0','0')");
                    List<DetPedido> numero2 = db.Database.SqlQuery<DetPedido>("SELECT * FROM [dbo].[DetPedido] WHERE IDDetPedido = (SELECT MAX(IDDetPedido) from DetPedido)").ToList();
                    int num2 = numero2.Select(s => s.IDDetPedido).FirstOrDefault();
                    db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Pedido'," + num + "," + i.Cantidad + ",'" + fecha + "'," + id + ",'Cotización'," + UserID + "," + i.IDDetCotizacion + "," + num2 + ")");
                }


                List<DetPedido> datostotales = db.Database.SqlQuery<DetPedido>("select * from dbo.DetPedido where IDPedido='" + num + "'").ToList();
                subtotal = datostotales.Sum(s => s.Importe);
                iva = subtotal * (decimal)0.16;
                total = subtotal + iva;
                db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [OCompra]= '" + OCompra + "',[TipoCambio]='" + Cambio + "',[Entrega]='" + DireccionEntrega + "',[Status]='Inactivo', Fecha='" + fecha + "', [Subtotal]='" + subtotal + "',[IVA]='" + iva + "',[Total]='" + total + "' where [IDPedido]='" + num + "'");

                db.Database.ExecuteSqlCommand("update [dbo].[EncCotizacion] set [Status]='Finalizado' where [IDCotizacion]='" + id + "'");
                db.Database.ExecuteSqlCommand("update [dbo].[DetCotizacion] set [Status]='Finalizado' where [IDCotizacion]='" + id + "'");

                return RedirectToAction("Index", "EncPedido");
            }
            List<VDetCotizacion> cotizacion = db.Database.SqlQuery<VDetCotizacion>("select DetCotizacion.IDDetCotizacion,DetCotizacion.Status,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetCotizacion.IDCotizacion,Articulo.Descripcion as Articulo,DetCotizacion.Cantidad,DetCotizacion.Costo,DetCotizacion.CantidadPedida,DetCotizacion.Descuento,DetCotizacion.Importe,DetCotizacion.IVA,DetCotizacion.ImporteIva,DetCotizacion.ImporteTotal, DetCotizacion.Nota,DetCotizacion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetCotizacion INNER JOIN Caracteristica ON DetCotizacion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where detcotizacion.IDCotizacion='" + id + "'").ToList();

            var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncCotizacion.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncCotizacion inner join Clientes on Clientes.IDCliente=EncCotizacion.IDCliente  where EncCotizacion.IDCotizacion='" + id + "' group by EncCotizacion.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;
            return View(cotizacion);
        }

        public ActionResult Details(int? id)
        {
            List<VDetCotizacion> cotizacion = db.Database.SqlQuery<VDetCotizacion>("select DetCotizacion.IDDetCotizacion, DetCotizacion.IDAlmacen,DetCotizacion.Status,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetCotizacion.IDCotizacion,Articulo.Descripcion as Articulo,DetCotizacion.Cantidad,DetCotizacion.Costo,DetCotizacion.CantidadPedida,DetCotizacion.Descuento,DetCotizacion.Importe,DetCotizacion.IVA,DetCotizacion.ImporteIva,DetCotizacion.ImporteTotal, DetCotizacion.Nota,DetCotizacion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetCotizacion INNER JOIN Caracteristica ON DetCotizacion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where detcotizacion.IDCotizacion='" + id + "'").ToList();

            ViewBag.req = cotizacion;

            var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncCotizacion.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncCotizacion inner join Clientes on Clientes.IDCliente=EncCotizacion.IDCliente  where EncCotizacion.IDCotizacion='" + id + "' group by EncCotizacion.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;

            EncCotizacion encCotizacion = db.EncCotizaciones.Find(id);

            return View(encCotizacion);
        }

        public ActionResult Cancelar(int? id)
        {
            string fecha = DateTime.Now.ToString("yyyyMMdd");


            EncCotizacion encCotizacion = db.EncCotizaciones.Find(id);
            List<VDetCotizacion> cotizacion = db.Database.SqlQuery<VDetCotizacion>("select DetCotizacion.Status,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetCotizacion.IDCotizacion,Articulo.Descripcion as Articulo,DetCotizacion.Cantidad,DetCotizacion.Costo,DetCotizacion.CantidadPedida,DetCotizacion.Descuento,DetCotizacion.Importe,DetCotizacion.IVA,DetCotizacion.ImporteIva,DetCotizacion.ImporteTotal, DetCotizacion.Nota,DetCotizacion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetCotizacion INNER JOIN Caracteristica ON DetCotizacion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDCotizacion='" + id + "' and Status='Activo'").ToList();

            ViewBag.req = cotizacion;

            db.Database.ExecuteSqlCommand("update EncCotizacion set [Status]='Cancelado' where IDCotizacion='" + id + "' and Status='Activo'");
            db.Database.ExecuteSqlCommand("update DetCotizacion set [Status]='Cancelado'  where IDCotizacion='" + id + "' and Status='Activo'");
            return RedirectToAction("Index");
        }

        [HttpPost]

        public ActionResult MonedaC(int? idmoneda)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            List<VCarrito> cotizacion = db.Database.SqlQuery<VCarrito>("select (Carrito.Precio * (select dbo.GetTipocambio(GETDATE(),Carrito.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * Carrito.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            ViewBag.carrito = cotizacion;


            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            for (int i = 0; i < cotizacion.Count(); i++)
            {
                int monedaorigen = ViewBag.carrito[i].IDMoneda;
                VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + monedaorigen + "," + idmoneda + ") as TC").ToList()[0];
                decimal Precio = ViewBag.carrito[i].Precio * cambio.TC;

                db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set  [IDMoneda]=" + idmoneda + ", [Precio]=" + Precio + "  where IDCarrito =" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");

            }
            return Json(true);

        }
        public ActionResult Create()
        {
            ViewBag.idalma = 2;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            CarritoContext car = new CarritoContext();
            ClsDatoEntero cambio = db.Database.SqlQuery<ClsDatoEntero>("select distinct IDCliente as Dato from Carrito where usuario=" + usuario + "").ToList()[0];

            List<SelectListItem> li = new List<SelectListItem>();
            Clientes mm = prov.Clientes.Find(cambio.Dato);
            li.Add(new SelectListItem { Text = mm.Nombre, Value = mm.IDCliente.ToString() });
            ViewBag.cliente = li;

            Clientes clientes = prov.Clientes.Find(cambio.Dato);
            List<SelectListItem> moneda = new List<SelectListItem>();

            ClsDatoEntero monedacarrito = db.Database.SqlQuery<ClsDatoEntero>("select distinct IDMoneda as Dato from Carrito where usuario=" + usuario + "").ToList()[0];
            c_Moneda monedap = prov.c_Monedas.Find(monedacarrito.Dato);
            moneda.Add(new SelectListItem { Text = monedap.Descripcion, Value = monedap.IDMoneda.ToString() });
            moneda.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosmoneda = prov.c_Monedas.ToList();
            if (todosmoneda != null)
            {
                foreach (var y in todosmoneda)
                {
                    moneda.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDMoneda.ToString() });
                }
            }

            ViewBag.moneda = moneda;

            List<SelectListItem> metodo = new List<SelectListItem>();
            c_MetodoPago metodop = prov.c_MetodoPagos.Find(clientes.IDMetodoPago);
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
            c_FormaPago formap = prov.c_FormaPagos.Find(clientes.IDFormapago);
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
            CondicionesPago condicionesp = prov.CondicionesPagos.Find(clientes.IDCondicionesPago);
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

            List<SelectListItem> vendedor = new List<SelectListItem>();
            Vendedor vendedorp = prov.Vendedores.Find(clientes.IDVendedor);
            vendedor.Add(new SelectListItem { Text = vendedorp.Nombre, Value = vendedorp.IDVendedor.ToString() });
            vendedor.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosvendedor = prov.Vendedores.ToList();
            if (todosvendedor != null)
            {
                foreach (var y in todosvendedor)
                {
                    vendedor.Add(new SelectListItem { Text = y.Nombre, Value = y.IDVendedor.ToString() });
                }
            }

            ViewBag.vendedor = vendedor;

            List<SelectListItem> usocfdi = new List<SelectListItem>();
            c_UsoCFDI usocfdib = prov.c_UsoCFDIS.Find(clientes.IDUsoCFDI);
            usocfdi.Add(new SelectListItem { Text = usocfdib.Descripcion, Value = usocfdib.IDUsoCFDI.ToString() });
            usocfdi.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosuso = prov.c_UsoCFDIS.ToList();
            if (todosuso != null)
            {
                foreach (var y in todosuso)
                {
                    usocfdi.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDUsoCFDI.ToString() });
                }
            }

            ViewBag.usocfdi = usocfdi;


            // ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS, "IDUsoCFDI", "Descripcion");
            ViewBag.IDAlmacen = new SelectList(db.Almacenes, "IDAlmacen", "Descripcion");

            List<VCarrito> cotizacion = db.Database.SqlQuery<VCarrito>("select (Carrito.Precio * (select dbo.GetTipocambio(GETDATE(),Carrito.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * Carrito.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad, Carrito.idalmacen,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=Carrito.IDMoneda) as MonedaOrigen, (select SUM(Carrito.Precio * Carrito.Cantidad)) as Subtotal, SUM(Carrito.Precio * Carrito.Cantidad)*0.16 as IVA, (SUM(Carrito.Precio * Carrito.Cantidad)) + (SUM(Carrito.Precio * Carrito.Cantidad)*0.16) as Total ,0 as TotalPesos from Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario=" + usuario + " group by Carrito.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;
            ViewBag.carrito = cotizacion;


            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarrito) as Dato from Carrito where  usuario='" + usuario + "'").ToList()[0];
            int x = c.Dato;
            ViewBag.dato = x;

            ClsDatoEntero cantidad = db.Database.SqlQuery<ClsDatoEntero>("select count(Cantidad) as Dato from Carrito where Cantidad=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datocantidad = cantidad.Dato;

            ClsDatoEntero preciocontar = db.Database.SqlQuery<ClsDatoEntero>("select count(Precio) as Dato from Carrito where Precio=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datoprecio = preciocontar.Dato;


            List<ValidarCarrito> validaprecio = db.Database.SqlQuery<ValidarCarrito>("select Carrito.Precio, dbo.GetValidaCosto(Articulo.IDArticulo, Carrito.Cantidad, Carrito.Precio) as Dato from Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo  where Carrito.usuario='" + usuario + "'").ToList();
            int countDato = validaprecio.Count(s => s.Dato.Equals(true));

            int countCarrito = cotizacion.Count();

            if (countDato == countCarrito)
            {
                ViewBag.validacarrito = 1;
            }
            else
            {
                ViewBag.validacarrito = 0;
            }

            //Termina la consulta del carrito
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EncCotizacion encCotizacion)
        {
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0, Cambio = 0, Precio = 0, subtotalaux = 0;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<VCarrito> cotizacion = db.Database.SqlQuery<VCarrito>("select (Carrito.Precio * (select dbo.GetTipocambio(GETDATE(),Carrito.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * Carrito.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad, Carrito.idalmacen, Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=Carrito.IDMoneda) as MonedaOrigen, (select SUM(Carrito.Precio * Carrito.Cantidad)) as Subtotal, SUM(Carrito.Precio * Carrito.Cantidad)*0.16 as IVA, (SUM(Carrito.Precio * Carrito.Cantidad)) + (SUM(Carrito.Precio * Carrito.Cantidad)*0.16) as Total ,0 as TotalPesos from Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario=" + usuario + " group by Carrito.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;

            ViewBag.carrito = cotizacion;
            //Termina 

            if (ModelState.IsValid)
            {
                int num = 0;
                DateTime fecha = encCotizacion.Fecha;
                string fecha1 = fecha.ToString("yyyy/MM/dd");

                DateTime fechareq = encCotizacion.FechaRequiere;
                string fecha2 = fechareq.ToString("yyyy/MM/dd");

                List<c_Moneda> monedaorigen;
                monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();



                VCambio tcambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + encCotizacion.IDMoneda + "," + origen + ") as TC").ToList()[0];
                decimal tCambio = tcambio.TC;
                encCotizacion.IDAlmacen = 2;

                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncCotizacion]([Fecha],[FechaRequiere],[IDCliente],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[TipoCambio],[UserID],[IDUsoCFDI],[IDVendedor]) values ('" + fecha1 + "','" + fecha2 + "','" + encCotizacion.IDCliente + "','" + encCotizacion.IDFormapago + "','" + encCotizacion.IDMoneda + "','" + encCotizacion.Observacion + "','" + subtotal + "','" + iva + "','" + total + "','" + encCotizacion.IDMetodoPago + "','" + encCotizacion.IDCondicionesPago + "','" + encCotizacion.IDAlmacen + "','Activo','" + tCambio + "','" + usuario + "','" + encCotizacion.IDUsoCFDI + "','" + encCotizacion.IDVendedor + "')");
                db.SaveChanges();

                List<EncCotizacion> numero;
                numero = db.Database.SqlQuery<EncCotizacion>("SELECT * FROM [dbo].[EncCotizacion] WHERE IDCotizacion = (SELECT MAX(IDCotizacion) from EncCotizacion)").ToList();
                num = numero.Select(s => s.IDCotizacion).FirstOrDefault();



                for (int i = 0; i < cotizacion.Count(); i++)
                {
                    //VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encCotizacion.IDMoneda + ") as TC").ToList()[0];
                    //Cambio = cambio.TC;
                    Precio = ViewBag.carrito[i].Precio;
                    importe = ViewBag.carrito[i].Cantidad * Precio;
                    importeiva = importe * (decimal)0.16;
                    importetotal = importeiva + importe;
                    db.Database.ExecuteSqlCommand("INSERT INTO DetCotizacion([IDCotizacion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion]) values ('" + num + "','" + ViewBag.carrito[i].IDArticulo + "','" + ViewBag.carrito[i].Cantidad + "','" + Precio + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encCotizacion.IDMoneda + "),'" + ViewBag.carrito[i].Cantidad + "','0','" + importe + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encCotizacion.IDMoneda + "),'true','" + importeiva + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encCotizacion.IDMoneda + "),'" + importetotal + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encCotizacion.IDMoneda + "),'" + ViewBag.carrito[i].Nota + "','0','" + ViewBag.carrito[i].IDCaracteristica + "','" + ViewBag.carrito[i].IDAlmacen + "','0','Activo','" + ViewBag.carrito[i].Presentacion + "','" + ViewBag.carrito[i].jsonPresentacion + "')");
                    //   db.Database.ExecuteSqlCommand("exec dbo.MovArt'" + fecha1 + "'," + ViewBag.carrito[i].IDCaracteristica + ",'CotVta'," + ViewBag.carrito[i].Cantidad + ",'Cotizacion'," + num + ",0,'" + encCotizacion.IDAlmacen + "','" + ViewBag.carrito[i].Nota + "',0");
                    db.Database.ExecuteSqlCommand("delete from Carrito where IDCarrito='" + ViewBag.carrito[i].IDCarrito + "'");
                    db.SaveChanges();


                }
                List<DetCotizacion> datostotales = db.Database.SqlQuery<DetCotizacion>("select * from dbo.DetCotizacion where IDCotizacion='" + num + "'").ToList();
                subtotalr = datostotales.Sum(s => s.Importe);
                ivar = subtotalr * (decimal)0.16;
                totalr = subtotalr + ivar;
                db.Database.ExecuteSqlCommand("update [dbo].[EncCotizacion] set [Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "' where [IDCotizacion]='" + num + "'");

                return RedirectToAction("Index");

            }


            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", encCotizacion.IDFormapago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", encCotizacion.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", encCotizacion.IDMoneda);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", encCotizacion.IDCondicionesPago);
            ViewBag.IDProveedor = new SelectList(db.Clientess, "IDProveedor", "Empresa", encCotizacion.IDCliente);
            ViewBag.IDAlmacen = new SelectList(db.Almacenes, "IDAlmacen", "Descripcion", encCotizacion.IDAlmacen);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS, "IDUsoCFDI", "Descripcion", encCotizacion.IDUsoCFDI);

            return View(encCotizacion);

        }
        public ActionResult getPrecio(int id)
        {

            TipoCambioContext db = new TipoCambioContext();
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();

            decimal Cambio = 0;
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + id + "," + origen + ") as TC").ToList()[0];
            Cambio = cambio.TC;
            ViewBag.datostc = Cambio;
            return Json(Cambio, JsonRequestBehavior.AllowGet);
        }
      

         public ActionResult PdfCotizacion(int id)
        {

            EncCotizacion cot = new CotizacionContext().EncCotizaciones.Find(id);
            DocumentoCO x = new DocumentoCO();

            x.claveMoneda = cot.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = cot.Fecha.ToShortDateString();
            x.fechaRequerida = cot.Fecha.ToShortDateString();
            x.Cliente = cot.Clientes.Nombre;
            x.formaPago = cot.c_FormaPago.ClaveFormaPago;
            x.metodoPago = cot.c_MetodoPago.ClaveMetodoPago;
            x.RFCCliente = cot.Clientes.RFC;
            x.TelefonoCliente = cot.Clientes.Telefono;
            x.total = float.Parse(cot.Total.ToString());
            x.subtotal = float.Parse(cot.Subtotal.ToString());
            x.tipo_cambio = cot.TipoCambio.ToString();
            x.serie = "";
            x.folio = cot.IDCotizacion.ToString();
            x.UsodelCFDI = cot.c_UsoCFDI.Descripcion;
            x.IDCotizacion = cot.Almacen.IDAlmacen;
            x.Empresa = cot.Almacen.Telefono;
            x.condicionesdepago = cot.CondicionesPago.Descripcion;
            x.vendedor = cot.Vendedor.Nombre;

            ImpuestoCO iva = new ImpuestoCO();
            iva.impuesto = "IVA";
            iva.tasa = 16;
            iva.importe = float.Parse(cot.IVA.ToString());


            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.DireccionCliente = cot.Clientes.Calle + " " + cot.Clientes.NumExt + " " + cot.Clientes.NumInt + "," + cot.Clientes.Colonia + "," + cot.Clientes.Municipio + "," + cot.Clientes.Estados.Estado;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
            x.firmadefinanzas = empresa.Director_finanzas;
            x.firmadecompras = empresa.Persona_de_Compras + "";

            List<DetCotizacion> detalles = db.Database.SqlQuery<DetCotizacion>("select * from [dbo].[DetCotizacion] where [IDCotizacion]= " + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoCO producto = new ProductoCO();
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

            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            try
            {
                CreaCOPDF documento = new CreaCOPDF(logoempresa, x);
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

        public ActionResult Trazabilidad()
        {



            return View();
        }

        public ActionResult EntreFechasCot()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasCot(EFecha modelo, FormCollection coleccion)
        {
            VCotizacionesContext dbe = new VCotizacionesContext();
            VDetCotizacionesContext dbr = new VDetCotizacionesContext();
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

                cadena = "select * from dbo.VCotizaciones where fecha >= '" + FI + "' and fecha  <='" + FF + "' ";
                var datos = dbe.Database.SqlQuery<VCotizaciones>(cadena).ToList();
                ViewBag.datos = datos;
                cadenaDet = "select * from [dbo].[VDetCotizaciones] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' ";
                var datosDet = dbr.Database.SqlQuery<VDetCotizaciones>(cadenaDet).ToList();
                ViewBag.datosDet = datosDet;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Cotizaciones");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:U1"].Style.Font.Size = 20;
                Sheet.Cells["A1:U1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:U3"].Style.Font.Bold = true;
                Sheet.Cells["A1:U1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:U1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Cotizaciones");

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
                Sheet.Cells["A3:V3"].Style.Font.Bold = true;
                Sheet.Cells["A3:V3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:V3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID Cotización");
                Sheet.Cells["B3"].RichText.Add("Fecha");
                Sheet.Cells["C3"].RichText.Add("Fecha Requiere");
                Sheet.Cells["D3"].RichText.Add("ID Cliente");
                Sheet.Cells["E3"].RichText.Add("NoExpediente");
                Sheet.Cells["F3"].RichText.Add("RFC");
                Sheet.Cells["G3"].RichText.Add("Cliente");
                Sheet.Cells["H3"].RichText.Add("Subtotal");
                Sheet.Cells["I3"].RichText.Add("IVA");
                Sheet.Cells["J3"].RichText.Add("Total");
                Sheet.Cells["K3"].RichText.Add("Moneda");
                Sheet.Cells["L3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["M3"].RichText.Add("Total en Pesos");
                Sheet.Cells["N3"].RichText.Add("Status");
                Sheet.Cells["O3"].RichText.Add("Condiciones de Pago");
                Sheet.Cells["P3"].RichText.Add("Forma de Pago");
                Sheet.Cells["Q3"].RichText.Add("Método de Pago");
                Sheet.Cells["R3"].RichText.Add("Uso CFDI");
                Sheet.Cells["S3"].RichText.Add("Vendedor");
                Sheet.Cells["T3"].RichText.Add("Oficina");
                Sheet.Cells["U3"].RichText.Add("Observación");


                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:U3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VCotizaciones item in ViewBag.datos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDCotizacion;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.FechaRequiere;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.IDCliente;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.noExpediente;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.RFC;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Cliente;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.TotalPesos;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.Status;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.CondicionesPago;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.FormaPago;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.MetodoPago;
                    Sheet.Cells[string.Format("R{0}", row)].Value = item.UsoCFDI;
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("T{0}", row)].Value = item.NombreOficina;
                    Sheet.Cells[string.Format("U{0}", row)].Value = item.Observacion;

                    row++;
                }

                //Hoja No. 2
                Sheet = Ep.Workbook.Worksheets.Add("Detalles");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:P1"].Style.Font.Size = 20;
                Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:P3"].Style.Font.Bold = true;
                Sheet.Cells["A1:P1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:P1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Detalle de Cotizaciones");

                row = 2;
                Sheet.Cells["A1:P1"].Style.Font.Size = 12;
                Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:P1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:P2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3:P3"].Style.Font.Bold = true;
                Sheet.Cells["A3:P3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:P3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID Detalle");
                Sheet.Cells["B3"].RichText.Add("ID Cotización");
                Sheet.Cells["C3"].RichText.Add("Fecha"); ;
                Sheet.Cells["D3"].RichText.Add("Cliente");
                Sheet.Cells["E3"].RichText.Add("Clave");
                Sheet.Cells["F3"].RichText.Add("Artículo");
                Sheet.Cells["G3"].RichText.Add("Presentación");
                Sheet.Cells["H3"].RichText.Add("Cantidad Pedida");
                Sheet.Cells["I3"].RichText.Add("Cantidad");
                Sheet.Cells["J3"].RichText.Add("Costo");
                Sheet.Cells["K3"].RichText.Add("Devolución");
                Sheet.Cells["L3"].RichText.Add("Importe");
                Sheet.Cells["M3"].RichText.Add("IVA");
                Sheet.Cells["N3"].RichText.Add("Total");
                Sheet.Cells["O3"].RichText.Add("Estado");
                Sheet.Cells["P3"].RichText.Add("Nota");
                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:P3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VDetCotizaciones itemD in ViewBag.datosDet)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.IDDetCotizacion;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.IDCotizacion;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemD.Cliente;
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemD.Cref;
                    Sheet.Cells[string.Format("F{0}", row)].Value = itemD.Articulo;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.Presentacion;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.CantidadPedida;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.Cantidad;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.Costo;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.Descuento;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.Importe;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = itemD.ImporteIva;
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("N{0}", row)].Value = itemD.ImporteTotal;
                    Sheet.Cells[string.Format("O{0}", row)].Value = itemD.Status;
                    Sheet.Cells[string.Format("P{0}", row)].Value = itemD.Nota;

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

    }
}