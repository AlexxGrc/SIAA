using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SIAAPI.ViewModels.Cfdi;
using System.IO;
using System.Text;
using SIAAPI.ViewModels.Comercial;
using System.Net;
using SIAAPI.Reportes;
using SIAAPI.Models.Administracion;
using iTextSharp.text;
using System.Xml;
using SIAAPI.clasescfdi;

namespace SIAAPI.Controllers.Cfdi
{
    public class FacturaCController : Controller
    {
        // GET: FacturaC
        EncfacturaContext db = new SIAAPI.Models.Cfdi.EncfacturaContext();
        VPagoFacturaCContext dbp = new VPagoFacturaCContext();
        private PedidoContext dbpe = new PedidoContext();
        private ClientesContext prov = new ClientesContext();

        [Authorize(Roles = "Cliente")]

        public ActionResult Index(string Numero, string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string TipoFac, string Estado = "A")
        {
            int p = 0;
            try
            {
                SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDCliente] as Dato from [dbo].[Clientes] where [RFC]='" + User.Identity.Name + "'").ToList()[0];
                p = c.Dato;
            }
            catch(Exception err)
            {
                string mensajederror = err.Message;
                ViewBag.Error = "Posiblemente tengas dos usuarios con el mismo mail o acceso";
                return RedirectToAction("Error", new { Mensaje = "Posiblemente tengas dos usuarios con el mismo mail o acceso" });
            }
          
            List<Clientes> cliente = db.Database.SqlQuery<Clientes>("select * from [dbo].[Clientes] where [IDCliente]=" + p + "").ToList();
            ViewBag.cliente = cliente;

            string ConsultaSql = "select top 200 * from Encfacturas ";
            string Filtro = " where [IDCliente] = " + p + " and Estado = 'A' ";
            string Orden = " order by Fecha desc, serie, numero desc ";

            //Facturas Pagadas
            var FacPagLst = new List<SelectListItem>();
            FacPagLst.Add(new SelectListItem { Text = "Pagada", Value = "SI" });
            FacPagLst.Add(new SelectListItem { Text = "NoPagada", Value = "NO" });
            ViewData["FacPag"] = FacPagLst;
            ViewBag.FacPag = new SelectList(FacPagLst, "Value", "Text");
            ViewBag.FacPagseleccionada = FacPag;

            //Facturas Pagadas
            var TipoFacLst = new List<SelectListItem>();
            TipoFacLst.Add(new SelectListItem { Text = "Facturas", Value = "B" });
            TipoFacLst.Add(new SelectListItem { Text = "Notas de Crédito", Value = "N" });
            ViewData["TipoFac"] = TipoFacLst;
            ViewBag.TipoFac = new SelectList(TipoFacLst, "Value", "Text");
            ViewBag.TipoFacseleccionada = TipoFac;

            /// Resumen
            string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from encfacturas ";
            string ConsultaAgrupado = "group by Moneda order by Moneda ";

            ///Busqueda///
            //Buscar por numero
            if (Numero == "")
            {
                Numero = string.Empty;
            }
            if (!String.IsNullOrEmpty(Numero))
            {
                if (Filtro != string.Empty)
                {
                    Filtro += " " + "and Numero= '" + Numero + "'";
                }

            }

            ///Busqueda: Factura pagada/no pagada 
            if (TipoFac != "Todas")
            {
                if (TipoFac == "B")
                {

                    Filtro += " " + "and Serie='B' ";

                }
                if (TipoFac == "N")
                {
                    Filtro += " " + "and Serie='N' ";
                }
            }

            ///Busqueda: Factura pagada/no pagada 
            if (FacPag != "Todas")
            {
                if (FacPag == "SI")
                {

                    Filtro += " " + "and pagada='1' ";

                }
                if (FacPag == "NO")
                {
                    Filtro += " " + "and pagada='0' ";
                }
            }


            ///Busqueda:Periodo de fechas

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
                if (Filtro == string.Empty)
                {
                    Filtro = " where  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                }
                else
                {
                    Filtro += " and  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                }
            }


            ViewBag.CurrentSort = sortOrder;
            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "Numero" : "";
            ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";

            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;

            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

            var elementos = db.Database.SqlQuery<Encfacturas>(cadenaSQl).ToList();

            var cadenaSaldoI = "select  e.Moneda as MonedaOrigen, (SUM(s.ImporteSaldoInsoluto)) as SaldoActual,  (SUM(s.ImporteSaldoInsoluto)* e.TC) as TotalenPesos   from EncFacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente=" + p + " and e.moneda='MXN' and e.Estado='A' and  e.Notacredito='0' group by e.Moneda, e.tc order by e.Moneda";

            List<ResumenSaldoInsoluto> dataSI = db.Database.SqlQuery<ResumenSaldoInsoluto>(cadenaSaldoI).ToList();
            ViewBag.sumSaldoI = dataSI;
            ViewBag.sumatoria = "";
            try
            {

                var SumaLst = new List<string>();
                var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
                List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
                ViewBag.sumatoria = data;

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.encfacturas.OrderBy(e => e.Serie).Count();// Total number of elements

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
            return View(elementos.ToPagedList(pageNumber, pageSize));
        }

        //public FileResult Descargarxml(int id)
        //{
        //    // Obtener contenido del archivo
        //    var elemento = db.encfacturas.Single(m => m.ID == id);

        //    var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaXML));

        //    return File(stream, "text/plain", "Factura" + elemento.Serie + elemento.Numero + ".xml");
        //}
        //public ActionResult DescargarPdf(int id)
        //{
        //    // Obtener contenido del archivo
        //    var elemento = db.encfacturas.Single(m => m.ID == id);

        //    var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaXML));
        //    StreamReader reader = new StreamReader(stream);
        //    String contenidoxml = reader.ReadToEnd();

        //    //Console.WriteLine("Esto es mi mensaje : entre aqui" );
        //    // toma la empresa que tenga el id 1 via de mientras 
        //    var empresa = db.Empresa.Single(m => m.IDEmpresa == 2);

        //    System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

        //    Generador.CreaPDF documento = new Generador.CreaPDF(contenidoxml, logoempresa,empresa.Telefono, true);
        //    //Dispose();
        //    //RedirectToAction("Index");
        //    string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
        //    return new FilePathResult(documento.nombreDocumento, contentType);
        //}

        public void Descargarxml(int id)
        {
            // Obtener contenido del archivo
            var elemento = db.encfacturas.Single(m => m.ID == id);

            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=Factura-" + elemento.Serie + elemento.Numero + ".Xml");
            System.Web.HttpContext.Current.Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + elemento.UUID + ".Xml"));

        }

        public ActionResult DescargarPdf(int id)
        {
            // Obtener contenido del archivo
            var elemento = db.encfacturas.Single(m => m.ID == id);



            string rutaArchivo = elemento.UUID + ".xml";
            string fileName = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + rutaArchivo);
            string xmlString = System.IO.File.ReadAllText(fileName);

            //Console.WriteLine("Esto es mi mensaje : entre aqui" );
            // toma la empresa que tenga el id 1 via de mientras 
            var empresa = db.Empresa.Single(m => m.IDEmpresa == 2);

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);



            List<EncPrefactura> prefactura = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("select * from encprefactura where Seriedigital='" + elemento.Serie + "' and [NumeroDigital]=" + elemento.Numero).ToList();
            Generador.CreaPDF documento = null;
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            if (prefactura.Count == 0)  /// checa prefactura
            {
                documento = new Generador.CreaPDF(xmlString, logoempresa, "Tel. " + empresa.Telefono + " " + empresa.mail, true);

                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            else
            {
                EncPrefactura prefact = prefactura[0];
                documento = new Generador.CreaPDF(xmlString, logoempresa, prefact, "Tel. " + empresa.Telefono + " " + empresa.mail, true);

                return new FilePathResult(documento.nombreDocumento, contentType);
            }

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


        public ActionResult VPagos(int id, List<VEncPagos> enc)
        {
            try
            {

                VEncPagos encFac = db.Database.SqlQuery<VEncPagos>("select ID, Nombre_Cliente, Numero as NoFactura, Total from dbo.EncFacturas where ID = " + id + "").ToList()[0];

                ViewBag.Nombre = encFac.Nombre_cliente;
                ViewBag.NoFactura = encFac.NoFactura;
                ViewBag.Total = encFac.Total;

            }
            catch (Exception err)
            {
                string MENSAJE = err.Message;
                ViewBag.Nombre = "";
                ViewBag.NoFactura = "";
                ViewBag.Total = "";
            }

            List<VPagos> elemento = db.Database.SqlQuery<VPagos>("select P.[FechaPago], F.Folio as FolioP, D.ImporteSaldoInsoluto, D.importepagado, D.NoParcialidad from dbo.PagoFactura as P left join ([dbo].[DocumentoRelacionado]as D left join [dbo].[PagoFacturaSPEI] as S on D.IDPagoFactura = S.IDPagoFactura) on P.[IDPagoFactura] = D.[IDPagoFactura] where  D.IDFactura = " + id + " order by NoParcialidad ").ToList();
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);

        }


        ////// INDEX
        /////////////////////////////////////////////LISTA PAGO FACTURA//////////////////////////////////////
        public ActionResult IndexPago(string Numero, string Fechainicio, string Fechafinal, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString)
        {
            SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDCliente] as Dato from [dbo].[Clientes] where [RFC]='" + User.Identity.Name + "'").ToList()[0];
            int p = c.Dato;
            List<Clientes> cliente = db.Database.SqlQuery<Clientes>("select * from [dbo].[Clientes] where [IDCliente]=" + p + "").ToList();
            ViewBag.cliente = cliente;

            string ConsultaSql = "SELECT * FROM [dbo].[VPagoFacturaC]";
            string Filtro = "where[IDCliente] = " + p + " and  StatusPago='A' ";
            string Orden = " order by  IDPagoFactura desc ";
            string cadenaSQl = string.Empty;
            var FacPagLst = new List<SelectListItem>();
            var EstadoLst = new List<SelectListItem>();

            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "IDPagoFactura" : "";
            ViewBag.FechaSortParm = sortOrder == "FechaPago" ? "FechaPago" : "";
            ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "";

            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;
            ViewBag.Numero = Numero;
 

            ViewBag.CurrentSort = sortOrder;

            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "IDPagoFactura" : "";
            ViewBag.FechaSortParm = sortOrder == "FechaPago" ? "FechaPago" : "";
            ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "";

            ViewBag.SearchString = searchString;
            ViewBag.CurrentFilter = searchString;

            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }
            try
            {

                var SumaLst = new List<string>();
               

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            if (Numero == "")
            {
                Numero = string.Empty;
            }
            //Buscar por numero
            if (!String.IsNullOrEmpty(Numero))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where Folio =" + Numero + "";
                }
                else
                {
                    Filtro += " and  Folio =" + Numero + "";
                }
            }
            ///Busqueda:Periodo de fechas

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
                if (Filtro == string.Empty)
                {
                    Filtro = " where  FechaPago BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                }
                else
                {
                    Filtro += " and  FechaPago BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                }
            }
            cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

            var elementos = dbp.Database.SqlQuery<VPagoFacturaC>(cadenaSQl).ToList();


            //Paginación
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbp.VPagoFacturasC.OrderBy(e => e.IDPagoFactura).Count();// Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "50", Value = "50", Selected = true },
                new SelectListItem { Text = "75", Value = "75" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 50);
            ViewBag.psize = pageSize;

            //Paginación

           
            return View(elementos.ToPagedList(pageNumber, pageSize));
        }
        //////   FIN INDEX


        public ActionResult VPagosC(int id, List<VEncPagos> enc)
        {
            

            try
            {


                VEncPagos encFac = db.Database.SqlQuery<VEncPagos>("select ID, Nombre_Cliente, Numero as NoFactura, Total from dbo.EncFacturas where ID = " + id+ "").ToList()[0];

                ViewBag.Nombre = encFac.Nombre_cliente;
                ViewBag.NoFactura = encFac.NoFactura;
                ViewBag.Total = encFac.Total;


            }
            catch (Exception err)
            {
                string MENSAJE = err.Message;
                ViewBag.Nombre = "";
                ViewBag.NoFactura = "";
                ViewBag.Total = "";
            }

            string cadenaP = "select P.[FechaPago], P.Folio as FolioP, D.ImporteSaldoInsoluto, D.importepagado, D.NoParcialidad from dbo.PagoFactura as P left join ([dbo].[DocumentoRelacionado]as D left join [dbo].[PagoFacturaSPEI] as S on D.IDPagoFactura = S.IDPagoFactura) on P.[IDPagoFactura] = D.[IDPagoFactura] where  D.numero = " + ViewBag.NoFactura + " order by NoParcialidad";
            List<VPagos> elemento = db.Database.SqlQuery<VPagos>(cadenaP).ToList();
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }
      
        public ActionResult IndexPedido(string idmoneda, string Status, string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDCliente] as Dato from [dbo].[Clientes] where [RFC]='" + User.Identity.Name + "'").ToList()[0];
            int p = c.Dato;
            List<Clientes> cliente = db.Database.SqlQuery<Clientes>("select * from [dbo].[Clientes] where [IDCliente]=" + p + "").ToList();
            ViewBag.cliente = cliente;
            string ConsultaSql = "select * from dbo.EncPedido ";
            string Filtro = "where[IDCliente] = " + p + " ";
            string Orden = " order by  IDPedido desc ";
            string cadenaSQl = string.Empty;
            string CadenaResumen = string.Empty;
            string ConsultaSqlR = "select(select ClaveMoneda FROM C_MONEDA WHERE IDMoneda = EncPedido.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPedido";
            string FiltroR= "where[IDCliente] = " + p + " ";
            String GrupoR = " group by EncPedido.IDMoneda";

  
              var idmonedaQry = db.Database.SqlQuery<c_Moneda>("select distinct M.* from c_Moneda as M inner join EncPedido as  E on M.IDMoneda= E.IDMoneda WHERE E.IDCliente= " + p + "").ToList();
            List < SelectListItem > li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "Todas", Value = "0" });
            foreach (var m in idmonedaQry)
            {
                li.Add(new SelectListItem { Text = m.ClaveMoneda, Value = m.IDMoneda.ToString() });
            }
            ViewBag.idmoneda = li;
            ViewBag.idmonedaSeleccionada = idmoneda;


            var StaLst = new List<string>();
            var StaQry = from d in dbpe.EncPedidos
                         where d.Status != "Cancelado" && d.Status != "Inactivo"
                         orderby d.IDPedido
                         select d.Status;
            StaLst.Add(" ");
            StaLst.AddRange(StaQry.Distinct());
            ViewBag.Status = new SelectList(StaLst);
           ViewBag.StatusSeleccionada = Status;
            cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

            var elementos = dbp.Database.SqlQuery<EncPedido>(cadenaSQl).ToList();


            //elementos = elementos.OrderByDescending(s => s.IDOrdenCompra);

         

            ///tabla filtro: Divisa
            if (!String.IsNullOrEmpty(idmoneda))
            {
                int num = int.Parse(idmoneda.ToString());
                if(num == 0)
                {
                }
                else
                     {
                        Filtro += " and IDMoneda=" + idmoneda + "";

                    }
            }

            ///tabla filtro: Status
            if (!String.IsNullOrEmpty(Status))
            {

                Filtro += " and Status='" + Status + "'";
                FiltroR += " and EncPedido.Status = '" + Status + "'";
            }
            else
            {
                FiltroR += " and EncPedido.Status <> 'Cancelado'";

            }



            ViewBag.CurrentSort = sortOrder;
            ViewBag.PedidoSortParm = String.IsNullOrEmpty(sortOrder) ? "Pedido" : "Pedido";
       
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
                case "Pedido":

                    Orden = string.Empty;
                    Orden = " order by  IDPedido asc ";
                    break;
                
                default:
                    Orden = string.Empty;
                    Orden = " order by  IDPedido desc ";
                    break;
            }
            
            cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;
            elementos = dbp.Database.SqlQuery<EncPedido>(cadenaSQl).ToList();
            CadenaResumen = ConsultaSqlR + " " + FiltroR + " " + GrupoR;

            var resumen = db.Database.SqlQuery<ResumenFac>(CadenaResumen).ToList();
            ViewBag.sumatoria = resumen;
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbpe.EncPedidos.OrderBy(e => e.IDPedido).Count(); // Total number of elements

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
            List<VDetPedido> pedido = db.Database.SqlQuery<VDetPedido>("select DetPedido.IDDetPedido,Articulo.GeneraOrden,DetPedido.Suministro,DetPedido.GeneraOrdenP,DetPedido.IDRemision,DetPedido.IDPrefactura,DetPedido.Status,Articulo.MinimoVenta,Caracteristica.ID as Caracteristica_ID,DetPedido.IDDetExterna,DetPedido.IDPedido,Articulo.Descripcion as Articulo,DetPedido.Cantidad,DetPedido.Costo,DetPedido.CantidadPedida,DetPedido.Descuento,DetPedido.Importe,DetPedido.IVA,DetPedido.ImporteIva,DetPedido.ImporteTotal, DetPedido.Nota,DetPedido.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion from  DetPedido INNER JOIN Caracteristica ON DetPedido.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPedido='" + id + "'").ToList();

            ViewBag.req = pedido;

            var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPedido.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncPedido inner join Clientes on Clientes.IDCliente=EncPedido.IDCliente where EncPedido.IDPedido='" + id + "' group by EncPedido.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EncPedido encPedido = dbpe.EncPedidos.Find(id);
            if (encPedido == null)
            {
                return HttpNotFound();
            }
            return View(encPedido);
        }

        public ActionResult PdfPedido(int id)
        {

            EncPedido pedido = new PedidoContext().EncPedidos.Find(id);
            DocumentoPedido x = new DocumentoPedido();

            x.claveMoneda = pedido.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = pedido.Fecha.ToShortDateString();
            x.fechaRequerida = pedido.Fecha.ToShortDateString();
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

                c_ClaveProductoServicio claveprodsat = db.Database.SqlQuery<c_ClaveProductoServicio>("select c_ClaveProductoServicio.* from (Articulo inner join Familia on articulo.IDFamilia= Familia.IDFamilia) inner join c_ClaveProductoServicio on c_ClaveProductoServicio.IDProdServ= Familia.IDProdServ where Articulo.IDArticulo= " + item.IDArticulo).ToList()[0];
                producto.ClaveProducto = claveprodsat.ClaveProdServ;

                producto.c_unidad = arti.c_ClaveUnidad.ClaveUnidad;
                producto.cantidad = item.Cantidad.ToString();
                producto.descripcion = arti.Descripcion;
                producto.valorUnitario = float.Parse(item.Costo.ToString());
                producto.v_unitario = float.Parse(item.Costo.ToString());
                producto.importe = float.Parse(item.Importe.ToString());

                try
                {
                    ClsDatoEntero idorden = db.Database.SqlQuery<ClsDatoEntero>("select IDOrden as Dato from OrdenProduccion where IDPedido=" + item.IDDetPedido + "").ToList()[0];


                    producto.OProduccion = idorden.Dato;



                }
                catch (Exception E)
                {
                    //producto.OProduccion = 0;
                }
                //
                producto.Presentacion = item.Presentacion; //item.presentacion;
                producto.Nota = item.Nota;
                ///
                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            //try
            //{


            CreaPedidoPDF documentop = new CreaPedidoPDF(logoempresa, x);

            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            //return new FilePathResult(documentop.nombreDocumento, contentType);

            //}
            //catch (Exception err)
            //{

            //}

            return RedirectToAction("IndexPedido");

        }

        /////////////////////////////////////////////DETALLES PAGO FACTURA//////////////////////////////////////

     

        public ActionResult Error(string mensaje)
        {
            ViewBag.Error = mensaje;
            return View();

        }

        public FileResult DescargarxmlPago(int id)
        {
           VPagoFacturaCContext db = new VPagoFacturaCContext();
            // Obtener contenido del archivo
            VPagoFacturaC elemento = db.VPagoFacturasC.Find(id);

            var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaXML));

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(elemento.RutaXML);



            XmlNodeList comprobante = doc.GetElementsByTagName("cfdi:Comprobante");
            string folio = "";

            if (((XmlElement)comprobante[0]).GetAttribute("Folio") != null)
            { folio = ((XmlElement)comprobante[0]).GetAttribute("Folio"); }

            clasescfdi.ClsXmlPagos pa = new ClsXmlPagos(elemento.RutaXML);

            return File(stream, "text/plain", "FacturaPagoP" + folio + ".xml");
        }

        public ActionResult DescargarPDFPago(int id)
        {
            VPagoFacturaCContext db = new VPagoFacturaCContext();
            // Obtener contenido del archivo
            VPagoFacturaC elemento = db.VPagoFacturasC.Find(id);


            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);


            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            PDFPago pago = new PDFPago(elemento.RutaXML, logoempresa, elemento.IDPagoFactura, "(55) 262 04200");

            //RedirectToAction("Index");
            //string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;

            //return new FilePathResult(pago.nombreDocumento, contentType);

            byte[] fileBytes = System.IO.File.ReadAllBytes(pago.nombreDocumento);
            return File(fileBytes, "application/pdf");

        }

    }
}
