
using PagedList;
using SIAAPI.Facturas;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

using System.Web.Mvc;
using System.Web.UI;
using System.Xml;

namespace SIAAPI.Controllers.Comercial
{
 [Authorize(Roles = "Administrador,Facturacion,Ventas,Sistemas,Almacenista,Comercial,Gerencia")]
    public class PrefacturaController : Controller
    {
        PrefacturaContext BD = new PrefacturaContext();
        public ActionResult Index(string SerieFac, string ClieFac,string Divisa, string Status, string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
           
            //Buscar Serie Factura
            var SerieLst = new List<string>();
            var SerieQry = from d in BD.EncPrefactura
                         orderby d.Serie
                         select d.Serie;
            SerieLst.Add(" ");
            SerieLst.AddRange(SerieQry.Distinct());
            ViewBag.SerieFac = new SelectList(SerieLst);
          

            //Buscar Cliente
            var ClieLst = new List<string>();
            var ClieQry = from d in BD.EncPrefactura
                          orderby d.Clientes.Nombre
                          select d.Clientes.Nombre;
            ClieLst.Add(" ");
            ClieLst.AddRange(ClieQry.Distinct());
            ViewBag.ClieFac = new SelectList(ClieLst);




            var SerLst = new List<string>();
            var SerQry = from d in BD.c_Monedas
                         orderby d.IDMoneda
                         select d.ClaveMoneda;
            SerLst.Add(" ");
            SerLst.AddRange(SerQry.Distinct());
            ViewBag.Divisa = new SelectList(SerLst);

            var StaLst = new List<string>();
            var StaQry = from d in BD.EncPrefactura
                         orderby d.IDPrefactura
                         select d.Status;
            StaLst.Add(" ");
            StaLst.AddRange(StaQry.Distinct());
            ViewBag.Status = new SelectList(StaLst);

            string ConsultaSql = "Select top 200 e.* from dbo.encprefactura as e inner join c_Moneda as cm on cm.idmoneda=e.idmoneda inner join  Clientes as c on c.idcliente=e.idcliente";
            string cadenaSQl = string.Empty;
            string Filtro = "";

            //var elementos = (from s in BD.EncPrefactura
            //                 select s).OrderByDescending(s => s.IDPrefactura);

            ////elementos = elementos.OrderByDescending(s => s.IDOrdenCompra);

            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPrefactura.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPrefactura where [Status]<>'Cancelado' group by EncPrefactura.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;


            if (!String.IsNullOrEmpty(searchString))
            {
                //elementos = (from s in BD.EncPrefactura
                //             select s).OrderByDescending(s => s.IDPrefactura);
                Filtro = " where e.Numero like '%"+searchString+"%' or c.Nombre like '%"+searchString+"%' order by e.IDPrefactura desc ";
                //elementos = elementos.Where(s => s.Numero.ToString().Contains(searchString) || s.Clientes.Nombre.Contains(searchString)).OrderByDescending(s => s.IDPrefactura);

                var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPrefactura.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPrefactura inner join Clientes on Clientes.IDCliente=EncPrefactura.IDCliente where (CAST(EncPrefactura.IDPrefactura AS nvarchar(max))='" + searchString + "' or Clientes.Nombre='" + searchString + "')  group by EncPrefactura.IDMoneda ").ToList();
                ViewBag.sumatoria = filtro;


            }
            //Filtro Divisa
            if (!String.IsNullOrEmpty(Divisa))
            {
                if (Filtro != "")
                {
                    Filtro = " where  cm.ClaveMoneda='"+Divisa+"' and (e.Numero like '%" + searchString + "%' or c.Nombre like '%" + searchString + "%') order by e.IDPrefactura desc";

                }
                else
                {
                    Filtro = " where cm.ClaveMoneda='" + Divisa + "' order by e.IDPrefactura desc";

                }
                //elementos = (from s in BD.EncPrefactura
                //             select s).OrderByDescending(s => s.IDPrefactura);
                //elementos = elementos.Where(s => s.c_Moneda.ClaveMoneda == Divisa).OrderByDescending(s => s.IDPrefactura);

                var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPrefactura.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPrefactura inner join c_Moneda on c_Moneda.IDMoneda=EncPrefactura.IDMoneda  where c_Moneda.ClaveMoneda='" + Divisa + "' and [Status]<>'Cancelado' group by EncPrefactura.IDMoneda").ToList();
                ViewBag.sumatoria = divisa;


            }

            ViewBag.SerieFacseleccionado = SerieFac;
            //Filtro Serie
            if (!String.IsNullOrEmpty(SerieFac))
            {
                //elementos = (from s in BD.EncPrefactura
                //             select s).OrderByDescending(s => s.IDPrefactura);
                //elementos = elementos.Where(s => s.Serie.Equals(SerieFac)).OrderByDescending(s => s.IDPrefactura); 
                if (Filtro != "")
                {
                    Filtro = " where  e.serie='" + SerieFac + "' and (e.Numero like '%" + searchString + "%' or c.Nombre like '%" + searchString + "%') order by e.IDPrefactura desc ";

                }
                else
                {
                    Filtro = " where e.serie='" + SerieFac + "' order by e.IDPrefactura desc";

                }
                var filtroserie = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPrefactura.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPrefactura where Serie='"+SerieFac+"' and [Status]<>'Cancelado' group by EncPrefactura.IDMoneda ").ToList();
                ViewBag.sumatoria = filtroserie;


            }
            ViewBag.Clienteseleccionado = ClieFac;
            //Filtro Cliente
            if (!String.IsNullOrEmpty(ClieFac))
            {
                //elementos = (from s in BD.EncPrefactura
                //             select s).OrderByDescending(s => s.IDPrefactura);
                //elementos = elementos.Where(s => s.Clientes.Nombre.Equals(ClieFac)).OrderByDescending(s => s.IDPrefactura);
                if (Filtro != "")
                {
                    Filtro = " where  c.Nombre='" + ClieFac + "' and (e.Numero like '%" + searchString + "%' or c.Nombre like '%" + searchString + "%') order by e.IDPrefactura desc";

                }
                else
                {
                    Filtro = " where  c.Nombre='" + ClieFac + "' order by e.IDPrefactura  desc";

                }

                var filtrocliente = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPrefactura.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPrefactura inner join Clientes on Clientes.IDCliente=EncPrefactura.IDCliente where Clientes.Nombre='" + ClieFac + "' and EncPrefactura.Status<>'Cancelado' group by EncPrefactura.IDMoneda ").ToList();
                ViewBag.sumatoria = filtrocliente;

            }

            //Filtro Status
            if (!String.IsNullOrEmpty(Status))
            {
                if (Filtro != "")
                {
                    Filtro = " where  e.status='" + Status + "' and (e.Numero like '%" + searchString + "%' or c.Nombre like '%" + searchString + "%') order by e.IDPrefactura desc";

                }
                else
                {
                    Filtro = " where  e.status='" + Status + "' order by e.IDPrefactura  desc";

                }
                //elementos = (from s in BD.EncPrefactura
                //             select s).OrderByDescending(s => s.IDPrefactura);
                //elementos = elementos.Where(s => s.Status.Equals(Status)).OrderByDescending(s => s.IDPrefactura);

                var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPrefactura.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPrefactura inner join c_Moneda on c_Moneda.IDMoneda=EncPrefactura.IDMoneda  where EncPrefactura.Status='" + Status + "' group by EncPrefactura.IDMoneda").ToList();
                ViewBag.sumatoria = divisa;


            }


            ViewBag.CurrentSort = sortOrder;
            ViewBag.PrefacturaSortParm = String.IsNullOrEmpty(sortOrder) ? "Prefactura" : "Prefactura";
            ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Cliente" : "Cliente";

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

            //switch (sortOrder)
            //{
            //    case "Prefactura":
            //        elementos = elementos.OrderByDescending(s => s.IDPrefactura);
            //        break;
            //    case "Cliente":
            //        elementos = elementos.OrderByDescending(s => s.Clientes.Nombre);
            //        break;
            //    default:
            //        elementos = elementos.OrderByDescending(s => s.IDPrefactura);
            //        break;
            //}

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = BD.EncPrefactura.Count(); // Total number of elements
            if (Filtro == "")
            {
                Filtro = " order by e.idprefactura desc";
            }
            cadenaSQl = ConsultaSql + " " + Filtro ;

            //var elementos = db.Database.SqlQuery<Encfacturas>(cadenaSQl).ToList();
            var elementos = BD.Database.SqlQuery<EncPrefactura>(cadenaSQl).ToList();


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
            List<VDetPrefactura> orden = BD.Database.SqlQuery<VDetPrefactura>("select articulo.IDArticulo, articulo.Cref, DetPrefactura.Caracteristica_ID,DetPrefactura.Lote,DetPrefactura.Proviene,DetPrefactura.Devolucion,DetPrefactura.Lote,Articulo.MinimoVenta,DetPrefactura.IDPrefactura,DetPrefactura.Suministro,Articulo.Descripcion as Articulo,DetPrefactura.Cantidad,DetPrefactura.Costo,DetPrefactura.CantidadPedida,DetPrefactura.Descuento,DetPrefactura.Importe,DetPrefactura.IVA,DetPrefactura.ImporteIva,DetPrefactura.ImporteTotal,DetPrefactura.Nota,DetPrefactura.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetPrefactura.Status from  DetPrefactura INNER JOIN Caracteristica ON DetPrefactura.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPrefactura='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = BD.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM c_moneda WHERE IDMoneda=EncPrefactura.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPrefactura inner join Clientes on Clientes.IDCliente=EncPrefactura.IDCliente  where EncPrefactura.IDPrefactura='" + id + "' group by EncPrefactura.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;

           ClsDatoEntero countpa = BD.Database.SqlQuery<ClsDatoEntero>("select count(IDPrefacturaAnticipo) as Dato from PrefacturaAnticipo where IDPrefactura='" + id + "'").ToList()[0];
            if (countpa.Dato == 0)
            {
                ViewBag.PrefacturaAnticipo = null;
            }
            else
            {
                List<PrefacturaAnticipo> prefacturaanticipo = BD.Database.SqlQuery<PrefacturaAnticipo>("select * from  PrefacturaAnticipo where IDPrefactura='" + id + "'").ToList();
                ViewBag.PrefacturaAnticipo = prefacturaanticipo;
            }
            EncPrefactura encPrefactura = BD.EncPrefactura.Find(id);

            return View(encPrefactura);
        }

        public ActionResult CambiarMoneda(int id)
        {
            EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Find(id);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas.Where(s=> s.IDMoneda== prefactura.IDMoneda), "IDMoneda", "Descripcion");
            ViewBag.IDMonedaD = new SelectList(db.c_Monedas.OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion");
            ViewBag.id = id;
            List<VDetPrefactura> orden = BD.Database.SqlQuery<VDetPrefactura>("select DetPrefactura.Proviene,DetPrefactura.Devolucion,DetPrefactura.Lote,Articulo.MinimoVenta,DetPrefactura.IDPrefactura,DetPrefactura.Suministro,Articulo.Descripcion as Articulo,DetPrefactura.Cantidad,DetPrefactura.Costo,DetPrefactura.CantidadPedida,DetPrefactura.Descuento,DetPrefactura.Importe,DetPrefactura.IVA,DetPrefactura.ImporteIva,DetPrefactura.ImporteTotal,DetPrefactura.Nota,DetPrefactura.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetPrefactura.Status from  DetPrefactura INNER JOIN Caracteristica ON DetPrefactura.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPrefactura='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = BD.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM c_moneda WHERE IDMoneda=EncPrefactura.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPrefactura inner join Clientes on Clientes.IDCliente=EncPrefactura.IDCliente  where EncPrefactura.IDPrefactura='" + id + "' group by EncPrefactura.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;


            return View();
        }
        [HttpPost]
        public ActionResult CambiarMoneda(int? id, int? IDMoneda, int? IDMonedaD,decimal? TC)
        {
            EncPrefactura prefacturaa = new PrefacturaContext().EncPrefactura.Find(id);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas.Where(s => s.IDMoneda == prefacturaa.IDMoneda), "IDMoneda", "Descripcion");
            ViewBag.IDMonedaD = new SelectList(db.c_Monedas.OrderBy(s => s.Descripcion), "IDMoneda", "Descripcion");
            ViewBag.id = id;
            List<VDetPrefactura> orden = BD.Database.SqlQuery<VDetPrefactura>("select DetPrefactura.Proviene,DetPrefactura.Devolucion,DetPrefactura.Lote,Articulo.MinimoVenta,DetPrefactura.IDPrefactura,DetPrefactura.Suministro,Articulo.Descripcion as Articulo,DetPrefactura.Cantidad,DetPrefactura.Costo,DetPrefactura.CantidadPedida,DetPrefactura.Descuento,DetPrefactura.Importe,DetPrefactura.IVA,DetPrefactura.ImporteIva,DetPrefactura.ImporteTotal,DetPrefactura.Nota,DetPrefactura.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetPrefactura.Status from  DetPrefactura INNER JOIN Caracteristica ON DetPrefactura.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPrefactura='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = BD.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM c_moneda WHERE IDMoneda=EncPrefactura.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPrefactura inner join Clientes on Clientes.IDCliente=EncPrefactura.IDCliente  where EncPrefactura.IDPrefactura='" + id + "' group by EncPrefactura.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;
            ViewBag.mensaje = null;
            if (IDMonedaD==null)
            {

             
                ViewBag.mensaje = "Se debe seleccionar una moneda";
                return View();
            }
            if (TC==null)
            {
                ViewBag.mensaje = "Tipo de Cambio sin especificar";
                return View();
            }
            decimal subtotal = 0, iva = 0, total = 0, costo = 0, importe = 0, importeiva = 0, importetotal=0;

            List<VDetPrefactura> prefactura = BD.Database.SqlQuery<VDetPrefactura>("select DetPrefactura.IDDetPrefactura,DetPrefactura.Proviene,DetPrefactura.Devolucion,DetPrefactura.Lote,Articulo.MinimoVenta,DetPrefactura.IDPrefactura,DetPrefactura.Suministro,Articulo.Descripcion as Articulo,DetPrefactura.Cantidad,DetPrefactura.Costo,DetPrefactura.CantidadPedida,DetPrefactura.Descuento,DetPrefactura.Importe,DetPrefactura.IVA,DetPrefactura.ImporteIva,DetPrefactura.ImporteTotal,DetPrefactura.Nota,DetPrefactura.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetPrefactura.Status from  DetPrefactura INNER JOIN Caracteristica ON DetPrefactura.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPrefactura='" + id + "'").ToList();

            ViewBag.carrito= prefactura;


            string fecha = DateTime.Now.ToString("yyyy/MM/dd");

            for (int i = 0; i < prefactura.Count(); i++)
            {
                costo = ViewBag.carrito[i].Costo * TC;
                importe = costo * ViewBag.carrito[i].Cantidad;
                importeiva = importe * (decimal) 0.16;
                importetotal = importe + importeiva;

                db.Database.ExecuteSqlCommand("update [dbo].[DetPrefactura] set  [Importe]=" + importe + ", [Costo]=" + costo + ",[ImporteIva]="+importeiva+",[ImporteTotal]="+importetotal+ " where IDDetPrefactura =" + ViewBag.carrito[i].IDDetPrefactura + "");

            }

            List<DetPrefactura> datostotales = db.Database.SqlQuery<DetPrefactura>("select * from dbo.DetPrefactura where IDPrefactura='" + id+ "'").ToList();
            subtotal = datostotales.Sum(s => s.Importe);
            iva = subtotal * (decimal)0.16;
            total = subtotal + iva;

            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + IDMonedaD + "," +origen + ") as TC").ToList()[0];

            db.Database.ExecuteSqlCommand("update [dbo].[EncPrefactura] set [IDMoneda]=" + IDMonedaD + ",[Subtotal]=" + subtotal + ",[IVA]=" + iva + ",[Total]=" + total + ",[TipoCambio]="+cambio.TC+" where [IDPrefactura]='" + id + "'");

            return RedirectToAction("CambiarMoneda",new {id=id});
        }



        public JsonResult getTC(int idmonedao,int idmonedad)
        {
            
            

            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + idmonedao + "," + idmonedad + ") as TC").ToList()[0];
            
            return Json(cambio.TC,JsonRequestBehavior.AllowGet);
        }



        // GET: Prefactura
        PedidoContext db = new PedidoContext();
        FolioVentasContext folio = new FolioVentasContext();
        c_TipoRelacionContext trelacion = new c_TipoRelacionContext();
        public ActionResult PrefacturaPedido(int? id, FolioVentas folioventas, string Observacion, FormCollection collection, int? IDTipoRelacion, int? IDCondicionesPago, int? IDFormapago, int? IDMetodoPago, int? IDUsoCFDI)
        {


            EncPedido pedidoc = new PedidoContext().EncPedidos.Find(id);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", pedidoc.IDCondicionesPago);
            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", pedidoc.IDFormapago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", pedidoc.IDMetodoPago);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", pedidoc.IDUsoCFDI);
            decimal TipoCambio = 0;
            try
            {
                string fechaC = DateTime.Now.ToString("yyyyMMdd");
                ////////////////////////////////////////////////////7
                ///

                List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
                VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fechaC + "',181,180) as TC").ToList().FirstOrDefault();
                TipoCambio = cambio.TC;
            }
            catch (Exception err)
            {
                TipoCambio = pedidoc.TipoCambio;
            }

            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            int contador = 0, numerofolio = 0;
            ViewBag.IDFolioVentas = new SelectList(folio.FoliosV, "IDFolioVentas", "Serie");
            List<SelectListItem> items = new SelectList(trelacion.c_TipoRelaciones, "IDTipoRelacion", "Descripcion", IDTipoRelacion).ToList();
            items.Insert(0, new SelectListItem { Selected = true, Text = "Sin relacion", Value = "" });
            SelectList relacion = new SelectList(items, "Value", "Text", null);


            List<DetPedido> detpedido = db.Database.SqlQuery<DetPedido>("select * from dbo.DetPedido where IDPedido='" + id + "' and (Status='Activo' or Status='Recepcionado'  or Status='PreFacturado')").ToList();
            ViewData["detpedido"] = detpedido;

            ViewBag.IDTipoRelacion = relacion;

            if (IDTipoRelacion == null)
            {
                IDTipoRelacion = 0;
            }
            if (folioventas.IDFolioVentas != 0)
            {
                FolioVentas foliov = folio.FoliosV.Find(folioventas.IDFolioVentas);
                string Serie = foliov.Serie;
                numerofolio = foliov.Numero + 1;
                decimal subtotal = 0, iva = 0, total = 0;
                EncPedido encPedido = db.EncPedidos.Find(id);


                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int UserID = userid.Select(s => s.UserID).FirstOrDefault();

                db.Database.ExecuteSqlCommand("insert into [dbo].[EncPrefactura] ([Fecha],[Observacion],[UserID],[DocumentoFactura],[Subtotal],[IVA],[Total],[IDCliente],[IDMetodoPago],[IDFormaPago],[IDMoneda],[IDCondicionesPago],[IDAlmacen],[IDUsoCFDI],[TipoCambio],[Status],[IDVendedor],[Serie],[Numero],[IDFacturaDigital],[SerieDigital],[NumeroDigital],[UUID],[Entrega],[IDTipoRelacion]) values('" + fecha + "','" + Observacion + "', '" + UserID + "','0', '0', '0', '0', '" + encPedido.IDCliente + "','" + encPedido.IDMetodoPago + "','" + encPedido.IDFormapago + "', '" + encPedido.IDMoneda + "', '" + encPedido.IDCondicionesPago + "','" + encPedido.IDAlmacen + "', '" + encPedido.IDUsoCFDI + "', '" + TipoCambio + "','Activo','" + encPedido.IDVendedor + "','" + Serie + "','" + numerofolio + "','0','0','0','0','" + encPedido.Entrega + "','" + IDTipoRelacion + "')");

                db.Database.ExecuteSqlCommand("update [dbo].[FolioVentas] set [Numero]='" + numerofolio + "' where [IDFolioVentas]='" + folioventas.IDFolioVentas + "'");

                ClsDatoEntero numero = db.Database.SqlQuery<ClsDatoEntero>("select MAX(IDPrefactura) as Dato from [dbo].[EncPrefactura]").ToList()[0];
                int num = numero.Dato;
                foreach (var key in collection.AllKeys)
                {

                    var value = collection[key];
                    contador++;
                    if (contador >= 8)
                    {

                        ClsDatoEntero countidfactura = db.Database.SqlQuery<ClsDatoEntero>("select count(ID) as Dato from [dbo].[EncFacturas] where [UUID]='" + value + "'").ToList()[0];
                        if (countidfactura.Dato != 0)
                        {
                            ClsDatoEntero idfactura = db.Database.SqlQuery<ClsDatoEntero>("select ID as Dato from [dbo].[EncFacturas] where [UUID]='" + value + "'").ToList()[0];


                            bd.Database.ExecuteSqlCommand("insert into [dbo].[PrefacturaAnticipo] ([IDPrefactura],[IDFacturaAnticipo],[UUIDAnticipo]) values('" + num + "','" + idfactura.Dato + "','" + value + "')");
                        }
                    }
                }


                Hashtable tabla = new Hashtable();
                foreach (DetPedido detalle in detpedido) //agrupamos partidas con sus picos
                {
                   
                  
                    db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [IDPrefactura]=" + num + ",[Status]='PreFacturado' where [IDDetPedido]='" + detalle.IDDetPedido + "'");

                    string llave = detalle.IDArticulo + "," + detalle.Caracteristica_ID;

                    if (!tabla.ContainsKey(llave))
                    {
                        db.Database.ExecuteSqlCommand("insert into DetPrefactura([IDPrefactura],[IDExterna],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[IDDetExterna],[Presentacion],[jsonPresentacion],[Lote],[Devolucion],[Proviene]) values ('" + num + "','" + detalle.IDPedido + "','" + detalle.IDArticulo + "','" + detalle.CantidadPedida + "','" + detalle.Costo + "','0','0','" + detalle.Importe + "','true','" + detalle.ImporteIva + "','" + detalle.ImporteTotal + "','" + detalle.Nota + "','0','" + detalle.Caracteristica_ID + "','" + detalle.IDAlmacen + "','0','Activo','" + detalle.IDDetPedido + "','" + detalle.Presentacion + "','" + detalle.jsonPresentacion + "','0','0','Pedido')");
                        tabla.Add(llave, detalle);
                        List<DetPrefactura> numero2 = db.Database.SqlQuery<DetPrefactura>("select * from [dbo].[DetPrefactura] WHERE IDDetPrefactura = (SELECT MAX(IDDetPrefactura) from DetPrefactura)").ToList();
                        int num2 = numero2.Select(s => s.IDDetPrefactura).FirstOrDefault();
                        db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Prefactura','" + num + "','" + detalle.CantidadPedida + "','" + fecha + "','" + detalle.IDPedido + "','Pedido','" + UserID + "','" + detalle.IDDetPedido + "',"+num2+")");
                        bd.Database.ExecuteSqlCommand("insert into [dbo].[elementosprefactura] ([documento],[iddocumento],[iddetdocumento],cantidad,iddetprefactura,idprefactura ) values('Pedido'," + id + "," + detalle.IDDetPedido + "," + detalle.CantidadPedida + ","+ num2 +"," + num + ")");
                    }
                    else
                    {
                        DetPedido anterior = (DetPedido)tabla[llave];
                        DetPedido nuevo = anterior;
                        nuevo.Cantidad = nuevo.Cantidad + detalle.Cantidad;
                        nuevo.CantidadPedida = nuevo.CantidadPedida + detalle.CantidadPedida;
                        nuevo.Importe = Math.Round( nuevo.CantidadPedida * nuevo.Costo,2);
                        nuevo.ImporteIva = Math.Round(nuevo.Importe * (decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA) ),2);
                        tabla.Remove(llave);
                        tabla.Add(llave, nuevo);
                        List<DetPrefactura> numero2 = db.Database.SqlQuery<DetPrefactura>("select * from [dbo].[DetPrefactura] WHERE IDArticulo = "+ detalle.IDArticulo + " and Caracteristica_ID="+ detalle.Caracteristica_ID +" and idprefactura="+ num +")").ToList();
                        int num2 = numero2.Select(s => s.IDDetPrefactura).FirstOrDefault();
                        db.Database.ExecuteSqlCommand("update deteprefactura set cantidad=" + nuevo.Cantidad + ",cantidadpedida=" + nuevo.CantidadPedida + ",importe=" + nuevo.Importe + ",importeiva=" + nuevo.ImporteIva + ", importetotal=" + nuevo.ImporteTotal + " where iddetprefactura=" + num2);
                        db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Prefactura','" + num + "','" + detalle.CantidadPedida + "','" + fecha + "','" + detalle.IDPedido + "','Pedido','" + UserID + "','" + detalle.IDDetPedido + "'," + num2 + ")");
                        bd.Database.ExecuteSqlCommand("insert into [dbo].[elementosprefactura] ([documento],[iddocumento],[iddetdocumento],cantidad,iddetprefactura,idprefactura ) values('Pedido'," + id + "," + detalle.IDDetPedido + "," + detalle.CantidadPedida + "," + num2 + "," + num + ")");
                    }
                }
              
                db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [Status]='PreFacturado' where [IDPedido]='" + id + "'");
                List<DetPrefactura> datostotales = db.Database.SqlQuery<DetPrefactura>("select * from dbo.DetPrefactura where IDPrefactura='" + num + "'").ToList();
                subtotal = datostotales.Sum(s => s.Importe);
                iva = subtotal * (decimal)0.16;
                total = subtotal + iva;
                db.Database.ExecuteSqlCommand("update [dbo].[EncPrefactura] set [IDCondicionesPago]=" + IDCondicionesPago + ",[IDFormapago]=" + IDFormapago + ",[IDMetodoPago]=" + IDMetodoPago + ",[IDUsoCFDI]=" + IDUsoCFDI + ",[Subtotal]='" + subtotal + "',[IVA]='" + iva + "',[Total]='" + total + "' where [IDPrefactura]='" + num + "'");


                return RedirectToAction("Index");
            }


            return View();
        }


        RemisionContext bd = new RemisionContext();
        FolioVentasContext folios = new FolioVentasContext();
        public ActionResult PrefacturaRemision(int? id, FolioVentas folioventas,string Observacion, FormCollection collection, int? IDTipoRelacion,int? IDCondicionesPago, int? IDFormapago, int? IDMetodoPago, int? IDUsoCFDI)
        {
            EncRemision remisionc = new RemisionContext().EncRemisiones.Find(id);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", remisionc.IDCondicionesPago);
            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", remisionc.IDFormapago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", remisionc.IDMetodoPago);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", remisionc.IDUsoCFDI);

            int contador = 0,numerofolio=0;
         
            ViewBag.IDFolioVentas = new SelectList(folio.FoliosV, "IDFolioVentas", "Serie");

            List<SelectListItem> items = new SelectList(trelacion.c_TipoRelaciones, "IDTipoRelacion", "Descripcion", IDTipoRelacion).ToList();
            items.Insert(0, new SelectListItem { Selected = true, Text = "Sin relacion", Value = "" });
            SelectList relacion = new SelectList(items, "Value", "Text", null); 


            ViewBag.IDTipoRelacion = relacion;
            if (IDTipoRelacion == null)
            {
                IDTipoRelacion = 0;
            }
            if (folioventas.IDFolioVentas != 0)
            {
                FolioVentas foliov = folio.FoliosV.Find(folioventas.IDFolioVentas);
                string Serie = foliov.Serie;
                numerofolio = foliov.Numero + 1;

                decimal subtotal = 0, iva = 0, total = 0, importe = 0, importetotal = 0, importeiva = 0;
                EncRemision encRemision = bd.EncRemisiones.Find(id);
                decimal TipoCambioR = 0;
                try
                {
                    string fechaC = DateTime.Now.ToString("yyyyMMdd");
                    ////////////////////////////////////////////////////7
                    ///

                    List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                    int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
                    VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fechaC + "',181,180) as TC").ToList().FirstOrDefault();
                    TipoCambioR = cambio.TC;
                }
                catch (Exception err)
                {
                    TipoCambioR = encRemision.TipoCambio;
                }
                string fecha = DateTime.Now.ToString("yyyyMMdd");

                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int UserID = userid.Select(s => s.UserID).FirstOrDefault();

                bd.Database.ExecuteSqlCommand("insert into [dbo].[EncPrefactura] ([Fecha],[Observacion],[UserID],[DocumentoFactura],[Subtotal],[IVA],[Total],[IDCliente],[IDMetodoPago],[IDFormaPago],[IDMoneda],[IDCondicionesPago],[IDAlmacen],[IDUsoCFDI],[TipoCambio],[Status],[IDVendedor],[Serie],[Numero],[IDFacturaDigital],[SerieDigital],[NumeroDigital],[UUID],[Entrega],[IDTipoRelacion]) values('" + fecha + "','" + Observacion + "', '" + UserID + "','0', '0', '0', '0', '" + encRemision.IDCliente + "','" + encRemision.IDMetodoPago + "','" + encRemision.IDFormapago + "', '" + encRemision.IDMoneda + "', '" + encRemision.IDCondicionesPago + "','" + encRemision.IDAlmacen + "', '" + encRemision.IDUsoCFDI + "', '" + TipoCambioR + "','Activo','" + encRemision.IDVendedor + "','" + Serie + "','" + numerofolio + "','0','0','0','0','" + encRemision.Entrega + "','"+ IDTipoRelacion+"')");

                db.Database.ExecuteSqlCommand("update [dbo].[FolioVentas] set [Numero]='" + numerofolio + "' where [IDFolioVentas]='" + folioventas.IDFolioVentas + "'");

                List<EncPrefactura> numero = bd.Database.SqlQuery<EncPrefactura>("select * from [dbo].[EncPrefactura] WHERE IDPrefactura = (SELECT MAX(IDPrefactura) from EncPrefactura)").ToList();
                int num = numero.Select(s => s.IDPrefactura).FirstOrDefault();

                foreach (var key in collection.AllKeys)
                {

                    var value = collection[key];
                    contador++;
                    if (contador >= 8)
                    {
                        bd.Database.ExecuteSqlCommand("insert into [dbo].[PrefacturaAnticipo] ([IDPrefactura],[IDFacturaAnticipo],[UUIDAnticipo]) values('" + num + "','0','" + value + "')");

                    }
                }

                db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='Activo' where [Status]='Devuelto' and IDRemision='"+id+"'");
                db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoDevolucionR] where IDRemision='" + id + "'");

                List<DetRemision> detPedido = bd.Database.SqlQuery<DetRemision>("select * from dbo.DetRemision where IDRemision='" + id + "' and Status='Activo'").ToList();
                ViewBag.pedidov = detPedido;

               


                Hashtable tabla = new Hashtable();
                foreach (DetRemision detalle in detPedido) //agrupamos partidas con sus picos
                {

                    db.Database.ExecuteSqlCommand("update [dbo].[Detremision] set [Status]='PreFacturado' where [IDDetRemision]='" + detalle.IDDetRemision + "'");


                    string llave = detalle.IDArticulo + "," + detalle.Caracteristica_ID;

                    if (!tabla.ContainsKey(llave))
                    {
                        db.Database.ExecuteSqlCommand("insert into DetPrefactura([IDPrefactura],[IDExterna],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[IDDetExterna],[Presentacion],[jsonPresentacion],[Lote],[Devolucion],[Proviene]) values ('" + num + "','" + detalle.IDRemision + "','" + detalle.IDArticulo + "','" + detalle.Cantidad + "','" + detalle.Costo + "','0','0','" + detalle.Importe + "','true','" + detalle.ImporteIva + "','" + detalle.ImporteTotal + "','" + detalle.Nota + "','0','" + detalle.Caracteristica_ID + "','" + detalle.IDAlmacen + "','0','Activo','" + detalle.IDDetRemision + "','" + detalle.Presentacion + "','" + detalle.jsonPresentacion + "','" + detalle.Lote + "','0','Remision')");
                        tabla.Add(llave, detalle);
                        List<DetPrefactura> numero2 = db.Database.SqlQuery<DetPrefactura>("select * from [dbo].[DetPrefactura] WHERE IDDetPrefactura = (SELECT MAX(IDDetPrefactura) from DetPrefactura)").ToList();
                        int num2 = numero2.Select(s => s.IDDetPrefactura).FirstOrDefault();

                        db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Prefactura','" + num + "','" + detalle.CantidadPedida + "','" + fecha + "','" + detalle.IDRemision + "','Remision','" + UserID + "','" + detalle.IDDetRemision + "'," + num2 + ")");
                        bd.Database.ExecuteSqlCommand("insert into [dbo].[elementosprefactura] ([documento],[iddocumento],[iddetdocumento],cantidad,iddetprefactura,idprefactura ) values('Remision'," + detalle.IDRemision + "," + detalle.IDDetRemision + "," + detalle.Cantidad + "," + num2 + "," + num + ")");

                    }
                    else
                    {
                        DetRemision anterior = (DetRemision)tabla[llave];
                        DetRemision nuevo = anterior;
                        nuevo.Cantidad = nuevo.Cantidad + detalle.Cantidad;
                        nuevo.CantidadPedida = nuevo.CantidadPedida + detalle.CantidadPedida;
                        nuevo.Importe = Math.Round(nuevo.Cantidad * nuevo.Costo, 2);
                        nuevo.ImporteIva = Math.Round(nuevo.Importe * (decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA) ), 2);
                        nuevo.ImporteTotal = nuevo.Importe + nuevo.ImporteIva;
                        tabla.Remove(llave);
                        tabla.Add(llave, nuevo);
                        List<DetPrefactura> numero2 = db.Database.SqlQuery<DetPrefactura>("select * from [dbo].[DetPrefactura] WHERE IDArticulo = " + detalle.IDArticulo + " and Caracteristica_ID=" + detalle.Caracteristica_ID + " and idprefactura=" + num + "").ToList();
                        int num2 = numero2.Select(s => s.IDDetPrefactura).FirstOrDefault();
                        db.Database.ExecuteSqlCommand("update detprefactura set cantidad=" + nuevo.Cantidad + ",cantidadpedida=" + nuevo.CantidadPedida + ",importe=" + nuevo.Importe + ",importeiva=" + nuevo.ImporteIva + ", importetotal=" + nuevo.ImporteTotal + " where iddetprefactura=" + num2);
                        db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Prefactura','" + num + "','" + detalle.CantidadPedida + "','" + fecha + "','" + detalle.IDRemision + "','Remision','" + UserID + "','" + detalle.IDDetRemision + "'," + num2 + ")");
                        try
                        {
                            bd.Database.ExecuteSqlCommand("insert into [dbo].[elementosprefactura] ([documento],[iddocumento],[iddetdocumento],cantidad,iddetprefactura,idprefactura ) values('Remision'," + detalle.IDRemision + "," + detalle.IDDetRemision + "," + detalle.CantidadPedida + "," + num2 + "," + num + ")");
                        }
                        catch(Exception err)
                        {
                            string mensajedeerror = err.Message;
                        }
                    }

                  //  db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [IDPrefactura]='" + num + "',[Status]='PreFacturado' where [IDDetPedido]='" + detalle.IDDetExterna + "' and [Status]<>'Finalizado'");
                    db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='PreFacturado' where [IDDetRemision]='" + detalle.IDDetRemision + "'");
                    //db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='PreFacturado' where [IDRemision]='" + detalle.IDRemision + "'");

                }

               

                db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='PreFacturado' where [IDRemision]='" + id + "'");

                List<DetPrefactura> datostotales2 = db.Database.SqlQuery<DetPrefactura>("select * from dbo.DetPrefactura where IDPrefactura='" + num + "'").ToList();
                subtotal = Math.Round( datostotales2.Sum(s => s.Importe),2);
                iva =  Math.Round( subtotal * (decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA) ),2);
                total = subtotal + iva;
                db.Database.ExecuteSqlCommand("update [dbo].[EncPrefactura] set [IDCondicionesPago]=" + IDCondicionesPago + ",[IDFormapago]=" + IDFormapago + ",[IDMetodoPago]=" + IDMetodoPago + ",[IDUsoCFDI]=" + IDUsoCFDI + ",[Subtotal]='" + subtotal + "',[IVA]='" + iva + "',[Total]='" + total + "' where [IDPrefactura]='" + num + "'");
                return RedirectToAction("Index");


            }


           


            return View();
            
          
        }


       






        public ActionResult Cancelar(int? id)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            string fecha = DateTime.Now.ToString("yyyyMMdd");

            List<elementosprefactura> elementos = BD.Database.SqlQuery<elementosprefactura>("select * from  dbo.elementosprefactura  where IDPrefactura='" + id + "'").ToList();

           
            foreach (elementosprefactura elemento in elementos)
            {
                if (elemento.documento.Equals("Pedido"))
                {
                   // DetPrefactura prefactura = new PrefacturaContext().DetPrefactura.Find(ViewBag.req[i].IDDetPrefactura);
                    db.Database.ExecuteSqlCommand("update DetPedido set [Status]='Activo' where IDDetPedido=" + elemento.iddetdocumento + "");
                    db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Pedido','" + elemento.iddocumento + "','" + elemento.cantidad + "','"+fecha+"','" + elemento.idprefactura + "','Prefactura','" + UserID + "','" + elemento.iddetprefactura + "','" +elemento.iddetdocumento+ "')");

                    db.Database.ExecuteSqlCommand("update EncPedido set [Status]='Activo' where IDPedido=" + elemento.iddocumento + "");
                    db.Database.ExecuteSqlCommand("update DetPedido set [Status]='Activo'  where IDDetPedido=" + elemento.iddetdocumento + "");
                }
                else if(elemento.documento.Equals("Remision"))
                {
                   // DetPrefactura prefactura = new PrefacturaContext().DetPrefactura.Find(ViewBag.req[i].IDDetPrefactura);
                    db.Database.ExecuteSqlCommand("update DetRemision set [Status]='Activo' where IDDetRemision=" + elemento.iddetdocumento + "");
                    db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Remision','" + elemento.iddocumento + "','" + elemento.cantidad + "','" + fecha + "','" + elemento.idprefactura + "','Prefactura','" + UserID + "','" + elemento.iddetprefactura + "','" + elemento.iddetdocumento + "')");

                    db.Database.ExecuteSqlCommand("update EncRemision set [Status]='Activo' where IDRemision=" + elemento.iddocumento + "");
                    //db.Database.ExecuteSqlCommand("update DetRemision set [Status]='Activo'  where IDDetRemision=" + prefactura.IDDetExterna + "");
                    db.Database.ExecuteSqlCommand("update DetRemision set [Status]='Activo'  where IDdetRemision=" + elemento.iddetdocumento );
                }
            }

            db.Database.ExecuteSqlCommand("update EncPrefactura set [Status]='Cancelado' where IDPrefactura=" + id + "");
            db.Database.ExecuteSqlCommand("update DetPrefactura set [Status]='Cancelado',IDdetExterna=0,Proviene='Cancelado'  where IDPrefactura=" + id + "");
            bd.Database.ExecuteSqlCommand("delete from [dbo].[PrefacturaAnticipo] where IDPrefactura="+id+"");

            return RedirectToAction("Index");
        }

        public ActionResult Mensaje(String Mensaje)
        {
            ViewBag.Mensaje = Mensaje;


            return View();
        }
        public ActionResult timbrarprefactura(int id)
        {
            VMFacDigital modelo = new VMFacDigital();
            modelo.Idprefacturafactura = id;
            EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Find(modelo.Idprefacturafactura);

            var listaserie = new FoliosRepository().GetFolios();
            ViewBag.listaserie = listaserie;
            try
            {
                if (prefactura.Clientes.Status == "Supendido")
                {
                    return RedirectToAction("Mensaje", "Prefactura", new { Mensaje = "El cliente " + prefactura.Clientes.Nombre + " Se encuentra suspendido por lo que no puede facturarse" });
                }
            }
            catch (Exception err)
            {

            }
            return View(modelo);
        }

        [HttpPost]
        public ActionResult timbrarprefactura(VMFacDigital modelo, FormCollection coleccion)
        {
            string Boton = "";
            try
            {
                Boton = coleccion.Get("Version");
            }
            catch (Exception err)
            {

            }
            if (Boton == "Crear factura v3")
            {
                timbrarprefactura3(modelo);

                return RedirectToAction("Index", "Prefactura");
            }

            PrefacturaContext db = new PrefacturaContext();
            EncPrefactura prefactura = db.EncPrefactura.Find(modelo.Idprefacturafactura);
            List<DetPrefactura> detalles = db.DetPrefactura.SqlQuery("select * from [DetPrefactura] where IDPrefactura=" + modelo.Idprefacturafactura).ToList();
            List<PrefacturaAnticipo> relacionados = db.Database.SqlQuery<PrefacturaAnticipo>("select * from [PrefacturaAnticipo] where [IDPrefactura]=" + modelo.Idprefacturafactura).ToList();
            ClsFactura40 factura = new ClsFactura40();

            EmpresaContext dbe = new EmpresaContext();

            Empresa emisora = dbe.empresas.Find(2);


            factura.Regimen = emisora.c_RegimenFiscal.ClaveRegimenFiscal.ToString();

            factura.Tipodecombrobante = "I";
            factura.Moneda = prefactura.c_Moneda.ClaveMoneda;
            if (factura.Moneda == "MXN")
            {
                factura.tipodecambio = "1";
            }
            else
            {
                c_Moneda monedanacional = new c_MonedaContext().c_Monedas.SqlQuery("select * from c_moneda where clavemoneda ='MXN'").ToList()[0];
                // factura.tipodecambio = dbe.Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetTipocambio] ('" + DateTime.Now.ToString("yyyy/MM/dd") + "'," + prefactura.IDMoneda + "," + monedanacional.IDMoneda + ") as Dato").ToList()[0].ToString();
                factura.tipodecambio = dbe.Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetTipocambio] ('" + DateTime.Now.ToString("s") + "'," + prefactura.IDMoneda + "," + monedanacional.IDMoneda + ") as Dato").ToList()[0].Dato.ToString();
            }

            factura.metododepago = prefactura.c_MetodoPago.ClaveMetodoPago;
            try
            {
                if (prefactura.c_TipoRelacion.ClaveTipoRelacion == "07")
                {

                    factura.metododepago = "PPD";
                }
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            factura.condicionesPago = prefactura.CondicionesPago.Descripcion;
            factura.formadepago = prefactura.c_FormaPago.ClaveFormaPago;
            factura.uso = prefactura.c_UsoCFDI.ClaveCFDI;

            FolioContext dbf = new FolioContext();

            Folio folio = dbf.Folios.Find(modelo.serie);

            factura._serie = folio.Serie;
            factura._folio = (folio.Numero + 1).ToString();

            factura.valoriva = decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
            factura.Descuento = prefactura.Descuento;


            factura.Emisora = emisora;

            Empresa Receptor = new Empresa();
            Receptor.RFC = prefactura.Clientes.RFC;
            Receptor.RazonSocial = prefactura.Clientes.Nombre40;
            Receptor.Calle = prefactura.Clientes.Calle;
            Receptor.NoExt = prefactura.Clientes.NumExt;
            Receptor.NoInt = prefactura.Clientes.NumInt;
            Receptor.CP = prefactura.Clientes.CP;
            factura.Receptora = Receptor;
            factura.RegimenFiscalReceptor = Convert.ToString(prefactura.Clientes.c_RegimenFiscal.ClaveRegimenFiscal);

            factura.Receptora = Receptor;


            List<Concepto40> conceptos = new List<Concepto40>();


            foreach (var detalle in detalles)
            {
                Concepto40 concepto1 = new Concepto40();
                c_ClaveProductoServicioContext dbclave = new c_ClaveProductoServicioContext();
                c_ClaveProductoServicio cs = dbclave.c_ClaveProductoServicios.Find(detalle.Articulo.Familia.IDProdServ);


                concepto1.ClaveProdServ = cs.ClaveProdServ;
                concepto1.NoIdentificacion = detalle.Articulo.Cref;
                concepto1.Cantidad = detalle.Cantidad;
                concepto1.ClaveUnidad = detalle.Articulo.c_ClaveUnidad.ClaveUnidad;
                concepto1.Unidad = detalle.Articulo.c_ClaveUnidad.Nombre;
                concepto1.Descripcion = detalle.Articulo.Descripcion;
                concepto1.ValorUnitario = detalle.Costo;
                concepto1.Importe = detalle.Importe;
                concepto1.llevaiva = detalle.IVA;
                concepto1.Descuento = detalle.Descuento;
                conceptos.Add(concepto1);
            }

            factura.Listaconceptos.conceptos = conceptos;


            if (relacionados.Count > 0 && prefactura.c_TipoRelacion.ClaveTipoRelacion != string.Empty)
            {
                factura.cfdirelacionados.relacion = prefactura.c_TipoRelacion.ClaveTipoRelacion;

                foreach (var cuales in relacionados)
                {

                    factura.cfdirelacionados.uuid.Add(cuales.UUIDAnticipo);
                }
            }


            MultiFacturasSDK.MFSDK sdk = factura.construirfactura2();
            MultiFacturasSDK.SDKRespuesta respuesta = factura.timbrar(sdk);
            bool pasa = false;
            try
            {
                if (respuesta.Codigo_MF_Texto.Contains("OK"))
                {
                    pasa = true;
                    try
                    {
                        Encfacturas facturae = new EncfacturaContext().encfacturas.Where(s => s.UUID == respuesta.UUID).ToList().FirstOrDefault();
                        if (facturae == null)
                        {
                            throw new Exception("Posbiblemente no tiene factura");
                        }
                        else
                        {
                            pasa = false;
                            return Content("UUID Existente");

                        }
                    }
                    catch (Exception err)
                    {
                        pasa = true;
                    }
                }

                if (!pasa)
                {
                    throw new Exception("RESPUESTA SAT "+ respuesta.Codigo_MF_Texto);
                    //return RedirectToAction("Index", "Prefactura"); 
                }
            }
            catch (Exception err)
            {

            }


            var reshtml = Server.HtmlEncode(respuesta.Codigo_MF_Texto + "->" + respuesta.CFDI);
            if (respuesta.Codigo_MF_Texto.Contains("OK"))
            {
                Generador.CreaPDF temp2 = new Generador.CreaPDF(respuesta.CFDI);
                Encfacturas elemento2 = new Encfacturas();

                elemento2.Fecha = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                elemento2.Serie = factura._serie;
                elemento2.Numero = int.Parse(factura._folio);
                elemento2.Nombre_cliente = prefactura.Clientes.Nombre;
                elemento2.Subtotal = decimal.Parse(temp2._templatePDF.subtotal.ToString());
                elemento2.Total = decimal.Parse(temp2._templatePDF.total.ToString());
                elemento2.IVA = decimal.Parse(temp2._templatePDF.totalImpuestosRetenidos.ToString());
                elemento2.Estado = "A";
                elemento2.Moneda = temp2._templatePDF.claveMoneda;
                elemento2.TC = Decimal.Parse(temp2._templatePDF.tipo_cambio);
                elemento2.pagada = false;



                elemento2.IDCliente = prefactura.IDCliente;




                elemento2.IDMetododepago = prefactura.c_MetodoPago.ClaveMetodoPago;

                elemento2.IDMoneda = prefactura.c_Moneda.IDMoneda;
                elemento2.RutaXML = respuesta.CFDI;

                elemento2.UUID = respuesta.UUID;
                elemento2.Estado = "A";
                elemento2.ConPagos = false;

                EncfacturaContext dbfa = new EncfacturaContext();

                elemento2.Descuento = prefactura.Descuento;
                elemento2.FechaRevision = System.DateTime.Now;

                CondicionesPago condicion = new CondicionesPagoContext().CondicionesPagos.Find(prefactura.IDCondicionesPago);


                elemento2.FechaVencimiento = elemento2.Fecha.AddDays(double.Parse(condicion.DiasCredito.ToString()));

                dbfa.encfacturas.Add(elemento2);
                dbfa.SaveChanges();

                db.Database.ExecuteSqlCommand("update folio set numero=" + elemento2.Numero + " where IDFolio=" + modelo.serie);

                Encfacturas facturagrabada = dbfa.encfacturas.ToList().Where(x => (x.UUID == elemento2.UUID)).ToList()[0];

                SaldoFactura saldo = new SaldoFactura();
                saldo.IDFactura = facturagrabada.ID;
                saldo.Serie = elemento2.Serie;
                saldo.Numero = elemento2.Numero;
                saldo.Total = elemento2.Total;
                saldo.ImporteSaldoAnterior = elemento2.Total;
                saldo.ImportePagado = 0;
                saldo.ImporteSaldoInsoluto = elemento2.Total;

                SaldoFacturaContext dbsp = new SaldoFacturaContext();
                dbsp.SaldoFacturas.Add(saldo);
                dbsp.SaveChanges();

                //GRABA XML 

                try
                {
                    Generador.CreaPDF temp = new Generador.CreaPDF(elemento2.RutaXML.ToString());
                    string fileName = temp._templatePDF.folioFiscalUUID + ".xml";

                    EscribeEnArchivo(elemento2.RutaXML.ToString(), fileName, true);
                }
                catch (Exception err)
                {
                    string mensajedeerror = err.Message;
                }




                dbsp.Database.ExecuteSqlCommand("update encprefactura SET IDFacturaDigital=" + facturagrabada.ID + ", SerieDigital='" + elemento2.Serie + "', NumeroDigital=" + elemento2.Numero + " where IDPrefactura=" + prefactura.IDPrefactura);


                try
                {
                    if (prefactura.c_TipoRelacion.ClaveTipoRelacion == "07")
                    {

                        return RedirectToAction("CrearAnticipoEgreso", "FacturaComplemento", new { UUID = elemento2.UUID });
                    }
                    else
                    {
                        return RedirectToAction("Index", "FacturaAll");
                    }
                }
                catch (Exception err)
                {
                    String mensaje = err.Message;
                    return RedirectToAction("Index", "FacturaAll");
                }


            }
            else
            {


                return Content(reshtml);
            }
        }
        //[HttpPost]
        public ActionResult timbrarprefactura3(VMFacDigital modelo)
        {
            //string Boton = "";
            //try
            //{
            //    Boton = coleccion.Get("Version");
            //}catch(Exception err){

            //}
            //if (Boton== "Crear factura v3")
            //{
            //    timbrarprefactura3(modelo);

            //    return RedirectToAction("Index", "Prefactura");
            //}

            PrefacturaContext db = new PrefacturaContext();
            EncPrefactura prefactura = db.EncPrefactura.Find(modelo.Idprefacturafactura);
            List<DetPrefactura> detalles = db.DetPrefactura.SqlQuery("select * from [DetPrefactura] where IDPrefactura=" + modelo.Idprefacturafactura).ToList();
            List<PrefacturaAnticipo> relacionados = db.Database.SqlQuery<PrefacturaAnticipo>("select * from [PrefacturaAnticipo] where [IDPrefactura]=" + modelo.Idprefacturafactura).ToList();
            ClsFactura factura = new ClsFactura();

            EmpresaContext dbe = new EmpresaContext();

            Empresa emisora = dbe.empresas.Find(2);


            factura.Regimen = emisora.c_RegimenFiscal.ClaveRegimenFiscal.ToString();

            factura.Tipodecombrobante = "I";
            factura.Moneda = prefactura.c_Moneda.ClaveMoneda;
            if (factura.Moneda == "MXN")
            {
                factura.tipodecambio = "1";
            }
            else
            {
                c_Moneda monedanacional = new c_MonedaContext().c_Monedas.SqlQuery("select * from c_moneda where clavemoneda ='MXN'").ToList()[0];
                // factura.tipodecambio = dbe.Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetTipocambio] ('" + DateTime.Now.ToString("yyyy/MM/dd") + "'," + prefactura.IDMoneda + "," + monedanacional.IDMoneda + ") as Dato").ToList()[0].ToString();
                factura.tipodecambio = dbe.Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetTipocambio] ('" + DateTime.Now.ToString("s") + "'," + prefactura.IDMoneda + "," + monedanacional.IDMoneda + ") as Dato").ToList()[0].Dato.ToString();
            }

            factura.metododepago = prefactura.c_MetodoPago.ClaveMetodoPago;
            try
            {
                if (prefactura.c_TipoRelacion.ClaveTipoRelacion == "07")
                {

                    factura.metododepago = "PPD";
                }
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            factura.formadepago = prefactura.c_FormaPago.ClaveFormaPago;
            factura.uso = prefactura.c_UsoCFDI.ClaveCFDI;

            FolioContext dbf = new FolioContext();

            Folio folio = dbf.Folios.Find(modelo.serie);

            factura._serie = folio.Serie;
            factura._folio = (folio.Numero + 1).ToString();

            factura.valoriva = decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
            factura.Descuento = prefactura.Descuento;


            factura.Emisora = emisora;

            Empresa Receptor = new Empresa();
            Receptor.RFC = prefactura.Clientes.RFC;
            Receptor.RazonSocial = prefactura.Clientes.Nombre;
            Receptor.Calle = prefactura.Clientes.Calle;
            Receptor.NoExt = prefactura.Clientes.NumExt;
            Receptor.NoInt = prefactura.Clientes.NumInt;
            Receptor.CP = prefactura.Clientes.CP;
            //factura.RegimenFiscalReceptor = Convert.ToString(prefactura.Clientes.c_RegimenFiscal.ClaveRegimenFiscal);

            factura.Receptora = Receptor;


            List<Concepto> conceptos = new List<Concepto>();


            foreach (var detalle in detalles)
            {
                Concepto concepto1 = new Concepto();
                c_ClaveProductoServicioContext dbclave = new c_ClaveProductoServicioContext();
                c_ClaveProductoServicio cs = dbclave.c_ClaveProductoServicios.Find(detalle.Articulo.Familia.IDProdServ);


                concepto1.ClaveProdServ = cs.ClaveProdServ;
                concepto1.NoIdentificacion = detalle.Articulo.Cref;
                concepto1.Cantidad = detalle.Cantidad;
                concepto1.ClaveUnidad = detalle.Articulo.c_ClaveUnidad.ClaveUnidad;
                concepto1.Unidad = detalle.Articulo.c_ClaveUnidad.Nombre;
                concepto1.Descripcion = detalle.Articulo.Descripcion;
                concepto1.ValorUnitario = detalle.Costo;
                concepto1.Importe = detalle.Importe;
                concepto1.llevaiva = detalle.IVA;
                concepto1.Descuento = detalle.Descuento;
                conceptos.Add(concepto1);
            }

            factura.Listaconceptos.conceptos = conceptos;


            if (relacionados.Count > 0 && prefactura.c_TipoRelacion.ClaveTipoRelacion != string.Empty)
            {
                factura.cfdirelacionados.relacion = prefactura.c_TipoRelacion.ClaveTipoRelacion;

                foreach (var cuales in relacionados)
                {

                    factura.cfdirelacionados.uuid.Add(cuales.UUIDAnticipo);
                }
            }


            MultiFacturasSDK.MFSDK sdk = factura.construirfactura2();
            MultiFacturasSDK.SDKRespuesta respuesta = factura.timbrar(sdk);
            bool pasa = false;
            try
            {
                if (respuesta.Codigo_MF_Texto.Contains("OK"))
                {
                    pasa = true;
                    try
                    {
                        Encfacturas facturae = new EncfacturaContext().encfacturas.Where(s => s.UUID == respuesta.UUID).ToList().FirstOrDefault();
                        if (facturae == null)
                        {
                            throw new Exception("Posbiblemente no tiene factura");
                        }
                        else
                        {
                            pasa = false;
                            return Content("UUID Existente");

                        }
                    }
                    catch (Exception err)
                    {
                        pasa = true;
                    }
                }

                if (!pasa)
                {
                    throw new Exception("RESPUESTA SAT " + respuesta.Codigo_MF_Texto);
                  
                }
            }
            catch (Exception err)
            {

            }


            var reshtml = Server.HtmlEncode(respuesta.Codigo_MF_Texto + "->" + respuesta.CFDI);
            if (respuesta.Codigo_MF_Texto.Contains("OK"))
            {
                Generador.CreaPDF temp2 = new Generador.CreaPDF(respuesta.CFDI);
                Encfacturas elemento2 = new Encfacturas();

                elemento2.Fecha = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                elemento2.Serie = factura._serie;
                elemento2.Numero = int.Parse(factura._folio);
                elemento2.Nombre_cliente = prefactura.Clientes.Nombre;
                elemento2.Subtotal = decimal.Parse(temp2._templatePDF.subtotal.ToString());
                elemento2.Total = decimal.Parse(temp2._templatePDF.total.ToString());
                elemento2.IVA = decimal.Parse(temp2._templatePDF.totalImpuestosRetenidos.ToString());
                elemento2.Estado = "A";
                elemento2.Moneda = temp2._templatePDF.claveMoneda;
                elemento2.TC = Decimal.Parse(temp2._templatePDF.tipo_cambio);
                elemento2.pagada = false;



                elemento2.IDCliente = prefactura.IDCliente;




                elemento2.IDMetododepago = prefactura.c_MetodoPago.ClaveMetodoPago;

                elemento2.IDMoneda = prefactura.c_Moneda.IDMoneda;
                elemento2.RutaXML = respuesta.CFDI;

                elemento2.UUID = respuesta.UUID;
                elemento2.Estado = "A";
                elemento2.ConPagos = false;

                EncfacturaContext dbfa = new EncfacturaContext();

                elemento2.Descuento = prefactura.Descuento;
                elemento2.FechaRevision = System.DateTime.Now;

                CondicionesPago condicion = new CondicionesPagoContext().CondicionesPagos.Find(prefactura.IDCondicionesPago);


                elemento2.FechaVencimiento = elemento2.Fecha.AddDays(double.Parse(condicion.DiasCredito.ToString()));

                dbfa.encfacturas.Add(elemento2);
                dbfa.SaveChanges();

                db.Database.ExecuteSqlCommand("update folio set numero=" + elemento2.Numero + " where IDFolio=" + modelo.serie);

                Encfacturas facturagrabada = dbfa.encfacturas.ToList().Where(x => (x.UUID == elemento2.UUID)).ToList()[0];

                SaldoFactura saldo = new SaldoFactura();
                saldo.IDFactura = facturagrabada.ID;
                saldo.Serie = elemento2.Serie;
                saldo.Numero = elemento2.Numero;
                saldo.Total = elemento2.Total;
                saldo.ImporteSaldoAnterior = elemento2.Total;
                saldo.ImportePagado = 0;
                saldo.ImporteSaldoInsoluto = elemento2.Total;

                SaldoFacturaContext dbsp = new SaldoFacturaContext();
                dbsp.SaldoFacturas.Add(saldo);
                dbsp.SaveChanges();

                //GRABA XML 

                try
                {
                    Generador.CreaPDF temp = new Generador.CreaPDF(elemento2.RutaXML.ToString());
                    string fileName = temp._templatePDF.folioFiscalUUID + ".xml";

                    EscribeEnArchivo(elemento2.RutaXML.ToString(), fileName, true);
                }
                catch (Exception err)
                {
                    string mensajedeerror = err.Message;
                }




                dbsp.Database.ExecuteSqlCommand("update encprefactura SET IDFacturaDigital=" + facturagrabada.ID + ", SerieDigital='" + elemento2.Serie + "', NumeroDigital=" + elemento2.Numero + " where IDPrefactura=" + prefactura.IDPrefactura);


                try
                {
                    if (prefactura.c_TipoRelacion.ClaveTipoRelacion == "07")
                    {

                        return RedirectToAction("CrearAnticipoEgreso", "FacturaComplemento", new { UUID = elemento2.UUID });
                    }
                    else
                    {
                        return RedirectToAction("Index", "FacturaAll");
                    }
                }
                catch (Exception err)
                {
                    String mensaje = err.Message;
                    return RedirectToAction("Index", "FacturaAll");
                }


            }
            else
            {


                return Content(reshtml);
            }
        }
        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
     
        [HttpPost]
        public JsonResult Edititem(int id, decimal cantidad, string lote)
        {
            try
            {
                CarritoContext car = new CarritoContext();
                db.Database.ExecuteSqlCommand("update [dbo].[CarritoPrefactura] set [Cantidad]=" + cantidad + ", [lote]='" + lote + "'  where IDCarritoPrefactura=" + id);

                //List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                //int usuario = userid.Select(s => s.UserID).FirstOrDefault();


                //List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select Carrito.IDCliente,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();


                //ViewBag.carrito = pedido;

                //for (int i = 0; i < pedido.Count(); i++)
                //{

                //    db.Database.ExecuteSqlCommand("update [dbo].[CarritoPrefactura] set [Costo]=(select dbo.GetPrecio(" + ViewBag.carrito[i].IDCliente + "," + ViewBag.carrito[i].IDArticulo + ",0," + cantidad + ")) where IDCarritoPrefactura=" + id + " and usuario=" + usuario + "");

                //}
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }




        public void Grabaranticipoaplicado(int IDFacturaAnticipo, decimal subtotal, decimal iva, decimal total, decimal TC, int IDpreFacturaAplicada)
        {
            try
            {
                DateTime Fecha = DateTime.Now;
                string cadenadefecha = Fecha.Year + "/" + Fecha.Month + "/" + Fecha.Day;
                EncfacturaContext db = new EncfacturaContext();
                db.Database.ExecuteSqlCommand("insert into AplicacionAnticipos (IDFacturaAnticipo ,Fecha ,	Subtotal ,	Iva, Total ,TC ,IDpreFacturaAplicada ) values (" + IDFacturaAnticipo + ",sysdatetime()," + subtotal + "," + iva + "," + total + "," + TC + "," + IDpreFacturaAplicada + ")");
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
        }



        public decimal getCantidadxaplicar(string uuidAnticipo, decimal totalprefactura)
        {
            decimal aplicado = getCantidadaplicada(uuidAnticipo);
            decimal anticipo = getCantidaddelanticipo(uuidAnticipo);
            if (aplicado == 0)
            { return anticipo; }

            if (aplicado > 0 && aplicado < anticipo)
            { return anticipo - aplicado; }

            if (aplicado >= anticipo)
            { return 0M; }


            return 0;

        }

        public decimal getCantidadaplicada(string uuidAnticipo)
        {
            EncfacturaContext db = new EncfacturaContext();
            Encfacturas factura = new EncfacturaContext().Database.SqlQuery<Encfacturas>("select * from Encfacturas where UUID='" + uuidAnticipo + "'").ToList().FirstOrDefault();
            int IDFacturaAnticipo = factura.ID;
            decimal cantidadaplicada = 0;
            try
            {
                cantidadaplicada = db.Database.SqlQuery<ClsDatoDecimal>("select iIF(sum(total) is NULL,0,sum(total)) as Dato  from AplicacionAnticipos where idfacturaanticipo =" + IDFacturaAnticipo).ToList().FirstOrDefault().Dato;

            }
            catch (SqlException err)
            {
                string mensajeerror = err.Message;
            }

            return cantidadaplicada;

        }


        public decimal getCantidaddelanticipo(string uuidAnticipo)
        {
            EncfacturaContext db = new EncfacturaContext();
            Encfacturas factura = new EncfacturaContext().Database.SqlQuery<Encfacturas>("select * from Encfacturas where UUID='" + uuidAnticipo + "'").ToList().FirstOrDefault();


            return factura.Total;

        }

        public ActionResult Aplicaranticipo(int id, string searchString="")
        {
            EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Find(id);
            PrefacturaAnticipo anticipo = new PrefacturaContext().PrefacturaAnticipo.Where(s => s.IDPrefactura ==id).ToList().FirstOrDefault();

            aplicaranticipo aplica = new aplicaranticipo();

            aplica.UUID = anticipo.UUIDAnticipo;
            ViewBag.Total = prefactura.Total.ToString("C");
            ViewBag.searchString = searchString;

            aplica.Prefactura = prefactura.IDPrefactura;

            aplica.Monto = getCantidadxaplicar(anticipo.UUIDAnticipo, prefactura.Total);


            return View(aplica);
        }
        [HttpPost]

        public ActionResult Aplicaranticipo(aplicaranticipo aplicar)
        {
            try
            {
                decimal montoaplicar = aplicar.Monto;



                EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Find(aplicar.Prefactura);

                // prefactura.Descuento = aplicar.Monto;

                if ((prefactura.Total) > montoaplicar)
                {



                    var detalles = new PrefacturaContext().DetPrefactura.Where(s => s.IDPrefactura == prefactura.IDPrefactura).ToList();


                    decimal acubaseiva = 0;
                    decimal acudescuento = 0;
                    decimal acuiva = 0;
                    decimal acuimportetotal = 0;

                    foreach (DetPrefactura detprefactura in detalles)
                    {
                        decimal porcentaje = detprefactura.Importe / prefactura.Subtotal;
                        decimal montoadetalle = aplicar.Monto * porcentaje;

                        if (detprefactura.IVA)
                        {


                            detprefactura.Descuento = detprefactura.Descuento + Math.Round(montoadetalle / (1 + decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA)), 2);
                            decimal baseiva = detprefactura.Importe - detprefactura.Descuento;
                            acubaseiva += baseiva;
                            detprefactura.ImporteIva = Math.Round(baseiva * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                            detprefactura.ImporteTotal = detprefactura.Importe - detprefactura.Descuento + detprefactura.ImporteIva;
                        }
                        else
                        {

                            detprefactura.Descuento = detprefactura.Descuento + Math.Round(montoadetalle, 2);
                            detprefactura.ImporteIva = Math.Round(detprefactura.Importe * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                            decimal baseiva = detprefactura.Importe;
                            acubaseiva += baseiva;
                            detprefactura.ImporteTotal = detprefactura.Importe - detprefactura.Descuento + detprefactura.ImporteIva;
                        }
                        acudescuento = acudescuento + detprefactura.Descuento;
                        acuiva = acuiva + detprefactura.ImporteIva;
                        acuimportetotal += detprefactura.ImporteTotal;
                        new PrefacturaContext().Database.ExecuteSqlCommand("update detprefactura set Descuento =" + detprefactura.Descuento + ", ImporteIva=" + detprefactura.ImporteIva + ",ImporteTotal=" + detprefactura.ImporteTotal + " where IDDetPrefactura=" + detprefactura.IDDetPrefactura);

                    }

                    Encfacturas facturadelanticipo = new EncfacturaContext().encfacturas.Where(s => s.UUID == aplicar.UUID).FirstOrDefault();

                    decimal ivadescuento = Math.Round(acubaseiva * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                    Grabaranticipoaplicado(facturadelanticipo.ID, acubaseiva, ivadescuento, acubaseiva + ivadescuento, prefactura.TipoCambio, prefactura.IDPrefactura);


                    new PrefacturaContext().Database.ExecuteSqlCommand(" update Encprefactura set Descuento=" + acudescuento + ", IVA= " + acuiva + ", total= " + acuimportetotal + " where idprefactura =" + prefactura.IDPrefactura);
                }

                else
                {


                    var detalles = new PrefacturaContext().DetPrefactura.Where(s => s.IDPrefactura == prefactura.IDPrefactura).ToList();


                    decimal acubaseiva = 0;
                    decimal acudescuento = 0;
                    decimal acuiva = 0;
                    decimal acuimportetotal = 0;

                    foreach (DetPrefactura detprefactura in detalles)
                    {
                        decimal porcentaje = detprefactura.Importe / prefactura.Subtotal;
                        decimal montoadetalle = aplicar.Monto * porcentaje;

                        if (detprefactura.IVA)
                        {


                            //detprefactura.Descuento = detprefactura.Descuento + Math.Round(montoadetalle / (1 + decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA)), 2);
                            decimal baseiva = detprefactura.Importe - detprefactura.Descuento;
                            acubaseiva += baseiva;
                            detprefactura.ImporteIva = Math.Round(baseiva * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                            detprefactura.ImporteTotal = detprefactura.Importe - detprefactura.Descuento + detprefactura.ImporteIva;
                        }
                        else
                        {

                            //detprefactura.Descuento = detprefactura.Descuento + Math.Round(montoadetalle, 2);
                            detprefactura.ImporteIva = Math.Round(detprefactura.Importe * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                            decimal baseiva = detprefactura.Importe;
                            acubaseiva += baseiva;
                            detprefactura.ImporteTotal = detprefactura.Importe - detprefactura.Descuento + detprefactura.ImporteIva;
                        }
                        acudescuento = acudescuento + detprefactura.Descuento;
                        acuiva = acuiva + detprefactura.ImporteIva;
                        acuimportetotal += detprefactura.ImporteTotal;
                        /// no actuliza el detalle por que es el monto total de la factura
                       // new PrefacturaContext().Database.ExecuteSqlCommand("update detprefactura set Descuento =" + detprefactura.Descuento + ", ImporteIva=" + detprefactura.ImporteIva + ",ImporteTotal=" + detprefactura.ImporteTotal + " where IDDetPrefactura=" + detprefactura.IDDetPrefactura);

                    }

                    Encfacturas facturadelanticipo = new EncfacturaContext().encfacturas.Where(s => s.UUID == aplicar.UUID).FirstOrDefault();

                    decimal ivadescuento = Math.Round(acubaseiva * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                    Grabaranticipoaplicado(facturadelanticipo.ID, acubaseiva, ivadescuento, acubaseiva + ivadescuento, prefactura.TipoCambio, prefactura.IDPrefactura);

                    // no graba el descuento o la factura saldria en 0
                    // new PrefacturaContext().Database.ExecuteSqlCommand(" update Encprefactura set Descuento=" + acudescuento + ", IVA= " + acuiva + ", total= " + acuimportetotal + " where idprefactura =" + prefactura.IDPrefactura);
                }







                return RedirectToAction("Index", new { searchString = ViewBag.searchString });


            }
            catch (Exception err)
            {
                ViewBag.Mesajederror = err.Message;
                return View();
            }
        }

        //[HttpPost]

        //public ActionResult Aplicaranticipo(aplicaranticipo aplicar)
        //{
        //    try
        //    {
        //        decimal montoaplicar = aplicar.Monto;



        //        EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Find(aplicar.Prefactura);

        //      // prefactura.Descuento = aplicar.Monto;

        //        if ((prefactura.Total) > montoaplicar)
        //        {



        //            var detalles = new PrefacturaContext().DetPrefactura.Where(s => s.IDPrefactura == prefactura.IDPrefactura).ToList();


        //            decimal acubaseiva = 0;
        //            decimal acudescuento = 0;
        //            decimal acuiva = 0;
        //            decimal acuimportetotal = 0;

        //            foreach (DetPrefactura detprefactura in detalles)
        //            {
        //                decimal porcentaje = detprefactura.Importe / prefactura.Subtotal;
        //                decimal montoadetalle = aplicar.Monto * porcentaje;

        //                if (detprefactura.IVA)
        //                {


        //                    detprefactura.Descuento = detprefactura.Descuento + Math.Round(montoadetalle / (1 + decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA)), 2);
        //                    decimal baseiva = detprefactura.Importe - detprefactura.Descuento;
        //                    acubaseiva += baseiva;
        //                    detprefactura.ImporteIva = Math.Round(baseiva * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
        //                    detprefactura.ImporteTotal = detprefactura.Importe - detprefactura.Descuento + detprefactura.ImporteIva;
        //                }
        //                else
        //                {

        //                    detprefactura.Descuento = detprefactura.Descuento + Math.Round(montoadetalle, 2);
        //                    detprefactura.ImporteIva = Math.Round(detprefactura.Importe * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
        //                    decimal baseiva = detprefactura.Importe ;
        //                    acubaseiva += baseiva;
        //                    detprefactura.ImporteTotal = detprefactura.Importe - detprefactura.Descuento + detprefactura.ImporteIva;
        //                }
        //                acudescuento = acudescuento + detprefactura.Descuento;
        //                acuiva = acuiva + detprefactura.ImporteIva;
        //                acuimportetotal += detprefactura.ImporteTotal;
        //                new PrefacturaContext().Database.ExecuteSqlCommand("update detprefactura set Descuento =" + detprefactura.Descuento + ", ImporteIva=" + detprefactura.ImporteIva + ",ImporteTotal=" + detprefactura.ImporteTotal + " where IDDetPrefactura=" + detprefactura.IDDetPrefactura);

        //            }

        //            Encfacturas facturadelanticipo = new EncfacturaContext().encfacturas.Where(s => s.UUID == aplicar.UUID).FirstOrDefault();

        //            decimal ivadescuento = Math.Round( acubaseiva * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA),2);
        //            Grabaranticipoaplicado( facturadelanticipo.ID, acubaseiva, ivadescuento , acubaseiva +ivadescuento,prefactura.TipoCambio, prefactura.IDPrefactura);


        //            new PrefacturaContext().Database.ExecuteSqlCommand(" update Encprefactura set Descuento=" + acudescuento + ", IVA= " + acuiva + ", total= " + acuimportetotal + " where idprefactura =" + prefactura.IDPrefactura);
        //        }

        //        else
        //        {


        //            var detalles = new PrefacturaContext().DetPrefactura.Where(s => s.IDPrefactura == prefactura.IDPrefactura).ToList();


        //            decimal acubaseiva = 0;
        //            decimal acudescuento = 0;
        //            decimal acuiva = 0;
        //            decimal acuimportetotal = 0;

        //            foreach (DetPrefactura detprefactura in detalles)
        //            {
        //                decimal porcentaje = detprefactura.Importe / prefactura.Subtotal;
        //                decimal montoadetalle = aplicar.Monto * porcentaje;

        //                if (detprefactura.IVA)
        //                {


        //                    detprefactura.Descuento = detprefactura.Descuento + Math.Round(montoadetalle / (1 + decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA)), 2);
        //                    decimal baseiva = detprefactura.Importe - detprefactura.Descuento;
        //                    acubaseiva += baseiva;
        //                    detprefactura.ImporteIva = Math.Round(baseiva * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
        //                    detprefactura.ImporteTotal = detprefactura.Importe - detprefactura.Descuento + detprefactura.ImporteIva;
        //                }
        //                else
        //                {

        //                    detprefactura.Descuento = detprefactura.Descuento + Math.Round(montoadetalle, 2);
        //                    detprefactura.ImporteIva = Math.Round(detprefactura.Importe * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
        //                    decimal baseiva = detprefactura.Importe;
        //                    acubaseiva += baseiva;
        //                    detprefactura.ImporteTotal = detprefactura.Importe - detprefactura.Descuento + detprefactura.ImporteIva;
        //                }
        //                acudescuento = acudescuento + detprefactura.Descuento;
        //                acuiva = acuiva + detprefactura.ImporteIva;
        //                acuimportetotal += detprefactura.ImporteTotal;
        //                /// no actuliza el detalle por que es el monto total de la factura
        //               // new PrefacturaContext().Database.ExecuteSqlCommand("update detprefactura set Descuento =" + detprefactura.Descuento + ", ImporteIva=" + detprefactura.ImporteIva + ",ImporteTotal=" + detprefactura.ImporteTotal + " where IDDetPrefactura=" + detprefactura.IDDetPrefactura);

        //            }

        //            Encfacturas facturadelanticipo = new EncfacturaContext().encfacturas.Where(s => s.UUID == aplicar.UUID).FirstOrDefault();

        //            decimal ivadescuento = Math.Round(acubaseiva * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
        //            Grabaranticipoaplicado(facturadelanticipo.ID, acubaseiva, ivadescuento, acubaseiva + ivadescuento, prefactura.TipoCambio, prefactura.IDPrefactura);

        //            // no graba el descuento o la factura saldria en 0
        //           // new PrefacturaContext().Database.ExecuteSqlCommand(" update Encprefactura set Descuento=" + acudescuento + ", IVA= " + acuiva + ", total= " + acuimportetotal + " where idprefactura =" + prefactura.IDPrefactura);
        //        }







        //        return RedirectToAction("Index", new { searchString = ViewBag.searchString });


        //    }
        //    catch(Exception err)
        //    {
        //        ViewBag.Mesajederror = err.Message;
        //        return View();
        //    }
        //}

        public static void EscribeEnArchivo(string contenido, string rutaArchivo, bool sobrescribir = true)
        {
            string archivoxml = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + rutaArchivo);

            using (FileStream fs = System.IO.File.Create(archivoxml))
            {
                AddText(fs, contenido);
            }

        }

        public ActionResult CambiarUsoCFDI(int id)
        {
            ViewBag.idprefactura= id;
            EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Find(id);
         
           
            if (prefactura == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS, "IDUsoCFDI", "Descripcion", prefactura.IDUsoCFDI);

            return View(prefactura);
        }

        // POST: Vendedor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarUsoCFDI(EncPrefactura prefactura, int id)
        {
            PrefacturaContext db1 = new PrefacturaContext();
            try
            {
                var elemento = db1.EncPrefactura.Single(m => m.IDPrefactura == id);
               

                if (TryUpdateModel(elemento))
                {
                    //string pass = MD5P(vendedor.Perfil);
                    //db.Database.ExecuteSqlCommand("update [dbo].[User] set [Username]='" + vendedor.Mail + "',[Password]='" + pass + "',[EmailID]='" + vendedor.Nombre + "' where Session='" + ses + "'");
                    db1.SaveChanges();
                    return RedirectToAction("Index");
                }
                db1.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS, "IDUsoCFDI", "Descripcion", prefactura.IDUsoCFDI);

                return View();
            }


        }



        public ActionResult CambiarFormaPago(int id)
        {
            ViewBag.idprefactura = id;

            EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Find(id);


            if (prefactura == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", prefactura.IDFormapago);

            return View(prefactura);
        }

        // POST: Vendedor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarFormaPago(EncPrefactura prefactura, int id)
        {
            PrefacturaContext db1 = new PrefacturaContext();
            try
            {
                var elemento = db1.EncPrefactura.Single(m => m.IDPrefactura == id);


                if (TryUpdateModel(elemento))
                {
                    //string pass = MD5P(vendedor.Perfil);
                    //db.Database.ExecuteSqlCommand("update [dbo].[User] set [Username]='" + vendedor.Mail + "',[Password]='" + pass + "',[EmailID]='" + vendedor.Nombre + "' where Session='" + ses + "'");
                    return RedirectToAction("Index");
                }
                db1.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", prefactura.IDFormapago);

                return View();
            }


        }
        //public void PdfPrefactura(int id)
        //{

        //    EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Find(id);
        //    DocumentoPre x = new DocumentoPre();

        //    x.claveMoneda = prefactura.c_Moneda.Descripcion;
        //    x.descuento = 0;
        //    x.fecha = prefactura.Fecha.ToShortDateString();
        //    x.fechaRequerida = prefactura.Fecha.ToShortDateString();
        //    x.Cliente = prefactura.Clientes.Nombre;
        //    x.formaPago = prefactura.c_FormaPago.ClaveFormaPago;
        //    x.metodoPago = prefactura.c_MetodoPago.ClaveMetodoPago;
        //    x.RFCCliente = prefactura.Clientes.RFC;
        //    x.TelefonoCliente = prefactura.Clientes.Telefono;
        //    x.total = float.Parse(prefactura.Total.ToString());
        //    x.subtotal = float.Parse(prefactura.Subtotal.ToString());
        //    x.tipo_cambio = prefactura.TipoCambio.ToString();
        //    x.serie = "";
        //    x.folio = prefactura.IDPrefactura.ToString();
        //    x.UsodelCFDI = prefactura.c_UsoCFDI.Descripcion;
        //    x.Almacen = prefactura.Almacen.Descripcion;
        //    x.IDPrefactura = prefactura.IDPrefactura;
        //    x.Empresa = prefactura.Almacen.Telefono;
        //    x.condicionesdepago = prefactura.CondicionesPago.Descripcion;


        //    ImpuestoPre iva = new ImpuestoPre();
        //    iva.impuesto = "IVA";
        //    iva.tasa = float.Parse(SIAAPI.Properties.Settings.Default.ValorIVA.ToString());
        //    iva.importe = float.Parse(prefactura.IVA.ToString());


        //    x.impuestos.Add(iva);

        //    EmpresaContext dbe = new EmpresaContext();

        //    var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
        //    x.Empresa = empresa.RazonSocial;
        //    x.Telefono = empresa.Telefono;
        //    x.RFC = empresa.RFC;
        //    x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
        //    x.firmadefinanzas = empresa.Director_finanzas;
        //    x.firmadecompras = empresa.Persona_de_Compras + "";

        //    List<DetPrefactura> detalles = db.Database.SqlQuery<DetPrefactura>("select * from [dbo].[DetPrefactura] where [IDPrefactura]= " + id).ToList();

        //    int contador = 1;
        //    foreach (var item in detalles)
        //    {
        //        ProductoPre producto = new ProductoPre();
        //        Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);
        //        Almacen alma = new AlmacenContext().Almacenes.Find(item.IDAlmacen);
        //        Caracteristica carateristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + item.Caracteristica_ID).ToList().FirstOrDefault();
        //        producto.IDPresentacion = carateristica.IDPresentacion;
        //        producto.ClaveProducto = arti.Cref;
        //        producto.idarticulo = arti.IDArticulo;
        //        producto.c_unidad = arti.c_ClaveUnidad.Nombre;
        //        producto.cantidad = item.Cantidad.ToString();
        //        producto.almacen = alma.CodAlm;
        //        producto.descripcion = arti.Descripcion;
        //        producto.valorUnitario = float.Parse(item.Costo.ToString());
        //        producto.v_unitario = float.Parse(item.Costo.ToString());
        //        producto.importe = float.Parse(item.Importe.ToString());
        //        ///
        //        if (item.Lote != "")
        //        {
        //            producto.Observacion = "Lote: " + item.Lote;
        //        }
        //        producto.Presentacion = item.presentacion; //item.presentacion;
        //        ///
        //        producto.numIdentificacion = contador.ToString();
        //        contador++;

        //        x.productos.Add(producto);

        //    }



        //    System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

        //    try
        //    {


        //        CreaPrePDF documento = new CreaPrePDF(logoempresa, x);

        //    }
        //    catch (Exception err)
        //    {
        //        String mensaje = err.Message;
        //    }
        //    RedirectToAction("Index");

        //}




    }
}