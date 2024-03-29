﻿using SIAAPI.clasescfdi;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PagedList;
using static SIAAPI.Models.Administracion.c_Banco;

using SIAAPI.ViewModels.Cfdi;
using SIAAPI.Models.Login;
using System.Drawing;
using System.Xml;
using static SIAAPI.Models.Comercial.ClienteRepository;
using SIAAPI.Reportes;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Globalization;

namespace SIAAPI.Controllers.Cfdi
{
    [Authorize(Roles = "Almacenista,Administrador,Compras,Sistemas,Facturacion, Proveedor")]
    public class PagoFacturaProveedorController : Controller
    {


        //[Authorize(Roles = "Administrador,Facturacion,Gerencia,Sistemas")]
        // GET: PagoProv
        private PagoFacturaProvContext db = new PagoFacturaProvContext();
        private VPagoFacturaProvElecContext dbv = new VPagoFacturaProvElecContext();
        private PagoFacturaSPEIProvContext spei = new PagoFacturaSPEIProvContext();
        private VBcoProvContext bancop = new VBcoProvContext();
        private TipoCambioContext ttc = new TipoCambioContext();
        private ProveedorContext prov = new ProveedorContext();

        /////////////////////// Index ///////////////////////
        ////////////////////////////////////////////////////
        public ActionResult Index(string Numero, string SerieP, string FolioP, string Empresa, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString, string Estado = "A")
        {

            //PagoFacturaProvContext pf = new PagoFacturaProvContext();
            //var lista = from e in pf.PagoFacturasProv
            //            orderby e.IDPagoFacturaProv
            //            select e;
            //return View(lista);

            if (Session["Proveedor"] != null)
            {
                RedirectToAction("Indexp");
            }


            ViewBag.sumatoria = "";


            ////Buscar Folio
            //var FolioLst = new List<string>();
            //var FolioQry = from d in new PagoFacturaProvContext().PagoFacturasProv
            //              orderby d.FolioP
            //              select d.FolioP;
            //FolioLst.AddRange(FolioQry.Distinct());
            //ViewBag.FolioP = new SelectList(FolioLst);

            //Buscar Proveedor
            var ProvLst = new List<string>();
            var ProvQry = from d in new PagoFacturaProvContext().PagoFacturasProv
                          orderby d.Empresa
                          select d.Empresa;
            ProvLst.AddRange(ProvQry.Distinct());
            ViewBag.Empresa = new SelectList(ProvLst);

            //Buscar Estado
            var EstadoLst = new List<SelectListItem>();
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            ViewData["Estado"] = EstadoLst;
            ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");


            string ConsultaSql = "select * from VPagoFacturaProvElec ";
            string ConsultaSqlResumen = " select ClaveMoneda as Moneda, (SUM(Monto)) as Total from dbo.VPagoFacturaProvElec";
            string ConsultaAgrupado = "group by ClaveMoneda order by ClaveMoneda ";
            string Filtro = string.Empty;
            string Orden = " order by fecha desc , SerieP , FolioP desc ";

            //Buscar por numero
            if (!String.IsNullOrEmpty(SerieP))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where SerieP=" + SerieP + "";
                }
                else
                {
                    Filtro += "and  SerieP=" + SerieP + "";
                }

            }

            //Buscar por numero
            if (!String.IsNullOrEmpty(FolioP))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where FolioP=" + FolioP + "";
                }
                else
                {
                    Filtro += "and  FolioP=" + FolioP + "";
                }

            }


            ///tabla filtro: Nombre Proveedor
            if (!String.IsNullOrEmpty(Numero))
            {

                if (Filtro == string.Empty)
                {
                    Filtro = "where IDPagoFacturaProv='" + Numero + "'";
                }
                else
                {
                    Filtro += "and  IDPagoFacturaProv='" + Numero + "'";
                }

            }


            ///tabla filtro: Empresa
            if (!String.IsNullOrEmpty(Empresa))
            {

                if (Filtro == string.Empty)
                {
                    Filtro = "where Empresa='" + Empresa + "'";
                }
                else
                {
                    Filtro += "and  Empresa='" + Empresa + "'";
                }

            }

            if (Estado != "Todos")
            {
                if (Estado == "C")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where Estado='C'";
                    }
                    else
                    {
                        Filtro += "and  Estado='C'";
                    }
                }
                if (Estado == "A")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where  Estado='A'";
                    }
                    else
                    {
                        Filtro += "and Estado='A'";
                    }
                }
            }


            if (!String.IsNullOrEmpty(searchString)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where  FechaPago BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and FechaPago BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999'";
                }
            }

            ViewBag.CurrentSort = sortOrder;

            ViewBag.FolioSortParm = String.IsNullOrEmpty(sortOrder) ? "FolioP" : "";
            ViewBag.FechaSortParm = sortOrder == "FechaPago" ? "FechaPago" : "";
            ViewBag.EmpresaSortParm = String.IsNullOrEmpty(sortOrder) ? "Empresa" : "";
            ViewBag.EstadoSortParm = String.IsNullOrEmpty(sortOrder) ? "Estado" : "";


            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.SearchString = searchString;
            // ViewBag.CurrentFilter = searchString;

            //Paginación
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            //Ordenacion

            switch (sortOrder)
            {
                case "SerieP":
                    Orden = " order by  SerieP , FolioP desc ";
                    break;
                case "FolioP":
                    Orden = " order by  FolioP desc ";
                    break;
                case "Numero":
                    Orden = " order by IDPagoFacturaProv asc ";
                    break;
                case "Fecha":
                    Orden = " order by FechaPago ";
                    break;
                case "Empresa":
                    Orden = " order by  Empresa ";
                    break;
                case "Estado":
                    Orden = " order by  Estado ";
                    break;
                default:
                    Orden = " order by FechaPago desc , SerieP , FolioP desc ";
                    break;
            }

            //var elementos = from s in db.encfacturas
            //select s;
            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

            var elementos = dbv.Database.SqlQuery<VPagoFacturaProvElec>(cadenaSQl).ToList();



            ViewBag.sumatoria = "";
            try
            {

                var SumaLst = new List<string>();
                var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
                List<ResumenFacP> data = Db.Database.SqlQuery<ResumenFacP>(SumaQry).ToList();
                ViewBag.sumatoria = data;

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = Db.PagoFacturasProv.OrderBy(e => e.SerieP).Count();// Total number of elements

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

            return View(elementos.ToPagedList(pageNumber, pageSize));

        }


        /////////////////////// Index Por proveedor ///////////////////////
        ////////////////////////////////////////////////////
        public ActionResult IndexP(string Numero, string SerieP, string FolioP, string Empresa, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString, string Estado = "A")
        {

            int p = 0;
            try
            {
                SIAAPI.Models.Comercial.ClsDatoEntero c = Db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].[Proveedores] where [RFC] ='" + User.Identity.Name + "'").ToList()[0];
                p = c.Dato;
            }
            catch (Exception err)
            {

            }

            Session["Proveedor"] = p;
            List<Proveedor> proveedor = Db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where [IDProveedor]=" + p + "").ToList();
            ViewBag.Proveedor = proveedor;


            ViewBag.sumatoria = "";


            //Buscar Estado
            var EstadoLst = new List<SelectListItem>();
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            ViewData["Estado"] = EstadoLst;
            ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");
            ViewBag.Estadoseleccionado = Estado;

            string ConsultaSql = "select * from [dbo].[VPagoFacturaProvT] ";
            string ConsultaSqlResumen = " select ClaveMoneda as Moneda, (SUM(Monto)) as Total from dbo.VPagoFacturaProvElec";
            string ConsultaAgrupado = "group by ClaveMoneda order by ClaveMoneda ";
            string Filtro = string.Empty;
            string Orden = " order by fecha desc , SerieP , FolioP desc ";

            //Buscar por numero
            if (!String.IsNullOrEmpty(SerieP))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where SerieP=" + SerieP + "";
                }
                else
                {
                    Filtro += "and  SerieP=" + SerieP + "";
                }

            }

            //Buscar por numero
            if (!String.IsNullOrEmpty(FolioP))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where FolioP=" + FolioP + "";
                }
                else
                {
                    Filtro += "and  FolioP=" + FolioP + "";
                }

            }



            if (!String.IsNullOrEmpty(searchString)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where  FechaPago BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and FechaPago BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999'";
                }
            }

            ViewBag.CurrentSort = sortOrder;

            ViewBag.FolioSortParm = String.IsNullOrEmpty(sortOrder) ? "FolioP" : "";
            ViewBag.FechaSortParm = sortOrder == "FechaPago" ? "FechaPago" : "";
            ViewBag.EmpresaSortParm = String.IsNullOrEmpty(sortOrder) ? "Empresa" : "";
            ViewBag.EstadoSortParm = String.IsNullOrEmpty(sortOrder) ? "Estado" : "";


            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.SearchString = searchString;
            // ViewBag.CurrentFilter = searchString;

            //Paginación
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            //Ordenacion

            switch (sortOrder)
            {
                case "SerieP":
                    Orden = " order by  SerieP , FolioP desc ";
                    break;
                case "FolioP":
                    Orden = " order by  FolioP desc ";
                    break;
                case "Numero":
                    Orden = " order by IDPagoFacturaProv asc ";
                    break;
                case "Fecha":
                    Orden = " order by FechaPago ";
                    break;
                case "Empresa":
                    Orden = " order by  Empresa ";
                    break;
                case "Estado":
                    Orden = " order by  Estado ";
                    break;
                default:
                    Orden = " order by FechaPago desc , SerieP , FolioP desc ";
                    break;
            }


            if (Filtro == string.Empty)
            {
                Filtro = Filtro + " where IDProveedor = " + p + " and Estado='A'";
            }
            else
            {
                Filtro = Filtro + " and IDProveedor = " + p + " and Estado='A'";
            }

            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

            var elementos = dbv.Database.SqlQuery<VPagoFacturaProvT>(cadenaSQl).ToList();



            ViewBag.sumatoria = "";
            try
            {

                var SumaLst = new List<string>();
                var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
                List<ResumenFacP> data = Db.Database.SqlQuery<ResumenFacP>(SumaQry).ToList();
                ViewBag.sumatoria = data;

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = Db.PagoFacturasProv.OrderBy(e => e.SerieP).Count();// Total number of elements

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

            return View(elementos.ToPagedList(pageNumber, pageSize));

        }


        /////////////////////////////////////////////LISTA PAGO FACTURA//////////////////////////////////////
        public ActionResult IndexTab()
        {
            ViewData["VPagoFacturaProv"] = GetPagoElectronico();
            ViewData["VPagoFacturaProvEfe"] = GetPagoEfectivo();
            return View();

            //Paginación
        }//Index()

        /////////////////////// Boton Cargar Pagos ///////////////////////
        /////////////////////////////////////////////////////////////////
        public ActionResult CargarPagos()
        {
            return View();
        }

        [HttpPost]

        public ActionResult CargarPagos(FormCollection collection)
        {
            try
            {
                HttpPostedFileBase archivo = Request.Files["Imag1"];
                //archivo.SaveAs(Path.Combine(directory, Path.GetFileName(archivo.FileName)));
                // Generador.CreaPDF(Path.Combine(@".\facturas", Path.GetFileName(archivo.FileName)));
                StreamReader reader = new StreamReader(archivo.InputStream);
                String contenidoxml = reader.ReadToEnd();

                ClsXmlPagos pagxml = new ClsXmlPagos(contenidoxml);

                int IDProveedor = 0;
                string mensajealusuario = string.Empty;

                try
                {
                    //EncfacturaProvContext dbp = new EncfacturaProvContext();
                    //EncfacturaProv proveedorenlabase = dbp.Database.SqlQuery<EncfacturaProv>("select * from EncfacturaProv where UUID='" + pagxml.Comprobante.Complemento.TimbreFiscalDigital.UUID + "'").ToList()[0];
                    PagoFacturaProvContext dbp = new PagoFacturaProvContext();
                    PagoFacturaProv proveedorenlabase = dbp.Database.SqlQuery<PagoFacturaProv>("select * from PagoFacturaProv where UUID='" + pagxml.Comprobante.Complemento.TimbreFiscalDigital.UUID + "'").ToList()[0];

                    int IDpbase = proveedorenlabase.IDProveedor; /// si la consulta no devolvio fila lanazara una excepcion 
                    mensajealusuario = "EL PAGO YA SE ENCUENTRA EN EL SISTEMA ";
                    ViewBag.Mensajeerror = mensajealusuario;
                    return View();
                }
                catch (Exception err)
                {

                    string error = err.Message;

                }

                try
                {

                    //EncfacturaProvContext dbp = new EncfacturaProvContext();
                    foreach (var pago in pagxml.Comprobante.Complemento.Pagos)
                    {
                        foreach (var Docre in pago.documentoRelacionado)
                        {
                            EncfacturaProvContext dbp = new EncfacturaProvContext();
                            EncfacturaProv proveedorenlabase = dbp.Database.SqlQuery<EncfacturaProv>("select * from EncfacturaProv where UUID='" + Docre.IdDocumento + "'").ToList()[0];
                            try
                            {
                                int nfac = proveedorenlabase.ID; /// si la consulta no devolvio fila lanazara una excepcion 

                            }
                            catch (Exception err)
                            {
                                string mensajederror = err.Message;
                                mensajealusuario = "EL DOCUMENTO RELACIONADO  CON UUID" + Docre.IdDocumento + " NO EXISTE";
                                ViewBag.Mensajeerror = mensajealusuario;
                                return View();
                            }
                        }
                    }
                }
                catch (Exception errgeneral)
                {
                    string me = errgeneral.Message;
                }






                try
                {
                    ProveedorContext dbp = new ProveedorContext();
                    Proveedor proveedorenlabase = dbp.Database.SqlQuery<Proveedor>("select * from proveedores where Empresa='" + pagxml.Comprobante.Emisor.Nombre + "'").ToList()[0];
                    int IDpbase = proveedorenlabase.IDProveedor; /// si la consulta no devolvio fila lanazara una excepcion 
                    IDProveedor = IDpbase;
                }
                catch (Exception err)
                {
                    string error = err.Message;
                    mensajealusuario = "EL PROVEEDOR NO SE ENCUENTRA REGISTRADO O SU NOMBRE NO COINCIDE CON EXACTITUD A TU REGISTRO VERIFICA PUNTOS, COMAS Y ESPACIOS ";
                    //ViewBag.Mensajeerror = mensajealusuario;
                    // return View();

                    IDProveedor = 0;



                }


                foreach (var pagoitem in pagxml.Comprobante.Complemento.Pagos)
                {
                    // aqui llenas un pago

                    string seriep = pagxml.Comprobante.Serie;
                    int foliop = int.Parse(pagxml.Comprobante.Folio.ToString());
                    string proveedor = pagxml.Comprobante.Emisor.Nombre;
                    string fechap = pagxml.Comprobante.Fecha.ToString();
                    string formap = pagoitem.FormaDePagoP;
                    string monedap = pagoitem.MonedaP.ToString();
                    //decimal tc = ;
                    //decimal totalp = Decimal.Parse(pagoitem.Monto.ToString());
                    string nooperacion = pagoitem.NumOperacion;
                    decimal monto = pagoitem.Monto;

                    string rfcBancoe = pagoitem.RfcEmisorCtaOrd;
                    string cuentae = pagoitem.CtaOrdenante;
                    string rfcBancor = pagoitem.RfcEmisorCtaBen;
                    string cuentar = pagoitem.CtaBeneficiario;
                    string tipocadp = pagoitem.TipoCadPago;

                    string uuid = pagxml.Comprobante.Complemento.TimbreFiscalDigital.UUID;

                    PagoFacturaProvContext pf = new PagoFacturaProvContext();

                    PagoFacturaProv pagof = new PagoFacturaProv();

                    pagof.SerieP = seriep;
                    pagof.FolioP = foliop;
                    pagof.IDProveedor = IDProveedor;
                    pagof.Empresa = proveedor;
                    pagof.FechaPago = DateTime.Parse(fechap);
                    pagof.ClaveFormaPago = formap;
                    pagof.ClaveMoneda = monedap;
                    try
                    {
                        pagof.NoOperacion = long.Parse(nooperacion);
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                        pagof.NoOperacion = 0;
                    }
                    pagof.Monto = monto;
                    pagof.RFCBancoEmpresa = rfcBancoe;
                    pagof.CuentaEmpresa = cuentae;
                    pagof.RFCBancoProv = rfcBancor;
                    pagof.CuentaProv = cuentar;
                    pagof.IDTipoCadenaPago = tipocadp;
                    pagof.Estado = "A";
                    pagof.UUID = uuid;
                    pagof.RutaXML = contenidoxml;
                    pagof.RutaPDF = "";
                    pagof.Observacion = "";
                    pagof.EstadoP = true;
                    pagof.TC = 1;

              

                    pf.PagoFacturasProv.Add(pagof);

                    pf.SaveChanges();

                    ClsDatoEntero idpagof = pf.Database.SqlQuery<ClsDatoEntero>("select IDPagoFacturaProv as Dato from dbo.PagoFacturaProv where Empresa='" + proveedor + "' and  SerieP='" + seriep + "' and FolioP =" + foliop + "  ").ToList()[0];

                    if (pagoitem.TipoCadPago == "01")
                    {
                        // pago spei
                        string certificadop = pagoitem.CertPago;
                        string idtipoCadenap = pagoitem.CadenaPago;
                        string sellop = pagoitem.SelloPago;
                        PagoFacturaSPEIProvContext pfs = new PagoFacturaSPEIProvContext();
                        string comando1 = "insert into [dbo].[PagoFacturaSPEIProv](IDPagoFacturaProv, CertificadoPago, IDTipoCadenaPago, SelloPago) values(" + idpagof.Dato + ", " + foliop + ", " + certificadop + ", '" + idtipoCadenap + "',  '" + sellop + "')";
                        pfs.Database.ExecuteSqlCommand(comando1);

                    }

                    foreach (DocumentoRelacionadoPago pagdocumento in pagoitem.documentoRelacionado)
                    {

                        // aqui llenas sus documentos relacionados
                        string iddocto = pagdocumento.IdDocumento;
                        int nofactura = 0;
                        try
                        {
                            nofactura = int.Parse(pagdocumento.Folio.ToString());
                        }
                        catch (Exception errdr)
                        {
                            string mensajederror = errdr.Message;
                            nofactura = 0;
                        }
                        int parcialidad = int.Parse(pagdocumento.NumParcialidad.ToString());
                        decimal importep = Decimal.Parse(pagdocumento.ImpPagado.ToString());
                        decimal importesalant = Decimal.Parse(pagdocumento.ImpSaldoAnt.ToString());
                        decimal importepag = Decimal.Parse(pagdocumento.ImpPagado.ToString());
                        decimal importesalins = Decimal.Parse(pagdocumento.ImpSaldoInsoluto.ToString());
                        string monedapa = pagdocumento.MonedaDR;
                        string metodopa = pagdocumento.MetodoDePagoDR;
                        string formapa = pagoitem.FormaDePagoP;

                        try
                        {
                            DocumentoRelacionadoProvContext drp = new DocumentoRelacionadoProvContext();
                            // Saldos Factura Proveedor
                            EncfacturaProvContext ef = new EncfacturaProvContext();
                            ClsDatoDecimal totalf = ef.Database.SqlQuery<ClsDatoDecimal>("select Total as Dato from dbo.EncFacturaProv where uuid = '" + iddocto + "'").ToList()[0];
                            ClsDatoEntero idf = ef.Database.SqlQuery<ClsDatoEntero>("select ID as Dato from dbo.EncFacturaProv where uuid = '" + iddocto + "'").ToList()[0];

                            ///
                            string comando2 = "insert into [dbo].[DocumentoRelacionadoProv](IDPagoFacturaProv, IDProveedor, Empresa,IDFacturaProv, Numero, ClaveMoneda, ClaveFormaPago, ClaveMetododepago, ImporteSaldoInsoluto, importepagado, NoParcialidad, UUID) values(" + idpagof.Dato + ", " + IDProveedor + ",  '" + proveedor + "', "+ idf.Dato + ", " + nofactura + ", '" + monedapa + "', '" + formapa + "',  '" + metodopa + "',  " + importesalins + ",  " + importepag + ", " + parcialidad + ",  '" + iddocto + "')";
                            drp.Database.ExecuteSqlCommand(comando2);

                            



                            SaldoFacturaProvContext sfp = new SaldoFacturaProvContext();
                            try
                            {
                                SaldoFacturaProv pagobd = sfp.Database.SqlQuery<SaldoFacturaProv>("select * from SaldoFacturaProv where IDFacturaProv='" + idf.Dato + "'").ToList()[0];
                                /// si la consulta no devolvio fila lanazara una excepcion 


                                int idpagofactura = pagobd.IDFacturaProv;
                            }
                            catch (Exception err)
                            {
                                string mensajederror = err.Message;
                                string comando3 = "insert into [dbo].[SaldoFacturaProv](IDFacturaProv, IDProveedor, Numero, Total, ImporteSaldoAnterior, importepagado,  ImporteSaldoInsoluto, empresa) values(" + idf.Dato + ", " + IDProveedor + ", " + nofactura + ",  " + totalf.Dato + ", '" + importesalant + "',  " + importepag + ",  " + importesalins + ", '" + proveedor + "')";
                                sfp.Database.ExecuteSqlCommand(comando3);
                            }

                            string comando4 = "update [dbo].[SaldoFacturaProv] set ImporteSaldoAnterior =" + importesalant + ", importepagado =  " + importepag + ", ImporteSaldoInsoluto= " + importesalins + " where IDFacturaProv = " + idf.Dato + " ";
                            sfp.Database.ExecuteSqlCommand(comando4);

                            sfp.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set conpagos='1' where UUID = '" + iddocto + "'");
                            ClsDatoEntero IDFacProv = pf.Database.SqlQuery<ClsDatoEntero>("select IDFacturaProv as Dato from dbo.EncFacturaProv  where UUID = '" + iddocto + "' ").ToList()[0];
                            if (importesalins == 0)
                            {
                                sfp.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set pagada='1' where UUID = '" + iddocto + "'");
                            }



                        }
                        catch (Exception err)
                        {
                            if (err.Message.Contains("index"))
                            {
                                ViewBag.Mensajeerror = "No encontre la factura No " + nofactura;
                                return View();
                            }
                        }

                    }

                   
                }
               

                try
                {

                }
                catch (Exception ERR2)
                {
                    string mensajederror = ERR2.Message;
                    ViewBag.Mensajeerror = "Hay un problema al aplicar pagos";
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }

            catch (Exception ERR2)
            {
                string mensajederror = ERR2.Message;
                ViewBag.Mensajeerror = "Este archivo Xml no contiene una cadena de pago valido";
                return View();
            }

        }

       

    /////
    /////////////////////// Boton Cargar Pagos ///////////////////////
    /////////////////////////////////////////////////////////////////
    public ActionResult CargarPagosP()
        {
            SIAAPI.Models.Comercial.ClsDatoEntero c = Db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].[ContactosProv] where [Email] ='" + User.Identity.Name + "'").ToList()[0];
            int p = c.Dato;
            List<Proveedor> prov = Db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where [IDProveedor]=" + p + "").ToList();
            ViewBag.Proveedor = prov;
            
            return View();
        }

        [HttpPost]

        public ActionResult CargarPagosP(FormCollection collection)
        {
            SIAAPI.Models.Comercial.ClsDatoEntero c = Db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].[ContactosProv] where [Email] ='" + User.Identity.Name + "'").ToList()[0];
            int p = c.Dato;
            List<Proveedor> prov = Db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where [IDProveedor]=" + p + "").ToList();
            ViewBag.Proveedor = prov;
            
            try
            {
                HttpPostedFileBase archivo = Request.Files["Imag1"];
                //archivo.SaveAs(Path.Combine(directory, Path.GetFileName(archivo.FileName)));
                // Generador.CreaPDF(Path.Combine(@".\facturas", Path.GetFileName(archivo.FileName)));
                StreamReader reader = new StreamReader(archivo.InputStream);
                String contenidoxml = reader.ReadToEnd();

                ClsXmlPagos pagxml = new ClsXmlPagos(contenidoxml);

                int IDProveedor = 0;
                string mensajealusuario = string.Empty;

                try
                {
                    //EncfacturaProvContext dbp = new EncfacturaProvContext();
                    //EncfacturaProv proveedorenlabase = dbp.Database.SqlQuery<EncfacturaProv>("select * from EncfacturaProv where UUID='" + pagxml.Comprobante.Complemento.TimbreFiscalDigital.UUID + "'").ToList()[0];
                    PagoFacturaProvContext dbp = new PagoFacturaProvContext();
                    PagoFacturaProv proveedorenlabase = dbp.Database.SqlQuery<PagoFacturaProv>("select * from PagoFacturaProv where UUID='" + pagxml.Comprobante.Complemento.TimbreFiscalDigital.UUID + "'").ToList()[0];

                    int IDpbase = proveedorenlabase.IDProveedor; /// si la consulta no devolvio fila lanazara una excepcion 
                    mensajealusuario = "EL PAGO YA SE ENCUENTRA EN EL SISTEMA ";
                    ViewBag.Mensajeerror = mensajealusuario;
                    return View();
                }
                catch (Exception err)
                {

                    string error = err.Message;

                }

                try
                {

                    //EncfacturaProvContext dbp = new EncfacturaProvContext();
                    foreach (var pago in pagxml.Comprobante.Complemento.Pagos)
                    {
                        foreach (var Docre in pago.documentoRelacionado)
                        {
                            EncfacturaProvContext dbp = new EncfacturaProvContext();
                            EncfacturaProv proveedorenlabase = dbp.Database.SqlQuery<EncfacturaProv>("select * from EncfacturaProv where UUID='" + Docre.IdDocumento + "'").ToList()[0];
                            try
                            {
                                int nfac = proveedorenlabase.ID; /// si la consulta no devolvio fila lanazara una excepcion 

                            }
                            catch (Exception err)
                            {
                                string mensajederror = err.Message;
                                mensajealusuario = "EL DOCUMENTO RELACIONADO  CON UUID" + Docre.IdDocumento + " NO EXISTE";
                                ViewBag.Mensajeerror = mensajealusuario;
                                return View();
                            }
                        }
                    }
                }
                catch (Exception errgeneral)
                {
                    string me = errgeneral.Message;
                }






                try
                {
                    ProveedorContext dbp = new ProveedorContext();
                    Proveedor proveedorenlabase = dbp.Database.SqlQuery<Proveedor>("select * from proveedores where Empresa='" + pagxml.Comprobante.Emisor.Nombre + "'").ToList()[0];
                    int IDpbase = proveedorenlabase.IDProveedor; /// si la consulta no devolvio fila lanazara una excepcion 
                    IDProveedor = IDpbase;
                }
                catch (Exception err)
                {
                    string error = err.Message;
                    mensajealusuario = "EL PROVEEDOR NO SE ENCUENTRA REGISTRADO O SU NOMBRE NO COINCIDE CON EXACTITUD A TU REGISTRO VERIFICA PUNTOS, COMAS Y ESPACIOS ";
                    //ViewBag.Mensajeerror = mensajealusuario;
                    // return View();

                    IDProveedor = 0;



                }


                foreach (var pagoitem in pagxml.Comprobante.Complemento.Pagos)
                {
                    // aqui llenas un pago

                    string seriep = pagxml.Comprobante.Serie;
                    int foliop = int.Parse(pagxml.Comprobante.Folio.ToString());
                    string proveedor = pagxml.Comprobante.Emisor.Nombre;
                    string fechap = pagxml.Comprobante.Fecha.ToString();
                    string formap = pagoitem.FormaDePagoP;
                    string monedap = pagoitem.MonedaP.ToString();
                    //decimal tc = ;
                    //decimal totalp = Decimal.Parse(pagoitem.Monto.ToString());
                    string nooperacion = pagoitem.NumOperacion;
                    decimal monto = pagoitem.Monto;

                    string rfcBancoe = pagoitem.RfcEmisorCtaOrd;
                    string cuentae = pagoitem.CtaOrdenante;
                    string rfcBancor = pagoitem.RfcEmisorCtaBen;
                    string cuentar = pagoitem.CtaBeneficiario;
                    string tipocadp = pagoitem.TipoCadPago;

                    string uuid = pagxml.Comprobante.Complemento.TimbreFiscalDigital.UUID;

                    PagoFacturaProvContext pf = new PagoFacturaProvContext();

                    PagoFacturaProv pagof = new PagoFacturaProv();

                    pagof.SerieP = seriep;
                    pagof.FolioP = foliop;
                    pagof.IDProveedor = IDProveedor;
                    pagof.Empresa = proveedor;
                    pagof.FechaPago = DateTime.Parse(fechap);
                    pagof.ClaveFormaPago = formap;
                    pagof.ClaveMoneda = monedap;
                    try
                    {
                        pagof.NoOperacion = long.Parse(nooperacion);
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                        pagof.NoOperacion = 0;
                    }
                    pagof.Monto = monto;
                    pagof.RFCBancoEmpresa = rfcBancoe;
                    pagof.CuentaEmpresa = cuentae;
                    pagof.RFCBancoProv = rfcBancor;
                    pagof.CuentaProv = cuentar;
                    pagof.IDTipoCadenaPago = tipocadp;
                    pagof.Estado = "A";
                    pagof.UUID = uuid;
                    pagof.RutaXML = contenidoxml;
                    pagof.RutaPDF = "";
                    pagof.Observacion = "";
                    pagof.EstadoP = true;
                    pagof.TC = 1;



                    pf.PagoFacturasProv.Add(pagof);

                    pf.SaveChanges();

                    ClsDatoEntero idpagof = pf.Database.SqlQuery<ClsDatoEntero>("select IDPagoFacturaProv as Dato from dbo.PagoFacturaProv where Empresa='" + proveedor + "' and  SerieP='" + seriep + "' and FolioP =" + foliop + "  ").ToList()[0];

                    if (pagoitem.TipoCadPago == "01")
                    {
                        // pago spei
                        string certificadop = pagoitem.CertPago;
                        string idtipoCadenap = pagoitem.CadenaPago;
                        string sellop = pagoitem.SelloPago;
                        PagoFacturaSPEIProvContext pfs = new PagoFacturaSPEIProvContext();
                        string comando1 = "insert into [dbo].[PagoFacturaSPEIProv](IDPagoFacturaProv, CertificadoPago, IDTipoCadenaPago, SelloPago) values(" + idpagof.Dato + ", " + foliop + ", " + certificadop + ", '" + idtipoCadenap + "',  '" + sellop + "')";
                        pfs.Database.ExecuteSqlCommand(comando1);

                    }

                    foreach (DocumentoRelacionadoPago pagdocumento in pagoitem.documentoRelacionado)
                    {

                        // aqui llenas sus documentos relacionados
                        string iddocto = pagdocumento.IdDocumento;
                        int nofactura = 0;
                        try
                        {
                            nofactura = int.Parse(pagdocumento.Folio.ToString());
                        }
                        catch (Exception errdr)
                        {
                            string mensajederror = errdr.Message;
                            nofactura = 0;
                        }
                        int parcialidad = int.Parse(pagdocumento.NumParcialidad.ToString());
                        decimal importep = Decimal.Parse(pagdocumento.ImpPagado.ToString());
                        decimal importesalant = Decimal.Parse(pagdocumento.ImpSaldoAnt.ToString());
                        decimal importepag = Decimal.Parse(pagdocumento.ImpPagado.ToString());
                        decimal importesalins = Decimal.Parse(pagdocumento.ImpSaldoInsoluto.ToString());
                        string monedapa = pagdocumento.MonedaDR;
                        string metodopa = pagdocumento.MetodoDePagoDR;
                        string formapa = pagoitem.FormaDePagoP;

                        try
                        {
                            DocumentoRelacionadoProvContext drp = new DocumentoRelacionadoProvContext();
                            //string comando2 = "insert into [dbo].[DocumentoRelacionadoProv](IDPagoFacturaProv, IDProveedor, Empresa, Numero, ClaveMoneda, ClaveFormaPago, ClaveMetododepago, ImporteSaldoInsoluto, importepagado, NoParcialidad) values(" + idpagof.Dato + ", " + IDProveedor + ",  '" + proveedor + "', " + nofactura + ", '" + monedapa + "', '" + formapa + "',  '" + metodopa + "',  " + importesalins + ",  " + importepag + ", " + parcialidad + ")";
                            //drp.Database.ExecuteSqlCommand(comando2);

                            //// Saldos Factura Proveedor
                            //EncfacturaProvContext ef = new EncfacturaProvContext();
                            //ClsDatoDecimal totalf = ef.Database.SqlQuery<ClsDatoDecimal>("select Total as Dato from dbo.EncFacturaProv where uuid = '" + iddocto + "'").ToList()[0];
                            //ClsDatoEntero idf = ef.Database.SqlQuery<ClsDatoEntero>("select ID as Dato from dbo.EncFacturaProv where uuid = '" + iddocto + "'").ToList()[0];
                            
                            // Saldos Factura Proveedor
                            EncfacturaProvContext ef = new EncfacturaProvContext();
                            ClsDatoDecimal totalf = ef.Database.SqlQuery<ClsDatoDecimal>("select Total as Dato from dbo.EncFacturaProv where uuid = '" + iddocto + "'").ToList()[0];
                            ClsDatoEntero idf = ef.Database.SqlQuery<ClsDatoEntero>("select ID as Dato from dbo.EncFacturaProv where uuid = '" + iddocto + "'").ToList()[0];

                            ///
                            string comando2 = "insert into [dbo].[DocumentoRelacionadoProv](IDPagoFacturaProv, IDProveedor, Empresa,IDFacturaProv, Numero, ClaveMoneda, ClaveFormaPago, ClaveMetododepago, ImporteSaldoInsoluto, importepagado, NoParcialidad, UUID) values(" + idpagof.Dato + ", " + IDProveedor + ",  '" + proveedor + "', " + idf.Dato + ", " + nofactura + ", '" + monedapa + "', '" + formapa + "',  '" + metodopa + "',  " + importesalins + ",  " + importepag + ", " + parcialidad + ", '" + iddocto + "')";
                            drp.Database.ExecuteSqlCommand(comando2);

                            SaldoFacturaProvContext sfp = new SaldoFacturaProvContext();
                            try
                            {
                                SaldoFacturaProv pagobd = sfp.Database.SqlQuery<SaldoFacturaProv>("select * from SaldoFacturaProv where IDFacturaProv='" + idf.Dato + "'").ToList()[0];
                                /// si la consulta no devolvio fila lanazara una excepcion 


                                int idpagofactura = pagobd.IDFacturaProv;
                            }
                            catch (Exception err)
                            {
                                string mensajederror = err.Message;
                                string comando3 = "insert into [dbo].[SaldoFacturaProv](IDFacturaProv, IDProveedor, Numero, Total, ImporteSaldoAnterior, importepagado,  ImporteSaldoInsoluto, empresa) values(" + idf.Dato + ", " + IDProveedor + ", " + nofactura + ",  " + totalf.Dato + ", '" + importesalant + "',  " + importepag + ",  " + importesalins + ", '" + proveedor + "')";
                                sfp.Database.ExecuteSqlCommand(comando3);
                            }

                            string comando4 = "update [dbo].[SaldoFacturaProv] set ImporteSaldoAnterior =" + importesalant + ", importepagado =  " + importepag + ", ImporteSaldoInsoluto= " + importesalins + " where IDFacturaProv = " + idf.Dato + " ";
                            sfp.Database.ExecuteSqlCommand(comando4);

                            sfp.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set conpagos='1' where UUID = '" + iddocto + "'");
                            if (importesalins == 0)
                            {
                                sfp.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set pagada='1' where UUID = '" + iddocto + "'");
                            }



                        }
                        catch (Exception err)
                        {
                            string mensajederror = err.Message;
                            if (err.Message.Contains("indexP"))
                            {
                                ViewBag.Mensajeerror = "No encontre la factura No " + nofactura;
                                return View();
                            }
                        }

                    }


                }


                try
                {

                }
                catch (Exception ERR2)
                {
                    string mensajederror = ERR2.Message;
                    ViewBag.Mensajeerror = "Hay un problema al aplicar pagos";
                    return RedirectToAction("IndexP");
                }

                return RedirectToAction("IndexP");
            }

            catch (Exception ERR2)
            {
                string mensajederror = ERR2.Message;
                ViewBag.Mensajeerror = "Este archivo Xml no contiene una cadena de pago valido";
                return View();
            }

        }

        //////




        /////////////////////// DEscargar XML ///////////////////////
        /////////////////////////////////////////////////////////////////
        public FileResult Descargarxml(int id)
        {
            // Obtener contenido del archivo

            PagoFacturaProvContext pf = new PagoFacturaProvContext();
            var elemento = pf.PagoFacturasProv.Single(m => m.IDPagoFacturaProv == id);
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaXML));

            return File(stream, "text/plain", "PagoFactura" + elemento.SerieP + elemento.FolioP + ".xml");
        }


        /////////////////////// Boton Subir PDF ///////////////////////
        /////////////////////////////////////////////////////////////////
        public ActionResult SubirArchivo(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult SubirArchivo(HttpPostedFileBase file, int id)
        {
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            if (file != null)
            {
                string ruta = Server.MapPath("~/PDFProveedor/");
                ruta += file.FileName;
                new VPagoFacturaEfeProvContext().Database.ExecuteSqlCommand("update [dbo].[PagoFacturaProv] set RutaPDF='" + ruta + "' where [IDPagoFacturaProv]=" + id);
                modelo.SubirArchivo(ruta, file);
                ViewBag.Error = modelo.error;
                ViewBag.Correcto = modelo.Confirmacion;

            }
            return View();
        }

        public ActionResult DescargarPDFP(int id)
        {
            // Obtener contenido del archivo
            PagoFacturaProvContext dbp = new PagoFacturaProvContext();
            PagoFacturaProv elemento = dbp.PagoFacturasProv.Find(id);

        //EmpresaContext dbe = new EmpresaContext();

        //var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);

        //    System.Drawing.Image logoempresa = byteArrayToImage(new byte[0]);
        ////Image logoempresa = byteArrayToImage(empresa.Logo);

        //PDFPagoP pago = new PDFPagoP(elemento.RutaXML, logoempresa, id, "");

        //RedirectToAction("Index");
        string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            return new FilePathResult(elemento.RutaPDF, contentType);
    }


    ///////////////////////Validar los proveedores que ya fueron registrados ///////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////

    public ActionResult ValidarProv()
        {

            //Valida proveedor en Pago Factura  
            PagoFacturaProvContext pprov = new PagoFacturaProvContext();
            string querySQL1 = "update dbo.PagoFacturaProv  set IDProveedor = t2.IDProveedor from dbo.PagoFacturaProv t1, dbo.Proveedores as t2  where t1.Empresa = t2.Empresa";
            pprov.Database.ExecuteSqlCommand(querySQL1);

            //Valida proveedor en Enc Factura 
            EncfacturaProvContext encf = new EncfacturaProvContext();
            string querySQL2 = "update dbo.EncFacturaProv set IDProveedor = t2.IDProveedor from dbo.EncFacturaProv t1, dbo.Proveedores as t2  where t1.Nombre_Proveedor = t2.Empresa";
            encf.Database.ExecuteSqlCommand(querySQL2);

            //Valida proveedor en Documento Relacionado 
            DocumentoRelacionadoProvContext drp = new DocumentoRelacionadoProvContext();
            string querySQL3 = "update dbo.DocumentoRelacionadoProv set IDProveedor = t2.IDProveedor from dbo.DocumentoRelacionadoProv t1, dbo.Proveedores as t2 where t1.Empresa = t2.Empresa";
            drp.Database.ExecuteSqlCommand(querySQL3);

            //Valida proveedor en Saldos Factura 
            SaldoFacturaProvContext sfp = new SaldoFacturaProvContext();
            string querySQL4 = "update dbo.SaldoFacturaProv set IDProveedor = t2.IDProveedor from  dbo.SaldoFacturaProv t1, dbo.Proveedores as t2  where t1.Empresa = t2.Empresa";
            sfp.Database.ExecuteSqlCommand(querySQL4);
            ViewBag.showSuccessAlert = true;

            //Valida si la relación tiene pagos ligados a EncFactura
            string querySQL5 = "update dbo.EncFacturaProv set conPagos = 1 from dbo.EncFacturaProv as t1, dbo.DocumentoRelacionadoProv as t2 where t1.Nombre_Proveedor = t2.Empresa and t1.Numero = t2.Numero";
            drp.Database.ExecuteSqlCommand(querySQL5);

            //Valida si la factura relacionada ya esta pagada
            string querySQL6 = "update dbo.EncFacturaProv set conPagos = 1 from dbo.EncFacturaProv as t1, dbo.SaldoFacturaProv as t2 where t1.Nombre_Proveedor = t2.Empresa and t1.Numero = t2.Numero and t2.Estado= 'A' and t2.ImporteSaldoInsoluto = 0";
            drp.Database.ExecuteSqlCommand(querySQL6);

            return RedirectToAction("Index");
        }

        /////////////////////// DEtalle del Pago ///////////////////////
        /////////////////////////////////////////////////////////////////

        public ActionResult Details(int id)
        {
            VPagoFacturaProvElecContext dbe = new VPagoFacturaProvElecContext();
            var elemento = dbe.VPagoFacturaProvElecs.Single(m => m.IDPagoFacturaProv == id);
            if (elemento == null)
            {
                return NotFound();
            }


            List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select IDPagoFacturaSPEIProv, IDPagoFacturaProv, CertificadoPago, IDTipoCadenaPago, SelloPago, Estado from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;

            VDocumentoRProvContext docto = new VDocumentoRProvContext();
            //List<VDocumentoRProv> detallesDR = docto.Database.SqlQuery<VDocumentoRProv>("select IDDocumentoRelacionadoProv, IDPagoFacturaProv, IDProveedor, Empresa, Serie, Numero, ClaveDivisa, Divisa,ClaveFormaPago, FormaPago, ClaveMetododepago, MetodoPAgo, ImportePagado,ImporteSaldoInsoluto, NoParcialidad, Estado, UUID from [dbo].[VDocumentoRProv] where IDPagoFacturaProv='" + id + "'").ToList();
            List<VDocumentoRProv> detallesDR = docto.Database.SqlQuery<VDocumentoRProv>("select * from [dbo].[VDocumentoRProv] where IDPagoFacturaProv='" + id + "'").ToList();

            ViewBag.detallesDR = detallesDR;


            return View(elemento);
        }

        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, PagoFacturaProv collection)
        {
            VPagoFacturaProvElecContext dbe = new VPagoFacturaProvElecContext();
            var elemento = dbe.VPagoFacturaProvElecs.Single(m => m.IDPagoFacturaProv == id);
            if (elemento == null)
            {
                return NotFound();
            }

            PagoFacturaSPEIProvContext spei = new PagoFacturaSPEIProvContext();
            List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select IDPagoFacturaSPEI, IDPagoFactura, CertificadoPago, IDTipoCadenaPago, SelloPago, StatusPago from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;

            VDocumentoRProvContext docto = new VDocumentoRProvContext();
            List<VDocumentoRProv> detallesDR = docto.Database.SqlQuery<VDocumentoRProv>("select IDDocumentoRelacionadoProv, IDPagoFacturaProv, IDProveedor, Empresa, Serie, Numero, ClaveDivisa, Divisa,ClaveFormaPago, FormaPago, ClaveMetododepago, MetodoPAgo, ImportePagado,ImporteSaldoInsoluto, NoParcialidad, Estado, UUID from [dbo].[VDocumentoRProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.detallesDR = detallesDR;



            return View(elemento);

        }


        /////////////////////// DEtalle del Pago por proveedor ///////////////////////
        /////////////////////////////////////////////////////////////////

        public ActionResult DetailsP(int id)
        {
            List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select IDPagoFacturaSPEIProv, IDPagoFacturaProv, CertificadoPago, IDTipoCadenaPago, SelloPago, Estado from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;

            VDocumentoRProvContext docto = new VDocumentoRProvContext();
            //List<VDocumentoRProv> detallesDR = docto.Database.SqlQuery<VDocumentoRProv>("select IDDocumentoRelacionadoProv, IDPagoFacturaProv, IDProveedor, Empresa, Serie, Numero, ClaveDivisa, Divisa,ClaveFormaPago, FormaPago, ClaveMetododepago, MetodoPAgo, ImportePagado,ImporteSaldoInsoluto, NoParcialidad, Estado, UUID from [dbo].[VDocumentoRProv] where IDPagoFacturaProv='" + id + "'").ToList();
            List<VDocumentoRProv> detallesDR = docto.Database.SqlQuery<VDocumentoRProv>("select * from [dbo].[VDocumentoRProv] where IDPagoFacturaProv='" + id + "'").ToList();

            ViewBag.detallesDR = detallesDR;


            VPagoFacturaProvTContext dbe = new VPagoFacturaProvTContext();
            var elemento = dbe.VPagoFacturaProvT.Single(m => m.IDPagoFacturaProv == id);

            return View(elemento);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailsP(int id, PagoFacturaProv collection)
        {
            //VPagoFacturaProvElecContext dbe = new VPagoFacturaProvElecContext();
            //var elemento = dbe.VPagoFacturaProvElecs.Single(m => m.IDPagoFacturaProv == id);
            VPagoFacturaProvTContext dbe = new VPagoFacturaProvTContext();
            var elemento = dbe.VPagoFacturaProvT.Single(m => m.IDPagoFacturaProv == id);
            if (elemento == null)
            {
                return NotFound();
            }

            PagoFacturaSPEIProvContext spei = new PagoFacturaSPEIProvContext();
            List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select IDPagoFacturaSPEI, IDPagoFactura, CertificadoPago, IDTipoCadenaPago, SelloPago, StatusPago from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;

            VDocumentoRProvContext docto = new VDocumentoRProvContext();



            return View(elemento);


        }

        /////////////////////// Cancelar Pagos y sus relaciones ///////////////////////
        //////////////////////////////////////////////////////////////////////////////
        public ActionResult CancelaPagoProv(int id, string uuid)
        {

            CancelaPagoProv pag = new CancelaPagoProv();
            List<VPagoFacturaProvElec> detallesP = dbv.Database.SqlQuery<VPagoFacturaProvElec>("select * from VPagoFacturaProvElec where IDPagoFacturaProv =" + id + " ").ToList();
            ViewBag.detallesP = detallesP;

           
                pag.IDPagoFactura = (int)id;


                return View(pag);

          
            
            //return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult CancelaPagoProv(CancelaPagoProv pag)
        {
            int id = pag.IDPagoFactura;
            string uuid = pag.UUID;

            PagoFacturaSPEIProvContext spei = new PagoFacturaSPEIProvContext();
            string querySQL2 = "update dbo.PagoFacturaSPEIProv set Estado='C' where IDPagoFacturaProv =" + id + " ";
            spei.Database.ExecuteSqlCommand(querySQL2);


            PagoFacturaProvContext pf = new PagoFacturaProvContext();
            string querySQL3 = "update dbo.PagoFacturaProv set Estado='C' where IDPagoFacturaProv =" + id + " ";
            pf.Database.ExecuteSqlCommand(querySQL3);
            PagoFacturaProv proveedorenlabase = pf.Database.SqlQuery<PagoFacturaProv>("select * from PagoFacturaProv where IDPagoFacturaProv =" + id + "").ToList()[0];
            string proveedor = proveedorenlabase.Empresa;


            DocumentoRelacionadoProvContext drp = new DocumentoRelacionadoProvContext();
            string querySQL4 = "update dbo.DocumentoRelacionadoProv set Estado='C' where IDPagoFacturaProv =" + id + " ";
            drp.Database.ExecuteSqlCommand(querySQL4);

             //Actualiza la tabla SaldoFactura con el importepagado en documento relacionado
            SaldoFacturaProvContext db1 = new SaldoFacturaProvContext();
            string consultaSQL = "select * from dbo.SaldosPr(" + id + ") ";
            List<SaldosPr> doctoS = db1.Database.SqlQuery<SaldosPr>(consultaSQL).ToList();
            foreach (var m in doctoS)
            {
                int IDFP = m.IDFacturaProv;
                db.Database.ExecuteSqlCommand("update dbo.SaldoFacturaProv set ImportePagado = (ImportePagado - " + m.ImportePagado + ") where IDFacturaProv = " + IDFP + " ");
                db.Database.ExecuteSqlCommand("update dbo.SaldoFacturaProv set ImporteSaldoInsoluto = (Total - ImportePagado), ImporteSaldoAnterior = (ImporteSaldoAnterior + ImportePagado) where IDFacturaProv = " + IDFP + " ");

                //Actualiza la tabla EncFacturas, con los valores actuales de los saldos

                db.Database.ExecuteSqlCommand("update [dbo].[EncFacturaProv] set pagada=0, conpagos=0 where ID=" + IDFP + " ");

                //List<DocumentoRelacionadoProv> doctos = drp.Database.SqlQuery<DocumentoRelacionadoProv>("select * from DocumentoRelacionadoProv where IDPagoFacturaProv =" + id + " ").ToList();
                //foreach (var docto in doctos)
                //{
                //    string prov = docto.Empresa;
                //    string num = docto.Numero;
                //    decimal imp = docto.ImportePagado;

                //    SaldoFacturaProvContext sf = new SaldoFacturaProvContext();
                //    //SaldoFacturaProv saldos = sf.Database.SqlQuery<SaldoFacturaProv>().ToList()[0];
                //    string cadena = "select * from SaldoFacturaProv where Empresa = '" + prov + "' and Numero = '" + num + "' ";
                //    List<SaldoFacturaProv> doctoS = sf.Database.SqlQuery<SaldoFacturaProv>(cadena).ToList();

                //    //if (saldos != null)
                //    //{
                //    //    decimal imppagact = 0;
                //    //    decimal imppag = saldos.ImportePagado;
                //    //    imppagact = imppag - imp;
                //    //    decimal impSalins = saldos.ImporteSaldoAnterior;
                //    //    if (imppagact < 0)
                //    //    {
                //    //        imppagact = 0;
                //    //    }
                //    //    if (impSalins < 0)
                //    //    {
                //    //        impSalins = 0;
                //    //    }
                //    //    string querySQL5 = "update dbo.SaldoFacturaProv set ImportePagado= " + imppagact + ", ImporteSaldoInsoluto= " + impSalins + " where Empresa ='" + prov + "' and Numero =" + num + " ";
                //    //    sf.Database.ExecuteSqlCommand(querySQL5);

                //    //    imppag = 0;
                //    //    imppagact = 0;
                //    //    impSalins = 0;

                //    //}
                //    //string uuid = pag.UUID;
                //    EncfacturaProvContext encf = new EncfacturaProvContext();
                //    string querySQL1 = "update dbo.EncFacturaProv set Pagada= '0' where Nombre_Proveedor = '" + prov + "' and Numero = " + num + "";
                //    encf.Database.ExecuteSqlCommand(querySQL1);

                //}
            }
            return RedirectToAction("Index");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////77
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////77
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////77


        // GetTC
        public ActionResult getTC(string id)
        {

            TipoCambioContext db = new TipoCambioContext();
            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
            List<c_Moneda> monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='" + id + "'").ToList();
            int destino = monedadestino.Select(s => s.IDMoneda).FirstOrDefault();
            decimal Cambio = 0;
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + destino + "," + origen + ") as TC").ToList()[0];
            Cambio = cambio.TC;
            ViewBag.datostc = Cambio;
            //return RedirectToAction("PagoFactura");
            return Json(Cambio, JsonRequestBehavior.AllowGet);
        }

        // GetBancoProveedor
        public JsonResult getbancoprov(int id)
        {
            VBcoProveedorContext bancop = new VBcoProveedorContext();
            c_BancoContext banco = new c_BancoContext();
            var listbanco = new List<SelectListItem>();
            try
            {
                string cadenasql = " select * from VBcoProveedor where idproveedor= " + id + " order by Nombre";

                List<VBcoProveedor> datoExiste = bancop.Database.SqlQuery<VBcoProveedor>(cadenasql).ToList();

                listbanco.Add(new SelectListItem { Text = "--Selecciona un banco--", Value = "0" });
                if (datoExiste.Count == 0)
                {

                    throw new Exception("No hay bancos registrados del proveedor");
                }
                else
                {
                    //listbanco.Add(new SelectListItem { Text = "--Selecciona un banco del proveedor--", Value = "0" });
                    //listbanco.AddRange(datoExiste, "Value", "TEXT", null);
                    foreach (VBcoProveedor x in datoExiste)
                    {
                        listbanco.Add(new SelectListItem { Text = x.ID + " | " + x.Nombre + " | " + x.Cuenta + " | " + x.ClaveMoneda, Value = x.RFC.ToString() + "," + x.Cuenta });
                    }
                }
                ViewBag.listbanco = new SelectList(listbanco);

            }
            catch (Exception er)
            {
                string mensajederror = er.Message;
                string mensaje = "No existe una cuenta registrada";
                var bancos = banco.c_Bancos.ToList();
                listbanco.Add(new SelectListItem { Text = "--Selecciona un banco--", Value = "0" });
                foreach (var x in bancos)
                {

                    listbanco.Add(new SelectListItem { Text = x.Nombre, Value = x.RFC.ToString() + "," + mensaje });
                }

            }
            return Json(new SelectList(listbanco, "Value", "Text", JsonRequestBehavior.AllowGet));

        } // Fin GetBancoProveedor

        // GetcuentaBancoProveedor
        public ActionResult getcuentabancoprov(string datos)
        {

            string[] arraydatos;

            arraydatos = datos.Split(',');
            string cuenta = arraydatos[1];
            //VBcoProveedorContext bancop = new VBcoProveedorContext();

            //List<VBcoProveedor> cuenta = db.Database.SqlQuery<VBcoProveedor>("select * from VBcoProveedor WHERE RFC='" + rfc+"' and idproveedor = "+id+" ").ToList();
            //ViewBag.datoscuenta = cuenta;

            return Json(cuenta, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<SelectListItem> GetProveedor()
        {
            PagoFacturaProv elemento = new PagoFacturaProv();
            var listaProveedor = new ProveedorAllRepository().GetProveedor();
            ViewBag.datosProvedor = listaProveedor;

            return listaProveedor;
        }



        ////////////////////////////////////////////////PAGAR FACTURA ELECTRONICA//////////////////////////////////////
        public ActionResult PagarFacturaProv()
        {

            PagoFacturaProv elemento = new PagoFacturaProv();
            ProveedorContext prov = new ProveedorContext();
            var proveedor = prov.Proveedores.OrderBy(m => m.Empresa).ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona un Proveedor--", Value = "0" });

            foreach (var p in proveedor)
            {
                li.Add(new SelectListItem { Text = p.Empresa, Value = p.IDProveedor.ToString() });
            }
            ViewBag.proveedor = li;

            var listaFormaPago = new FormaPagoRepository().GetFormasdepagoElectronica();
            ViewBag.datosFormaPago = listaFormaPago;
            //c_FormaPagoContext formap = new c_FormaPagoContext();
            //var formapago = formap.c_FormaPagos.ToList();
            //List<SelectListItem> listafp = new List<SelectListItem>();
            //listafp.Add(new SelectListItem { Text = "--Selecciona Forma Pago--", Value = "0" });

            //foreach (var f in formapago)
            //{
            //    listafp.Add(new SelectListItem { Text = f.ClaveFormaPago + " | " + f.Descripcion, Value = f.ClaveFormaPago.ToString() });
            //    ViewBag.datosFormaPago = listafp;
            //}

            c_MonedaContext moneda = new c_MonedaContext();
            var monedas = moneda.c_Monedas.ToList();
            List<SelectListItem> listamoneda = new List<SelectListItem>();
            listamoneda.Add(new SelectListItem { Text = "--Selecciona Moneda--", Value = "0" });

            foreach (var m in monedas)
            {
                listamoneda.Add(new SelectListItem { Text = m.ClaveMoneda + " | " + m.Descripcion, Value = m.ClaveMoneda.ToString() });
                ViewBag.datosMoneda = listamoneda;

            }

            elemento.FechaPago = DateTime.Now;

            List<c_Moneda> monedadestino = Db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int destino = monedadestino.Select(s => s.IDMoneda).FirstOrDefault();


            List<c_Moneda> monedaorigen = Db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='USD'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();

            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            decimal Cambio = 0;
            VCambio cambio = Db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + origen + "," + destino + ") as TC").ToList()[0];
            Cambio = cambio.TC;
            ViewBag.datostc = Cambio;
            elemento.TC = cambio.TC;


            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;

            //elemento.FechaPago = DateTime.Now;
            ViewBag.datosProveedor = new SelectList(GetProveedor());


            var listaBancoEmp = new BancoEmpresaRepository().GetBancoEmpresa();
            ViewBag.datosBancoEmp = listaBancoEmp;

            var listaTipoCadena = new c_TipoCadenaPagoRepository().GetTipoCadenaPago();
            ViewBag.datosTipoCadena = listaTipoCadena;

            c_TipoCadenaPagoContext tipo = new c_TipoCadenaPagoContext();
            var tipos = tipo.c_TipoCadenaPagos.ToList();
            List<SelectListItem> litipo = new List<SelectListItem>();
            litipo.Add(new SelectListItem { Text = "--Selecciona un tipo pago--", Value = "0" });

            foreach (var t in tipos)
            {
                litipo.Add(new SelectListItem { Text = t.Clave + " | " + t.Descripcion, Value = t.Clave.ToString() });
                ViewBag.datostipoCadena = litipo;

            }




            return View(elemento);
        }// Fin PagarFacturaProv()

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagarFacturaProv(PagoFacturaProv PagoFacturaProv)
        {

            string[] arraydatos;
            arraydatos = PagoFacturaProv.RFCBancoProv.Split(',');
            PagoFacturaProv.RFCBancoProv = arraydatos[0];
            string[] arraydatos1;
            arraydatos1 = PagoFacturaProv.RFCBancoEmpresa.Split(',');
            PagoFacturaProv.RFCBancoEmpresa = arraydatos1[0];
            BancoEmpresa bancoempresa = Db.Database.SqlQuery<BancoEmpresa>("select * from BancoEmpresa where IDBancoEmpresa  = " + PagoFacturaProv.RFCBancoEmpresa + "").ToList()[0];

            string Ctaemp = bancoempresa.CuentaBanco;
            int idbancocliente = bancoempresa.IDBanco;
            c_Banco banco = new c_BancoContext().c_Bancos.Find(idbancocliente);
            string rfcbancoempresa = banco.RFC;

            ClsDatoString serie = prov.Database.SqlQuery<ClsDatoString>("select serie as Dato from dbo.FolioPagoProv where IdFolioPAgoProv=1").ToList().FirstOrDefault();
            ClsDatoEntero numero = prov.Database.SqlQuery<ClsDatoEntero>("select numero as Dato from dbo.FolioPagoProv  where IdFolioPAgoProv=1").ToList().FirstOrDefault();


            PagoFacturaProv.SerieP = serie.Dato;
            PagoFacturaProv.FolioP = numero.Dato;


            if (ModelState.IsValid)
            {

                DateTime fechareq = PagoFacturaProv.FechaPago;
                string fecha2 = fechareq.ToString("yyyy/MM/dd HH:mm:ss");
                ClsDatoString claveformaPago = Db.Database.SqlQuery<ClsDatoString>("select distinct ClaveFormaPago as Dato from dbo.c_FormaPago where IDFormaPago = '" + PagoFacturaProv.ClaveFormaPago + "'").ToList()[0]; ;

                ProveedorContext p = new ProveedorContext();
                int idp = int.Parse(PagoFacturaProv.IDProveedor.ToString());
                ClsDatoString empresa = Db.Database.SqlQuery<ClsDatoString>("Select Empresa as Dato from[dbo].[Proveedores] where IDProveedor = " + idp + "").ToList()[0];
                //VBcoEmpresaContext be = new VBcoEmpresaContext();
                //VBcoEmpresaContext be = new VBcoEmpresaContext();

                ClsDatoString rfcempresa = Db.Database.SqlQuery<ClsDatoString>("Select RFC as Dato from dbo.c_Banco where IDBanco= '" + PagoFacturaProv.RFCBancoEmpresa + "'").ToList()[0];

                try
                {

                    string comando1 = "insert into [dbo].[PagoFacturaProv](SerieP,FolioP, IDProveedor,Empresa, FechaPago, ClaveFormaPago, ClaveMoneda, NoOperacion, Monto,RFCBancoEmpresa, CuentaEmpresa, RFCBancoProv, CuentaProv, IDTipoCadenaPago, Observacion,Estado, UUID, RutaXML, RutaPDF, TC, EstadoP) values('" + PagoFacturaProv.SerieP + "', " + PagoFacturaProv.FolioP + ",'" + PagoFacturaProv.IDProveedor + "', '" + empresa.Dato + "', Convert (datetime,'" + fecha2 + "',101), '" + claveformaPago.Dato + "','" + PagoFacturaProv.ClaveMoneda + "', " + PagoFacturaProv.NoOperacion + ", " + PagoFacturaProv.Monto + ", '" + rfcbancoempresa + "', '" + Ctaemp + "', '" + PagoFacturaProv.RFCBancoProv + "', '" + PagoFacturaProv.CuentaProv + "', '" + PagoFacturaProv.IDTipoCadenaPago + "', '" + PagoFacturaProv.Observacion + "','A', '', '', '', " + PagoFacturaProv.TC + ",0)";
                    Db.Database.ExecuteSqlCommand(comando1);
                }
                catch (Exception ex)
                {
                    string mensajederror = ex.Message;
                    string comando2 = "insert into [dbo].[PagoFacturaProv](SerieP,FolioP, IDProveedor,Empresa, FechaPago, ClaveFormaPago, ClaveMoneda, NoOperacion, Monto,RFCBancoEmpresa, CuentaEmpresa, RFCBancoProv, CuentaProv, IDTipoCadenaPago, Observacion,Estado, UUID, RutaXML, RutaPDF, TC, EstadoP) values('" + PagoFacturaProv.SerieP + "', " + PagoFacturaProv.FolioP + ",'" + PagoFacturaProv.IDProveedor + "', '" + empresa.Dato + "', Convert (datetime,'" + fecha2 + "',101), '" + claveformaPago.Dato + "','" + PagoFacturaProv.ClaveMoneda + "', " + PagoFacturaProv.NoOperacion + ", " + PagoFacturaProv.Monto + ", '" + rfcbancoempresa + "', '" + Ctaemp + "', '" + PagoFacturaProv.RFCBancoProv + "', '" + PagoFacturaProv.CuentaProv + "', '" + PagoFacturaProv.IDTipoCadenaPago + "', '" + PagoFacturaProv.Observacion + "','A', '', '', ''," + PagoFacturaProv.TC + ",0)";
                    Db.Database.ExecuteSqlCommand(comando2);
                }
                Db.Database.ExecuteSqlCommand("update foliopagoprov set numero=" + (PagoFacturaProv.FolioP+1)+ " where idfoliopagoprov=1");

            }
            return RedirectToAction("Index");
        }// PagarFacturaProv() Post





        /////////////////////////////////////////////LISTA PAGO FACTURA//////////////////////////////////////


        private static List<VPagoFacturaProvElec> GetPagoElectronico()

        {
            VPagoFacturaProvElecContext dbpf = new VPagoFacturaProvElecContext();
            List<VPagoFacturaProvElec> pagoFactura = dbpf.Database.SqlQuery<VPagoFacturaProvElec>("SELECT * FROM [dbo].[VPagoFacturaProvElec] order by FechaPago desc").ToList();
            return pagoFactura.ToList();
        }
        private static List<VPagoFacturaProvEfe> GetPagoEfectivo()
        {
            VPagoFacturaProvEfeContext db1 = new VPagoFacturaProvEfeContext();
            List<VPagoFacturaProvEfe> pagoFacturaEfe = db1.Database.SqlQuery<VPagoFacturaProvEfe>("SELECT * FROM [dbo].VPagoFacturaProvEfe order by FechaPago desc").ToList();

            return pagoFacturaEfe.ToList();
        } //Index


        ////////////////////////////////////////Documento Relacionado electrónico////////////////////////////////////
        private DocumentoRelacionadoProvContext dbDocto = new DocumentoRelacionadoProvContext();


        //idpf = item.IDPagoFacturaProv, idp = item.IDProveedor, nombrep = item.Empresa, monto = item.Monto
        public ActionResult DocumentoRelacionadoProv(int? idpf, int? idp, string nombrep, decimal monto)
        {
            
            PagoFacturaProv pago = new PagoFacturaProvContext().PagoFacturasProv.Find(idpf);

            VEncFacturaProvContext enc = new VEncFacturaProvContext();
            //List<VEncFacturaProv> metpag = enc.Database.SqlQuery<VEncFacturaProv>("select * from [dbo].VEncFacturaProv where Nombre_Proveedor = '" + pago.Empresa + "'").ToList();

            ProveedorContext prov = new ProveedorContext();
          List<Proveedor> datos = Db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where IDProveedor = " + pago.IDProveedor + " or Empresa = '"+ pago.Empresa +"'").ToList();

            if (idp == null)
            {
               idp = pago.IDProveedor;
             
                if (idp != null)
                {
                    idp = datos.Select(s => s.IDProveedor).FirstOrDefault();
                }
            }

            ViewBag.idproveedor = idp;
            if (nombrep == null)
            {
                nombrep = pago.Empresa;
                if (idp != null)
                {
                    nombrep = datos.Select(s => s.Empresa).FirstOrDefault();
                }
            }
            ViewBag.nombrep = nombrep;
            ViewBag.idp = idp;
            ViewBag.idpf = idpf;
            ViewBag.monto = monto;
            ViewBag.TC = pago.TC;


            List<VEncFacturaProv> encfactura = enc.Database.SqlQuery<VEncFacturaProv>("SELECT  * from dbo.VEncFacturaProv  where IDProveedor= '" + idp + "' ").ToList();
            ViewBag.EncFactura = encfactura;
            var resumen = Db.Database.SqlQuery<ResumenFacP>("select EncFacturaProv.Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, case WHEN EncFacturaProv.Moneda<>'MXN' THEN  (SUM(Total * TC)) WHEN EncFacturaProv.Moneda='MXN' THEN  (SUM(Total )) END  as TotalenPesos from EncFacturaProv where pagada = 0 and estado = 'A' and Nombre_Proveedor ='" + nombrep + "' group by EncFacturaProv.Moneda").ToList();
      
            ViewBag.sumatoria = resumen;
            return View(encfactura);
        }

        [HttpPost]
        public ActionResult DocumentoRelacionadoProv(int? idpf, int? idp, string nombrep, decimal monto, List<VEncFacturaProv> cr)
        {

            decimal sumatoria = 0, importesi = 0, ImporteSaldoInsoluto = 0;
            int noparcialidad = 0, contador = 0;
            PagoFacturaProv pago = new PagoFacturaProvContext().PagoFacturasProv.Find(idpf);


            VEncFacturaProvContext enc = new VEncFacturaProvContext();
            List<Proveedor> metpag = Db.Database.SqlQuery<Proveedor>("select * from [dbo].VEncFacturaProv where Nombre_Proveedor= '"+ pago.Empresa + "'").ToList();



            ProveedorContext prov = new ProveedorContext();
            List<Proveedor> datos = Db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where IDProveedor = " + pago.IDProveedor + " ").ToList();

            if (idp == null)
            {
                idp = pago.IDProveedor;

                if (idp != null)
                {
                    idp = datos.Select(s => s.IDProveedor).FirstOrDefault();
                }
            }

            ViewBag.idproveedor = idp;
            if (nombrep == null)
            {
                nombrep = pago.Empresa;
                if (idp != null)
                {
                    nombrep = datos.Select(s => s.Empresa).FirstOrDefault();
                }
            }
            ViewBag.nombrep = pago.Empresa;
            ViewBag.idp = pago.IDProveedor;
            ViewBag.idpf = idpf;
            ViewBag.monto = pago.Monto;
            ViewBag.TC = pago.TC;
            string Monedadelpago = pago.ClaveMoneda;

           
           // //string Monedadelpago = new c_MonedaContext().c_Monedas.Find(pago.ClaveMoneda).ClaveMoneda;
           // List<c_Moneda> monedapago = dbm.Database.SqlQuery<c_Moneda>("select * from dbo.c_Moneda where ClaveMoneda = '" + pago.ClaveMoneda +"'").ToList();
           //int Monedadelpago = monedapago.Select(s => s.IDMoneda).FirstOrDefault();

            decimal tipocambio = pago.TC;

            //Verificar la sumatoria del importe pagado
            foreach (var a in cr)
            {
                //string Monedadefactura = new c_MonedaContext().c_Monedas.Find(a.IDMoneda).IDMoneda;
                c_MonedaContext dbm = new c_MonedaContext();
                string Monedadefactura = new c_MonedaContext().c_Monedas.Find(a.IDMoneda).ClaveMoneda;
                //string Monedadefactura = new c_MonedaContext().c_Monedas.Find(a.IDMoneda).ClaveMoneda;


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
                            sumatoria += Math.Round(a.ImportePagado / tipocambio, 2);
                        }
                    }
                }
            }

            //Verificar que el importe pagado cumpla con las condiciones

            if (Math.Round(monto, 2) == Math.Round(sumatoria, 2))
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
                             ClsDatoEntero parcialidad = Db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(NoParcialidad), 0) AS INT) as Dato from dbo.DocumentoRelacionadoProv where IDFacturaProv='" + i.ID + "' and Estado = 'A'").ToList().FirstOrDefault(); 
					

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

                            c_FormaPagoContext dbf = new c_FormaPagoContext();
                            List<c_FormaPago> FormaPago = dbf.Database.SqlQuery<c_FormaPago>("select * from dbo.c_FormaPago where IDFormaPago = '" + pago.ClaveFormaPago + "'").ToList();
                            string clave = FormaPago.Select(s => s.ClaveFormaPago).FirstOrDefault();
                            
                            c_MetodoPagoContext dbmp = new c_MetodoPagoContext();
                            //List<c_MetodoPago> MetodoPago = dbf.Database.SqlQuery<c_MetodoPago>("select * from dbo.c_MetodoPago where ClaveMetodoPago = '" + metpag + "'").ToList();
                            //string clavemp = MetodoPago.Select(s => s.ClaveMetodoPago).FirstOrDefault();


                            Db.Database.ExecuteSqlCommand("INSERT INTO DocumentoRelacionadoProv([IDPagoFacturaProv],[IDProveedor],[IDFacturaProv],[Empresa],[Serie],[Numero],[ClaveMoneda],[ClaveFormaPago], [ClaveMetododePago],[ImporteSaldoInsoluto],[ImportePagado],[NoParcialidad], Estado) values('" + idpf + "','" + idp + "', '" + i.ID + "','" + pago.Empresa + "', '" + i.Serie + "','" + i.Numero + "','" + Monedadelpago + "', '"+ i.FormadePago + "', '"+i.MetododePago+"','" + ImporteSaldoInsoluto + "','" + i.ImportePagado + "','" + noparcialidad + "', 'A')");
                            Db.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set ConPagos='true' where ID= " + i.ID );
  
                              Db.Database.ExecuteSqlCommand("update dbo.PagoFacturaProv set Estado= 'A', EstadoP='true'where IDPagoFacturaProv='" + idpf + "'");

                            ClsDatoEntero numero = Db.Database.SqlQuery<ClsDatoEntero>("select count(IDFacturaProv) as Dato from SaldoFacturaProv where IDFacturaProv='" + i.ID + "'").ToList()[0];
                            int num = numero.Dato;
                            if (num != 0)
                            {

                                
                                Db.Database.ExecuteSqlCommand("update SaldoFacturaProv set ImporteSaldoAnterior='" + i.ImporteSaldoInsoluto + "',ImportePagado='" + i.ImportePagado + "',ImporteSaldoInsoluto='" + ImporteSaldoInsoluto + "' where IDFacturaProv=" + i.ID + "");

                            }
                            else
                            {
                                Db.Database.ExecuteSqlCommand("INSERT INTO SaldoFacturaProv([IDFacturaProv], [Serie], [Numero],IDProveedor,[Total],[ImporteSaldoAnterior],[ImportePagado],[ImporteSaldoInsoluto],Estado, Empresa) values (" + i.ID + ",'" + i.Serie + "', " + i.Numero + ", '"+idpf+"', " + i.Total + "," + i.Total + "," + i.ImportePagado + ", " + ImporteSaldoInsoluto + ", 'A', '"+nombrep+"')");
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
                            
                            Db.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set pagada= 1 where ID='" + i.ID + "'");
                            Db.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set conPagos='true' where ID=" + i.ID + " and Serie='" + i.Serie + "' and Numero='" + i.Numero + "'");
                        }


                    }
                    if (Math.Round(sumatoria, 2) == monto)
                    { 
                        Db.Database.ExecuteSqlCommand("update dbo.PagoFacturaProv set Estado= 'A', EstadoP='true'where IDPagoFacturaProv='" + idpf + "'");
                        Db.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set ConPagos='true' where ID=" + idpf + "");

                    }
                }

                else
                {

                    ViewBag.nombrep = nombrep;
                    ViewBag.idp = idp;
                    ViewBag.idpf = idpf;
                    ViewBag.monto = monto;

                    List<VEncFacturaProv> encfactura = enc.Database.SqlQuery<VEncFacturaProv>("SELECT  * from dbo.VEncFacturaProv  where IDProveedor= '" + idp + "' ").ToList();
                    ViewBag.EncFactura = encfactura;
                    var resumen = Db.Database.SqlQuery<ResumenFacP>("select EncFacturaProv.Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, case WHEN EncFacturaProv.Moneda<>'MXN' THEN  (SUM(Total * TC)) WHEN EncFacturaProv.Moneda='MXN' THEN  (SUM(Total )) END  as TotalenPesos from EncFacturaProv where pagada = 0 and estado = 'A' and Nombre_Proveedor ='" + nombrep + "' group by EncFacturaProv.Moneda").ToList();

                    ViewBag.sumatoria = resumen;
                    ViewBag.mensaje = "La cantidad del importe pagado excede el Importe del Saldo Insoluto, verifique por favor";
                    return View("DocumentoRelacionadoProv", encfactura);
                }
            }
            else
            {


                ViewBag.nombrep = nombrep;
                ViewBag.idc = idp;
                ViewBag.idpf = idpf;
                ViewBag.monto = monto;

                List<VEncFacturaProv> encfactura = enc.Database.SqlQuery<VEncFacturaProv>("SELECT  * from dbo.VEncFacturaProv  where IDProveedor= '" + idp + "' ").ToList();
                ViewBag.EncFactura = encfactura;
                var resumen = Db.Database.SqlQuery<ResumenFacP>("select EncFacturaProv.Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, case WHEN EncFacturaProv.Moneda<>'MXN' THEN  (SUM(Total * TC)) WHEN EncFacturaProv.Moneda='MXN' THEN  (SUM(Total )) END  as TotalenPesos from EncFacturaProv where pagada = 0 and estado = 'A' and Nombre_Proveedor ='" + nombrep + "' group by EncFacturaProv.Moneda").ToList();
                ViewBag.sumatoria = resumen;
                ViewBag.mensaje = " El importe de los pagos excede el Monto Total a Pagar, verifique por favor";
                return View("DocumentoRelacionadoProv", encfactura);
            }



            return RedirectToAction("Index");
        }

        ////////////////////////////////////////////////////////SPEI//////////////////////////////////////

        private PagoFacturaSPEIProvContext dbSPEI = new PagoFacturaSPEIProvContext();

        public PagoFacturaProvContext Db
        {
            get
            {
                return db;
            }

            set
            {
                db = value;
            }
        }

        public ActionResult PagoFacturaSPEIProv(int id)
        {
            PagoFacturaSPEIProv pagoFacturaSPEI = null;
            PagoFacturaSPEIProv elemento = new PagoFacturaSPEIProv();
           
                System.Web.HttpContext.Current.Session["IDPagoFacturaProv"] = id;
                try
                {
                    pagoFacturaSPEI = Db.Database.SqlQuery<PagoFacturaSPEIProv>("select IDPagoFacturaSPEIProv, IDPagoFacturaProv, CertificadoPago, IDTipoCadenaPago, SelloPago from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList()[0];


                }
                catch (Exception err)
                {
                    string error = err.Message;
                    elemento.IDPagoFacturaProv = id;
                }
         

            if (pagoFacturaSPEI != null)
            {
                elemento.CertificadoPago = pagoFacturaSPEI.CertificadoPago;
                elemento.SelloPago = pagoFacturaSPEI.SelloPago;
                elemento.IDTipoCadenaPago = pagoFacturaSPEI.IDTipoCadenaPago;
                elemento.IDPagoFacturaProv = id;
                elemento.IDPagoFacturaSPEIProv = pagoFacturaSPEI.IDPagoFacturaSPEIProv;
                // si esiste pasa a modificarlo
            }
            return View(elemento); // si no existe pasa a crearlo


        }
        // POST: PagoFacturaSPEI/Create
        [HttpPost]
        public ActionResult PagoFacturaSPEIProv(PagoFacturaSPEIProv elemento, int id)
        {
            elemento.Estado = "A";
            
            if (ModelState.IsValid) // si llenaron los datos como los validadores del sistema intento grabar
            {

                if (elemento.IDPagoFacturaSPEIProv != 0) // si ya venia con un dato de IDPagoFactura
                {

                    Db.Database.ExecuteSqlCommand("update [dbo].[PagoFacturaSPEIProv] set CertificadoPago='" + elemento.CertificadoPago + "', IDTipoCadenaPago='" + elemento.IDTipoCadenaPago + "', SelloPago='" + elemento.SelloPago + "' where IDPagoFacturaProv=" + id);
                }
                else
                {
                    string cadenaSql= "insert into[dbo].[PagoFacturaSPEIProv]([IDPagoFacturaProv], [CertificadoPago],[IDTipoCadenaPago],[SelloPago], [Estado]) values( " + id+",'" + elemento.CertificadoPago + "', '" + elemento.IDTipoCadenaPago + "',  '" + elemento.SelloPago + "','A')";
                    Db.Database.ExecuteSqlCommand(cadenaSql);
                        //dbSPEI.PagoFacturaSPEIsProv.Add(elemento);
                        //dbSPEI.SaveChanges();

                }



                return RedirectToAction("Index");

            }
            else // si no es valido lo regreso
            {
                return View();
            }


        }
        public FileResult DescargarPdf(int id)
        {
            // Obtener contenido del archivo

            PagoFacturaProvContext p = new PagoFacturaProvContext();
            PagoFacturaProv elemento = p.PagoFacturasProv.Find(id);


            EmpresaContext dbe = new EmpresaContext();
            var empresa = dbe.empresas.Single(m => m.IDEmpresa == id);


            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            PDFPago pago = new PDFPago(elemento.RutaXML, logoempresa,id, empresa.Telefono);
            return new FileStreamResult(Response.OutputStream, "application/pdf");
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


        /////////////////////////////////////////////elimina solo el PAGO FACTURA en el  index//////////////////////////////////////

        public ActionResult EliminaPagoProv(int? id)
        {
            EliminaPagoProv pag = new EliminaPagoProv();

            PagoFacturaProvContext dbe = new PagoFacturaProvContext();
            List<PagoFacturaProv> detallesP = dbe.Database.SqlQuery<PagoFacturaProv>("select * from PagoFacturaProv where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.detallesP = detallesP;

            if (id != null)
            {
                pag.IDPagoFacturaProv = (int)id;
                return View(pag);

            }
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult EliminaPagoProv(EliminaPagoProv pag)
        {
            int id;
            try
            {
                id = pag.IDPagoFacturaProv;
                PagoFacturaProvContext dbe = new PagoFacturaProvContext();
                List<PagoFacturaProv> detallesP = dbe.Database.SqlQuery<PagoFacturaProv>("select * from PagoFacturaProv where IDPagoFacturaProv='" + id + "'").ToList();
                ViewBag.detallesP = detallesP;

                //Actualiza la tabla PagoFactura
                PagoFacturaProv datoC = Db.PagoFacturasProv.Single(m => m.IDPagoFacturaProv == id);
                Db.Database.ExecuteSqlCommand("delete from dbo.PagoFacturaProv where IDPagoFacturaProv = " + id + " ");
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return View(pag);
            }
            return RedirectToAction("Index");
        }
        /////////////////////////////////////////////elimina solo el PAGO FACTURA en el  indexEfee//////////////////////////////////////

        public ActionResult EliminaPagoProvE(int? id)
        {
            EliminaPagoProv pag = new EliminaPagoProv();

            PagoFacturaProvContext dbe = new PagoFacturaProvContext();
            List<PagoFacturaProv> detallesP = dbe.Database.SqlQuery<PagoFacturaProv>("select * from PagoFacturaProv where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.detallesPEfe = detallesP;

            if (id != null)
            {
                pag.IDPagoFacturaProv = (int)id;
                return View(pag);

            }
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction("IndexEfe");
        }


        [HttpPost]
        public ActionResult EliminaPagoProvE(EliminaPagoProv pag)
        {
            int id = pag.IDPagoFacturaProv;
            PagoFacturaProvContext dbe = new PagoFacturaProvContext();
            List<PagoFacturaProv> detallesP = dbe.Database.SqlQuery<PagoFacturaProv>("select * from PagoFacturaProv where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.detallesPEfe = detallesP;

            //Actualiza la tabla PagoFactura
            PagoFacturaProv datoC = Db.PagoFacturasProv.Single(m => m.IDPagoFacturaProv == id);
            Db.Database.ExecuteSqlCommand("delete from dbo.PagoFacturaProv where IDPagoFacturaProv = " + id + " ");

            return RedirectToAction("IndexEFe");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////pago en efectivo//////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult IndexEfe(string Numero, string FolioP, string Empresa, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString, string Estado = "A")
{
    ViewBag.sumatoria = "";
            
            VPagoFacturaProvEfeContext dbv = new VPagoFacturaProvEfeContext();
    //Buscar Proveedor
    var ProvLst = new List<string>();
    var ProvQry = from d in new VPagoFacturaProvEfeContext().VPagoFacturaProvEfes
                  orderby d.Empresa
                  select d.Empresa;
    ProvLst.AddRange(ProvQry.Distinct());
    ViewBag.Empresa = new SelectList(ProvLst);

    //Buscar Estado
    var EstadoLst = new List<SelectListItem>();
    EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });
    EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
    ViewData["Estado"] = EstadoLst;
    ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");


    string ConsultaSql = "select * from VPagoFacturaProvEfe ";
    string ConsultaSqlResumen = " select ClaveMoneda as Moneda, (SUM(Monto)) as Total from dbo.VPagoFacturaProvEfe";
    string ConsultaAgrupado = "group by ClaveMoneda order by ClaveMoneda ";
    string Filtro = string.Empty;
    string Orden = " order by fecha desc , SerieP , FolioP desc ";


    //Buscar por numero
    if (!String.IsNullOrEmpty(FolioP))
    {
        if (Filtro == string.Empty)
        {
            Filtro = "where FolioP=" + FolioP + "";
        }
        else
        {
            Filtro += "and  FolioP=" + FolioP + "";
        }

    }


    ///tabla filtro: Nombre Proveedor
    if (!String.IsNullOrEmpty(Numero))
    {

        if (Filtro == string.Empty)
        {
            Filtro = "where IDPagoFacturaProv='" + Numero + "'";
        }
        else
        {
            Filtro += "and  IDPagoFacturaProv='" + Numero + "'";
        }

    }


    ///tabla filtro: Empresa
    if (!String.IsNullOrEmpty(Empresa))
    {

        if (Filtro == string.Empty)
        {
            Filtro = "where Empresa='" + Empresa + "'";
        }
        else
        {
            Filtro += "and  Empresa='" + Empresa + "'";
        }

    }

    if (Estado != "Todos")
    {
        if (Estado == "C")
        {
            if (Filtro == string.Empty)
            {
                Filtro = "where Estado='C'";
            }
            else
            {
                Filtro += "and  Estado='C'";
            }
        }
        if (Estado == "A")
        {
            if (Filtro == string.Empty)
            {
                Filtro = "where  Estado='A'";
            }
            else
            {
                Filtro += "and Estado='A'";
            }
        }
    }


    if (!String.IsNullOrEmpty(searchString)) //pusieron una fecha
    {
        if (Filtro == string.Empty)
        {
            Filtro = "where  FechaPago BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999' ";
        }
        else
        {
            Filtro += " and FechaPago BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999'";
        }
    }

    ViewBag.CurrentSort = sortOrder;

    ViewBag.FolioSortParm = String.IsNullOrEmpty(sortOrder) ? "FolioP" : "";
    ViewBag.FechaSortParm = sortOrder == "FechaPago" ? "FechaPago" : "";
    ViewBag.EmpresaSortParm = String.IsNullOrEmpty(sortOrder) ? "Empresa" : "";
    ViewBag.EstadoSortParm = String.IsNullOrEmpty(sortOrder) ? "Estado" : "";


    // Not sure here
    if (searchString == null)
    {
        searchString = currentFilter;
    }

    ViewBag.SearchString = searchString;
    // ViewBag.CurrentFilter = searchString;

    //Paginación
    if (searchString != null)
    {
        page = 1;
    }
    else
    {
        searchString = currentFilter;
    }
    //Ordenacion

    switch (sortOrder)
    {
        case "SerieP":
            Orden = " order by  SerieP , FolioP desc ";
            break;
        case "FolioP":
            Orden = " order by  FolioP desc ";
            break;
        case "Numero":
            Orden = " order by IDPagoFacturaProv asc ";
            break;
        case "Fecha":
            Orden = " order by FechaPago ";
            break;
        case "Empresa":
            Orden = " order by  Empresa ";
            break;
        case "Estado":
            Orden = " order by  Estado ";
            break;
        default:
            Orden = " order by FechaPago desc , SerieP , FolioP desc ";
            break;
    }

    //var elementos = from s in db.encfacturas
    //select s;
    string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

    var elementos = dbv.Database.SqlQuery<VPagoFacturaProvEfe>(cadenaSQl).ToList();



    ViewBag.sumatoria = "";
    try
    {

        var SumaLst = new List<string>();
        var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
        List<ResumenFacP> data = Db.Database.SqlQuery<ResumenFacP>(SumaQry).ToList();
        ViewBag.sumatoria = data;

    }
    catch (Exception err)
    {
        string mensaje = err.Message;
    }

    //Paginación
    // DROPDOWNLIST FOR UPDATING PAGE SIZE
    int count = dbv.VPagoFacturaProvEfes.OrderBy(e => e.SerieP).Count();// Total number of elements

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

    return View(elementos.ToPagedList(pageNumber, pageSize));
}

////////////////////////////////////////////////PAGO FACTURA EFECTIVO//////////////////////////////////////
public ActionResult PagarFacturaProvE()
{

    PagoFacturaProv elemento = new PagoFacturaProv();
    ProveedorContext prov = new ProveedorContext();
    var proveedor = prov.Proveedores.ToList();
    List<SelectListItem> li = new List<SelectListItem>();
    li.Add(new SelectListItem { Text = "--Selecciona un Proveedor--", Value = "0" });

    foreach (var p in proveedor)
    {
        li.Add(new SelectListItem { Text = p.Empresa, Value = p.IDProveedor.ToString() });
        ViewBag.proveedor = li;

    }


            List<SelectListItem> liseries = new List<SelectListItem>();
            liseries.Add(new SelectListItem { Text = "PE", Value = "PE" });
            liseries.Add(new SelectListItem { Text = "PN", Value = "PN" });

            ViewBag.SerieP = liseries;


            c_MonedaContext moneda = new c_MonedaContext();
    var monedas = moneda.c_Monedas.ToList();
    List<SelectListItem> listamoneda = new List<SelectListItem>();
    listamoneda.Add(new SelectListItem { Text = "--Selecciona Moneda--", Value = "0" });

    foreach (var m in monedas)
    {
        listamoneda.Add(new SelectListItem { Text = m.ClaveMoneda + " | " + m.Descripcion, Value = m.ClaveMoneda.ToString() });
        ViewBag.datosMoneda = listamoneda;

    }

    //var formapago = formap.c_FormaPagos.ToList();
    var FPLst = new List<string>();
    var FPQry = from p in new c_FormaPagoContext().c_FormaPagos
                orderby p.Descripcion
                select p.Descripcion;
    FPLst.AddRange(FPQry.Distinct());
    ViewBag.datosFormaPago = FPLst;



    elemento.FechaPago = DateTime.Now;

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

    ViewBag.datosProveedor = new SelectList(GetProveedor());

    var listaFormaPago = new FormaPagoRepository().GetFormasdepagoOtra();
    ViewBag.datosFormaPago = listaFormaPago;

    var listaTipoCadena = new c_TipoCadenaPagoRepository().GetTipoCadenaPagoE();
    ViewBag.datosTipoCadena = listaTipoCadena;




    return View(elemento);
}// Fin PagarFacturaProv()

[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult PagarFacturaProvE(PagoFacturaProv PagoFacturaProv)
{

    if (ModelState.IsValid)
    {
        int nuevofolio = 0;
        DateTime fechareq = PagoFacturaProv.FechaPago;
        string fecha2 = fechareq.ToString("yyyy/MM/dd HH:mm:ss");
        ClsDatoString claveformaPago = Db.Database.SqlQuery<ClsDatoString>("select distinct ClaveFormaPago as Dato from dbo.c_FormaPago where IDFormaPago = '" + PagoFacturaProv.ClaveFormaPago + "'").ToList()[0]; ;
                try
                {
            ClsDatoEntero foliop = Db.Database.SqlQuery<ClsDatoEntero>("Select Numero+1 as Dato from[dbo].[FolioPagoProv] where SerieP = '"+PagoFacturaProv.SerieP +"'").ToList()[0];
            nuevofolio = foliop.Dato;
        }
        catch (Exception ex)
        {
                    string mensajederror =ex.Message;
                    nuevofolio = 1;
        }   
        ProveedorContext p = new ProveedorContext();
        int idp = int.Parse(PagoFacturaProv.IDProveedor.ToString());
        ClsDatoString empresa = Db.Database.SqlQuery<ClsDatoString>("Select Empresa as Dato from[dbo].[Proveedores] where IDProveedor = " + idp + "").ToList()[0];

        //ClsDatoString rfcempresa = db.Database.SqlQuery<ClsDatoString>("Select RFC as Dato from dbo.c_Banco where IDBanco= '" + PagoFacturaProv.RFCBancoEmpresa + "'").ToList()[0];
        try
        {
            //ClsDatoString cuenta = db.Database.SqlQuery<ClsDatoString>(" Select CuentaBanco as Dato from dbo.VBancoEmpresa where RFC= '" + rfcempresa.Dato + "'").ToList()[0];
            string comando1 = "insert into [dbo].[PagoFacturaProv](SerieP,FolioP, IDProveedor,Empresa, FechaPago, ClaveFormaPago, ClaveMoneda, Monto, IDTipoCadenaPago, Observacion,Estado, UUID, RutaXML, RutaPDF, TC, NoOperacion, EstadoP) values('"+PagoFacturaProv.SerieP+"', " + nuevofolio + ",'" + PagoFacturaProv.IDProveedor + "', '" + empresa.Dato + "', Convert (datetime,'" + fecha2 + "',101), '" + claveformaPago.Dato + "','" + PagoFacturaProv.ClaveMoneda + "', 0, " + PagoFacturaProv.Monto + ",'00' '" + PagoFacturaProv.Observacion + "','A', '', '', '',"+PagoFacturaProv.TC+",0,0)";
            Db.Database.ExecuteSqlCommand(comando1);
        }
        catch (Exception ex)
        {
                    string mensajederror = ex.Message;
                    string comando2 = "insert into [dbo].[PagoFacturaProv](SerieP,FolioP, IDProveedor,Empresa, FechaPago, ClaveFormaPago, ClaveMoneda, Monto, IDTipoCadenaPago, Observacion,Estado, UUID, RutaXML, RutaPDF, TC, NoOperacion,EstadoP) values('"+PagoFacturaProv.SerieP+"', " + nuevofolio + ",'" + PagoFacturaProv.IDProveedor + "', '" + empresa.Dato + "', Convert (datetime,'" + fecha2 + "',101), '" + claveformaPago.Dato + "','" + PagoFacturaProv.ClaveMoneda + "', " + PagoFacturaProv.Monto + ", '00','" + PagoFacturaProv.Observacion + "','A', '', '', '', " + PagoFacturaProv.TC + ",0,0)";
            Db.Database.ExecuteSqlCommand(comando2);
        }

                try
                {
                    //ClsDatoString cuenta = db.Database.SqlQuery<ClsDatoString>(" Select CuentaBanco as Dato from dbo.VBancoEmpresa where RFC= '" + rfcempresa.Dato + "'").ToList()[0];
                    string comando1 = "update FolioPagoProv set numero=" +nuevofolio+ " where serie='"+ PagoFacturaProv.SerieP+"'";
                    Db.Database.ExecuteSqlCommand(comando1);
                }
                catch (Exception ex)
                {
                }

            }
    return RedirectToAction("IndexEfe");
}// PagarFacturaProv() Post

        public ActionResult DetailsEfe(int id)
        {
            VPagoFacturaProvEfeContext dbe = new VPagoFacturaProvEfeContext();
            var elemento = dbe.VPagoFacturaProvEfes.Single(m => m.IDPagoFacturaProv == id);
            if (elemento == null)
            {
                return NotFound();
            }


            //List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select * from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList();
            //ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;
            List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select * from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;


            VDocumentoRProvContext docto = new VDocumentoRProvContext();
          List<VDocumentoRProv> detallesDR = docto.Database.SqlQuery<VDocumentoRProv>("select IDDocumentoRelacionadoProv, IDPagoFacturaProv, IDProveedor, Empresa, Serie, Numero, ClaveDivisa, Divisa,ClaveFormaPago, FormaPago, ImportePagado,ImporteSaldoInsoluto, NoParcialidad, Estado,clavemetododepago as MetodoPago ,UUID from [dbo].[VDocumentoRProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.detallesDR = detallesDR;



            return View(elemento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailsEfe(int id,VPagoFacturaEfeProv collection)
        {
            //PagoFacturaProvContext pprov = new PagoFacturaProvContext();
            //var elemento = pprov.PagoFacturasProv.Single(m => m.IDPagoFacturaProv == id);
            VPagoFacturaEfeContext dbe = new VPagoFacturaEfeContext();
            var elemento = dbe.VPagoFacturaEfes.Single(m => m.IDPagoFactura == id);
            if (elemento == null)
            {
                return NotFound();
            }

            PagoFacturaSPEIProvContext spei = new PagoFacturaSPEIProvContext();
            //List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select D.*, E.UUID  from dbo.DocumentoRelacionadoProv as D inner join [dbo].[EncFacturaProv] as E on D.IDFacturaProv = E.ID and D.IDPagoFacturaProv='" + id + "'").ToList();
            //ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;
            List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select * from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;




            VDocumentoRProvContext docto = new VDocumentoRProvContext();
            List<VDocumentoRProv> detallesDR = docto.Database.SqlQuery<VDocumentoRProv>("select IDDocumentoRelacionadoProv, IDPagoFacturaProv, IDProveedor, Empresa, Serie, Numero, ClaveDivisa, Divisa,ClaveFormaPago, FormaPago, ClaveMetododepago, MetodoPAgo, ImportePagado,ImporteSaldoInsoluto, NoParcialidad, Estado, UUID from [dbo].[VDocumentoRProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.detallesDR = detallesDR;




            //return View(elemento);
            return RedirectToAction("IndexEfe");

        }// DetailsEfe

        ////////////////////////////////////////Documento Relacionado Efectivo////////////////////////////////////

     
        //idpf = item.IDPagoFacturaProv, idp = item.IDProveedor, nombrep = item.Empresa, monto = item.Monto
        public ActionResult DocumentoRelacionadoProvE( int? idpf, int? idp, string nombrep, decimal monto)
        {
            //DocumentoRelacionadoProvContext dbDocto = new DocumentoRelacionadoProvContext();
            //ProveedorContext p = new ProveedorContext();
            //Proveedor nombrep = p.Proveedores.Find(idp);
            DocumentoRelacionadoProvContext docto = new DocumentoRelacionadoProvContext();
            List<DocumentoRelacionadoProv> doctorel = docto.Database.SqlQuery<DocumentoRelacionadoProv>("select *  from dbo.DocumentoRelacionadoProv where IDProveedor='" + idp + "'").ToList();
            PagoFacturaProv pago = new PagoFacturaProvContext().PagoFacturasProv.Find(idpf);

            ProveedorContext prov = new ProveedorContext();
            List<Proveedor> datos = Db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where IDProveedor = " + pago.IDProveedor + " or Empresa = '" + pago.Empresa + "'").ToList();

            if (idp == null)
            {
                idp = pago.IDProveedor;

                if (idp != null)
                {
                    idp = datos.Select(s => s.IDProveedor).FirstOrDefault();
                }
            }

            ViewBag.idproveedor = idp;
            if (nombrep == null)
            {
                nombrep = pago.Empresa;
                if (idp != null)
                {
                    nombrep = datos.Select(s => s.Empresa).FirstOrDefault();
                }
            }
            ViewBag.nombrep = pago.Empresa;
            ViewBag.idp = pago.IDProveedor;
            ViewBag.idpf = idpf;
            ViewBag.monto = pago.Monto;
            ViewBag.TC = pago.TC;

            VEncFacturaProvContext enc = new VEncFacturaProvContext();

            List<VEncFacturaProv> encfactura = enc.Database.SqlQuery<VEncFacturaProv>("SELECT * from dbo.VEncFacturaProv  where IDProveedor= '" + idp + "' ").ToList();
            ViewBag.EncFactura = encfactura;

            var resumen = Db.Database.SqlQuery<ResumenFacP>("Select Moneda, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from [dbo].[VEncFacturaProv] where IDProveedor= '" + idp + "' group by Moneda").ToList();
            ViewBag.sumatoria = resumen;
         
           return View(encfactura);
        }

        [HttpPost]
        public ActionResult DocumentoRelacionadoProvE(int? idpf, int? idp, string nombrep, decimal monto, List<VEncFacturaProv> cr)
        {

            decimal sumatoria = 0, importesi = 0, ImporteSaldoInsoluto = 0;
            int noparcialidad = 0, contador = 0;
            DocumentoRelacionadoProvContext docto = new DocumentoRelacionadoProvContext();
            List<DocumentoRelacionadoProv> doctorel = docto.Database.SqlQuery<DocumentoRelacionadoProv>("select *  from dbo.DocumentoRelacionadoProv as D where D.IDPagoFacturaProv='" + idp + "'").ToList();
            PagoFacturaProv pago = new PagoFacturaProvContext().PagoFacturasProv.Find(idpf);


            VEncFacturaProvContext enc = new VEncFacturaProvContext();
            List<Proveedor> metpag = Db.Database.SqlQuery<Proveedor>("select * from [dbo].VEncFacturaProv where Nombre_Proveedor= '" + pago.Empresa + "'").ToList();
            ProveedorContext prov = new ProveedorContext();
            List<Proveedor> datos = Db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where IDProveedor = " + pago.IDProveedor + " or Empresa = '" + pago.Empresa + "'").ToList();

            if (idp == null)
            {
                idp = pago.IDProveedor;

                if (idp != null)
                {
                    idp = datos.Select(s => s.IDProveedor).FirstOrDefault();
                }
            }

            ViewBag.idproveedor = idp;
            if (nombrep == null)
            {
                nombrep = pago.Empresa;
                if (idp != null)
                {
                    nombrep = datos.Select(s => s.Empresa).FirstOrDefault();
                }
            }
            ViewBag.nombrep = pago.Empresa;
            ViewBag.idp = pago.IDProveedor;
            ViewBag.idpf = idpf;
            ViewBag.monto = pago.Monto;
            ViewBag.TC = pago.TC;
            string Monedadelpago = pago.ClaveMoneda;
            decimal tipocambio = pago.TC;

            //Verificar la sumatoria del importe pagado
            foreach (var a in cr)
            {
                // string Monedadefactura = new c_MonedaContext().c_Monedas.Find(a.IDMoneda).ClaveMoneda;
                c_MonedaContext dbm = new c_MonedaContext();
                List<c_Moneda> monedafac = dbm.Database.SqlQuery<c_Moneda>("select * from dbo.c_Moneda where ClaveMoneda = '" + pago.ClaveMoneda + "'").ToList();
                string Monedadefactura = monedafac.Select(s => s.ClaveMoneda).FirstOrDefault();


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
                    if (Monedadelpago == "USD" && a.Moneda == "MXN")
                    {
                        sumatoria += a.ImportePagado / tipocambio;
                    }
                }
            }


            //Verificar que el importe pagado cumpla con las condiciones

            if (Math.Round(monto, 2) == Math.Round(sumatoria, 2))
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
                            ClsDatoEntero parcialidad = Db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(NoParcialidad), 0) AS INT) as Dato from dbo.DocumentoRelacionadoProv where IDFacturaProv='" + i.ID + "' and Estado = 'A'").ToList().FirstOrDefault(); 

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

                            c_FormaPagoContext dbf = new c_FormaPagoContext();
                            List<c_FormaPago> FormaPago = dbf.Database.SqlQuery<c_FormaPago>("select * from dbo.c_FormaPago where IDFormaPago = '" + pago.ClaveFormaPago + "'").ToList();
                            string clave = FormaPago.Select(s => s.ClaveFormaPago).FirstOrDefault();

                            c_MetodoPagoContext dbmp = new c_MetodoPagoContext();

                            Db.Database.ExecuteSqlCommand("INSERT INTO DocumentoRelacionadoProv([IDPagoFacturaProv],[IDProveedor],[IDFacturaProv],[Empresa],[Serie],[Numero],[ClaveMoneda],[ClaveFormaPago],[ClaveMetododepago],[ImporteSaldoInsoluto],[ImportePagado],[NoParcialidad], Estado) values ('" + idpf + "','" + idp + "', '" + i.ID + "','" + pago.Empresa + "', '" + i.Serie + "','" + i.Numero + "','" + Monedadelpago + "', '" + i.FormadePago + "', '" + i.MetododePago + "','" + ImporteSaldoInsoluto + "','" + i.ImportePagado + "','" + noparcialidad + "', 'A')");
                            Db.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set ConPagos='true' where ID=" + i.ID + "");

                            Db.Database.ExecuteSqlCommand("update dbo.PagoFacturaProv set Estado= 'A', EstadoP='true' where IDPagoFacturaProv='" + idpf + "'");

                            ClsDatoEntero numero = Db.Database.SqlQuery<ClsDatoEntero>("select count(IDFacturaProv) as Dato from SaldoFacturaProv where IDFacturaProv='" + i.ID + "'").ToList().FirstOrDefault();
                            int num = numero.Dato;
                            if (num != 0)
                            {


                                Db.Database.ExecuteSqlCommand("update SaldoFacturaProv set ImporteSaldoAnterior='" + i.ImporteSaldoInsoluto + "',ImportePagado='" + i.ImportePagado + "',ImporteSaldoInsoluto='" + ImporteSaldoInsoluto + "' where IDFacturaProv=" + i.ID + "");

                            }
                            else
                            {
                                Db.Database.ExecuteSqlCommand("INSERT INTO SaldoFacturaProv([IDFacturaProv], [Serie], [Numero],IDProveedor,[Total],[ImporteSaldoAnterior],[ImportePagado],[ImporteSaldoInsoluto],Estado, Empresa) values (" + i.ID + ",'" + i.Serie + "', " + i.Numero + ", '" + idpf + "', " + i.Total + "," + i.Total + "," + i.ImportePagado + ", " + ImporteSaldoInsoluto + ", 'A', '" + nombrep + "')");
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

                            Db.Database.ExecuteSqlCommand("update EncFacturaProv set pagada= 1 where ID=" + i.ID + "");
                            Db.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set conPagos='true' where ID=" + i.ID + "and Serie='" + i.Serie + "' and Numero= " + i.Numero + " ");

                            /// desactiva documentos relacionados
                            /// 
                        }


                    }
                    if (Math.Round(sumatoria, 2) == monto)
                    {
                        Db.Database.ExecuteSqlCommand("update dbo.PagoFacturaProv set Estado= 'A', EstadoP='true'where IDPagoFacturaProv='" + idpf + "'");
                        Db.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set ConPagos='true' where ID=" + idpf + "");

                    }
                }

                else
                {

                    ViewBag.nombrep = nombrep;
                    ViewBag.idp = idp;
                    ViewBag.idpf = idpf;
                    ViewBag.monto = monto;
                    List<VEncFactura> encfactura = Db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Proveedor, E.TC, E.IDMoneda, E.Moneda,  E.Metododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturaProv as E left outer join (DocumentoRelacionadoProv  as D left outer join SaldoFacturaProv as S on D.IDFacturaProv = S.IDFacturaProv) on E.ID = D.IDFacturaProv where pagada = 0 and E.Estado = 'A' and Nombre_Proveedor ='" + nombrep + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Proveedor, E.TC, E.IDMoneda, E.Moneda, E.Metododepago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();
                    ViewBag.EncFactura = encfactura;
                    var resumen = Db.Database.SqlQuery<ResumenFacP>("select EncFacturaProv.Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, case WHEN EncFacturaProv.Moneda<>'MXN' THEN  (SUM(Total * TC)) WHEN EncFacturaProv.Moneda='MXN' THEN  (SUM(Total )) END  as TotalenPesos from EncFacturaProv where pagada = 0 and estado = 'A' and Nombre_Proveedor ='" + nombrep + "' group by EncFacturaProv.Moneda").ToList();

                    ViewBag.sumatoria = resumen;
                    ViewBag.mensaje = "La cantidad del importe pagado excede el Importe del Saldo Insoluto, verifique por favor";
                    return View("DocumentoRelacionadoProvE", encfactura);
                }
            }
            else
            {


                ViewBag.nombrep = nombrep;
                ViewBag.idc = idp;
                ViewBag.idpf = idpf;
                ViewBag.monto = monto;
                List<VEncFacturaProv> encfactura = Db.Database.SqlQuery<VEncFacturaProv>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Proveedor, E.TC, E.IDMoneda, E.Moneda,  E.Metododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturaPRov as E left outer join (DocumentoRelacionadoProv  as D left outer join SaldoFacturaProv as S on D.IDFacturaProv = S.IDFacturaProv) on E.ID = D.IDFacturaProv where pagada = 0 and E.Estado = 'A' and Nombre_Proveedor ='" + nombrep + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Proveedor, E.TC, E.IDMoneda, E.Moneda,E.Metododepago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();
                ViewBag.EncFactura = encfactura;
                var resumen = Db.Database.SqlQuery<ResumenFacP>("select EncFacturaProv.Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, case WHEN EncFacturaProv.Moneda<>'MXN' THEN  (SUM(Total * TC)) WHEN EncFacturaProv.Moneda='MXN' THEN  (SUM(Total )) END  as TotalenPesos from EncFacturaProv where pagada = 0 and estado = 'A' and Nombre_Proveedor ='" + nombrep + "' group by EncFacturaProv.Moneda").ToList();
                ViewBag.sumatoria = resumen;
                ViewBag.mensaje = " El importe de los pagos excede el Monto Total a Pagar, verifique por favor";
                return View("DocumentoRelacionadoProvE", encfactura);
            }



            return RedirectToAction("IndexEfe");
        }



        // // // // // // // // // // // // // // // // // // // //Reportes // // // // // // // // // // // // // // // // // // // //

        public ActionResult CreaReporteporFecha()
        {
            //Buscar Cliente
            ProveedorContext dbc = new ProveedorContext();
            var prov = dbc.Proveedores.OrderBy(m => m.Empresa).ToList();
            List<SelectListItem> listaPro = new List<SelectListItem>();
            listaPro.Add(new SelectListItem { Text = "--Selecciona Empresa--", Value = "0" });

            var EstadoLst = new List<SelectListItem>();


            EstadoLst.Add(new SelectListItem { Text = "Todos", Value = "T" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });

            ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

            foreach (var m in prov)
            {
                listaPro.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });
            }
            ViewBag.proveedor = listaPro;
            return View();
        }


      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


     

       
        public ActionResult SubirArchivoPagProv(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult SubirArchivoPagProv(HttpPostedFileBase file, int id)
        {
            int idF = int.Parse(id.ToString());
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            string extension = Path.GetExtension(file.FileName).ToLower();

            if (file != null && file.ContentLength > 0)
            {

                if (extension == ".pdf")
                {
                    string ruta = Server.MapPath("~/PagosProvAdd/");
                    ruta += "Doc_" + id + "_" + file.FileName;
                    string cad = "insert into  [dbo].[PagoProvAdd]([IDPagoFacturaProv], [RutaArchivo], nombreArchivo) values(" + idF + ", '" + ruta + "','" + "Doc_" + id + "_" + file.FileName + "' )";
                    new PagoProvAddContext().Database.ExecuteSqlCommand(cad);
                    modelo.SubirArchivo(ruta, file);
                    ViewBag.Error = modelo.error;
                    ViewBag.Correcto = modelo.Confirmacion;
                }
                else
                {
                    ViewBag.Mensajeerror1 = "No se pudo subir el archivo";
                }
                if (extension == ".xml")
                {
                    string ruta = Server.MapPath("~/PagosProvAdd/");
                    ruta += "XML_" + id + "_" + file.FileName;
                    string cad = "insert into  [dbo].[PagoProvAdd]([IDPagoFacturaProv], [RutaArchivo], nombreArchivo) values(" + idF + ", '" + ruta + "','" + "Xml_" + id + "_" + file.FileName + "' )";
                    new PagoProvAddContext().Database.ExecuteSqlCommand(cad);
                    modelo.SubirArchivo(ruta, file);
                    ViewBag.Error = modelo.error;
                    ViewBag.Correcto = modelo.Confirmacion;
                }
                else
                {
                    ViewBag.Mensajeerror1 = "No se pudo subir el archivo";
                }
            }

            return RedirectToAction("index");
        }
        public ActionResult SubirArchivoPagPProv(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult SubirArchivoPagPProv(HttpPostedFileBase file, int id)
        {
            int idF = int.Parse(id.ToString());
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            string extension = Path.GetExtension(file.FileName).ToLower();

            if (file != null && file.ContentLength > 0)
            {

                if (extension == ".pdf")
                {
                    string ruta = Server.MapPath("~/PagosProvAdd/");
                    ruta += "Doc_" + id + "_" + file.FileName;
                    string cad = "insert into  [dbo].[PagoProvAdd]([IDPagoFacturaProv], [RutaArchivo], nombreArchivo) values(" + idF + ", '" + ruta + "','" + "Doc_" + id + "_" + file.FileName + "' )";
                    new PagoProvAddContext().Database.ExecuteSqlCommand(cad);
                    modelo.SubirArchivo(ruta, file);
                    ViewBag.Error = modelo.error;
                    ViewBag.Correcto = modelo.Confirmacion;
                }
                else
                {
                    ViewBag.Mensajeerror1 = "No se pudo subir el archivo";
                }
                if (extension == ".xml")
                {
                    string ruta = Server.MapPath("~/PagosProvAdd/");
                    ruta += "XML_" + id + "_" + file.FileName;
                    string cad = "insert into  [dbo].[PagoProvAdd]([IDPagoFacturaProv], [RutaArchivo], nombreArchivo) values(" + idF + ", '" + ruta + "','" + "Xml_" + id + "_" + file.FileName + "' )";
                    new PagoProvAddContext().Database.ExecuteSqlCommand(cad);
                    modelo.SubirArchivo(ruta, file);
                    ViewBag.Error = modelo.error;
                    ViewBag.Correcto = modelo.Confirmacion;
                }
                else
                {
                    ViewBag.Mensajeerror1 = "No se pudo subir el archivo";
                }
            }

            return RedirectToAction("indexP");
        }

        public ActionResult DescargarPDFPagProv(int id)
        {
            // Obtener contenido del archivo
            PagoProvAddContext dbp = new PagoProvAddContext();
            PagoProvAdd elemento = dbp.PagoProvAdd.Find(id);
            string extension = elemento.nombreArchivo.Substring(elemento.nombreArchivo.Length - 3, 3);
            extension = extension.ToLower();
            if (extension == "pdf")
            {
                string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == "xml")
            {
                string contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            return RedirectToAction("Index");
        }




        public ActionResult DescargarPDFPagPProv(int id)
        {
            // Obtener contenido del archivo
            PagoProvAddContext dbp = new PagoProvAddContext();
            PagoProvAdd elemento = dbp.PagoProvAdd.Find(id);
            string extension = elemento.nombreArchivo.Substring(elemento.nombreArchivo.Length - 3, 3);
            extension = extension.ToLower();
            if (extension == "pdf")
            {
                string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == "xml")
            {
                string contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);

            }
            return RedirectToAction("IndexP");
        }

        public ActionResult EliminarArchivoPagProv(int id)
        {
            PagoProvAddContext db = new PagoProvAddContext();
            string cad = "delete from [dbo].[PagoProvAdd] where ID= " + id + "";
            new PagoProvAddContext().Database.ExecuteSqlCommand(cad);
            return RedirectToAction("Index");
        }

        public ActionResult EntreFechaSaldoP()
        {
            ProveedorContext prov = new ProveedorContext();
            List<Proveedor> proveedor = new List<Proveedor>();
            string cadena = "select* from dbo.Proveedores where Empresa in (select distinct Nombre_Proveedor from[dbo].[VEncFacturaSaldoProv] where ImporteSaldoInsoluto> 0) order by Empresa";
            proveedor = prov.Database.SqlQuery<Proveedor>(cadena).ToList();
            List<SelectListItem> listaproveedor = new List<SelectListItem>();
            listaproveedor.Add(new SelectListItem { Text = "--Todos los proveedores--", Value = "0" });

            foreach (var m in proveedor)
            {
                listaproveedor.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });
            }
            ViewBag.proveedor = listaproveedor;
            return View();

        }

        [HttpPost]
        public ActionResult EntreFechaSaldoP(ReporteSaldoProveedor modelo, Proveedor p, FormCollection coleccion)
        {
            int idproveedor = p.IDProveedor;
            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {
                try
                {

                    ProveedorContext db = new ProveedorContext();
                    Proveedor prov = db.Proveedores.Find(p.IDProveedor);
                }
                catch (Exception ERR)
                {

                }

                ReporteSaldoProveedor report = new ReporteSaldoProveedor();
                //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
                byte[] abytes = report.PrepareReport(idproveedor);
                return File(abytes, "application/pdf");
            }
            else
            {
                string cadena = "";


                if (idproveedor != 0)
                {
                    cadena = "select distinct e.idproveedor, e.Nombre_Proveedor, e.ID as Ticket, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, e.fechaVencimiento, e.FechaRevision, s.ImporteSaldoInsoluto, e.Fecha,e.TC from enCfacturaprov as e inner join SaldoFacturaprov as s on s.IDFacturaProv = e.id  where s.ImporteSaldoInsoluto > 0   and e.Estado = 'A' and e.idproveedor = " + idproveedor + "  order by idproveedor, numero";

                }
                else
                {
                    cadena = "select distinct e.idproveedor, e.Nombre_Proveedor, e.ID as Ticket, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, e.fechaVencimiento, e.FechaRevision, s.ImporteSaldoInsoluto, e.Fecha,e.TC  from enCfacturaprov as e inner join SaldoFacturaprov as s on s.IDFacturaProv=e.id  where s.ImporteSaldoInsoluto>0   and  e.Estado='A' order by idproveedor, numero";
                }
                List<EncFacProveedor> facturassaldos = db.Database.SqlQuery<EncFacProveedor>(cadena).ToList();
                ViewBag.facturassaldos = facturassaldos;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Antiguedad de Saldos");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:L1"].Style.Font.Size = 20;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L3"].Style.Font.Bold = true;
                Sheet.Cells["A1:L1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Antiguedad de Saldos de Proveedores");

                row = 2;
                Sheet.Cells["A1:L1"].Style.Font.Size = 12;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:L2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;

                string Fec = DateTime.Today.Day.ToString() + "- " + DateTime.Today.Month.ToString() + "- " + DateTime.Today.Year.ToString();
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha del reporte";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = Fec;
                //Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                //Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                //Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:L3"].Style.Font.Bold = true;
                Sheet.Cells["A3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Proveedor");
                Sheet.Cells["B3"].RichText.Add("Fecha Fac.");
                Sheet.Cells["C3"].RichText.Add("Ticket");
                Sheet.Cells["D3"].RichText.Add("No.");
                Sheet.Cells["E3"].RichText.Add("Fecha Rev.");
                Sheet.Cells["F3"].RichText.Add("Fecha Ven.");
                Sheet.Cells["G3"].RichText.Add("Total");
                Sheet.Cells["H3"].RichText.Add("TC");
                Sheet.Cells["I3"].RichText.Add("Al Corriente");
                Sheet.Cells["J3"].RichText.Add("0-30 días");
                Sheet.Cells["K3"].RichText.Add("31-90 días");
                Sheet.Cells["L3"].RichText.Add("91 o mas");




                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;

                Sheet.Cells.Style.Font.Bold = false;
                foreach (var item in ViewBag.facturassaldos)
                {

                    ClsOperacionesFecha fechas = new ClsOperacionesFecha();
                    fechas.fechaini = item.FechaVencimiento;
                    fechas.fechafin = DateTime.Now;
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Nombre_Proveedor;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Ticket;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Numero;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.FechaRevision;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.FechaVencimiento;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.TC;

                    if (fechas.getcorriente())
                    {
                        Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("I{0}", row)].Value = item.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("I{0}", row)].Value = 0;
                    }

                    if (fechas.get30())
                    {

                        Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("J{0}", row)].Value = item.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("J{0}", row)].Value = 0;
                    }

                    if (fechas.get3190())
                    {
                        Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("K{0}", row)].Value = item.ImporteSaldoInsoluto;

                    }
                    else
                    {
                        Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("K{0}", row)].Value = 0;
                    }

                    if (fechas.get91mas())
                    {
                        Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("L{0}", row)].Value = item.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("L{0}", row)].Value = 0;
                    }

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteSaldoF.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            //return Redirect("index");
        }

       



        //Reporte
        public ActionResult EntreFechasPagoProv()
        {
            EFecha elemento = new EFecha();
            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasPagoProv(EFecha modelo, FormCollection coleccion)
        {
            VPagoProveedorContext dbe = new VPagoProveedorContext();
            VPagoProvDoctoContext dbr = new VPagoProvDoctoContext();
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
                List<VPagoProveedor> datos;
                List<VPagoProvDocto> datosDet;
                //try
                //{
                cadena = "select * from dbo.VPagoProveedor where FechaPago >= '" + FI + "' and FechaPago  <='" + FF + "' ";
                datos = db.Database.SqlQuery<VPagoProveedor>(cadena).ToList();
                //}
                //catch (Exception err)
                //{
                //    throw new Exception("Hay un error " + err.Message);
                //}

                try
                {
                    cadenaDet = "select * from [dbo].[VPagoProvDocto] where FechaPago >= '" + FI + "' and FechaPago <='" + FF + "' ";
                    datosDet = db.Database.SqlQuery<VPagoProvDocto>(cadenaDet).ToList();
                }
                catch (Exception err)
                {
                    throw new Exception("Hay un error " + err.Message);
                }


                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Pago Factura Proveedor");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:U1"].Style.Font.Size = 20;
                Sheet.Cells["A1:U1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:U3"].Style.Font.Bold = true;
                Sheet.Cells["A1:U1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:U1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Pagos de Facturas de Proveedores");

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
                Sheet.Cells["A3"].RichText.Add("ID Pago Factura");
                Sheet.Cells["B3"].RichText.Add("Serie Pago");
                Sheet.Cells["C3"].RichText.Add("Folio Pago");
                Sheet.Cells["D3"].RichText.Add("Fecha Pago");
                Sheet.Cells["E3"].RichText.Add("RFC");
                Sheet.Cells["F3"].RichText.Add("Empresa");
                Sheet.Cells["G3"].RichText.Add("Monto");
                Sheet.Cells["H3"].RichText.Add("Clave Moneda");
                Sheet.Cells["I3"].RichText.Add("TC");
                Sheet.Cells["J3"].RichText.Add("Clave Forma de Pago");
                Sheet.Cells["K3"].RichText.Add("Forma de Pago");
                Sheet.Cells["L3"].RichText.Add("No. de Operación");
                Sheet.Cells["M3"].RichText.Add("RFC Banco Empresa");
                Sheet.Cells["N3"].RichText.Add("Cuenta Empresa");
                Sheet.Cells["O3"].RichText.Add("RFC Banco del Proveedor");
                Sheet.Cells["P3"].RichText.Add("Banco Proveedor");
                Sheet.Cells["Q3"].RichText.Add("Cuenta Proveedor");
                Sheet.Cells["R3"].RichText.Add("TipoCadenaPago");
                Sheet.Cells["S3"].RichText.Add("Observación");
                Sheet.Cells["T3"].RichText.Add("Estado del Pago");
                Sheet.Cells["U3"].RichText.Add("UUID");

                Sheet.Cells["A3:U3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VPagoProveedor item in datos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDPagoFacturaProv;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.SerieP;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.FolioP;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.FechaPago;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.RFC;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Empresa;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Monto;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.ClaveFormaPago;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.FormaPago;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.NoOperacion;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.RFCBancoEmpresa;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.CuentaEmpresa;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.RFCBancoProv;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.BancoProv;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.CuentaProv;
                    Sheet.Cells[string.Format("R{0}", row)].Value = item.IDTipoCadenaPago;
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.Observacion;
                    Sheet.Cells[string.Format("T{0}", row)].Value = item.Estado;
                    Sheet.Cells[string.Format("U{0}", row)].Value = item.IDTipoCadenaPago;
                    Sheet.Cells[string.Format("V{0}", row)].Value = item.UUID;
                    row++;
                }

                //Hoja No. 2
                Sheet = Ep.Workbook.Worksheets.Add("Documentos Relacionados");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:P1"].Style.Font.Size = 20;
                Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:P3"].Style.Font.Bold = true;
                Sheet.Cells["A1:P1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:P1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Documentos Relacionados");

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
                Sheet.Cells["A3"].RichText.Add("ID Documento");
                Sheet.Cells["B3"].RichText.Add("ID Pago Factura");
                Sheet.Cells["C3"].RichText.Add("Fecha Pago");
                Sheet.Cells["D3"].RichText.Add("Serie Pago");
                Sheet.Cells["E3"].RichText.Add("Folio Pago");
                Sheet.Cells["F3"].RichText.Add("Empresa"); ;
                Sheet.Cells["G3"].RichText.Add("Serie Factura");
                Sheet.Cells["H3"].RichText.Add("No. Factura ");
                Sheet.Cells["I3"].RichText.Add("Total");
                Sheet.Cells["J3"].RichText.Add("Importe Pagado");
                Sheet.Cells["K3"].RichText.Add("Importe Saldo Insoluto");
                Sheet.Cells["L3"].RichText.Add("No. Parcialidad");
                Sheet.Cells["M3"].RichText.Add("Clave Moneda");
                Sheet.Cells["N3"].RichText.Add("ClaveFormaPago");
                Sheet.Cells["O3"].RichText.Add("ClaveMetododePago");
                Sheet.Cells["P3"].RichText.Add("Estado");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:P3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VPagoProvDocto itemD in datosDet)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.IDDocumentoRelacionadoProv;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.IDPagoFacturaProv;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.FechaPago;
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemD.SerieP;
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemD.FolioP;
                    Sheet.Cells[string.Format("F{0}", row)].Value = itemD.Empresa;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.SerieF;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.NumeroF;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.Total;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.ImportePagado;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.ImporteSaldoInsoluto;
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.NoParcialidad;
                    Sheet.Cells[string.Format("M{0}", row)].Value = itemD.ClaveMoneda;
                    Sheet.Cells[string.Format("N{0}", row)].Value = itemD.ClaveFormapago;
                    Sheet.Cells[string.Format("O{0}", row)].Value = itemD.ClaveMetododepago;
                    Sheet.Cells[string.Format("P{0}", row)].Value = itemD.Estado;


                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "PagoProveedor.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }


    }//Public class
}//name space