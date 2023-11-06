using Automatadll;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SIAAPI.ClasesProduccion;
using SIAAPI.Controllers.Cfdi;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.Models.PlaneacionProduccion;
using SIAAPI.Models.Produccion;
using SIAAPI.Reportes;
using SIAAPI.ViewModels.Articulo;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace SIAAPI.Controllers.Comercial
{
    public class EncPedidoController : Controller
    {
        private PedidoContext db = new PedidoContext();
        private ClientesContext prov = new ClientesContext();
        private VPedidosContext dbvp = new VPedidosContext();
        private VendedorContext dbv = new VendedorContext();

        public ActionResult Index(string Cliente, string Vendedor, string Oficina, string Divisa, string Status, string OCompra, string sortOrder, string currentFilter, string searchString, string Fechainicio, string Fechafinal, int? page, int? PageSize)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            List<UserRole> userrol = db.Database.SqlQuery<UserRole>("select * from [dbo].[UserRole] where userid='" + usuario + "'").ToList();

            bool todas = false;
            //foreach (UserRole roless in userrol)
            //{
            //    Roles rol = new RolesContext().Roless.Find(roless.RoleID);
            //    if (rol.ROleName == "Gerencia" || rol.ROleName == "GerenteVentas" || rol.ROleName == "Administrador" || rol.ROleName == "Sistemas" || rol.ROleName == "AdminProduccion" || rol.ROleName == "Almacenista")
            //    {
                    todas = true;
            //    }
            //}

            string ConsultaSql = "select top 500 * from [dbo].[VPedidos]";
            string FiltroSql = string.Empty;
            string OrdenSql = "order by IDPedido desc";
            string SumaSql = "select ClaveMoneda as MonedaOrigen, Sum(Subtotal) as Subtotal, Sum(IVA) as IVA, Sum(Total) as Total, sum(Total * TipoCambio) as TotalenPesos from dbo.VPedidos";
            string FiltroSuma = "and Status != 'Cancelado'";
            string GroupSql = "group by ClaveMoneda";
            string CadenaSql = string.Empty;
            string CadenaResumenSql = string.Empty;

            try
            {


                ViewBag.IDproveedorDesconocido = SIAAPI.Properties.Settings.Default.IDProveedorDesconocido;

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
                var StaQry = from d in dbvp.VPedidos
                             orderby d.IDPedido
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
                if (OCompra == " ")
                {
                    OCompra = string.Empty;
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
                if (!String.IsNullOrEmpty(OCompra))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where OCompra = '" + OCompra + "'";
                    }
                    else
                    {
                        FiltroSql += " and  OCompra = '" + OCompra + "'";
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
                        FiltroSql = "where IDPedido = " + int.Parse(searchString.ToString()) + "";
                    }
                    else
                    {
                        FiltroSql += " and  IDPedido = " + int.Parse(searchString.ToString()) + "";
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

                ViewBag.Clienteseleccionado = Cliente;
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

                ViewBag.OCompra = OCompra;
                ViewBag.CurrentSort = sortOrder;
                ViewBag.DivisaSortParm = String.IsNullOrEmpty(sortOrder) ? "Divisa" : "";
                ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) ? "Status" : "";
                ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
                ViewBag.EmpresaSortParm = String.IsNullOrEmpty(sortOrder) ? "Cliente" : "";
                ViewBag.Fechainicio = Fechainicio;
                ViewBag.Fechafinal = Fechafinal;

                switch (sortOrder)
                {
                    case "Pedido":
                        OrdenSql = " order by  IDPedido asc ";
                        break;

                    case "Cliente":
                        OrdenSql = " order by  Cliente, IDPedido desc ";
                        break;

                    default:
                        OrdenSql = "order by IDPedido desc ";
                        break;
                }

                if (FiltroSql == string.Empty)
                {
                    FiltroSql = "where (Fecha between  convert(varchar,DATEADD(mm, -4, getdate()), 23) and convert(varchar, getdate(), 23)) ";
                }

                CadenaSql = ConsultaSql + " " + FiltroSql + " " + OrdenSql;
                dbv.Database.CommandTimeout = 300;

                var elementos = dbv.Database.SqlQuery<VPedidos>(CadenaSql).ToList();
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
        public ActionResult Autorizar(int? id, int page = 1)
        {

            EncPedido encPedido = db.EncPedidos.Find(id);
            string fecha = DateTime.Now.ToString("yyyyMMdd");

            List<User> userid;
            userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            List<DetPedido> pedidos = db.Database.SqlQuery<DetPedido>("select DetPedido.GeneraOrdenP,DetPedido.IDRemision,DetPedido.IDPrefactura,DetPedido.IDDetPedido,DetPedido.IDPedido,DetPedido.IDArticulo,DetPedido.Caracteristica_ID,DetPedido.Costo,DetPedido.CantidadPedida,DetPedido.Cantidad,DetPedido.Descuento,DetPedido.IDAlmacen,DetPedido.Importe,DetPedido.IVA,DetPedido.ImporteIva,DetPedido.ImporteTotal,DetPedido.Suministro,DetPedido.Nota, DetPedido.IDAlmacen,DetPedido.Status,DetPedido.Ordenado,Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetPedido INNER JOIN Caracteristica ON DetPedido.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPedido='" + id + "'").ToList();
            // ViewBag.pedidos = pedido;
            int AlmacenViene = 0;

            foreach (DetPedido detalle in pedidos)
            {

                Articulo articulo = new ArticuloContext().Articulo.Find(detalle.IDArticulo);
                AlmacenViene = detalle.IDAlmacen;
                if (articulo.CtrlStock)
                {
                    if (articulo.esKit)
                    {
                        InventarioAlmacen invKit = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(a => a.IDAlmacen == AlmacenViene && a.IDCaracteristica == detalle.Caracteristica_ID).FirstOrDefault();

                        decimal existenciakit = 0;

                        if (invKit != null)
                        {
                            /// kit
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado+" + detalle.Cantidad + "), Disponibilidad=(Disponibilidad-" + detalle.Cantidad + ") where IDArticulo= " + detalle.IDArticulo + " and IDCaracteristica=" + detalle.Caracteristica_ID + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0  where apartado<0 and IDArticulo= " + detalle.IDArticulo + " and IDCaracteristica=" + detalle.Caracteristica_ID + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set disponibilidad=(existencia-apartado)  where IDArticulo= " + detalle.IDArticulo + " and IDCaracteristica=" + detalle.Caracteristica_ID + " and IDAlmacen=" + AlmacenViene + "");

                            // existenciakit = invKit.Existencia;
                        }
                        else
                        {

                            db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                + AlmacenViene + "," + detalle.IDArticulo + "," + detalle.Caracteristica_ID + ",0,0," + detalle.Cantidad + ",(0-" + detalle.Cantidad + "))");

                            existenciakit = 0;
                        }
                         int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                        try
                        {
                            // movimiento kit
                            Caracteristica carateristicakit = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + detalle.Caracteristica_ID).ToList().FirstOrDefault();

                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                            cadenam += " (getdate(), " + carateristicakit.ID + "," + carateristicakit.IDPresentacion + "," + detalle.IDArticulo + ",'Pedido',";
                            cadenam += detalle.Cantidad + ",'Pedido Kit'," + detalle.IDPedido + ",'',";
                            cadenam += detalle.IDAlmacen + ",'N/A'," + (existenciakit) + ",'',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";

                            db.Database.ExecuteSqlCommand(cadenam);
                        }
                        catch (Exception err)
                        {

                        }
                        ArticuloContext dbart = new ArticuloContext();
                        List<Kit> liAC;
                        liAC = dbart.Database.SqlQuery<Kit>("select * from [dbo].[Kit] where idarticulo=" + articulo.IDArticulo).ToList();

                        ViewBag.articulos = liAC;


                        foreach (Kit c in liAC)
                        {
                            Articulo articulocom = new ArticuloContext().Articulo.Find(c.IDArticuloComp);
                            if (articulocom.CtrlStock)
                            {
                                decimal existencia = 0;
                                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(a => a.IDAlmacen == AlmacenViene && a.IDCaracteristica == c.IDCaracteristica).FirstOrDefault();

                                decimal cantidad = detalle.Cantidad * c.Cantidad;
                                if (inv != null)
                                {
                                    // componente kit
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado+" + cantidad + "), Disponibilidad=(Disponibilidad-" + cantidad + ") where IDArticulo= " + c.IDArticuloComp + " and IDCaracteristica=" + c.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where apartado<0 and IDArticulo= " + c.IDArticuloComp + " and IDCaracteristica=" + c.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set disponibilidad=(existencia-apartado) where IDArticulo= " + c.IDArticuloComp + " and IDCaracteristica=" + c.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");

                                    existencia = inv.Existencia;
                                }
                                else
                                {
                                    db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                        + AlmacenViene + "," + c.IDArticuloComp + "," + c.IDCaracteristica + ",0,0," + cantidad + ",(0-" + cantidad + "))");


                                    existencia = 0;
                                }

                                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + c.IDCaracteristica).ToList().FirstOrDefault();
                                try
                                {

                                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + c.IDArticuloComp + ",'Pedido',";
                                    cadenam += cantidad + ",'Pedido '," + detalle.IDPedido + ",'',";
                                    cadenam += detalle.IDAlmacen + ",'N/A'," + (existencia) + ",'',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";

                                    db.Database.ExecuteSqlCommand(cadenam);
                                }
                                catch (Exception err)
                                {

                                }

                            }

                        }

                    }
                    else // no es kit
                    {
                        decimal existencia = 0;
                        InventarioAlmacen inv = null;
                        try
                        {
                            inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == detalle.Caracteristica_ID).FirstOrDefault();
                        }
                        catch (Exception err)
                        {

                        }
                        if (inv != null)
                        {
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado+" + detalle.Cantidad + "), Disponibilidad=(Disponibilidad-" + detalle.Cantidad + ") where IDArticulo= " + detalle.IDArticulo + " and IDCaracteristica=" + detalle.Caracteristica_ID + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where  apartado<0 and IDArticulo= " + detalle.IDArticulo + " and IDCaracteristica=" + detalle.Caracteristica_ID + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set disponibilidad=(existencia-apartado) where   IDArticulo= " + detalle.IDArticulo + " and IDCaracteristica=" + detalle.Caracteristica_ID + " and IDAlmacen=" + AlmacenViene + "");

                            existencia = inv.Existencia;
                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                             + detalle.IDAlmacen + "," + detalle.IDArticulo + "," + detalle.Caracteristica_ID + ",0,0," + detalle.Cantidad + ",(0-" + detalle.Cantidad + "))");
                            existencia = 0;
                        }

                        int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                        Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + detalle.Caracteristica_ID).ToList().FirstOrDefault();
                        try
                        {
                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                            cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + detalle.IDArticulo + ",'Pedido',";
                            cadenam += detalle.Cantidad + ",'Pedido '," + detalle.IDPedido + ",'',";
                            cadenam += detalle.IDAlmacen + ",'N/A'," + (existencia) + ",'',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";

                            db.Database.ExecuteSqlCommand(cadenam);
                        }
                        catch (Exception err)
                        {

                        }
                    }

                
                //////////////////77777ArticulosComprados

                int car = detalle.Caracteristica_ID;
                int articuloD = detalle.IDArticulo;
                ClsDatoEntero cuantoshay = db.Database.SqlQuery<ClsDatoEntero>("select count(idarticulo) as Dato from dbo.ArticulosComprados where IDCaracteristica = " + car + " and IDArticulo = " + articuloD + " and IDCliente ='" + encPedido.IDCliente + "'").ToList()[0];
                Articulo art = db.Database.SqlQuery<Articulo>("select * from Articulo where IDArticulo ='" + articuloD + "'").ToList().FirstOrDefault();
                Clientes clie = db.Database.SqlQuery<Clientes>("select * from Clientes  where IDCliente ='" + encPedido.IDCliente + "'").ToList().FirstOrDefault();
                try
                {
                    if (cuantoshay.Dato == 0)
                    {
                        string cadenaIns = ("insert into dbo.ArticulosComprados([IDArticulo],[Fecha],[cantidad],[Cref],[IDCliente],[IDCaracteristica],[Cliente],[Descripcion],[Presentacion],[nameFoto]) ");
                        cadenaIns += (" values('" + detalle.IDArticulo + "', '" + fecha + "', '" + detalle.Cantidad + "', '" + art.Cref + "','" + encPedido.IDCliente + "', '" + detalle.Caracteristica_ID + "','" + clie.Nombre + "','" + art.Descripcion + "','" + detalle.Presentacion + "', '" + art.nameFoto + "')");
                        db.Database.ExecuteSqlCommand(cadenaIns);

                    }
                    if (cuantoshay.Dato > 0)
                    {
                        string cadenaUp = ("update dbo.ArticulosComprados set Fecha= '" + fecha + "',  [cantidad]= '" + detalle.Cantidad + "', nameFoto = '" + art.nameFoto + "' where IDCaracteristica ='" + detalle.Caracteristica_ID + "' and IDArticulo = '" + detalle.IDArticulo + "' and IDCliente ='" + encPedido.IDCliente + "' ");
                        db.Database.ExecuteSqlCommand(cadenaUp);

                    }
                }
                catch (Exception err)
                {

                }

            }
            //  db.Database.ExecuteSqlCommand("exec dbo.MovArt '"+ fecha + "'," + detalle.Caracteristica_ID + ",'PedVta'," + detalle.Cantidad + ",'Pedido'," + id + ",0,'" + detalle.IDAlmacen + "','" + detalle.Nota + "',0");
            if (articulo.esKit)
                {
                    ArticuloContext dbart = new ArticuloContext();
                    List<Kit> liAC;
                    liAC = dbart.Database.SqlQuery<Kit>("select * from [dbo].[Kit] where idarticulo=" + articulo.IDArticulo).ToList();
                    foreach (Kit kitd in liAC)
                    {
                        Articulo articulok = new ArticuloContext().Articulo.Find(kitd.IDArticuloComp);
                        Caracteristica carateristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + kitd.IDCaracteristica).ToList().FirstOrDefault();
                        if (!articulok.GeneraOrden)
                        {
                            if (articulok.CtrlStock)
                            {


                                decimal cantidadk = kitd.Cantidad * detalle.Cantidad;
                                string costocad = "(select (dbo.GetCosto(0," + articulok.IDArticulo + ", " + cantidadk + "))  *(dbo. GetTipocambio('" + fecha + "', " + articulok.IDMoneda + ", " + encPedido.IDMoneda + ")) as Dato)";
                                string costo = db.Database.SqlQuery<ClsDatoDecimal>(costocad).ToList().FirstOrDefault().Dato.ToString();
                                string importecosto = "((" + costo + ") * " + cantidadk + ")";
                                string importeivacosto = importecosto + "*(" + SIAAPI.Properties.Settings.Default.ValorIVA + ")";
                                string importetotalcosto = "(" + importecosto + ")" + "+(" + importeivacosto + ")";
                                if (articulo.Cref != "GRABADO")
                                {
                                    string cadenac = "INSERT INTO DetSolicitud([IDArticulo], [Caracteristica_ID],Documento,Numero,[Cantidad] ,[Costo] ,[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Requerido],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion]) values (" + articulok.IDArticulo + "," + carateristica.ID + ",'Pedido'," + encPedido.IDPedido + "," + cantidadk + "," + costo + "," + cantidadk + ",'0', " + importecosto + " ,'true'," + importeivacosto + "," + importetotalcosto + ",'" + detalle.Nota + "','0'," + detalle.IDAlmacen + ",0,'Solicitado','" + carateristica.Presentacion + "','" + carateristica.jsonPresentacion + "');";

                                    db.Database.ExecuteSqlCommand(cadenac);
                                }
                            }
                        }


                    }

                }


                if (!articulo.GeneraOrden && !articulo.esKit)
                {



                    if (articulo.CtrlStock)
                    {
                        string costocad = "(select (dbo.GetCosto(0," + detalle.IDArticulo + ", " + detalle.Cantidad + "))  *(dbo. GetTipocambio('" + fecha + "', " + articulo.IDMoneda + ", " + encPedido.IDMoneda + ")) as Dato)";
                        string costo = db.Database.SqlQuery<ClsDatoDecimal>(costocad).ToList().FirstOrDefault().Dato.ToString();
                        string importecosto = "((" + costo + ") * " + detalle.Cantidad + ")";
                        string importeivacosto = importecosto + "*(" + SIAAPI.Properties.Settings.Default.ValorIVA + ")";
                        string importetotalcosto = "(" + importecosto + ")" + "+(" + importeivacosto + ")";
                        if (articulo.Cref != "GRABADO")
                        {
                            string cadenac = "INSERT INTO DetSolicitud([IDArticulo], [Caracteristica_ID],Documento,Numero,[Cantidad] ,[Costo] ,[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Requerido],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion]) values (" + detalle.IDArticulo + "," + detalle.Caracteristica_ID + ",'Pedido'," + encPedido.IDPedido + "," + detalle.Cantidad + "," + costo + "," + detalle.Cantidad + ",'0', " + importecosto + " ,'true'," + importeivacosto + "," + importetotalcosto + ",'" + detalle.Nota + "','0'," + detalle.IDAlmacen + ",0,'Solicitado','" + detalle.Presentacion + "','" + detalle.jsonPresentacion + "');";
                            db.Database.ExecuteSqlCommand(cadenac);
                        }
                    }
                }

            }

            db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [Status]='Activo' where [IDPedido]='" + id + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [Status]='Activo' where [IDPedido]='" + id + "'");
            db.Database.ExecuteSqlCommand("insert into [dbo].[MovAutorizacion] ([Documento],[IDDocumento],[Fecha],[UserID]) values('Pedido','" + encPedido.IDPedido + "','" + fecha + "','" + UserID + "')");

            return RedirectToAction("Index", new { page = page });
        }

        

        public ActionResult Details(int? id, int page, string  searchString)
        {
            
			System.Web.HttpContext.Current.Session["idpedido"] = id;

            List<VDetPedido> pedido = db.Database.SqlQuery<VDetPedido>("select* from VDetPedido where IDPedido='" + id + "'").ToList();

            ViewBag.req = pedido;
            ViewBag.page = page;
            ViewBag.searchString = searchString;

            var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPedido.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPedido inner join Clientes on Clientes.IDCliente=EncPedido.IDCliente where EncPedido.IDPedido='" + id + "' group by EncPedido.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EncPedido encPedido = db.EncPedidos.Find(id);
            if (encPedido == null)
            {
                return HttpNotFound();
            }
            return View(encPedido);
        }
 		public ActionResult OrdenesAGenerar(int id, string Mensaje="")
        {
            List<VDetPedido> pedido = new List<VDetPedido>();

            
            ViewBag.Mensaje = Mensaje;
            string cadena = "select DetPedido.IDDetPedido, DetPedido.IDArticulo, DetPedido.IDAlmacen,Articulo.GeneraOrden,Articulo.Cref as Cref,DetPedido.Suministro,DetPedido.GeneraOrdenP,DetPedido.IDRemision,DetPedido.IDPrefactura,DetPedido.Status,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetPedido.IDDetExterna,DetPedido.IDPedido,Articulo.Descripcion as Articulo,DetPedido.Cantidad,DetPedido.Costo,DetPedido.CantidadPedida,DetPedido.Descuento,DetPedido.Importe,DetPedido.IVA,DetPedido.ImporteIva,DetPedido.ImporteTotal, DetPedido.Nota,DetPedido.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from(DetPedido INNER JOIN Caracteristica ON DetPedido.Caracteristica_ID = Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo)  where detpedido.IDPedido = " + id + "";
            List<VDetPedido> pedidosin  = db.Database.SqlQuery<VDetPedido>(cadena).ToList();
            foreach (VDetPedido detalle in pedidosin)
            {
               
                OrdenProduccion ord = new OrdenProduccionContext().OrdenesProduccion.Where(s => s.IDDetPedido == detalle.IDDetPedido).FirstOrDefault();
                OrdenProduccion ordenP = new OrdenProduccionContext().OrdenesProduccion.Where(s => s.IDDetPedido == detalle.IDDetPedido && s.EstadoOrden!="Cancelada").FirstOrDefault();

                Articulo ar = new ArticuloContext().Articulo.Find(detalle.IDArticulo);
                if (ordenP != null)
                {
                    if (Mensaje != "")
                    {

                    }
                    else
                    {
                        return RedirectToAction("OrdenesAGenerar", new { id = id, Mensaje = "No se puede generar OP, porque ya tiene una OP, artículo" + ar.Cref });

                    }
                }

                if (ord==null)
                {
                    
                        pedido.Add(detalle);
                                    
                }
                try 
                {

                    if(detalle.IDDetPedido==ord.IDDetPedido && ord.EstadoOrden.Equals("Cancelada"))
                    {
                        pedido.Add(detalle);
                    }

                } catch(Exception err)
                {
                }
            }

            ViewBag.req = pedido;
            return View();
        }


        public ActionResult GeneraOrden(FormCollection collection)
        {

            int idpedido = int.Parse(System.Web.HttpContext.Current.Session["idpedido"].ToString());
			var datosOrden = new List<string>();
            datosOrden.Add(collection.Get("Cantidad"));
            datosOrden.Add(collection.Get("iddetpedido"));
            datosOrden.Add(collection.Get("idarticulo"));
            datosOrden.Add(collection.Get("idcotizacionarticulo"));
            datosOrden.Add(collection.Get("Observacion"));


            string[] valoresdecantidad = datosOrden[0].Split(',');
            string[] valoresiddetpedido = datosOrden[1].Split(',');
            string[] valoresidarticulo = datosOrden[2].Split(',');
            string[] valoresidcotizacionarticulo = datosOrden[3].Split(',');
            string[] valoresobservacionarticulo = datosOrden[4].Split(',');

            int renglones = valoresdecantidad.Length;

            for (int con = 0; con < renglones; con++)
            {
               

                decimal cantidad = decimal.Parse(valoresdecantidad[con]); //
                if (cantidad > 0)
                {
                    int id = int.Parse(valoresiddetpedido[con]);
                    int idarticulocomp = int.Parse(valoresidarticulo[con]);
                    string indicaciones = valoresobservacionarticulo[con];

                    int idhe = 0;
                    List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                    int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                    DetPedido detpedido = db.DetPedido.Find(id);
                    OrdenProduccion ordenP = new OrdenProduccionContext().OrdenesProduccion.Where(s => s.IDDetPedido == detpedido.IDDetPedido && s.EstadoOrden != "Cancelada").FirstOrDefault();

                    if (ordenP==null)
                    {
                        EncPedido pedido = db.EncPedidos.Find(detpedido.IDPedido);
                        VCaracteristicaContext presentacionBase = new VCaracteristicaContext();
                        Articulo articulos = new ArticuloContext().Articulo.Find(detpedido.IDArticulo);

                        if (articulos.esKit)
                        {
                            ArticuloContext dbart = new ArticuloContext();
                            List<Kit> liAC;
                            liAC = dbart.Database.SqlQuery<Kit>("select * from [dbo].[Kit] where idarticulo=" + articulos.IDArticulo + " and IDArticuloComp=" + idarticulocomp).ToList();

                            // ViewBag.articulos = liAC;


                            foreach (Kit c in liAC)
                            {
                                Articulo articulo = new ArticuloContext().Articulo.Find(c.IDArticuloComp);


                                if (articulo.GeneraOrden)
                                {


                                    VCaracteristica presentacion1 = presentacionBase.VCaracteristica.Find(c.IDCaracteristica);
                                    ClsDatoEntero dato1 = new HEspecificacionEContext().Database.SqlQuery<ClsDatoEntero>("select IDCotizacion as Dato from Caracteristica where id=" + presentacion1.ID).FirstOrDefault();
                                    idhe = dato1.Dato;

                                    if (idhe == 0)
                                    {
                                        return Json(new { errorMessage = "El producto no  ha sido cotizado " + articulos.Cref + " No de presentacion " + presentacion1.IDPresentacion });
                                    }

                                    /// 
                                }
                            }



                        }

                        else
                        {
                            VCaracteristica presentacion = presentacionBase.VCaracteristica.Find(detpedido.Caracteristica_ID);

                            ClsDatoEntero dato2 = new HEspecificacionEContext().Database.SqlQuery<ClsDatoEntero>("select IDCotizacion as Dato from Caracteristica where id=" + presentacion.ID).FirstOrDefault();

                            idhe = dato2.Dato;
                            if (idhe == 0)
                            {
                                return Json(new { errorMessage = "El producto no  ha sido cotizado " + articulos.Cref + " No de presentacion " + presentacion.IDPresentacion });
                            }
                        }

                        try
                        {
                            //HEspecificacionE hespecificacion = new HEspecificacionEContext().HEspecificacionesE.Find(idhe);

                            automata Automata;

                            XmlSerializer serializer = new XmlSerializer(typeof(automata));

                            string path = Path.Combine(Server.MapPath("~/Automatas/Ordenes.Xml"));


                            using (TextReader reader = new StreamReader(path))
                            {
                                serializer = new XmlSerializer(typeof(automata));
                                Automata = (automata)serializer.Deserialize(reader);
                            }

                            Automata.establecerestadoinicial();

                            string estadoinicial = Automata.Estadoactual.Nombre;


                            bool modeloTermoEncogible = false;
                            bool modeloMangaTranparente = false;
                            bool modeloAdherible = false;
                            modeloTermoEncogible = siTermoencongible(int.Parse(valoresidcotizacionarticulo[con]));
                            modeloMangaTranparente = siTermoencongibleTrans(int.Parse(valoresidcotizacionarticulo[con]));
                            modeloAdherible = siAdherible(int.Parse(valoresidcotizacionarticulo[con]));
                            int ModelosDeProduccion_IDModeloProduccion = 0;
                            if (modeloTermoEncogible)
                            {
                                ModelosDeProduccion_IDModeloProduccion = 8;
                            }
                            if (modeloMangaTranparente)
                            {
                                ModelosDeProduccion_IDModeloProduccion = 7;
                            }
                            if (modeloAdherible)
                            {
                                ModelosDeProduccion_IDModeloProduccion = 4;
                            }
                            if (ModelosDeProduccion_IDModeloProduccion == 0)
                            {
                                return Json(new { errorMessage = "Sin modelo de producción" });
                            }


                            string MensajeValidacion = "";
                            try
                            {
                                MensajeValidacion = ValidadArticulosOP(idhe, ModelosDeProduccion_IDModeloProduccion);

                                if (MensajeValidacion !="")
                                {
                                    return Json(new { errorMessage = MensajeValidacion });
                                }
                            } catch (Exception err) {
                            }
                            int idprocesoactual = db.Database.SqlQuery<int>("select Proceso_IDProceso from ModeloProceso where ModelosDeProduccion_IDModeloProduccion='" + ModelosDeProduccion_IDModeloProduccion + "' and orden=1").FirstOrDefault();
                            string fecharequiere = pedido.FechaRequiere.ToString("yyyy/MM/dd");
                            string fecharegistro = pedido.Fecha.ToString("yyyy/MM/dd");

                            ArticulosProduccionContext dba = new ArticulosProduccionContext();
                            Articulo articuloaproducion = new ArticuloContext().Articulo.Find(detpedido.IDArticulo);

                            int idordenproduccion = 0;
                            string fecha = "";

                            if (articulos.esKit)
                            {
                                ArticuloContext dbart = new ArticuloContext();
                                string cadenaki = "select * from [dbo].[Kit] where idarticulo=" + articulos.IDArticulo + " and IDArticuloComp=" + idarticulocomp;
                                List<Kit> liAC;
                                liAC = dbart.Database.SqlQuery<Kit>(cadenaki).ToList();

                                //  ViewBag.articulos = liAC;

                                foreach (Kit comp in liAC)
                                {
                                    Articulo articulo = new ArticuloContext().Articulo.Find(comp.IDArticuloComp);

                                    if (articulo.GeneraOrden)
                                    {
                                        VCaracteristica presentacion1 = presentacionBase.VCaracteristica.Find(comp.IDCaracteristica);
                                        db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[OrdenProduccion] ([IDModeloProduccion],[IDCliente],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[Indicaciones],[FechaCompromiso],[FechaInicio],[FechaProgramada],[FechaRealdeInicio],[FechaRealdeTerminacion],[Cantidad],[IDPedido],[IDDetPedido],[Prioridad],[EstadoOrden],[UserID],[Liberar],idhe) VALUES(" + ModelosDeProduccion_IDModeloProduccion + ",'" + pedido.IDCliente + "','" + articulo.IDArticulo + "','" + comp.IDCaracteristica + "','" + articulo.Descripcion + "','" + presentacion1.Presentacion + "','" + indicaciones + "','" + fecharequiere + "','" + fecharegistro + "',null,null,null,'" + cantidad + "','" + detpedido.IDPedido + "','" + id + "',0,'" + estadoinicial + "'," + usuario + ",'Activo'," + idhe + ")");
                                        idordenproduccion = db.Database.SqlQuery<int>("select max(IDOrden) from OrdenProduccion").FirstOrDefault();
                                        fecha = DateTime.Now.ToString("yyyy-MM-dd");

                                        // db.Database.ExecuteSqlCommand("exec dbo.OrdPro " + comp.IDCaracteristica + ", 'OrdPro',0," + cantidad + ", 'Orden'," + idordenproduccion + ",''," + detpedido.IDAlmacen + ",'" + detpedido.Nota + "'");
                                        DetPedido dpedido = db.Database.SqlQuery<DetPedido>("select * from [dbo].[detPedido] where iddetpedido=" + id).FirstOrDefault();
                                        int AlmacenViene = 0;


                                        AlmacenViene = dpedido.IDAlmacen;



                                        decimal existencia = 0;
                                        decimal llegar = 0;
                                        InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == comp.IDCaracteristica).FirstOrDefault();

                                        if (ia != null)
                                        {

                                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set PorLlegar=(PorLlegar+" + cantidad + ") where IDArticulo= " + idarticulocomp + "and IDCaracteristica=" + comp.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                                            existencia = ia.Existencia;
                                        }
                                        else
                                        {
                                            db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                                + AlmacenViene + "," + idarticulocomp + "," + comp.IDCaracteristica + ",0," + cantidad + ",0,0)");
                                            existencia = 0;
                                        }
                                        

                                        try
                                        {

                                            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + comp.IDCaracteristica).ToList().FirstOrDefault();
                                            InventarioAlmacen iai = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == comp.IDCaracteristica).FirstOrDefault();

                                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                                            cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Orden Producción'," + cantidad + ",'Orden Producción '," + idordenproduccion + ",''," + AlmacenViene + ",'NA'," + iai.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";
                                            db.Database.ExecuteSqlCommand(cadenam);
                                        }
                                        catch (Exception err)
                                        {
                                            string mensajee = err.Message;
                                        }



                                        var lista1 = new VistaModeloProcesoContext().Database.SqlQuery<VistaModeloProceso>("Select * from [VModeloProceso] where idModeloProduccion=" + ModelosDeProduccion_IDModeloProduccion + " order by orden").ToList();
                                        ViewBag.listaprocesos = lista1;



                                        for (int i = 0; i < lista1.Count(); i++)
                                        {

                                            db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[OrdenProduccionDetalle] ([IDOrden],[IDProceso],[EstadoProceso]) VALUES ('" + idordenproduccion + "','" + ViewBag.listaprocesos[i].IDProceso + "','Conflicto')");
                                        }

                                        decimal tc = db.Database.SqlQuery<ClsDatoDecimal>("select dbo.GetTipocambio(GETDATE(),(select idMoneda from C_MONEDA WHERE clavemoneda='USD'),(select idMoneda from C_MONEDA WHERE clavemoneda='MXN')  )  as Dato").ToList()[0].Dato;

                                        string queonda = CrearOrden(idhe, cantidad, idordenproduccion, tc, detpedido.Articulo.IDMoneda, ModelosDeProduccion_IDModeloProduccion);



                                        //List<DocumentoE> documentos = db.Database.SqlQuery<DocumentoE>("select * from DocumentoE where IDHE='" + idhe + "'").ToList();
                                        //ViewBag.Documentos = documentos;

                                        //for (int i = 0; i < documentos.Count; i++)
                                        //{
                                        //    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[DocumentoProduccion] ([IDOrden],[Version],[IDProceso],[Descripcion],[Planeacion],[Nombre]) VALUES('" + idordenproduccion + "','" + ViewBag.Documentos[i].Version + "','" + ViewBag.Documentos[i].IDProceso + "','" + ViewBag.Documentos[i].Descripcion + "','" + ViewBag.Documentos[i].Planeacion + "','" + ViewBag.Documentos[i].Nombre + "')");

                                        //}


                                    } // fin de si el articulo del kit genera orden
                                }

                            }
                            else  // el articulo no es kit
                            {
                                string cadenaorden = "INSERT INTO [dbo].[OrdenProduccion] ([IDModeloProduccion],[IDCliente],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[Indicaciones],[FechaCompromiso],[FechaInicio],[FechaProgramada],[FechaRealdeInicio],[FechaRealdeTerminacion],[Cantidad],[IDPedido],[IDDetPedido],[Prioridad],[EstadoOrden],[UserID],[Liberar],idhe) VALUES(" +
                                     ModelosDeProduccion_IDModeloProduccion + ",'" + pedido.IDCliente + "','" + detpedido.IDArticulo + "','" + detpedido.Caracteristica_ID + "','" + articuloaproducion.Descripcion + "','" + detpedido.Presentacion + "','" + indicaciones + "','" + fecharequiere + "','" + fecharegistro + "',null,null,null,'" + cantidad + "','" + detpedido.IDPedido + "','" + id + "',0,'" + estadoinicial + "'," + usuario + ",'Activo'," + idhe + ")";
                                db.Database.ExecuteSqlCommand(cadenaorden);
                                idordenproduccion = db.Database.SqlQuery<int>("select max(IDOrden) from OrdenProduccion").FirstOrDefault();

                                fecha = DateTime.Now.ToString("yyyy-MM-dd");

                                DetPedido dpedido = db.Database.SqlQuery<DetPedido>("select * from [dbo].[detPedido] where iddetpedido=" + id).FirstOrDefault();
                                int AlmacenViene = 0;
                                int cara = 0;

                                AlmacenViene = dpedido.IDAlmacen;
                                cara = dpedido.Caracteristica_ID;

                                decimal existencia = 0;
                                decimal llegar = 0;
                                InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == cara).FirstOrDefault();

                                if (ia != null)
                                {

                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set PorLlegar=(PorLlegar+" + cantidad + ") where IDArticulo= " + detpedido.IDArticulo + " and IDCaracteristica=" + cara + " and IDAlmacen=" + AlmacenViene + "");
                                    existencia = ia.Existencia;
                                }
                                else
                                {
                                    db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                        + AlmacenViene + "," + detpedido.IDArticulo + "," + cara + ",0," + cantidad + ",0,0)");
                                    existencia = 0;
                                }


                                try
                                {
                                    ClsDatoEntero ORDEN = new OrdenCompraContext().Database.SqlQuery<ClsDatoEntero>("select idorden as Dato from OrdenProduccion where iddetpedido=" + id).ToList().FirstOrDefault();
                                    InventarioAlmacen iai = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == cara).FirstOrDefault();

                                    Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + cara).ToList().FirstOrDefault();

                                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Orden Producción'," + (existencia + cantidad) + ",'Orden Producción '," + ORDEN.Dato + ",''," + AlmacenViene + ",'NA'," + iai.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";
                                    db.Database.ExecuteSqlCommand(cadenam);
                                }
                                catch (Exception err)
                                {
                                    string mensajee = err.Message;
                                }

                                var lista = new VistaModeloProcesoContext().Database.SqlQuery<VistaModeloProceso>("Select * from [VModeloProceso] where idModeloProduccion=" + ModelosDeProduccion_IDModeloProduccion + " order by orden").ToList();
                                ViewBag.listaprocesos = lista;



                                for (int i = 0; i < lista.Count(); i++)
                                {

                                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[OrdenProduccionDetalle] ([IDOrden],[IDProceso],[EstadoProceso]) VALUES ('" + idordenproduccion + "','" + ViewBag.listaprocesos[i].IDProceso + "','Conflicto')");

                                }

                                decimal tc = db.Database.SqlQuery<ClsDatoDecimal>("select dbo.GetTipocambio(GETDATE(),(select idMoneda from C_MONEDA WHERE clavemoneda='USD'),(select idMoneda from C_MONEDA WHERE clavemoneda='MXN')  )  as Dato").ToList()[0].Dato;

                                string queonda = CrearOrden(idhe, cantidad, idordenproduccion, tc, detpedido.Articulo.IDMoneda, ModelosDeProduccion_IDModeloProduccion);



                            }



                        }
                        catch (Exception err)
                        {
                            string mensaje = err.Message;
                            return Json(new { errorMessage = "El producto tuvo un error " + articulos.Cref + " ->" + err.Message });
                        }
                    }
                   
                }
            }
            return RedirectToAction("Details", new { id = idpedido , page=1  });

        }


        public ActionResult Cancelar(int? id)
        {
            List<User> userid;
            userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            try
            {
                List<OrdenProduccion> op = new OrdenProduccionContext().Database.SqlQuery<OrdenProduccion>("select*from Ordenproduccion where idpedido="+ id+" and (estadoOrden<>'Cancelada' and estadoOrden<>'Conflicto')").ToList();

                if (op.Count > 0)
                {
                    throw new Exception("Hay ordenes de producción activas, por lo que no es posible cancelar el pedido");
                }
                List<OrdenProduccion> oprnConflcito = new OrdenProduccionContext().Database.SqlQuery<OrdenProduccion>("select*from Ordenproduccion where idpedido=" + id + " and estadoOrden='Conflicto'").ToList();


                foreach (OrdenProduccion opp in oprnConflcito)
                {
                      string usuarioNombre = userid.Select(s => s.Username).FirstOrDefault();
                    OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(opp.IDOrden);


                    try
                    {
                        string cadena3 = " insert into dbo.CambioEstado(IDOrden, fecha, hora, EstadoAnterior, EstadoActual, motivo, usuario) values " +
                            "(" + opp.IDOrden + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "','" + opp.EstadoOrden + "', 'Cancelada', 'Cancelación de pedido no autorizado','" + usuarioNombre + "')";
                        db.Database.ExecuteSqlCommand(cadena3);


                        db.Database.ExecuteSqlCommand("update ordenproduccion set estadoorden='Cancelada' where idorden="+ opp.IDOrden);



                    }
                    catch (Exception err)
                    {

                    }

                }

                List<DetSolicitud> listasolicitudes = new ArticuloContext().Database.SqlQuery<DetSolicitud>("select * from detsolicitud where Documento='Pedido' and numero=" + id + " and status='Requerido' ").ToList();
                if (listasolicitudes.Count > 0)
                {
                    throw new Exception("Hay requisiciones de compra activas, por lo que no es posible cancelar el pedido");
                }

                List<DetPedido> detalles = new PrefacturaContext().Database.SqlQuery<DetPedido>("Select * from detpedido where IDpedido=" + id + " and status!='Cancelado'").ToList();


                foreach (DetPedido detalle in detalles)
                {

                    List<DetRemision> detalleremsion = new PrefacturaContext().Database.SqlQuery<DetRemision>("Select * from detRemision where IDDetExterna=" + detalle.IDDetPedido + " and status!='Cancelado'").ToList();

                    if (detalleremsion.Count > 0)
                    {
                        throw new Exception("Hay remisiones activas, por lo que no es posible cancelar el pedido");
                    }

                }

                string fecha = DateTime.Now.ToString("yyyyMMdd");

                EncPedido encPedido = db.EncPedidos.Find(id);
                List<VDetPedido> pedido = db.Database.SqlQuery<VDetPedido>("select DetPedido.Status,DetPedido.IDAlmacen,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetPedido.IDDetExterna,DetPedido.IDPedido,Articulo.Descripcion as Articulo,DetPedido.Cantidad,DetPedido.Costo,DetPedido.CantidadPedida,DetPedido.Descuento,DetPedido.Importe,DetPedido.IVA,DetPedido.ImporteIva,DetPedido.ImporteTotal, DetPedido.Nota,DetPedido.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetPedido INNER JOIN Caracteristica ON DetPedido.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPedido='" + id + "'").ToList();

                ViewBag.ordenes = pedido;
           

                foreach (var details in pedido)
                {
                    Caracteristica carateristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + details.Caracteristica_ID).ToList().FirstOrDefault();
                    Articulo articulodetalle = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);
                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();
                   
                        if (inv != null)
                        {


                            if (articulodetalle.CtrlStock)
                        {
                            if (details.Status == "Activo")
                            {
                                string c = "UPDATE dbo.InventarioAlmacen SET  Apartado =(Apartado-" + details.Cantidad + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID;
                                db.Database.ExecuteSqlCommand(c);

                                string csa = "UPDATE dbo.InventarioAlmacen SET Apartado=0 WHERE Apartado<0 and IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID;
                                db.Database.ExecuteSqlCommand(csa);

                                string c1 = "UPDATE dbo.InventarioAlmacen SET  Disponibilidad =(existencia-apartado) WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID;
                                db.Database.ExecuteSqlCommand(c1);

                                try
                                {
                                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Pedido'," + details.Cantidad + ",'Pedido '," + details.IDPedido + ",''," + details.IDAlmacen + ",'N/A'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET())," + UserID + ")";
                                    db.Database.ExecuteSqlCommand(cadenam);

                                }
                                catch (Exception err)
                                {

                                }
                            }
                            else
                            {

                                try
                                {
                                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Pedido no autorizado'," + details.Cantidad + ",'Pedido '," + details.IDPedido + ",''," + details.IDAlmacen + ",'N/A'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET())," + UserID + ")";
                                    db.Database.ExecuteSqlCommand(cadenam);

                                }
                                catch (Exception err)
                                {

                                }
                            }
                               

                            }


                        }
                        else
                        {
                            if (articulodetalle.CtrlStock)
                            {
                            if (details.Status == "Activo")
                            {
                                db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                        + details.IDAlmacen + "," + articulodetalle.IDArticulo + "," + carateristica.ID + ",0,0,0,0)");
                                
                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Pedido'," + details.Cantidad + ",'Pedido '," + details.IDPedido + ",''," + details.IDAlmacen + ",'N/A',0,'',CONVERT (time, SYSDATETIMEOFFSET())," + UserID + ")";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            else
                            {
                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Pedido no autorizado'," + details.Cantidad + ",'Pedido '," + details.IDPedido + ",''," + details.IDAlmacen + ",'N/A',0,'',CONVERT (time, SYSDATETIMEOFFSET())," + UserID + ")";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            }
                           

                        }
                 



                }








                db.Database.ExecuteSqlCommand("update EncPedido set [Status]='Cancelado' where IDPedido='" + id + "'");
                db.Database.ExecuteSqlCommand("update DetPedido set [Status]='Cancelado'  where IDPedido='" + id + "'");
                db.Database.ExecuteSqlCommand("delete detsolicitud   where numero=" + id + "  and documento='Pedido'");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
            return RedirectToAction("Index");
        }
        //public ActionResult Cancelar(int? id)
        //{
        //    try
        //    {
        //        List<OrdenProduccion> op = new OrdenProduccionContext().OrdenesProduccion.Where(s => s.IDPedido == id && s.EstadoOrden != "Cancelada").ToList();

        //        if (op.Count > 0)
        //        {
        //            throw new Exception("Hay ordenes de producción activas, por lo que no es posible cancelar el pedido");
        //        }


        //        List<DetSolicitud> listasolicitudes = new ArticuloContext().Database.SqlQuery<DetSolicitud>("select * from detsolicitud where Documento='Pedido' and numero=" + id + " and status='Requerido' ").ToList();
        //        if (listasolicitudes.Count > 0)
        //        {
        //            throw new Exception("Hay requisiciones de compra activas, por lo que no es posible cancelar el pedido");
        //        }


        //        string fecha = DateTime.Now.ToString("yyyyMMdd");

        //        EncPedido encPedido = db.EncPedidos.Find(id);
        //        List<VDetPedido> pedido = db.Database.SqlQuery<VDetPedido>("select DetPedido.Status,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetPedido.IDDetExterna,DetPedido.IDPedido,Articulo.Descripcion as Articulo,DetPedido.Cantidad,DetPedido.Costo,DetPedido.CantidadPedida,DetPedido.Descuento,DetPedido.Importe,DetPedido.IVA,DetPedido.ImporteIva,DetPedido.ImporteTotal, DetPedido.Nota,DetPedido.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetPedido INNER JOIN Caracteristica ON DetPedido.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPedido='" + id + "' and Status='Activo'").ToList();

        //        ViewBag.ordenes = pedido;
        //        for (int i = 0; i < pedido.Count(); i++)
        //        {
        //            db.Database.ExecuteSqlCommand("exec dbo.CancelaMovArt'" + fecha + "'," + ViewBag.ordenes[i].Caracteristica_ID + ",'CanPedVta'," + ViewBag.ordenes[i].Cantidad + ",'Pedido'," + id + ",0,'" + encPedido.IDAlmacen + "','" + ViewBag.ordenes[i].Nota + "',1");
        //        }

        //        db.Database.ExecuteSqlCommand("update EncPedido set [Status]='Cancelado' where IDPedido='" + id + "'  and Status='Activo'");
        //        db.Database.ExecuteSqlCommand("update DetPedido set [Status]='Cancelado'  where IDPedido='" + id + "'  and Status='Activo'");
        //        db.Database.ExecuteSqlCommand("delete detsolicitud   where numero=" + id + "  and documento='Pedido'");
        //    }
        //    catch (Exception err)
        //    {
        //        return Content(err.Message);
        //    }
        //    return RedirectToAction("Index");
        //}
        //public ActionResult Cancelar(int? id)
        //       {
        //try
        //           {
        //               List<OrdenProduccion> op = new OrdenProduccionContext().OrdenesProduccion.Where(s => s.IDPedido == id && s.EstadoOrden != "Cancelada").ToList();

        //               if (op.Count>0)
        //               {
        //                   throw new Exception("Hay ordenes de producción activas, por lo que no es posible cancelar el pedido");
        //               }


        //               List<DetSolicitud> listasolicitudes = new ArticuloContext().Database.SqlQuery<DetSolicitud>("select * from detsolicitud where Documento='Pedido' and numero=" + id + " and status='Requerido' ").ToList();
        //               if (listasolicitudes.Count>0)
        //               {
        //                   throw new Exception("Hay requisiciones de compra activas, por lo que no es posible cancelar el pedido");
        //               }


        //               string fecha = DateTime.Now.ToString("yyyyMMdd");

        //               EncPedido encPedido = db.EncPedidos.Find(id);
        //               List<VDetPedido> pedido = db.Database.SqlQuery<VDetPedido>("select DetPedido.Status,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetPedido.IDDetExterna,DetPedido.IDPedido,Articulo.Descripcion as Articulo,DetPedido.Cantidad,DetPedido.Costo,DetPedido.CantidadPedida,DetPedido.Descuento,DetPedido.Importe,DetPedido.IVA,DetPedido.ImporteIva,DetPedido.ImporteTotal, DetPedido.Nota,DetPedido.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetPedido INNER JOIN Caracteristica ON DetPedido.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPedido='" + id + "' and Status='Activo'").ToList();

        //               ViewBag.ordenes = pedido;
        //               for (int i = 0; i < pedido.Count(); i++)
        //               {
        //                   db.Database.ExecuteSqlCommand("exec dbo.CancelaMovArt'" + fecha + "'," + ViewBag.ordenes[i].Caracteristica_ID + ",'CanPedVta'," + ViewBag.ordenes[i].Cantidad + ",'Pedido'," + id + ",0,'" + encPedido.IDAlmacen + "','" + ViewBag.ordenes[i].Nota + "',1");
        //               }

        //               db.Database.ExecuteSqlCommand("update EncPedido set [Status]='Cancelado' where IDPedido='" + id + "'  and Status='Activo'");
        //               db.Database.ExecuteSqlCommand("update DetPedido set [Status]='Cancelado'  where IDPedido='" + id + "'  and Status='Activo'");
        //               db.Database.ExecuteSqlCommand("delete detsolicitud   where numero=" + id + "  and documento='Pedido'");
        //           }
        //           catch(Exception err)
        //           {
        //               return Content(err.Message);
        //           }
        //           return RedirectToAction("Index");
        //       }

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

        public FormulaEspecializada.Formulaespecializada CosteaTintas(FormulaEspecializada.Formulaespecializada formula)
        {
            decimal costoacu = 0;
            // formula.Tintas.Clear();
            List<FormulaEspecializada.Tinta> listanueva = new List<FormulaEspecializada.Tinta>();

            foreach (FormulaEspecializada.Tinta tin in formula.Tintas)
            {
                FormulaEspecializada.Tinta nueva = new FormulaEspecializada.Tinta();
                nueva.IDTinta = tin.IDTinta;
                nueva.Area = tin.Area;
                int metroscuadrados = SIAAPI.Properties.Settings.Default.ValorMtsXkg;
                if (tin.Area == 0)
                {
                    tin.Area = 0.01M;
                }
                decimal senecesita = (formula.CantidadMPMts2 * (tin.Area / 100)) / decimal.Parse(metroscuadrados.ToString());
                tin.kg = senecesita;

                nueva.kg = tin.kg;


                decimal costo = new ArticuloContext().Database.SqlQuery<decimal>("select dbo.GetCosto(0," + nueva.IDTinta + "," + nueva.kg + ")").ToList().FirstOrDefault();

                nueva.Costo = Math.Round(tin.kg * costo, 2);
                costoacu += nueva.Costo;
                listanueva.Add(nueva);
            }
            formula.Costodetintas = costoacu;
            formula.Tintas = listanueva;
            return formula;
        }

        public ActionResult Create()
        {

           ViewBag.idalma = 2;
           
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<Carrito> carritoalmacen = new CarritoContext().Database.SqlQuery<Carrito>("Select *from Carrito where usuario=" + usuario + " and IDAlmacen=0 ").ToList();

            if (carritoalmacen.Count >= 1)
            {
                string Mensajedeerror = "No Puedes crear un pedido sin haber seleccionado un almacén";
                return RedirectToAction("Index", "Carrito", new { Mensaje = Mensajedeerror });
            }
            List<Carrito> carritoPorcentajes = new CarritoContext().Database.SqlQuery<Carrito>("Select *from Carrito where usuario=" + usuario + "").ToList();

            //if (carritoPorcentajes.Count >= 1)
            //{
            //    string Mensajedeerror = "";
            //    foreach (Carrito item in carritoPorcentajes)
            //    {
            //        decimal porcentaje = 0M;
            //        decimal costoCotizacion=0M;
            //        Caracteristica caracteristica = new CarritoContext().Database.SqlQuery<Caracteristica>("select*from Caracteristica where id= "+ item.IDCaracteristica).ToList().FirstOrDefault();
            //        Articulo articulo = new ArticuloContext().Articulo.Find(caracteristica.Articulo_IDArticulo);
                  

            //        ClsCotizador elemento;
            //        try
            //        {
            //            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(caracteristica.IDCotizacion);
            //            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            //            try
            //            {
            //                XmlDocument documento = new XmlDocument();
            //                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
            //                documento.Load(nombredearchivo);


            //                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            //                {
            //                    // Call the Deserialize method to restore the object's state.
            //                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
            //                }
            //            }
            //            catch (Exception er)
            //            {
            //                string mensajedeerror = er.Message;
            //                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            //            }

            //            try
            //            {
            //                FormulaEspecializada.Formulaespecializada Formulapara2 = new FormulaEspecializada.Formulaespecializada();
            //                Formulapara2 = igualar(elemento, Formulapara2);
            //                Formulapara2.Cantidad = elemento.Rango1;
            //                Formulapara2 = pasaTintas(elemento.Tintas, Formulapara2);
            //                Formulapara2.CobrarMaster = elemento.CobrarMaster;
            //                Formulapara2.anchommmaster = elemento.anchommmaster;
            //                if (elemento.IDMaterial2 != 0)
            //                {
            //                    try
            //                    {
            //                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
            //                        if (mat2.Completo)
            //                        {
            //                            Formulapara2.CobrarMaster2m = true;
            //                            Formulapara2.anchomaster2m = mat2.Ancho;
            //                            Formulapara2.largomaster2m = mat2.Largo;


            //                        }
            //                    }
            //                    catch (Exception err)
            //                    {
            //                        string mensajeerror = err.Message;
            //                    }
            //                }

            //                Formulapara2.Calcular();
            //                Formulapara2 = CosteaTintas(Formulapara2);
            //                Formulapara2.calcularCostoMO();

            //                decimal costototal = Formulapara2.CostototalMP + Formulapara2.Costodetintas + Formulapara2.CostototalMO;//+ (costosuaje) + SIAAPI.Properties.Settings.Default.costodeempaque;

            //                decimal costoxmillar = costototal / Formulapara2.Cantidad;

            //                costoCotizacion= costoxmillar;
            //            }
            //            catch (Exception err)
            //            {
            //                decimal costo = new ArticuloContext().Database.SqlQuery<decimal>("select dbo.GetCosto(0," + articulo.IDArticulo + "," + item.Cantidad + ")").ToList().FirstOrDefault();

            //                costoCotizacion = costo;
            //            }
            //            try
            //            {
            //                string cadena = "select*from NivelesGanancia where IDFamilia=" +articulo.IDFamilia ;
            //                List<NivelesGanancia> ganancias = new NivelesGananciaContext().Database.SqlQuery<NivelesGanancia>(cadena).ToList();

            //                foreach (NivelesGanancia nivelesGanancia in ganancias)
            //                {
            //                    if (nivelesGanancia.RangInf <= item.Cantidad && nivelesGanancia.RangSup >= item.Cantidad)
            //                    {
            //                        porcentaje = nivelesGanancia.Porcentaje;
            //                    }
            //                }
            //            }
            //            catch (Exception err)
            //            {

            //            }

            //            costoCotizacion= Math.Round((costoCotizacion * (1 / ((100 - porcentaje) / 100))), 2);

            //        }
            //        catch (Exception err)
            //        {

            //        }


            //        decimal precioventa = item.Precio;
            //        if (precioventa<costoCotizacion)
            //        {
            //            Mensajedeerror += Mensajedeerror +"Tu precio de venta está por debajo de lo permitido del artículo "+ articulo.Cref + " de la presentación "+ caracteristica.IDPresentacion + "\n";
            //        }
                   
            //    }

            //    if (Mensajedeerror != "")
            //    {
            //        return RedirectToAction("Index", "Carrito", new { Mensaje = Mensajedeerror });
            //    }
              
            //}
          
            
            
            List<int> carrito = new CarritoContext().Database.SqlQuery<int>("Select IDMoneda from Carrito where usuario="+ usuario +" group by IDMoneda" ).ToList();
            if (carrito.Count>1 )
            {
                 string Mensajedeerror = "No Puedes crear un pedido de dos monedas diferentes";
                return RedirectToAction("Index", "Carrito", new { Mensaje = Mensajedeerror });
            }



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


            List<SelectListItem> entrega = new List<SelectListItem>();
            List<VEntrega> entregasall = db.Database.SqlQuery<VEntrega>("select en.IDEntrega,en.IDCliente,en.CalleEntrega,en.NumExtEntrega,ISNULL(en.NumIntEntrega,0),en.ColoniaEntrega,en.MunicipioEntrega,en.CPEntrega,en.ObservacionEntrega,es.Estado from dbo.Entrega as en inner join Estados as es on es.IDEstado=en.IDEstado where IDCliente='" + cambio.Dato + "'").ToList();

            entrega.Add(new SelectListItem { Text = clientes.Calle + " " + clientes.NumExt + " " + clientes.NumInt + " " + clientes.Colonia, Value = clientes.Calle + " " + clientes.NumExt + " " + clientes.NumInt + " " + clientes.Colonia + " " + clientes.Municipio });
            entrega.Add(new SelectListItem { Text = "El Cliente Recoge", Value = "El Cliente Recoge" });   
            entrega.Add(new SelectListItem { Text = "Enviar por paqueteria", Value = "Enviar por paqueteria" });
            entrega.Add(new SelectListItem { Text = "Ocurre: " +  clientes.Municipio , Value = "Ocurre: " + clientes.Municipio });

            foreach( VEntrega entregax in entregasall)
            {
                entrega.Add(new SelectListItem { Text = "Calle: " + entregax.CalleEntrega + " " + "No. Exterior: " + entregax.NumExtEntrega + " " + "No. Interior: " + entregax.NumIntentrega + " " + "Colonia: " + entregax.ColoniaEntrega + " " + "Municipio: " + entregax.MunicipioEntrega + " " + "C.P.: " + entregax.CPEntrega + " " + "Estado: " + entregax.Estado + " ", Value = "Calle: " + entregax.CalleEntrega + " " + "No. Exterior: " + entregax.NumExtEntrega + " " + "No. Interior: " + entregax.NumIntentrega + " " + "Colonia: " + entregax.ColoniaEntrega + " " + "Municipio: " + entregax.MunicipioEntrega + " " + "C.P.: " + entregax.CPEntrega + " " + "Estado: " + entregax.Estado + " " });
                entrega.Add(new SelectListItem { Text = "Transportista Calle: " + entregax.CalleEntrega + " " + "No. Exterior: " + entregax.NumExtEntrega + " " + "No. Interior: " + entregax.NumIntentrega + " " + "Colonia: " + entregax.ColoniaEntrega + "Municipio: " + entregax.MunicipioEntrega + " " + " " + "C.P.: " + entregax.CPEntrega + " " + "Estado: " + entregax.Estado + " ", Value = "Transportista Calle: " + entregax.CalleEntrega + " " + "No. Exterior: " + entregax.NumExtEntrega + " " + "No. Interior: " + entregax.NumIntentrega + " " + "Colonia: " + entregax.ColoniaEntrega + " " + "Municipio: " + entregax.MunicipioEntrega + " " + "C.P.: " + entregax.CPEntrega + " " + "Estado: " + entregax.Estado + " " });
                entrega.Add(new SelectListItem { Text = "Mensajeria Calle: " + entregax.CalleEntrega + " " + "No. Exterior: " + entregax.NumExtEntrega + " " + "No. Interior: " + entregax.NumIntentrega + " " + "Colonia: " + entregax.ColoniaEntrega + " " + "Municipio: " + entregax.MunicipioEntrega + " " + "C.P.: " + entregax.CPEntrega + " " + "Estado: " + entregax.Estado + " ", Value = "Mensajeria Calle: " + entregax.CalleEntrega + " " + "No. Exterior: " + entregax.NumExtEntrega + " " + "No. Interior: " + entregax.NumIntentrega + " " + "Colonia: " + entregax.ColoniaEntrega + " " + "Municipio: " + entregax.MunicipioEntrega + " " + "C.P.: " + entregax.CPEntrega + " " + "Estado: " + entregax.Estado + " " });
                entrega.Add(new SelectListItem { Text = "Ocurre Colonia: " + entregax.ColoniaEntrega + " " + "Municipio: " + entregax.MunicipioEntrega + " " + "C.P.: " + entregax.CPEntrega + " " + "Estado: " + entregax.Estado + " ", Value = "Ocurre Colonia: " + entregax.ColoniaEntrega + " " + "Municipio: " + entregax.MunicipioEntrega + " " + "C.P.: " + entregax.CPEntrega + " " + "Estado: " + entregax.Estado + " " });
            }


            ViewBag.entrega = entrega;


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

          

           List<VCarrito> pedido = db.Database.SqlQuery<VCarrito>("select (Carrito.Precio * (select dbo.GetTipocambio(GETDATE(),Carrito.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * Carrito.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota, Carrito.IDAlmacen, Articulo.Cref from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=Carrito.IDMoneda) as MonedaOrigen, (select SUM(Carrito.Precio * Carrito.Cantidad)) as Subtotal, SUM(Carrito.Precio * Carrito.Cantidad)*0.16 as IVA, (SUM(Carrito.Precio * Carrito.Cantidad)) + (SUM(Carrito.Precio * Carrito.Cantidad)*0.16) as Total ,0 as TotalPesos from Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario=" + usuario + " group by Carrito.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;
            //try
            //{
            //    subtotal = pedido.Sum(s => s.Importe);
            //    iva = subtotal * (decimal)0.16;
            //    total = subtotal + iva;
            //}
            //catch (Exception e)
            //{

            //}
            //ViewBag.Subtotal = subtotal.ToString("C");
            //ViewBag.IVA = iva.ToString("C");
            //ViewBag.Total = total.ToString("C");

            ViewBag.carrito = pedido;


            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarrito) as Dato from Carrito where  usuario='" + usuario + "'").ToList()[0];
            int x = c.Dato;
            ViewBag.dato = x;

            ClsDatoEntero cantidad = db.Database.SqlQuery<ClsDatoEntero>("select count(Cantidad) as Dato from Carrito where Cantidad=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datocantidad = cantidad.Dato;

            ClsDatoEntero preciocontar = db.Database.SqlQuery<ClsDatoEntero>("select count(Precio) as Dato from Carrito where Precio=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datoprecio = preciocontar.Dato;


            List<ValidarCarrito> validaprecio = db.Database.SqlQuery<ValidarCarrito>("select Carrito.Precio, dbo.GetValidaCosto(Articulo.IDArticulo, Carrito.Cantidad, Carrito.Precio) as Dato from Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo  where Carrito.usuario='" + usuario + "'").ToList();
            int countDato = validaprecio.Count(s => s.Dato.Equals(true));

            int countCarrito = pedido.Count();

            if (countDato == countCarrito)
            {
                ViewBag.validacarrito = 1;
            }
            else
            {
                ViewBag.validacarrito = 0;
            }

            Vendedor vendedorO = new VendedorContext().Vendedores.Find(clientes.IDVendedor);
            if (!vendedorO.Activo)
            {
                ViewBag.validaVendedor = 0;

            }
            //Termina la consulta del carrito
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       public ActionResult Create(EncPedido encPedido)
        {
               
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0, Precio = 0;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
             List<VCarrito> pedido = db.Database.SqlQuery<VCarrito>("select (Carrito.Precio * (select dbo.GetTipocambio(GETDATE(),Carrito.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * Carrito.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota, Carrito.IDAlmacen from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            
            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=Carrito.IDMoneda) as MonedaOrigen, (select SUM(Carrito.Precio * Carrito.Cantidad)) as Subtotal, SUM(Carrito.Precio * Carrito.Cantidad)*0.16 as IVA, (SUM(Carrito.Precio * Carrito.Cantidad)) + (SUM(Carrito.Precio * Carrito.Cantidad)*0.16) as Total ,0 as TotalPesos from Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario=" + usuario + " group by Carrito.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;
            var carritoP = db.Database.SqlQuery<Carrito>("select *from Carrito where usuario=" + usuario).ToList();
            int IDCliente = 0;
            foreach (var c in carritoP)
            {
                IDCliente = c.IDCLiente;
            }
            //encPedido.Fecha = DateTime.Now;

            ViewBag.carrito = pedido;
            //Termina 

            Clientes cliente = new ClientesContext().Clientes.Find(IDCliente);

            if (ModelState.IsValid)
            {
                int num = 0;
                DateTime fecha = DateTime.Now;
                string fecha1 = fecha.ToString("yyyy/MM/dd");

                DateTime fechareq = encPedido.FechaRequiere;
                string fecha2 = fechareq.ToString("yyyy/MM/dd");

                List<c_Moneda> monedaorigen;
                monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();




                VCambio tcambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + encPedido.IDMoneda + "," + origen + ") as TC").ToList()[0];
                decimal tCambio = tcambio.TC;

               db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncPedido]([Fecha],[FechaRequiere],[IDCliente],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[Status],[TipoCambio],[UserID],[IDUsoCFDI],[IDVendedor],[Entrega], [OCompra]) values ('" + fecha1 + "','" + fecha2 + "','" + encPedido.IDCliente + "','" + encPedido.IDFormapago + "','" + encPedido.IDMoneda + "','" + encPedido.Observacion + "','" + subtotal + "','" + iva + "','" + total + "','" + encPedido.IDMetodoPago + "','" + encPedido.IDCondicionesPago + "','Inactivo','" + tCambio + "','" + usuario + "','" + encPedido.IDUsoCFDI + "','" + encPedido.IDVendedor + "','" + encPedido.Entrega + "','" + encPedido.OCompra + "')");
                    //db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncPedido]([Fecha],[FechaRequiere],[IDCliente],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[TipoCambio],[UserID],[IDUsoCFDI],[IDVendedor],[Entrega]) values ('" + fecha1 + "','" + fecha2 + "','" + encPedido.IDCliente + "','" + encPedido.IDFormapago + "','" + encPedido.IDMoneda + "','" + encPedido.Observacion + "','" + subtotal + "','" + iva + "','" + total + "','" + encPedido.IDMetodoPago + "','" + encPedido.IDCondicionesPago + "','" + encPedido.IDAlmacen + "','Inactivo','" + tCambio + "','" + usuario + "','" + encPedido.IDUsoCFDI + "','" + encPedido.IDVendedor + "','" + encPedido.Entrega + "')");
                    db.SaveChanges();


                    List<EncPedido> numero;
                    numero = db.Database.SqlQuery<EncPedido>("SELECT * FROM [dbo].[EncPedido] WHERE IDPedido = (SELECT MAX(IDPedido) from EncPedido)").ToList();
                    num = numero.Select(s => s.IDPedido).FirstOrDefault();

                    //   string cadenafinal = "";
                    for (int i = 0; i < pedido.Count(); i++)
                    {

                        Articulo articulo = new ArticuloContext().Articulo.Find(ViewBag.carrito[i].IDArticulo);
                        Precio = ViewBag.carrito[i].Precio;
                        importe = ViewBag.carrito[i].Cantidad * Precio;
                        importeiva = importe * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                        importetotal = importeiva + importe;
                        db.Database.ExecuteSqlCommand("INSERT INTO DetPedido([IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna],[GeneraOrdenP],[IDRemision],[IDPrefactura]) values ('" + num + "','" + ViewBag.carrito[i].IDArticulo + "','" + ViewBag.carrito[i].Cantidad + "','" + Precio + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encPedido.IDMoneda + "),'" + ViewBag.carrito[i].Cantidad + "','0','" + importe + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encPedido.IDMoneda + "),'true','" + importeiva + "' *  dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encPedido.IDMoneda + "),'" + importetotal + "' * dbo.GetTipocambio('" + fecha1 + "'," + ViewBag.carrito[i].IDMoneda + "," + encPedido.IDMoneda + "),'" + ViewBag.carrito[i].Nota + "','0','" + ViewBag.carrito[i].IDCaracteristica + "','" + ViewBag.carrito[i].IDAlmacen + "','0','Inactivo','" + ViewBag.carrito[i].Presentacion + "','" + ViewBag.carrito[i].jsonPresentacion + "','0','" + articulo.GeneraOrden + "','0','0')");


                   



                    db.Database.ExecuteSqlCommand("delete from Carrito where IDCarrito='" + ViewBag.carrito[i].IDCarrito + "'");
                        db.SaveChanges();


                    
                    }

				var detallepedidoa = new PedidoContext().DetPedido.Where(s => s.IDPedido == encPedido.IDPedido);

                List<DetPedido> datostotales = db.Database.SqlQuery<DetPedido>("select * from dbo.DetPedido where IDPedido='" + num + "'").ToList();
                subtotalr = datostotales.Sum(s => s.Importe);
                ivar = subtotalr * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                totalr = subtotalr + ivar;
                db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "' where [IDPedido]='" + num + "'");


              

                return RedirectToAction("Index");

            }


            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", encPedido.IDFormapago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", encPedido.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", encPedido.IDMoneda);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", encPedido.IDCondicionesPago);
            ViewBag.IDCliente = new SelectList(db.Clientess, "IDCliente", "Nombre", encPedido.IDCliente);
            ViewBag.IDAlmacen = new SelectList(db.Almacenes, "IDAlmacen", "Descripcion", encPedido.IDAlmacen);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS, "IDUsoCFDI", "Descripcion", encPedido.IDUsoCFDI);
            List<SelectListItem> vendedor = new List<SelectListItem>();
            Vendedor vendedorp = prov.Vendedores.Find(cliente.IDVendedor);
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
            return View(encPedido);

        }

        ////////////////////////////////////////////////////////////////////Pedido///////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Cotizaciones(int? id, EncPedido encPedido)
        {
            ViewBag.otro = 0;
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0, Precio = 0;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

  
            
            //Si el id es nullo y se verifica que exista un dato en el carrito para extraer al cliente de la cotización en proceso
                ClsDatoEntero w = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoCotizacion) as Dato from CarritoCotizacion where UserID='" + UserID + "'").ToList()[0];
                if (id == null && w.Dato != 0)
                {
                    ClsDatoEntero maxidreq = db.Database.SqlQuery<ClsDatoEntero>("select max(IDCarritoCotizacion) as Dato from CarritoCotizacion  where UserID='" + UserID + "'").ToList()[0];
                    ClsDatoEntero idreq = db.Database.SqlQuery<ClsDatoEntero>("select IDCotizacion as Dato from CarritoCotizacion where UserID='" + UserID + "' and IDCarritoCotizacion='" + maxidreq.Dato + "'").ToList()[0];

                    CotizacionContext bd = new CotizacionContext();
                    ClientesContext pr = new ClientesContext();
                    EncCotizacion encCotizacion = bd.EncCotizaciones.Find(idreq.Dato);


               List<SelectListItem> usocfdi = new List<SelectListItem>();
                c_UsoCFDI usocfdib = prov.c_UsoCFDIS.Find(encCotizacion.IDUsoCFDI);
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

                ViewBag.IDAlmacen = new SelectList(db.Almacenes.Where(s => s.IDAlmacen.Equals(encCotizacion.IDAlmacen)), "IDAlmacen", "Descripcion");

                    var cliente = prov.Clientes.ToList();

                    List<SelectListItem> moneda = new List<SelectListItem>();
                    c_Moneda monedap = pr.c_Monedas.Find(encCotizacion.IDMoneda);
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
                    c_MetodoPago metodop = pr.c_MetodoPagos.Find(encCotizacion.IDMetodoPago);
                    metodo.Add(new SelectListItem { Text = metodop.Descripcion, Value = metodop.IDMetodoPago.ToString() });
                    ViewBag.metodo = metodo;

                    List<SelectListItem> forma = new List<SelectListItem>();
                    c_FormaPago formap = pr.c_FormaPagos.Find(encCotizacion.IDFormapago);
                    forma.Add(new SelectListItem { Text = formap.Descripcion, Value = formap.IDFormaPago.ToString() });
                    ViewBag.forma = forma;

                    List<SelectListItem> condiciones = new List<SelectListItem>();
                    CondicionesPago condicionesp = pr.CondicionesPagos.Find(encCotizacion.IDCondicionesPago);
                    condiciones.Add(new SelectListItem { Text = condicionesp.Descripcion, Value = condicionesp.IDCondicionesPago.ToString() });
                    ViewBag.condiciones = condiciones;

                    List<SelectListItem> vendedor = new List<SelectListItem>();
                    Vendedor vendedorp = pr.Vendedores.Find(encCotizacion.IDVendedor);
                    vendedor.Add(new SelectListItem { Text = vendedorp.Nombre, Value = vendedorp.IDVendedor.ToString() });
                    ViewBag.vendedor = vendedor;

                    List<SelectListItem> li = new List<SelectListItem>();
                    Clientes mm = pr.Clientes.Find(encCotizacion.IDCliente);
                    li.Add(new SelectListItem { Text = mm.Nombre, Value = mm.IDCliente.ToString() });
                    ViewBag.cliente = li;

                    List<SelectListItem> entrega = new List<SelectListItem>();
                    List<VEntrega> entregasall = db.Database.SqlQuery<VEntrega>("select en.IDEntrega,en.IDCliente,en.CalleEntrega,en.NumExtEntrega,ISNULL(en.NumIntEntrega,0),en.ColoniaEntrega,en.MunicipioEntrega,en.CPEntrega,en.ObservacionEntrega,es.Estado from dbo.Entrega as en inner join Estados as es on es.IDEstado=en.IDEstado where IDCliente='" + encCotizacion.IDCliente + "'").ToList();
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
                    ViewBag.otro = 0;
                }

                //Se verifica que el id sea valido 
            if (id != null)
            {
                int? idaux = id;
                ClsDatoEntero existe = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCotizacion) as Dato from EncCotizacion where IDCotizacion= '" + id + "' and UserID='" + UserID + "'").ToList()[0];
                if (existe.Dato == 0)
                {
                    ViewBag.mensaje = "Cotización inexistente";
                    ClsDatoEntero countcc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoCotizacion) as Dato from CarritoCotizacion where UserID='" + UserID + "'").ToList()[0];
                    if (countcc.Dato != 0) {
                        ClsDatoEntero idcotizacion = db.Database.SqlQuery<ClsDatoEntero>("select EncCotizacion.IDCotizacion as Dato from EncCotizacion inner join CarritoCotizacion  on EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion where EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion").ToList()[0];
                        idaux = idcotizacion.Dato;
                        ViewBag.otro = 0;
                    }
                    else
                    {
                        ViewBag.data = null;

                        ViewBag.otro = 0;

                        ViewBag.datos = null;
                        ViewBag.dato = 0;
                    }
                    return View();
                }
                else
                {
                    ClsDatoEntero countcc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoCotizacion) as Dato from CarritoCotizacion where UserID='" + UserID + "'").ToList()[0];
                    if (countcc.Dato != 0)
                    {
                        ClsDatoEntero clientecotizacion = db.Database.SqlQuery<ClsDatoEntero>("select EncCotizacion.IDCliente as Dato from EncCotizacion inner join CarritoCotizacion  on EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion where EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion").ToList()[0];
                        CotizacionContext cotizacion = new CotizacionContext();
                        EncCotizacion encCotizacion2 = cotizacion.EncCotizaciones.Find(id);
                        if (clientecotizacion.Dato != encCotizacion2.IDCliente)
                        {
                            ClsDatoEntero idcotizacion = db.Database.SqlQuery<ClsDatoEntero>("select EncCotizacion.IDCotizacion as Dato from EncCotizacion inner join CarritoCotizacion  on EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion where EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion").ToList()[0];
                            idaux = idcotizacion.Dato;
                        }
                        
                    }
                }
                CotizacionContext bd = new CotizacionContext();
                    ClientesContext pr = new ClientesContext();
                    EncCotizacion encCotizacion = bd.EncCotizaciones.Find(idaux);

                    ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.Where(s => s.IDUsoCFDI.Equals(encCotizacion.IDUsoCFDI)), "IDUsoCFDI", "Descripcion");
                    ViewBag.IDAlmacen = new SelectList(db.Almacenes.Where(s => s.IDAlmacen.Equals(encCotizacion.IDAlmacen)), "IDAlmacen", "Descripcion");

                    var cliente = prov.Clientes.ToList();

                    List<SelectListItem> moneda = new List<SelectListItem>();
                    c_Moneda monedap = pr.c_Monedas.Find(encCotizacion.IDMoneda);
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
                    c_MetodoPago metodop = pr.c_MetodoPagos.Find(encCotizacion.IDMetodoPago);
                    metodo.Add(new SelectListItem { Text = metodop.Descripcion, Value = metodop.IDMetodoPago.ToString() });
                    ViewBag.metodo = metodo;

                    List<SelectListItem> forma = new List<SelectListItem>();
                    c_FormaPago formap = pr.c_FormaPagos.Find(encCotizacion.IDFormapago);
                    forma.Add(new SelectListItem { Text = formap.Descripcion, Value = formap.IDFormaPago.ToString() });
                    ViewBag.forma = forma;

                    List<SelectListItem> condiciones = new List<SelectListItem>();
                    CondicionesPago condicionesp = pr.CondicionesPagos.Find(encCotizacion.IDCondicionesPago);
                    condiciones.Add(new SelectListItem { Text = condicionesp.Descripcion, Value = condicionesp.IDCondicionesPago.ToString() });
                    ViewBag.condiciones = condiciones;

                    List<SelectListItem> vendedor = new List<SelectListItem>();
                    Vendedor vendedorp = pr.Vendedores.Find(encCotizacion.IDVendedor);
                    vendedor.Add(new SelectListItem { Text = vendedorp.Nombre, Value = vendedorp.IDVendedor.ToString() });
                    ViewBag.vendedor = vendedor;

                    List<SelectListItem> li = new List<SelectListItem>();
                    Clientes mm = pr.Clientes.Find(encCotizacion.IDCliente);
                    li.Add(new SelectListItem { Text = mm.Nombre, Value = mm.IDCliente.ToString() });
                    ViewBag.cliente = li;

                    List<SelectListItem> entrega = new List<SelectListItem>();
                    List<VEntrega> entregasall = db.Database.SqlQuery<VEntrega>("select en.IDEntrega,en.IDCliente,en.CalleEntrega,en.NumExtEntrega,ISNULL(en.NumIntEntrega,0),en.ColoniaEntrega,en.MunicipioEntrega,en.CPEntrega,en.ObservacionEntrega,es.Estado from dbo.Entrega as en inner join Estados as es on es.IDEstado=en.IDEstado where IDCliente='" + encCotizacion.IDCliente + "'").ToList();
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
                }
            
                List<VEncCotizacion> orden = db.Database.SqlQuery<VEncCotizacion>("select EncCotizacion.IDCotizacion,EncCotizacion.Fecha, EncCotizacion.FechaRequiere, Clientes.Nombre as Cliente, c_FormaPago.Descripcion as FormaPago, c_Moneda.Descripcion as Divisa, EncCotizacion.Observacion, EncCotizacion.Subtotal, EncCotizacion.IVA, EncCotizacion.Total,c_MetodoPago.Descripcion as MetodoPago, CondicionesPago.Descripcion as CondicionesPago, Almacen.Descripcion as Almacen, EncCotizacion.Status,EncCotizacion.UserID, EncCotizacion.TipoCambio, c_UsoCFDI.Descripcion as UsoCFDI from EncCotizacion inner join  Clientes on Clientes.IDCliente=EncCotizacion.IDCliente inner join c_FormaPago on c_FormaPago.IDFormaPago=EncCotizacion.IDFormapago inner join c_Moneda on c_Moneda.IDMoneda=EncCotizacion.IDMoneda inner join c_MetodoPago on c_MetodoPago.IDMetodoPago=EncCotizacion.IDMetodoPago inner join CondicionesPago on CondicionesPago.IDCondicionesPago=EncCotizacion.IDCondicionesPago inner join Almacen on Almacen.IDAlmacen=EncCotizacion.IDAlmacen inner join c_UsoCFDI on c_UsoCFDI.IDUsoCFDI=EncCotizacion.IDUsoCFDI where EncCotizacion.IDCotizacion='" + id + "' and EncCotizacion.Status='Activo'").ToList();
                ViewBag.data = orden;

                ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCotizacion) as Dato from EncCotizacion where IDCotizacion='" + id + "' and Status='Activo'").ToList()[0];

                List<VDetCotizacion> elementos = db.Database.SqlQuery<VDetCotizacion>("select Articulo.MinimoCompra,DetCotizacion.IDDetCotizacion, DetCotizacion.IDCotizacion,DetCotizacion.Suministro,Articulo.Descripcion as Articulo,DetCotizacion.Cantidad,DetCotizacion.Costo,DetCotizacion.CantidadPedida,DetCotizacion.Descuento,DetCotizacion.Importe,DetCotizacion.IVA,DetCotizacion.ImporteIva,DetCotizacion.ImporteTotal,DetCotizacion.Nota,DetCotizacion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetCotizacion.Status from  DetCotizacion INNER JOIN Caracteristica ON DetCotizacion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where DetCotizacion.IDCotizacion='" + id + "' and Status='Activo'").ToList();
                ViewBag.datos = elementos;

                ClsDatoEntero dcompra = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCotizacion) as Dato from DetCotizacion where IDCotizacion='" + id + "' and Status='Activo'").ToList()[0];


                if (id != null && denc.Dato > 0 && dcompra.Dato > 0)
                {

                    ClsDatoEntero cantcarrito = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoCotizacion) as Dato from CarritoCotizacion where UserID='" + UserID + "'").ToList()[0];
                    if (cantcarrito.Dato != 0)
                    {
                        ClsDatoEntero clientecotizacion = db.Database.SqlQuery<ClsDatoEntero>("select EncCotizacion.IDCliente as Dato from EncCotizacion inner join CarritoCotizacion  on EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion where EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion").ToList()[0];
                        CotizacionContext cotizacion = new CotizacionContext();
                        EncCotizacion encCotizacion = cotizacion.EncCotizaciones.Find(id);
                        if (clientecotizacion.Dato == encCotizacion.IDCliente)
                        {

                        List<VDetCotizacion> datoscotizacion = db.Database.SqlQuery<VDetCotizacion>("select DetCotizacion.IDDetCotizacion,DetCotizacion.Status,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetCotizacion.IDCotizacion,Articulo.Descripcion as Articulo,DetCotizacion.Cantidad,DetCotizacion.Costo,DetCotizacion.CantidadPedida,DetCotizacion.Descuento,DetCotizacion.Importe,DetCotizacion.IVA,DetCotizacion.ImporteIva,DetCotizacion.ImporteTotal, DetCotizacion.Nota,DetCotizacion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetCotizacion INNER JOIN Caracteristica ON DetCotizacion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDCotizacion='" + id + "'").ToList();
                        ViewBag.datoscotizacion = datoscotizacion;
                        for (int i = 0; i < datoscotizacion.Count(); i++)
                        {
                            DetCotizacion detCotizacion = cotizacion.DetCotizaciones.Find(ViewBag.datoscotizacion[i].IDDetCotizacion);
                            ClsDatoEntero caractpres = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoCotizacion) as Dato from CarritoCotizacion where IDArticulo ='" + detCotizacion.IDArticulo + "' and Caracteristica_ID='" + detCotizacion.Caracteristica_ID + "' ").ToList()[0];
                            if (caractpres.Dato != 0)
                            {
                                ViewBag.mensaje = "No se puede repetir una presentación";
                            }
                            else
                            {
                                db.Database.ExecuteSqlCommand("INSERT INTO CarritoCotizacion([IDDetExterna],[IDCotizacion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion]) values ('" + ViewBag.datoscotizacion[i].IDDetCotizacion + "','" + id + "','" + detCotizacion.IDArticulo + "','" + ViewBag.datoscotizacion[i].Cantidad + "','" + ViewBag.datoscotizacion[i].Costo + "','" + ViewBag.datoscotizacion[i].Cantidad + "','0','" + ViewBag.datoscotizacion[i].Importe + "','true','" + ViewBag.datoscotizacion[i].ImporteIva + "','" + ViewBag.datoscotizacion[i].ImporteTotal + "','" + ViewBag.datoscotizacion[i].Nota + "','0','" + detCotizacion.Caracteristica_ID + "','" + encCotizacion.IDAlmacen + "','0','Activo','" + ViewBag.datoscotizacion[i].Presentacion + "','" + ViewBag.datoscotizacion[i].jsonPresentacion + "')");
                                db.Database.ExecuteSqlCommand("update [dbo].[DetCotizacion] set [Status]='Finalizado' where [IDDetCotizacion]='" + ViewBag.datoscotizacion[i].IDDetCotizacion + "'");
                                db.Database.ExecuteSqlCommand("update [dbo].[CarritoCotizacion] set [UserID]='" + UserID + "' where [IDCotizacion]='" + id + "'");
                            }
                  
                        }
                       
                      

                        ClsDatoEntero a = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCotizacion) as Dato from DetCotizacion where IDCotizacion ='" + id + "'").ToList()[0];
                        int x = a.Dato;

                        ClsDatoEntero b = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCotizacion) as Dato from DetCotizacion where IDCotizacion ='" + id+ "' and Status='Finalizado'").ToList()[0];
                        int y = b.Dato;

                        if (x == y)
                        {
                            db.Database.ExecuteSqlCommand("update [dbo].[EncCotizacion] set [Status]='Finalizado' where [IDCotizacion]='" + id + "'");
                        }
                       
                    }
                        else
                        {
                            ViewBag.otro = 0;
                            ViewBag.mensaje = "La cotización que se desea agregar proviene de un cliente distinto, por lo tanto, no se puede continuar con la operación";
                        }
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO CarritoCotizacion([IDDetExterna],[IDCotizacion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion]) SELECT [IDDetCotizacion],[IDCotizacion],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion] FROM DetCotizacion where IDCotizacion='" + id + "' and Status='Activo'");
                        db.Database.ExecuteSqlCommand("update [dbo].[CarritoCotizacion] set [UserID]='" + UserID + "' where [IDCotizacion]='" + id + "'");
                        db.Database.ExecuteSqlCommand("update [dbo].[EncCotizacion] set [Status]='Finalizado' where [IDCotizacion]='" + id + "'");
                        db.Database.ExecuteSqlCommand("update [dbo].[DetCotizacion] set [Status]='Finalizado' where [IDCotizacion]='" + id + "'");
                    }


                }

                List<VCarritoCotizacion> lista = db.Database.SqlQuery<VCarritoCotizacion>("select Articulo.MinimoCompra,CarritoCotizacion.IDDetExterna,CarritoCotizacion.IDCarritoCotizacion,CarritoCotizacion.IDCotizacion,CarritoCotizacion.Suministro,Articulo.Descripcion as Articulo,CarritoCotizacion.Cantidad,CarritoCotizacion.Costo,CarritoCotizacion.CantidadPedida,CarritoCotizacion.Descuento,CarritoCotizacion.Importe,CarritoCotizacion.IVA,CarritoCotizacion.ImporteIva,CarritoCotizacion.ImporteTotal,CarritoCotizacion.Nota,CarritoCotizacion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoCotizacion INNER JOIN Caracteristica ON CarritoCotizacion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();
                ViewBag.carritor = lista;

                var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncCotizacion.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncCotizacion.TipoCambio)) as TotalenPesos from CarritoCotizacion inner join EncCotizacion on EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion where CarritoCotizacion.UserID='" + UserID + "' group by EncCotizacion.IDMoneda").ToList();
                ViewBag.sumatoria = resumen;


                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoCotizacion) as Dato from CarritoCotizacion where  UserID='" + UserID + "'").ToList()[0];
                ViewBag.dato = c.Dato;
          

                //Insertar Encabezado de Orden de Compra
                if (encPedido.IDFormapago != 0)
                {


                    List<VCarritoCotizacion> carritor = db.Database.SqlQuery<VCarritoCotizacion>("select Caracteristica.ID as IDCaracteristica,Articulo.IDArticulo,Articulo.MinimoCompra,CarritoCotizacion.IDDetExterna,CarritoCotizacion.IDCarritoCotizacion,CarritoCotizacion.IDCotizacion,CarritoCotizacion.Suministro,Articulo.Descripcion as Articulo,CarritoCotizacion.Cantidad,CarritoCotizacion.Costo,CarritoCotizacion.CantidadPedida,CarritoCotizacion.Descuento,CarritoCotizacion.Importe,CarritoCotizacion.IVA,CarritoCotizacion.ImporteIva,CarritoCotizacion.ImporteTotal,CarritoCotizacion.Nota,CarritoCotizacion.Ordenado,Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoCotizacion INNER JOIN Caracteristica ON CarritoCotizacion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();

                    ViewBag.carritor = carritor;
                    //Termina 


                    int num = 0;
                    DateTime fecha = encPedido.Fecha;
                    string fecha1 = fecha.ToString("yyyy/MM/dd");

                    DateTime fechareq = encPedido.FechaRequiere;
                    string fecha2 = fechareq.ToString("yyyy/MM/dd");

                    List<c_Moneda> clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + encPedido.IDMoneda + "'").ToList();
                    string clave = clavemoneda.Select(s => s.ClaveMoneda).FirstOrDefault();


                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncPedido]([Fecha],[FechaRequiere],[IDCliente],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[TipoCambio],[UserID],[IDUsoCFDI],[IDVendedor],[Entrega],[OCompra]) values ('" + fecha1 + "','" + fecha2 + "','" + encPedido.IDCliente + "','" + encPedido.IDFormapago + "','" + encPedido.IDMoneda + "','" + encPedido.Observacion + "','" + subtotal + "','" + iva + "','" + total + "','" + encPedido.IDMetodoPago + "','" + encPedido.IDCondicionesPago + "','" + encPedido.IDAlmacen + "','Inactivo','1','" + UserID + "','" + encPedido.IDUsoCFDI + "','" + encPedido.IDVendedor + "','" + encPedido.Entrega + "','"+encPedido.OCompra+"')");
                    db.SaveChanges();

                    List<EncPedido> numero = db.Database.SqlQuery<EncPedido>("SELECT * FROM [dbo].[EncPedido] WHERE IDPedido = (SELECT MAX(IDPedido) from EncPedido)").ToList();
                    num = numero.Select(s => s.IDPedido).FirstOrDefault();


                    for (int i = 0; i < carritor.Count(); i++)
                    {
                        CotizacionContext dbreq = new CotizacionContext();
                        EncCotizacion encCotizacion = dbreq.EncCotizaciones.Find(ViewBag.carritor[i].IDCotizacion);
                        //Datos para tipo de cambio
                        List<c_Moneda> claved = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + encCotizacion.IDMoneda + "'").ToList();
                        string clavedet = claved.Select(s => s.ClaveMoneda).FirstOrDefault();

                    List<c_Moneda> uno = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='" + clave + "'").ToList();
                    int unoc = uno.Select(s => s.IDMoneda).FirstOrDefault();

                    List<c_Moneda> dos = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='" + clavedet + "'").ToList();
                    int dosc = dos.Select(s => s.IDMoneda).FirstOrDefault();

                    VCambio cambiodet = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + dosc + "," + unoc + ") as TC").ToList()[0];
                    decimal Cambiodet = cambiodet.TC;

                    //Fin de tipo de cambio
                        Precio = ViewBag.carritor[i].Costo * Cambiodet;
                        importe = ViewBag.carritor[i].Cantidad * Precio;
                        importeiva = importe * (decimal)0.16;
                        importetotal = importeiva + importe;

                        //Insercion de Detalle de Orden de Compra
                    Articulo articulo = new ArticuloContext().Articulo.Find(ViewBag.carritor[i].IDArticulo);
                    db.Database.ExecuteSqlCommand("INSERT INTO DetPedido([IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna],[GeneraOrdenP],[IDRemision],[IDPrefactura]) values ('" + num + "','" + ViewBag.carritor[i].IDArticulo + "','" + ViewBag.carritor[i].Cantidad + "','" + Precio + "','" + ViewBag.carritor[i].Cantidad + "','0','" + importe + "','true','" + importeiva + "','" + importetotal + "','" + ViewBag.carritor[i].Nota + "','0','" + ViewBag.carritor[i].IDCaracteristica + "','" + encPedido.IDAlmacen + "','0','Inactivo','" + ViewBag.carritor[i].Presentacion + "','" + ViewBag.carritor[i].jsonPresentacion + "','" + ViewBag.carritor[i].IDCotizacion + "','" + articulo.GeneraOrden + "','0','0')");

                    //db.Database.ExecuteSqlCommand("exec dbo.MovArt'" + fecha1 + "'," + ViewBag.carritor[i].IDCaracteristica + ",'PedVta'," + ViewBag.carritor[i].Cantidad + ",'Pedido'," + num + ",0,'" + encPedido.IDAlmacen + "','" + ViewBag.carritor[i].Nota + "',0");
                    List<DetPedido> numero2 = db.Database.SqlQuery<DetPedido>("SELECT * FROM [dbo].[DetPedido] WHERE IDDetPedido = (SELECT MAX(IDDetPedido) from DetPedido)").ToList();
                        int num2 = numero2.Select(s => s.IDDetPedido).FirstOrDefault();
                        db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Pedido','" + num + "','" + ViewBag.carritor[i].Cantidad + "','" + fecha1 + "','" + ViewBag.carritor[i].IDCotizacion + "','Cotización','" + UserID + "','" + ViewBag.carritor[i].IDDetExterna + "','" + num2 + "')");

                        db.Database.ExecuteSqlCommand("delete from CarritoCotizacion where IDCarritoCotizacion='" + ViewBag.carritor[i].IDCarritoCotizacion + "'");


                    }
                    List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                    int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();

                    VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + encPedido.IDMoneda + "," + origen + ") as TC").ToList()[0];
                    decimal Cambio = cambio.TC;

                    List<DetPedido> datostotales = db.Database.SqlQuery<DetPedido>("select * from dbo.DetPedido where IDPedido='" + num + "'").ToList();
                    subtotalr = datostotales.Sum(s => s.Importe);
                    ivar = subtotalr * (decimal)0.16;
                    totalr = subtotalr + ivar;
                    db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [TipoCambio]='" + Cambio + "',[Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "' where [IDPedido]='" + num + "'");

                    return RedirectToAction("Index");

                }
            
            return View();
        }
    
        public ActionResult deletecarrito(int? id)
        {
            CarritoContext cr = new CarritoContext();
            CarritoCotizacion carritoCotizacion = cr.CarritoCotizaciones.Find(id);
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            
            db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoCotizacion] where [IDCarritoCotizacion]='" + id + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetCotizacion] set [Status]='Activo' where [IDDetCotizacion]='" + carritoCotizacion.IDDetExterna + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[EncCotizacion] set [Status]='Activo' where [IDCotizacion]='" + carritoCotizacion.IDCotizacion + "'");


            ClsDatoEntero dato = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoCotizacion) as Dato from CarritoCotizacion where UserID ='" + UserID + "'").ToList()[0];
            if (dato.Dato == 0)
            {
                return RedirectToAction("Index", "EncCotizacion");
            }
            ViewBag.id = carritoCotizacion.IDCotizacion;



            ClientesContext pr = new ClientesContext();
            CotizacionContext bd = new CotizacionContext();
            EncCotizacion encCotizacion = bd.EncCotizaciones.Find(carritoCotizacion.IDCotizacion);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.Where(s => s.IDUsoCFDI.Equals(encCotizacion.IDUsoCFDI)), "IDUsoCFDI", "Descripcion");
            ViewBag.IDAlmacen = new SelectList(db.Almacenes.Where(s => s.IDAlmacen.Equals(encCotizacion.IDAlmacen)), "IDAlmacen", "Descripcion");

            var cliente = prov.Clientes.ToList();

            List<SelectListItem> moneda = new List<SelectListItem>();
            c_Moneda monedap = pr.c_Monedas.Find(encCotizacion.IDMoneda);
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
            c_MetodoPago metodop = pr.c_MetodoPagos.Find(encCotizacion.IDMetodoPago);
            metodo.Add(new SelectListItem { Text = metodop.Descripcion, Value = metodop.IDMetodoPago.ToString() });
            ViewBag.metodo = metodo;

            List<SelectListItem> forma = new List<SelectListItem>();
            c_FormaPago formap = pr.c_FormaPagos.Find(encCotizacion.IDFormapago);
            forma.Add(new SelectListItem { Text = formap.Descripcion, Value = formap.IDFormaPago.ToString() });
            ViewBag.forma = forma;

            List<SelectListItem> condiciones = new List<SelectListItem>();
            CondicionesPago condicionesp = pr.CondicionesPagos.Find(encCotizacion.IDCondicionesPago);
            condiciones.Add(new SelectListItem { Text = condicionesp.Descripcion, Value = condicionesp.IDCondicionesPago.ToString() });
            ViewBag.condiciones = condiciones;

            List<SelectListItem> li = new List<SelectListItem>();
            Clientes mm = pr.Clientes.Find(encCotizacion.IDCliente);
            li.Add(new SelectListItem { Text = mm.Nombre, Value = mm.IDCliente.ToString() });
            ViewBag.cliente = li;

            List<SelectListItem> vendedor = new List<SelectListItem>();
            Vendedor vendedorp = pr.Vendedores.Find(encCotizacion.IDVendedor);
            vendedor.Add(new SelectListItem { Text = vendedorp.Nombre, Value = vendedorp.IDVendedor.ToString() });
            ViewBag.vendedor = vendedor;

            List<SelectListItem> entrega = new List<SelectListItem>();
            List<VEntrega> entregasall = db.Database.SqlQuery<VEntrega>("select en.IDEntrega,en.IDCliente,en.CalleEntrega,en.NumExtEntrega,ISNULL(en.NumIntEntrega,0),en.ColoniaEntrega,en.MunicipioEntrega,en.CPEntrega,en.ObservacionEntrega,es.Estado from dbo.Entrega as en inner join Estados as es on es.IDEstado=en.IDEstado where IDCliente='" + encCotizacion.IDCliente + "'").ToList();
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

            ViewBag.data = null;

            ViewBag.otro = 0;

            ViewBag.datos = null;


            List<VCarritoCotizacion> lista = db.Database.SqlQuery<VCarritoCotizacion>("select Articulo.MinimoCompra,CarritoCotizacion.IDDetExterna,CarritoCotizacion.IDCarritoCotizacion,CarritoCotizacion.IDCotizacion,CarritoCotizacion.Suministro,Articulo.Descripcion as Articulo,CarritoCotizacion.Cantidad,CarritoCotizacion.Costo,CarritoCotizacion.CantidadPedida,CarritoCotizacion.Descuento,CarritoCotizacion.Importe,CarritoCotizacion.IVA,CarritoCotizacion.ImporteIva,CarritoCotizacion.ImporteTotal,CarritoCotizacion.Nota,CarritoCotizacion.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoCotizacion INNER JOIN Caracteristica ON CarritoCotizacion.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();
            ViewBag.carritor = lista;

            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncCotizacion.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncCotizacion.TipoCambio)) as TotalenPesos from CarritoCotizacion inner join EncCotizacion on EncCotizacion.IDCotizacion=CarritoCotizacion.IDCotizacion where CarritoCotizacion.UserID='" + UserID + "' group by EncCotizacion.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;

            return View("Cotizaciones");


        }
        public ActionResult Edit(int id, int? page, string searchString)
        {

            System.Web.HttpContext.Current.Session["idpedido"] = id;

            List<VDetPedido> pedidodetalle = db.Database.SqlQuery<VDetPedido>("select* from VDetPedido where IDPedido='" + id + "'").ToList();

            ViewBag.req = pedidodetalle;

            EncPedido pedido = new PedidoContext().EncPedidos.Find(id);

            List<SelectListItem> vendedor = new List<SelectListItem>();
            ViewBag.searchString = searchString;
            ViewBag.page = page;
            var todosvendedor = prov.Vendedores.ToList();
            if (todosvendedor != null)
            {
                foreach (Vendedor y in todosvendedor)
                {
                    SelectListItem eleven = new SelectListItem { Text = y.Nombre, Value = y.IDVendedor.ToString() };
                    if (y.IDVendedor == pedido.IDVendedor)
                    {
                        eleven.Selected = true;
                    }
                    vendedor.Add(eleven);
                }
            }

            ViewBag.vendedor = vendedor;
            return View(pedido);
        }


        [HttpPost]
        public ActionResult Edit(EncPedido elemento, int? page, string searchString, FormCollection coleccion)

        {
            // si cambia el numero de elementos en el view este codigo tendraq que reescribirse 
            // restndo el numero de elementos antes de llegar a la nota de partida

            int resta = coleccion.Count - 22; //22 es el numero de elementos antes de llegar al iddetventa y nota

            // como son dos elementos en el det pedido iddetpedido y nota dividiremos entre 2

            int cuantos = 1;

            try
            {
                cuantos = resta / 2;

            }
            catch (Exception err)
            {

            }
            string notas = coleccion.Get("Nota");
            string ids = coleccion.Get("Iddetpedido");
            string[] arrayID = ids.Split(',');
            string[] arrayNota = notas.Split(',');
            for (int i = 0; i <= /*(cuantos)*/arrayID.Length; i++)
            {
                //string id = coleccion[22 + (i * 2) - 1];
                //string nota = coleccion[22 + (i * 2)];
              
                string id = "";
                string nota = "";
                try
                {
                    id = arrayID[i];
                    nota = arrayNota[i];
                    string cadenax = "update Detpedido set Nota='" + nota + "' where iddetPedido=" + id;
                    new PedidoContext().Database.ExecuteSqlCommand(cadenax);
                }
                catch (Exception error)
                {
                    //try
                    //{
                    //    string cadenax = "update Detpedido set Nota='" + notas + "' where iddetPedido=" + ids;
                    //    new PedidoContext().Database.ExecuteSqlCommand(cadenax);
                    //}
                    //catch (Exception err)
                    //{

                    //}
                }


             
            }


            string cadena = "update Encpedido set IDVendedor='" + elemento.IDVendedor + "', Observacion='" + elemento.Observacion + "',Ocompra='" + elemento.OCompra + "', Entrega='" + elemento.Entrega + "', TipoCambio=" + elemento.TipoCambio + " where idPedido=" + elemento.IDPedido;
            new PedidoContext().Database.ExecuteSqlCommand(cadena);
            return RedirectToAction("Details", new { id = elemento.IDPedido, page = page, searchString = searchString });
        }

        //public ActionResult Edit(int id, int? page,string  searchString)
        //{

        //    EncPedido pedido = new PedidoContext().EncPedidos.Find(id);

        //    List<SelectListItem> vendedor = new List<SelectListItem>();
        //    ViewBag.searchString = searchString;
        //    ViewBag.page = page;
        //    var todosvendedor = prov.Vendedores.ToList();
        //    if (todosvendedor != null)
        //    {
        //        foreach (Vendedor y in todosvendedor)
        //        {
        //            SelectListItem eleven = new SelectListItem { Text = y.Nombre, Value = y.IDVendedor.ToString() };
        //            if (y.IDVendedor==pedido.IDVendedor)
        //            {
        //                eleven.Selected = true;
        //            }
        //            vendedor.Add(eleven);
        //        }
        //    }

        //    ViewBag.vendedor = vendedor;
        //    return View(pedido);
        //}


        //[HttpPost]
        //public ActionResult Edit(EncPedido elemento, int? page, string searchString)

        //{
        //    string cadena = "update Encpedido set IDVendedor=" + elemento.IDVendedor + ", Observacion='" + elemento.Observacion + "',Ocompra='" + elemento.OCompra + "', Entrega='" + elemento.Entrega + "', TipoCambio=" + elemento.TipoCambio + " where idPedido=" + elemento.IDPedido;
        //    new PedidoContext().Database.ExecuteSqlCommand(cadena);
        //    return RedirectToAction("Details", new { id = elemento.IDPedido, page=page, searchString= searchString });
        //}

        [HttpPost]
        public JsonResult Edititem(int id, decimal cantidad)
        {
            try
            {
                decimal importe = 0, importeiva = 0, importetotal = 0;
                CarritoContext car = new CarritoContext();
                CarritoCotizacion carritocotizacion = car.CarritoCotizaciones.Find(id);
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                importe =carritocotizacion.Costo*cantidad;
                importeiva = importe * (decimal)0.16; ;
                importetotal =importe+importeiva;

                 db.Database.ExecuteSqlCommand("update [dbo].[CarritoCotizacion] set [Cantidad]=" + cantidad + ",[Importe]='"+importe+"',[ImporteIva]='"+importeiva+"',[ImporteTotal]='"+importetotal+ "' where IDCarritoCotizacion=" + id+ " and UserID='"+usuario+"'");

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
      
     public ActionResult PdfPedido(int id)
        {

            EncPedido pedido = new PedidoContext().EncPedidos.Find(id);
            DocumentoPedido x = new DocumentoPedido();

            x.claveMoneda = pedido.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = pedido.Fecha.ToShortDateString();
            x.fechaRequerida = pedido.FechaRequiere.ToShortDateString();
            x.Cliente = pedido.Clientes.Nombre;
            x.formaPago = pedido.c_FormaPago.ClaveFormaPago;
            x.metodoPago = pedido.c_MetodoPago.ClaveMetodoPago;
            x.RFCCliente = pedido.Clientes.RFC;
            x.TelefonoCliente = pedido.Clientes.Telefono;
            x.total = float.Parse(pedido.Total.ToString());
            x.subtotal = float.Parse(pedido.Subtotal.ToString());
            x.tipo_cambio = pedido.TipoCambio.ToString();
            x.serie = "";
            x.folio = pedido.IDPedido.ToString();
            x.UsodelCFDI = pedido.c_UsoCFDI.Descripcion;
            x.IDPedido = pedido.Almacen.IDAlmacen;
            x.Empresa = pedido.Almacen.Telefono;
            x.condicionesdepago = pedido.CondicionesPago.Descripcion;
            x.OCompra = pedido.OCompra;
            x.facturaexacto = pedido.Clientes.FacturacionExacta;
            x.RequiereCertificado = pedido.Clientes.CertificadoCalidad;
            x.Entrega = pedido.Entrega;
            x.Observacion = pedido.Observacion;
            x.Vendedor = pedido.Vendedor.Nombre;
            x.IDPedido = pedido.IDPedido;

            ImpuestoPedido iva = new ImpuestoPedido();
            iva.impuesto = "IVA";
            iva.tasa = 16;
            iva.importe = float.Parse(pedido.IVA.ToString());


            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
            x.firmadefinanzas = empresa.Director_finanzas;
            x.firmadecompras = empresa.Persona_de_Compras + "";

            List<DetPedido> detalles = db.Database.SqlQuery<DetPedido>("select * from [dbo].[DetPedido] where IDPedido=" + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoPedido producto = new ProductoPedido();
                Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);
				Almacen alma = new AlmacenContext().Almacenes.Find(item.IDAlmacen);
               
                producto.ClaveProducto = arti.Cref;
				producto.idarticulo = arti.IDArticulo;
                producto.c_unidad = arti.c_ClaveUnidad.ClaveUnidad;
                producto.cantidad = item.Cantidad.ToString();
                producto.descripcion = arti.Descripcion;
				producto.almacen = alma.CodAlm;
                producto.valorUnitario = float.Parse(item.Costo.ToString());
                producto.v_unitario = float.Parse(item.Costo.ToString());
                producto.importe = float.Parse(item.Importe.ToString());
                producto.iddetpedido = item.IDDetPedido;

                try
                {
                    ClsDatoEntero idorden = db.Database.SqlQuery<ClsDatoEntero>("select IDOrden as Dato from OrdenProduccion where IDdetPedido=" + item.IDDetPedido + " and EstadoOrden!='Cancelada'").FirstOrDefault();
                    
                    producto.OProduccion = idorden.Dato;

                }
                catch (Exception E)
                {
                    string mensajedeerror = E.Message;
                }
                // para que aparezca el numero de presentacion en el pedido antes de la presentacion NP = numero de la presentacion
                Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + item.Caracteristica_ID).FirstOrDefault();
                //
                producto.Presentacion = "NP "+ caracteristica.IDPresentacion + "  " + item.Presentacion; //item.presentacion;//item.presentacion;
                producto.Nota = item.Nota;
                ///
                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            try
            {
                CreaPedidoPDF documentop = new CreaPedidoPDF(logoempresa, x);
                return new FilePathResult(documentop.nombreDocumento, contentType);
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            return RedirectToAction("IndexPedido");
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

        public string CrearOrden(int cotizacion, decimal cantidad, int ordenproduccion, decimal TC, int IDMoneda, int modelo) // cfreamos mensajes que devuelve la rutina
        {

            string mensajeerror = string.Empty;

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cotizacion);
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            try
            {
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);


                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                string mensajedeerror = er.Message;
                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            }

            elemento.Cantidad = cantidad;

            FormulaEspecializada.Formulaespecializada formula = new FormulaEspecializada.Formulaespecializada();
            formula = igualar(elemento, formula);

            formula.Calcular();

            elemento.CantidadMPMts2 = formula.CantidadMPMts2;
            elemento.anchomaterialenmm = formula.anchomaterialenmm;
            elemento.largomaterialenMts = formula.largomaterialenMts;
            elemento.CintasMaster = formula.CintasMaster;
            elemento.Numerodecintas = formula.Numerodecintas;
            elemento.MtsdeMerma = formula.MtsdeMerma;
            elemento.CostototalMP = formula.CostototalMP;

            elemento.HrPrensa = formula.getHoraPrensa();

            if (elemento.mangatermo)
            {
                elemento.HrSellado = formula.HrSellado;
                elemento.HrInspeccion = formula.HrInspeccion;
                elemento.HrCorte = formula.HrCorte;
            }
            else
            {
                elemento.HrEmbobinado = formula.getHoraEmbobinado();
            }


            Plantilla articulosdeplantilla = null;

            if (modelo == 7)
            {
                try
                {
                    articulosdeplantilla = Modelo7(elemento);
                }
                catch (Exception err)
                {
                    string mensajeerror1 = err.Message;
                }




            }

            if (modelo == 4)
            {
                try
                {
                    articulosdeplantilla = Modelo4(elemento);

                }
                catch (Exception err)
                {
                    string mensajeerror1 = err.Message;
                }

            }

            if (modelo == 8)
            {
                try
                {
                    articulosdeplantilla = Modelo8(elemento);

                }
                catch (Exception err)
                {
                    string mensajeerror1 = err.Message;
                }

            }

            foreach (ArticuloXML artipro in articulosdeplantilla.Articulos)
            {


                FormulaSiaapi.Formulas formulaparasacarinfo = new FormulaSiaapi.Formulas();

                string forMU = artipro.Formula;


                ArticuloContext basa = new ArticuloContext();
                int idar = int.Parse(artipro.IDArticulo);
                Articulo articulo = basa.Articulo.Find(idar);

                Caracteristica caraarticulo = basa.Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + artipro.IDCaracteristica).FirstOrDefault();


                string formulanueva = forMU;
                try
                {
                    formulanueva = formulaparasacarinfo.sustituircontenidocadena(forMU, double.Parse(cantidad.ToString()));
                }
                catch (Exception err)
                {


                }
                if (artipro.FactorCierre == null)
                { artipro.FactorCierre = "0"; }


                double valorfin = formulaparasacarinfo.Calcular(formulanueva, double.Parse(artipro.FactorCierre.ToString()));

                //////////////////////checa el costo ///////////////////////

                double costo = 0;
                double costounitario = 0;
                try
                {
                    ClsDatoDecimal cuanto = new CobroContext().Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetCosto] (0," + articulo.IDArticulo + "," + valorfin + ") as Dato ").ToList()[0];
                    costo = double.Parse(cuanto.Dato.ToString());
                }
                catch (Exception err)
                {
                    mensajeerror += err.Message;
                }

                string clavemonedacotizar = new c_MonedaContext().c_Monedas.Find(articulo.IDMoneda).ClaveMoneda;


                decimal tcc = TC;
                if (articulo.IDMoneda == IDMoneda)
                {
                    tcc = 1;
                }
                costounitario = costo;
                if (valorfin >= 0)
                {
                    costo = costo * valorfin;
                }


                decimal costofinal = 0;

                if (articulo.c_Moneda.ClaveMoneda == "MXN" && clavemonedacotizar == "USD")
                {
                    costofinal = (decimal)costo / tcc;
                }
                if (articulo.c_Moneda.ClaveMoneda == "USD" && clavemonedacotizar == "MXN")
                {
                    costofinal = (decimal)costo * tcc;
                }
                if (articulo.c_Moneda.ClaveMoneda == clavemonedacotizar)
                {
                    costofinal = (decimal)costo;
                }

                costofinal = Math.Round(costofinal, 2);

                if ((articulo.IDTipoArticulo == 1) || (articulo.IDTipoArticulo == 4) || (articulo.IDTipoArticulo == 6) || (articulo.IDTipoArticulo == 7))
                {
                    StringBuilder cadenasolcitud = new StringBuilder();

                    cadenasolcitud.Append("insert into [DetSolicitud]([IDArticulo],[Caracteristica_ID],[IDAlmacen],[Documento],[Numero],[Cantidad],[Costo],[CantidadPedida],");
                    cadenasolcitud.Append("[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Requerido],[Suministro],[Status],[DocumentoR],[NumeroDR],[Presentacion],[jsonPresentacion]) values (");


                    cadenasolcitud.Append(artipro.IDArticulo + ",");
                    cadenasolcitud.Append(artipro.IDCaracteristica + ",");
                    Caracteristica caracterisp = null;
                    try
                    {
                        caracterisp = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + artipro.IDCaracteristica).ToList().FirstOrDefault();
                    }
                    catch (Exception err)
                    {
                        string mensajeerro = err.Message;
                    }



                    switch (articulo.IDTipoArticulo)   /// de acurdo altipo de articulo es al almacen 
                    {
                        case 6:
                            cadenasolcitud.Append("6,"); // 
                            break;
                        case 7:
                            cadenasolcitud.Append("1,");
                            break;
                        default:
                            cadenasolcitud.Append("2,");
                            break;

                    }

                    cadenasolcitud.Append("'Orden de  Produccion',");
                    cadenasolcitud.Append(ordenproduccion + ",");
                    cadenasolcitud.Append(valorfin + ","); //PEDIDO
                    decimal costofin = 0;
                    try
                    {
                        costofin = Math.Round((costofinal / decimal.Parse(valorfin.ToString())), 2);
                    }
                    catch (Exception err)
                    {

                    }
                    cadenasolcitud.Append(costofin + ",");
                    cadenasolcitud.Append("0,");
                    cadenasolcitud.Append("0,"); //descuento
                    cadenasolcitud.Append(costofinal + ",");

                    cadenasolcitud.Append("'1',");
                    decimal ivaimp = Math.Round(costofinal * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                    cadenasolcitud.Append(ivaimp + ",");
                    cadenasolcitud.Append(costofinal + ivaimp + ",");
                    cadenasolcitud.Append("'',");
                    cadenasolcitud.Append("'0',");
                    cadenasolcitud.Append("0,"); //suministro
                    cadenasolcitud.Append("'Solicitado',"); //status
                    cadenasolcitud.Append("'',"); //documento
                    cadenasolcitud.Append("0,");
                    cadenasolcitud.Append("'" + caracterisp.Presentacion + "',");
                    cadenasolcitud.Append("'')");




                    try
                    {
                        db.Database.ExecuteSqlCommand(cadenasolcitud.ToString());
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                    }

                }

                try
                {
                    string existe = "'0'";
                    if ((articulo.IDTipoArticulo == 3 || articulo.IDTipoArticulo == 5) || (artipro.IDProceso == "3" || artipro.IDProceso == "7" || artipro.IDProceso == "10" || artipro.IDProceso == "11" || artipro.IDProceso == "12" || artipro.IDProceso == "16" || artipro.IDProceso == "4"))
                    {
                        existe = "'1'";
                    }
                    else
                    {

                    }
                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]([IDHE],[IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],TC,TCR,[Existe]) VALUES('"
                                                                        + caraarticulo.IDCotizacion + "','" + articulo.IDArticulo + "','" + articulo.IDTipoArticulo + "','" + artipro.IDCaracteristica + "','" + artipro.IDProceso + "','" + ordenproduccion + "'," + valorfin + ",'" + articulo.IDClaveUnidad + "','" + artipro.Indicaciones + "'," + costofinal + ",0," + tcc + ",0," + existe + ")");
                    /////   aqui vamos a traer el costo del articulo /////

                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                }
            }
            // fin del for


            return mensajeerror;


        }

        public string ValidadArticulosOP(int cotizacion, int modelo) // cfreamos mensajes que devuelve la rutina
     {

            string Mensaje = "";
        	string mensajeerror = string.Empty;

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cotizacion);
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            try
            {
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);


                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
                }
            }
            catch(Exception er)
            {
                string mensajedeerror = er.Message;
                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            }

            


            Plantilla articulosdeplantilla = null;

            if (modelo == 7)
            {
                try
                {
                    articulosdeplantilla = Modelo7(elemento);
                }
                catch (Exception err)
                {
                    string mensajeerror1 = err.Message;
                }




            }

            if (modelo == 4)
            {
                try
                {
                    articulosdeplantilla = Modelo4(elemento);

                }
                catch(Exception err)
                {
                    string mensajeerror1 = err.Message;
                }

            }

            if (modelo == 8)
            {
                try
                {
                    articulosdeplantilla = Modelo8(elemento);

                }
                catch (Exception err)
                {
                    string mensajeerror1 = err.Message;
                }

            }

            foreach (ArticuloXML artipro in articulosdeplantilla.Articulos)
            {


                FormulaSiaapi.Formulas formulaparasacarinfo = new FormulaSiaapi.Formulas();

                string forMU = artipro.Formula;


                ArticuloContext basa = new ArticuloContext();
                int idar = int.Parse(artipro.IDArticulo);
                Articulo articulo = basa.Articulo.Find(idar);
                try
                {

                    Caracteristica caraarticulo = basa.Database.SqlQuery<Caracteristica>("select * from caracteristica where  obsoleto='false' and id=" + artipro.IDCaracteristica).FirstOrDefault();

                    if (caraarticulo == null)
                    {
                        Mensaje += "La presentación del artículo " + articulo.Cref + " está Obsoleta\n";
                    }
                    else
                    {

                    }

                }
                catch (Exception err)
                {

                }







            }

            return Mensaje;


        }


        public ActionResult EntreFechas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EntreFechas(ReportePedidosD modelo, FormCollection coleccion)
        {
           
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();
            string cual = coleccion.Get("Enviar");

            string cadena = "";
            if (cual == "Generar reporte pdf")
            {
                ReportePedidosD report = new ReportePedidosD();
                byte[] abytes = report.PrepareReport(modelo.fechaini, modelo.fechafin);

                return File(abytes, "application/pdf");
            }
            else
            {
                VPedidosContext dbp = new VPedidosContext();
                VPedidoDetContext dbd = new VPedidoDetContext();
                string CadSql1 = "select* from VPedidos where fecha >= '" + FI + "' and fecha <= '" + FF + "' and Status != 'Cancelado' order by Cliente asc, idpedido desc";
                string CadSql2 = "select* from  VPedidoDet where fecha >= '" + FI + "' and fecha <= '" + FF + "' and Status != 'Cancelado' order by Cliente asc, idpedido desc";
                var datosPed = dbp.Database.SqlQuery<VPedidos>(CadSql1).ToList();
                ViewBag.datosPed = datosPed;
                var datosDet = dbp.Database.SqlQuery<VPedidoDet>(CadSql2).ToList();
                ViewBag.datosDet = datosDet;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Pedidos");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:L1"].Style.Font.Size = 20;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L3"].Style.Font.Bold = true;
                Sheet.Cells["A1:L1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Pedidos");

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
                Sheet.Cells["A3"].RichText.Add("No. Pedido");
                Sheet.Cells["B3"].RichText.Add("O.C.");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("Subtotal");
                Sheet.Cells["E3"].RichText.Add("Iva");
                Sheet.Cells["F3"].RichText.Add("Total");
                Sheet.Cells["G3"].RichText.Add("Moneda");
                Sheet.Cells["H3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["I3"].RichText.Add("Estado");
                Sheet.Cells["J3"].RichText.Add("Cliente");
                Sheet.Cells["K3"].RichText.Add("Oficina");
                Sheet.Cells["L3"].RichText.Add("Vendedor");
                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:L3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VPedidos item in ViewBag.datosPed)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDPedido;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.OCompra;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Status;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Cliente;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.NombreOficina;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Nombre;


                    row++;
                }

                Sheet = Ep.Workbook.Worksheets.Add("Suministro");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:L1"].Style.Font.Size = 20;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L3"].Style.Font.Bold = true;
                Sheet.Cells["A1:L1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Pedidos suministro");

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
                Sheet.Cells["A3"].RichText.Add("No. Pedido");
                Sheet.Cells["B3"].RichText.Add("No. Remision");
                Sheet.Cells["C3"].RichText.Add("No. Prefactura");
                Sheet.Cells["D3"].RichText.Add("No. Fecha");
                Sheet.Cells["E3"].RichText.Add("Cliente");
                Sheet.Cells["F3"].RichText.Add("Código");
                Sheet.Cells["G3"].RichText.Add("Articulo");
                Sheet.Cells["H3"].RichText.Add("Presentacion");
                Sheet.Cells["I3"].RichText.Add("Cantidad Pedida");
                Sheet.Cells["J3"].RichText.Add("Cantidad Suministrada");
                Sheet.Cells["K3"].RichText.Add("Precio Unitario");
                Sheet.Cells["L3"].RichText.Add("IVA");
                Sheet.Cells["M3"].RichText.Add("Importe");
                Sheet.Cells["N3"].RichText.Add("Cod Almacen");
                Sheet.Cells["O3"].RichText.Add("Almacen");

                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:O3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VPedidoDet item in ViewBag.datosDet)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDPedido;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.IDRemision;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.IDPrefactura;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Cliente;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Cref;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Articulo;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Presentacion;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Cantidad;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.suministro;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Costo;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.ImporteIva;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.ImporteTotal;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.CodAlm;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.Almacen;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelPedidos.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }

            
        }

        public ActionResult ReportePedidos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReportePedidos(ReportePedidos modelo)
        {
            ReportePedidos report = new ReportePedidos();
            byte[] abytes = report.PrepareReport(modelo.fechaini, modelo.fechafin);

            return File(abytes, "application/pdf");
            
        }

        public ActionResult ReportePedidosCliente()
        {
            List<ClientesPedido> clientes = new List<ClientesPedido>();
            string cadena = "SELECT  distinct c.IDCliente, c.nombre as Nombre, c.Telefono , c.IDVendedor from clientes as c inner join encPedido as p on c.idcliente=p.idcliente where c.status='activo'";
            clientes = db.Database.SqlQuery<ClientesPedido>(cadena).ToList();
            List<SelectListItem> listacliente = new List<SelectListItem>();
            listacliente.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            foreach (var m in clientes)
            {
                listacliente.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
            }
            ViewBag.cliente = listacliente;
            return View();
        }

        [HttpPost]
        public ActionResult ReportePedidosCliente(ReportePeCliente modelo, ClientesPedido C)
        {
            int idcliente = C.IDCliente;
            try
            {

                ClientesContext dbc = new ClientesContext();
                Clientes cls = dbc.Clientes.Find(C.IDCliente);
            }
            catch (Exception ERR)
            {

            }

            ReportePeCliente report = new ReportePeCliente();
            //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
            byte[] abytes = report.PrepareReport(modelo.fechaini, modelo.fechafin, idcliente);
            return File(abytes, "application/pdf");
            //return Redirect("index");
        }

        public ActionResult EntreFechasVen()
        {
            List<VendedorPe> vendedor = new List<VendedorPe>();
            string cadena = "select distinct v.IdVendedor, v.Nombre from vendedor as v inner join encpedido as e on v.idvendedor=e.IDVendedor";
            vendedor = db.Database.SqlQuery<VendedorPe>(cadena).ToList();
            List<SelectListItem> listavendedor = new List<SelectListItem>();
            listavendedor.Add(new SelectListItem { Text = "--Selecciona Vendedor--", Value = "0" });

            foreach (var m in vendedor)
            {
                listavendedor.Add(new SelectListItem { Text = m.Nombre, Value = m.IDVendedor.ToString() });
            }
            ViewBag.vendedor = listavendedor;
            return View();

        }

        [HttpPost]
        public ActionResult EntreFechasVen(ReportePedidosVendedor modelo, VendedorPe v, FormCollection coleccion)
        {
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();
            DateTime fecAct = DateTime.Today;
            string FA = fecAct.ToString("dd/MM/yyyy");
            string cual = coleccion.Get("Enviar");

            int idvendedor = v.IDVendedor;


            string cadena = "";
            if (cual == "Generar reporte pdf")
            {
                try
                {

                    VendedorContext dbc = new VendedorContext();
                    Vendedor ven = dbc.Vendedores.Find(v.IDVendedor);
                }
                catch (Exception ERR)
                {

                }

                ReportePedidosVendedor report = new ReportePedidosVendedor();
                //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
                byte[] abytes = report.PrepareReport(modelo.fechaini, modelo.fechafin, idvendedor);
                return File(abytes, "application/pdf");
            }

            List<VendedorPe> vendedores = new List<VendedorPe>();
            if (cual == "Generar excel")
            {

                try
                {
                    string cadena1 = "select  distinct E.IDVendedor, V.Nombre, V.CuotaVendedor, M.ClaveMoneda from [dbo].[EncPedido] as E inner join [dbo].[Vendedor] as V on E.IDVendedor = V.IDVendedor inner join [dbo].[c_Moneda] as M on V.IDMoneda = M.IDMoneda where e.IDVendedor= " + idvendedor + " and fecha>='" + FI + " 00:00:01' and fecha <='" + FF + " 23:59:59' and e.status<>'cancelado' order by nombre";
                    vendedores = db.Database.SqlQuery<VendedorPe>(cadena1).ToList();
                }
                catch (SqlException err)
                {
                    string mensajedeerror = err.Message;
                }



                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Reporte de Pedidos en Dolares por Vendedor");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:I1"].Style.Font.Size = 20;
                Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:I3"].Style.Font.Bold = true;
                Sheet.Cells["A1:I1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Pedidos por Vendedor");

                row = 2;
                Sheet.Cells["A1:I1"].Style.Font.Size = 12;
                Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:I2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha Reporte";
                Sheet.Cells[string.Format("C2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C2", row)].Value = fecAct;

                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:I3"].Style.Font.Bold = true;
                Sheet.Cells["A3:I3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:I3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos. 
                
                Sheet.Cells["C3:H3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;


                row = 4;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A4:I4"].Style.Font.Bold = true;
                Sheet.Cells["A4:I4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                Sheet.Cells["C4:E4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleGoldenrod);
                Sheet.Cells["F4:H4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PowderBlue);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A4"].RichText.Add("Fecha");
                Sheet.Cells["B4"].RichText.Add("Pedido");
                Sheet.Cells["C4"].RichText.Add("Subtotal");
                Sheet.Cells["D4"].RichText.Add("IVA");
                Sheet.Cells["E4"].RichText.Add("Total");
                Sheet.Cells["F4"].RichText.Add("Status");
                Sheet.Cells["G4"].RichText.Add("TC");
                Sheet.Cells["H4"].RichText.Add("Total can");
                //Sheet.Cells["I4"].RichText.Add("TotalMXN(AñoActual - AñoAnterior)");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A4:H4"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 5;
                Sheet.Cells.Style.Font.Bold = false;
                decimal acumuladodlsvendedor = 0M;
                foreach (VendedorPe item in vendedores)
                {
                    string concatecaE = "A" + row + ":I" + row;
                    Sheet.Cells[concatecaE].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[concatecaE].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    Sheet.Cells[concatecaE].Style.Font.Bold = true;

                    Sheet.Cells[string.Format("A{0}", row)].Style.Font.Bold = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "ID Vendedor: " + item.IDVendedor;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("C{0}", row)].Value = "Cuota: " + item.CuotaVendedor;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveMoneda;

                    row++;
                    List<ClientesPed> clientes = new List<ClientesPed>();

                    try
                    {
                        string cadena1 = "Select distinct P.IDCliente,  C.Nombre, C.Telefono from dbo.EncPedido as P inner join Clientes as C on P.IDCliente = C.IDCliente where P.IDVendedor= " + item.IDVendedor + " and fecha>='" + FI+ " 00:00:01' and fecha <='" +FF + " 23:59:59' and p.status<>'cancelado' order by C.Nombre";
                        clientes = db.Database.SqlQuery<ClientesPed>(cadena1).ToList();
                    }
                    catch (SqlException err)
                    {
                        string mensajedeerror = err.Message;
                    }
                    foreach (ClientesPed cliente in clientes)
                    {
                        string concateca = "A" + row + ":I" + row;
                        Sheet.Cells[concateca].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[concateca].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                        Sheet.Cells[concateca].Style.Font.Bold = true;

                        Sheet.Cells[string.Format("A{0}", row)].Value = cliente.IDCliente;
                        Sheet.Cells[string.Format("B{0}", row)].Value = cliente.Nombre;

                        Sheet.Cells[string.Format("C{0}", row)].Value = cliente.Telefono;

                        row++;

                        List<ReporteVen> pedidos = new List<ReporteVen>();
                        try
                        {
                            string cadena2 = "SELECT P.idPedido, p.fecha,p.subtotal, p.iva, p.total, p.IDMoneda, CAST(1 AS DECIMAL(8,2)) as TC, p.status, CAST(0 AS DECIMAL(8,2)) as TotalDls FROM clientes AS c INNER JOIN encPedido AS p ON c.IDCliente=p.idcliente where p.idvendedor="+item.IDVendedor+" and p.IdCliente=" + cliente.IDCliente + " and fecha>='" + FI + " 00:00:01' and fecha <='" + FF + " 23:59:59' and p.status<>'Cancelado' order by p.fecha";
                            pedidos = db.Database.SqlQuery<ReporteVen>(cadena2).ToList();
                        }
                        catch (SqlException err)
                        {
                            string mensajedeerror = err.Message;
                        }
                        decimal acumulasubtotalxcliente = 0;
                        decimal acumulaivaxcliente = 0;
                        decimal acumuatotalxcliente = 0;
                        decimal acumuladlsxcliente = 0;

                        foreach (ReporteVen pedido in pedidos)
                        {
                            if (pedido.status == "Cancelado")
                            {
                                pedido.subtotal = 0;
                                pedido.IVA = 0;
                                pedido.total = 0;

                            }
                            Sheet.Cells[string.Format("A{0}", row)].Style.Numberformat.Format = "yyyy-mm-dd";
                            Sheet.Cells[string.Format("A{0}", row)].Value = pedido.fecha;
                            Sheet.Cells[string.Format("B{0}", row)].Value = pedido.IdPedido;
                            Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("C{0}", row)].Value = pedido.subtotal;
                            Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("D{0}", row)].Value = pedido.IVA;
                            Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            string clavemoneda = new c_MonedaContext().c_Monedas.Find(pedido.IDMoneda).ClaveMoneda; // esta es la moneda del pedido
                            string clavemonedafinal = "USD";

                            Sheet.Cells[string.Format("E{0}", row)].Value = pedido.total + " " + clavemoneda;
                            if (clavemoneda == clavemonedafinal)
                            {

                                pedido.TC = 1;
                                pedido.TotalDls = pedido.subtotal; /// por que son dolares
                            }
                            else
                            {
                                // voy al tipo de cambio de ese dia
                                string cadenadetipo = "SELECT [dbo].[GetTipocambioCadena] ('" + pedido.fecha.Year + "/" + pedido.fecha.Month + "/" + pedido.fecha.Day + "','" + clavemonedafinal + "','" + clavemoneda + "') as Dato";
                                Decimal tcdeldia = db.Database.SqlQuery<ClsDatoDecimal>(cadenadetipo).ToList().FirstOrDefault().Dato;

                                pedido.TC = tcdeldia;
                                pedido.TotalDls = pedido.subtotal / tcdeldia; // aqui obtengo la conversion


                            }
                            acumuladodlsvendedor += pedido.TotalDls;
                            string totaldls = pedido.TotalDls.ToString("C");

                            Sheet.Cells[string.Format("F{0}", row)].Value = pedido.status;
                            Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("G{0}", row)].Value = pedido.TC;
                            Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("H{0}", row)].Value = totaldls;
                            row++;

                            acumulasubtotalxcliente += pedido.subtotal;
                            acumulaivaxcliente += pedido.IVA;
                            acumuatotalxcliente += pedido.total;

                            acumuladlsxcliente += pedido.TotalDls;


                            List<detVen> detpedidos = new List<detVen>();
                            try
                            {
                                string cadena3 = "SELECT d.IDPEdido, a.Cref, d.Cantidad, d.suministro from detpedido as d  inner join encpedido as e on d.IDPedido=e.IDpedido inner join articulo as a on d.idarticulo=a.idarticulo and e.IDpedido=" + pedido.IdPedido + " and e.status<>'cancelado'";
                                detpedidos = db.Database.SqlQuery<detVen>(cadena3).ToList();
                            }
                            catch (SqlException err)
                            {
                                string mensajedeerror = err.Message;
                            }
                            foreach (detVen dpedido in detpedidos)
                            {
                                Sheet.Cells[string.Format("A{0}", row)].Value = "Clave: " + dpedido.Cref;
                                Sheet.Cells[string.Format("B{0}", row)].Value = "Cantidad: " + dpedido.cantidad;
                                Sheet.Cells[string.Format("C{0}", row)].Value = "Suministro: " + dpedido.suministro;
                                row++;

                            }
                        
                        }
                        decimal subtotalacu = Math.Round(acumulasubtotalxcliente, 2); // COn esto le indico que lo comverta a formato meneda y luego a string para que los millares a aparezcan separados por coma
                        decimal ivaacu = Math.Round(acumulaivaxcliente, 2);
                        decimal totalacu = Math.Round(acumuatotalxcliente, 2);
                        decimal totaldslacu = Math.Round(acumuladlsxcliente, 2);
                        string c = "A" + row + ":H" + row;
                        Sheet.Cells[c].Style.Font.Bold = true;
                        Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("E{0}", row)].Value = "Importe Subtotal: $" + subtotalacu;
                        Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("F{0}", row)].Value = "Importe IVA: $" + ivaacu;
                        Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("G{0}", row)].Value = "Importe Total: $" + totalacu;
                        Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("H{0}", row)].Value = "Importe Total USD: $" + totaldslacu;
                        row++;


                    }

                    Sheet.Cells.Style.Font.Size = 15;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Font.Bold = true;
                    Sheet.Cells[string.Format("G{0}", row)].Value = "TOTAL USD VENDEDOR: ";
                    Sheet.Cells[string.Format("H{0}", row)].Style.Font.Bold = true;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = acumuladodlsvendedor;
                    row++;
                }

                //Se genera el archivo y se descarga
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "PedidosAnoVsAnterior.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();

            }


            return View();
        }


        public ActionResult EntreFechasV()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EntreFechasV(ReportePedidosDlsVen modelo, FormCollection coleccion )
        {
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();
            DateTime fecAct = DateTime.Today;
            string FA = fecAct.ToString("dd/MM/yyyy");
            string cual = coleccion.Get("Enviar");

            string cadena = "";
            if (cual == "Generar reporte pdf")
            {
                ReportePedidosDlsVen report = new ReportePedidosDlsVen();
                //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"),DateTime.Parse( "2019-07-30"));
                byte[] abytes = report.PrepareReport(modelo.fechaini, modelo.fechafin);
                return File(abytes, "application/pdf");
              
            }
            List<VendedorPedidoDls> vendedores = new List<VendedorPedidoDls>();
            if (cual == "Generar excel")
            {
               
                try
                {
                     cadena = "select  distinct E.IDVendedor, V.Nombre, V.CuotaVendedor, M.ClaveMoneda from [dbo].[EncPedido] as E inner join [dbo].[Vendedor] as V on E.IDVendedor = V.IDVendedor inner join [dbo].[c_Moneda] as M on V.IDMoneda = M.IDMoneda where fecha>='"+ FI + " 00:00:01' and fecha <='"+ FF + " 23:59:59' and e.status<>'cancelado' order by v.nombre";
                    vendedores = db.Database.SqlQuery<VendedorPedidoDls>(cadena).ToList();
                }
                catch (SqlException err)
                {
                    string mensajedeerror = err.Message;
                }


                decimal acumuladodlsvendedor = 0M;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Pedidos en Dólares por Vendedor");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:I1"].Style.Font.Size = 20;
                Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:I3"].Style.Font.Bold = true;
                Sheet.Cells["A1:I1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Pedidos en Dólares por Vendedor");

                row = 2;
                Sheet.Cells["A1:I1"].Style.Font.Size = 12;
                Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:I2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha Reporte";
                Sheet.Cells[string.Format("C2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C2", row)].Value = fecAct;

                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:I3"].Style.Font.Bold = true;
                Sheet.Cells["A3:I3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:I3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                Sheet.Cells["C3:H3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;


                row = 4;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A4:I4"].Style.Font.Bold = true;
                Sheet.Cells["A4:I4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                Sheet.Cells["C4:E4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleGoldenrod);
                Sheet.Cells["F4:H4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PowderBlue);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A4"].RichText.Add("Fecha");
                Sheet.Cells["B4"].RichText.Add("Pedido");
                Sheet.Cells["C4"].RichText.Add("Subtotal");
                Sheet.Cells["D4"].RichText.Add("IVA");
                Sheet.Cells["E4"].RichText.Add("Total");
                Sheet.Cells["F4"].RichText.Add("Status");
                Sheet.Cells["G4"].RichText.Add("TC");
                Sheet.Cells["H4"].RichText.Add("Total can");
                //Sheet.Cells["I4"].RichText.Add("TotalMXN(AñoActual - AñoAnterior)");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A4:H4"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 5;
                Sheet.Cells.Style.Font.Bold = false;
                
                foreach (VendedorPedidoDls item in vendedores)
                {
                    string concatecaE = "A" + row + ":I" + row;
                    Sheet.Cells[concatecaE].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[concatecaE].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    Sheet.Cells[concatecaE].Style.Font.Bold = true;

                    Sheet.Cells[string.Format("A{0}", row)].Style.Font.Bold = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "ID Vendedor: " + item.IDVendedor;
                    Sheet.Cells[string.Format("B{0}", row)].Value =  item.Nombre;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("C{0}", row)].Value = "Cuota: "+item.CuotaVendedor;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveMoneda;
                   
                    row++;
                    List<ClientesPedidoDls> clientes = new List<ClientesPedidoDls>();

                    try
                    {
                        string cadena1 = "Select distinct P.IDCliente,  C.Nombre, C.Telefono from dbo.EncPedido as P inner join Clientes as C on P.IDCliente = C.IDCliente where P.IDVendedor= " + item.IDVendedor + " and fecha>='" + FI + " 00:00:01' and fecha <='" + FF + " 23:59:59' and p.status<>'cancelado' order by C.Nombre";
                        clientes = db.Database.SqlQuery<ClientesPedidoDls>(cadena1).ToList();
                    }
                    catch (SqlException err)
                    {
                        string mensajedeerror = err.Message;
                    }
                    foreach (ClientesPedidoDls cliente in clientes)
                    {
                        string concateca = "A" + row + ":I" + row;
                        Sheet.Cells[concateca].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[concateca].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                        Sheet.Cells[concateca].Style.Font.Bold = true;

                        Sheet.Cells[string.Format("A{0}", row)].Value = cliente.IDCliente;
                        Sheet.Cells[string.Format("B{0}", row)].Value = cliente.Nombre;
                        
                        Sheet.Cells[string.Format("C{0}", row)].Value = cliente.Telefono;

                        row++;

                        List<ReportePedidoDlsVen> pedidos = new List<ReportePedidoDlsVen>();
                        try
                        {
                            string cadena2 = "SELECT P.idPedido, p.fecha,p.subtotal, p.iva, p.total, p.IDMoneda, CAST(1 AS DECIMAL(8,2)) as TC, p.status, CAST(0 AS DECIMAL(8,2)) as TotalDls FROM clientes AS c INNER JOIN encPedido AS p ON c.IDCliente=p.idcliente where p.idvendedor= " + item.IDVendedor + " and p.IdCliente=" + cliente.IDCliente + " and fecha>='" +FI+ " 00:00:01' and fecha <='" + FF + " 23:59:59' and p.status <>'cancelado' order by p.fecha";
                            pedidos = db.Database.SqlQuery<ReportePedidoDlsVen>(cadena2).ToList();
                        }
                        catch (SqlException err)
                        {
                            string mensajedeerror = err.Message;
                        }
                        decimal acumulasubtotalxcliente = 0;
                        decimal acumulaivaxcliente = 0;
                        decimal acumuatotalxcliente = 0;
                        decimal acumuladlsxcliente = 0;

                        foreach (ReportePedidoDlsVen pedido in pedidos)
                        {
                            if (pedido.status == "Cancelado")
                            {
                                pedido.subtotal = 0;
                                pedido.IVA = 0;
                                pedido.total = 0;

                            }

                            string clavemoneda = new c_MonedaContext().c_Monedas.Find(pedido.IDMoneda).ClaveMoneda; // esta es la moneda del pedido
                            string clavemonedafinal = "USD";

                            // voy a compara si la meneda el pedido viene en dolares o en otra moneda si viene en otra moneda el totalen dolares vendra del tipo de cambio de ese dia

                            if (clavemoneda == clavemonedafinal)
                            {

                                pedido.TC = 1;
                                pedido.TotalDls = pedido.subtotal; /// por que son dolares
                            }
                            else
                            {
                                // voy al tipo de cambio de ese dia
                                string cadenadetipo = "SELECT [dbo].[GetTipocambioCadena] ('" + pedido.fecha.Year + "/" + pedido.fecha.Month + "/" + pedido.fecha.Day + "','" + clavemonedafinal + "','" + clavemoneda + "') as Dato";
                                Decimal tcdeldia = db.Database.SqlQuery<ClsDatoDecimal>(cadenadetipo).ToList().FirstOrDefault().Dato;

                                pedido.TC = tcdeldia;
                                pedido.TotalDls = pedido.subtotal / tcdeldia; // aqui obtengo la conversion


                            }
                            acumuladodlsvendedor += pedido.TotalDls;
                            string totaldls = pedido.TotalDls.ToString("C");

                            Sheet.Cells[string.Format("A{0}", row)].Style.Numberformat.Format = "yyyy-mm-dd";
                            Sheet.Cells[string.Format("A{0}", row)].Value = pedido.fecha;
                            Sheet.Cells[string.Format("B{0}", row)].Value = pedido.IdPedido;
                            Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("C{0}", row)].Value = pedido.subtotal;
                            Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("D{0}", row)].Value = pedido.IVA;
                            Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                           
                            Sheet.Cells[string.Format("F{0}", row)].Value = pedido.status;
                            Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("G{0}", row)].Value = pedido.TC;
                            Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("H{0}", row)].Value = totaldls;
                            row++;

                            acumulasubtotalxcliente += pedido.subtotal;
                            acumulaivaxcliente += pedido.IVA;
                            acumuatotalxcliente += pedido.total;

                            acumuladlsxcliente += pedido.TotalDls;

                            List<detpedidoReporte> detpedidos = new List<detpedidoReporte>();
                            try
                            {
                                string cadena3 = "SELECT d.IDPEdido, a.Cref, d.Cantidad, d.suministro from detpedido as d  inner join encpedido as e on d.IDPedido=e.IDpedido inner join articulo as a on d.idarticulo=a.idarticulo and e.IDpedido=" + pedido.IdPedido + " and e.status<>'cancelado'";
                                detpedidos = db.Database.SqlQuery<detpedidoReporte>(cadena3).ToList();
                            }
                            catch (SqlException err)
                            {
                                string mensajedeerror = err.Message;
                            }
                            foreach (detpedidoReporte dpedido in detpedidos)
                            {

                                Sheet.Cells[string.Format("A{0}", row)].Value = "Clave: "+dpedido.Cref;
                                Sheet.Cells[string.Format("B{0}", row)].Value = "Cantidad: "+dpedido.cantidad;
                                Sheet.Cells[string.Format("C{0}", row)].Value = "Suministro: "+dpedido.suministro;
                                row++;

                            }
                           
                        }
                        decimal subtotalacu = Math.Round(acumulasubtotalxcliente, 2); // COn esto le indico que lo comverta a formato meneda y luego a string para que los millares a aparezcan separados por coma
                        decimal ivaacu = Math.Round(acumulaivaxcliente, 2);
                        decimal totalacu = Math.Round(acumuatotalxcliente, 2);
                        decimal totaldslacu = Math.Round(acumuladlsxcliente, 2);

                        string c = "A" + row + ":H" + row;
                        Sheet.Cells[c].Style.Font.Bold = true;
                        Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("E{0}", row)].Value = "Importe Subtotal: $" + subtotalacu;
                        Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("F{0}", row)].Value = "Importe IVA: $" + ivaacu;
                        Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("G{0}", row)].Value = "Importe Total: $" + totalacu;
                        Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("H{0}", row)].Value = "Importe Total USD: $" + totaldslacu;
                        row++;


                        

                    }
                    Sheet.Cells.Style.Font.Size = 15;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Font.Bold = true;
                    Sheet.Cells[string.Format("G{0}", row)].Value = "TOTAL USD VENDEDOR: ";
                    Sheet.Cells[string.Format("H{0}", row)].Style.Font.Bold = true;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = acumuladodlsvendedor;
                    row++;
                    acumuladodlsvendedor = 0;
                }

                //Se genera el archivo y se descarga
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "PedidosAnoVsAnterior.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();

            }

            return View();
        }
     
        public ActionResult EntreFechasPed()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasPed(EFecha modelo, FormCollection coleccion)
        {
            VPedidosContext dbe = new VPedidosContext();
            VPedidoDetContext dbr = new VPedidoDetContext();
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

                cadena = "select * from dbo.VPedidos where fecha >= '" + FI + "' and fecha  <='" + FF + "' ";
                var datos = dbe.Database.SqlQuery<VPedidos>(cadena).ToList();
                ViewBag.datos = datos;
                cadenaDet = "select * from [dbo].[VPedidoDet] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' ";
                var datosDet = dbr.Database.SqlQuery<VPedidoDet>(cadenaDet).ToList();
                ViewBag.datosDet = datosDet;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Pedidos");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:U1"].Style.Font.Size = 20;
                Sheet.Cells["A1:U1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:U3"].Style.Font.Bold = true;
                Sheet.Cells["A1:U1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:U1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Pedidos");

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
                Sheet.Cells["A3"].RichText.Add("ID Pedido");
                Sheet.Cells["B3"].RichText.Add("Orden Compra Cliente");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("Fecha Requiere");
                Sheet.Cells["E3"].RichText.Add("ID Cliente");
                //Sheet.Cells["E3"].RichText.Add("NoExpediente");
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
                Sheet.Cells["V3"].RichText.Add("Entrega");


                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:U3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VPedidos item in ViewBag.datos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDPedido;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.OCompra;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.FechaRequiere;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.IDCliente;
                    //Sheet.Cells[string.Format("E{0}", row)].Value = item.noExpediente;
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
                    Sheet.Cells[string.Format("V{0}", row)].Value = item.Entrega;

                    row++;
                }

                //Hoja No. 2
                Sheet = Ep.Workbook.Worksheets.Add("Detalles");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:R1"].Style.Font.Size = 20;
                Sheet.Cells["A1:R1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:R3"].Style.Font.Bold = true;
                Sheet.Cells["A1:R1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Detalle de Pedidos");

                row = 2;
                Sheet.Cells["A1:R1"].Style.Font.Size = 12;
                Sheet.Cells["A1:R1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:R1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:R2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3:R3"].Style.Font.Bold = true;
                Sheet.Cells["A3:R3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:R3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID Detalle");
                Sheet.Cells["B3"].RichText.Add("ID Pedido");
                Sheet.Cells["C3"].RichText.Add("ID Remision");
                Sheet.Cells["D3"].RichText.Add("ID Prefactura");
                Sheet.Cells["E3"].RichText.Add("Fecha"); ;
                Sheet.Cells["F3"].RichText.Add("Cliente");
                Sheet.Cells["G3"].RichText.Add("Clave");
                Sheet.Cells["H3"].RichText.Add("Artículo");
                Sheet.Cells["I3"].RichText.Add("Presentación");
                Sheet.Cells["J3"].RichText.Add("Cantidad Pedida");
                Sheet.Cells["K3"].RichText.Add("Cantidad");
                Sheet.Cells["L3"].RichText.Add("Costo");
                Sheet.Cells["M3"].RichText.Add("Devolución");
                Sheet.Cells["N3"].RichText.Add("Importe");
                Sheet.Cells["O3"].RichText.Add("IVA");
                Sheet.Cells["P3"].RichText.Add("Total");
                Sheet.Cells["Q3"].RichText.Add("Estado");
                Sheet.Cells["R3"].RichText.Add("Nota");
                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:R3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VPedidoDet itemD in ViewBag.datosDet)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.IDDetPedido;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.IDPedido;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.IDRemision;
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemD.IDPrefactura;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemD.Fecha;
                    Sheet.Cells[string.Format("F{0}", row)].Value = itemD.Cliente;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.Cref;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.Articulo;
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.Presentacion;
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.CantidadPedida;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.Cantidad;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.Costo;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = itemD.Descuento;
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("N{0}", row)].Value = itemD.Importe;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("O{0}", row)].Value = itemD.ImporteIva;
                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("P{0}", row)].Value = itemD.ImporteTotal;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = itemD.Status;
                    Sheet.Cells[string.Format("R{0}", row)].Value = itemD.Nota;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Pedido.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }

        public ActionResult SubirArchivoPed(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult SubirArchivoPed(HttpPostedFileBase file, int id)
        {
            int idP = int.Parse(id.ToString());
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            if (file != null)
            {
                string ruta = Server.MapPath("~/PDFPedidoAdd/");
                ruta += "Ped_" + id + "_" + file.FileName;
                string cad = "insert into  [dbo].[PedidoAdd]([IDPedido], [RutaArchivo], nombreArchivo) Values (" + idP + ", '" + ruta + "','" + "Ped_" + id + "_" + file.FileName + "' )";
                new PedidoAddContext().Database.ExecuteSqlCommand(cad);
                modelo.SubirArchivo(ruta, file);
                ViewBag.Error = modelo.error;
                ViewBag.Correcto = modelo.Confirmacion;
            }
            return RedirectToAction("index", new { searchString = id });
        }

        public ActionResult DescargarPDFPed(int id)
        {
            // Obtener contenido del archivo
            PedidoAddContext dbp = new PedidoAddContext();
            PedidoAdd elemento = dbp.PedidoAdd.Find(id);
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
        }

        public ActionResult EliminarArchivoPed(int id)
        {
            PedidoAddContext dbp = new PedidoAddContext();
            string cadena = "select * from dbo.PedidoAdd where IDPedido= " + id + "";
            var datos = dbp.Database.SqlQuery<PedidoAdd>(cadena).ToList();
            ViewBag.datos = datos;
            return View(datos);
        }

        public FormulaEspecializada.Formulaespecializada igualar(ClsCotizador elemento, FormulaEspecializada.Formulaespecializada formula)
        {
            formula.anchoproductomm = elemento.anchoproductomm;
            formula.largoproductomm = elemento.largoproductomm;
            formula.Cantidad = elemento.Cantidad;
            formula.Cantidadxrollo = elemento.Cantidadxrollo;
            formula.gapeje = elemento.gapeje;
            formula.gapavance = elemento.gapavance;
            formula.Numerodetintas = elemento.Numerodetintas;
            formula.cavidadesdesuaje = elemento.cavidadesdesuajeEje;
            formula.conadhesivo = elemento.conadhesivo;
            formula.hotstamping = elemento.hotstamping;
            formula.CostoM2Cinta = elemento.CostoM2Cinta;
            formula.LargoCinta = elemento.LargoCinta;

            return formula;
        }

        public FormulaEspecializada.Formulaespecializada pasaTintas(List<Tinta> tintas, FormulaEspecializada.Formulaespecializada formula)
        {
            decimal costoacu = 0;
            formula.Tintas.Clear();
            foreach (Tinta tin in tintas)
            {
                FormulaEspecializada.Tinta nueva = new FormulaEspecializada.Tinta();
                nueva.IDTinta = tin.IDTinta;
                nueva.Area = tin.Area;
                nueva.kg = tin.kg;
                costoacu += nueva.Costo;
                formula.Tintas.Add(nueva);
            }
            formula.Costodetintas = costoacu;
            return formula;
        }


        public bool siTermoencongible(int cotizacion)
        {
            ClsCotizador elemento = new ClsCotizador();
            elemento = null;

            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cotizacion);
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            try
            {
                 XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);


                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                string mensajedeerror = er.Message;
                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            }


               
             
            if (elemento.SuajeNuevo)
            {
                if (elemento.IDSuaje==0)
                {
                    elemento.mangatermo = true;
                    return elemento.mangatermo;
                }
                if (elemento.IDMaterial==294)
                {
                    elemento.mangatermo = true;
                    return elemento.mangatermo;

                }
                if (elemento.IDMaterial == 290)
                {
                    elemento.mangatermo = true;
                    return elemento.mangatermo;

                }
            }
            return elemento.mangatermo;


        }

        public bool siTermoencongibleTrans(int cotizacion)
        {
            ClsCotizador elemento = new ClsCotizador();
            elemento = null;

            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cotizacion);
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            try
            {
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);


                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                string mensajedeerror = er.Message;
                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            }

            bool tran = false;
            if (elemento.Tintas.Count() == 0 && elemento.mangatermo)
            {

                tran = true;
            }
            return tran;

        }

        public bool siAdherible(int cotizacion)
        {
            ClsCotizador elemento = new ClsCotizador();
            elemento = null;

            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cotizacion);
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            try
            {
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);


                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                string mensajedeerror = er.Message;
                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            }


            bool tipoetiquetaAdhe = false;
            if (!elemento.mangatermo)
            {
                if (elemento.IDSuaje==0)
                {
                    if (elemento.SuajeNuevo)
                    {
                        tipoetiquetaAdhe = true;
                    }
                    else
                    {
                       
                    }
                }
                else
                {
                    tipoetiquetaAdhe = true;
                }
               
                
            }
            return tipoetiquetaAdhe;



        }


        public Plantilla Modelo7(ClsCotizador elemento)
        {

            Plantilla plantilla = new Plantilla();
            Materiales material = new MaterialesContext().Materiales.Find(elemento.IDMaterial);


            int IDMaterial = verificamaterial(material.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

            ArticuloXML nuevoplaneacion2 = new ArticuloXML();

            int IDMaterialArticulo = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + IDMaterial).FirstOrDefault().Articulo_IDArticulo;

            nuevoplaneacion2.IDArticulo = IDMaterialArticulo.ToString();
            nuevoplaneacion2.IDCaracteristica = IDMaterial.ToString();
            nuevoplaneacion2.IDTipoArticulo = "6";
            nuevoplaneacion2.IDProceso = "5";
            nuevoplaneacion2.Formula = elemento.CantidadMPMts2.ToString();
            nuevoplaneacion2.Indicaciones = "";
            plantilla.Articulos.Add(nuevoplaneacion2);

            /**** proceso sellado *******************/

            ArticuloXML nuevotiemposellado = new ArticuloXML();



            nuevotiemposellado.IDArticulo = 231.ToString();
            nuevotiemposellado.IDCaracteristica = 220.ToString();
            nuevotiemposellado.IDTipoArticulo = "5";
            nuevotiemposellado.IDProceso = "11";
            nuevotiemposellado.Formula = "C*" + Math.Round((elemento.MaterialNecesitado / 7200) / elemento.Cantidad, 6); // 7200 MTS X HRS
            nuevotiemposellado.FactorCierre = "0.15";
            nuevotiemposellado.Indicaciones = "";

            plantilla.Articulos.Add(nuevotiemposellado);

            ArticuloXML nuevamaquinaosellado = new ArticuloXML();

            nuevamaquinaosellado.IDArticulo = 2317.ToString();
            nuevamaquinaosellado.IDCaracteristica = 12325.ToString();
            nuevamaquinaosellado.IDTipoArticulo = "3";
            nuevamaquinaosellado.IDProceso = "11";
            nuevamaquinaosellado.Formula = "C*" + Math.Round((elemento.MaterialNecesitado / 7200) / elemento.Cantidad, 6); // 7200 MTS X HRS
            nuevamaquinaosellado.FactorCierre = "0.15";
            nuevamaquinaosellado.Indicaciones = "";

            plantilla.Articulos.Add(nuevamaquinaosellado);

            /**** proceso inspeccion *******************/

            ArticuloXML nuevotiempoinspeccion = new ArticuloXML();

            nuevotiempoinspeccion.IDArticulo = 232.ToString();
            nuevotiempoinspeccion.IDCaracteristica = 219.ToString();
            nuevotiempoinspeccion.IDTipoArticulo = "5";
            nuevotiempoinspeccion.IDProceso = "12";
            nuevotiempoinspeccion.Formula = "(((" + elemento.Cantidad + "* " + elemento.largoproductomm + ") / 50) / 60 )+0.33";
            nuevotiempoinspeccion.FactorCierre = "0.25";
            nuevotiempoinspeccion.Indicaciones = "";

            plantilla.Articulos.Add(nuevotiempoinspeccion);
            ArticuloXML nuevamaquinainspeccion = new ArticuloXML();

            nuevamaquinainspeccion.IDArticulo = 2684.ToString();
            nuevamaquinainspeccion.IDCaracteristica = 3733.ToString();
            nuevamaquinainspeccion.IDTipoArticulo = "3";
            nuevamaquinainspeccion.IDProceso = "12";
            nuevamaquinainspeccion.Formula = "(((C  * " + elemento.largoproductomm + ") / 50) / 60 )+0.33"; // 7200 MTS X HRS
            nuevamaquinainspeccion.FactorCierre = "0.15";
            nuevamaquinainspeccion.Indicaciones = "";

            plantilla.Articulos.Add(nuevamaquinainspeccion);

            /**** proceso inspeccion *******************/

            ArticuloXML tiempocorte = new ArticuloXML();

            tiempocorte.IDArticulo = 232.ToString();
            tiempocorte.IDCaracteristica = 219.ToString();
            tiempocorte.IDTipoArticulo = "5";
            tiempocorte.IDProceso = "12";
            tiempocorte.Formula = "  ((((" + elemento.Cantidad + " * 1000*" + elemento.largoproductomm + ")) / 13000) / 60) + (((" + elemento.Cantidad * 1000 + ") /" + elemento.Cantidadxrollo + " * 3) / 60"; // 13 MTS * MIN
            tiempocorte.FactorCierre = "0.25";
            tiempocorte.Indicaciones = "";

            plantilla.Articulos.Add(tiempocorte);
            ArticuloXML nuevamaquinacorte = new ArticuloXML();

            nuevamaquinacorte.IDArticulo = 2686.ToString();
            nuevamaquinacorte.IDCaracteristica = 4503.ToString();
            nuevamaquinacorte.IDTipoArticulo = "3";
            nuevamaquinacorte.IDProceso = "16";
            nuevamaquinacorte.Formula = "  ((((C * 1000) * LARGO) / 13000) / 60) + (((C * 1000) / ETIQUETAXR) * 3) / 60"; // 13 MTS * MIN
            nuevamaquinacorte.FactorCierre = "0.15";
            nuevamaquinacorte.Indicaciones = "";


            plantilla.Articulos.Add(nuevamaquinacorte);


            /******* proceso de empaque ******************/

            if (elemento.IDCentro > 0)
            {
                ArticuloXML centros = new ArticuloXML();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCentro).FirstOrDefault();
                centros.IDArticulo = elemento.IDCentro.ToString();

                centros.IDCaracteristica = carac.ID.ToString();

                centros.IDTipoArticulo = "4";
                centros.IDProceso = "4";
                decimal numeroderollos = (elemento.Cantidad * 1000) / elemento.Cantidadxrollo;
                decimal cuantoscaven = 1000M / (elemento.anchoproductomm * elemento.productosalpaso);
                decimal numerodecentros = (numeroderollos / Math.Truncate(cuantoscaven));
                centros.Formula = numerodecentros.ToString(); /// la cantidad de centros entre la cantidad de centros que caben en una pieza de rollo completa)
                centros.FactorCierre = "1";
                centros.Indicaciones = "";

                plantilla.Articulos.Add(centros);

            }

            if (elemento.IDCaja > 0)
            {
                ArticuloXML cajas = new ArticuloXML();



                cajas.IDArticulo = elemento.IDCaja.ToString();

                Caracteristica caracaja = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCaja).FirstOrDefault();
                cajas.IDCaracteristica = caracaja.ID.ToString();
                cajas.IDTipoArticulo = "4";
                cajas.IDProceso = "16";
                int numcajas = int.Parse(Math.Truncate(((elemento.Cantidad / elemento.Minimoproducir) * 0.75M) + 0.5M).ToString());
                cajas.Formula = numcajas.ToString();
                cajas.FactorCierre = "1";
                cajas.Indicaciones = "";

                plantilla.Articulos.Add(cajas);
            }

            return plantilla;


        }
        public Plantilla Modelo4(ClsCotizador elemento )
        {

            
            Plantilla plantilla = new Plantilla();

            /////// proceso prensa suaje materiales y tintas ********************************
            ///////// manode obra y maquina de prensa
            /********************  mano de obra *******************/
            ArticuloXML nuevomanodeobra = new ArticuloXML();

            nuevomanodeobra.IDArticulo = "2916";
            nuevomanodeobra.IDCaracteristica = "3972";
            nuevomanodeobra.Formula = elemento.HrPrensa.ToString();
            nuevomanodeobra.IDTipoArticulo = "5";
            nuevomanodeobra.IDProceso = "5";
            nuevomanodeobra.FactorCierre = "0.166667";

            plantilla.Articulos.Add(nuevomanodeobra);


            /********************************* maquina de obra *********************************/

            if (elemento.IDMaquinaPrensa == 0)
            {

                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = "87";
                nuevomaquina.IDCaracteristica = "32";
                nuevomaquina.Formula = elemento.HrPrensa.ToString();
                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.16667";

                plantilla.Articulos.Add(nuevomaquina);
            }

            else

            {
                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDMaquinaPrensa).FirstOrDefault();
                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = elemento.IDMaquinaPrensa.ToString();
                nuevomaquina.IDCaracteristica = cara.ID.ToString();
                nuevomaquina.Formula = elemento.HrPrensa.ToString();
                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);

            }

            /***************************************  suaje ****************************/

            if (elemento.IDSuaje==0)
            {
                ArticuloXML nuevoplaneacion = new ArticuloXML();

                nuevoplaneacion.IDArticulo = "8674";
                nuevoplaneacion.IDCaracteristica = "9763";
                nuevoplaneacion.IDTipoArticulo = "2";
                nuevoplaneacion.IDProceso = "5";
                nuevoplaneacion.Formula = "1/50";
                nuevoplaneacion.Indicaciones = "Esperando Suaje";

                plantilla.Articulos.Add(nuevoplaneacion);
            }


            if (elemento.IDSuaje > 0)
            {
                    ArticuloXML nuevoplaneacion = new ArticuloXML();

                    Caracteristica CIDSuaje = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + elemento.IDSuaje).ToList().FirstOrDefault();

                    int IDSuaje = CIDSuaje.Articulo_IDArticulo;

                    nuevoplaneacion.IDArticulo = IDSuaje.ToString();
                    nuevoplaneacion.IDCaracteristica = elemento.IDSuaje.ToString();
                    nuevoplaneacion.IDTipoArticulo = "2";
                    nuevoplaneacion.IDProceso = "5";
                    nuevoplaneacion.Formula = "1/50";
                    nuevoplaneacion.Indicaciones = "";

                        plantilla.Articulos.Add(nuevoplaneacion);
            }
            if (elemento.IDSuaje2 > 0)
            {
                ArticuloXML nuevoplaneacion3 = new ArticuloXML();

                Caracteristica CIDSuaje = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + elemento.IDSuaje2).ToList().FirstOrDefault();

                int IDSuaje = CIDSuaje.Articulo_IDArticulo;

                nuevoplaneacion3.IDArticulo = IDSuaje.ToString();
                nuevoplaneacion3.IDCaracteristica = elemento.IDSuaje2.ToString();
                nuevoplaneacion3.IDTipoArticulo = "2";
                nuevoplaneacion3.IDProceso = "5";
                nuevoplaneacion3.Formula = "1/50";
                nuevoplaneacion3.Indicaciones = "";

                plantilla.Articulos.Add(nuevoplaneacion3);
            }


            Materiales material = new MaterialesContext().Materiales.Find(elemento.IDMaterial);


            int IDMaterial = verificamaterial(material.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

            ArticuloXML nuevoplaneacion2 = new ArticuloXML();

            int IDMaterialArticulo = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial).ToList().FirstOrDefault().Articulo_IDArticulo;

            nuevoplaneacion2.IDArticulo = IDMaterialArticulo.ToString();
            nuevoplaneacion2.IDCaracteristica = IDMaterial.ToString();
            nuevoplaneacion2.IDTipoArticulo = "6";
            nuevoplaneacion2.IDProceso = "5";
            nuevoplaneacion2.Formula = elemento.CantidadMPMts2.ToString();
            nuevoplaneacion2.FactorCierre = "0";

            nuevoplaneacion2.Indicaciones = "";

            plantilla.Articulos.Add(nuevoplaneacion2);

           // *************************

            if (elemento.IDMaterial2 > 0)
            {

                ArticuloXML nuevoplaneacion3 = new ArticuloXML();


                Materiales material2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                int IDMaterial2 = verificamaterial(material2.Clave, elemento.anchomaterialenmm, material2.Largo, material2.Fam, material2.Descripcion, material2.Precio);

                int IDMaterialArticulo2 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial2).ToList().FirstOrDefault().Articulo_IDArticulo;

                nuevoplaneacion3.IDArticulo = IDMaterialArticulo2.ToString();
                nuevoplaneacion3.IDCaracteristica = IDMaterial2.ToString();
                nuevoplaneacion3.IDTipoArticulo = "6";
                nuevoplaneacion3.IDProceso = "5";
                nuevoplaneacion3.Formula = elemento.CantidadMPMts2.ToString(); 

                nuevoplaneacion3.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneacion3);
            }
            if (elemento.IDMaterial3 > 0)
            {

                ArticuloXML nuevoplaneacion3 = new ArticuloXML();


                Materiales material2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial3);
                int IDMaterial3 = verificamaterial(material2.Clave, elemento.anchomaterialenmm, material2.Largo, material2.Fam, material2.Descripcion, material2.Precio);

                int IDMaterialArticulo3 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial3).ToList().FirstOrDefault().Articulo_IDArticulo;

                nuevoplaneacion3.IDArticulo = IDMaterialArticulo3.ToString();
                nuevoplaneacion3.IDCaracteristica = IDMaterial3.ToString();
                nuevoplaneacion3.IDTipoArticulo = "6";
                nuevoplaneacion3.IDProceso = "5";
                nuevoplaneacion3.Formula = elemento.CantidadMPMts2.ToString();

                nuevoplaneacion3.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneacion3);
            }

            //////////////////////////////////////////////////////////tintas //////////////////////
            foreach (Tinta tinta in elemento.Tintas)
            {
                ArticuloXML nuevoplaneaciontinta = new ArticuloXML();

                int IDTinta = tinta.IDTinta;



                nuevoplaneaciontinta.IDArticulo = IDTinta.ToString();

                try
                {
                    int IDMpresentaciontin = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDTinta).ToList().FirstOrDefault().ID;

                    nuevoplaneaciontinta.IDCaracteristica = IDMpresentaciontin.ToString();
                    nuevoplaneaciontinta.IDTipoArticulo = "7";
                    nuevoplaneaciontinta.IDProceso = "5";

                    decimal cantidadmillartinta = (elemento.Cantidad / elemento.CantidadMPMts2);

                    nuevoplaneaciontinta.Formula = ""+ elemento.CantidadMPMts2+" / 300";
                    nuevoplaneaciontinta.FactorCierre = "0.125";
                    nuevoplaneaciontinta.Indicaciones = "";

                    plantilla.Articulos.Add(nuevoplaneaciontinta);
                }

                catch (Exception err)
                {
                    string mensajedeerror = err.Message;
                }


            }
            if (elemento.Tintas.Count>0)
            {
                ArticuloXML CYREL = new ArticuloXML();
                CYREL.IDArticulo = "11470";
                CYREL.IDCaracteristica = "12570";
                CYREL.IDTipoArticulo = "4";
                CYREL.IDProceso = "5";
                
                CYREL.Formula = "1";
              
                CYREL.Indicaciones = "";


                plantilla.Articulos.Add(CYREL);

                ArticuloXML RODILLO = new ArticuloXML();
                RODILLO.IDArticulo = "11471";
                RODILLO.IDCaracteristica = "12571";
                RODILLO.IDTipoArticulo = "2";
                RODILLO.IDProceso = "5";
                RODILLO.Formula = "1";
                RODILLO.Indicaciones = "";

                plantilla.Articulos.Add(RODILLO);

            }

            /**** proceso embobinado *******************/
            ArticuloXML nuevotiempoembobinado = new ArticuloXML();


                nuevotiempoembobinado.IDArticulo = 226.ToString();
                nuevotiempoembobinado.IDCaracteristica = 215.ToString();
                nuevotiempoembobinado.IDTipoArticulo = "5";
                nuevotiempoembobinado.IDProceso = "4";
                nuevotiempoembobinado.Formula = elemento.HrEmbobinado.ToString();
                nuevotiempoembobinado.FactorCierre = "0.1666";
                nuevotiempoembobinado.Indicaciones = "";


                plantilla.Articulos.Add(nuevotiempoembobinado);

          
            if (elemento.IDMaquinaEmbobinado == 0)
            {

                ArticuloXML nuevomaquinaenbobinado = new ArticuloXML();

                nuevomaquinaenbobinado.IDArticulo = "91";
                nuevomaquinaenbobinado.IDCaracteristica = "3970";


                nuevomaquinaenbobinado.Formula = elemento.HrEmbobinado.ToString();


                nuevomaquinaenbobinado.IDTipoArticulo = "3";
                nuevomaquinaenbobinado.IDProceso = "4";
                nuevomaquinaenbobinado.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquinaenbobinado);
            }
            else
            {
                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDMaquinaEmbobinado).FirstOrDefault();
                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = elemento.IDMaquinaEmbobinado.ToString();
                nuevomaquina.IDCaracteristica = cara.ID.ToString();


                nuevomaquina.Formula = elemento.HrEmbobinado.ToString();

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "4";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);
            }


            if (elemento.IDCentro > 0)
            {
                ArticuloXML centros = new ArticuloXML();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCentro).FirstOrDefault();
                centros.IDArticulo = elemento.IDCentro.ToString();

                centros.IDCaracteristica = carac.ID.ToString();

                centros.IDTipoArticulo = "4";
                centros.IDProceso = "4";
                decimal numeroderollos = (elemento.Cantidad * 1000) / elemento.Cantidadxrollo;
                decimal cuantoscaven = 1000M / (elemento.anchoproductomm * elemento.productosalpaso);
                decimal numerodecentros = ( numeroderollos / Math.Truncate(cuantoscaven));
                centros.Formula =  numerodecentros.ToString(); /// la cantidad de centros entre la cantidad de centros que caben en una pieza de rollo completa)
                centros.FactorCierre = "1";
                centros.Indicaciones = "";

                plantilla.Articulos.Add(centros);

            }

            if (elemento.IDCaja>0)
            {
                ArticuloXML cajas = new ArticuloXML();



                cajas.IDArticulo = elemento.IDCaja.ToString();

                Caracteristica caracaja = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCaja).FirstOrDefault();
                cajas.IDCaracteristica = caracaja.ID.ToString();
                cajas.IDTipoArticulo = "4";
                cajas.IDProceso = "4";
                int numcajas =int.Parse( Math.Truncate(((elemento.Cantidad / elemento.Minimoproducir) * 0.75M)+0.5M).ToString());
                cajas.Formula = numcajas.ToString();
                cajas.FactorCierre = "1";
                cajas.Indicaciones = "";

                plantilla.Articulos.Add(cajas);
            }



            return plantilla;
        }

        public Plantilla Modelo8(ClsCotizador elemento)
        {

            Plantilla plantilla = new Plantilla();

            ArticuloXML nuevomanodeobra = new ArticuloXML();

            nuevomanodeobra.IDArticulo = "2916";
            nuevomanodeobra.IDCaracteristica = "3972";
            nuevomanodeobra.Formula = elemento.HrPrensa.ToString();
            nuevomanodeobra.IDTipoArticulo = "5";
            nuevomanodeobra.IDProceso = "5";
            nuevomanodeobra.FactorCierre = "0.16667";

            plantilla.Articulos.Add(nuevomanodeobra);


            /*********************************   maquina *********************************/

            if (elemento.IDMaquinaPrensa == 0)
            {


                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = "3558";  // nilpeter uv
                nuevomaquina.IDCaracteristica = "4504";


                nuevomaquina.Formula = elemento.HrPrensa.ToString();
               

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.166667";

                plantilla.Articulos.Add(nuevomaquina);

            }
            else
            {

                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDMaquinaPrensa).FirstOrDefault();
                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = elemento.IDMaquinaPrensa.ToString();
                nuevomaquina.IDCaracteristica = cara.ID.ToString();


                nuevomaquina.Formula = elemento.HrPrensa.ToString();

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);

                }

            /***************************************  suaje ****************************/
            try
            {
                ArticuloXML nuevosuaje = new ArticuloXML();

             

                nuevosuaje.IDArticulo ="8674";
                nuevosuaje.IDCaracteristica = "9763";
                nuevosuaje.IDTipoArticulo = "2";
                nuevosuaje.IDProceso = "5";
                nuevosuaje.Formula = "1/" + elemento.DiluirSuajeEnPedidos;
                nuevosuaje.Indicaciones = "";


                plantilla.Articulos.Add(nuevosuaje);
            }
            catch (Exception err)
            {
                string mensajedeerro = err.Message;

            }

            Materiales material = new MaterialesContext().Materiales.Find(elemento.IDMaterial);


            int IDMaterial = verificamaterial(material.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

            ArticuloXML nuevoplaneacion2 = new ArticuloXML();

            int IDMaterialArticulo = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id="+IDMaterial).FirstOrDefault().Articulo_IDArticulo;

            nuevoplaneacion2.IDArticulo = IDMaterialArticulo.ToString();
            nuevoplaneacion2.IDCaracteristica = IDMaterial.ToString();
            nuevoplaneacion2.IDTipoArticulo = "6";
            nuevoplaneacion2.IDProceso = "5";

            nuevoplaneacion2.Formula = elemento.CantidadMPMts2.ToString();

            nuevoplaneacion2.Indicaciones = "";


            plantilla.Articulos.Add(nuevoplaneacion2);

            foreach (Tinta tinta in elemento.Tintas)
            {
                ArticuloXML nuevoplaneaciontinta = new ArticuloXML();

                int IDTinta = tinta.IDTinta;

                int IDMpresentaciontin = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDTinta).ToList().FirstOrDefault().ID;

                nuevoplaneaciontinta.IDArticulo = IDTinta.ToString();
                nuevoplaneaciontinta.IDCaracteristica = IDMpresentaciontin.ToString();
                nuevoplaneaciontinta.IDTipoArticulo = "7";
                nuevoplaneaciontinta.IDProceso = "5";

                nuevoplaneaciontinta.Formula = "(" + elemento.CantidadMPMts2 + "*" + (tinta.Area/100M) + ")/300";

                nuevoplaneaciontinta.FactorCierre = "0.25";

                nuevoplaneaciontinta.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneaciontinta);


            }
            if (elemento.Tintas.Count > 0)
            {
                ArticuloXML CYREL = new ArticuloXML();
                CYREL.IDArticulo = "11470";
                CYREL.IDCaracteristica = "12570";
                CYREL.IDTipoArticulo = "4";
                CYREL.IDProceso = "5";

                CYREL.Formula="1";

                CYREL.Indicaciones = "";


                plantilla.Articulos.Add(CYREL);

                ArticuloXML RODILLO = new ArticuloXML();
                RODILLO.IDArticulo = "11471";
                RODILLO.IDCaracteristica = "12571";
                RODILLO.IDTipoArticulo = "2";
                RODILLO.IDProceso = "5";

                RODILLO.Formula = "1";

                RODILLO.Indicaciones = "";


                plantilla.Articulos.Add(RODILLO);

            }


            /**** proceso sellado *******************/

            ArticuloXML nuevotiemposellado = new ArticuloXML();


            nuevotiemposellado.IDArticulo = 231.ToString();
            nuevotiemposellado.IDCaracteristica = 220.ToString();
            nuevotiemposellado.IDTipoArticulo = "5";
            nuevotiemposellado.IDProceso = "11";
            nuevotiemposellado.Formula = "C*" + Math.Round((elemento.MaterialNecesitado / 7200) / elemento.Cantidad, 6); // 7200 MTS X HRS
            nuevotiemposellado.FactorCierre = "0.15";
            nuevotiemposellado.Indicaciones = "";


            plantilla.Articulos.Add(nuevotiemposellado);


            ArticuloXML nuevamaquinaosellado = new ArticuloXML();

            nuevamaquinaosellado.IDArticulo = 2317.ToString();
            nuevamaquinaosellado.IDCaracteristica = 12325.ToString();
            nuevamaquinaosellado.IDTipoArticulo = "3";
            nuevamaquinaosellado.IDProceso = "11";
            nuevamaquinaosellado.Formula = "C*" + Math.Round((elemento.MaterialNecesitado / 7200) / elemento.Cantidad, 6); // 7200 MTS X HRS
            nuevamaquinaosellado.FactorCierre = "0.15";
            nuevamaquinaosellado.Indicaciones = "";


            plantilla.Articulos.Add(nuevamaquinaosellado);

            /**** proceso inspeccion *******************/

            ArticuloXML nuevotiempoinspeccion = new ArticuloXML();

            nuevotiempoinspeccion.IDArticulo = 232.ToString();
            nuevotiempoinspeccion.IDCaracteristica = 219.ToString();
            nuevotiempoinspeccion.IDTipoArticulo = "5";
            nuevotiempoinspeccion.IDProceso = "12";
            nuevotiempoinspeccion.Formula = "((("+elemento.Cantidad+"* " + elemento.largoproductomm + ") / 50) / 60 )+0.33";
            nuevotiempoinspeccion.FactorCierre = "0.25";
            nuevotiempoinspeccion.Indicaciones = "";


            plantilla.Articulos.Add(nuevotiempoinspeccion);
            ArticuloXML nuevamaquinainspeccion = new ArticuloXML();

            nuevamaquinainspeccion.IDArticulo = 2684.ToString();
            nuevamaquinainspeccion.IDCaracteristica = 3733.ToString();
            nuevamaquinainspeccion.IDTipoArticulo = "3";
            nuevamaquinainspeccion.IDProceso = "12";
            nuevamaquinainspeccion.Formula = "(((C  * " + elemento.largoproductomm + ") / 50) / 60 )+0.33"; // 7200 MTS X HRS
            nuevamaquinainspeccion.FactorCierre = "0.15";
            nuevamaquinainspeccion.Indicaciones = "";


            plantilla.Articulos.Add(nuevamaquinainspeccion);

            /**** proceso inspeccion *******************/

            ArticuloXML tiempocorte = new ArticuloXML();

            tiempocorte.IDArticulo = 232.ToString();
            tiempocorte.IDCaracteristica = 219.ToString();
            tiempocorte.IDTipoArticulo = "5";
            tiempocorte.IDProceso = "12";
            tiempocorte.Formula = "  (((("+elemento.Cantidad+" * 1000*" + elemento.largoproductomm + ")) / 13000) / 60) + ((("+ elemento.Cantidad * 1000+") /" + elemento.Cantidadxrollo + " * 3) / 60"; // 13 MTS * MIN
            tiempocorte.FactorCierre = "0.25";
            tiempocorte.Indicaciones = "";


            plantilla.Articulos.Add(tiempocorte);
            ArticuloXML nuevamaquinacorte = new ArticuloXML();

            nuevamaquinacorte.IDArticulo = 2686.ToString();
            nuevamaquinacorte.IDCaracteristica = 4503.ToString();
            nuevamaquinacorte.IDTipoArticulo = "3";
            nuevamaquinacorte.IDProceso = "16";
            nuevamaquinacorte.Formula = "  ((((C * 1000) * LARGO) / 13000) / 60) + (((C * 1000) / ETIQUETAXR) * 3) / 60"; // 13 MTS * MIN
            nuevamaquinacorte.FactorCierre = "0.15";
            nuevamaquinacorte.Indicaciones = "";


            plantilla.Articulos.Add(nuevamaquinacorte);


            /******* proceso de empaque ******************/

            if (elemento.IDCentro > 0)
            {
                ArticuloXML centros = new ArticuloXML();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCentro).FirstOrDefault();
                centros.IDArticulo = elemento.IDCentro.ToString();

                centros.IDCaracteristica = carac.ID.ToString();

                centros.IDTipoArticulo = "4";
                centros.IDProceso = "4";
                decimal numeroderollos = (elemento.Cantidad * 1000) / elemento.Cantidadxrollo;
                decimal cuantoscaven = 1000M / (elemento.anchoproductomm * elemento.productosalpaso);
                decimal numerodecentros = (numeroderollos / Math.Truncate(cuantoscaven));
                centros.Formula = numerodecentros.ToString(); /// la cantidad de centros entre la cantidad de centros que caben en una pieza de rollo completa)
                centros.FactorCierre = "1";
                centros.Indicaciones = "";

                plantilla.Articulos.Add(centros);

            }

            if (elemento.IDCaja > 0)
            {
                ArticuloXML cajas = new ArticuloXML();

                cajas.IDArticulo = elemento.IDCaja.ToString();

                Caracteristica caracaja = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCaja).FirstOrDefault();
                cajas.IDCaracteristica = caracaja.ID.ToString();
                cajas.IDTipoArticulo = "4";
                cajas.IDProceso = "16";
                int numcajas = int.Parse(Math.Truncate(((elemento.Cantidad / elemento.Minimoproducir) * 0.75M) + 0.5M).ToString());
                cajas.Formula = numcajas.ToString();
                cajas.FactorCierre = "1";
                cajas.Indicaciones = "";

                plantilla.Articulos.Add(cajas);
            }

            return plantilla;


        }

        /// <summary>
        /// retorna la atipica del material  en su defecto tambien crea el articulo
        /// si no existe lo crea
        /// </summary>
        /// <param name="clave"></param>
        /// <param name="ancho"></param>
        /// <param name="largo"></param>
        /// <returns></returns>
        public int verificamaterial(string clave, int ancho, int largo, string fam, string Material, decimal Costo)
        {
            ArticuloContext db = new ArticuloContext();
            string clavemat = clave + "-" + ancho;

            int IDArticulo = 0;

            List<Articulo> arti = db.Database.SqlQuery<Articulo>("select * from Articulo where cref='" + clavemat + "'").ToList();

            if (arti.Count > 0)
            {
                IDArticulo = arti.FirstOrDefault().IDArticulo;

            }
            else /// no existe el articulo
            {
                string cadenax = "select * from Familia where ccodfam='" + fam + "'";
                int IDFam = db.Database.SqlQuery<Familia>(cadenax).ToList().FirstOrDefault().IDFamilia;
                if (IDFam == 0)
                {
                    IDFam = 56;
                }


                Articulo NuevoArticulo = new Articulo();

                NuevoArticulo.Cref = clavemat;
                NuevoArticulo.IDAQL = 0;
                NuevoArticulo.IDClaveUnidad = 57;
                NuevoArticulo.IDInspeccion = 0;
                NuevoArticulo.IDFamilia = IDFam;
                NuevoArticulo.IDInspeccion = 0;
                NuevoArticulo.IDMoneda = 181;
                NuevoArticulo.IDMuestreo = 0;
                NuevoArticulo.IDTipoArticulo = 6;
                NuevoArticulo.ManejoCar = false;
                NuevoArticulo.nameFoto = "";
                NuevoArticulo.CtrlStock = true;
                NuevoArticulo.GeneraOrden = false;
                NuevoArticulo.esKit = false;
                NuevoArticulo.Descripcion = Material + " " + ancho + " MM X " + largo + " MTS";
                NuevoArticulo.ExistenDev = false;
                NuevoArticulo.obsoleto = false;
                NuevoArticulo.bCodigodebarra = true;


                db.Articulo.Add(NuevoArticulo);
                db.SaveChanges();

                IDArticulo = db.Database.SqlQuery<Articulo>("select * from Articulo where cref='" + clavemat + "'").ToList().FirstOrDefault().IDArticulo;
                try
                {
                    db.Database.ExecuteSqlCommand("delete from MatrizCosto  where IDArticulo=" + IDArticulo);
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + ",0,999999," + Costo + ")");
                }
                catch(Exception err)
                {
                    string Mensaje = err.Message;
                }



            }

            List<Caracteristica> Carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDArticulo + " and Presentacion like '%ANCHO:%" + ancho + "%' AND presentacion like '%LARGO:%" + largo + "%'").ToList();

            if (Carac.Count > 0) // existe la presntacion
            {
                return Carac.FirstOrDefault().ID; // retorna el id de la presentacion existente
            }
            else
            {
                string Presentacion = "ANCHO:" + ancho + ",LARGO:" + largo;
                string jsonPresentacion = "{" + Presentacion + "}";

                int NewIDP = db.Database.SqlQuery<int>("SELECT ISNULL(MAX(IDPRESENTACION)+1,0) from Caracteristica where Articulo_IDArticulo = " + IDArticulo + " ").FirstOrDefault();
                NewIDP = NewIDP > 0 ? NewIDP : 1;
                db.Database.ExecuteSqlCommand("insert into Caracteristica ( IDPresentacion, Cotizacion, version, Presentacion, jsonPresentacion, Articulo_IDArticulo )  values (" + NewIDP + ",0,0,'" + Presentacion + "','" + jsonPresentacion + "'," + IDArticulo + ")");
                
                int retornar = 0;
                retornar = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDArticulo + " and IDPresentacion=" + NewIDP).ToList().FirstOrDefault().ID;
                try
                {
                    db.Database.ExecuteSqlCommand("insert into inventarioAlmacen ( IDAlmacen, IDArticulo, IDCaracteristica, Existencia, PorLlegar,Apartado, Disponibilidad )  values (6," + IDArticulo + "," + retornar + ",0,0,0,0)");
                }
                catch(Exception err)
                {
                    string mensaje = err.Message;
                }
                return retornar;

            }

        }

        public ActionResult EliminarArchivo(int id, PedidoAdd mod)
        {

            PedidoAddContext db = new PedidoAddContext();
            List<SelectListItem> docto = new List<SelectListItem>();
            ClsDatoEntero contard = db.Database.SqlQuery<ClsDatoEntero>("select count(ID) as dato from dbo.PedidoAdd where ID= " + id + "").ToList().FirstOrDefault();
            int pedido = 0;
            if (contard.Dato != 0)
            {
                var elemento = db.PedidoAdd.Single(m => m.ID == id);
                pedido = elemento.IDPedido;

                string cad = "delete from dbo.PedidoAdd where ID= " + elemento.ID + "";
                new PedidoAddContext().Database.ExecuteSqlCommand(cad);

            }

            return RedirectToAction("index", new { searchString = pedido });
        }

        public ActionResult CreateEmpaque (int IDPedido)
        {
            EncPedido pedido = new PedidoContext().EncPedidos.Find(IDPedido);
            EncPack existe = new EncPackContext().Database.SqlQuery<EncPack>("select * from encPack where IDPedido=" + IDPedido).FirstOrDefault();
            ViewBag.Pedido = pedido;
            Clientes cliente = new ClientesContext().Clientes.Find(pedido.IDCliente);
            ViewBag.Cliente = cliente;
            int version = 1;
            if (existe!=null)
            {
                version = existe.Version+1;
            }

         
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            List<SIAAPI.Models.Comercial.VPedidoDet> empacados = null;
            try 
            {
                new EncPackContext().Database.ExecuteSqlCommand("drop table dbo.temporalempaque" + usuario);

            }
            catch(Exception err)
            {
                string mensaje = err.Message;
            }

            try
            {
                string cadenaxz = "SELECT * into dbo.temporalempaque" + usuario + " FROM VPedidoDet where idpedido =" + IDPedido + " ";
                new EncPackContext().Database.ExecuteSqlCommand(cadenaxz);

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            try
            {
                 empacados = new PedidoContext().Database.SqlQuery<SIAAPI.Models.Comercial.VPedidoDet>("select * from dbo.temporalempaque" + usuario).ToList();
                ViewBag.Numero = empacados.Count();
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                ViewBag.Numero = 0;
            }

            ViewBag.Empacados = empacados;

            EncPack nuevoempaque = new EncPack();
            nuevoempaque.idPedido = IDPedido;
            nuevoempaque.Fecha = DateTime.Now;
            nuevoempaque.Cliente = cliente.Nombre;
            nuevoempaque.status = "SOLICITADO";
            nuevoempaque.Idusuario = usuario;
            nuevoempaque.Version = version;
            return View(nuevoempaque);

        }

        [HttpPost]
        public ActionResult CreateEmpaque(EncPack elemento)
        {
            
            try
            {
                EncPackContext db = new EncPackContext();

                String cadenainsert = "INSERT INTO ENCPACK (idpedido,version,fecha,Cliente,status,chofer,cajas,Repartidor,Idusuario,paquetes,observa) values";
                cadenainsert += "  (" + elemento.idPedido + "," + elemento.Version + ",SYSDATETIME()   ,'" + elemento.Cliente + "','" + elemento.status + "'";
                cadenainsert += ",'',0,''," + elemento.Idusuario + ",0,'"+ elemento.observa +"')";
                db.Database.ExecuteSqlCommand(cadenainsert);


                int maximo = db.Database.SqlQuery<ClsDatoEntero>(" select max(idencpack) as Dato from encpack").FirstOrDefault().Dato;

                List<SIAAPI.Models.Comercial.VPedidoDet> empacar = db.Database.SqlQuery<SIAAPI.Models.Comercial.VPedidoDet>("select * from temporalempaque"+ elemento.Idusuario).ToList();

                foreach (VPedidoDet detalle in empacar)
                {
                    try
                    {
                        Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + detalle.Caracteristica_ID).FirstOrDefault();
                        string cadenainsedet = " insert into detpack (iddetpedido,idencpack,version,cref,np,Descripcion,Cantidad, Lote,LoteMP,Serie,Pedimento, CantEmp, IDOrden,Observacion,Estado,Cajas,Paquetes,Kilos) values (";

                        cadenainsedet += detalle.IDDetPedido + "," + maximo + "," + elemento.Version + ",'" + detalle.Cref + "'," + cara.IDPresentacion + ",'" + detalle.Articulo + "'," + detalle.Cantidad + ",'','','','',0,0,'','Solicitado',0,0,0)";



                        db.Database.ExecuteSqlCommand(cadenainsedet);
                    }
                    catch(Exception errr)
                    {
                        string mensaje = errr.Message;
                    }
                }

                return RedirectToAction("Index", "EncPedido", new { searchString = elemento.idPedido });
            }
            catch(Exception err)
            {
                return RedirectToAction("Index", "EncPedido");
            }
        }


        public void PedidosAnoVsAnterior()
        {
            PedidoContext db = new PedidoContext();
            ClsDatoEntero anoactual = db.Database.SqlQuery<ClsDatoEntero>("select year(getdate()) as Dato").ToList()[0];
            ClsDatoEntero anoanterior = db.Database.SqlQuery<ClsDatoEntero>("select (year(getdate())-1) as Dato").ToList()[0];
            ClsDatoEntero mesact = db.Database.SqlQuery<ClsDatoEntero>("select (month(getdate())+1) as Dato").ToList()[0];
            var cadEjecini = "update DiferenciaPed set AnoActualMXN = 0, AnoActualUSD = 0, AnoActualTotMXN = 0, AnoAnteriorMXN = 0, AnoAnteriorUSD = 0, AnoAnteriorTotMXN = 0";
            db.Database.ExecuteSqlCommand(cadEjecini);

            DateTime fecAct = DateTime.Today;
            string FA = fecAct.ToString("dd/MM/yyyy");

            List<c_Moneda> monedamxn = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int mxn = monedamxn.Select(s => s.IDMoneda).FirstOrDefault();
            List<c_Moneda> monedausd = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='USD'").ToList();
            int usd = monedausd.Select(s => s.IDMoneda).FirstOrDefault();
            string cadena = "";
            decimal AcMXN = 0;
            decimal AcUSD = 0;
            decimal AcTot = 0;
            decimal AnMXN = 0;
            decimal AnUSD = 0;
            decimal AnTot = 0;

            //Actualizo la tabla con los datos de los totales de la suma de los pedidos del año actual y el año anterior
            int n = 1;

            while (n < 13)
            {
                //Año actual
                ClsDatoEntero ActExisten = db.Database.SqlQuery<ClsDatoEntero>("select count(*) as Dato from encPedido where year(Fecha) = " + anoactual.Dato + " and month(Fecha)  = " + n + "").ToList()[0];
                if (ActExisten.Dato != 0)
                {
                    var cadMXNAct = "select case WHEN sum(Total) IS NULL THEN (0) ELSE sum(Total) END AS Dato from encpedido where (Status != 'Inactivo' and Status != 'Cancelado') and year(Fecha) = " + anoactual.Dato + " and month(Fecha)  = " + n + " and IDMoneda = " + mxn + "";
                    var cadUSDAct = "select case WHEN sum(Total) IS NULL THEN (0) ELSE sum(Total) END AS Dato from encpedido where (Status != 'Inactivo' and Status != 'Cancelado') and year(Fecha) = " + anoactual.Dato + " and month(Fecha)  = " + n + " and IDMoneda = " + usd + "";
                    var cadtotAct = "select case WHEN sum(Total*TipoCambio) IS NULL THEN (0) ELSE sum(Total*TipoCambio) END AS Dato from encpedido where (Status != 'Inactivo' and Status != 'Cancelado') and year(Fecha) = " + anoactual.Dato + " and month(Fecha)  = " + n + " ";
                    ClsDatoDecimal ActMXN = db.Database.SqlQuery<ClsDatoDecimal>(cadMXNAct).ToList().FirstOrDefault();
                    ClsDatoDecimal ActUSD = db.Database.SqlQuery<ClsDatoDecimal>(cadUSDAct).ToList().FirstOrDefault();
                    ClsDatoDecimal ActTot = db.Database.SqlQuery<ClsDatoDecimal>(cadtotAct).ToList().FirstOrDefault();
                    AcMXN = ActMXN.Dato;
                    AcUSD = ActUSD.Dato;
                    AcTot = ActTot.Dato;
                }
                else
                {
                    AcMXN = 0;
                    AcUSD = 0;
                    AcTot = 0;
                }
                //Año anterior
                ClsDatoEntero AntExisten = db.Database.SqlQuery<ClsDatoEntero>("select count(*) as Dato from encPedido where year(Fecha) = " + anoanterior.Dato + " and month(Fecha)  = " + n + "").ToList()[0];
                if (AntExisten.Dato != 0)
                {
                    var cadMXNAnt = "select case WHEN sum(Total) IS NULL THEN (0) ELSE sum(Total) END AS Dato from encpedido where (Status != 'Inactivo' and Status != 'Cancelado') and year(Fecha) = " + anoanterior.Dato + " and month(Fecha)  = " + n + " and IDMoneda = " + mxn + "";
                    var cadUSDAnt = "select case WHEN sum(Total) IS NULL THEN (0) ELSE sum(Total) END AS Dato from encpedido where (Status != 'Inactivo' and Status != 'Cancelado') and year(Fecha) = " + anoanterior.Dato + " and month(Fecha)  = " + n + " and IDMoneda = " + usd + "";
                    var cadtotAnt = "select case WHEN sum(Total*TipoCambio) IS NULL THEN (0) ELSE sum(Total*TipoCambio) END AS Dato from encpedido where (Status != 'Inactivo' and Status != 'Cancelado') and year(Fecha) = " + anoanterior.Dato + " and month(Fecha)  = " + n + " ";
                    ClsDatoDecimal AntMXN = db.Database.SqlQuery<ClsDatoDecimal>(cadMXNAnt).ToList().FirstOrDefault();
                    ClsDatoDecimal AntUSD = db.Database.SqlQuery<ClsDatoDecimal>(cadUSDAnt).ToList().FirstOrDefault();
                    ClsDatoDecimal AntTot = db.Database.SqlQuery<ClsDatoDecimal>(cadtotAnt).ToList().FirstOrDefault();
                    AnMXN = AntMXN.Dato;
                    AnUSD = AntUSD.Dato;
                    AnTot = AntTot.Dato;
                }
                else
                {
                    AnMXN = 0;
                    AnUSD = 0;
                    AnTot = 0;
                }

                // Se actualizan los valores de la tabla
                var cadEjec = "update DiferenciaPed set AnoActualMXN = " + AcMXN + ", AnoActualUSD = " + AcUSD + ", AnoActualTotMXN = " + AcTot + "";
                cadEjec += ", AnoAnteriorMXN = " + AnMXN + ", AnoAnteriorUSD = " + AnUSD + ", AnoAnteriorTotMXN = " + AnTot + " ";
                cadEjec += "where IDMes = " + n + "";
                db.Database.ExecuteSqlCommand(cadEjec);

               

                n++;
            }
            
            // Cargo los datos actualizados para el reporte
            cadena = "select * from DiferenciaPed ";
            var datos = db.Database.SqlQuery<DiferenciaPed>(cadena).ToList();
            ViewBag.datos = datos;


            ExcelPackage Ep = new ExcelPackage();
            //Crear la hoja y poner el nombre de la pestaña del libro
            var Sheet = Ep.Workbook.Worksheets.Add("Pedidos");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:I1"].Style.Font.Size = 20;
            Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:I3"].Style.Font.Bold = true;
            Sheet.Cells["A1:I1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Pedidos Año Actual vs Año Anterior");

            row = 2;
            Sheet.Cells["A1:I1"].Style.Font.Size = 12;
            Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:I2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado del periodo de datos
            row = 2;
            Sheet.Cells[string.Format("A2", row)].Value = "Fecha Reporte";
            Sheet.Cells[string.Format("C2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("C2", row)].Value = fecAct;

            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:I3"].Style.Font.Bold = true;
            Sheet.Cells["A3:I3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:I3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos. 
            Sheet.Cells["C3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["C3:E3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Goldenrod);
            Sheet.Cells[string.Format("C3", row)].Value = anoactual.Dato;

            Sheet.Cells["F3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["F3:H3"].Style.Font.Color.SetColor(Color.White);
            Sheet.Cells["F3:H3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.MidnightBlue);
            Sheet.Cells[string.Format("F3", row)].Value = anoanterior.Dato;
            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["C3:H3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;


            row = 4;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A4:I4"].Style.Font.Bold = true;
            Sheet.Cells["A4:I4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            Sheet.Cells["C4:E4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleGoldenrod);
            Sheet.Cells["F4:H4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PowderBlue);
            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A4"].RichText.Add("No");
            Sheet.Cells["B4"].RichText.Add("Mes");
            Sheet.Cells["C4"].RichText.Add("MXN");
            Sheet.Cells["D4"].RichText.Add("USD");
            Sheet.Cells["E4"].RichText.Add("Total MXN");
            Sheet.Cells["F4"].RichText.Add("MXN");
            Sheet.Cells["G4"].RichText.Add("USD");
            Sheet.Cells["H4"].RichText.Add("Total MXN");
            Sheet.Cells["I4"].RichText.Add("TotalMXN(AñoActual - AñoAnterior)");

            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A4:I4"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 5;
            Sheet.Cells.Style.Font.Bold = false;
            foreach (DiferenciaPed item in ViewBag.datos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDMes;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Mes;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("C{0}", row)].Value = item.AnoActualMXN;
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("D{0}", row)].Value = item.AnoActualUSD;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.AnoActualTotMXN;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.AnoAnteriorMXN;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("G{0}", row)].Value = item.AnoAnteriorUSD;
                Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("H{0}", row)].Value = item.AnoAnteriorTotMXN;
                Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("I{0}", row)].Value = item.AnoActualTotMXN - item.AnoAnteriorTotMXN;
                row++;
            }

            //Se genera el archivo y se descarga
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "PedidosAnoVsAnterior.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
            //return Redirect("/blah");
        }
        public ActionResult Deleteitemempaque(int id)
        {
            try
            {
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                new PedidoAddContext().Database.ExecuteSqlCommand("delete from dbo.temporalempaque" + usuario + " where iddetpedido="+id);
                return Json(true);
            }
            catch(Exception err)
            {
                string mensaje = err.Message;
                return Json("bad");
            }

        }



        public ActionResult OrdenesAGenerarP(int id, string Mensaje = "")
        {
            Articulo art = new Articulo();
            var cintas = new MaterialRepository().GetCintasMaterial(art.IDArticulo);
            ViewBag.IDMaterial = cintas;

            List<VDetPedido> pedido = new List<VDetPedido>();


            ViewBag.Mensaje = Mensaje;
            string cadena = "select DetPedido.IDDetPedido, DetPedido.IDArticulo, DetPedido.IDAlmacen,Articulo.GeneraOrden,Articulo.Cref as Cref,DetPedido.Suministro,DetPedido.GeneraOrdenP,DetPedido.IDRemision,DetPedido.IDPrefactura,DetPedido.Status,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetPedido.IDDetExterna,DetPedido.IDPedido,Articulo.Descripcion as Articulo,DetPedido.Cantidad,DetPedido.Costo,DetPedido.CantidadPedida,DetPedido.Descuento,DetPedido.Importe,DetPedido.IVA,DetPedido.ImporteIva,DetPedido.ImporteTotal, DetPedido.Nota,DetPedido.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from(DetPedido INNER JOIN Caracteristica ON DetPedido.Caracteristica_ID = Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo)  where detpedido.IDPedido = " + id + "";
            List<VDetPedido> pedidosin = db.Database.SqlQuery<VDetPedido>(cadena).ToList();
            foreach (VDetPedido detalle in pedidosin)
            {

                OrdenProduccion ord = new OrdenProduccionContext().OrdenesProduccion.Where(s => s.IDDetPedido == detalle.IDDetPedido).FirstOrDefault();
                OrdenProduccion ordenP = new OrdenProduccionContext().OrdenesProduccion.Where(s => s.IDDetPedido == detalle.IDDetPedido && s.EstadoOrden != "Cancelada").FirstOrDefault();

                Articulo ar = new ArticuloContext().Articulo.Find(detalle.IDArticulo);
                if (ordenP != null)
                {
                    if (Mensaje != "")
                    {

                    }
                    else
                    {
                        return RedirectToAction("OrdenesAGenerarP", new { id = id, Mensaje = "No se puede generar OP, porque ya tiene una OP, artículo" + ar.Cref });

                    }
                }

                if (ord == null)
                {

                    pedido.Add(detalle);

                }
                try
                {

                    if (detalle.IDDetPedido == ord.IDDetPedido && ord.EstadoOrden.Equals("Cancelada"))
                    {
                        pedido.Add(detalle);
                    }

                }
                catch (Exception err)
                {
                }
            }

            ViewBag.req = pedido;
            return View();
        }
        public ActionResult GeneraOrdenP(FormCollection collection, decimal[] materialNecesitado = null)
        {

            int idpedido = int.Parse(System.Web.HttpContext.Current.Session["idpedido"].ToString());
            var datosOrden = new List<string>();
            datosOrden.Add(collection.Get("Cantidad"));
            datosOrden.Add(collection.Get("iddetpedido"));
            datosOrden.Add(collection.Get("idarticulo"));
            datosOrden.Add(collection.Get("Observacion"));
            datosOrden.Add(collection.Get("IDMaterial"));

            string[] valoresdecantidad = datosOrden[0].Split(',');
            string[] valoresiddetpedido = datosOrden[1].Split(',');
            string[] valoresidarticulo = datosOrden[2].Split(',');
            string[] valoresobservacionarticulo = datosOrden[3].Split(',');
            string[] valoresmaterial = datosOrden[4].Split(',');

            int renglones = valoresdecantidad.Length;

            for (int con = 0; con < renglones; con++)
            {


                decimal cantidad = decimal.Parse(valoresdecantidad[con]); //
                int IDMaterialSeleccionado  = int.Parse(valoresmaterial[con]); //

                if (cantidad > 0)
                {
                    int id = int.Parse(valoresiddetpedido[con]);
                    int idarticulocomp = int.Parse(valoresidarticulo[con]);
                    string indicaciones = valoresobservacionarticulo[con];
                    decimal MaterialNecesitado = materialNecesitado[con];

                    //int idhe = 0;
                    List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                    int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                    DetPedido detpedido = db.DetPedido.Find(id);
                    OrdenProduccion ordenP = new OrdenProduccionContext().OrdenesProduccion.Where(s => s.IDDetPedido == detpedido.IDDetPedido && s.EstadoOrden != "Cancelada").FirstOrDefault();

                    if (ordenP == null)
                    {
                        EncPedido pedido = db.EncPedidos.Find(detpedido.IDPedido);
                        VCaracteristicaContext presentacionBase = new VCaracteristicaContext();
                        Articulo articulos = new ArticuloContext().Articulo.Find(detpedido.IDArticulo);




                        try
                        {
                            //HEspecificacionE hespecificacion = new HEspecificacionEContext().HEspecificacionesE.Find(idhe);

                            automata Automata;

                            XmlSerializer serializer = new XmlSerializer(typeof(automata));

                            string path = Path.Combine(Server.MapPath("~/Automatas/Ordenes.Xml"));


                            using (TextReader reader = new StreamReader(path))
                            {
                                serializer = new XmlSerializer(typeof(automata));
                                Automata = (automata)serializer.Deserialize(reader);
                            }

                            Automata.establecerestadoinicial();

                            string estadoinicial = Automata.Estadoactual.Nombre;

                            int ModelosDeProduccion_IDModeloProduccion = 10;

                            int idprocesoactual = db.Database.SqlQuery<int>("select Proceso_IDProceso from ModeloProceso where ModelosDeProduccion_IDModeloProduccion='" + ModelosDeProduccion_IDModeloProduccion + "' and orden=1").FirstOrDefault();
                            string fecharequiere = pedido.FechaRequiere.ToString("yyyy/MM/dd");
                            string fecharegistro = pedido.Fecha.ToString("yyyy/MM/dd");

                            ArticulosProduccionContext dba = new ArticulosProduccionContext();
                            Articulo articuloaproducion = new ArticuloContext().Articulo.Find(detpedido.IDArticulo);

                            int idordenproduccion = 0;
                            string fecha = "";


                            string cadenaorden = "INSERT INTO [dbo].[OrdenProduccion] ([IDModeloProduccion],[IDCliente],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[Indicaciones],[FechaCompromiso],[FechaInicio],[FechaProgramada],[FechaRealdeInicio],[FechaRealdeTerminacion],[Cantidad],[IDPedido],[IDDetPedido],[Prioridad],[EstadoOrden],[UserID],[Liberar],idhe) VALUES(" +
                                 ModelosDeProduccion_IDModeloProduccion + ",'" + pedido.IDCliente + "','" + detpedido.IDArticulo + "','" + detpedido.Caracteristica_ID + "','" + articuloaproducion.Descripcion + "','" + detpedido.Presentacion + "','" + indicaciones + "','" + fecharequiere + "','" + fecharegistro + "',null,null,null,'" + cantidad + "','" + detpedido.IDPedido + "','" + id + "',0,'" + estadoinicial + "'," + usuario + ",'Activo'," + "0" + ")";
                             db.Database.ExecuteSqlCommand(cadenaorden);
                            idordenproduccion = db.Database.SqlQuery<int>("select max(IDOrden) from OrdenProduccion").FirstOrDefault();

                            fecha = DateTime.Now.ToString("yyyy-MM-dd");

                            DetPedido dpedido = db.Database.SqlQuery<DetPedido>("select * from [dbo].[detPedido] where iddetpedido=" + id).FirstOrDefault();
                            int AlmacenViene = 0;
                            int cara = 0;

                            AlmacenViene = dpedido.IDAlmacen;
                            cara = dpedido.Caracteristica_ID;

                            decimal existencia = 0;
                            decimal llegar = 0;
                            InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == cara).FirstOrDefault();

                            if (ia != null)
                            {

                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set PorLlegar=(PorLlegar+" + cantidad + ") where IDArticulo= " + detpedido.IDArticulo + " and IDCaracteristica=" + cara + " and IDAlmacen=" + AlmacenViene + "");
                                existencia = ia.Existencia;
                            }
                            else
                            {
                                db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                  + AlmacenViene + "," + detpedido.IDArticulo + "," + cara + ",0," + cantidad + ",0,0)");
                                //existencia = 0;
                            }


                            try
                            {
                                ClsDatoEntero ORDEN = new OrdenCompraContext().Database.SqlQuery<ClsDatoEntero>("select idorden as Dato from OrdenProduccion where iddetpedido=" + id).ToList().FirstOrDefault();
                                InventarioAlmacen iai = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == cara).FirstOrDefault();

                                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + cara).ToList().FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Orden Producción'," + (existencia + cantidad) + ",'Orden Producción '," + ORDEN.Dato + ",''," + AlmacenViene + ",'NA'," + iai.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()), " + usuario + ")";
                                //db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }

                            var lista = new VistaModeloProcesoContext().Database.SqlQuery<VistaModeloProceso>("Select * from [VModeloProceso] where idModeloProduccion=" + ModelosDeProduccion_IDModeloProduccion + " order by orden").ToList();
                            ViewBag.listaprocesos = lista;



                            for (int i = 0; i < lista.Count(); i++)
                            {

                               db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[OrdenProduccionDetalle] ([IDOrden],[IDProceso],[EstadoProceso]) VALUES ('" + idordenproduccion + "','" + ViewBag.listaprocesos[i].IDProceso + "','Conflicto')");

                            }

                            decimal tc = db.Database.SqlQuery<ClsDatoDecimal>("select dbo.GetTipocambio(GETDATE(),(select idMoneda from C_MONEDA WHERE clavemoneda='USD'),(select idMoneda from C_MONEDA WHERE clavemoneda='MXN')  )  as Dato").ToList()[0].Dato;

                            string queonda = CrearOrdenP(cantidad, IDMaterialSeleccionado, MaterialNecesitado, idordenproduccion, tc, detpedido.Articulo.IDMoneda, ModelosDeProduccion_IDModeloProduccion);



                        }
                        catch (Exception err)
                        {
                            string mensaje = err.Message;
                            return Json(new { errorMessage = "El producto tuvo un error " + articulos.Cref + " ->" + err.Message });
                        }
                    }

                }
            }
            return RedirectToAction("Details", new { id = idpedido, page = 1 });

        }
        public string CrearOrdenP(decimal cantidad,int IDMaterialSeleccionado, decimal MaterialNecesitado, int ordenproduccion, decimal TC, int IDMoneda, int modelo) // cfreamos mensajes que devuelve la rutina
        {

            string mensajeerror = string.Empty;




            Plantilla articulosdeplantilla = null;


            ArticuloXML nuevamaquinaP = new ArticuloXML();

            nuevamaquinaP.IDArticulo = 14797.ToString();
            nuevamaquinaP.IDCaracteristica = 16043.ToString();
            nuevamaquinaP.IDTipoArticulo = "3";
            nuevamaquinaP.IDProceso = "17";
            nuevamaquinaP.Formula = "C*" + Math.Round((MaterialNecesitado / 7200) / cantidad, 6); // 7200 MTS X HRS
            nuevamaquinaP.FactorCierre = "0.15";
            nuevamaquinaP.Indicaciones = "";

            Caracteristica caracteristicamaquina = new ArticuloContext().Database.SqlQuery<Caracteristica>("select*from Caracteristica where id="+ nuevamaquinaP.IDCaracteristica).ToList().FirstOrDefault();
            Articulo articuloMaquina = new ArticuloContext().Database.SqlQuery<Articulo>("select*from Articulo where idarticulo="+ caracteristicamaquina.Articulo_IDArticulo).ToList().FirstOrDefault(); 
             Articulo articuloMaterial = new ArticuloContext().Articulo.Find(IDMaterialSeleccionado);
            Caracteristica caracteristicaMaterial = new ArticuloContext().Database.SqlQuery<Caracteristica>("select*from Caracteristica where Articulo_IDArticulo=" + IDMaterialSeleccionado + " and obsoleto='0'").ToList().FirstOrDefault();

            try
            {
                   //INSERT MAQUINA
                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]([IDHE],[IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],TC,TCR,[Existe]) VALUES('"
                                                                        + caracteristicamaquina.IDCotizacion + "','" + caracteristicamaquina.Articulo_IDArticulo + "','" + articuloMaquina.IDTipoArticulo + "','" + caracteristicamaquina.ID + "','"+nuevamaquinaP.IDProceso+"','" + ordenproduccion + "'," + 0 + ",'" + articuloMaquina.IDClaveUnidad + "','',0,0,0,0,'1')");
                /////  INSERT MATERIAL 
                 db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]([IDHE],[IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],TC,TCR,[Existe]) VALUES('"
                + caracteristicaMaterial.IDCotizacion + "','" + caracteristicaMaterial.Articulo_IDArticulo + "','" + articuloMaterial.IDTipoArticulo + "','" + caracteristicaMaterial.ID + "','" + nuevamaquinaP.IDProceso + "','" + ordenproduccion + "'," + MaterialNecesitado + ",'" + articuloMaterial.IDClaveUnidad + "','',0,0,0,0,'0')");


            }
            catch (Exception err)
                {
                    string mensaje = err.Message;
                }
            
            // fin del for


            return mensajeerror;


        }
        public JsonResult getarticulosblandoP(string buscar)
        {

            var Articulos = new ArticuloContext().Articulo.Where(s => s.Descripcion.Contains(buscar)).OrderBy(S => S.Descripcion);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Articulo art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Descripcion;
                elemento.Value = art.IDArticulo.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }


    }

    public class MaterialRepository
    {
        public IEnumerable<SelectListItem> GetCintasMaterial(int seleccionado)
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select*from Articulo where IDTipoArticulo=6 and obsoleto='0'";
                List<Articulo> listadecintas = context.Database.SqlQuery<Articulo>(cadena).ToList();

                List<SelectListItem> listadecintacombo = new List<SelectListItem>();
                foreach (Articulo item in listadecintas)
                {
                    SelectListItem art = new SelectListItem { Text = item.Descripcion, Value = item.IDArticulo.ToString() };
                    if (item.IDArticulo == seleccionado)
                    {
                        art.Selected = true;
                    }
                    listadecintacombo.Add(art);
                }

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Cinta  ---"
                };
                listadecintacombo.Insert(0, descripciontip);
                return new SelectList(listadecintacombo, "Value", "Text");
            }

        }
    }
    public static class StringExtensiones
    {
        internal static XmlReader ToXmlReader(this string value)
        {
            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, IgnoreWhitespace = true, IgnoreComments = true };

            var xmlReader = XmlReader.Create(new StringReader(value), settings);
            xmlReader.Read();
            return xmlReader;
        }
    }

   
}


