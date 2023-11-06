
using MultiFacturasSDK;
using PagedList;
using SIAAPI.clasescfdi;
using SIAAPI.Facturas;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using static SIAAPI.Models.Comercial.ClienteRepository;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SIAAPI.Reportes;
using System.Globalization;
using SIAAPI.Models.Login;

namespace SIAAPI.Controllers.Cfdi
{
    [Authorize(Roles = "Administrador,Facturacion,Gerencia,Ventas,Sistemas,Almacenista,Comercial")]
    public class PagoFacturaController : Controller
    {

        private PagoFacturaContext db = new PagoFacturaContext();

        // GetTC
    

        ////// INDEX
        /////////////////////////////////////////////LISTA PAGO FACTURA ELECTRÓNICO//////////////////////////////////////
        public ActionResult Index(string Numero, string Numerof, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A")
        {
            var FacPagLst = new List<SelectListItem>();
            var EstadoLst = new List<SelectListItem>();
            //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });

            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });

            ViewData["Estado"] = EstadoLst;
            ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

            ViewData["FacPag"] = FacPagLst;
            ViewBag.FacPag = new SelectList(FacPagLst, "Value", "Text");



            //Buscar Cliente
            var ClieLst = new List<string>();
            var ClieQry = from d in new ClientesContext().Clientes
                          orderby d.Nombre
                          select d.Nombre;
           // ClieLst.AddRange(ClieQry.Distinct());
            ViewBag.ClieFac = new SelectList(ClieQry);

            ViewBag.CurrentSort = sortOrder;

            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "IDPagoFactura" : "";
            ViewBag.FechaSortParm = sortOrder == "FechaPago" ? "FechaPago" : "";
            ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "";

            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;

            ViewBag.Clienteseleccionado = ClieFac;


            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.PagoFacturas.OrderBy(e => e.Serie).Count();// Total number of elements

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

            var datos = GetPagoElectronico(Numero, Numerof, ClieFac, sortOrder, page, pageSize, Fechainicio, Fechafinal, FacPag, Estado);

            //ViewData["VPagoFactura"] = datos.ToPagedList(pageNumber, pageSize);


            ViewData["VPagoFacturaEfe"] = GetPagoEfectivo(Numero, Numerof, ClieFac, sortOrder, page, pageSize, Fechainicio, Fechafinal, FacPag, Estado);
            return View(datos.ToPagedList(pageNumber, pageSize));
        }
        //////   FIN INDEX




        private static List<VPagoFactura> GetPagoElectronico(string Numero, string Numerof, string ClieFac, string sortOrder, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string Estado)

        {
            string ConsultaSql = "SELECT * FROM [dbo].[VPagoFactura]";
            string Filtro = string.Empty;
            string Orden = " order by  IDPagoFactura desc ";


            //Buscar por numero
            if (!String.IsNullOrEmpty(Numero))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = " where IDPagoFactura>=" + Numero + "";
                }
                else
                {
                    Filtro += " and  IDPagoFactura>=" + Numero + "";
                }
            }

            if (!String.IsNullOrEmpty(Numerof))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = " where IDPagoFactura<=" + Numerof + "";
                }
                else
                {
                    Filtro += " and  IDPagoFactura<=" + Numerof + "";
                }
            }

            ///tabla filtro: Nombre Cliente
            if (!String.IsNullOrEmpty(ClieFac))
            {

                if (Filtro == string.Empty)
                {
                    Filtro = "where Nombre='" + ClieFac + "'";
                }
                else
                {
                    Filtro += "and  Nombre='" + ClieFac + "'";
                }

            }

            ///tabla filtro: Factura pagada/no pagada && Serie, Nombre Cliente



            if (Estado != "Todos")
            {
                if (Estado == "C")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where StatusPago='C'";
                    }
                    else
                    {
                        Filtro += "and  StatusPago='C'";
                    }
                }
                if (Estado == "A")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where StatusPago='A'";
                    }
                    else
                    {
                        Filtro += "and StatusPago='A'";
                    }
                }
            }



            if (!String.IsNullOrEmpty(Fechainicio) && !String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where  FechaPago BETWEEN '" + Fechainicio + "' and '" + Fechafinal + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and FechaPago BETWEEN '" + Fechainicio + "' and '" + Fechafinal + " 23:59:59.999'";
                }
            }



            //Ordenacion

            switch (sortOrder)
            {

                case "Numero":
                    Orden = " order by   IDPagoFactura asc ";
                    break;
                case "Fecha":
                    Orden = " order by FechaPago ";
                    break;
                case "Nombre_Cliente":
                    Orden = " order by  Nombre ";
                    break;
                default:
                    Orden = " order by  IDPagoFactura desc ";
                    break;
            }

            //var elementos = from s in db.encfacturas
            //select s;
            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;





            VPagoFacturaContext db = new VPagoFacturaContext();
            List<VPagoFactura> pagoFactura = db.Database.SqlQuery<VPagoFactura>(cadenaSQl).ToList();
            return pagoFactura.ToList();
        }
        ////// FIN  PAGO FACTURA  Electrónico//////////////////////////////////////


        ////// INDEX 
        /////////////////////////////////////////////LISTA PAGO FACTURA EFECTIVO//////////////////////////////////////
        public ActionResult IndexEfe(string Numero, string Numerof, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A")
        {
            var FacPagLst = new List<SelectListItem>();
            var EstadoLst = new List<SelectListItem>();
            //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });

            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });

            ViewData["Estado"] = EstadoLst;
            ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

            ViewData["FacPag"] = FacPagLst;
            ViewBag.FacPag = new SelectList(FacPagLst, "Value", "Text");



            //Buscar Cliente
            var ClieLst = new List<string>();
            var ClieQry = from d in new VPagoFacturaEfeContext().VPagoFacturaEfes
                          orderby d.Nombre
                          select d.Nombre;
            ClieLst.AddRange(ClieQry.Distinct());
            ViewBag.ClieFac = new SelectList(ClieLst);

            ViewBag.CurrentSort = sortOrder;

            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "IDPagoFactura" : "";
            ViewBag.FechaSortParm = sortOrder == "FechaPago" ? "FechaPago" : "";
            ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "";


            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;

            
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.PagoFacturas.OrderBy(e => e.Serie).Count();// Total number of elements

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

            var datos = GetPagoEfectivo(Numero, Numerof, ClieFac, sortOrder, page, pageSize, Fechainicio, Fechafinal,FacPag, Estado);

            //ViewData["VPagoFactura"] = datos.ToPagedList(pageNumber, pageSize);


            ViewData["VPagoFacturaEfe"] = GetPagoEfectivo(Numero, Numerof, ClieFac, sortOrder, page, pageSize, Fechainicio, Fechafinal, FacPag, Estado);
            return View(datos.ToPagedList(pageNumber, pageSize));
        }
        //////   FIN INDEX


        private static List<VPagoFacturaEfe> GetPagoEfectivo(string Numero,string  Numerof, string ClieFac, string sortOrder, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string Estado)
        {
            string ConsultaSql = "SELECT * FROM [dbo].[VPagoFacturaEfe]";
            string Filtro = string.Empty;
            string Orden = " order by  IDPagoFactura desc ";


            //Buscar por numero
            if (!String.IsNullOrEmpty(Numero))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where IDPagoFactura>=" + Numero + "";
                }
                else
                {
                    Filtro += "and  IDPagoFactura>=" + Numero + "";
                }
            }

            if (!String.IsNullOrEmpty(Numerof))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where IDPagoFactura<=" + Numerof + "";
                }
                else
                {
                    Filtro += "and  IDPagoFactura<=" + Numerof + "";
                }
            }

            ///tabla filtro: Nombre Cliente
            if (!String.IsNullOrEmpty(ClieFac))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where Nombre='" + ClieFac + "'";
                }
                else
                {
                    Filtro += "and  Nombre='" + ClieFac + "'";
                }

            }
            ///tabla filtro: Factura pagada/no pagada && Serie, Nombre Cliente

            if (Estado != "Todos")
            {
                if (Estado == "C")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where StatusPago='C'";
                    }
                    else
                    {
                        Filtro += "and  StatusPago='C'";
                    }
                }
                if (Estado == "A")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where StatusPago='A'";
                    }
                    else
                    {
                        Filtro += "and StatusPago='A'";
                    }
                }
            }
            if (!String.IsNullOrEmpty(Fechainicio) && !String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where  FechaPago BETWEEN '" + Fechainicio + "' and '" + Fechafinal + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and FechaPago BETWEEN '" + Fechainicio + "' and '" + Fechafinal + " 23:59:59.999'";
                }
            }
            //Ordenacion

            switch (sortOrder)
            {

                case "Numero":
                    Orden = " order by   IDPagoFactura asc ";
                    break;
                case "Fecha":
                    Orden = " order by FechaPago ";
                    break;
                case "Nombre_Cliente":
                    Orden = " order by  Nombre ";
                    break;
                default:
                    Orden = " order by  IDPagoFactura desc ";
                    break;
            }

            //var elementos = from s in db.encfacturas
            //select s;
            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;


            VPagoFacturaEfeContext db1 = new VPagoFacturaEfeContext();
            List<VPagoFacturaEfe> pagoFacturaEfe = db1.Database.SqlQuery<VPagoFacturaEfe>(cadenaSQl).ToList();

            return pagoFacturaEfe.ToList();
        }

        // pago FACTURA EFECTIVO
        ////////////////////////////////////////////////fin PAGO FACTURA EFECTIVO//////////////////////////////////////




        /////////////////////////////////////////////DETALLES PAGO FACTURA//////////////////////////////////////

        public ActionResult Details(int id)
        {
            VPagoFacturaContext db = new VPagoFacturaContext();
            var elemento = db.VPagoFacturas.Single(m => m.IDPagoFactura == id);
            if (elemento == null)
            {
                return NotFound();
            }
            List<PagoFacturaSPEI> pagoFacturaSPEI = db.Database.SqlQuery<PagoFacturaSPEI>("select IDPagoFacturaSPEI, IDPagoFactura, CertificadoPago, IDTipoCadenaPago, SelloPago from [dbo].[PagoFacturaSPEI] where IDPagoFactura='" + id + "'").ToList();
            ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;
            List<DetallesDR> detallesDR = db.Database.SqlQuery<DetallesDR>("select IDFactura, Serie, Numero,D.IDMoneda, M.ClaveMoneda, M.Descripcion, TC, IDMetododepago, ImporteSaldoInsoluto, ImportePagado, NoParcialidad from [dbo].[DocumentoRelacionado] as D inner join c_Moneda as M on D.IDMoneda= M.IDMoneda  where IDPagoFactura='" + id + "'").ToList();
            ViewBag.detallesDR = detallesDR;

            return View(elemento);
        }

        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        /////////////////////////////////////////////DETALLES PAGO FACTURA EFECTIVO//////////////////////////////////////

        public ActionResult DetailsEfe(int id)
        {
            VPagoFacturaEfeContext db = new VPagoFacturaEfeContext();
            var elemento = db.VPagoFacturaEfes.Single(m => m.IDPagoFactura == id);
            if (elemento == null)
            {
                return NotFound();
            }

            List<DetallesDR> detallesDR = db.Database.SqlQuery<DetallesDR>("select IDFactura, Serie, Numero,D.IDMoneda, M.ClaveMoneda, M.Descripcion, TC, IDMetododepago, ImporteSaldoInsoluto, ImportePagado, NoParcialidad from [dbo].[DocumentoRelacionado] as D inner join c_Moneda as M on D.IDMoneda= M.IDMoneda  where IDPagoFactura='" + id + "'").ToList();
            ViewBag.detallesDR = detallesDR;

            return View(elemento);
        }


        public IEnumerable<SelectListItem> GetClientes()
        {
            PagoFactura elemento = new PagoFactura();
            var listaClientes = new ClienteAllRepository().GetClientes();
            ViewBag.datosCliente = listaClientes;

            return listaClientes;
        }

        public IEnumerable<SelectListItem> GetBancoEmpresa()
        {
            PagoFactura elemento = new PagoFactura();
            var listaBancoEmp = new BancoEmpresaRepository().GetBancoEmpresa();
            ViewBag.datosBancoEmp = listaBancoEmp;
            return ViewBag.datosBancoEmp;
        }


        ////////////////////////////////////////////////////////SPEI//////////////////////////////////////

        private PagoFacturaSPEIContext dbSPEI = new PagoFacturaSPEIContext();

        public ActionResult SPEI(int id)
        {
            PagoFacturaSPEI pagoFacturaSPEI = null;
            PagoFacturaSPEI elemento = new PagoFacturaSPEI();
           
                System.Web.HttpContext.Current.Session["IDPagoFactura"] = id;
                try
                {
                    pagoFacturaSPEI = db.Database.SqlQuery<PagoFacturaSPEI>("select IDPagoFacturaSPEI, IDPagoFactura, CertificadoPago, IDTipoCadenaPago, SelloPago from [dbo].[PagoFacturaSPEI] where IDPagoFactura='" + id + "'").ToList()[0];


                }
                catch (Exception err)
                {
                    string error = err.Message;
                    elemento.IDPagoFactura = id;
                }
      

            if (pagoFacturaSPEI != null)
            {
                elemento.CertificadoPago = pagoFacturaSPEI.CertificadoPago;
                elemento.SelloPago = pagoFacturaSPEI.SelloPago;
                elemento.IDTipoCadenaPago = pagoFacturaSPEI.IDTipoCadenaPago;
                elemento.IDPagoFactura = id;
                elemento.IDPagoFacturaSPEI = pagoFacturaSPEI.IDPagoFacturaSPEI;
                // si esiste pasa a modificarlo
            }
            return View(elemento); // si no existe pasa a crearlo


        }
        // POST: PagoFacturaSPEI/Create
        [HttpPost]
        public ActionResult SPEI(PagoFacturaSPEI elemento, int id)
        {

            if (ModelState.IsValid) // si llenaron los datos como los validadores del sistema intento grabar
            {

                if (elemento.IDPagoFacturaSPEI != 0) // si ya venia con un dato de IDPagoFactura
                {

                    db.Database.ExecuteSqlCommand("update [dbo].[PagoFacturaSPEI] set CertificadoPago='" + elemento.CertificadoPago + "', IDTipoCadenaPago='" + elemento.IDTipoCadenaPago + "', SelloPago='" + elemento.SelloPago + "' where IDPagoFactura=" + id);
                }
                else
                {
                    dbSPEI.PagoFacturaSPEIs.Add(elemento);
                    dbSPEI.SaveChanges();

                }



                return RedirectToAction("Index");

            }
            else // si no es valido lo regreso
            {
                return View();
            }


        }


        ////////////////////////////////////////Documento Relacionado////////////////////////////////////
        private DocumentoRelacionadoContext dbDocto = new DocumentoRelacionadoContext();



        public ActionResult DocumentoR(string nombrec, int idc, int idpf, decimal monto)
        {

            ViewBag.nombrec = nombrec;
            ViewBag.idc = idc;
            ViewBag.idpf = idpf;
            ViewBag.monto = monto;

            decimal TC = new PagoFacturaContext().PagoFacturas.Find(idpf).TC;
            ViewBag.TC = TC;
            List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and E.serie<>'N' and E.IDCliente ='" + idc + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, E.IDMetododePago, E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();
            ViewBag.EncFactura = encfactura;

            var resumen = db.Database.SqlQuery<ResumenFac>("select EncFacturas.Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, case WHEN EncFacturas.Moneda<>'MXN' THEN  (SUM(Total * TC)) WHEN EncFacturas.Moneda='MXN' THEN  (SUM(Total )) END  as TotalenPesos from EncFacturas where pagada = 0 and estado = 'A' and  EncFacturas.IDCliente='" + idc + "' group by EncFacturas.Moneda").ToList();
            ViewBag.sumatoria = resumen;



            return View(encfactura);
        }


        public ActionResult FactorajeR(string nombrec, int idc, int idpf, decimal monto)
        {

            ViewBag.nombrec = nombrec;
            ViewBag.idc = idc;
            ViewBag.idpf = idpf;
            ViewBag.monto = monto;

            decimal TC = new PagoFacturaContext().PagoFacturas.Find(idpf).TC;
            ViewBag.TC = TC;
            List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, E.IDMetododePago, E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();
            ViewBag.EncFactura = encfactura;

            var resumen = db.Database.SqlQuery<ResumenFac>("select EncFacturas.Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, case WHEN EncFacturas.Moneda<>'MXN' THEN  (SUM(Total * TC)) WHEN EncFacturas.Moneda='MXN' THEN  (SUM(Total )) END  as TotalenPesos from EncFacturas where pagada = 0 and estado = 'A'  group by EncFacturas.Moneda").ToList();
            ViewBag.sumatoria = resumen;



            return View(encfactura);
        }




        //[HttpPost]
        //public ActionResult Ejecutar(string nombrec, int idpf, int idc, decimal monto, List<VEncFactura> cr)
        //{

        //    PagoFactura pago = new PagoFacturaContext().PagoFacturas.Find(idpf);

        //    string Monedadelpago = new c_MonedaContext().c_Monedas.Find(pago.IDMoneda).ClaveMoneda;
        //    decimal tipocambio = pago.TC;

        //    decimal sumatoria = 0, importesi = 0, ImporteSaldoInsoluto = 0;
        //    int noparcialidad = 0, contador = 0;

        //     string cadenapago = String.Empty;


        //    //Verificar la sumatoria del importe pagado
        //    foreach (var a in cr)
        //    {
        //        string Monedadefactura = new c_MonedaContext().c_Monedas.Find(a.IDMoneda).ClaveMoneda;

        //      //  cadenapago += a.ImportePagado + " " + Monedadefactura + ", ";

        //        if (a.ImportePagado > 0)
        //        {
        //            if (Monedadelpago == Monedadefactura)
        //            {
        //                sumatoria += a.ImportePagado;
        //            }
        //            else
        //            {
        //                if (Monedadelpago != Monedadefactura)
        //                {

        //                    sumatoria += a.ImportePagado * tipocambio;
        //                }
        //            }
        //        }
        //    }

        //    //Verificar que el importe pagado cumpla con las condiciones

        //    if ((Math.Round(sumatoria, 2)  == Math.Round(monto, 2)) || (  Math.Round(sumatoria, 2) / Math.Round(monto, 2)) <1M) // o es igual o menor a 1 porciento 
        //    {
        //        foreach (var i in cr)
        //        {
        //            // Actualiza el importe del saldo insoluto
        //            if (i.ImporteSaldoInsoluto == 0)
        //            {
        //                importesi = i.Total;
        //            }
        //            else
        //            {
        //                importesi = i.ImporteSaldoInsoluto;
        //            }
        //            if (importesi >= i.ImportePagado)
        //            {
        //                contador++;
        //            }
        //        }
        //        if (contador == cr.Count())
        //        {
        //            foreach (var i in cr)
        //            {

        //                /// registra documentoR
        //                if (i.ImportePagado != 0)
        //                {
        //                    //ClsDatoEntero parcialidad = db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(NoParcialidad), 0) AS INT) as Dato from dbo.DocumentoRelacionado where IDFactura='" + i.ID + "'").ToList()[0];
        //                    ClsDatoEntero parcialidad = db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(NoParcialidad), 0) AS INT) as Dato from dbo.DocumentoRelacionado where IDFactura='" + i.ID + "' and StatusDocto='A'").ToList()[0];
        //                    if (parcialidad.Dato != 0)
        //                    {


        //                        noparcialidad = parcialidad.Dato + 1;
        //                    }
        //                    else
        //                    {
        //                        noparcialidad = 1;
        //                    }
        //                    if (i.ImporteSaldoInsoluto == 0)
        //                    {
        //                        ImporteSaldoInsoluto = i.Total - i.ImportePagado;
        //                    }
        //                    else
        //                    {
        //                        ImporteSaldoInsoluto = i.ImporteSaldoInsoluto - i.ImportePagado;
        //                    }

        //                    db.Database.ExecuteSqlCommand("INSERT INTO DocumentoRelacionado([IDPagoFactura],[IDCliente],[IDFactura],[Serie],[Numero],[IDMoneda],[TC],[IDMetododepago],[ImporteSaldoInsoluto],[ImportePagado],[NoParcialidad]) values('" + idpf + "','" + idc + "','" + i.ID + "','" + i.Serie + "','" + i.Numero + "','" + i.IDMoneda + "','" + i.TC + "','" + i.IDMetododepago + "','" + ImporteSaldoInsoluto + "','" + i.ImportePagado + "','" + noparcialidad + "')");
        //                    db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + i.ID + " and Serie='" + i.Serie + "' and Numero= " + i.Numero + " ");

        //                    ClsDatoEntero numero = db.Database.SqlQuery<ClsDatoEntero>("select count(IDFactura) as Dato from SaldoFactura where IDFactura='" + i.ID + "'").ToList()[0];
        //                    int num = numero.Dato;
        //                    if (num != 0)
        //                    {


        //                        db.Database.ExecuteSqlCommand("update SaldoFactura set ImporteSaldoAnterior='" + i.ImporteSaldoInsoluto + "',ImportePagado=Importepagado+" + i.ImportePagado + ",ImporteSaldoInsoluto='" + ImporteSaldoInsoluto + "' where IDFactura='" + i.ID + "' and Serie ='" + i.Serie + "' and Numero= " + i.Numero + "");

        //                    }
        //                    else
        //                    {
        //                        db.Database.ExecuteSqlCommand("INSERT INTO SaldoFactura([IDFactura], [Serie], [Numero],[Total],[ImporteSaldoAnterior],[ImportePagado],[ImporteSaldoInsoluto]) values (" + i.ID + ",'" + i.Serie + "', " + i.Numero + ", " + i.Total + "," + i.Total + "," + i.ImportePagado + ", " + ImporteSaldoInsoluto + ")");
        //                    }
        //                }
        //                if (i.ImporteSaldoInsoluto == 0)
        //                {
        //                    importesi = i.Total;
        //                }
        //                else
        //                {
        //                    importesi = i.ImporteSaldoInsoluto;
        //                }

        //                if (i.ImportePagado == importesi)
        //                {
        //                    db.Database.ExecuteSqlCommand("update EncFacturas set Pagada= 1 where ID='" + i.ID + "'");
        //                    db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + i.ID + " and Serie='" + i.Serie + "' and Numero= " + i.Numero + " ");
        //                }


        //            }
        //            //if ((Math.Round(sumatoria, 2) == Math.Round(monto, 2)) || (Math.Round(monto, 2) - Math.Round(sumatoria, 2)) < 1M)
        //            //{
        //                db.Database.ExecuteSqlCommand("update PagoFactura set Estado= 1 where IDPagoFactura='" + idpf + "'");
        //                db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + idpf + "");

        //            //}
        //        }

        //        else
        //        {
        //            ViewBag.nombrec = nombrec;
        //            ViewBag.idc = idc;
        //            ViewBag.idpf = idpf;
        //            ViewBag.monto = monto;
        //            List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, D.MetodoPago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();

        //            ViewBag.EncFactura = encfactura;
        //            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncFacturas.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncFacturas where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by EncFacturas.IDMoneda").ToList();
        //            ViewBag.sumatoria = resumen;
        //            ViewBag.mensaje = "La cantidad del importe pagado excede el Importe del Saldo Insoluto, verifique por favor";
        //            return View("DocumentoR", encfactura);
        //        }
        //    }
        //    else
        //    {

        //        ViewBag.TC = tipocambio;
        //        ViewBag.nombrec = nombrec;
        //        ViewBag.idc = idc;
        //        ViewBag.idpf = idpf;
        //        ViewBag.monto = monto;
        //        List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, E.IDMetododePago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();

        //        ViewBag.EncFactura = encfactura;
        //        var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncFacturas.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncFacturas where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by EncFacturas.IDMoneda").ToList();
        //        ViewBag.sumatoria = resumen;
        //        ViewBag.mensaje = " La suma de las facturas deber ser menor en un rango de peso o igual al pago " + Math.Round(monto, 2) + " " +Monedadelpago+ " != " + Math.Round(sumatoria, 2) + " " + cadenapago ;
        //        return View("DocumentoR", encfactura);
        //    }



        //    return RedirectToAction("Index");
        //}


        [HttpPost]
        public ActionResult Ejecutar(string nombrec, int idpf, int idc, decimal monto, List<VEncFactura> cr)
        {

            PagoFactura pago = new PagoFacturaContext().PagoFacturas.Find(idpf);

            string Monedadelpago = new c_MonedaContext().c_Monedas.Find(pago.IDMoneda).ClaveMoneda;
            decimal tipocambio = pago.TC;

            decimal sumatoria = 0, importesi = 0, ImporteSaldoInsoluto = 0;
            int noparcialidad = 0, contador = 0;

            string cadenapago = String.Empty;


            //Verificar la sumatoria del importe pagado
            foreach (var a in cr)
            {
                string Monedadefactura = new c_MonedaContext().c_Monedas.Find(a.IDMoneda).ClaveMoneda;

                //  cadenapago += a.ImportePagado + " " + Monedadefactura + ", ";

                if (a.ImportePagado > 0)
                {
                    if (Monedadelpago == Monedadefactura)
                    {
                        sumatoria += a.ImportePagado;
                    }
                    else
                    {
                        if (Monedadelpago == "MXN" && Monedadefactura == "USD")
                        {
                            sumatoria += a.ImportePagado * tipocambio;
                        }
                        if (Monedadelpago == "USD" && Monedadefactura == "MXN")
                        {
                            sumatoria += (a.ImportePagado / tipocambio);
                        }
                    }
                }
            }

            //Verificar que el importe pagado cumpla con las condiciones
            decimal SumatoriaRedondeada = Math.Round(sumatoria, 2);
            if (Math.Round(monto, 2) == SumatoriaRedondeada)
            {
                foreach (var i in cr)
                {
                    // Actualiza el importe del saldo insoluto
                    if (i.ImporteSaldoInsoluto == 0)
                    {
                        importesi = i.Total;
                    }
                    else
                    {
                        importesi = i.ImporteSaldoInsoluto;
                    }
                    if (importesi >= i.ImportePagado)
                    {
                        contador++;
                    }
                }
                if (contador == cr.Count())
                {
                    foreach (var i in cr)
                    {

                        /// registra documentoR
                        if (i.ImportePagado != 0)
                        {
                            ClsDatoEntero parcialidad = db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(NoParcialidad), 0) AS INT) as Dato from dbo.DocumentoRelacionado where IDFactura='" + i.ID + "' and StatusDocto='A'").ToList().FirstOrDefault();
                            if (parcialidad.Dato != 0)
                            {


                                noparcialidad = parcialidad.Dato + 1;
                            }
                            else
                            {
                                noparcialidad = 1;
                            }
                            if (i.ImporteSaldoInsoluto == 0)
                            {
                                ImporteSaldoInsoluto = i.Total - i.ImportePagado;
                            }
                            else
                            {
                                ImporteSaldoInsoluto = i.ImporteSaldoInsoluto - i.ImportePagado;
                            }

                            db.Database.ExecuteSqlCommand("INSERT INTO DocumentoRelacionado([IDPagoFactura],[IDCliente],[IDFactura],[Serie],[Numero],[IDMoneda],[TC],[IDMetododepago],[ImporteSaldoInsoluto],[ImportePagado],[NoParcialidad]) values('" + idpf + "','" + idc + "','" + i.ID + "','" + i.Serie + "','" + i.Numero + "','" + i.IDMoneda + "','" + i.TC + "','" + i.IDMetododepago + "','" + ImporteSaldoInsoluto + "','" + i.ImportePagado + "','" + noparcialidad + "')");
                            db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + i.ID + " and Serie='" + i.Serie + "' and Numero= " + i.Numero + " ");

                            ClsDatoEntero numero = db.Database.SqlQuery<ClsDatoEntero>("select count(IDFactura) as Dato from SaldoFactura where IDFactura='" + i.ID + "'").ToList()[0];
                            int num = numero.Dato;
                            if (num != 0)
                            {


                                db.Database.ExecuteSqlCommand("update SaldoFactura set ImporteSaldoAnterior='" + i.ImporteSaldoInsoluto + "',ImportePagado=Importepagado+" + i.ImportePagado + ",ImporteSaldoInsoluto='" + ImporteSaldoInsoluto + "' where IDFactura='" + i.ID + "' and Serie ='" + i.Serie + "' and Numero= " + i.Numero + "");

                            }
                            else
                            {
                                db.Database.ExecuteSqlCommand("INSERT INTO SaldoFactura([IDFactura], [Serie], [Numero],[Total],[ImporteSaldoAnterior],[ImportePagado],[ImporteSaldoInsoluto]) values (" + i.ID + ",'" + i.Serie + "', " + i.Numero + ", " + i.Total + "," + i.Total + "," + i.ImportePagado + ", " + ImporteSaldoInsoluto + ")");
                            }
                        }
                        if (i.ImporteSaldoInsoluto == 0)
                        {
                            importesi = i.Total;
                        }
                        else
                        {
                            importesi = i.ImporteSaldoInsoluto;
                        }

                        if (i.ImportePagado == importesi)
                        {
                            db.Database.ExecuteSqlCommand("update EncFacturas set Pagada= 1 where ID='" + i.ID + "'");
                            db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + i.ID + " and Serie='" + i.Serie + "' and Numero= " + i.Numero + " ");
                        }


                    }
                    if (Math.Round(sumatoria, 2) == monto)
                    {
                        db.Database.ExecuteSqlCommand("update PagoFactura set Estado= 1 where IDPagoFactura='" + idpf + "'");
                        db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + idpf + "");

                    }
                }

                else
                {
                    ViewBag.nombrec = nombrec;
                    ViewBag.idc = idc;
                    ViewBag.idpf = idpf;
                    ViewBag.monto = monto;
                    List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, D.MetodoPago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();

                    ViewBag.EncFactura = encfactura;
                    var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncFacturas.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncFacturas where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by EncFacturas.IDMoneda").ToList();
                    ViewBag.sumatoria = resumen;
                    ViewBag.mensaje = "La cantidad del importe pagado excede el Importe del Saldo Insoluto, verifique por favor";
                    return View("DocumentoR", encfactura);
                }
            }
            else
            {

                ViewBag.TC = tipocambio;
                ViewBag.nombrec = nombrec;
                ViewBag.idc = idc;
                ViewBag.idpf = idpf;
                ViewBag.monto = monto;
                List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, E.IDMetododePago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();

                ViewBag.EncFactura = encfactura;
                var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncFacturas.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncFacturas where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by EncFacturas.IDMoneda").ToList();
                ViewBag.sumatoria = resumen;
                ViewBag.mensaje = " El importe de los pagos No corresponde al Monto Total a Pagar, verifique por favor" + Math.Round(monto, 2) + " " + Monedadelpago + " != " + Math.Round(sumatoria, 2) + " " + cadenapago;
                return View("DocumentoR", encfactura);
            }



            return RedirectToAction("Index");
        }


        public ActionResult DocumentoRE(string nombrec, int idc, int idpf, decimal monto)
        {

            ViewBag.nombrec = nombrec;
            ViewBag.idc = idc;
            ViewBag.idpf = idpf;
            ViewBag.monto = monto;

            decimal TC = new PagoFacturaContext().PagoFacturas.Find(idpf).TC;
            ViewBag.TC = TC;
            List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, E.IDMetododePago, E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();
            ViewBag.EncFactura = encfactura;

            var resumen = db.Database.SqlQuery<ResumenFac>("select EncFacturas.Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, case WHEN EncFacturas.Moneda<>'MXN' THEN  (SUM(Total * TC)) WHEN EncFacturas.Moneda='MXN' THEN  (SUM(Total )) END  as TotalenPesos from EncFacturas where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by EncFacturas.Moneda").ToList();
            ViewBag.sumatoria = resumen;



            return View(encfactura);
        }

        [HttpPost]
        public ActionResult EjecutarRE(string nombrec, int idpf, int idc, decimal monto, List<VEncFactura> cr)
        {

            PagoFactura pago = new PagoFacturaContext().PagoFacturas.Find(idpf);

            string Monedadelpago = new c_MonedaContext().c_Monedas.Find(pago.IDMoneda).ClaveMoneda;
            decimal tipocambio = pago.TC;

            decimal sumatoria = 0, importesi = 0, ImporteSaldoInsoluto = 0;
            int noparcialidad = 0, contador = 0;




            //Verificar la sumatoria del importe pagado
            foreach (var a in cr)
            {
                string Monedadefactura = new c_MonedaContext().c_Monedas.Find(a.IDMoneda).ClaveMoneda;


                if (Monedadelpago == Monedadefactura)
                {
                    sumatoria += a.ImportePagado;
                }
                else
                {
                    //if (Monedadelpago == "MXN" && Monedadefactura == "USD")
                    //{
                        sumatoria += a.ImportePagado * tipocambio;
                    //}
                    //if (Monedadelpago == "USD" && a.Moneda == "MXN")
                    //{
                    //    sumatoria += a.ImportePagado / tipocambio;
                    //}
                }
            }

            //Verificar que el importe pagado cumpla con las condiciones

            if ((Math.Round(sumatoria, 2) == Math.Round(monto, 2)) || (Math.Round(sumatoria, 2) / Math.Round(monto, 2)) < 1M) // o es igual o menor a 1 porciento 
            {
                foreach (var i in cr)
                {
                    // Actualiza el importe del saldo insoluto
                    if (i.ImporteSaldoInsoluto == 0)
                    {
                        importesi = i.Total;
                    }
                    else
                    {
                        importesi = i.ImporteSaldoInsoluto;
                    }
                    if (importesi >= i.ImportePagado)
                    {
                        contador++;
                    }
                }
                if (contador == cr.Count())
                {
                    foreach (var i in cr)
                    {

                        /// registra documentoR
                        if (i.ImportePagado != 0)
                        {
                            //ClsDatoEntero parcialidad = db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(NoParcialidad), 0) AS INT) as Dato from dbo.DocumentoRelacionado where IDFactura='" + i.ID + "'").ToList()[0];
                            ClsDatoEntero parcialidad = db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(NoParcialidad), 0) AS INT) as Dato from dbo.DocumentoRelacionado where IDFactura='" + i.ID + "' and StatusDocto='A'").ToList()[0];
                            if (parcialidad.Dato != 0)
                            {


                                noparcialidad = parcialidad.Dato + 1;
                            }
                            else
                            {
                                noparcialidad = 1;
                            }
                            if (i.ImporteSaldoInsoluto == 0)
                            {
                                ImporteSaldoInsoluto = i.Total - i.ImportePagado;
                            }
                            else
                            {
                                ImporteSaldoInsoluto = i.ImporteSaldoInsoluto - i.ImportePagado;
                            }
                            db.Database.ExecuteSqlCommand("INSERT INTO DocumentoRelacionado([IDPagoFactura],[IDCliente],[IDFactura],[Serie],[Numero],[IDMoneda],[TC],[IDMetododepago],[ImporteSaldoInsoluto],[ImportePagado],[NoParcialidad], SaldoAnterior) values('" + idpf + "','" + idc + "','" + i.ID + "','" + i.Serie + "','" + i.Numero + "','" + i.IDMoneda + "','" + i.TC + "','" + i.IDMetododepago + "','" + ImporteSaldoInsoluto + "','" + i.ImportePagado + "','" + noparcialidad + "', (" + ImporteSaldoInsoluto + " + " + i.ImportePagado + "))");
                           // db.Database.ExecuteSqlCommand("INSERT INTO DocumentoRelacionado([IDPagoFactura],[IDCliente],[IDFactura],[Serie],[Numero],[IDMoneda],[TC],[IDMetododepago],[ImporteSaldoInsoluto],[ImportePagado],[NoParcialidad]) values('" + idpf + "','" + idc + "','" + i.ID + "','" + i.Serie + "','" + i.Numero + "','" + i.IDMoneda + "','" + i.TC + "','" + i.IDMetododepago + "','" + ImporteSaldoInsoluto + "','" + i.ImportePagado + "','" + noparcialidad + "')");
                            db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + i.ID + " and Serie='" + i.Serie + "' and Numero= " + i.Numero + " ");

                            ClsDatoEntero numero = db.Database.SqlQuery<ClsDatoEntero>("select count(IDFactura) as Dato from SaldoFactura where IDFactura='" + i.ID + "'").ToList()[0];
                            int num = numero.Dato;
                            if (num != 0)
                            {


                                db.Database.ExecuteSqlCommand("update SaldoFactura set ImporteSaldoAnterior='" + i.ImporteSaldoInsoluto + "',ImportePagado='" + i.ImportePagado + "',ImporteSaldoInsoluto='" + ImporteSaldoInsoluto + "' where IDFactura='" + i.ID + "' and Serie ='" + i.Serie + "' and Numero= " + i.Numero + "");

                            }
                            else
                            {
                                db.Database.ExecuteSqlCommand("INSERT INTO SaldoFactura([IDFactura], [Serie], [Numero],[Total],[ImporteSaldoAnterior],[ImportePagado],[ImporteSaldoInsoluto]) values (" + i.ID + ",'" + i.Serie + "', " + i.Numero + ", " + i.Total + "," + i.Total + "," + i.ImportePagado + ", " + ImporteSaldoInsoluto + ")");
                            }
                        }
                        if (i.ImporteSaldoInsoluto == 0)
                        {
                            importesi = i.Total;
                        }
                        else
                        {
                            importesi = i.ImporteSaldoInsoluto;
                        }

                        if (i.ImportePagado == importesi)
                        {
                            db.Database.ExecuteSqlCommand("update EncFacturas set Pagada= 1 where ID='" + i.ID + "'");
                            db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + i.ID + " and Serie='" + i.Serie + "' and Numero= " + i.Numero + " ");
                        }


                    }
                    if ((Math.Round(sumatoria, 2) == Math.Round(monto, 2)) || (Math.Round(sumatoria, 2) / Math.Round(monto, 2)) < 1M) // o es igual o menor a 1 porciento 
                        {
                        db.Database.ExecuteSqlCommand("update PagoFactura set Estado= 1 where IDPagoFactura='" + idpf + "'");
                        db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + idpf + "");

                    }
                }

                else
                {
                    ViewBag.nombrec = nombrec;
                    ViewBag.idc = idc;
                    ViewBag.idpf = idpf;
                    ViewBag.monto = monto;
                    List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, D.MetodoPago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();

                    ViewBag.EncFactura = encfactura;
                    var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncFacturas.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncFacturas where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by EncFacturas.IDMoneda").ToList();
                    ViewBag.sumatoria = resumen;
                    ViewBag.mensaje = "La cantidad del importe pagado excede el Importe del Saldo Insoluto, verifique por favor";
                    return View("DocumentoRE", encfactura);
                }
            }
            else
            {


                ViewBag.nombrec = nombrec;
                ViewBag.idc = idc;
                ViewBag.idpf = idpf;
                ViewBag.monto = monto;
                List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, E.IDMetododePago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();

                ViewBag.EncFactura = encfactura;
                var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncFacturas.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncFacturas where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrec + "' group by EncFacturas.IDMoneda").ToList();
                ViewBag.sumatoria = resumen;
                ViewBag.mensaje = " El importe de los pagos No corresponde al Monto Total a Pagar, verifique por favor  " ;
                return View("DocumentoRE", encfactura);
            }



            return RedirectToAction("IndexEfe");
        }



        public ActionResult Timbrarpago(int id)
        {
            VPagoFacturaContext db = new VPagoFacturaContext();
            EncfacturaContext dbf = new EncfacturaContext();
            EmpresaContext dbe = new EmpresaContext();
            List<VPagoFacturaC> pagoFactura = db.Database.SqlQuery<VPagoFacturaC>("SELECT * FROM [dbo].[VPagoFacturaC]  where idpagofactura=" + id + " order by FechaPago desc").ToList();
            VPagoFacturaC pago = pagoFactura[0];
            ClsFactura factura = new ClsFactura();


            Folio dbfolio = new Models.Cfdi.Folio();


            Folio folioW = dbf.Database.SqlQuery<Folio>("Select * from folio where serie='P'").ToList().FirstOrDefault();
            factura._serie = "P";
            factura._folio = (folioW.Numero + 1).ToString();

            factura.Tipodecombrobante = "P";
            factura.tipodecambio = pago.TC.ToString();



            factura.valoriva = decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA.ToString());




            Empresa emisor = dbe.empresas.Find(2);
            factura.Emisora = emisor;
            factura.Regimen = emisor.c_RegimenFiscal.ClaveRegimenFiscal.ToString();


            Empresa Receptor = new Empresa();
            Receptor.RFC = pago.RFC;
            Receptor.RazonSocial = pago.Nombre;
            Receptor.Calle = pago.Nombre;
            Receptor.NoExt = "";
            Receptor.NoInt = "";
            Receptor.CP = "";
            factura.Receptora = Receptor;


            List<Concepto> conceptos = new List<Concepto>();
            Concepto concepto1 = new Concepto();
            concepto1.ClaveProdServ = "84111506";
            concepto1.NoIdentificacion = "1";
            concepto1.Cantidad = 1;
            concepto1.ClaveUnidad = "ACT";
            concepto1.Unidad = "ACTIVIDAD";
            concepto1.Descripcion = "Pago";
            concepto1.ValorUnitario = 0;
            concepto1.Importe = 0;
            concepto1.llevaiva = true;
            concepto1.Descuento = 0;


            conceptos.Add(concepto1);


            factura.Listaconceptos.conceptos = conceptos;

            //  factura.timbrarpueba = true;
            // public Cfdirelacionados cfdirelacionados = null;
            // public Certificados certificado;
            Encpagofactura encabezado1 = new Encpagofactura();
            encabezado1.FechaPago = pago.FechaPago.ToString("s");
            encabezado1.Formadepago = pago.ClaveFormaPago;
            encabezado1.Moneda = pago.ClaveDivisa;
            encabezado1.Monto = pago.Monto;
            encabezado1.TipoCambioP = factura.tipodecambio;

            if (!string.IsNullOrEmpty(pago.RFCBancoEmisor))
            {
                encabezado1.RfcEmisorCtaOrd = pago.RFCBancoEmisor;
                if (encabezado1.RfcEmisorCtaOrd== "XEXX010101000")
                {
                    encabezado1.NomBancoOrdExt = pago.RazonSocialEmisor;
                }
            }
            try
            {
                if (!string.IsNullOrEmpty(pago.CuentaEmisor.ToString()))
                {
                    encabezado1.CtaOrdenante = pago.CuentaEmisor.ToString();
                }
            }
            catch (Exception err)
            {
                String Mensaje = err.Message;
            }
            try
            {
                if (!string.IsNullOrEmpty(pago.RFCBancoReceptor))
                {
                    encabezado1.RfcEmisorCtaBen = pago.RFCBancoReceptor;
                }
            }
            catch (Exception err)
            {
                String Mensaje = err.Message;
            }
            try
            {
                if (!string.IsNullOrEmpty(pago.CuentaReceptor))
                {
                    encabezado1.CtaBeneficiario = pago.CuentaReceptor.ToString();
                }
            }
            catch (Exception err)
            {
                String Mensaje = err.Message;
            }

            if (pago.NoOperacion > 0)
            {
                encabezado1.NumOperacion = pago.NoOperacion.ToString();
            }

            List<VDocumentoR> VDocumentoRs = db.Database.SqlQuery<VDocumentoR>("SELECT * FROM [dbo].[VDocumentoR]  where idpagofactura=" + id + " ").ToList();
            

            foreach (VDocumentoR documentoR in VDocumentoRs)
            {


                pagofactura pago1 = new pagofactura();
                pago1.IdDocumento = documentoR.UUID.ToUpper();
                pago1.Serie = documentoR.Serie;
                pago1.Folio = documentoR.Numero;
                pago1.MonedaDR = documentoR.Moneda;
                if (pago1.MonedaDR != pago.ClaveDivisa)
                {
                    pago1.TipoCambioDR = Math.Round(pago.TC, 6);
                    if (pago.ClaveDivisa == "MXN" && pago1.MonedaDR == "USD")

                    {
                        //ESPERA
                        decimal montodolares = Math.Round(pago.Monto / pago.TC,2);
                        decimal Regla3 = (documentoR.ImportePagado * pago.Monto) / montodolares;
                        decimal tc = Math.Round(documentoR.ImportePagado / Regla3,6);
                        pago1.TipoCambioDR = tc + 0.00001M;

                    }
                }
                if (pago1.MonedaDR == pago.ClaveDivisa)
                {
                    pago1.TipoCambioDR = 1;
                }

                pago1.MetodoDePagoDR = documentoR.IDMetododepago;
                pago1.NumParcialidad = documentoR.NoParcialidad;
                //documentoR.ImporteSaldoAnterior - documentoR.ImportePagado;
                decimal saldoanterior = documentoR.Total;
                if (documentoR.ImporteSaldoAnterior == 0)
                {
                    saldoanterior = documentoR.Total;
                } else
                { saldoanterior = documentoR.ImporteSaldoAnterior; }

                pago1.ImpSaldoAnt = saldoanterior;
                pago1.ImpSaldoInsoluto = saldoanterior - documentoR.ImportePagado;
                pago1.ImpPagado = documentoR.ImportePagado;
                encabezado1.pago.Add(pago1);



            }


            factura.Encabezadosfacturas.encabezados.Add(encabezado1);

            MFSDK X = factura.construirfacturadepagos();
            //  facturaejemplo.EscribeEnArchivo(strini, HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"), true);
            //   string ini = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini")));
            SDKRespuesta respuesta = factura.timbrar(X);

            bool pasa = false;
            try
            {
                if (respuesta.Codigo_MF_Texto.Contains("OK"))
                {
                    pasa = true;
                    try
                    {
                        PagoFactura facturae = new PagoFacturaContext().PagoFacturas.Where(s => s.UUID == respuesta.UUID).ToList().FirstOrDefault();
                        if (facturae == null)
                        {
                            throw new Exception("Posbiblemente no tiene factura");
                        }
                        else
                        {
                            pasa = false;
                        }
                    }
                    catch (Exception err)
                    {
                        pasa = true;
                    }
                }

                if (!pasa)
                {
                    return RedirectToAction("Index", "Prefactura"); ;
                }
            }
            catch (Exception err)
            {

            }

            if (respuesta.Codigo_MF_Texto.Contains("OK"))
            {
                db.Database.ExecuteSqlCommand("update PagoFactura set rutaxml='" + respuesta.CFDI + "', UUID='" + respuesta.UUID + "', SERIE='P',FOLIO=" + factura._folio + " where IdPagoFactura=" + id);
                db.Database.ExecuteSqlCommand("update folio set Numero=" + factura._folio + " where serie='P'");
                return RedirectToAction("Index");
            }

            //if (respuesta.Codigo_MF_Texto.Contains("ERROR"))
            //{
            var reshtml = Server.HtmlEncode(respuesta.Codigo_MF_Texto + "->" + respuesta.CFDI);
            return Content(reshtml);
            //  }



        }

        public FileResult Descargarxml(int id)
        {
            // Obtener contenido del archivo
            PagoFactura elemento = db.PagoFacturas.Find(id);

            var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaXML));

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(elemento.RutaXML);



            XmlNodeList comprobante = doc.GetElementsByTagName("cfdi:Comprobante");
            string folio = "";

            if (((XmlElement)comprobante[0]).GetAttribute("Folio") != null)
            { folio = ((XmlElement)comprobante[0]).GetAttribute("Folio"); }

            ClsXmlPagos pa = new ClsXmlPagos(elemento.RutaXML);

            return File(stream, "text/plain", "FacturaPagoP" + folio + ".xml");
        }

        public ActionResult DescargarPDF(int id)
        {
            // Obtener contenido del archivo
            PagoFactura elemento = db.PagoFacturas.Find(id);


            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);


            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            PDFPago pago = new PDFPago(elemento.RutaXML, logoempresa, id, "(55) 262 04200");

            //RedirectToAction("Index");
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            return new FilePathResult(pago.nombreDocumento, contentType);



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


        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        /////////////////////////////////////////////CANCELA PAGO FACTURA//////////////////////////////////////
        public ActionResult CancelaPago(int id, string Viene)
        {

            //RecepcionContext db1 = new RecepcionContext();

            RegistroCancelacionFacturas elemento = new RegistroCancelacionFacturas();

            elemento.IDFactura = id;
            elemento.FViene = "PagoFactura";


            ViewBag.Motivo = new SelectList(new MotivoCancelacionContext().MotivoCancelacions, "IDCancelacion", "DescripcionCan");

            CancelaPago pag = new CancelaPago();


            List<VPagoFactura> detallesP = db.Database.SqlQuery<VPagoFactura>("select IDPagoFactura, FechaPago,Nombre, Monto, ClaveDivisa from VPagoFactura where IDPagoFactura ='" + id + "'").ToList();
            ViewBag.detallesP = detallesP;

            
            return View(elemento);

        }


        [HttpPost]
        public ActionResult CancelaPago(FormCollection datos)
        {

            //RecepcionContext db1 = new RecepcionContext();
            RegistroCancelacionFacturas motivo = new RegistroCancelacionFacturas();
            motivo.IDFactura = int.Parse(datos.Get("IDPago"));
            motivo.FViene = datos.Get("FViene");
            motivo.Motivo = int.Parse(datos.Get("Motivo"));

            try
            {
                motivo.FolioFiscal = datos.Get("FolioFiscal");
            }
            catch (Exception err)
            {

            }
            var elemento = db.PagoFacturas.Find(motivo.IDFactura);


           


            string xmlString = ""; 
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();


            try
            {
                bool salida = false;
                try
                {
                    if (elemento.UUID != "" && elemento.UUID != null)
                    {

                        MotivosCancelacion motivos = new MotivoCancelacionContext().MotivoCancelacions.Find(motivo.Motivo);
                        string ClaveMotivo = motivos.ClaveCan;

                        if (ClaveMotivo == "01")
                        {
                            if (motivo.FolioFiscal == null)
                            {
                                return Content("Falta agregar Folio Fiscal ");
                            }
                        }
                        else
                        {
                            motivo.FolioFiscal = "";
                        }
                        salida = new ClsFactura().cancela40(elemento.UUID, motivo.FolioFiscal, ClaveMotivo, motivo.IDFactura, UserID,"PagoF");

                    }

                }
                catch (Exception ERR)
                {

                }

                


                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public JsonResult cancelarFacturaPago(int id)
        {
            var elemento = db.PagoFacturas.Find(id);

            try
            {

                List<VPagoFactura> detallesP = db.Database.SqlQuery<VPagoFactura>("select IDPagoFactura, FechaPago,Nombre, Monto, ClaveDivisa from VPagoFactura where IDPagoFactura ='" + id + "'").ToList();
                ViewBag.detallesP = detallesP;

                //Actualiza la tabla PagoFactura
                PagoFactura datoC = db.PagoFacturas.Single(m => m.IDPagoFactura == id);
                db.Database.ExecuteSqlCommand("update dbo.PagoFactura set StatusPago= 'C', FechaCancelacion= SYSDATETIME ( ), ObsCancela= '" + datoC.ObsCancela + "' where IDPagoFactura = " + id + " ");

                //Actualiza la tabla DocumentoRelacionado
                //DocumentoRelacionadoContext db1 = new DocumentoRelacionadoContext();
                //List<DocumentoRelacionado> doctoR = db1.Database.SqlQuery<DocumentoRelacionado>("SELECT * FROM [dbo].[DocumentoRelacionado] where IDPagoFactura=" + id + " ").ToList();
                db.Database.ExecuteSqlCommand("update dbo.DocumentoRelacionado set StatusDocto= 'C' where IDPagoFactura= " + id + " ");

                //Actualiza la tabla PagoFacturaSPEI
                //PagoFacturaSPEIContext db2 = new PagoFacturaSPEIContext();
                db.Database.ExecuteSqlCommand("update dbo.PagoFacturaSPEI set StatusPago= 'C' where IDPagoFactura= " + id + " ");

                //Actualiza la tabla SaldoFactura con el importepagado en documento relacionado
                SaldoFacturaContext db1 = new SaldoFacturaContext();
                string consultaSQL = "select * from dbo.Saldos(" + id + ") ";
                List<Saldos> doctoS = db1.Database.SqlQuery<Saldos>(consultaSQL).ToList();
                foreach (var m in doctoS)
                {
                    int IDF = m.IDFactura;
                    db.Database.ExecuteSqlCommand("update dbo.SaldoFactura set ImportePagado = (ImportePagado - " + m.ImportePagado + ") where IDFactura = " + IDF + " ");
                    db.Database.ExecuteSqlCommand("update dbo.SaldoFactura set ImporteSaldoInsoluto = (Total - ImportePagado), ImporteSaldoAnterior = (ImporteSaldoAnterior + ImportePagado) where IDFactura = " + IDF + " ");

                    //Actualiza la tabla EncFacturas, con los valores actuales de los saldos

                    db.Database.ExecuteSqlCommand("update [dbo].[EncFacturas] set pagada=0 where ID=" + IDF + " ");



                }

                return Json(new HttpStatusCodeResult(200));



            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }

        }

        /////////////////////////////////////////////elimina solo el PAGO FACTURA en index//////////////////////////////////////

        public ActionResult EliminaPago(int? id)
        {
            EliminaPagoEfe pag = new EliminaPagoEfe();


            List<VPagoFactura> detallesP = db.Database.SqlQuery<VPagoFactura>("select IDPagoFactura, FechaPago,Nombre, Monto, ClaveDivisa from VPagoFactura where IDPagoFactura ='" + id + "'").ToList();
            ViewBag.detallesP = detallesP;

            if (id != null)
            {
                pag.IDPagoFactura = (int)id;
                return View(pag);

            }
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult EliminaPago(EliminaPagoEfe pag)
        {
            int id = pag.IDPagoFactura;

            List<VPagoFactura> detallesP = db.Database.SqlQuery<VPagoFactura>("select IDPagoFactura, FechaPago,Nombre, Monto, ClaveDivisa from VPagoFactura where IDPagoFactura ='" + id + "'").ToList();
            ViewBag.detallesP = detallesP;

            //Actualiza la tabla PagoFactura
            PagoFactura datoC = db.PagoFacturas.Single(m => m.IDPagoFactura == id);
            db.Database.ExecuteSqlCommand("delete from dbo.PagoFactura where IDPagoFactura = " + id + " ");

            return RedirectToAction("Index");
        }



    /////////////////////////////////////////////elimina solo el PAGO FACTURA EFECTIVO en index//////////////////////////////////////

    public ActionResult EliminaPagoEfe(int? id)
    {
        EliminaPagoEfe pag = new EliminaPagoEfe();

        VPagoFacturaEfeProvContext dbe = new VPagoFacturaEfeProvContext();
        List<VPagoFacturaEfe> detallesP = dbe.Database.SqlQuery<VPagoFacturaEfe>("select IDPagoFactura, FechaPago,Nombre, Monto, ClaveDivisa,folio from VPagoFacturaEfe where IDPagoFactura ='" + id + "'").ToList();
        ViewBag.detallesPEfe = detallesP;

        if (id != null)
        {
            pag.IDPagoFactura = (int)id;
            return View(pag);

        }
        if (id == null)
        {
            return NotFound();
        }
        return RedirectToAction("IndexEfe");
    }


    [HttpPost]
    public ActionResult EliminaPagoEfe(EliminaPagoEfe pag)
    {
        int id = pag.IDPagoFactura;
            VPagoFacturaEfeProvContext dbe = new VPagoFacturaEfeProvContext();
            List<VPagoFacturaEfe> detallesPEfe = dbe.Database.SqlQuery<VPagoFacturaEfe>("select IDPagoFactura, FechaPago,Nombre, Monto, ClaveDivisa,folio from VPagoFacturaEfe where IDPagoFactura ='" + id + "'").ToList();
        ViewBag.detallesPEfe= detallesPEfe;

        //Actualiza la tabla PagoFactura
        PagoFactura datoC = db.PagoFacturas.Single(m => m.IDPagoFactura == id);
        db.Database.ExecuteSqlCommand("delete from dbo.PagoFactura where IDPagoFactura = " + id + " ");

        return RedirectToAction("IndexEfe");
    }

        /////////////REPORTES/////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     



        public ActionResult getTC(int? id)
        {

            TipoCambioContext db = new TipoCambioContext();
            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();

            decimal Cambio = 0;
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + id + "," + origen + ") as TC").ToList()[0];
            Cambio = cambio.TC;
            ViewBag.datostc = Cambio;
            //return RedirectToAction("PagoFactura");
            return Json(Cambio, JsonRequestBehavior.AllowGet);
        }


        // GetBanco
        public JsonResult getbanco(int id)
        {
            List<SelectListItem> listbanco = new List<SelectListItem>();

            BancoClienteContext ban = new BancoClienteContext();
            var banco = ban.BancoClientes.Where(x => x.IDCliente == id).ToList();

            if (banco.Count() != 0)
            {
                foreach (var x in banco)
                {

                    listbanco.Add(new SelectListItem { Text = x.c_Banco.Nombre + " | " + x.c_Banco.RFC + " | " + x.CuentaBanco + " | " + x.c_Moneda.ClaveMoneda + " | " + x.c_Moneda.Descripcion, Value = x.IDBancoCliente.ToString() });
                }
            }
            else
            {

                var bancos = ban.c_Bancos.ToList();
                listbanco.Add(new SelectListItem { Text = "--Selecciona un banco--", Value = "0" });
                foreach (var x in bancos)
                {
                    listbanco.Add(new SelectListItem { Text = x.Nombre, Value = x.RFC.ToString() });
                }
            }

            return Json(new SelectList(listbanco, "Value", "Text", JsonRequestBehavior.AllowGet));
        }





        //////////////////////////////////////////////////ENVIAR PDF///////////////////////////////////////////////////

        public ActionResult EnviarPdf(int id)
        {
            VPagoFacturaContext D = new VPagoFacturaContext();
            EmpresaContext EM = new EmpresaContext();
            // Obtener contenido del archivo
            var elemento = D.VPagoFacturas.Find(id);

            Clientes Cliente = null;

            try
            {

                ClsDatoEntero idcliente = db.Database.SqlQuery<ClsDatoEntero>("select IDCliente as Dato from [dbo].[Clientes] where Nombre='" + elemento.Nombre + "'").ToList()[0];

                ClientesContext clientes = new ClientesContext();
                Cliente = clientes.Clientes.Find(idcliente.Dato);
            }

            catch (Exception err)
            {
                string error = err.Message;
                Cliente = null;
            }

            try
            {
                if (string.IsNullOrEmpty(Cliente.CorreoCfdi))
                {
                    var reshtml = Server.HtmlEncode("No hay correo cfdi resistrado o no existe registro del cliente");

                    return Content(reshtml);
                }
            }
            catch (Exception err)
            {

                string error = err.Message;
                var reshtml = Server.HtmlEncode("No hay correo cfdi resistrado o no existe registro del cliente ");

                return Content(reshtml);

            }

            try
            {
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaXML));
                StreamReader reader = new StreamReader(stream);
                String contenidoxml = reader.ReadToEnd();

                //Console.WriteLine("Esto es mi mensaje : entre aqui" );
                // toma la empresa que tenga el id 1 via de mientras 
                var empresa = EM.empresas.Single(m => m.IDEmpresa == 2);

                System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);


                PDFPago pagopdf = new PDFPago(elemento.RutaXML, logoempresa, elemento.folio, empresa.Telefono);



                string correo = string.Empty;
                string password = string.Empty;
                string correofirma = string.Empty;
                string servidor = "smtp.gmail.com";
                int puerto = 587;
                string nombrecorreo = empresa.RazonSocial;
                string asunto = "Factura";
                string titulo = "Gracias por su preferencia  ";
                string cuerpo = "Adjuntamos su pago ";
                string firma = "Si tiene alguna duda o comentario no replique este mail, favor de escribir a";
                string copiaoculta = string.Empty;

                XmlDocument xmail = new XmlDocument();
                xmail.Load(System.Web.HttpContext.Current.Server.MapPath("~/configcorreopago.xml"));


                XmlNode mailnode = xmail.FirstChild;
                try
                {
                    foreach (XmlNode elementomail in mailnode.ChildNodes)
                    {

                        if (elementomail.Name == "correo")
                        {
                            correo = elementomail.InnerText;
                        }
                        if (elementomail.Name == "password")
                        {
                            password = elementomail.InnerText;
                        }
                        if (elementomail.Name == "CorreofirmaFactura")
                        {
                            correofirma = elementomail.InnerText;
                        }
                        if (elementomail.Name == "servidor")
                        {
                            servidor = elementomail.InnerText;
                        }

                        if (elementomail.Name == "puerto")
                        {
                            puerto = Int32.Parse(elementomail.InnerText);
                        }

                        if (elementomail.Name == "nombre")
                        {
                            nombrecorreo = elementomail.InnerText;
                        }

                        if (elementomail.Name == "asunto")
                        {
                            asunto = elementomail.InnerText;
                        }

                        if (elementomail.Name == "titulo")
                        {
                            asunto = elementomail.InnerText;
                        }
                        if (elementomail.Name == "cuerpo")
                        {
                            cuerpo = elementomail.InnerText;
                        }
                        if (elementomail.Name == "copiaoculta")
                        {
                            copiaoculta = elementomail.InnerText;
                        }


                    }
                }
                catch (Exception err)
                {
                    string error = err.Message;
                }


                SmtpClient mySmtpClient = new SmtpClient();

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(correo, password);
                mySmtpClient.Credentials = basicAuthenticationInfo;
                mySmtpClient.Host = servidor;
                mySmtpClient.Port = puerto;
                mySmtpClient.EnableSsl = false;
                mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                // add from,to mailaddresses
                MailAddress from = new MailAddress(correo, nombrecorreo);
                // MailAddress to = new MailAddress(Cliente.CorreoCfdi, elemento.Nombre_cliente);

                // VAMOS AMANDAR A DIFERENTES CORREOS SEPARADOS POR COMAS
                // 
                Cliente.CorreoPagoC.Replace(';', ',');
                string[] correosclientes = Cliente.CorreoPagoC.Split(',');


                if (correosclientes.Length == 0)
                {
                    throw new Exception("No hay Correos registrados en correos para Complemento de pago");
                }

                MailAddress to = new MailAddress(correosclientes[0], elemento.Nombre);
                MailMessage myMail = new MailMessage(from, to);


                if (correosclientes.Length > 1)
                {
                    for (int i = 1; i <= correosclientes.Length - 1; i++)
                    {
                        if (correosclientes[i] == string.Empty)
                        {
                            throw new Exception("Tienes al final una coma sin correo o un correo vacio");
                        }
                        MailAddress copy = new MailAddress(correosclientes[i]);
                        myMail.CC.Add(copy);
                    }
                }





                string archivoxml = System.Web.HttpContext.Current.Server.MapPath("~/Documentostemporales/Complementopago" + elemento.folio + ".xml");

                using (FileStream fs = System.IO.File.Create(archivoxml))
                {
                    AddText(fs, elemento.RutaXML);
                }



                // add ReplyTo
                //MailAddress replyto = new MailAddress("evidenciaclase@gmail.com");
                //myMail.ReplyToList.Add(replyto);
                //A quien responder
                // myMail.ReplyToList.Add("dianarojasortega20@gmail.com");
                //Con copia a 
                myMail.Bcc.Add(copiaoculta);


                // set subject and encoding
                myMail.Subject = asunto;
                myMail.SubjectEncoding = Encoding.UTF8;

                //   myMail.Attachments.Add(New Attachment(directorio & "\XmlFactura" & seriefactura & foliofactura & ".xml")) // aqui corregir para jalar crear el xml y el pdf
                //                 myMail.Attachments.Add(New Attachment(directorio & "\PdfFactura" & seriefactura & foliofactura & ".pdf"))
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append("<html>");
                sb.Append("<head><title>" + titulo + "</title></ head >");

                sb.Append("</head>");
                sb.Append("<body>");
                sb.Append("<h4>" + cuerpo + " " + " del dia " + elemento.FechaPago.ToShortDateString() + "por un monto de  " + elemento.Monto.ToString("C") + " </h4>");
                sb.Append("<h6>" + firma + "</h6 >" + correofirma);

                sb.Append("</body></html>");
                // set body-message and encoding
                myMail.Body = sb.ToString();
                //_memoryStream.Position = 0;
                myMail.Attachments.Add(new Attachment(pagopdf.nombreDocumento));
                myMail.Attachments.Add(new Attachment(archivoxml));


                myMail.BodyEncoding = Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);

                Dispose();

                Dispose();
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;

                var reshtml = Server.HtmlEncode("No pude enviar el mail, posible error de formato en el mail de envio o no tiene un mail registrado \n El error tecnico es  " + mensajedeerror);

                return Content(reshtml);

            }

            return Content("<html><body>tu mail se ha enviado correctamente</body></html>", "text/html");
            //return File(nombrededocumento, "application / pdf", nombrededocumento); 
        }














        ////////////////////////////////////////////////PAGAR FACTURA ELECTRONICA//////////////////////////////////////
        public ActionResult PagarFactura()
        {
            ClientesContext prov = new ClientesContext();
            var cliente = prov.Clientes.OrderBy(m => m.Nombre).ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            foreach (var m in cliente)
            {
                li.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
                ViewBag.cliente = li;
            }


            c_MonedaContext moneda = new c_MonedaContext();
            var monedas = moneda.c_Monedas.ToList();
            List<SelectListItem> lii = new List<SelectListItem>();
            lii.Add(new SelectListItem { Text = "--Selecciona Moneda--", Value = "0" });

            foreach (var m in monedas)
            {
                lii.Add(new SelectListItem { Text = m.ClaveMoneda, Value = m.IDMoneda.ToString() });
                ViewBag.datosMoneda = lii;

            }



            PagoFactura elemento = new PagoFactura();


            TipoCambioContext db = new TipoCambioContext();

            List<c_Moneda> monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int destino = monedadestino.Select(s => s.IDMoneda).FirstOrDefault();


            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='USD'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();



            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            decimal Cambio = 0;
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + origen + "," + destino + ") as TC").ToList()[0];
            Cambio = cambio.TC;
            ViewBag.datostc = Cambio;

            elemento.TC = cambio.TC;


            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;

            //elemento.FechaPago = DateTime.Now;
            ViewBag.datosClientes = new SelectList(GetClientes());

            var listaFormaPago = new FormaPagoRepository().GetFormasdepagoElectronica();
            ViewBag.datosFormaPago = listaFormaPago;

            //var listaMoneda = new MonedaRepository().GetMoneda();
            //ViewBag.datosMoneda = listaMoneda;


            var listaBancoEmp = new BancoEmpresaRepository().GetBancoEmpresa();
            ViewBag.datosBancoEmp = listaBancoEmp;
            var listaTipoCadena = new c_TipoCadenaPagoRepository().GetTipoCadenaPago();
            ViewBag.datosTipoCadena = listaTipoCadena;



            return View(elemento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagarFactura(PagoFactura PagoFactura)
        {
            int cuantoserrores = 0;
            if (!ModelState.IsValid)
            {
                for (int i = 0; i < ModelState.Values.Count; i++)
                {
                    cuantoserrores += ModelState.Values.ElementAt(i).Errors.Count;
                }
            }



            if (ModelState.IsValid || ((ModelState.Values.ElementAt(7).Errors.Count == 1 && cuantoserrores == 1) || (ModelState.Values.ElementAt(8).Errors.Count == 1 && cuantoserrores == 1)))
            {
                try
                {
                    DateTime fechareq = PagoFactura.FechaPago;
                    string fecha2 = fechareq.ToString("yyyy/MM/dd HH:mm:ss");
                    ClsDatoEntero folio = db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(Folio), 0) AS INT)+1 as Dato from [dbo].[PagoFactura] ").ToList()[0];

                    string comando = "insert into [dbo].[PagoFactura](IDCliente,FechaPago,IDFormaPago, IDMoneda,TC, NoOperacion, Monto, IDBancoCliente, IDBancoEmpresa, IDTipoCadenaPago, Observacion,Estado, Folio, StatusPago) values('" + PagoFactura.IDCliente + "', Convert (datetime,'" + fecha2 + "',101), " + PagoFactura.IDFormaPago + ", " + PagoFactura.IDMoneda + ", " + PagoFactura.TC + ", " + PagoFactura.NoOperacion + ", " + PagoFactura.Monto + ", " + PagoFactura.IDBancoCliente + ", " + PagoFactura.IDBancoEmpresa + ", " + PagoFactura.IDTipoCadenaPago + ", '" + PagoFactura.Observacion + "',0," + folio.Dato + ", 'A')";
                    db.Database.ExecuteSqlCommand(comando);
                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                }
                return RedirectToAction("Index");
            }
            ClientesContext prov = new ClientesContext();
            var cliente = prov.Clientes.OrderBy(m => m.Nombre).ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            foreach (var m in cliente)
            {
                li.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
                ViewBag.proveedor = li;

            }


            c_MonedaContext moneda = new c_MonedaContext();
            var monedas = moneda.c_Monedas.ToList();
            List<SelectListItem> lii = new List<SelectListItem>();
            lii.Add(new SelectListItem { Text = "--Selecciona Moneda--", Value = "0" });

            foreach (var m in monedas)
            {
                lii.Add(new SelectListItem { Text = m.ClaveMoneda, Value = m.IDMoneda.ToString() });
                ViewBag.datosMoneda = lii;

            }

            ViewBag.datosClientes = new SelectList(GetClientes());

            var listaFormaPago = new FormaPagoRepository().GetFormasdepagoElectronica();
            ViewBag.datosFormaPago = listaFormaPago;

            //var listaMoneda = new MonedaRepository().GetMoneda();
            //ViewBag.datosMoneda = listaMoneda;


            var listaBancoEmp = new BancoEmpresaRepository().GetBancoEmpresa();
            ViewBag.datosBancoEmp = listaBancoEmp;
            var listaTipoCadena = new c_TipoCadenaPagoRepository().GetTipoCadenaPago();
            ViewBag.datosTipoCadena = listaTipoCadena;

            return View(PagoFactura);
        }


        // pago FACTURA EFECTIVO
        ////////////////////////////////////////////////PAGO FACTURA EFECTIVO//////////////////////////////////////
        public ActionResult PagarFacturaE()
        {
            ClientesContext prov = new ClientesContext();
            var cliente = prov.Clientes.ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            foreach (var m in cliente)
            {
                li.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
                ViewBag.proveedor = li;

            }


            c_MonedaContext moneda = new c_MonedaContext();
            var monedas = moneda.c_Monedas.ToList();
            List<SelectListItem> lii = new List<SelectListItem>();
            lii.Add(new SelectListItem { Text = "--Selecciona Moneda--", Value = "0" });

            foreach (var m in monedas)
            {
                lii.Add(new SelectListItem { Text = m.ClaveMoneda, Value = m.IDMoneda.ToString() });
                ViewBag.datosMoneda = lii;
            }








            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;






            PagoFactura elemento = new PagoFactura();
            //elemento.FechaPago = DateTime.Now;

            List<c_Moneda> monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int destino = monedadestino.Select(s => s.IDMoneda).FirstOrDefault();
            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='USD'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            decimal Cambio = 0;
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + origen + "," + destino + ") as TC").ToList()[0];
            Cambio = cambio.TC;
            ViewBag.datostc = Cambio;
            elemento.TC = cambio.TC;




            ViewBag.datosClientes = new SelectList(GetClientes());

            var listaFormaPago = new FormaPagoRepository().GetFormasdepagoOtra();
            ViewBag.datosFormaPago = listaFormaPago;

            //var listaBancoEmp = new BancoEmpresaRepository().GetBancoEmpresa();
            // ViewBag.datosBancoEmp = listaBancoEmp;
            var listaTipoCadena = new c_TipoCadenaPagoRepository().GetTipoCadenaPago();
            ViewBag.datosTipoCadena = listaTipoCadena;

            return View(elemento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagarFacturaE(PagoFactura PagoFactura)
        {


            if (ModelState.IsValid)
            {
                DateTime fechareq = PagoFactura.FechaPago;
                string fecha2 = fechareq.ToString("yyyy/MM/dd HH:mm:ss");

                ClsDatoEntero folio = db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(Folio), 0) AS INT)+1 as Dato from [dbo].[PagoFactura] ").ToList()[0];
                string comando = "insert into [dbo].[PagoFactura](IDCliente,FechaPago,IDFormaPago, IDMoneda,TC, NoOperacion, Monto, IDBancoCliente, IDBancoEmpresa, IDTipoCadenaPago, Observacion,Estado, Folio ,StatusPago) values('" + PagoFactura.IDCliente + "', Convert (datetime,'" + fecha2 + "',101), " + PagoFactura.IDFormaPago + ", " + PagoFactura.IDMoneda + ", " + PagoFactura.TC + ", 0, " + PagoFactura.Monto + ",  0, 0, 1 , '" + PagoFactura.Observacion + "',0," + folio.Dato + ", 'A')";
                db.Database.ExecuteSqlCommand(comando);

            }
            return RedirectToAction("IndexEfe");

        }


        public FileResult DescargaPoliza(string Numero, string Numerof, string SerieFac, string ClieFac, string sortOrder, string currentFilter, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A")
        {
            string ConsultaSql = "SELECT * FROM [dbo].[VPagoFactura]";
            string Filtro = string.Empty;
            string Orden = " order by  IDPagoFactura desc ";


            //Buscar por numero
            if (!String.IsNullOrEmpty(Numero))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where IDPagoFactura>=" + Numero + "";
                }
                else
                {
                    Filtro += "and  IDPagoFactura>=" + Numero + "";
                }
            }

            if (!String.IsNullOrEmpty(Numerof))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where IDPagoFactura<=" + Numerof + "";
                }
                else
                {
                    Filtro += "and  IDPagoFactura<=" + Numerof + "";
                }
            }

            ///tabla filtro: Nombre Cliente
            if (!String.IsNullOrEmpty(ClieFac) && ClieFac!="Todas")
            {

                if (Filtro == string.Empty)
                {
                    Filtro = "where Nombre='" + ClieFac + "'";
                }
                else
                {
                    Filtro += " and  Nombre='" + ClieFac + "'";
                }

            }

            ///tabla filtro: Factura pagada/no pagada && Serie, Nombre Cliente



           
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where StatusPago='A'";
                    }
                    else
                    {
                        Filtro += "and StatusPago='A'";
                    }
            



            if (!String.IsNullOrEmpty(Fechainicio) && !String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where  FechaPago BETWEEN '" + Fechainicio + "' and '" + Fechafinal + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and FechaPago BETWEEN '" + Fechainicio + "' and '" + Fechafinal + " 23:59:59.999'";
                }
            }



            //Ordenacion

          
                    Orden = " order by FechaPago ";
                   

            //var elementos = from s in db.encfacturas
            //select s;
            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

            VPagoFacturaContext dbpv = new VPagoFacturaContext();
            List<VPagoFactura> pagoFactura = dbpv.Database.SqlQuery<VPagoFactura>(cadenaSQl).ToList();



            try
            {


               
            

                StringBuilder renglon = new StringBuilder();
                Char comillas = new Char();
                comillas = System.Char.Parse("\""); //comillas ;


                string cuentaVentas = string.Empty;
                string VentasConcepto = string.Empty;
                string cuentaiva16 = string.Empty;
                string conceptoiva16 = string.Empty;
                string VersionCOI = string.Empty;
                string Perdidacambiaria = string.Empty;
                string Gananciacambiaria = string.Empty;
                string CuentaComplementariaDLS = string.Empty;
                string conceptoComplementariaDLS = string.Empty;
                String CuentaIVACobrado = string.Empty;
                string Conceptoivacobrado = string.Empty;
                string CuentaCheque = string.Empty;
                string CuentaCaja = string.Empty;
                string CuentaTD = string.Empty;
                string CuentaTC = string.Empty;

                try
                {
                    XmlDocument xm = new XmlDocument();
                    xm.Load(System.Web.HttpContext.Current.Server.MapPath("~/configCuentas.xml"));


                    XmlNode cuentasnode = xm.FirstChild;
                    try
                    {
                        foreach (XmlNode cuentas in cuentasnode.ChildNodes)
                        {

                            if (cuentas.Name == "VentasCuenta")
                            {
                                cuentaVentas = cuentas.InnerText;
                            }
                            if (cuentas.Name == "VentasConcepto")
                            {
                                VentasConcepto = cuentas.InnerText;
                            }
                            if (cuentas.Name == "IVACuenta16")
                            {
                                cuentaiva16 = cuentas.InnerText;
                            }
                            if (cuentas.Name == "IVAConcepto16")
                            {
                                conceptoiva16 = cuentas.InnerText;
                            }

                            if (cuentas.Name == "versionCOI")
                            {
                                VersionCOI = cuentas.InnerText;
                            }

                            if (cuentas.Name == "Perdidacambiaria")
                            {
                                Perdidacambiaria = cuentas.InnerText;


                            }
                            if (cuentas.Name == "Gananciacambiaria")
                            {
                                Gananciacambiaria = cuentas.InnerText;
                            }
                            if (cuentas.Name == "CuentaComplementariaDLS")
                            {
                                CuentaComplementariaDLS = cuentas.InnerText;
                            }
                            if (cuentas.Name == "ConceptoComplementariaDLS")
                            {
                                conceptoComplementariaDLS = cuentas.InnerText;
                            }
                            if (cuentas.Name == "IVACobradoCuenta")
                            {
                                CuentaIVACobrado = cuentas.InnerText;
                            }
                            if (cuentas.Name == "IVACobradoConcepto")
                            {
                                Conceptoivacobrado = cuentas.InnerText;
                            }
                            if (cuentas.Name == "CuentaCheque")
                            {
                                CuentaCheque = cuentas.InnerText;
                            }
                            if (cuentas.Name == "CuentaCaja")
                            {
                                CuentaCaja = cuentas.InnerText;
                            }
                            if (cuentas.Name == "CuentaTD")
                            {
                                CuentaTD = cuentas.InnerText;
                            }
                            if (cuentas.Name == "CuentaTC")
                            {
                                CuentaTC = cuentas.InnerText;
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        throw new Exception("Hay un error con una configuracion de cuentas Contables " + err.Message);
                    }
                }

                catch (Exception err)
                {
                    throw new Exception("Hay un error con tu archivo de configuracion de cuentas Contables " + err.Message);
                }

             
                decimal acuiva = 0;
                decimal acuivaUSD = 0;



                renglon.Append("<?xml version=" + comillas + "1.0" + comillas + " encoding=" + comillas + "utf-8" + comillas + " standalone=" + comillas + "yes" + comillas + "?>\n");
                renglon.Append("<DATAPACKET Version=" + comillas + "2.0" + comillas + ">\n");
                renglon.Append("<METADATA>\n");
                renglon.Append("<FIELDS>\n");
                renglon.Append("<FIELD attrname=" + comillas + "VersionCOI" + comillas + " fieldtype=" + comillas + "i2" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "TipoPoliz" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "2" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "DiaPoliz" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "2" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "ConcepPoliz" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "120" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "Partidas" + comillas + " fieldtype=" + comillas + "nested" + comillas + ">\n");
                renglon.Append("<FIELDS>\n");
                renglon.Append("<FIELD attrname=" + comillas + "Cuenta" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "21" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "Depto" + comillas + " fieldtype=" + comillas + "i4" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "ConceptoPol" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "120" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "Monto" + comillas + " fieldtype=" + comillas + "r8" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "TipoCambio" + comillas + " fieldtype=" + comillas + "r8" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "DebeHaber" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "1" + comillas + " />\n");
                renglon.Append("</FIELDS>\n");
                renglon.Append("<PARAMS />\n");
                renglon.Append("</FIELD>\n");
                renglon.Append("</FIELDS>\n");
                renglon.Append("<PARAMS />\n");
                renglon.Append("</METADATA>\n");
                renglon.Append("<ROWDATA>\n");
                renglon.Append("<ROW VersionCOI=" + comillas + VersionCOI + comillas + " TipoPoliz=" + comillas + "Dr" + comillas + " DiaPoliz=" + comillas + "1" + comillas + " ConcepPoliz=" + comillas + "Poliza de Ventas" + comillas + ">\n");
                renglon.Append("<Partidas>\n");
                decimal acuganancia = 0;
                decimal acuperdida = 0;
         
               foreach (VPagoFactura pago in pagoFactura)
                {
                    PagoFacturaContext dbf = new PagoFacturaContext();
                    PagoFactura pagof =  dbf.PagoFacturas.Find(pago.IDPagoFactura);
                  //  var documentos = dbf.Database.SqlQuery<DocumentoRelacionado>("select * from [dbo].[DocumentoRelacionado] where IDPagoFactura=" + pagof.IDPagoFactura ).ToList();
                    List<DetallesDR> documentos = dbf.Database.SqlQuery<DetallesDR>("select IDFactura, Serie, Numero,D.IDMoneda, M.ClaveMoneda, M.Descripcion, TC, IDMetododepago, ImporteSaldoInsoluto, ImportePagado, NoParcialidad from [dbo].[DocumentoRelacionado] as D inner join c_Moneda as M on D.IDMoneda= M.IDMoneda  where IDPagoFactura='" + pagof.IDPagoFactura + "'").ToList();
                    Clientes cliente = new ClientesContext().Clientes.Find(pago.IDCliente);
                    decimal ivaporpago = 0M;
                    decimal montocliente = 0M;
                    decimal acumonto = 0;
                    decimal acumonto2 = 0;
                    foreach (DetallesDR dr in documentos)
                    {
                        Encfacturas facturacliente = new EncfacturaContext().encfacturas.Find(dr.IDFactura);
                        decimal monto = dr.ImportePagado;
                        decimal IVATEM = 0;
                        decimal monto2 = 0;
                        Decimal resta = 0;
                      
                            if (dr.ClaveMoneda == pago.ClaveDivisa && dr.ClaveMoneda=="MXN")   // PAGA EN PESOS UNA FACTURA EN PESOS
                            {
                                monto = Decimal.Parse((dr.ImportePagado / 1.16M).ToString());
                                IVATEM = dr.ImportePagado - monto;
                                resta = 0;
                                acuiva += IVATEM;
                                acumonto += monto;
                            }

                        if (dr.ClaveMoneda!="MXN" && pago.ClaveDivisa != "MXN" && pago.ClaveDivisa==dr.ClaveMoneda) // PAGA EN MONEDA EXTRANJERA UNA FACTURA EN MONEDA EXTRANJERA
                        {
                            resta = diferencia(facturacliente.TC, dr.ImportePagado, pago.TC);

                            monto = Decimal.Parse(((dr.ImportePagado * pago.TC)).ToString()) - resta;
                            IVATEM = 0;

                            if (resta > 0)
                            {
                                acuganancia += resta;
                            }
                            if (resta < 0)
                            {
                                acuperdida += resta * -1M;
                            }


                            if (facturacliente.IVA > 0)
                            {
                                monto = monto / 1.16M; // lo que restaba de la factura subtotal
                                IVATEM = monto * 0.16M; // lo que restaba del iva si todos los registros tienen IVA

                                /// EN CASO DE QUE NO TODAS LAS PARTIDAS USEN IVA, TENDRA QUE HACERCE TODO UN SISTEMA DE PAGO 

                            }
                            else
                            {

                                IVATEM = 0;
                            }


                            monto = monto +IVATEM - dr.ImportePagado;
                            monto2 = dr.ImportePagado;
                            acuivaUSD += IVATEM;
                            acumonto += monto;
                            acumonto2 += monto2;

                        }

                        if (dr.ClaveMoneda != pago.ClaveDivisa && pago.ClaveDivisa == "MXN") /// PAGA EN PESOS UNA FACTURA EN DOLARES
                            {
                            resta = diferencia(facturacliente.TC, dr.ImportePagado, pago.TC);

                            monto = Decimal.Parse(((dr.ImportePagado * pago.TC)).ToString()) - resta;
                            IVATEM = 0;

                            if (resta > 0)
                            {
                                acuganancia += resta;
                            }
                            if (resta < 0)
                            {
                                acuperdida += resta * -1M;
                            }


                            if (facturacliente.IVA > 0)
                            {
                                monto = monto / 1.16M; // lo que restaba de la factura subtotal
                                IVATEM = monto * 0.16M; // lo que restaba del iva si todos los registros tienen IVA

                                /// EN CASO DE QUE NO TODAS LAS PARTIDAS USEN IVA, TENDRA QUE HACERCE TODO UN SISTEMA DE PAGO 

                            }
                            else
                            {

                                IVATEM = 0;
                            }


                            monto = monto - dr.ImportePagado;
                            monto2 = dr.ImportePagado;


                            acuivaUSD += IVATEM;
                            acumonto += monto;
                            acumonto2 += monto2;
                        }



                        // IF MONTO2 TRAE VALOR SE HACE UN REGISTRO POR ESTE MONTO2 Y MONTO SE VA A COMPLEMENTARIA
                        // IF MONTO VIENE SIN MONTO2 SE HACE UN REGISTRO SOLO PUES EL PAGO FUA A PESOS
                        // SE REGISTRA EN EL BANCO MONTO + MONTO2


                        //if (cliente.cuentaContable == string.Empty)
                        //{
                        //    throw new Exception("No existe cuenta contable para el cliente " + cliente.Nombre);
                        //}


                        renglon.Append("<ROWPartidas Cuenta=" + comillas + cliente.cuentaContable + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + " FACTURA " + dr.Serie+ dr.Numero + " No de parc. " + dr.NoParcialidad  + comillas + " Monto=" + comillas + Math.Round(monto + monto2 + IVATEM, 2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "H" + comillas + " />\n");
                       

                        montocliente += monto + monto2 + IVATEM ;
                         // ACUMULA LOS IVA DE LOS DOCUMENTOS RELACIONADOS
                        ivaporpago += IVATEM;

                    }  //POR CADA DOCUMENTO RELACIONADO


                    

                                        

                 
                    BancoEmpresa banco = new BancoEmpresaContext().BancoEmpresa.Find(pagof.IDBancoEmpresa);

                    if (banco==null)

                        {

                        banco = new BancoEmpresaContext().BancoEmpresa.Find(1);
                    }

                    

                    if (banco.cuentaContable== string.Empty || banco.cuentaContable==null)
                    {
                        throw new Exception("No existe cuenta contable para el banco " + banco.c_Banco.RazonSocial + " " + banco.CuentaBanco);
                    }

                    string asientocuenta = banco.cuentaContable;

                    string asientoConcepto = banco.c_Banco.Nombre + " " + banco.c_Moneda.ClaveMoneda + " " + banco.CuentaBanco;

                    if (pago.ClaveFormaPago=="02")
                    {
                        asientocuenta = banco.cuentaContable;  // sustituimos al banco por la cuenta de chques 
                        asientoConcepto = "CHEQUE NOMINATIVO " +pago.NoOperacion;
                    }
                    if (pago.ClaveFormaPago == "01")
                    {
                        banco.cuentaContable = CuentaCaja;  // sustituimos al banco por la cuenta de efectivo
                        asientoConcepto = "EFECTIVO";
                    }

                    if (pago.ClaveFormaPago == "04")
                    {
                        asientocuenta = CuentaTD;  // sustituimos al banco por la cuenta de tarjeta de CREDITO 
                        asientoConcepto = "TARJETA DE CREDITO " + pago.NoOperacion;  
                    }

                    if (pago.ClaveFormaPago == "28")
                    {
                        asientocuenta = CuentaTD;  // sustituimos al banco por la cuenta de tarjeta de debito 
                        asientoConcepto = "TARJETA DE DEBITO " + pago.NoOperacion;
                    }

                    // asiento en el debe del banco
                    if  (acumonto2 == 0)

                    {
                        renglon.Append("<ROWPartidas Cuenta=" + comillas + asientocuenta + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + asientoConcepto+ comillas + " Monto=" + comillas + Math.Round(montocliente ,2 ) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "D" + comillas + " />\n");
                    }
                    else
                    {
                        renglon.Append("<ROWPartidas Cuenta=" + comillas + asientocuenta + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + asientoConcepto + comillas + " Monto=" + comillas + Math.Round(acumonto2, 2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "D" + comillas + " />\n");
                        renglon.Append("<ROWPartidas Cuenta=" + comillas + CuentaComplementariaDLS + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + conceptoComplementariaDLS + comillas + " Monto=" + comillas + Math.Round(acumonto + ivaporpago, 2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "D" + comillas + " />\n");
                    }
                    ivaporpago = 0;
                    montocliente = 0;
                }   /// final de cada pago

                if (acuivaUSD > 0)
                {
                    renglon.Append("<ROWPartidas Cuenta=" + comillas + cuentaiva16 + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + conceptoiva16 + " MONEDA EXTRANJERA" + comillas + " Monto=" + comillas + Math.Round( acuivaUSD,2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "D" + comillas + " />\n");
                    renglon.Append("<ROWPartidas Cuenta=" + comillas + CuentaIVACobrado + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + Conceptoivacobrado + " MONEDA EXTRANJERA" + comillas + " Monto=" + comillas + Math.Round(acuivaUSD, 2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "H" + comillas + " />\n");
                }

                if (acuiva > 0)
                {
                    renglon.Append("<ROWPartidas Cuenta=" + comillas + cuentaiva16 + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + conceptoiva16 + " MXN" + comillas + " Monto=" + comillas + Math.Round(acuiva,2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "D" + comillas + " />\n");
                    renglon.Append("<ROWPartidas Cuenta=" + comillas + CuentaIVACobrado + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + Conceptoivacobrado + " MXN" + comillas + " Monto=" + comillas + Math.Round(acuiva, 2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "H" + comillas + " />\n");
                }

                if (acuganancia>0)
                {
                    renglon.Append("<ROWPartidas Cuenta=" + comillas + CuentaComplementariaDLS + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + conceptoComplementariaDLS + comillas + " Monto=" + comillas + Math.Round(acuganancia, 2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "D" + comillas + " />\n");
                    renglon.Append("<ROWPartidas Cuenta=" + comillas + Gananciacambiaria + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas +"GANANCIA CAMBIARIA " + comillas + " Monto=" + comillas + Math.Round(acuganancia, 2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "H" + comillas + " />\n");
                }

                if (acuperdida > 0)
                {
                    renglon.Append("<ROWPartidas Cuenta=" + comillas + CuentaComplementariaDLS + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + conceptoComplementariaDLS + comillas + " Monto=" + comillas + Math.Round(acuperdida, 2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "H" + comillas + " />\n");
                    renglon.Append("<ROWPartidas Cuenta=" + comillas + Perdidacambiaria + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + "PERDIDA CAMBIARIA " + comillas + " Monto=" + comillas + Math.Round(acuperdida, 2) + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "D" + comillas + " />\n");
                }


                renglon.Append("</Partidas>\n");
                renglon.Append("</ROW>\n");
                renglon.Append("</ROWDATA>\n");
                renglon.Append("</DATAPACKET>\n");


                var stream = new MemoryStream(Encoding.ASCII.GetBytes(renglon.ToString()));

                return File(stream, "text/plain", "PolizaFacturacion.Pol");


            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                 string  renglon=  "Error : " + err.Message;

                //   var filresult = File(new System.Text.UTF8Encoding().GetBytes(renglon), "application/csv", "downloaddocuments.csv");
                // return filresult;

                var stream = new MemoryStream(Encoding.ASCII.GetBytes(renglon));

                return File(stream, "text/plain", "Error.txt");



            }

        }

        /// <summary>
        /// pagos en pesos de una factura identificada por IDFactura y cuya moneda es claveMoneda MXN ,USD
        /// </summary>
        /// <param name="IDfactura"></param>
        /// <param name="claveMoneda"></param>
        /// <returns></returns>
       public decimal pagoenpesos(int IDfactura, string claveMoneda)
        {
          var pagodefactura =  db.Database.SqlQuery<DocumentoRelacionado>("select DocumentoRelacionado.* from [DocumentoRelacionado] inner join encFacturas on [DocumentoRelacionado].IDFactura=" + IDfactura).ToList();
            decimal acupago = 0M;
            foreach (DocumentoRelacionado item in pagodefactura )
            {
                string monedadepago = new c_MonedaContext().c_Monedas.Find(item.IDMoneda).ClaveMoneda; 
                if (claveMoneda=="MXN" && monedadepago=="MXN")
                {
                    acupago += item.ImportePagado;
                }
                if (claveMoneda != "MXN" && monedadepago == "MXN")
                {
                    acupago += item.ImportePagado;
                }
                if (claveMoneda == "MXN" && monedadepago != "MXN")
                {
                    acupago += item.ImportePagado * item.TC;
                }
                if (claveMoneda != "MXN" && monedadepago != "MXN")
                {
                    acupago += item.ImportePagado * item.TC;
                }
            }

            return acupago;
        } 

        public decimal diferencia ( decimal tcfactura, decimal totalpagado, decimal tcpago )
        {
            decimal monto1 = totalpagado * tcfactura;
            decimal monto2 = totalpagado * tcpago;
            decimal dif = monto2 - monto1;
            return dif;
        }
      
        public ActionResult EntreFechasPago()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasPago(EFecha modelo, FormCollection coleccion)
        {
            VPagoClieContext dbe = new VPagoClieContext();
            VPagoClieDoctosContext dbr = new VPagoClieDoctosContext();
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
                List<VPagoClie> datos;
                List<VPagoClieDoctos> datosDet;
                try
                {
                    cadena = "select * from dbo.VPagoClie where FechaPago >= '" + FI + "' and FechaPago  <='" + FF + "' ";
                    datos = db.Database.SqlQuery<VPagoClie>(cadena).ToList();
                }
                catch (Exception err)
                {
                    throw new Exception("Hay un error " + err.Message);
                }

                //ViewBag.req = pedido;
                //var datos = dbe.Database.SqlQuery<VPagoClie>(cadena).ToList();
                //ViewBag.datos = datos;
                try
                {
                    cadenaDet = "select * from [dbo].[VPagoClieDoctos] where FechaPago >= '" + FI + "' and FechaPago <='" + FF + "' ";
                    datosDet = db.Database.SqlQuery<VPagoClieDoctos>(cadenaDet).ToList();
                }
                catch (Exception err)
                {
                    throw new Exception("Hay un error " + err.Message);
                }
                //var datosDet = dbr.Database.SqlQuery<VPagoClieDoctos>(cadenaDet).ToList();
                //ViewBag.datosDet = datosDet;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Pago Factura");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:X1"].Style.Font.Size = 20;
                Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:X3"].Style.Font.Bold = true;
                Sheet.Cells["A1:X1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:X1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Pago Factura");

                row = 2;
                Sheet.Cells["A1:X1"].Style.Font.Size = 12;
                Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:X1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:X2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3:X3"].Style.Font.Bold = true;
                Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID Pago Factura");
                Sheet.Cells["B3"].RichText.Add("Serie");
                Sheet.Cells["C3"].RichText.Add("Folio");
                Sheet.Cells["D3"].RichText.Add("Fecha Pago");
                Sheet.Cells["E3"].RichText.Add("RFC");
                Sheet.Cells["F3"].RichText.Add("Nombre");
                Sheet.Cells["G3"].RichText.Add("Monto");
                Sheet.Cells["H3"].RichText.Add("Clave Moneda");
                Sheet.Cells["I3"].RichText.Add("TC");
                Sheet.Cells["J3"].RichText.Add("Clave Forma de Pago");
                Sheet.Cells["K3"].RichText.Add("Forma de Pago");
                Sheet.Cells["L3"].RichText.Add("No. de Operación");
                Sheet.Cells["M3"].RichText.Add("Banco Cliente");
                Sheet.Cells["N3"].RichText.Add("Banco Empresa");
                Sheet.Cells["O3"].RichText.Add("Observación");
                Sheet.Cells["P3"].RichText.Add("Fecha Cancelación");
                Sheet.Cells["Q3"].RichText.Add("Estado del Pago");
                Sheet.Cells["R3"].RichText.Add("Ruta XML");
                Sheet.Cells["S3"].RichText.Add("UUID");
                Sheet.Cells["T3"].RichText.Add("Observación de cancelación");
                Sheet.Cells["U3"].RichText.Add("Tipo Cadena de Pago");
                Sheet.Cells["V3"].RichText.Add("Certificado de Pago");
                Sheet.Cells["W3"].RichText.Add("ID Cadena de Pago");
                Sheet.Cells["X3"].RichText.Add("Sello de Pago");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VPagoClie item in datos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDPagoFactura;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Serie;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Folio;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.FechaPago;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.RFC;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Monto;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.ClaveFormaPago;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.FormaPago;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.NoOperacion;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.BancoCliente;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.BancoEmpresa;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.Observacion;
                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.FechaCancelacion;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.StatusPago;
                    Sheet.Cells[string.Format("R{0}", row)].Value = item.RutaXML;
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.UUID;
                    Sheet.Cells[string.Format("T{0}", row)].Value = item.ObsCancela;
                    Sheet.Cells[string.Format("U{0}", row)].Value = item.IDTipoCadenaPago;
                    Sheet.Cells[string.Format("V{0}", row)].Value = item.CertificadoPago;
                    Sheet.Cells[string.Format("W{0}", row)].Value = item.IDTipoCadenaPago;
                    Sheet.Cells[string.Format("X{0}", row)].Value = item.SelloPago;
                    row++;
                }

                //Hoja No. 2
                Sheet = Ep.Workbook.Worksheets.Add("Documentos Relacionados");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:V1"].Style.Font.Size = 20;
                Sheet.Cells["A1:V1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:V3"].Style.Font.Bold = true;
                Sheet.Cells["A1:V1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:V1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Documentos Relacionados");

                row = 2;
                Sheet.Cells["A1:V1"].Style.Font.Size = 12;
                Sheet.Cells["A1:V1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:V1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:V2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3"].RichText.Add("ID Documento");
                Sheet.Cells["B3"].RichText.Add("ID Pago Factura");
                Sheet.Cells["C3"].RichText.Add("Fecha Pago");
                Sheet.Cells["D3"].RichText.Add("RFC");
                Sheet.Cells["E3"].RichText.Add("Cliente"); ;
                Sheet.Cells["F3"].RichText.Add("No. Factura");
                Sheet.Cells["G3"].RichText.Add("Fecha Factura ");
                Sheet.Cells["H3"].RichText.Add("Serie");
                Sheet.Cells["I3"].RichText.Add("Número");
                Sheet.Cells["J3"].RichText.Add("No. Operación");
                Sheet.Cells["K3"].RichText.Add("Subtotal");
                Sheet.Cells["L3"].RichText.Add("IVA");
                Sheet.Cells["M3"].RichText.Add("Total");
                Sheet.Cells["N3"].RichText.Add("Clave Moneda");
                Sheet.Cells["O3"].RichText.Add("TC");
                Sheet.Cells["P3"].RichText.Add("Saldo Anterior");
                Sheet.Cells["Q3"].RichText.Add("Importe Pagado");
                Sheet.Cells["R3"].RichText.Add("Importe Saldo Insoluto");
                Sheet.Cells["S3"].RichText.Add("No. Parcialidad");
                Sheet.Cells["T3"].RichText.Add("Estado del documento");
                Sheet.Cells["U3"].RichText.Add("Clave Método de pago");
                Sheet.Cells["V3"].RichText.Add("Método de pago");
                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:V3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VPagoClieDoctos itemD in datosDet)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.IDDocumentoRelacionado;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.IDPagoFactura;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.FechaFactura;
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemD.RFC;
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemD.Nombre;
                    Sheet.Cells[string.Format("F{0}", row)].Value = itemD.IDFactura;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.FechaFactura;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.Serie;
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.Numero;
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.NoOperacion;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.Subtotal;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.IVA;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = itemD.Total;
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("N{0}", row)].Value = itemD.ClaveMoneda;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("O{0}", row)].Value = itemD.TC;
                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("P{0}", row)].Value = itemD.SaldoAnterior;
                    Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("Q{0}", row)].Value = itemD.ImportePagado;
                    Sheet.Cells[string.Format("R{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("R{0}", row)].Value = itemD.ImporteSaldoInsoluto;
                    Sheet.Cells[string.Format("S{0}", row)].Value = itemD.NoParcialidad;
                    Sheet.Cells[string.Format("T{0}", row)].Value = itemD.StatusDocto;
                    Sheet.Cells[string.Format("U{0}", row)].Value = itemD.IDMetododepago;
                    Sheet.Cells[string.Format("V{0}", row)].Value = itemD.MetodoPago;

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
        public ActionResult CreaReporteGeneralClie()
        {
            //Buscar Cliente
            ClientesContext dbc = new ClientesContext();
            var cliente = dbc.Clientes.OrderBy(m => m.Nombre).ToList();
            List<SelectListItem> listaCliente = new List<SelectListItem>();
            listaCliente.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            foreach (var m in cliente)
            {
                listaCliente.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
            }
            ViewBag.cliente = listaCliente;
            return View();
        }

        [HttpPost]
        public ActionResult CreaReporteGeneralClie(ReportefenoClie A, FormCollection coleccion)
        {
            int idcliente = A.IDCliente;
            DateTime fi = A.Fechainicio;
            DateTime ff = A.Fechafinal;
            string nota = A.Nota;

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {
                try
                {
                    ClientesContext dbc = new ClientesContext();
                    Clientes cliente = dbc.Clientes.Find(A.IDCliente);
                }
                catch (Exception er)
                {
                }
                ReporteporCliente report = new ReporteporCliente();

                byte[] abytes = report.PrepareReport(fi, ff, idcliente, nota);
                return File(abytes, "application/pdf");
            }
            else
            {
                List<ReportefenoClie> pagoFactura = new List<ReportefenoClie>();

                string FI = A.Fechainicio.Year.ToString() + "-" + A.Fechainicio.Month.ToString() + "-" + A.Fechainicio.Day.ToString();
                string FF = A.Fechafinal.Year.ToString() + "-" + A.Fechafinal.Month.ToString() + "-" + A.Fechafinal.Day.ToString();

                string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59'";
                if (idcliente != 0)
                {
                    cadena = cadena + "and IDCliente=" + idcliente + "";
                }
                pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();

                ViewBag.pagoFactura = pagoFactura;

                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("Clientes");

                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Reporte General del Cliente");
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 2;
                Sheet.Cells["A2:J2"].Style.Font.Size = 12;
                Sheet.Cells["A2:J2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:J2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos

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
                Sheet.Cells["A3:J3"].Style.Font.Bold = true;
                Sheet.Cells["A3:J3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Fecha de Pago");
                Sheet.Cells["B3"].RichText.Add("Nombre");
                Sheet.Cells["C3"].RichText.Add("Descripción");
                Sheet.Cells["D3"].RichText.Add("Clave Divisa");
                Sheet.Cells["E3"].RichText.Add("TC");
                Sheet.Cells["F3"].RichText.Add("No. Operación");
                Sheet.Cells["G3"].RichText.Add("Monto");
                Sheet.Cells["H3"].RichText.Add("RFC Banco");
                Sheet.Cells["I3"].RichText.Add("Cuenta Emisor");
                Sheet.Cells["J3"].RichText.Add("Estado");

                row = 4;
                foreach (var item in pagoFactura)
                {
                    string es = "";
                    if (item.Estado == true)
                    {
                        es = "Activo";
                    }
                    else
                    {
                        es = "Cancelado";
                    }
                    string FPago = item.FechaPago.Year.ToString() + "-" + item.FechaPago.Month.ToString() + "-" + item.FechaPago.Day.ToString();

                    Sheet.Cells[string.Format("A{0}", row)].Value = FPago;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveDivisa;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.NoOperacion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Monto;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.RFCBancoEmisor;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.CuentaEmisor;
                    Sheet.Cells[string.Format("J{0}", row)].Value = es;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturasPagadas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();

            }
            return View(ViewBag.pagoFactura);
        }
        public ActionResult CreaReporteporFPagoClie()
        {
            //Buscar fPago
            c_FormaPagoContext dbc = new c_FormaPagoContext();
            var fpa = dbc.c_FormaPagos.OrderBy(m => m.Descripcion).ToList();
            List<SelectListItem> listafrmP = new List<SelectListItem>();
            listafrmP.Add(new SelectListItem { Text = "--Selecciona Forma de pago--", Value = "0" });

            var EstadoLst = new List<SelectListItem>();

            EstadoLst.Add(new SelectListItem { Text = "Todos", Value = "T" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });


            ViewBag.Status = new SelectList(EstadoLst, "Value", "Text");

            foreach (var m in fpa)
            {
                listafrmP.Add(new SelectListItem { Text = m.Descripcion, Value = m.ClaveFormaPago.ToString() });//IDFormaPago
            }
            ViewBag.pago = listafrmP;
            return View();
        }

        [HttpPost]
        public ActionResult CreaReporteporFPagoClie(ReportefenoClie A, FormCollection coleccion)
        {

            DateTime fi = A.Fechainicio;
            DateTime ff = A.Fechafinal;
            string esta = A.Status;
            string nota = A.Nota;
            string pago = A.IDPago;

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {
                try
                {
                    ClientesContext dbc = new ClientesContext();
                    Clientes cliente = dbc.Clientes.Find(A.IDCliente);
                }
                catch (Exception er)
                {
                }
                ReporteporFPago report = new ReporteporFPago();

                byte[] abytes = report.PrepareReport(fi, ff, pago, esta, nota);
                return File(abytes, "application/pdf");
            }
            else
            {
                List<ReportefenoClie> pagoFactura = new List<ReportefenoClie>();

                string FI = A.Fechainicio.Year.ToString() + "-" + A.Fechainicio.Month.ToString() + "-" + A.Fechainicio.Day.ToString();
                string FF = A.Fechafinal.Year.ToString() + "-" + A.Fechafinal.Month.ToString() + "-" + A.Fechafinal.Day.ToString();

                if (esta == "T")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59'";
                    if (pago != "")
                    {
                        cadena = cadena + "and ClaveFormaPago='" + pago + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }

                if (esta == "A")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59' and Estado = 1";
                    if (pago != "")
                    {
                        cadena = cadena + "and ClaveFormaPago='" + pago + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                if (esta == "C")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";

                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59' and Estado = 0";
                    if (pago != "")
                    {
                        cadena = cadena + "and ClaveFormaPago='" + pago + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                ViewBag.pagoFactura = pagoFactura;

                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("Clientes");

                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Reporte por Fecha de Pago");
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 2;
                Sheet.Cells["A2:J2"].Style.Font.Size = 12;
                Sheet.Cells["A2:J2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:J2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos

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
                Sheet.Cells["A3:J3"].Style.Font.Bold = true;
                Sheet.Cells["A3:J3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Fecha de Pago");
                Sheet.Cells["B3"].RichText.Add("Nombre");
                Sheet.Cells["C3"].RichText.Add("Descripción");
                Sheet.Cells["D3"].RichText.Add("Clave Divisa");
                Sheet.Cells["E3"].RichText.Add("TC");
                Sheet.Cells["F3"].RichText.Add("No. Operación");
                Sheet.Cells["G3"].RichText.Add("Monto");
                Sheet.Cells["H3"].RichText.Add("RFC Banco");
                Sheet.Cells["I3"].RichText.Add("Cuenta Emisor");
                Sheet.Cells["J3"].RichText.Add("Estado");

                row = 4;
                foreach (var item in pagoFactura)
                {
                    string es = "";
                    if (item.Estado == true)
                    {
                        es = "Activo";
                    }
                    else
                    {
                        es = "Cancelado";
                    }
                    string FPago = item.FechaPago.Year.ToString() + "-" + item.FechaPago.Month.ToString() + "-" + item.FechaPago.Day.ToString();

                    Sheet.Cells[string.Format("A{0}", row)].Value = FPago;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveDivisa;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.NoOperacion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Monto;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.RFCBancoEmisor;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.CuentaEmisor;
                    Sheet.Cells[string.Format("J{0}", row)].Value = es;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturasPagadas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();

            }
            return View(ViewBag.pagoFactura);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult CreaReporteOficinaClie()
        {
            //Buscar Cliente
            OficinaContext dbc = new OficinaContext();
            var oficina = dbc.Oficinas.OrderBy(m => m.NombreOficina).ToList();
            List<SelectListItem> listaOfi = new List<SelectListItem>();
            listaOfi.Add(new SelectListItem { Text = "--Selecciona Oficina--", Value = "0" });

            var EstadoLst = new List<SelectListItem>();


            EstadoLst.Add(new SelectListItem { Text = "Todos", Value = "T" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });

            ViewBag.Status = new SelectList(EstadoLst, "Value", "Text");

            foreach (var m in oficina)
            {
                listaOfi.Add(new SelectListItem { Text = m.NombreOficina, Value = m.IDOficina.ToString() });
            }
            ViewBag.ofi = listaOfi;
            return View();
        }

        public List<Oficina> GetOficina(int oficina)
        {
            List<Oficina> of = new List<Oficina>();
            string cadena = "select * from oficina where IDOficina =" + oficina;
            of = db.Database.SqlQuery<Oficina>(cadena).ToList();
            return of;
        }
        [HttpPost]
        public ActionResult CreaReporteOficinaClie(ReportefenoClie A, FormCollection coleccion)
        {
            DateTime fi = A.Fechainicio;
            DateTime ff = A.Fechafinal;
            string esta = A.Status;
            string nota = A.Nota;
            int oficina = A.IDOfi;

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {
                try
                {
                    ClientesContext dbc = new ClientesContext();
                    Clientes cliente = dbc.Clientes.Find(A.IDCliente);
                }
                catch (Exception er)
                {
                }
                ReporteClientesOficina report = new ReporteClientesOficina();

                byte[] abytes = report.PrepareReport(fi, ff, oficina, esta, nota);
                return File(abytes, "application/pdf");
            }
            else
            {


                List<ReportefenoClie> pagoFactura = new List<ReportefenoClie>();

                string FI = A.Fechainicio.Year.ToString() + "-" + A.Fechainicio.Month.ToString() + "-" + A.Fechainicio.Day.ToString();
                string FF = A.Fechafinal.Year.ToString() + "-" + A.Fechafinal.Month.ToString() + "-" + A.Fechafinal.Day.ToString();

                if (esta == "T")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59'";
                    if (oficina != 0)
                    {
                        cadena = cadena + "and IDOficina = '" + oficina + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }

                if (esta == "A")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59' and Estado=1";
                    if (oficina != 0)
                    {
                        cadena = cadena + "and IDOficina = '" + oficina + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                if (esta == "C")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + A.Fechainicio.Year + "/" + A.Fechainicio.Month + "/" + A.Fechainicio.Day + " 00:00:01' and FechaPago <='" + A.Fechafinal.Year + "/" + A.Fechafinal.Month + "/" + A.Fechafinal.Day + " 23:59:59' and IDOficina = '" + oficina + "' and Estado=0";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59' and Estado=0";
                    if (oficina != 0)
                    {
                        cadena = cadena + "and IDOficina = '" + oficina + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                ViewBag.pagoFactura = pagoFactura;

                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("Clientes");


                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Reporte por Oficina");
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 2;
                Sheet.Cells["A2:J2"].Style.Font.Size = 12;
                Sheet.Cells["A2:J2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:J2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos

                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                ////////////////////////////////////////////////////
                List<Oficina> ofi = new List<Oficina>();
                ofi = GetOficina(oficina);
                foreach (Oficina noficina in ofi)
                {
                    row = 3;
                    Sheet.Cells.Style.Font.Name = "Calibri";
                    Sheet.Cells.Style.Font.Size = 10;
                    Sheet.Cells["A3:J3"].Style.Font.Bold = true;
                    Sheet.Cells["A3:J3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["A3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    Sheet.Cells["A3"].RichText.Add("OFICINA: " + noficina.NombreOficina);
                    Sheet.Cells["C3"].RichText.Add("RESPONSABLE: " + noficina.Responsable);
                }
                //En la fila3 se da el formato a el encabezado
                row = 4;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A4:J4"].Style.Font.Bold = true;
                Sheet.Cells["A4:J4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A4:J4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A4"].RichText.Add("Fecha de Pago");
                Sheet.Cells["B4"].RichText.Add("Nombre");
                Sheet.Cells["C4"].RichText.Add("Descripción");
                Sheet.Cells["D4"].RichText.Add("Clave Divisa");
                Sheet.Cells["E4"].RichText.Add("TC");
                Sheet.Cells["F4"].RichText.Add("No. Operación");
                Sheet.Cells["G4"].RichText.Add("Monto");
                Sheet.Cells["H4"].RichText.Add("RFC Banco");
                Sheet.Cells["I4"].RichText.Add("Cuenta Emisor");
                Sheet.Cells["J4"].RichText.Add("Estado");

                row = 5;
                foreach (var item in pagoFactura)
                {
                    string es = "";
                    if (item.Estado == true)
                    {
                        es = "Activo";
                    }
                    else
                    {
                        es = "Cancelado";
                    }
                    string FPago = item.FechaPago.Year.ToString() + "-" + item.FechaPago.Month.ToString() + "-" + item.FechaPago.Day.ToString();

                    Sheet.Cells[string.Format("A{0}", row)].Value = FPago;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveDivisa;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.NoOperacion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Monto;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.RFCBancoEmisor;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.CuentaEmisor;
                    Sheet.Cells[string.Format("J{0}", row)].Value = es;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturasPagadas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }
            return View(ViewBag.pagoFactura);

        }
        public int idoficina { get; set; }

        public ClientesContext dbc = new ClientesContext();
        /////////////REPORTES/////////////

        public ActionResult CreaReporteporFechaClie()
        {
            //Buscar Cliente
            ClientesContext dbc = new ClientesContext();
            var cliente = dbc.Clientes.OrderBy(m => m.Nombre).ToList();
            List<SelectListItem> listaCliente = new List<SelectListItem>();
            listaCliente.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            var EstadoLst = new List<SelectListItem>();

            EstadoLst.Add(new SelectListItem { Text = "Todos", Value = "T" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });

            ViewBag.Status = new SelectList(EstadoLst, "Value", "Text");

            foreach (var m in cliente)
            {
                listaCliente.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
            }
            ViewBag.cliente = listaCliente;
            return View();
        }
        [HttpPost]
        public ActionResult CreaReporteporFechaClie(ReportefenoClie A, FormCollection coleccion)
        {
            int idcliente = A.IDCliente;
            DateTime fi = A.Fechainicio;
            DateTime ff = A.Fechafinal;
            string esta = A.Status;
            string nota = A.Nota;

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {
                try
                {
                    ClientesContext dbc = new ClientesContext();
                    Clientes cliente = dbc.Clientes.Find(A.IDCliente);
                }
                catch (Exception er)
                {
                }
                ReporteporFecha report = new ReporteporFecha();

                byte[] abytes = report.PrepareReport(fi, ff, idcliente, esta, nota);
                return File(abytes, "application/pdf");
            }
            else
            {
                List<ReportefenoClie> pagoFactura = new List<ReportefenoClie>();

                string FI = A.Fechainicio.Year.ToString() + "-" + A.Fechainicio.Month.ToString() + "-" + A.Fechainicio.Day.ToString();
                string FF = A.Fechafinal.Year.ToString() + "-" + A.Fechafinal.Month.ToString() + "-" + A.Fechafinal.Day.ToString();

                if (esta == "T")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59'";
                    if (idcliente != 0)
                    {
                        cadena = cadena + "and IDCliente = '" + idcliente + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                if (esta == "A")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59' and Estado = 1";
                    if (idcliente != 0)
                    {
                        cadena = cadena + "and IDCliente = '" + idcliente + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                if (esta == "C")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59' and Estado = 0";
                    if (idcliente != 0)
                    {
                        cadena = cadena + "and IDCliente = '" + idcliente + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                ViewBag.pagoFactura = pagoFactura;


                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("Clientes");

                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Reporte por fecha");
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 2;
                Sheet.Cells["A2:J2"].Style.Font.Size = 12;
                Sheet.Cells["A2:J2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:J2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos

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
                Sheet.Cells["A3:J3"].Style.Font.Bold = true;
                Sheet.Cells["A3:J3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Fecha de Pago");
                Sheet.Cells["B3"].RichText.Add("Nombre");
                Sheet.Cells["C3"].RichText.Add("Descripción");
                Sheet.Cells["D3"].RichText.Add("Clave Divisa");
                Sheet.Cells["E3"].RichText.Add("TC");
                Sheet.Cells["F3"].RichText.Add("No. Operación");
                Sheet.Cells["G3"].RichText.Add("Monto");
                Sheet.Cells["H3"].RichText.Add("RFC Banco");
                Sheet.Cells["I3"].RichText.Add("Cuenta Emisor");
                Sheet.Cells["J3"].RichText.Add("Estado");

                row = 4;
                foreach (var item in pagoFactura)
                {
                    string es = "";
                    if (item.Estado == true)
                    {
                        es = "Activo";
                    }
                    else
                    {
                        es = "Cancelado";
                    }
                    string FPago = item.FechaPago.Year.ToString() + "-" + item.FechaPago.Month.ToString() + "-" + item.FechaPago.Day.ToString();

                    Sheet.Cells[string.Format("A{0}", row)].Value = FPago;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveDivisa;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.NoOperacion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Monto;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.RFCBancoEmisor;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.CuentaEmisor;
                    Sheet.Cells[string.Format("J{0}", row)].Value = es;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturasPagadas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();

            }
            return View(ViewBag.pagoFactura);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult CreaReporteporMonedaClie()
        {
            //Buscar 
            c_MonedaContext dbc = new c_MonedaContext();
            var mone = dbc.c_Monedas.OrderBy(m => m.Descripcion).ToList();
            List<SelectListItem> listaMonedas = new List<SelectListItem>();
            listaMonedas.Add(new SelectListItem { Text = "--Selecciona Moneda--", Value = "0" });

            var EstadoLst = new List<SelectListItem>();
            EstadoLst.Add(new SelectListItem { Text = "Todos", Value = "T" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });

            //ViewData["Estado"] = EstadoLst;
            ViewBag.Status = new SelectList(EstadoLst, "Value", "Text");

            foreach (var m in mone)
            {
                listaMonedas.Add(new SelectListItem { Text = m.Descripcion, Value = m.ClaveMoneda.ToString() });
            }
            ViewBag.ClaveDivisa = listaMonedas;
            return View();
        }
        [HttpPost]
        public ActionResult CreaReporteporMonedaClie(ReportefenoClie A, FormCollection coleccion)
        {
            DateTime fi = A.Fechainicio;
            DateTime ff = A.Fechafinal;
            string esta = A.Status;
            string nota = A.Nota;
            string moneda = A.ClaveDivisa;

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {
                try
                {
                    ClientesContext dbc = new ClientesContext();
                    Clientes cliente = dbc.Clientes.Find(A.IDCliente);
                }
                catch (Exception er)
                {
                }
                ReporteClientesMoneda report = new ReporteClientesMoneda();

                byte[] abytes = report.PrepareReport(fi, ff, moneda, esta, nota);
                return File(abytes, "application/pdf");
            }
            else
            {
                List<ReportefenoClie> pagoFactura = new List<ReportefenoClie>();

                string FI = A.Fechainicio.Year.ToString() + "-" + A.Fechainicio.Month.ToString() + "-" + A.Fechainicio.Day.ToString();
                string FF = A.Fechafinal.Year.ToString() + "-" + A.Fechafinal.Month.ToString() + "-" + A.Fechafinal.Day.ToString();

                if (esta == "T")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59'";
                    if (moneda != "")
                    {
                        cadena = cadena + "and ClaveDivisa='" + moneda + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                if (esta == "A")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59' and Estado = 1";
                    if (moneda != "")
                    {
                        cadena = cadena + "and ClaveDivisa='" + moneda + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                if (esta == "C")
                {
                    string cadena = "select IDPagoFactura, FechaPago, IDCliente, Nombre, ClaveFormaPago, Descripcion, ClaveDivisa, TC, NoOperacion, Monto, RFCBancoEmisor,NombreBancoEmisor CuentaEmisor, RFCBancoReceptor, CuentaReceptor,  Clave as ClaveCadenaPago, TipoCadenaPago, CertificadoPago, IDTipoCadenaPago, SelloPago, Observacion, Estado from [dbo].[VPagoFacturaC]";
                    cadena = cadena + "where FechaPago>='" + FI + "' and FechaPago <='" + FF + " 23:59:59' and Estado = 0";
                    if (moneda != "")
                    {
                        cadena = cadena + "and ClaveDivisa='" + moneda + "'";
                    }
                    pagoFactura = db.Database.SqlQuery<ReportefenoClie>(cadena).ToList();
                }
                ViewBag.pagoFactura = pagoFactura;

                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("Clientes");

                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Reporte por Moneda");
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 2;
                Sheet.Cells["A2:J2"].Style.Font.Size = 12;
                Sheet.Cells["A2:J2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:J2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos

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
                Sheet.Cells["A3:J3"].Style.Font.Bold = true;
                Sheet.Cells["A3:J3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Fecha de Pago");
                Sheet.Cells["B3"].RichText.Add("Nombre");
                Sheet.Cells["C3"].RichText.Add("Descripción");
                Sheet.Cells["D3"].RichText.Add("Clave Divisa");
                Sheet.Cells["E3"].RichText.Add("TC");
                Sheet.Cells["F3"].RichText.Add("No. Operación");
                Sheet.Cells["G3"].RichText.Add("Monto");
                Sheet.Cells["H3"].RichText.Add("RFC Banco");
                Sheet.Cells["I3"].RichText.Add("Cuenta Emisor");
                Sheet.Cells["J3"].RichText.Add("Estado");

                row = 4;
                foreach (var item in pagoFactura)
                {
                    string es = "";
                    if (item.Estado == true)
                    {
                        es = "Activo";
                    }
                    else
                    {
                        es = "Cancelado";
                    }
                    string FPago = item.FechaPago.Year.ToString() + "-" + item.FechaPago.Month.ToString() + "-" + item.FechaPago.Day.ToString();

                    Sheet.Cells[string.Format("A{0}", row)].Value = FPago;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveDivisa;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.NoOperacion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Monto;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.RFCBancoEmisor;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.CuentaEmisor;
                    Sheet.Cells[string.Format("J{0}", row)].Value = es;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturasPagadas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();

            }
            return View(ViewBag.pagoFactura);
        }

        //////////////////
        ///
        public ActionResult PeriodoFechasPagoF()
        {
            PeriodoFechasPagoF elemento = new PeriodoFechasPagoF();
            return View(elemento);
        }

        [HttpPost]
        public ActionResult PeriodoFechasPagoF(PeriodoFechasPagoF elemento, FormCollection coleccion)
        {

            string CadenaSQL = string.Empty;
            string ConsultaSql = "select * from VPagoFacMesS_NC_A ";
            string Filtro = string.Empty;
            string Orden = " order by nombre_cliente ";

            string FI = elemento.fechaInicial.Year.ToString() + "-" + elemento.fechaInicial.Month.ToString() + "-" + elemento.fechaInicial.Day.ToString();
            string FF = elemento.fechaFinal.Year.ToString() + "-" + elemento.fechaFinal.Month.ToString() + "-" + elemento.fechaFinal.Day.ToString();


            Filtro = " where  FechaPago BETWEEN '" + FI + "' and '" + FF + " 23:59:59.999' ";


            //Listado de datos
            CadenaSQL = ConsultaSql + " " + Filtro + " " + Orden;
            VPagoFacMesS_NC_AContext db = new VPagoFacMesS_NC_AContext();
            List<VPagoFacMesS_NC_A> pagoFactura = db.Database.SqlQuery<VPagoFacMesS_NC_A>(CadenaSQL).ToList();
            ViewBag.pagofact = pagoFactura;

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {
                ReporteListadoFacPag report = new ReporteListadoFacPag();
                //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"),DateTime.Parse( "2019-07-30"));
                byte[] abytes = report.PrepareReport(elemento.fechaInicial, elemento.fechaFinal);
                return File(abytes, "application/pdf", "ReportePagos.pdf");
            }
            else
            {


                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("FacturasPagadas");

                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:N1"].Style.Font.Size = 20;
                Sheet.Cells["A1:N1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:N1"].Style.Font.Bold = true;
                Sheet.Cells["A1:N1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Facturas pagada Sin Notas de Crédito ni Anticipo");
                Sheet.Cells["A1:N1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 2;
                Sheet.Cells["A2:N2"].Style.Font.Size = 12;
                Sheet.Cells["A2:N2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:N2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:N2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos

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
                Sheet.Cells["A3:N3"].Style.Font.Bold = true;
                Sheet.Cells["A3:N3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:N3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID");
                Sheet.Cells["B3"].RichText.Add("Fecha de Pago");
                Sheet.Cells["C3"].RichText.Add("Importe Pagado");
                Sheet.Cells["D3"].RichText.Add("Moneda de Pago");
                Sheet.Cells["E3"].RichText.Add("Serie");
                Sheet.Cells["F3"].RichText.Add("Numero");
                Sheet.Cells["G3"].RichText.Add("Fecha Factura");
                Sheet.Cells["H3"].RichText.Add("Cliente");
                Sheet.Cells["I3"].RichText.Add("Subtotal");
                Sheet.Cells["J3"].RichText.Add("IVA");
                Sheet.Cells["K3"].RichText.Add("total");
                Sheet.Cells["L3"].RichText.Add("Moneda");
                Sheet.Cells["M3"].RichText.Add("Importe Saldo Insoluto");
                Sheet.Cells["N3"].RichText.Add("Vendedor");

                row = 4;
                foreach (var item in ViewBag.pagofact)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.fechapago;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.importepagado;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Monedapago;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.serie;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.numero;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.fechaFactura;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.nombre_cliente;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.subtotal;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.iva;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.total;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.moneda;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Importesaldoinsoluto;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.Vendedor;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturasPagadas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }

            return View(ViewBag.pagofact);
        }
       
    }
}


