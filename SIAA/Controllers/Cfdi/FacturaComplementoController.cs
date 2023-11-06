using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Cfdi;
using System.IO;
using System.Text;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.Models.Administracion;
using SIAAPI.Facturas;

using Generador;
using System.Collections.Generic;
using PagedList;
using SIAAPI.Models.Login;

namespace SIAAPI.Controllers
{

    [Authorize(Roles = "Administrador,Facturacion,Ventas,Sistemas,Almacenista,Comercial")]
    public class FacturaComplementoController : Controller
    {
        EncfacturaContext db = new EncfacturaContext();
        
        // GET: FacturaComplemento
        public ActionResult Index(string Numero, string SerieFac, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString, string FacPag)
        {
            
            //Buscar Facturas: Pagadas o no pagadas
            var FacPagLst = new List<SelectListItem>();
            //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });

            FacPagLst.Add(new SelectListItem { Text = "Si", Value = "Si" });
            FacPagLst.Add(new SelectListItem { Text = "No", Value = "No" });

            ViewData["FactPag"] = FacPagLst;
            ViewBag.FacPag = new SelectList(FacPagLst, "Value", "Text");
            //ViewBag.FacPag = new SelectList(FacPagLst);

          



            //Buscar Serie Factura
            var SerLst = new List<string>();
            var SerQry = from d in db.encfacturas
                         orderby d.Serie
                         select d.Serie;

            SerLst.AddRange(SerQry.Distinct());
            ViewBag.SerieFac = new SelectList(SerLst);
            ViewBag.sumatoria = "";

            //Buscar Cliente
            var ClieLst = new List<string>();
            var ClieQry = from d in db.encfacturas
                          orderby d.Nombre_cliente
                          select d.Nombre_cliente;
            ClieLst.AddRange(ClieQry.Distinct());
            ViewBag.ClieFac = new SelectList(ClieLst);




            string ConsultaSql = "select top 200 * from Encfacturas";
            string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from encfacturas ";
            string ConsultaAgrupado = "group by Moneda order by Moneda ";
            string Filtro = "where Estado='A'" ;
            string Orden = " order by fecha desc , serie , numero desc ";

            ///tabla filtro: serie
            if (!String.IsNullOrEmpty(SerieFac))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where Serie='" + SerieFac + "'";
                }
                else
                {
                    Filtro += "and  Serie='" + SerieFac + "'";
                }

            }
            //Buscar por numero
            if (!String.IsNullOrEmpty(Numero))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where Numero=" + Numero + "";
                }
                else
                {
                    Filtro += "and  Numero=" + Numero + "";
                }

            }

            ///tabla filtro: Nombre Cliente
            if (!String.IsNullOrEmpty(ClieFac))
            {

                if (Filtro == string.Empty)
                {
                    Filtro = "where Nombre_cliente='" + ClieFac + "'";
                }
                else
                {
                    Filtro += "and  Nombre_cliente='" + ClieFac + "'";
                }

            }

            ///tabla filtro: Factura pagada/no pagada && Serie, Nombre Cliente
            if (FacPag != "Todas")
            {
                if (FacPag == "Si")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where pagada='1' ";
                    }
                    else
                    {
                        Filtro += "and  pagada='1' ";
                    }
                }
                if (FacPag == "NO")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where pagada='0' ";
                    }
                    else
                    {
                        Filtro += "and  pagada='0' ";
                    }
                }
            }


          


            if (!String.IsNullOrEmpty(searchString)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where  Fecha BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and Fecha BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999'";
                }
            }


            ViewBag.CurrentSort = sortOrder;
            ViewBag.SerieSortParm = String.IsNullOrEmpty(sortOrder) ? "Serie" : "";
            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "Numero" : "";
            ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
            ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre_Cliente" : "";


            // Not sure here
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
                case "Serie":
                    Orden = " order by  serie , numero desc ";
                    break;
                case "Numero":
                    Orden = " order by   numero asc ";
                    break;
                case "Fecha":
                    Orden = " order by fecha ";
                    break;
                case "Nombre_Cliente":
                    Orden = " order by  Nombre_cliente ";
                    break;
                default:
                    Orden = " order by fecha desc , serie , numero desc ";
                    break;
            }

            //var elementos = from s in db.encfacturas
            //select s;
            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

            var elementos = db.Database.SqlQuery<Encfacturas>(cadenaSQl).ToList();



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


        public ActionResult CreatedesdeArchivo()
        {
            return View();
        }


      
        public FileResult Descargarxml(int id)
        {
            // Obtener contenido del archivo
            var elemento = db.encfacturas.Single(m => m.ID == id);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(elemento.RutaXML));

            return File(stream, "text/plain", "Factura" + elemento.Serie + elemento.Numero + ".xml");
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
                documento = new Generador.CreaPDF(xmlString, logoempresa, "Tel. " + empresa.Telefono + "  www.class-label.com", true);

                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            else
            {
                EncPrefactura prefact = prefactura[0];
                documento = new Generador.CreaPDF(xmlString, logoempresa, prefact, "Tel. " + empresa.Telefono + " www.class-label.com", true);

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


       


        public ActionResult CrearComplemento(int id)
        {
            var elemento = db.encfacturas.Single(m => m.ID == id);
            ViewBag.Cliente = elemento.Nombre_cliente;
            ViewBag.Monto = elemento.Total;
            ViewComplento elementoacomplementar = new ViewComplento();
            elementoacomplementar.UUID = elemento.UUID;
            elementoacomplementar.idEncFac = id;
            var listauso = new UsoCfdiRepository().GetusosCfdi();
            var listaTipoRelacion = new TipoRelacionRepository().GetTiposderelacion();
            var listametodopago = new MetodoPagoRepository().GetMetodosdepago();
            var listatipocomprobante = new TipoComprobanteRepository().GetTiposComprobante();
            var listaformapago = new FormaPagoRepository().GetFormasdepago();
            var listaserie = new FoliosRepository().GetFolios();
            ViewBag.Mensaje = "";

            ViewBag.listauso = listauso;
            ViewBag.listaTipoRelacion = listaTipoRelacion;
            ViewBag.listametodopago = new MetodoPagoRepository().GetMetodosdepago();
            ViewBag.listatipocomprobante = listatipocomprobante;
            ViewBag.listaformapago = listaformapago;
            ViewBag.listaserie = listaserie;

            return View(elementoacomplementar);
        }


        /// <summary>
        /// Crea la factura complemento
        /// </summary>
        /// <param name="elemento"> son los datos del formulario</param>
        /// <returns></returns>


        [HttpPost]
        public ActionResult CrearComplemento(ViewComplento elemento)
        {
            Folio dbf = new Models.Cfdi.Folio();
            var facturaoriginal = db.encfacturas.Single(m => m.ID == elemento.idEncFac);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(facturaoriginal.RutaXML));

            if (!ModelState.IsValid)
            {
                var listauso = new UsoCfdiRepository().GetusosCfdi();
                var listaTipoRelacion = new TipoRelacionRepository().GetTiposderelacion();
                var listametodopago = new MetodoPagoRepository().GetMetodosdepago();
                var listatipocomprobante = new TipoComprobanteRepository().GetTiposComprobante();
                var listaformapago = new FormaPagoRepository().GetFormasdepago();
                var listaserie = new FoliosRepository().GetFolios();

                ViewBag.listauso = listauso;
                ViewBag.listaTipoRelacion = listaTipoRelacion;
                ViewBag.listametodopago = new MetodoPagoRepository().GetMetodosdepago();
                ViewBag.listatipocomprobante = listatipocomprobante;
                ViewBag.listaformapago = listaformapago;
                ViewBag.listaserie = listaserie;
                ViewBag.Mensaje = "";
                return View(elemento);
            }

            Generador.CreaPDF temp = new Generador.CreaPDF(StreamToString(stream));

            ClsFactura factura = new ClsFactura();

            try
            {
                factura.cfdirelacionados.relacion = db.relaciones.Find(elemento.IDTipoRelacion).ClaveTipoRelacion.Trim();//;
                factura.formadepago = db.FormaPagos.Find(elemento.IDFormaPago).ClaveFormaPago;
                factura.metododepago = db.metodopagos.Find(elemento.IDMetodoPago).ClaveMetodoPago;
                factura.tipodecambio = "1";
                factura.Tipodecombrobante = db.tipocomprobantes.Find(elemento.IDTipoComprobante).ClaveTipoComprobante;
                factura.uso = db.c_UsoCFDIs.Find(elemento.IDUsoCFDI).ClaveCFDI;
                factura.Moneda = facturaoriginal.c_Moneda.ClaveMoneda;
               

                factura.cfdirelacionados.uuid.Add(elemento.UUID);

                dbf = db.Folios.Find(elemento.serie);
                factura._serie = dbf.Serie;
                factura._folio = (dbf.Numero + 1).ToString();
          


                Empresa emi = db.Empresa.Find(2);
                factura.Emisora = emi;
               factura.Regimen =emi.c_RegimenFiscal.ClaveRegimenFiscal.ToString();

                Empresa rec = new Empresa();
                rec.RFC = temp._templatePDF.receptor.rfc;
                rec.RazonSocial = temp._templatePDF.receptor.razonSocial;
                factura.Receptora = rec;

                int CONTADOR = 1;

                foreach (Generador.ProductoCFD x in temp._templatePDF.productos)
                {
                    Concepto concepto = new Concepto();
                    concepto.NoIdentificacion = (x.numIdentificacion== string.Empty) ? CONTADOR.ToString() : x.numIdentificacion;
                    concepto.ClaveUnidad = (factura.Tipodecombrobante == "E" && factura.cfdirelacionados.relacion == "01") ? "ACT" : x.c_unidad;


                    concepto.Unidad = (factura.Tipodecombrobante == "E" && factura.cfdirelacionados.relacion == "01") ? "ACTIVIDAD" : x.unidad;
                    concepto.Cantidad = decimal.Parse(x.cantidad);
                    concepto.ClaveProdServ = (factura.Tipodecombrobante == "E" && factura.cfdirelacionados.relacion == "01") ? "84111506" : x.ClaveProducto;

                    concepto.Descripcion = x.desc;


                    concepto.Descuento = decimal.Parse(x.descuento.ToString());
                    concepto.Importe = decimal.Parse(x.importe.ToString());
                    concepto.ValorUnitario = decimal.Parse(x.v_unitario.ToString());
                  


                    factura.Listaconceptos.conceptos.Add(concepto);
                    CONTADOR++;

                }
  

            }
            catch(Exception err)
            {
                string mensaje = err.Message;
                var listauso = new UsoCfdiRepository().GetusosCfdi();
                var listaTipoRelacion = new TipoRelacionRepository().GetTiposderelacion();
                var listametodopago = new MetodoPagoRepository().GetMetodosdepago();
                var listatipocomprobante = new TipoComprobanteRepository().GetTiposComprobante();
                var listaformapago = new FormaPagoRepository().GetFormasdepago();
                var listaserie = new FoliosRepository().GetFolios();

                ViewBag.listauso = listauso;
                ViewBag.listaTipoRelacion = listaTipoRelacion;
                ViewBag.listametodopago = new MetodoPagoRepository().GetMetodosdepago();
                ViewBag.listatipocomprobante = listatipocomprobante;
                ViewBag.listaformapago = listaformapago;
                ViewBag.listaserie = listaserie;
                ViewBag.Mensaje = "No se Puede Generar la factura ";
                return View(elemento);
            }

            //string strini = factura.construirfactura();
            //factura.EscribeEnArchivo(strini, System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"), true);
            //string ini = Convert.ToBase64String(System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini")));
            //WSmultifacturas.RespuestaWS respuesta = factura.timbrar(ini);
            //var reshtml = Server.HtmlEncode(respuesta.codigo_mf_texto + "->" + respuesta.cfdi);
            //if (respuesta.codigo_mf_texto.Contains("OK") )
            //{
            //    Generador.CreaPDF temp2 = new Generador.CreaPDF(respuesta.cfdi);



           MultiFacturasSDK.MFSDK sdk = factura.construirfactura2();
              MultiFacturasSDK.SDKRespuesta respuesta = factura.timbrar(sdk);

         //   MultiFacturasSDK.SDKRespuesta respuesta = null;


            var reshtml = Server.HtmlEncode(respuesta.Codigo_MF_Texto + "->" + respuesta.CFDI);
            if (respuesta.Codigo_MF_Texto.Contains("OK"))
            {
                Generador.CreaPDF temp2 = new Generador.CreaPDF(respuesta.CFDI);
                Encfacturas elemento2 = new Encfacturas();

                elemento2.Fecha = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                elemento2.Serie = temp2._templatePDF.serie;
                elemento2.Numero = Int32.Parse(temp2._templatePDF.folio);
                elemento2.Nombre_cliente = temp2._templatePDF.receptor.razonSocial;
                elemento2.Subtotal = decimal.Parse(temp2._templatePDF.subtotal.ToString());
                elemento2.Total = decimal.Parse(temp2._templatePDF.total.ToString());
                elemento2.IVA = elemento2.Total - elemento2.Subtotal;
                elemento2.Estado = "A";
                elemento2.Moneda = temp2._templatePDF.claveMoneda;
                elemento2.TC = Decimal.Parse(temp2._templatePDF.tipo_cambio);
                elemento2.pagada = false;


                int idcliente = 0;

                try
                {
                    ClsDatoEntero clientecapturado = db.Database.SqlQuery<ClsDatoEntero>("select IDCLIENTE AS Dato from clientes where NOMBRE='" + elemento2.Nombre_cliente + "'").ToList()[0];
                    idcliente = clientecapturado.Dato;
                    elemento2.IDCliente = idcliente;
                }

                catch (Exception err)
                {
                    string mensaje = err.Message;
                }

                List<c_Moneda> clavemoneda;

                elemento2.IDMetododepago = temp2._templatePDF.metodoPago;
                clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE claveMoneda='" + temp._templatePDF.claveMoneda + "'").ToList();
                int clave = clavemoneda.Select(s => s.IDMoneda).FirstOrDefault();
                elemento2.IDMoneda = clave;
                elemento2.RutaXML = respuesta.CFDI;

                elemento2.UUID = temp2._templatePDF.folioFiscalUUID;
                elemento2.IDMetododepago = temp2._templatePDF.metodoPago;
                elemento2.ConPagos = false;
                db.encfacturas.Add(elemento2);
                db.SaveChanges();

                try
                {
                   // Generador.CreaPDF temp3 = new Generador.CreaPDF(elemento2.RutaXML.ToString());
                    string fileName = temp2._templatePDF.folioFiscalUUID + ".xml";

                    EscribeEnArchivo(elemento2.RutaXML.ToString(), fileName, true);
                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;

                }



                db.Database.ExecuteSqlCommand("update folio set numero=" + elemento2.Numero + " where IDFolio=" + dbf.IDFolio);
                return RedirectToAction("Index");
            }
            else
            {
                

                return Content(reshtml);
            }


        }

        /// <summary>
        /// Convierte un stream en cadena
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public  string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }


        public ActionResult CrearAnticipo()
        {
            var listacliente = new ClienteRepository().GetClientes() ;

            ViewAnticipo elementoacomplementar = new ViewAnticipo();
        
            var listauso = new UsoCfdiRepository().GetusosCfdi();
         
         
         
            var listaformapago = new FormaPagoRepository().GetFormasdepago();
            var usoscfdi = new UsoCfdiRepository().GetusosCfdi();
            var listaserie = new FoliosRepository().GetFolios();
            ViewBag.Mensaje = "";

            ViewBag.listauso = listauso;


            ViewBag.listausoscfdi = usoscfdi;
            ViewBag.listaformapago = listaformapago;
            ViewBag.listaserie = listaserie;
            ViewBag.listaClientes = listacliente;
            ViewBag.IDMoneda = new SelectList(new c_MonedaContext().c_Monedas, "IDMoneda", "Descripcion");

            return View(elementoacomplementar);
        }



        [HttpPost]
        public ActionResult CrearAnticipo(ViewAnticipo elemento)
        {
    
            
            if (!ModelState.IsValid)
            {
                var listacliente = new ClienteRepository().GetClientes();


                var usoscfdi = new UsoCfdiRepository().GetusosCfdi();


                var listaformapago = new FormaPagoRepository().GetFormasdepago();
                var listaserie = new FoliosRepository().GetFolios();
                ViewBag.Mensaje = "No pude crear la factura anticipo";


                ViewBag.listausoscfdi = usoscfdi;
                ViewBag.listaformapago = listaformapago;
                ViewBag.listaserie = listaserie;
                ViewBag.listaClientes = listacliente;
                ViewBag.IDMoneda = new SelectList(new c_MonedaContext().c_Monedas, "IDMoneda", "Descripcion");

                return View(elemento);
            }
            else
            {
                ClsFactura factura = new ClsFactura();
                
                factura.formadepago = db.FormaPagos.Find(elemento.IDFormaPago).ClaveFormaPago;
                factura.metododepago = "PUE";




                factura.Tipodecombrobante = "I";
                string ClaveMoneda = new c_MonedaContext().c_Monedas.Find(elemento.IDMoneda).ClaveMoneda;

                factura.Moneda = ClaveMoneda;

                if (ClaveMoneda=="MXN")
                {

                    factura.tipodecambio = "1";
                }
                else
                {
                    string fecfac = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();

                    decimal tcd = 0;

                    try
                    {
                        tcd = db.Database.SqlQuery<ClsDatoDecimal>("select [dbo].[GetTipocambioCadena]('" + fecfac + "','USD', 'MXN') as Dato").ToList().FirstOrDefault().Dato;
                    }
                    catch (Exception err)
                    {

                    }
                    factura.tipodecambio = tcd.ToString();
                }

                string claveuso = new c_UsoCFDIContext().c_UsoCFDIS.Find(elemento.IDUsoCfdi).ClaveCFDI;

                factura.uso = claveuso;




                Folio dbf = new Models.Cfdi.Folio();
                dbf = db.Folios.Find(elemento.serie);
                factura._serie = dbf.Serie;
                factura._folio = (dbf.Numero + 1).ToString();



                Empresa emi = db.Empresa.Find(2);
                factura.Emisora = emi;
                factura.Regimen = emi.c_RegimenFiscal.ClaveRegimenFiscal.ToString();

                Empresa rec = new Empresa();
                Clientes clienteseleccionado = new ClientesContext().Clientes.Find(elemento.IDCliente);

                rec.RFC = clienteseleccionado.RFC;
                rec.RazonSocial = clienteseleccionado.Nombre;
                factura.Receptora = rec;

                Concepto concepto = new Concepto();
                concepto.NoIdentificacion = "1";
                concepto.ClaveUnidad = "ACT";
                concepto.Unidad = "ACTIVIDAD";
                concepto.Cantidad = 1;
                concepto.ClaveProdServ = "84111506";
                concepto.Descripcion ="Anticipo del bien o servicio";


                concepto.Descuento = 0;
                concepto.Importe = decimal.Parse(Math.Round( (elemento.Anticipo/1.16),2) .ToString());
                concepto.ValorUnitario = decimal.Parse(Math.Round((elemento.Anticipo / 1.16), 2).ToString());



                factura.Listaconceptos.conceptos.Add(concepto);

                //string strini = factura.construirfactura();
                //factura.EscribeEnArchivo(strini, System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"), true);
                //string ini = Convert.ToBase64String(System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini")));
                //WSmultifacturas.RespuestaWS respuesta = factura.timbrar(ini);

                MultiFacturasSDK.MFSDK sdk = factura.construirfactura2();
                MultiFacturasSDK.SDKRespuesta respuesta = factura.timbrar(sdk);

             


                var reshtml = Server.HtmlEncode(respuesta.Codigo_MF_Texto+ "->" + respuesta.CFDI);
                if (respuesta.Codigo_MF_Texto.Contains("OK"))
                {
                    Generador.CreaPDF temp2 = new Generador.CreaPDF(respuesta.CFDI);
                    Encfacturas elemento2 = new Encfacturas();

                    elemento2.Fecha = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                    elemento2.Serie = temp2._templatePDF.serie;
                    elemento2.Numero = Int32.Parse(temp2._templatePDF.folio);
                    elemento2.Nombre_cliente = temp2._templatePDF.receptor.razonSocial;
                    elemento2.Subtotal = decimal.Parse(temp2._templatePDF.subtotal.ToString());
                    elemento2.Total = decimal.Parse(temp2._templatePDF.total.ToString());
                    elemento2.IVA = elemento2.Total - elemento2.Subtotal;
                    elemento2.Estado = "A";
                    elemento2.pagada = false;
                    elemento2.IDMetododepago = temp2._templatePDF.metodoPago;
                    elemento2.ConPagos = false;

                    int idcliente = 0;

                    try
                    {
                        ClsDatoEntero clientecapturado = db.Database.SqlQuery<ClsDatoEntero>("select IDCLIENTE AS Dato from clientes  where NOMBRE='" + elemento2.Nombre_cliente + "'").ToList()[0];
                        idcliente = clientecapturado.Dato;
                    }

                    catch
                    {

                    }
                    elemento2.IDCliente = idcliente;


                    elemento2.Moneda = temp2._templatePDF.claveMoneda;
                    elemento2.TC = Decimal.Parse(temp2._templatePDF.tipo_cambio);
                    List<c_Moneda> clavemoneda;
                    clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE claveMoneda='" + temp2._templatePDF.claveMoneda + "'").ToList();
                    int clave = clavemoneda.Select(s => s.IDMoneda).FirstOrDefault();
                    elemento2.IDMoneda = clave;

                    try
                    {
                        
                        string fileName = respuesta.UUID + ".xml";

                        EscribeEnArchivo(respuesta.CFDI, fileName, true);
                    }
                    catch (Exception err)
                    {

                    }




                    elemento2.RutaXML = respuesta.CFDI;
                    elemento2.Moneda = temp2._templatePDF.claveMoneda;
   
                    elemento2.UUID = temp2._templatePDF.folioFiscalUUID;


                    elemento2.NotaCredito = false;
                    elemento2.Anticipo = true;
                    elemento2.FechaRevision = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                    elemento2.FechaVencimiento = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                    elemento2.Descuento = 0;


                    db.encfacturas.Add(elemento2);
                    db.SaveChanges();


                    // CREAR EL XML EN LA RUTA

                    try
                    {
                        Generador.CreaPDF temp = new Generador.CreaPDF(elemento2.RutaXML.ToString());
                        string fileName = temp._templatePDF.folioFiscalUUID + ".xml";

                        EscribeEnArchivo(elemento2.RutaXML.ToString(), fileName, true);
                    }
                    catch (Exception err)
                    {

                    }



                    db.Database.ExecuteSqlCommand("update folio set numero=" + elemento2.Numero + " where IDFolio=" + dbf.IDFolio);
                    return RedirectToAction("Index");
                }
                else
                {


                    return Content(reshtml);
                }


              
            }
          
        }


        public ActionResult CrearAnticipoEgreso(string UUID)
        {

            ViewBag.Mensaje = "";
            ViewBag.Monto = "";
            ViewBag.Cliente = "";
            ViewBag.Factura = 0;
            ViewBag.Moneda = "";
            ViewBag.Anticipo = "";
            ViewAnticipoAplicacion elementoacomplementar = new ViewAnticipoAplicacion();
            if (UUID.Length >= 30)
            {
                elementoacomplementar.UUID = UUID;
                Encfacturas factura = new EncfacturaContext().encfacturas.Where(x => x.UUID == UUID).ToList()[0];
                ViewBag.Cliente = factura.Nombre_cliente;
                ViewBag.Factura = factura.Serie + factura.Numero;
                ViewBag.Monto = factura.Total + factura.Moneda;
                string cadenaanticipo = "Anticipo ";

                Generador.CreaPDF temp = new Generador.CreaPDF(factura.RutaXML);

                try
                {
                    Encfacturas facturaan = new EncfacturaContext().encfacturas.Where(x => x.UUID == temp._templatePDF.UUIDrelacionados[0].UUID).ToList()[0];
                    cadenaanticipo += facturaan.Total.ToString("C") + " ";

                }
                catch (Exception err)
                {
                    string uf = temp._templatePDF.UUIDrelacionados[0].UUID;
                    Encfacturas facturaan = new EncfacturaContext().encfacturas.Where(x => x.UUID == uf).ToList().FirstOrDefault();
                    cadenaanticipo += facturaan.Total.ToString("C") + " ";

                }



                ViewBag.Anticipo = cadenaanticipo;
            }


            var listaserie = new FoliosRepository().GetFolios();


            ViewBag.listaserie = listaserie;

            return View(elementoacomplementar);
        }



        [HttpPost]
        public ActionResult CrearAnticipoEgreso(ViewAnticipoAplicacion elemento)
        {
            Encfacturas facturadigital = new EncfacturaContext().encfacturas.Where(x => x.UUID == elemento.UUID).ToList()[0];
            ViewBag.Cliente = facturadigital.Nombre_cliente;
            ViewBag.Factura = facturadigital.Serie + facturadigital.Numero;
            ViewBag.Monto = facturadigital.Total + facturadigital.Moneda;


            Generador.CreaPDF xmldigital = new Generador.CreaPDF(facturadigital.RutaXML);
            Encfacturas facturaan = null;
            try
            {
                facturaan = new EncfacturaContext().encfacturas.Where(x => x.UUID == xmldigital._templatePDF.UUIDrelacionados[0].UUID).ToList()[0];

            }
            catch (Exception err)
            {
                string uf = xmldigital._templatePDF.UUIDrelacionados[0].UUID;
                facturaan = new EncfacturaContext().encfacturas.Where(x => x.UUID == uf).ToList().FirstOrDefault();


            }

            Generador.CreaPDF xmlanticipo = new Generador.CreaPDF(facturaan.RutaXML);

            if (!ModelState.IsValid)
            {

                var listaserie = new FoliosRepository().GetFolios();
                ViewBag.Mensaje = "";

                ViewBag.listaserie = listaserie;
                return View(elemento);
            }
            else
            {


                ClsFactura factura = new ClsFactura();

                factura.formadepago = xmlanticipo._templatePDF.formaPago;
                factura.metododepago = "PUE";
                factura.tipodecambio = "1";
                factura.Tipodecombrobante = "E";
                factura.uso = "P01";

                factura.cfdirelacionados.relacion = "07";
                factura.cfdirelacionados.uuid.Add(facturadigital.UUID);


                Folio dbf = new Models.Cfdi.Folio();
                dbf = db.Folios.Find(elemento.serie);
                factura._serie = dbf.Serie;
                factura._folio = (dbf.Numero + 1).ToString();



                Empresa emi = db.Empresa.Find(2);
                factura.Emisora = emi;
                factura.Regimen = emi.c_RegimenFiscal.ClaveRegimenFiscal.ToString();

                Empresa rec = new Empresa();
                //Clientes clienteseleccionado = new ClientesContext().Clientes.Find(elemento.IDCliente);

                rec.RFC = xmlanticipo._templatePDF.receptor.rfc;
                rec.RazonSocial = facturaan.Nombre_cliente;
                factura.Receptora = rec;

                Concepto concepto = new Concepto();
                concepto.NoIdentificacion = "1";
                concepto.ClaveUnidad = "ACT";
                concepto.Unidad = "ACTIVIDAD";
                concepto.Cantidad = 1;
                concepto.ClaveProdServ = "84111506";
                concepto.Descripcion = "APLICACION DE ANTICIPO";


                concepto.Descuento = 0;
                concepto.Importe = facturaan.Subtotal;
                concepto.ValorUnitario = facturaan.Subtotal;



                factura.Listaconceptos.conceptos.Add(concepto);

                //string strini = factura.construirfactura();
                //factura.EscribeEnArchivo(strini, System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"), true);
                //string ini = Convert.ToBase64String(System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini")));
                //WSmultifacturas.RespuestaWS respuesta = factura.timbrar(ini);

                MultiFacturasSDK.MFSDK sdk = factura.construirfactura2();
                MultiFacturasSDK.SDKRespuesta respuesta = factura.timbrar(sdk);




                var reshtml = Server.HtmlEncode(respuesta.Codigo_MF_Texto + "->" + respuesta.CFDI);
                if (respuesta.Codigo_MF_Texto.Contains("OK"))
                {
                    Generador.CreaPDF temp2 = new Generador.CreaPDF(respuesta.CFDI);

                    Encfacturas elemento2 = new Encfacturas();
                    try
                    {

                        elemento2.Fecha = Convert.ToDateTime(temp2._templatePDF.fechaEmisionCFDI);

                        elemento2.Serie = temp2._templatePDF.serie;
                        elemento2.Numero = Int32.Parse(temp2._templatePDF.folio);
                        elemento2.Nombre_cliente = temp2._templatePDF.receptor.razonSocial;
                        elemento2.Subtotal = decimal.Parse(temp2._templatePDF.subtotal.ToString());
                        elemento2.Total = decimal.Parse(temp2._templatePDF.total.ToString());
                        elemento2.IVA = elemento2.Total - elemento2.Subtotal;
                        elemento2.Estado = "A";
                        elemento2.pagada = true;
                        elemento2.IDMetododepago = temp2._templatePDF.metodoPago;
                        elemento2.ConPagos = false;
                        elemento2.IDTipoComprobante=1;
                        int idcliente = 0;

                        try
                        {
                            ClsDatoEntero clientecapturado = db.Database.SqlQuery<ClsDatoEntero>("select IDCliente AS Dato from clientes where  NOMBRE='" + elemento2.Nombre_cliente + "'").ToList().FirstOrDefault();
                            idcliente = clientecapturado.Dato;
                        }

                        catch
                        {

                        }
                        elemento2.IDCliente = idcliente;


                        elemento2.Moneda = temp2._templatePDF.claveMoneda;
                        elemento2.TC = Decimal.Parse(temp2._templatePDF.tipo_cambio);
                        List<c_Moneda> clavemoneda;
                        clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE claveMoneda='" + temp2._templatePDF.claveMoneda + "'").ToList();
                        int clave = clavemoneda.Select(s => s.IDMoneda).FirstOrDefault();
                        elemento2.IDMoneda = clave;






                        elemento2.RutaXML = respuesta.CFDI;
                        elemento2.Moneda = temp2._templatePDF.claveMoneda;

                        elemento2.UUID = temp2._templatePDF.folioFiscalUUID;
                        db.encfacturas.Add(elemento2);
                        string insert = "INSERT INTO [dbo].[EncFacturas]([Serie],[Numero],[Nombre_Cliente],[Subtotal],[IVA],[Total],[UUID],[RutaXML],[pagada],[Fecha],[TC],[Moneda],[IDMoneda],[Estado],[IDMetododepago],[ConPagos],[IDCliente],[Anticipo],[NotaCredito],[IDTipoComprobante],[Descuento])" +
                            "VALUES ('"+elemento2.Serie+"','"+elemento2.Numero+"','"+elemento2.Nombre_cliente+"','"+elemento2.Subtotal+"','"+elemento2.IVA+"','"+elemento2.Total+"','"+elemento2.UUID+"','"+elemento2.RutaXML+"','"+elemento2.pagada+ "',SYSDATETIME(),'" + elemento2.TC+"','"+elemento2.Moneda+"'," +
                            "                                                                                                                                                           '"+elemento2.IDMoneda+"','"+elemento2.Estado+"','"+elemento2.IDMetododepago+"','"+elemento2.ConPagos+"','"+elemento2.IDCliente+"','"+elemento2.Anticipo+"','"+elemento2.NotaCredito+"','"+elemento2.IDTipoComprobante+"','"+elemento2.Descuento+"')";
                        db.Database.ExecuteSqlCommand(insert);
                        //db.SaveChanges();
                    }
                    catch (Exception err)
                    {

                    }


                    try
                    {

                        string fileName = respuesta.UUID + ".xml";

                        EscribeEnArchivo(respuesta.CFDI, fileName, true);
                    }
                    catch (Exception err)
                    {

                    }

                    ////saldo factura del ANTICIPO
                    //try
                    //{
                    //    decimal total = 0M;
                    //    List<AplicacionAnticipos> aplicacion = new AplicacionAnticiposContext().Database.SqlQuery<AplicacionAnticipos>("select*from AplicacionAnticipos where idacturaanticipo="+facturadigital.ID).ToList();

                    //    foreach (AplicacionAnticipos anticipos in aplicacion)
                    //    {
                    //        total += anticipos.Total;
                    //    }

                    //    db.Database.ExecuteSqlCommand("update [SaldoFactura] set ImportePagado=" + total + ", ImporteSaldoInsoluto=(total -"+ total+")  where IDFactura=" + facturadigital.ID);


                    //}
                    //catch (Exception error)
                    //{

                    //}
                    Encfacturas facturagrabada = db.encfacturas.ToList().Where(x => (x.UUID == elemento2.UUID)).ToList()[0];

                    SaldoFactura saldo = new SaldoFactura();
                    saldo.IDFactura = facturagrabada.ID;
                    saldo.Serie = elemento2.Serie;
                    saldo.Numero = elemento2.Numero;
                    saldo.Total = elemento2.Total;
                    saldo.ImporteSaldoAnterior = elemento2.Total;
                    saldo.ImportePagado = elemento2.Total;
                    saldo.ImporteSaldoInsoluto = 0;

                    SaldoFacturaContext dbsp = new SaldoFacturaContext();
                    dbsp.SaldoFacturas.Add(saldo);
                    dbsp.SaveChanges();

                    db.Database.ExecuteSqlCommand("update folio set numero=" + elemento2.Numero + " where IDFolio=" + dbf.IDFolio);
                    db.Database.ExecuteSqlCommand("update [SaldoFactura] set ImporteSaldoAnterior=(Total-importepagado)  where IDFactura=" + facturadigital.ID);

                    db.Database.ExecuteSqlCommand("update [SaldoFactura] set ImportePagado=" + facturaan.Total + ", ImporteSaldoInsoluto=" + (facturadigital.Total - facturaan.Total) + "  where IDFactura=" + facturadigital.ID);

                    try
                    {
                        SaldoFactura saldoF = new SaldoFacturaContext().Database.SqlQuery<SaldoFactura>("select*from saldofactura where idfactura=" + facturadigital.ID).FirstOrDefault();

                        if (saldoF.ImporteSaldoInsoluto == 0)
                        {
                            db.Database.ExecuteSqlCommand("update encfacturas set pagada='true', conpagos='true' where id=" + facturadigital.ID);
                            //db.Database.ExecuteSqlCommand("update encfacturas set pagada='true' where id=" + facturadigital.ID);
                        }

                    }
                    catch (Exception err)
                    {

                    }
                    return RedirectToAction("Index", "FacturaAll");

                }
                else
                {


                    return Content(reshtml);
                }



            }

        }


        public ActionResult CrearNotadecredito(string uuid, decimal monto, int id)
        {

            ViewBag.MensajeSaldo = "";
            ViewNotaCredito elementoacomplementar = new ViewNotaCredito();
            int IDSaldo = 0;
            try
            {
                ClsDatoEntero clsDato = new SaldoFacturaContext().Database.SqlQuery<ClsDatoEntero>("select max (idsaldofactura) as dato  from SaldoFactura where idfactura=" + id).ToList().FirstOrDefault();
                IDSaldo = clsDato.Dato;
            }
            catch (Exception err)
            {

            }
            try
            {
                SaldoFactura saldo = new SaldoFacturaContext().Database.SqlQuery<SaldoFactura>("select*from SaldoFactura where idsaldofactura=" + IDSaldo).ToList().FirstOrDefault();
                if (saldo.ImporteSaldoInsoluto == 0 || saldo.ImporteSaldoInsoluto < 0)
                {
                    ViewBag.MensajeSaldo = "SALDO INSOLUTO 0";
                }
            }
            catch (Exception err)
            {
                SaldoFactura saldo = new SaldoFacturaContext().Database.SqlQuery<SaldoFactura>("select*from SaldoFactura where idfactura=" + id).ToList().FirstOrDefault();
                if (saldo.ImporteSaldoInsoluto == 0 || saldo.ImporteSaldoInsoluto < 0)
                {
                    ViewBag.MensajeSaldo = "SALDO INSOLUTO 0";
                }

            }
            //ViewNotaCredito elementoacomplementar = new ViewNotaCredito();

            elementoacomplementar.uuid = uuid;
            elementoacomplementar.Monto = monto;
            elementoacomplementar.IDTipoRelacion = "03";
            elementoacomplementar.IDUsoCFDI = "G02";
            elementoacomplementar.IDTipoComprobante = "E";
            elementoacomplementar.IDFactura = id;


            Encfacturas facturaanterior = new EncfacturaContext().encfacturas.Find(elementoacomplementar.IDFactura);
            elementoacomplementar.Moneda = facturaanterior.Moneda;


            string rutaArchivo = elementoacomplementar.uuid + ".xml";
            string fileName = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + rutaArchivo);
            string xmlString = System.IO.File.ReadAllText(fileName);

            Generador.CreaPDF tempf = new Generador.CreaPDF(xmlString);

            elementoacomplementar.nombre = tempf._templatePDF.receptor.razonSocial;
            elementoacomplementar.rfc = tempf._templatePDF.receptor.rfc;

            var listametodopago = new MetodoPagoRepository().GetMetodosdepago();

            var listaformapago = new FormaPagoRepository().GetFormasdepago();
            var listaserie = new FoliosNotaCreditoRepository().GetFolios();

            ViewBag.Montodefactura = monto;

            ViewBag.Mensaje = "";
            ViewBag.uuid = uuid;

            ViewBag.listametodopago = new MetodoPagoRepository().GetMetodosdepago();

            ViewBag.listaformapago = listaformapago;
            ViewBag.listaserie = listaserie;


            return View(elementoacomplementar);
        }



        [HttpPost]
        public ActionResult CrearNotadecredito(ViewNotaCredito elemento)
        {

            if (!ModelState.IsValid)
            {

                elemento.IDTipoRelacion = "03";
                elemento.IDUsoCFDI = "G02";
                elemento.IDTipoComprobante = "E";

                var listacliente = new ClienteRepository().GetClientes();
                var listametodopago = new MetodoPagoRepository().GetMetodosdepago();

                var listaformapago = new FormaPagoRepository().GetFormasdepago();
                var listaserie = new FoliosRepository().GetFolios();
                ViewBag.Mensaje = "No pude crear la nota de credito";

                ViewBag.listametodopago = new MetodoPagoRepository().GetMetodosdepago();

                ViewBag.listaformapago = listaformapago;
                ViewBag.listaserie = listaserie;
                ViewBag.listaClientes = listacliente;

                return View(elemento);



            }
            else
            {

                string uuid = elemento.uuid;

                ClsFactura factura = new ClsFactura();


                factura.formadepago = db.FormaPagos.Find(elemento.IDFormaPago).ClaveFormaPago;
                factura.metododepago = db.metodopagos.Find(elemento.IDMetodoPago).ClaveMetodoPago;
                if (elemento.Moneda == "MXN")
                {
                    factura.tipodecambio = "1";
                }
                else
                {

                    c_Moneda monedanacional = new c_MonedaContext().c_Monedas.SqlQuery("select * from c_moneda where clavemoneda ='MXN'").ToList()[0];
                    c_Moneda monedaforanea = new c_MonedaContext().c_Monedas.SqlQuery("select * from c_moneda where clavemoneda ='" + elemento.Moneda + "'").ToList()[0];
                    try
                    {
                        factura.tipodecambio = new c_MonedaContext().Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetTipocambio] ('" + DateTime.Now.ToString("s") + "'," + monedaforanea.IDMoneda + "," + monedanacional.IDMoneda + ") as Dato").ToList()[0].Dato.ToString();
                    }
                    catch
                    {
                        factura.tipodecambio = "1";
                    }
                }
                factura.Tipodecombrobante = elemento.IDTipoComprobante;
                factura.uso = elemento.IDUsoCFDI;
                factura.cfdirelacionados.relacion = elemento.IDTipoRelacion;
                factura.cfdirelacionados.uuid.Add(elemento.uuid);
                factura.Moneda = elemento.Moneda;


                Folio dbf = new Models.Cfdi.Folio();
                dbf = db.Folios.Find(elemento.serie);
                factura._serie = dbf.Serie;
                factura._folio = (dbf.Numero + 1).ToString();



                Empresa emi = db.Empresa.Find(2);
                factura.Emisora = emi;
                factura.Regimen = emi.c_RegimenFiscal.ClaveRegimenFiscal.ToString();

                Empresa rec = new Empresa();


                rec.RFC = elemento.rfc;
                rec.RazonSocial = elemento.nombre;
                factura.Receptora = rec;

                Concepto concepto = new Concepto();
                switch (factura.formadepago)
                {
                    case "30":
                        {
                            concepto.NoIdentificacion = "1";
                            concepto.ClaveUnidad = "ACT";
                            concepto.Unidad = "ACTIVIDAD";
                            concepto.Cantidad = 1;
                            concepto.ClaveProdServ = "84111506";
                            concepto.Descripcion = "APLICACION DE ANTICIPO";
                            factura.cfdirelacionados.relacion = "07";
                            break;
                        }
                    default:
                        {
                            concepto.NoIdentificacion = "1";
                            concepto.ClaveUnidad = "ACT";
                            concepto.Unidad = "ACTIVIDAD";
                            concepto.Cantidad = 1;
                            concepto.ClaveProdServ = "84111506";
                            concepto.Descripcion = elemento.Observacion;
                            break;
                        }
                }




                concepto.Descuento = 0;
                concepto.Importe = Math.Round((elemento.Monto / decimal.Parse(1.16.ToString())), 2);
                concepto.ValorUnitario = Math.Round((elemento.Monto / decimal.Parse(1.16.ToString())), 2);



                factura.Listaconceptos.conceptos.Add(concepto);

                //string strini = factura.construirfactura();
                //factura.EscribeEnArchivo(strini, System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"), true);
                //string ini = Convert.ToBase64String(System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini")));
                //WSmultifacturas.RespuestaWS respuesta = factura.timbrar(ini);
                //var reshtml = Server.HtmlEncode(respuesta.codigo_mf_texto + "->" + respuesta.cfdi);


                MultiFacturasSDK.MFSDK sdk = factura.construirfactura2();
                MultiFacturasSDK.SDKRespuesta respuesta = factura.timbrar(sdk);

                var reshtml = Server.HtmlEncode(respuesta.Codigo_MF_Texto + "->" + respuesta.CFDI);
                if (respuesta.Codigo_MF_Texto.Contains("OK"))
                {
                    Generador.CreaPDF temp2 = new Generador.CreaPDF(respuesta.CFDI);
                    Encfacturas elemento2 = new Encfacturas();

                    elemento2.Fecha = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                    elemento2.Serie = temp2._templatePDF.serie;
                    elemento2.Numero = Int32.Parse(temp2._templatePDF.folio);
                    string NombreC = "";
                    try
                    {
                        Clientes clientes = new ClientesContext().Database.SqlQuery<Clientes>("select*from clientes where nombre40='"+ temp2._templatePDF.receptor.razonSocial + "'").ToList().FirstOrDefault();

                        NombreC = clientes.Nombre;
                    }
                    catch (Exception err)
                    {
                        NombreC = temp2._templatePDF.receptor.razonSocial;

                    }
                    elemento2.Nombre_cliente = NombreC;
                    elemento2.Subtotal = decimal.Parse(temp2._templatePDF.subtotal.ToString());
                    elemento2.Total = decimal.Parse(temp2._templatePDF.total.ToString());
                    elemento2.IVA = elemento2.Total - elemento2.Subtotal;
                    elemento2.Estado = "A";
                    elemento2.pagada = false;
                    elemento2.ConPagos = false;


                    int idcliente = 0;

                    try
                    {
                        ClsDatoEntero clientecapturado = db.Database.SqlQuery<ClsDatoEntero>("select IDCliente AS Dato from clientes NOMBRE='" + elemento2.Nombre_cliente + "'").ToList()[0];
                        idcliente = clientecapturado.Dato;
                        elemento2.IDCliente = idcliente;
                    }

                    catch
                    {

                    }

                    elemento2.Moneda = temp2._templatePDF.claveMoneda;
                    elemento2.TC = Decimal.Parse(temp2._templatePDF.tipo_cambio);
                    List<c_Moneda> clavemoneda;
                    clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE claveMoneda='" + temp2._templatePDF.claveMoneda + "'").ToList();
                    int clave = clavemoneda.Select(s => s.IDMoneda).FirstOrDefault();
                    elemento2.IDMoneda = clave;
                    elemento2.IDMetododepago = temp2._templatePDF.metodoPago;

                    elemento2.RutaXML = respuesta.CFDI;
                    elemento2.Moneda = temp2._templatePDF.claveMoneda;

                    elemento2.UUID = temp2._templatePDF.folioFiscalUUID;
                    db.encfacturas.Add(elemento2);


                    elemento2.NotaCredito = true;
                    elemento2.Anticipo = false;
                    elemento2.FechaRevision = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                    elemento2.FechaVencimiento = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                    elemento2.Descuento = 0;





                    db.SaveChanges();

                    // CREAR EL XML EN LA RUTA

                    try
                    {
                        Generador.CreaPDF temp = new Generador.CreaPDF(elemento2.RutaXML.ToString());
                        string fileName = temp._templatePDF.folioFiscalUUID + ".xml";

                        EscribeEnArchivo(elemento2.RutaXML.ToString(), fileName, true);
                    }
                    catch (Exception err)
                    {

                    }





                    db.Database.ExecuteSqlCommand("update folio set numero=" + elemento2.Numero + " where IDFolio=" + dbf.IDFolio);


                    Encfacturas idfacturaoriginal = db.Database.SqlQuery<Encfacturas>("select * from [EncFacturas] where uuid='" + uuid + "'").ToList().FirstOrDefault();

                    VEncFactura vencfactura = db.Database.SqlQuery<VEncFactura>("SELECT  E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda,  E.IDMetododepago, E.Subtotal, E.IVA, E.Total, CAST(ISNULL(S.ImporteSaldoAnterior, 0) AS DECIMAL(15,4)) as ImporteSaldoAnterior , CAST(ISNULL(S.ImportePagado, 0) AS DECIMAL(15,4)) as ImportePagado, CAST(ISNULL(S.ImporteSaldoInsoluto, 0) AS DECIMAL (15,4)) as ImporteSaldoInsoluto, CAST(ISNULL(MAX(D.NoParcialidad), 0) AS INT) as NoParcialidad from EncFacturas as E left outer join (DocumentoRelacionado  as D left outer join SaldoFactura as S on D.IDFactura = S.IDFactura) on E.ID = D.IDFactura where pagada = 0 and estado = 'A' and E.ID=" + idfacturaoriginal.ID + " group by E.ID, E.Serie, E.Numero, E.Nombre_Cliente, E.TC, E.IDMoneda, E.Moneda, E.IDMetododePago, E.Subtotal, E.IVA, E.Total,S.ImporteSaldoAnterior, S.ImportePagado, S.ImporteSaldoInsoluto").ToList().FirstOrDefault();

                    ClsDatoEntero parcialidad = db.Database.SqlQuery<ClsDatoEntero>("select  CAST(ISNULL(max(NoParcialidad), 0) AS INT) as Dato from dbo.DocumentoRelacionado where IDFactura=" + idfacturaoriginal.ID + "").ToList()[0];

                    int noparcialidad = 0;


                    decimal ImporteSaldoInsoluto = vencfactura.Total;


                    if (parcialidad.Dato != 0)
                    {


                        noparcialidad = parcialidad.Dato + 1;
                        ImporteSaldoInsoluto = vencfactura.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        noparcialidad = 1;
                        ImporteSaldoInsoluto = vencfactura.Total;
                    }

                    ImporteSaldoInsoluto = ImporteSaldoInsoluto - elemento2.Total;


                    db.Database.ExecuteSqlCommand("INSERT INTO DocumentoRelacionado([IDPagoFactura],[IDCliente],[IDFactura],[Serie],[Numero],[IDMoneda],[TC],[IDMetododepago],[ImporteSaldoInsoluto],[ImportePagado],[NoParcialidad]) values(0,'" + idfacturaoriginal.IDCliente + "','" + idfacturaoriginal.ID + "','" + idfacturaoriginal.Serie + "','" + idfacturaoriginal.Numero + "','" + idfacturaoriginal.IDMoneda + "','" + idfacturaoriginal.TC + "','" + idfacturaoriginal.IDMetododepago + "','" + ImporteSaldoInsoluto + "','" + elemento2.Total + "','" + noparcialidad + "')");

                   //insertar relacion NC
                    try
                    {
                        List<DocumentoRelacionado> Documento;
                        Documento = db.Database.SqlQuery<DocumentoRelacionado>("SELECT * FROM [dbo].[DocumentoRelacionado] WHERE IDDocumentoRelacionado = (SELECT MAX(IDDocumentoRelacionado) from DocumentoRelacionado)").ToList();
                        int IDDocumentoRelacionado = Documento.Select(s => s.IDDocumentoRelacionado).FirstOrDefault();


                        List<Encfacturas> IDNotaC;
                        IDNotaC = db.Database.SqlQuery<Encfacturas>("SELECT * FROM [dbo].[Encfacturas] WHERE ID = (SELECT MAX(ID) from Encfacturas)").ToList();
                        int Nota = IDNotaC.Select(s => s.ID).FirstOrDefault();
                        List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                        int UserID = userid.Select(s => s.UserID).FirstOrDefault();

                        string insert = "insert into NotasCredito(IDFacturaNota,IDFacturaRelacionada,EstadoNC,Usuario,Fecha, IDDocumentoRelacionado) values " +
                            "(" + Nota + "," + idfacturaoriginal.ID + ",'A'," + UserID + ",sysdatetime(),"+ IDDocumentoRelacionado + ")";

                        db.Database.ExecuteSqlCommand(insert);
                    }
                    catch (Exception err)
                    {

                    }
                    
                    
                    db.Database.ExecuteSqlCommand("update dbo.EncFacturas set ConPagos='true' where ID=" + idfacturaoriginal.ID + "  ");

                    ClsDatoEntero numero = db.Database.SqlQuery<ClsDatoEntero>("select count(IDFactura) as Dato from SaldoFactura where IDFactura='" + idfacturaoriginal.ID + "'").ToList()[0];
                    int num = numero.Dato;
                    if (num != 0)
                    {
                        SaldoFactura saldof = db.Database.SqlQuery<SaldoFactura>("select * from SaldoFactura where IDFactura='" + idfacturaoriginal.ID + "'").ToList()[0];

                        db.Database.ExecuteSqlCommand("update SaldoFactura set ImporteSaldoAnterior='" + saldof.ImporteSaldoInsoluto + "',ImportePagado=Importepagado+" + elemento2.Total + ",ImporteSaldoInsoluto='" + ImporteSaldoInsoluto + "' where IDFactura=" + idfacturaoriginal.ID + "");

                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO SaldoFactura([IDFactura], [Serie], [Numero],[Total],[ImporteSaldoAnterior],[ImportePagado],[ImporteSaldoInsoluto]) values (" + idfacturaoriginal.ID + ",'" + idfacturaoriginal.Serie + "', " + idfacturaoriginal.Numero + ", " + idfacturaoriginal.Total + "," + idfacturaoriginal.Total + "," + elemento2.Total + ", " + ImporteSaldoInsoluto + ")");
                    }

                    SaldoFactura saldofinal = db.Database.SqlQuery<SaldoFactura>("select * from SaldoFactura where IDFactura='" + idfacturaoriginal.ID + "'").ToList()[0];

                    if (saldofinal.ImporteSaldoInsoluto == 0)
                    {
                        db.Database.ExecuteSqlCommand("update PagoFactura set Estado= 1 where IDPagoFactura='" + idfacturaoriginal.ID + "'");
                    }


                    return RedirectToAction("Index");
                }
                else
                {


                    return Content(reshtml);
                }



            }

        }



        public static void EscribeEnArchivo(string contenido, string rutaArchivo, bool sobrescribir = true)
        {
            string archivoxml = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + rutaArchivo);

            using (FileStream fs = System.IO.File.Create(archivoxml))
            {
                AddText(fs, contenido);
            }

        }


        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }





    }




}