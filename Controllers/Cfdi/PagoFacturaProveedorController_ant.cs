using SIAAPI.clasescfdi;
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

namespace SIAAPI.Controllers.Cfdi
{
    public class PagoFacturaProveedorController : Controller
    {
        //[Authorize(Roles = "Administrador,Facturacion,Gerencia,Sistemas")]
        // GET: PagoProv
        private PagoFacturaProvContext db = new PagoFacturaProvContext();
        private VPagoFacturaProvContext dbv = new VPagoFacturaProvContext();
        private PagoFacturaSPEIProvContext spei = new PagoFacturaSPEIProvContext();
        private VBcoProvContext bancop = new VBcoProvContext();
        private TipoCambioContext ttc = new TipoCambioContext();
        private ProveedorContext prov = new ProveedorContext();
        /////////////////////// Index ///////////////////////
        ////////////////////////////////////////////////////
        public ActionResult Index(string Numero,string FolioP, string Empresa, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString,  string Estado = "A")
        {

            //PagoFacturaProvContext pf = new PagoFacturaProvContext();
            //var lista = from e in pf.PagoFacturasProv
            //            orderby e.IDPagoFacturaProv
            //            select e;
            //return View(lista);

            
           
            
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


            string ConsultaSql = "select * from PagoFacturaProv ";
            string ConsultaSqlResumen = " select ClaveMoneda as Moneda, (SUM(Monto)) as Total from dbo.PagoFacturaProv";
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

            var elementos = db.Database.SqlQuery<PagoFacturaProv>(cadenaSQl).ToList();



            ViewBag.sumatoria = "";
            try
            {

                var SumaLst = new List<string>();
                var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
                List<ResumenFacP> data = db.Database.SqlQuery<ResumenFacP>(SumaQry).ToList();
                ViewBag.sumatoria = data;

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.PagoFacturasProv.OrderBy(e => e.SerieP).Count();// Total number of elements

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
                    //string error = err.Message;
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
                    pagof.FechaPago =DateTime.Parse( fechap);
                    pagof.ClaveFormaPago = formap;
                    pagof.ClaveMoneda = monedap;
                    try
                    {
                        pagof.NoOperacion = long.Parse(nooperacion);
                    }
                    catch(Exception err)
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
                    pagof.TC = 1;

                    //  pagof.TC = pagoitem.;


                    //string comando = @"insert into [dbo].[PagoFacturaProv](SerieP, FolioP,IDProveedor,Empresa, FechaPago,ClaveFormaPago,
                    //ClaveMoneda, NoOperacion, Monto,  RFCBancoEmpresa, CuentaEmpresa, RFCBancoProv, CuentaProv, IDTipoCadenaPago, 

                    //Estado,UUID, RutaXml) values('" + seriep + "', " + foliop + ", " + IDProveedor + ", '" + proveedor + "', '" + fechap + "', '" + formap + "', '" +
                    //monedap + "','" + nooperacion + "', " + monto + ", '" + rfcBancoe + "', '" + cuentae + "', '" + rfcBancor + "', '" + cuentar + "', '" 
                    //+ tipocadp + "', '" + 'A' + "','" + uuid + "','" + contenidoxml + "')";

                    //pf.Database.ExecuteSqlCommand(comando);

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
                            string comando2 = "insert into [dbo].[DocumentoRelacionadoProv](IDPagoFacturaProv, IDProveedor, Empresa, Numero, ClaveMoneda, ClaveFormaPago, ClaveMetododepago, ImporteSaldoInsoluto, importepagado, NoParcialidad) values(" + idpagof.Dato + ", " + IDProveedor + ",  '" + proveedor + "', " + nofactura + ", '" + monedapa + "', '" + formapa + "',  '" + metodopa + "',  " + importesalins + ",  " + importepag + ", " + parcialidad + ")";
                            drp.Database.ExecuteSqlCommand(comando2);

                            // Saldos Factura Proveedor
                            EncfacturaProvContext ef = new EncfacturaProvContext();
                            ClsDatoDecimal totalf = ef.Database.SqlQuery<ClsDatoDecimal>("select Total as Dato from dbo.EncFacturaProv where uuid = '" + iddocto + "'").ToList()[0];
                            ClsDatoEntero idf = ef.Database.SqlQuery<ClsDatoEntero>("select ID as Dato from dbo.EncFacturaProv where uuid = '" + iddocto + "'").ToList()[0];


                            SaldoFacturaProvContext sfp = new SaldoFacturaProvContext();
                            try
                            {
                                SaldoFacturaProv pagobd = sfp.Database.SqlQuery<SaldoFacturaProv>("select * from SaldoFacturaProv where IDFacturaProv='" + idf.Dato + "'").ToList()[0];
                            /// si la consulta no devolvio fila lanazara una excepcion 

                           
                                int idpagofactura = pagobd.IDFacturaProv;
                            }
                            catch (Exception err)
                            {
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
                    ViewBag.Mensajeerror = "Hay un problema al aplicar pagos";
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }

            catch (Exception ERR2)
            {
                ViewBag.Mensajeerror = "Este archivo Xml no contiene una cadena de pago valido";
                return View();
            }

        }

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
        public ActionResult SubirArchivo()
        {
            return View();
        }

        /////////////////////// Boton Subir PDF ///////////////////////
        /////////////////////////////////////////////////////////////////

        [HttpPost]
        public ActionResult SubirArchivo(HttpPostedFileBase file)
        {
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            if (file != null)
            {
                string ruta = Server.MapPath("../Temp/");
                ruta += file.FileName;
                modelo.SubirArchivo(ruta, file);
                ViewBag.Error = modelo.error;
                ViewBag.Corrcto = modelo.Confirmacion;

            }
            return View();
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
            var elemento = db.PagoFacturasProv.Single(m => m.IDPagoFacturaProv == id);
            if (elemento == null)
            {
                return NotFound();
            }


            List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select IDPagoFacturaSPEIProv, IDPagoFacturaProv, CertificadoPago, IDTipoCadenaPago, SelloPago, Estado from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;

            VDocumentoRProvContext docto = new VDocumentoRProvContext();
            List<VDocumentoRProv> detallesDR = docto.Database.SqlQuery<VDocumentoRProv>("select IDDocumentoRelacionadoProv, IDPagoFacturaProv, IDProveedor, Empresa, Numero, ClaveDivisa, Divisa,ClaveFormaPago, FormaPago, ClaveMetododepago, MetodoPAgo, ImportePagado,ImporteSaldoInsoluto, NoParcialidad, Estado from [dbo].[VDocumentoRProv] where IDPagoFacturaProv='" + id + "'").ToList();
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
            PagoFacturaProvContext pprov = new PagoFacturaProvContext();
            var elemento = pprov.PagoFacturasProv.Single(m => m.IDPagoFacturaProv == id);
            if (elemento == null)
            {
                return NotFound();
            }

            PagoFacturaSPEIProvContext spei = new PagoFacturaSPEIProvContext();
            List<PagoFacturaSPEIProv> pagoFacturaSPEI = spei.Database.SqlQuery<PagoFacturaSPEIProv>("select IDPagoFacturaSPEI, IDPagoFactura, CertificadoPago, IDTipoCadenaPago, SelloPago, StatusPago from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList();
            ViewBag.pagoFacturaSPEI = pagoFacturaSPEI;

            VDocumentoRProvContext docto = new VDocumentoRProvContext();
            List<VDocumentoRProv> detallesDR = docto.Database.SqlQuery<VDocumentoRProv>("select IDPagoFacturaProv,IDProveedor, Empresa, Numero, ClaveDivisa, Divisa, ClaveFormaPago, FormaPago, ClaveMetododepago, MetodoPago,  ImportePagado,ImporteSaldoInsoluto, NoParcialidad, Estado from [dbo].[VDocumentoRProv]  where IDPagFacturaProv='" + id + "'").ToList();
            ViewBag.detallesDR = detallesDR;

            return View(elemento);

        }

        /////////////////////// Cancelar Pagos y sus relaciones ///////////////////////
        //////////////////////////////////////////////////////////////////////////////
        public ActionResult CancelaPagoProv(int id, string uuid)
        {

            CancelaPagoProv pag = new CancelaPagoProv();
            List<VPagoFacturaProv> detallesP = dbv.Database.SqlQuery<VPagoFacturaProv>("select IDPagoFacturaProv, FechaPago,Empresa, Monto, ClaveMoneda, UUID from VPagoFacturaProv where IDPagoFacturaProv =" + id + " ").ToList();
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

            List<DocumentoRelacionadoProv> doctos = drp.Database.SqlQuery<DocumentoRelacionadoProv>("select * from DocumentoRelacionadoProv where IDPagoFacturaProv =" + id + " ").ToList();
            foreach (var docto in doctos)
            {
                string prov = docto.Empresa;
                int num = docto.Numero;
                decimal imp = docto.ImportePagado;

                SaldoFacturaProvContext sf = new SaldoFacturaProvContext();
                SaldoFacturaProv saldos = sf.Database.SqlQuery<SaldoFacturaProv>("select * from SaldoFacturaProv where Empresa = '" + prov + "' and Numero = " + num + " ").ToList()[0];
                if (saldos != null)
                {
                    decimal imppagact = 0;
                    decimal imppag = saldos.ImportePagado;
                    imppagact = imppag - imp;
                    decimal impSalins = saldos.ImporteSaldoAnterior;
                    if (imppagact < 0)
                    {
                        imppagact = 0;
                    }
                    if (impSalins < 0)
                    {
                        impSalins = 0;
                    }
                    string querySQL5 = "update dbo.SaldoFacturaProv set ImportePagado= " + imppagact + ", ImporteSaldoInsoluto= " + impSalins + " where Empresa ='" + prov + "' and Numero =" + num + " ";
                    sf.Database.ExecuteSqlCommand(querySQL5);

                    imppag = 0;
                    imppagact = 0;
                    impSalins = 0;

                }
                //string uuid = pag.UUID;
                EncfacturaProvContext encf = new EncfacturaProvContext();
                string querySQL1 = "update dbo.EncFacturaProv set Pagada= '0' where Nombre_Proveedor = '" + prov + "' and Numero = " + num + "";
                encf.Database.ExecuteSqlCommand(querySQL1);

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
            List<c_Moneda> monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='"+ id+"'").ToList();
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
                string cadenasql = " select * from VBcoProveedor where idproveedor= CAST('" + id + "' as int) order by Nombre";

                var datoExiste = (from x in bancop.VBcoProveedores
                                  where x.IDProveedor == id
                                  orderby x.Nombre
                                  select x);

                listbanco.Add(new SelectListItem { Text = "--Selecciona un banco--", Value = "0" });
                if (datoExiste.ToList().Count() == 0)
                {
                   
                    var bancos = banco.c_Bancos.ToList();
                    //List<SelectListItem> listbancop = new List<SelectListItem>();
                    foreach (var x in bancos)
                    {
                        listbanco.Add(new SelectListItem { Text = x.Nombre, Value = x.RFC.ToString() });
                    }
                }
                else
                {
                    //listbanco.Add(new SelectListItem { Text = "--Selecciona un banco del proveedor--", Value = "0" });
                    //listbanco.AddRange(datoExiste, "Value", "TEXT", null);
                    foreach (var x in datoExiste)
                    {
                        listbanco.Add(new SelectListItem { Text = x.ID + " | " + x.Nombre + " | " + x.Cuenta + " | " + x.ClaveMoneda, Value = x.RFC.ToString()});
                    }
                }
                ViewBag.listbanco = new SelectList(listbanco);

            }
            catch (Exception er)
            {
                var bancos = banco.c_Bancos.ToList();
                listbanco.Add(new SelectListItem { Text = "--Selecciona un banco--", Value = "0" });
                foreach (var x in bancos)
                {
                    listbanco.Add(new SelectListItem { Text = x.Nombre, Value = x.RFC.ToString() });
                }
               
            }
            return Json(new SelectList(listbanco, "Value", "Text", JsonRequestBehavior.AllowGet));

        } // Fin GetBancoProveedor

        // GetcuentaBancoProveedor
        public ActionResult getcuentabancoprov(string datos )
        {

            //VBcoProveedorContext bancop = new VBcoProveedorContext();
           
            //List<VBcoProveedor> cuenta = db.Database.SqlQuery<VBcoProveedor>("select * from VBcoProveedor WHERE RFC='" + rfc+"' and idproveedor = "+id+" ").ToList();
            //ViewBag.datoscuenta = cuenta;

            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<SelectListItem> GetProveedor()
        {
            PagoFacturaProv elemento = new PagoFacturaProv();
            var listaProveedor = new ProveedorAllRepository().GetProveedor();
            ViewBag.datosProvedor = listaProveedor;

            return listaProveedor;
        }
        public IEnumerable<SelectListItem> GetBancoEmpresa()
        {
            PagoFactura elemento = new PagoFactura();
            var listaBancoEmp = new BancoEmpresaRepository().GetBancoEmpresa();
            ViewBag.datosBancoEmp = listaBancoEmp;
            return ViewBag.datosBancoEmp;
        }

        ////////////////////////////////////////////////PAGAR FACTURA ELECTRONICA//////////////////////////////////////
        public ActionResult PagarFacturaProv()
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

            c_FormaPagoContext formap = new c_FormaPagoContext();
            var formapago = formap.c_FormaPagos.ToList();
            List<SelectListItem> listafp = new List<SelectListItem>();
            listafp.Add(new SelectListItem { Text = "--Selecciona Forma Pago--", Value = "0" });

            foreach (var f in formapago)
            {
                listafp.Add(new SelectListItem { Text = f.ClaveFormaPago + " | " + f.Descripcion, Value = f.ClaveFormaPago.ToString() });
                ViewBag.datosFormaPago = listafp;
            }

                c_MonedaContext moneda = new c_MonedaContext();
            var monedas = moneda.c_Monedas.ToList();
            List<SelectListItem> listamoneda = new List<SelectListItem>();
            listamoneda.Add(new SelectListItem { Text = "--Selecciona Moneda--", Value = "0" });

            foreach (var m in monedas)
            {
                listamoneda.Add(new SelectListItem { Text =m.ClaveMoneda + " | " + m.Descripcion, Value = m.ClaveMoneda.ToString() });
                ViewBag.datosMoneda = listamoneda;

            }

            elemento.FechaPago = DateTime.Now;

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

           

            if (ModelState.IsValid)
            {
                int nuevofolio = 0;
                DateTime fechareq = PagoFacturaProv.FechaPago;
                string fecha2 = fechareq.ToString("yyyy/MM/dd HH:mm:ss");
                try
                {
                    ClsDatoEntero foliop = db.Database.SqlQuery<ClsDatoEntero>("Select max(FolioP)+1 from[dbo].[PagoFacturaProv]").ToList()[0];
                     nuevofolio = foliop.Dato;
                }
                catch (Exception ex)
                {
                    nuevofolio = 1;
                }
                    ProveedorContext p = new ProveedorContext();
                int idp = int.Parse(PagoFacturaProv.IDProveedor.ToString());
                ClsDatoString empresa = db.Database.SqlQuery<ClsDatoString>("Select Empresa as Dato from[dbo].[Proveedores] where IDProveedor = "+idp+"").ToList()[0];
                //VBcoEmpresaContext be = new VBcoEmpresaContext();
                //VBcoEmpresaContext be = new VBcoEmpresaContext();

                    ClsDatoString rfcempresa = db.Database.SqlQuery<ClsDatoString>("Select RFC as Dato from dbo.c_Banco where IDBanco= '" + PagoFacturaProv.RFCBancoEmpresa + "'").ToList()[0];
                try
                {
                    ClsDatoString cuenta = db.Database.SqlQuery<ClsDatoString>(" Select CuentaBanco as Dato from dbo.VBancoEmpresa where RFC= '" + rfcempresa.Dato + "'").ToList()[0];
                    string comando1 = "insert into [dbo].[PagoFacturaProv](SerieP,FolioP, IDProveedor,Empresa, FechaPago, ClaveFormaPago, ClaveMoneda, NoOperacion, Monto,RFCBancoEmpresa, CuentaEmpresa, RFCBancoProv, CuentaProv, IDTipoCadenaPago, Observacion,Estado, UUID, RutaXML, RutaPDF) values('P', " + nuevofolio + ",'" + PagoFacturaProv.IDProveedor + "', '" + empresa.Dato + "', Convert (datetime,'" + fecha2 + "',101), '" + PagoFacturaProv.ClaveFormaPago + "','" + PagoFacturaProv.ClaveMoneda + "', " + PagoFacturaProv.NoOperacion + ", " + PagoFacturaProv.Monto + ", '" + rfcempresa.Dato + "', '" + cuenta.Dato + "', '" + PagoFacturaProv.RFCBancoProv + "', '" + PagoFacturaProv.CuentaProv + "', '" + PagoFacturaProv.IDTipoCadenaPago + "', '" + PagoFacturaProv.Observacion + "',0, '', '', '')";
                    db.Database.ExecuteSqlCommand(comando1);
                }
                catch(Exception ex)
                {
                    string comando2 = "insert into [dbo].[PagoFacturaProv](SerieP,FolioP, IDProveedor,Empresa, FechaPago, ClaveFormaPago, ClaveMoneda, NoOperacion, Monto,RFCBancoEmpresa, CuentaEmpresa, RFCBancoProv, CuentaProv, IDTipoCadenaPago, Observacion,Estado, UUID, RutaXML, RutaPDF) values('P', " + nuevofolio + ",'" + PagoFacturaProv.IDProveedor + "', '" + empresa.Dato + "', Convert (datetime,'" + fecha2 + "',101), '" + PagoFacturaProv.ClaveFormaPago + "','" + PagoFacturaProv.ClaveMoneda + "', " + PagoFacturaProv.NoOperacion + ", " + PagoFacturaProv.Monto + ", '" + rfcempresa.Dato + "', 'CtaNoDefinida', '" + PagoFacturaProv.RFCBancoProv + "', '" + PagoFacturaProv.CuentaProv + "', '" + PagoFacturaProv.IDTipoCadenaPago + "', '" + PagoFacturaProv.Observacion + "',0, '', '', '')";
                    db.Database.ExecuteSqlCommand(comando2);
                }

                }
            return RedirectToAction("Index");
        }// PagarFacturaProv() Post

        ////////////////////////////////////////////////PAGO FACTURA EFECTIVO//////////////////////////////////////
        public ActionResult PagarFacturaProvE()
        {
            PagoFacturaProv elemento = new PagoFacturaProv();
            var proveedor = prov.Proveedores.ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona un Proveedor--", Value = "0" });

            foreach (var m in proveedor)
            {
                li.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });
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
            elemento.FechaPago = DateTime.Now;
            var listaFormaPago = new FormaPagoRepository().GetFormasdepagoOtra();
            ViewBag.datosFormaPago = listaFormaPago;
            return View(elemento);

        } //PagarFacturaProvE()
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagarFacturaProvE(PagoFacturaProv PagoFacturaProv)
        {
            if (ModelState.IsValid)
            {
                DateTime fechareq = PagoFacturaProv.FechaPago;
                string fecha2 = fechareq.ToString("yyyy/MM/dd HH:mm:ss");
                ClsDatoEntero foliop = db.Database.SqlQuery<ClsDatoEntero>("Select max(FolioP)+1 from[dbo].[PagoFacturaProv]").ToList()[0];


                string comando = "insert into[dbo].[PagoFacturaProv](SerieP, FolioP, IDProveedor, Empresa, FechaPago, ClaveFormaPago, ClaveMoneda, NoOperacion, Monto, IDTipoCadenaPago, Observacion, Estado, UUID, RutaXML, RutaPDF) values('P', " + foliop + ", '" + PagoFacturaProv.IDProveedor + "', Convert(datetime, '" + fecha2 + "', 101), '" + PagoFacturaProv.ClaveFormaPago + "', '" + PagoFacturaProv.ClaveMoneda + "', " + 0 + ", " + PagoFacturaProv.Monto + ", '00', '" + PagoFacturaProv.Observacion + "', 0, '" + PagoFacturaProv.UUID + "', '" + PagoFacturaProv.RutaXML + "', '" + PagoFacturaProv.RutaPDF + "')";
                db.Database.ExecuteSqlCommand(comando);

            }
            return RedirectToAction("Index");

        } //PagarFacturaProvE()


        public void FunctionFile(HttpPostedFileBase file)
        {
            try
            {
                string result = new StreamReader(file.InputStream).ReadToEnd();
                ViewBag.result = result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }// FunctionFile


        /////////////////////////////////////////////LISTA PAGO FACTURA//////////////////////////////////////


        private static List<VPagoFacturaProvT> GetPagoElectronico()

        {
            VPagoFacturaProvTContext dbpf = new VPagoFacturaProvTContext();
            List<VPagoFacturaProvT> pagoFactura = dbpf.Database.SqlQuery<VPagoFacturaProvT>("SELECT * FROM [dbo].[VPagoFacturaProvT] order by FechaPago desc").ToList();
            return pagoFactura.ToList();
        }
        private static List<VPagoFacturaEfeProv> GetPagoEfectivo()
        {
              VPagoFacturaEfeProvContext db1 = new VPagoFacturaEfeProvContext();
            List<VPagoFacturaEfeProv> pagoFacturaEfe = db1.Database.SqlQuery<VPagoFacturaEfeProv>("SELECT [ID],[IDPagoFacturaprov],[FechaPago],SerieP, FolioP, [IDProveedor],[Empresa],[RFC],[ClaveFormaPago],[Descripcion],[ClaveDivisa],[Divisa],[Monto],[Observacion],[Estado], UUID, RutaXML, RutaPDF FROM [dbo].VPagoFacturaEfeProv order by FechaPago desc").ToList();

            return pagoFacturaEfe.ToList();
        } //Index


        ////////////////////////////////////////Documento Relacionado////////////////////////////////////
        private DocumentoRelacionadoContext dbDocto = new DocumentoRelacionadoContext();



        public ActionResult DocumentoRelacionadoProv(string idproveedor, int idp, int idpf, decimal monto)
        {

            ViewBag.idproveedor = idproveedor;
            ProveedorContext p = new ProveedorContext();
            Proveedor nombrep= p.Proveedores.Find(idp);
            ViewBag.nombrep = nombrep;
            ViewBag.idp = idp;
            ViewBag.idpf = idpf;
            ViewBag.monto = monto;
            VEncFacturaProvContext enc = new VEncFacturaProvContext();

            List<VEncFacturaProv> encfactura = enc.Database.SqlQuery<VEncFacturaProv>("SELECT  ID, Serie, Numero, IDProveedor, Nombre_Proveedor, TC, IDMoneda, Moneda, Metododepago, Subtotal, IVA, Total, ImporteSaldoAnterior , ImportePagado, ImporteSaldoInsoluto, NoParcialidad from dbo.VEncFacturaProv  where IDProveedor= '" + idproveedor + "' ").ToList();
            ViewBag.EncFactura = encfactura;
            var resumen = db.Database.SqlQuery<VEncFacturaProv>("Select Moneda, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from [dbo].[VEncFacturaProv] where IDProveedor= '" + idproveedor + "' group by Moneda").ToList();
           
            ViewBag.sumatoria = resumen;
            return View(encfactura);
        }

        [HttpPost]
        public ActionResult Ejecutar(string idproveedor, int idpf, int idp, decimal monto, List<VEncFacturaProv> cr)
        {

            decimal sumatoria = 0, importesi = 0, ImporteSaldoInsoluto = 0;
            int noparcialidad = 0, contador = 0;
            ProveedorContext p = new ProveedorContext();
            Proveedor nombrep = p.Proveedores.Find(idp);
            ViewBag.nombrep = nombrep;

            //Verificar la sumatoria del importe pagado
            foreach (var a in cr)
            {
                sumatoria += a.ImportePagado;

            }

            //Verificar que el importe pagado cumpla con las condiciones

            if (monto >= sumatoria)
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
                            ClsDatoEntero parcialidad = db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(NoParcialidad), 0) AS INT) as Dato from dbo.DocumentoRelacionadoProv where IDFacturaProv='" + i.ID + "'").ToList()[0];
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
                            
                            db.Database.ExecuteSqlCommand("INSERT INTO DocumentoRelacionadoProv([IDPagoFacturaProv],[IDProveedor],[IDFactura],[Empresa],[Serie],[Numero],[ClaveMoneda],[ClaveFormaPago],[ClaveMetododepago],[ImporteSaldoInsoluto],[ImportePagado],[NoParcialidad], Estado) values('" + idpf + "','" + idp + "', '" + i.ID + "','" + i.Nombre_Proveedor + "', '" + i.Serie + "','" + i.Numero + "','" + i.ClaveMoneda + "', '"+ i.ClaveFormaPago + "', '"+i.ClaveMetodoPago + "','" + ImporteSaldoInsoluto + "','" + i.ImportePagado + "','" + noparcialidad + "', 'A')");
                            db.Database.ExecuteSqlCommand("update dbo.EncFacturasProv set ConPagos='true' where ID=" + i.ID + " and Serie='" + i.Serie + "' and Numero= " + i.Numero + " ");

                            ClsDatoEntero numero = db.Database.SqlQuery<ClsDatoEntero>("select count(IDFacturaProv) as Dato from SaldoFacturaProv where IDFacturaProv='" + i.ID + "'").ToList()[0];
                            int num = numero.Dato;
                            if (num != 0)
                            {

                                
                                db.Database.ExecuteSqlCommand("update SaldoFacturaProv set ImporteSaldoAnterior='" + i.ImporteSaldoInsoluto + "',ImportePagado='" + i.ImportePagado + "',ImporteSaldoInsoluto='" + ImporteSaldoInsoluto + "' where IDFacturaProv='" + i.ID + "' and Serie ='" + i.Serie + "' and Numero= " + i.Numero + "");

                            }
                            else
                            {
                                db.Database.ExecuteSqlCommand("INSERT INTO SaldoFacturaProv([IDFacturaProv], [Serie], [Numero],[Total],[ImporteSaldoAnterior],[ImportePagado],[ImporteSaldoInsoluto]) values (" + i.ID + ",'" + i.Serie + "', " + i.Numero + ", " + i.Total + "," + i.Total + "," + i.ImportePagado + ", " + ImporteSaldoInsoluto + ")");
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
                            
                            db.Database.ExecuteSqlCommand("update EncFacturasProv set pagada= 1 where ID='" + i.ID + "'");
                            db.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set conPagos='true' where ID=" + i.ID + " and Serie='" + i.Serie + "' and Numero= " + i.Numero + " ");
                        }


                    }
                    if (sumatoria == monto)
                    { //Aqui voy
                        db.Database.ExecuteSqlCommand("update PagoFacturaProv set Estado= 1 where IDPagoFactura='" + idpf + "'");
                        db.Database.ExecuteSqlCommand("update dbo.EncFacturaProv set ConPagos='true' where ID=" + idpf + "");

                    }
                }

                else
                {

                    ViewBag.nombrep = nombrep;
                    ViewBag.idp = idproveedor;
                    ViewBag.idpf = idpf;
                    ViewBag.monto = monto;
                    List<VEncFactura> encfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrep + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, D.MetodoPago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();
                    ViewBag.EncFactura = encfactura;
                    var resumen = db.Database.SqlQuery<VEncFacturaProv>("Select Moneda, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from [dbo].[VEncFacturaProv] where IDProveedor= '" + idproveedor + "' group by Moneda").ToList();

                    ViewBag.sumatoria = resumen;
                    ViewBag.mensaje = "La cantidad del importe pagado excede el Importe del Saldo Insoluto, verifique por favor";
                    return View("DocumentoR", encfactura);
                }
            }
            else
            {


                ViewBag.nombrep = nombrep;
                ViewBag.idc = idp;
                ViewBag.idpf = idpf;
                ViewBag.monto = monto;
                List<VEncFacturaProv> encfactura = db.Database.SqlQuery<VEncFacturaProv>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and Nombre_Cliente ='" + nombrep + "' group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, D.MetodoPago,E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList();
                ViewBag.EncFactura = encfactura;
                var resumen = db.Database.SqlQuery<VEncFacturaProv>("Select Moneda, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from [dbo].[VEncFacturaProv] where IDProveedor= '" + idproveedor + "' group by Moneda").ToList();
                ViewBag.sumatoria = resumen;
                ViewBag.me3nsaje = " El importe de los pagos excede el Monto Total a Pagar, verifique por favor";
                return View("DocumentoR", encfactura);
            }



            return RedirectToAction("Index");
        }

        ////////////////////////////////////////////////////////SPEI//////////////////////////////////////

        private PagoFacturaSPEIProvContext dbSPEI = new PagoFacturaSPEIProvContext();

        public ActionResult PagoFacturaSPEIProv(int id)
        {
            PagoFacturaSPEIProv pagoFacturaSPEI = null;
            PagoFacturaSPEIProv elemento = new PagoFacturaSPEIProv();
            if (id != null)
            {
                System.Web.HttpContext.Current.Session["IDPagoFacturaProv"] = id;
                try
                {
                    pagoFacturaSPEI = db.Database.SqlQuery<PagoFacturaSPEIProv>("select IDPagoFacturaSPEIProv, IDPagoFacturaProv, CertificadoPago, IDTipoCadenaPago, SelloPago from [dbo].[PagoFacturaSPEIProv] where IDPagoFacturaProv='" + id + "'").ToList()[0];


                }
                catch (Exception err)
                {
                    string error = err.Message;
                    elemento.IDPagoFacturaProv = id;
                }
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

            if (ModelState.IsValid) // si llenaron los datos como los validadores del sistema intento grabar
            {

                if (elemento.IDPagoFacturaSPEIProv != 0) // si ya venia con un dato de IDPagoFactura
                {

                    db.Database.ExecuteSqlCommand("update [dbo].[PagoFacturaSPEIProv] set CertificadoPago='" + elemento.CertificadoPago + "', IDTipoCadenaPago='" + elemento.IDTipoCadenaPago + "', SelloPago='" + elemento.SelloPago + "' where IDPagoFacturaProv=" + id);
                }
                else
                {
                    dbSPEI.PagoFacturaSPEIsProv.Add(elemento);
                    dbSPEI.SaveChanges();

                }



                return RedirectToAction("Index");

            }
            else // si no es valido lo regreso
            {
                return View();
            }


        }
        public void DescargarPDF(int id)
        {
            // Obtener contenido del archivo

            PagoFacturaProvContext p = new PagoFacturaProvContext();
            PagoFacturaProv elemento = p.PagoFacturasProv.Find(id);


            EmpresaContext dbe = new EmpresaContext();
            var empresa = dbe.empresas.Single(m => m.IDEmpresa == id);


            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            PDFPago pago = new PDFPago(elemento.RutaXML, logoempresa,id, "(55) 262 04200");

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
            EliminaPagoEfe pag = new EliminaPagoEfe();

            PagoFacturaProvContext dbe = new PagoFacturaProvContext();
            List<PagoFacturaProv> detallesP = dbe.Database.SqlQuery<PagoFacturaProv>("select IDPagoFactura, FechaPago,Nombre, Monto, ClaveDivisa from PagoFacturaProv where IDPagoFactura ='" + id + "'").ToList();
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
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult EliminaPagoProv(EliminaPagoEfe pag)
        {
            int id = pag.IDPagoFactura;
            PagoFacturaProvContext dbe = new PagoFacturaProvContext();
            List<PagoFacturaProv> detallesP = dbe.Database.SqlQuery<PagoFacturaProv>("select IDPagoFacturaProv, FechaPago,Empresa, Monto, ClaveMoneda from PagoFacturaProv where IDPagoFacturaProv ='" + id + "'").ToList();
            ViewBag.detallesPEfe = detallesP;

            //Actualiza la tabla PagoFactura
            PagoFacturaProv datoC = db.PagoFacturasProv.Single(m => m.IDPagoFacturaProv == id);
            db.Database.ExecuteSqlCommand("delete from dbo.PagoFacturaProv where IDPagoFacturaProv = " + id + " ");

            return RedirectToAction("Index");
        }

    }//Public class
}//name space