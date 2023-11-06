using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

using System.Xml;
using System.Globalization;
using System.Web;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Administracion;
using System.Collections;
using SIAAPI.Reportes;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.CartaPorte;

namespace GeneradorCartaPorte
{
    public class Emisor
    {

        public string rfc = string.Empty;
        public string razonSocial = string.Empty;
        public string calle = string.Empty;
        public string numeroExterior = string.Empty;
        public string numeroInterior = string.Empty;
        public string colonia = string.Empty;
        public string localidad = string.Empty;
        public string municipio = string.Empty;
        public string estado = string.Empty;
        public string pais = string.Empty;
        public string cp = string.Empty;
        public string telefono = string.Empty;

        public string Nombre { get; internal set; }
        public string RegimenFiscal { get; internal set; }
        public string usocfdi { get; internal set; }


    }


    public class ProductoCFD
    {

        public string cantidad = string.Empty;
        public string descripcion = string.Empty;
        public string unidad = string.Empty;
        public float valorUnitario = 0.00f;
        public float importe = 0.00f;
        public string numIdentificacion = string.Empty;

        public string ClaveProducto { get; internal set; }
        public string id { get; internal set; }
        public string c_unidad { get; internal set; }
        public string desc { get; internal set; }
        public float v_unitario = 0.00f;
        public float descuento { get; internal set; }
    }

    public class CartaPorte
    {
        public string version = string.Empty;
        public string distanciaRecorrida = string.Empty;
        public string transporteIn = string.Empty;
    }

    public class Ubicaciones
    {
        public string FechaHSLLO = string.Empty;
        public string FechaHSLLD = string.Empty;
        public string RFCremitente = string.Empty;
        public string IDUbicacion = string.Empty;
        public string Tubicacion = string.Empty;
        public string DistanciaRecorrida = string.Empty;
        public string TipoUbicacion = string.Empty;
        public string IDUbicacionDos = string.Empty;
    }

    public class Mercancias
    {
        public float pesoBT = 0.00f;
        public int totalMer = 0;
        public string UnidadPeso = string.Empty;
        public decimal cantidad = 0;
        public float pesokg = 0.00f;
        public string bienes = string.Empty;
        public string descripcion = string.Empty;
        public string claveUnidad = string.Empty;
        public decimal cantidadt = 0;
        public string moneda = "XXX";
        public string idorigen = string.Empty;
        public string iddestino = string.Empty;

    }

    public class autotransporte
    {
        public string NumPermiso = string.Empty;
        public string permisoSCT = string.Empty;
        public string anioMod = string.Empty;
        public string configV = string.Empty;
        public string placaVM = string.Empty;
        public string polizaRC = string.Empty;
        public string aseguraRC = string.Empty;
        public string aseguraCarga = string.Empty;
    }

    public class FiguraTransporte
    {
        public string Tipofigura = string.Empty;
        public string RFCfigura = string.Empty;
        public string numLic = string.Empty;
    }

    public class DocumentoPDF
    {


        public string serie = string.Empty;
        public string folio = string.Empty;
        public string folioFiscalUUID = string.Empty;
        public string noSerieCertificadoSAT = string.Empty;
        public string noSerieCertificadoEmisor = string.Empty;
        public string fechaCertificacion = string.Empty;
        public string fechaEmisionCFDI = string.Empty;
        public String TipoDecomprobrante = string.Empty;
        public String TipoDeRelacion = string.Empty;
        public String Certificado = string.Empty;
        public string rfcprovCertif = string.Empty;

        public string regimenFiscal = string.Empty;
        public string lugarExpedicion = string.Empty;
        public string formaPago = string.Empty;
        public string metodoPago = string.Empty;
        public string claveMoneda = string.Empty;

        public string selloDigitalCFDI = string.Empty;
        public string selloDigitalSAT = string.Empty;
        public string cadenaOriginal = string.Empty;

        public decimal subtotal = 0M;
        public decimal total = 0M;
        public decimal descuento = 0M;

        public string fechaExpedicion = string.Empty;

        public Emisor emisor = new Emisor();
        public Emisor receptor = new Emisor();
        public CartaPorte cp = new CartaPorte();
        public Mercancias mercan = new Mercancias();
        public autotransporte auto = new autotransporte();
        public Ubicaciones ubica = new Ubicaciones();

        public FiguraTransporte figura = new FiguraTransporte();
        public List<ProductoCFD> productos = new List<ProductoCFD>();
        public List<Ubicaciones> ubicacion = new List<Ubicaciones>();
        public List<Mercancias> mercancias = new List<Mercancias>();
        public List<autotransporte> autotransporte = new List<autotransporte>();




        public string Telefono = "";

        public string tipo_cambio { get; internal set; }


    }

    public class CreaPDFCP
    {
        ClsColoresReporte colorf;
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public DocumentoPDF _templatePDF; //Objeto que contendra la información del documento pdf
        XmlDocument xDoc; // Objeto para abrir el archivo xml

        public string nombreDocumento = string.Empty;

        public EncfacturasSaldos prefactura = null;

        public DetPrefactura detprefactura = null;
        public CMYKColor colordefinido;
        public int _IDOperador = 0;
        public string FechaSalida = "";
        public string FechaLlegada = "";
        public CreaPDFCP(string rutaXML, int IDOperador, System.Drawing.Image logo, string Telefono = "", bool descarga = false)
        {
            LeerArtributosXML(rutaXML);
            _IDOperador = IDOperador;
            //Trabajos con el documento XML
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + ".pdf");
            _documento = new Document(PageSize.LETTER);
            _documento = new Document(PageSize.LETTER.Rotate());
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;
            colorf = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            CMYKColor colordefinidoX = colorf.color;
            //Creamos el documento
            colordefinido = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            try
            {
                if (File.Exists(nombreDocumento))
                {
                    nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + DateTime.Now.Day + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".pdf"); ;
                }

            }
            catch (Exception ERR)
            {
                string mensajederror = ERR.Message;
            }
            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            _writer.PageEvent = new ITextEvents();

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor(Telefono, logo);
            AgregarDatosFactura();
            AgregarDatosCP(_IDOperador);
            AgregarSellos();


            _documento.Close();
            _writer.Close();
            _writer.Dispose();
            //if (descarga)
            //{
            //    HttpContext.Current.Response.ContentType = "pdf/application";
            //    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;" +"filename=Factura" + _templatePDF.serie + _templatePDF.folio + ".pdf");
            //    HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //    HttpContext.Current.Response.Write(_documento);

            //    HttpContext.Current.Response.End();

            //    //byte[] fileBytes = System.IO.File.ReadAllBytes(nombreDocumento);
            //    //MemoryStream ms = new MemoryStream(fileBytes, 0, 0, true, true);
            //    //Response.AddHeader("content-disposition", "attachment;filename= NombreArchivo");
            //    //Response.Buffer = true;
            //    //Response.Clear();
            //    //Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);

            //    //Response.End();

            //}

        }

        public CreaPDFCP(string rutaXML, System.Drawing.Image logo, EncfacturasSaldos _factura, string Telefono = "", bool descarga = false)
        {
            prefactura = _factura;
            colordefinido = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;


            LeerArtributosXML(rutaXML);

            //Trabajos con el documento XML
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + ".pdf");
            _documento = new Document(PageSize.LETTER.Rotate());
            _documento.SetMargins(25, 10f, 20f, 60f);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            //Creamos el documento
            try
            {
                int contadoarchivo = 1;
                while (File.Exists(nombreDocumento))
                {

                    nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + "(" + contadoarchivo + ").pdf");
                    contadoarchivo++;
                }
            }
            catch (Exception ERR)
            {
                string mesanjedeerror = ERR.Message;
            }
            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            _writer.PageEvent = new ITextEvents();


            colorf = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            CMYKColor colordefinidoX = colorf.color;
            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor(Telefono, logo);
            AgregarDatosFactura();
            AgregarDatosCP(_IDOperador);
            AgregarSellos();

            //Cerramoe el documento

            _documento.Close();
            _writer.Close();
            _writer.Dispose();


        }


        public CreaPDFCP(string rutaXML)
        {

            xDoc = new XmlDocument(); //Instancia documento pdf
            _templatePDF = new DocumentoPDF(); //Instancia que contendrá la información para llenar el pdf
            xDoc.LoadXml(rutaXML);
            ObtenerNodoCfdiComprobante();
            ObtenerNodoEmisor();
            ObtenerNodoReceptor();
            ObtenerNodoConceptos();
            ObtenerNodoCartaPorte();
            ObtenerNodoUbicaciones();
            ObtenerNodoMercancias();
            ObtenerNodoAutotransporte();
            ObtenerNodoTiposFigura();
            ObtenerNodoComplementoDigital();


        }




        #region Leer datos del .xml

        private void LeerArtributosXML(string rutaXML)
        {
            xDoc = new XmlDocument(); //Instancia documento pdf
            _templatePDF = new DocumentoPDF(); //Instancia que contendrá la información para llenar el pdf
            xDoc.LoadXml(rutaXML);
            ObtenerNodoCfdiComprobante();
            ObtenerNodoEmisor();
            ObtenerNodoReceptor();
            ObtenerNodoConceptos();
            ObtenerNodoCartaPorte();
            ObtenerNodoUbicaciones();
            ObtenerNodoMercancias();
            ObtenerNodoAutotransporte();
            ObtenerNodoTiposFigura();
            ObtenerNodoComplementoDigital();

        }

        private void ObtenerNodoCfdiComprobante()
        {
            decimal valFloat;
            if (xDoc.GetElementsByTagName("cfdi:Comprobante") == null)
                return;

            XmlNodeList comprobante = xDoc.GetElementsByTagName("cfdi:Comprobante");
            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "LugarExpedicion")/*((XmlElement)comprobante[0]).GetAttribute("Serie")*/ != null)
                _templatePDF.lugarExpedicion = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "LugarExpedicion");// ((XmlElement)comprobante[0]).GetAttribute("Serie");

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Fecha")/*((XmlElement)comprobante[0]).GetAttribute("Fecha")*/ != null)
                //XmlAttributeExtensions.GetValue(nodo, "NoIdentificacion")
                _templatePDF.fechaEmisionCFDI = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Fecha");  // ((XmlElement)comprobante[0]).GetAttribute("Fecha");

            try
            {
                if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Folio")/*((XmlElement)comprobante[0]).GetAttribute("Folio")*/ != null)
                    _templatePDF.folio = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Folio");// ((XmlElement)comprobante[0]).GetAttribute("Folio");
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                _templatePDF.folio = "0";
            }

            try
            {
                if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Serie")/*((XmlElement)comprobante[0]).GetAttribute("Serie")*/ != null)
                    _templatePDF.serie = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Serie");// ((XmlElement)comprobante[0]).GetAttribute("Serie");
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                _templatePDF.serie = "";
            }

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "SubTotal")/*((XmlElement)comprobante[0]).GetAttribute("SubTotal")*/ != null)
            {
                decimal.TryParse(XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "SubTotal")/*((XmlElement)comprobante[0]).GetAttribute("SubTotal")*/, out valFloat);
                _templatePDF.subtotal = valFloat;
            }

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Moneda")/*((XmlElement)comprobante[0]).GetAttribute("Moneda")*/ != null)
                _templatePDF.claveMoneda = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Moneda");// ((XmlElement)comprobante[0]).GetAttribute("Moneda");

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Total")/*((XmlElement)comprobante[0]).GetAttribute("Total")*/ != null)
            {
                decimal.TryParse(XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Total")/*((XmlElement)comprobante[0]).GetAttribute("Total")*/, out valFloat);
                _templatePDF.total = valFloat;

                Numalet numaLet = new Numalet();
                numaLet.MascaraSalidaDecimal = "00/100 M.N.";
                if (_templatePDF.claveMoneda == "MXN")
                {
                    numaLet.SeparadorDecimalSalida = "pesos";
                }
                if (_templatePDF.claveMoneda == "USD")
                {
                    numaLet.SeparadorDecimalSalida = "dolares";
                    numaLet.MascaraSalidaDecimal = "00/100";
                }
                if (_templatePDF.claveMoneda == "EUR")
                {
                    numaLet.SeparadorDecimalSalida = "Euros";
                    numaLet.MascaraSalidaDecimal = "00/100";
                }
            }

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "TipoDeComprobante")/*((XmlElement)comprobante[0]).GetAttribute("Serie")*/ != null)
                _templatePDF.TipoDecomprobrante = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "TipoDeComprobante");// ((XmlElement)comprobante[0]).GetAttribute("Serie");

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Certificado")/*((XmlElement)comprobante[0]).GetAttribute("Serie")*/ != null)
                _templatePDF.Certificado = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Certificado");// ((XmlElement)comprobante[0]).GetAttribute("Serie");

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "NoCertificado")/*((XmlElement)comprobante[0]).GetAttribute("NoCertificado")*/ != null)
                _templatePDF.noSerieCertificadoEmisor = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "NoCertificado"); ((XmlElement)comprobante[0]).GetAttribute("NoCertificado");

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Sello")/*((XmlElement)comprobante[0]).GetAttribute("Sello")*/ != null)
                _templatePDF.selloDigitalCFDI = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Sello"); // ((XmlElement)comprobante[0]).GetAttribute("Sello");

        }


        private void ObtenerNodoEmisor()
        {
            //Trabajamos con Emisor
            if (xDoc.GetElementsByTagName("cfdi:Emisor") == null)
                return;
            XmlNodeList emisor = xDoc.GetElementsByTagName("cfdi:Emisor");
            _templatePDF.emisor.rfc = XmlAttributeExtensions.GetValue(((XmlElement)emisor[0]), "Rfc");// ((XmlElement)emisor[0]).GetAttribute("Rfc");
            _templatePDF.emisor.Nombre = XmlAttributeExtensions.GetValue(((XmlElement)emisor[0]), "Nombre");// ((XmlElement)emisor[0]).GetAttribute("Nombre");
            _templatePDF.emisor.RegimenFiscal = XmlAttributeExtensions.GetValue(((XmlElement)emisor[0]), "RegimenFiscal");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");
        }


        private void ObtenerNodoReceptor()
        {
            //Trabajamos con receptor
            XmlNodeList receptor = xDoc.GetElementsByTagName("cfdi:Receptor");
            _templatePDF.receptor.razonSocial = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "Nombre"); //((XmlElement)receptor[0]).GetAttribute("Nombre");
            _templatePDF.receptor.rfc = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "Rfc"); //((XmlElement)receptor[0]).GetAttribute("Rfc");
            _templatePDF.receptor.usocfdi = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "UsoCFDI"); //(XmlElement)receptor[0]).GetAttribute("UsoCFDI");
        }

        private void ObtenerNodoConceptos()
        {
            ProductoCFD p;

            if (xDoc.GetElementsByTagName("cfdi:Conceptos") == null)
                return;
            XmlNodeList conceptos = xDoc.GetElementsByTagName("cfdi:Conceptos");

            if (((XmlElement)conceptos[0]).GetElementsByTagName("cfdi:Concepto") == null)
                return;
            XmlNodeList lista = ((XmlElement)conceptos[0]).GetElementsByTagName("cfdi:Concepto");
            int conta = 1;
            foreach (XmlElement nodo in lista)
            {
                p = new ProductoCFD();



                p.desc = XmlAttributeExtensions.GetValue(nodo, "Descripcion");// nodo.GetAttribute("Descripcion");
                p.id = (XmlAttributeExtensions.GetValue(nodo, "NoIdentificacion")/*nodo.GetAttribute("NoIdentificacion")*/ == "") ? conta.ToString() : XmlAttributeExtensions.GetValue(nodo, "NoIdentificacion");// nodo.GetAttribute("NoIdentificacion");
                p.unidad = XmlAttributeExtensions.GetValue(nodo, "Unidad");// nodo.GetAttribute("Unidad");
                p.cantidad = XmlAttributeExtensions.GetValue(nodo, "Cantidad");  // nodo.GetAttribute("Cantidad");
                p.v_unitario = float.Parse(XmlAttributeExtensions.GetValue(nodo, "ValorUnitario")); //nodo.GetAttribute("ValorUnitario"));
                p.importe = float.Parse(XmlAttributeExtensions.GetValue(nodo, "Importe")); //nodo.GetAttribute("Importe"));
                p.ClaveProducto = XmlAttributeExtensions.GetValue(nodo, "ClaveProdServ");  // nodo.GetAttribute("ClaveProdServ");
                p.c_unidad = XmlAttributeExtensions.GetValue(nodo, "ClaveUnidad"); //nodo.GetAttribute("ClaveUnidad");




                p.descuento = 0;

                _templatePDF.productos.Add(p);
            }
        }

        private void ObtenerNodoCartaPorte()
        {
            if (xDoc.GetElementsByTagName("cartaporte20:CartaPorte") == null)
                return;
            XmlNodeList cp = xDoc.GetElementsByTagName("cartaporte20:CartaPorte");
            _templatePDF.cp.version = XmlAttributeExtensions.GetValue(((XmlElement)cp[0]), "Version");// ((XmlElement)emisor[0]).GetAttribute("Rfc");
            _templatePDF.cp.distanciaRecorrida = XmlAttributeExtensions.GetValue(((XmlElement)cp[0]), "TotalDistRec");// ((XmlElement)emisor[0]).GetAttribute("Nombre");
            _templatePDF.cp.transporteIn = XmlAttributeExtensions.GetValue(((XmlElement)cp[0]), "TranspInternac");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");


        }

        private void ObtenerNodoUbicaciones()
        {
            Ubicaciones u;

            if (xDoc.GetElementsByTagName("cartaporte20:Ubicaciones") == null)
                return;
            XmlNodeList conceptos = xDoc.GetElementsByTagName("cartaporte20:Ubicaciones");
            XmlNodeList lista = ((XmlElement)conceptos[0]).GetElementsByTagName("cartaporte20:Ubicacion");
            int conta = 1;
            if (xDoc.GetElementsByTagName("cartaporte20:Ubicacion") == null)
                return;
            int c = 0;
            foreach (XmlElement nodo in lista)
            {
                u = new Ubicaciones();



                XmlNodeList conceptoss = xDoc.GetElementsByTagName("cartaporte20:Ubicacion");
                u.FechaHSLLO = XmlAttributeExtensions.GetValue(((XmlElement)conceptoss[c]), "FechaHoraSalidaLlegada");// ((XmlElement)emisor[0]).GetAttribute("Rfc");
                u.RFCremitente = XmlAttributeExtensions.GetValue(((XmlElement)conceptoss[c]), "RFCRemitenteDestinatario");// ((XmlElement)emisor[0]).GetAttribute("Nombre");
                u.IDUbicacion = XmlAttributeExtensions.GetValue(((XmlElement)conceptoss[c]), "IDUbicacion");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");
                u.TipoUbicacion = XmlAttributeExtensions.GetValue(((XmlElement)conceptoss[c]), "TipoUbicacion");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");
                u.DistanciaRecorrida = XmlAttributeExtensions.GetValue(((XmlElement)conceptoss[c]), "DistanciaRecorrida");
                _templatePDF.ubicacion.Add(u);
                c++;
            }

            FechaSalida = _templatePDF.ubicacion[0].FechaHSLLO;
            FechaLlegada = _templatePDF.ubicacion[1].FechaHSLLO;
            //if (xDoc.GetElementsByTagName("cartaporte20:Ubicacion") == null)
            //    return;
            //XmlNodeList Ubicacion = xDoc.GetElementsByTagName("cartaporte20:Ubicacion");
            //_templatePDF.ubica.DistanciaRecorrida = XmlAttributeExtensions.GetValue(((XmlElement)Ubicacion[0]), "DistanciaRecorrida");// ((XmlElement)emisor[0]).GetAttribute("Rfc");
            //_templatePDF.ubica.FechaHSLLD = XmlAttributeExtensions.GetValue(((XmlElement)Ubicacion[0]), "FechaHoraSalidaLlegada");// ((XmlElement)emisor[0]).GetAttribute("Nombre");
            //_templatePDF.ubica.RFCremitente = XmlAttributeExtensions.GetValue(((XmlElement)Ubicacion[0]), "RFCRemitenteDestinatario");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");
            //_templatePDF.ubica.IDUbicacion = XmlAttributeExtensions.GetValue(((XmlElement)Ubicacion[0]), "IDUbicacion");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");
            //_templatePDF.ubica.TipoUbicacion = XmlAttributeExtensions.GetValue(((XmlElement)Ubicacion[0]), "TipoUbicacion");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");




        }

        private void ObtenerNodoMercancias()
        {
            Mercancias m;

            if (xDoc.GetElementsByTagName("cartaporte20:Mercancias") == null)
                return;
            XmlNodeList mercancias = xDoc.GetElementsByTagName("cartaporte20:Mercancias");
            _templatePDF.mercan.pesoBT = float.Parse(XmlAttributeExtensions.GetValue(((XmlElement)mercancias[0]), "PesoBrutoTotal"));// ((XmlElement)emisor[0]).GetAttribute("Rfc");
            _templatePDF.mercan.totalMer = int.Parse(XmlAttributeExtensions.GetValue(((XmlElement)mercancias[0]), "NumTotalMercancias"));// ((XmlElement)emisor[0]).GetAttribute("Nombre");
            _templatePDF.mercan.UnidadPeso = XmlAttributeExtensions.GetValue(((XmlElement)mercancias[0]), "UnidadPeso");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");
            int c = 0;
            XmlNodeList mercanciasNodo = xDoc.GetElementsByTagName("cartaporte20:Mercancia");
            foreach (XmlElement nodo in mercanciasNodo)
            {
                m = new Mercancias();
                if (xDoc.GetElementsByTagName("cartaporte20:Mercancia") == null)
                    return;
                XmlNodeList mercancia = xDoc.GetElementsByTagName("cartaporte20:Mercancia");
                m.cantidad = decimal.Parse(XmlAttributeExtensions.GetValue(((XmlElement)mercancia[c]), "Cantidad"));// ((XmlElement)emisor[0]).GetAttribute("Rfc");
                m.pesokg = float.Parse(XmlAttributeExtensions.GetValue(((XmlElement)mercancia[c]), "PesoEnKg"));// ((XmlElement)emisor[0]).GetAttribute("Nombre");
                m.bienes = XmlAttributeExtensions.GetValue(((XmlElement)mercancia[c]), "BienesTransp");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");
                m.descripcion = XmlAttributeExtensions.GetValue(((XmlElement)mercancia[c]), "Descripcion");// ((XmlElement)emisor[0]).GetAttribute("Rfc");
                m.claveUnidad = XmlAttributeExtensions.GetValue(((XmlElement)mercancia[c]), "ClaveUnidad");// ((XmlElement)emisor[0]).GetAttribute("Nombre");
                //m.impoorte = XmlAttributeExtensions.GetValue(((XmlElement)mercancia[0]), "moneda");
                //if (xDoc.GetElementsByTagName("cartaporte20:CantidadTransporta") == null)
                //    return;
                //XmlNodeList transporta = xDoc.GetElementsByTagName("cartaporte20:CantidadTransporta");
                //m.cantidadt = decimal.Parse(XmlAttributeExtensions.GetValue(((XmlElement)transporta[c]), "Cantidad"));// ((XmlElement)emisor[0]).GetAttribute("Rfc");
                //m.idorigen = XmlAttributeExtensions.GetValue(((XmlElement)transporta[c]), "IDOrigen");// ((XmlElement)emisor[0]).GetAttribute("Nombre");
                //m.iddestino = XmlAttributeExtensions.GetValue(((XmlElement)transporta[c]), "IDDestino");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");

                _templatePDF.mercancias.Add(m);
                c++;
            }





        }

        private void ObtenerNodoAutotransporte()
        {
            autotransporte a;

            if (xDoc.GetElementsByTagName("cartaporte20:Autotransporte") == null)
                return;
            XmlNodeList autotransporte = xDoc.GetElementsByTagName("cartaporte20:Autotransporte");
            _templatePDF.auto.NumPermiso = XmlAttributeExtensions.GetValue(((XmlElement)autotransporte[0]), "NumPermisoSCT");// ((XmlElement)emisor[0]).GetAttribute("Rfc");
            _templatePDF.auto.permisoSCT = XmlAttributeExtensions.GetValue(((XmlElement)autotransporte[0]), "PermSCT");// ((XmlElement)emisor[0]).GetAttribute("Nombre");

            if (xDoc.GetElementsByTagName("cartaporte20:IdentificacionVehicular") == null)
                return;
            XmlNodeList identificacion = xDoc.GetElementsByTagName("cartaporte20:IdentificacionVehicular");
            _templatePDF.auto.anioMod = XmlAttributeExtensions.GetValue(((XmlElement)identificacion[0]), "AnioModeloVM");// ((XmlElement)emisor[0]).GetAttribute("Rfc");
            _templatePDF.auto.configV = XmlAttributeExtensions.GetValue(((XmlElement)identificacion[0]), "ConfigVehicular");// ((XmlElement)emisor[0]).GetAttribute("Nombre");
            _templatePDF.auto.placaVM = XmlAttributeExtensions.GetValue(((XmlElement)identificacion[0]), "PlacaVM");// ((XmlElement)emisor[0]).GetAttribute("Rfc");

            if (xDoc.GetElementsByTagName("cartaporte20:Seguros") == null)
                return;
            XmlNodeList seguro = xDoc.GetElementsByTagName("cartaporte20:Seguros");
            _templatePDF.auto.polizaRC = XmlAttributeExtensions.GetValue(((XmlElement)seguro[0]), "PolizaRespCivil");// ((XmlElement)emisor[0]).GetAttribute("Rfc");
            _templatePDF.auto.aseguraRC = XmlAttributeExtensions.GetValue(((XmlElement)seguro[0]), "AseguraRespCivil");// ((XmlElement)emisor[0]).GetAttribute("Nombre");
            _templatePDF.auto.aseguraCarga = XmlAttributeExtensions.GetValue(((XmlElement)seguro[0]), "AseguraCarga");// ((XmlElement)emisor[0]).GetAttribute("Rfc");


        }

        private void ObtenerNodoTiposFigura()
        {
            XmlNodeList figura = xDoc.GetElementsByTagName("cfdi:Receptor");
            _templatePDF.figura.Tipofigura = XmlAttributeExtensions.GetValue(((XmlElement)figura[0]), "TipoFigura"); //((XmlElement)receptor[0]).GetAttribute("Nombre");
            _templatePDF.figura.RFCfigura = XmlAttributeExtensions.GetValue(((XmlElement)figura[0]), "RFCFigura"); //((XmlElement)receptor[0]).GetAttribute("Rfc");
            _templatePDF.figura.numLic = XmlAttributeExtensions.GetValue(((XmlElement)figura[0]), "NumLicencia"); //(XmlElement)receptor[0]).GetAttribute("UsoCFDI");

        }


        private void ObtenerNodoComplementoDigital()
        {
            XmlNodeList tfDigital = xDoc.GetElementsByTagName("tfd:TimbreFiscalDigital");
            if (tfDigital.Count <= 0)
                return;

            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "UUID")/*((XmlElement)tfDigital[0]).GetAttribute("UUID")*/ != null)
                _templatePDF.folioFiscalUUID = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "UUID");// ((XmlElement)tfDigital[0]).GetAttribute("UUID");
            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "FechaTimbrado")/*((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado")*/ != null)
                _templatePDF.fechaCertificacion = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "FechaTimbrado");// ((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado");
            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "RfcProvCertif")/*((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado")*/ != null)
                _templatePDF.rfcprovCertif = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "RfcProvCertif");// ((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado");
            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "SelloCFD")/*((XmlElement)tfDigital[0]).GetAttribute("SelloSAT")*/ != null)
                _templatePDF.selloDigitalCFDI = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "SelloCFD");// ((XmlElement)tfDigital[0]).GetAttribute("SelloSAT");
            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "SelloSAT")/*((XmlElement)tfDigital[0]).GetAttribute("SelloSAT")*/ != null)
                _templatePDF.selloDigitalSAT = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "SelloSAT");// ((XmlElement)tfDigital[0]).GetAttribute("SelloSAT");
            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "NoCertificadoSAT")/*((XmlElement)tfDigital[0]).GetAttribute("NoCertificadoSAT")*/ != null)
                _templatePDF.noSerieCertificadoSAT = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "NoCertificadoSAT");// ((XmlElement)tfDigital[0]).GetAttribute("NoCertificadoSAT");

        }






        #endregion

        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {

        }

        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;
            //_cb.SaveState();
            //_cb.BeginText();
            //_cb.MoveText(1, 1);
            //_cb.SetFontAndSize(_fuenteTitulos, 8);
            //_cb.ShowText("Faustino Rojas Arelano");
            //_cb.EndText();
            //_cb.RestoreState();

            colorf = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            CMYKColor colordefinidoX = colorf.color;

            //Agrego cuadro al documento
            _cb.SetColorStroke(new BaseColor(colordefinidoX.R, colordefinidoX.G, colordefinidoX.B, colordefinidoX.A)); //Color de la linea
            _cb.SetColorFill(new BaseColor(colordefinidoX.R, colordefinidoX.G, colordefinidoX.B, colordefinidoX.A)); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(410, 730, 10, 70);
            _cb.FillStroke();
        }

        private void AgregarDatosEmisor(String Telefono, System.Drawing.Image logoEmpresa)
        {
            //Agrega logo en la primer columna
            Empresa empresa = new EmpresaContext().empresas.Find(2);

            //Datos del receptor
            float[] anchoColumnaencabezadogral = { 90f, 200f, 320f, 170f };
            PdfPTable tablaencagral = new PdfPTable(anchoColumnaencabezadogral);
            tablaencagral.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaencagral.SetTotalWidth(anchoColumnaencabezadogral);
            tablaencagral.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencagral.LockedWidth = true;

            // logo en la primer columna

            if (logoEmpresa == null)
                return;
            //Agrego la imagen al documento
            //Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
            //imagen.ScaleToFit(140,100);
            //imagen.Alignment = Element.ALIGN_TOP;
            //Chunk logo = new Chunk(imagen, 1, -40);
            //_documento.Add(logo);

            try
            {
                Image jpg = iTextSharp.text.Image.GetInstance(logoEmpresa, System.Drawing.Imaging.ImageFormat.Jpeg); // Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/logo.jpg"));

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(30f, 50F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(30f, 670f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                tablaencagral.AddCell(jpg);
                //  doc.Add(paragraph);
            }
            catch (Exception err)
            {
                string MENSAJEDEERROR = err.Message;
                tablaencagral.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            }
            Paragraph p11 = new Paragraph();

            string direccionC = "CALLE " + empresa.Calle + " NO. " + empresa.NoExt + ", COL. " + empresa.Colonia + "";

            string muni = empresa.Municipio + ", ESTADO DE MEXICO, CP. " + empresa.CP;

            p11.Add(new Phrase(_templatePDF.emisor.Nombre, new Font(Font.FontFamily.HELVETICA, 8)));
            p11.Add("\n");
            p11.Add(new Phrase("" + _templatePDF.emisor.rfc, new Font(Font.FontFamily.HELVETICA, 8)));
            p11.Add("\n");
            p11.Add(new Phrase("PLANTA\n", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            p11.Add(new Phrase(direccionC.ToUpperInvariant(), new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL)));
            p11.Add("\n");
            p11.Add(new Phrase(muni.ToUpperInvariant(), new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL)));
            p11.Add("\n");
            p11.Add(new Phrase("TELEFONO " + empresa.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p11.Add("\n");
            String regimen_fiscal = DecodificadorSAT.getRegimen(_templatePDF.emisor.RegimenFiscal);


            p11.Add(new Phrase(regimen_fiscal, new Font(Font.FontFamily.HELVETICA, 6)));



            tablaencagral.AddCell(p11);

            Paragraph p1 = new Paragraph();

            p1.Alignment = Element.ALIGN_CENTER;
            if (_templatePDF.TipoDeRelacion == String.Empty)
            {
                p1.Add(new Phrase("FACTURA : " + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            }

            p1.Add(new Phrase("\nTIPO COMPROBANTE : " + _templatePDF.TipoDecomprobrante, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p1.Add("\n");
            p1.Add(new Phrase("FOLIO FISCAL (UUID): ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p1.Add("\n");
            p1.Add(new Phrase(_templatePDF.folioFiscalUUID, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            p1.Add(new Phrase("NO. DE SERIE DEL CERTIFICADO DEL SAT:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p1.Add("\n");
            p1.Add(new Phrase(_templatePDF.noSerieCertificadoSAT, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            tablaencagral.AddCell(p1);





            Paragraph p2 = new Paragraph();



            p2.Alignment = Element.ALIGN_CENTER;


            p2.Add(new Phrase("NO. DE SERIE DEL CERTIFICADO DEL EMISOR:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(_templatePDF.noSerieCertificadoEmisor, new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");
            p2.Add(new Phrase("FECHA Y HORA DE CERTIFICACIÓN:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(_templatePDF.fechaCertificacion, new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");
            p2.Add(new Phrase("FECHA Y HORA DE EMISIÓN DE CFDI:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(_templatePDF.fechaEmisionCFDI, new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");
            p2.Add("\n");

            PdfPCell celda0 = new PdfPCell(p2);
            celda0.HorizontalAlignment = Element.ALIGN_CENTER;
            celda0.Border = 0;

            tablaencagral.AddCell(celda0);


            _documento.Add(tablaencagral);


        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("FACTURA CFDI");
            _documento.AddSubject("DOCUMENTO CREADO APARTIR DE UN XML");
            _documento.AddTitle("FACTURA");
            _documento.SetMargins(5, 5, 5, 5);
        }

        private void AgregarDatosFactura()
        {
            //Datos de la factura

        }


        private void AgregarDatosCP(int IDOperador)
        {


            CMYKColor colorTITULO = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
            VOperadores operador = new ChoferesContext().VOperadores.Find(IDOperador);

            float[] tamanoColumnasPrincipal = { 780f };
            PdfPTable tabla = new PdfPTable(tamanoColumnasPrincipal);
            tabla.SetTotalWidth(tamanoColumnasPrincipal);
            tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.LockedWidth = true;

            //float[] tamanoColumnastitulos = { 50f, 340f, 50f, 340f };


            //float[] tamanoColumnasVacioa = { 780f };

            float[] tamanoColumnasUno = { 780f };

            float[] tamanoColumnasDos = { 390f, 390f };

            float[] tamanoColumnastres = { 280f, 280f, 220f };

            float[] tamanoColumnascuatro = { 100f, 70f, 200f, 410f };

            float[] tamanoColumnas7 = { 100f, 110f, 120f, 130f, 90f, 100f, 130f };

            float[] tamanoColumnas7s = { 50f, 120f, 120f, 120f, 100f, 130f, 140f };

            float[] tamanoColumnas11 = { 80f, 80f, 80f, 50f, 50f, 130f, 50f, 40f, 30f, 100f, 90f };

            float[] tamanoColumnas9s = { 60f, 120f, 60f, 40f, 130f, 80f, 70f, 120f, 100f };

            //float[] tamanoColumnasF = { 100f, 100f, 100f, 100f, 200f, 180f };

            float[] tamanoColumnas9 = { 120, 140f, 50f, 80f, 100f, 100f, 120f, 50f, 20f };

            //string Cadenafactura = string.Empty;

            PdfPTable tablaTitulo = new PdfPTable(tamanoColumnasUno);
            tablaTitulo.SetTotalWidth(tamanoColumnasUno);
            tablaTitulo.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaTitulo.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaTitulo.LockedWidth = true;
            tablaTitulo.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPCell cell0 = new PdfPCell(new Phrase("COMPLEMENTO CARTA PORTE\n", new Font(Font.FontFamily.HELVETICA, 15, Font.BOLD)));
            //cell0.BackgroundColor = colordefinido;
            cell0.HorizontalAlignment = Element.ALIGN_CENTER;
            cell0.Border = Rectangle.NO_BORDER;
            tablaTitulo.AddCell(cell0);

            PdfPTable tablados = new PdfPTable(tamanoColumnasPrincipal);
            tablados.SetTotalWidth(tamanoColumnasPrincipal);
            tablados.HorizontalAlignment = Element.ALIGN_CENTER;
            tablados.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablados.LockedWidth = true;
            tablados.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPCell cell1 = new PdfPCell(new Phrase("Carta Porte", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.Border = Rectangle.NO_BORDER;

            tablados.AddCell(cell1);

            PdfPTable tablaCartaPorte = new PdfPTable(tamanoColumnas7s);
            tablaCartaPorte.SetTotalWidth(tamanoColumnas7s);
            tablaCartaPorte.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaCartaPorte.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaCartaPorte.LockedWidth = true;
            tablaCartaPorte.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell cellV = new PdfPCell(new Phrase("Versión", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell cellT = new PdfPCell(new Phrase("Translado Internacional", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell cellD = new PdfPCell(new Phrase("Total Distancia Recorrida", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell cellP = new PdfPCell(new Phrase("Peso Total Mercancía", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell cellN = new PdfPCell(new Phrase("No. Total Mercancías", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell cellC = new PdfPCell(new Phrase("Clave Transporte", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell cell = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

            cellV.BackgroundColor = colordefinido;
            cellT.BackgroundColor = colordefinido;
            cellD.BackgroundColor = colordefinido;
            cellP.BackgroundColor = colordefinido;
            cellN.BackgroundColor = colordefinido;
            cellC.BackgroundColor = colordefinido;
            cell.BackgroundColor = colordefinido;

            PdfPCell cellVD = new PdfPCell(new Phrase(_templatePDF.cp.version, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell cellTD = new PdfPCell(new Phrase(_templatePDF.cp.transporteIn, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell cellDD = new PdfPCell(new Phrase(_templatePDF.cp.distanciaRecorrida, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell cellPD = new PdfPCell(new Phrase(_templatePDF.mercan.pesoBT.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell cellND = new PdfPCell(new Phrase(_templatePDF.mercan.totalMer.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell cellCD = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell cellDa = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));

            tablaCartaPorte.AddCell(cellV);
            tablaCartaPorte.AddCell(cellT);
            tablaCartaPorte.AddCell(cellD);
            tablaCartaPorte.AddCell(cellP);
            tablaCartaPorte.AddCell(cellN);
            tablaCartaPorte.AddCell(cellC);
            tablaCartaPorte.AddCell(cell);

            tablaCartaPorte.AddCell(cellVD);
            tablaCartaPorte.AddCell(cellTD);
            tablaCartaPorte.AddCell(cellDD);
            tablaCartaPorte.AddCell(cellPD);
            tablaCartaPorte.AddCell(cellND);
            tablaCartaPorte.AddCell(cellCD);
            tablaCartaPorte.AddCell(cellDa);

            PdfPTable tablaDiv = new PdfPTable(tamanoColumnasDos);
            tablaDiv.SetTotalWidth(tamanoColumnasDos);
            tablaDiv.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaDiv.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaDiv.LockedWidth = true;
            tablaDiv.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell cellda = new PdfPCell(new Phrase("Remitente", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celldados = new PdfPCell(new Phrase("Destinatario", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            cellda.BackgroundColor = BaseColor.LIGHT_GRAY;
            celldados.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellda.HorizontalAlignment = Element.ALIGN_CENTER;
            celldados.HorizontalAlignment = Element.ALIGN_CENTER;

            cellda.Border = Rectangle.NO_BORDER;
            celldados.Border = Rectangle.NO_BORDER;

            tablaDiv.AddCell(cellda);
            tablaDiv.AddCell(celldados);

            PdfPTable tablaReDes = new PdfPTable(tamanoColumnas7);
            tablaReDes.SetTotalWidth(tamanoColumnas7);
            tablaReDes.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaReDes.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaReDes.LockedWidth = true;

            PdfPCell celdarr = new PdfPCell(new Phrase("RFC Remitente", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdanr = new PdfPCell(new Phrase("Nombre Remitente", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdafhs = new PdfPCell(new Phrase("Fecha y Hora Salida", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdard = new PdfPCell(new Phrase("RFC Destinatario", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdand = new PdfPCell(new Phrase("Nombre Destinatario", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdafhll = new PdfPCell(new Phrase("Fecha y Hora Llegada", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

            celdarr.BackgroundColor = colordefinido;
            celdanr.BackgroundColor = colordefinido;
            celdafhs.BackgroundColor = colordefinido;
            celda.BackgroundColor = colordefinido;
            celdard.BackgroundColor = colordefinido;
            celdand.BackgroundColor = colordefinido;
            celdafhll.BackgroundColor = colordefinido;

            PdfPCell celdarrD = new PdfPCell(new Phrase(_templatePDF.emisor.rfc, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdanrD = new PdfPCell(new Phrase(_templatePDF.emisor.Nombre, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdafhsD = new PdfPCell(new Phrase(FechaSalida, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaD = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdardD = new PdfPCell(new Phrase(_templatePDF.receptor.rfc, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdandD = new PdfPCell(new Phrase(_templatePDF.receptor.razonSocial, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdafhllD = new PdfPCell(new Phrase(FechaLlegada, new Font(Font.FontFamily.HELVETICA, 8)));

            tablaReDes.AddCell(celdarr);
            tablaReDes.AddCell(celdanr);
            tablaReDes.AddCell(celdafhs);
            tablaReDes.AddCell(celda);
            tablaReDes.AddCell(celdard);
            tablaReDes.AddCell(celdand);
            tablaReDes.AddCell(celdafhll);

            tablaReDes.AddCell(celdarrD);
            tablaReDes.AddCell(celdanrD);
            tablaReDes.AddCell(celdafhsD);
            tablaReDes.AddCell(celdaD);
            tablaReDes.AddCell(celdardD);
            tablaReDes.AddCell(celdandD);
            tablaReDes.AddCell(celdafhllD);

            PdfPTable tablaUbicacionO = new PdfPTable(tamanoColumnasUno);
            tablaUbicacionO.SetTotalWidth(tamanoColumnasUno);
            tablaUbicacionO.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaUbicacionO.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaUbicacionO.LockedWidth = true;
            int contador = 0;
            PdfPTable tablaUbicacionD = new PdfPTable(tamanoColumnasUno);
            tablaUbicacionD.SetTotalWidth(tamanoColumnasUno);
            tablaUbicacionD.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaUbicacionD.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaUbicacionD.LockedWidth = true;
            tablaUbicacionD.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPTable tablaUD = new PdfPTable(tamanoColumnas11);
            tablaUD.SetTotalWidth(tamanoColumnas11);
            tablaUD.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaUD.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaUD.LockedWidth = true;
            tablaUD.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPTable tablaUO = new PdfPTable(tamanoColumnas11);
            tablaUO.SetTotalWidth(tamanoColumnas11);
            tablaUO.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaUO.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaUO.LockedWidth = true;
            tablaUO.DefaultCell.Border = Rectangle.NO_BORDER;
            foreach (Ubicaciones p in _templatePDF.ubicacion)
            {
                if (contador == 0)
                {
                    string obtenerid = p.IDUbicacion.Remove(0, 1);
                    string remover = obtenerid.Remove(0, 1);
                    string idubi = remover.TrimStart('0');
                    int IDOrigen = int.Parse(idubi);

                    Origen vorigen = new OrigenContext().Origen.Find(IDOrigen);



                    PdfPCell celda1 = new PdfPCell(new Phrase("Ubicación Origen", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


                    celda1.BackgroundColor = BaseColor.LIGHT_GRAY;
                    celda1.HorizontalAlignment = Element.ALIGN_CENTER;
                    celda1.Border = Rectangle.NO_BORDER;

                    tablaUbicacionO.AddCell(celda1);



                    PdfPCell celdauo1 = new PdfPCell(new Phrase("Calle", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo2 = new PdfPCell(new Phrase("Número Exterior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo3 = new PdfPCell(new Phrase("Numero Interior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo4 = new PdfPCell(new Phrase("Colonia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo5 = new PdfPCell(new Phrase("Localidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo6 = new PdfPCell(new Phrase("Referencia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo7 = new PdfPCell(new Phrase("Municipio", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo8 = new PdfPCell(new Phrase("Estado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo9 = new PdfPCell(new Phrase("Pais", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo10 = new PdfPCell(new Phrase("Código Postal", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdauo11 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

                    celdauo1.BackgroundColor = colordefinido;
                    celdauo2.BackgroundColor = colordefinido;
                    celdauo3.BackgroundColor = colordefinido;
                    celdauo4.BackgroundColor = colordefinido;
                    celdauo5.BackgroundColor = colordefinido;
                    celdauo6.BackgroundColor = colordefinido;
                    celdauo7.BackgroundColor = colordefinido;
                    celdauo8.BackgroundColor = colordefinido;
                    celdauo9.BackgroundColor = colordefinido;
                    celdauo10.BackgroundColor = colordefinido;
                    celdauo11.BackgroundColor = colordefinido;

                    PdfPCell celdauo1D = new PdfPCell(new Phrase(vorigen.Calle, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo2D = new PdfPCell(new Phrase(vorigen.NumExt, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo3D = new PdfPCell(new Phrase(vorigen.NumInt, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo4D = new PdfPCell(new Phrase(vorigen.c_Colonia, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo5D = new PdfPCell(new Phrase(vorigen.c_Localidad, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo6D = new PdfPCell(new Phrase(vorigen.Referencia, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo7D = new PdfPCell(new Phrase(vorigen.c_Municipio, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo8D = new PdfPCell(new Phrase(vorigen.c_Estado, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo9D = new PdfPCell(new Phrase(vorigen.c_Pais, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo10D = new PdfPCell(new Phrase(vorigen.CP, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauo11D = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));

                    tablaUO.AddCell(celdauo1);
                    tablaUO.AddCell(celdauo2);
                    tablaUO.AddCell(celdauo3);
                    tablaUO.AddCell(celdauo4);
                    tablaUO.AddCell(celdauo5);
                    tablaUO.AddCell(celdauo6);
                    tablaUO.AddCell(celdauo7);
                    tablaUO.AddCell(celdauo8);
                    tablaUO.AddCell(celdauo9);
                    tablaUO.AddCell(celdauo10);
                    tablaUO.AddCell(celdauo11);

                    tablaUO.AddCell(celdauo1D);
                    tablaUO.AddCell(celdauo2D);
                    tablaUO.AddCell(celdauo3D);
                    tablaUO.AddCell(celdauo4D);
                    tablaUO.AddCell(celdauo5D);
                    tablaUO.AddCell(celdauo6D);
                    tablaUO.AddCell(celdauo7D);
                    tablaUO.AddCell(celdauo8D);
                    tablaUO.AddCell(celdauo9D);
                    tablaUO.AddCell(celdauo10D);
                    tablaUO.AddCell(celdauo11D);

                }
                else
                {

                    string obtenerid = p.IDUbicacion.Remove(0, 1);
                    string remover = obtenerid.Remove(0, 1);
                    string idubi = remover.TrimStart('0');
                    int IDOrigen = int.Parse(idubi);

                    Entrega vorigen = new EntregaContext().Entregas.Find(IDOrigen);

                    PdfPCell celda2 = new PdfPCell(new Phrase("Ubicación Destino", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


                    celda2.BackgroundColor = BaseColor.LIGHT_GRAY;
                    celda2.HorizontalAlignment = Element.ALIGN_CENTER;
                    celda2.Border = Rectangle.NO_BORDER;

                    tablaUbicacionD.AddCell(celda2);



                    PdfPCell celdaud1 = new PdfPCell(new Phrase("Calle", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud2 = new PdfPCell(new Phrase("Número Exterior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud3 = new PdfPCell(new Phrase("Numero Interior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud4 = new PdfPCell(new Phrase("Colonia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud5 = new PdfPCell(new Phrase("Localidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud6 = new PdfPCell(new Phrase("Referencia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud7 = new PdfPCell(new Phrase("Municipio", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud8 = new PdfPCell(new Phrase("Estado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud9 = new PdfPCell(new Phrase("Pais", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud10 = new PdfPCell(new Phrase("Código Postal", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                    PdfPCell celdaud11 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

                    celdaud1.BackgroundColor = colordefinido;
                    celdaud2.BackgroundColor = colordefinido;
                    celdaud3.BackgroundColor = colordefinido;
                    celdaud4.BackgroundColor = colordefinido;
                    celdaud5.BackgroundColor = colordefinido;
                    celdaud6.BackgroundColor = colordefinido;
                    celdaud7.BackgroundColor = colordefinido;
                    celdaud8.BackgroundColor = colordefinido;
                    celdaud9.BackgroundColor = colordefinido;
                    celdaud10.BackgroundColor = colordefinido;
                    celdaud11.BackgroundColor = colordefinido;

                    PdfPCell celdaud1D = new PdfPCell(new Phrase(vorigen.CalleEntrega, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud2D = new PdfPCell(new Phrase(vorigen.NumExtEntrega, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud3D = new PdfPCell(new Phrase(vorigen.NumIntentrega, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud4D = new PdfPCell(new Phrase(vorigen.c_Colonia, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud5D = new PdfPCell(new Phrase(vorigen.c_Localidad, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud6D = new PdfPCell(new Phrase(vorigen.Referencia, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud7D = new PdfPCell(new Phrase(vorigen.c_Municipio, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud8D = new PdfPCell(new Phrase(vorigen.c_Estado, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud9D = new PdfPCell(new Phrase(vorigen.c_Pais, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud10D = new PdfPCell(new Phrase(vorigen.CPEntrega, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdaud11D = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));

                    tablaUD.AddCell(celdaud1);
                    tablaUD.AddCell(celdaud2);
                    tablaUD.AddCell(celdaud3);
                    tablaUD.AddCell(celdaud4);
                    tablaUD.AddCell(celdaud5);
                    tablaUD.AddCell(celdaud6);
                    tablaUD.AddCell(celdaud7);
                    tablaUD.AddCell(celdaud8);
                    tablaUD.AddCell(celdaud9);
                    tablaUD.AddCell(celdaud10);
                    tablaUD.AddCell(celdaud11);

                    tablaUD.AddCell(celdaud1D);
                    tablaUD.AddCell(celdaud2D);
                    tablaUD.AddCell(celdaud3D);
                    tablaUD.AddCell(celdaud4D);
                    tablaUD.AddCell(celdaud5D);
                    tablaUD.AddCell(celdaud6D);
                    tablaUD.AddCell(celdaud7D);
                    tablaUD.AddCell(celdaud8D);
                    tablaUD.AddCell(celdaud9D);
                    tablaUD.AddCell(celdaud10D);
                    tablaUD.AddCell(celdaud11D);
                }
                contador++;
            }



            PdfPTable tablaM = new PdfPTable(tamanoColumnasUno);
            tablaM.SetTotalWidth(tamanoColumnasUno);
            tablaM.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaM.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaM.LockedWidth = true;
            tablaM.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPCell celda3 = new PdfPCell(new Phrase("Mercancías", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


            celda3.BackgroundColor = BaseColor.LIGHT_GRAY;
            celda3.HorizontalAlignment = Element.ALIGN_CENTER;
            celda3.Border = Rectangle.NO_BORDER;

            tablaM.AddCell(celda3);



            PdfPTable tablaMercancia = new PdfPTable(tamanoColumnas9);
            tablaMercancia.SetTotalWidth(tamanoColumnas9);
            tablaMercancia.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaMercancia.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaMercancia.LockedWidth = true;
            foreach (Mercancias p in _templatePDF.mercancias)
            {
                PdfPCell celdam1 = new PdfPCell(new Phrase("Bienes Transportados", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                PdfPCell celdam2 = new PdfPCell(new Phrase("Descripción", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                PdfPCell celdam3 = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                PdfPCell celdam4 = new PdfPCell(new Phrase("Clave Unidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                PdfPCell celdam5 = new PdfPCell(new Phrase("Material Peligroso", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                PdfPCell celdam6 = new PdfPCell(new Phrase("Peso en Kg", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                PdfPCell celdam7 = new PdfPCell(new Phrase("Valor Mercancia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                PdfPCell celdam8 = new PdfPCell(new Phrase("Moneda", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
                PdfPCell celdam9 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

                celdam1.BackgroundColor = colordefinido;
                celdam2.BackgroundColor = colordefinido;
                celdam3.BackgroundColor = colordefinido;
                celdam4.BackgroundColor = colordefinido;
                celdam5.BackgroundColor = colordefinido;
                celdam6.BackgroundColor = colordefinido;
                celdam7.BackgroundColor = colordefinido;
                celdam8.BackgroundColor = colordefinido;
                celdam9.BackgroundColor = colordefinido;

                PdfPCell celdam1D = new PdfPCell(new Phrase(p.bienes, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdam2D = new PdfPCell(new Phrase(p.descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdam3D = new PdfPCell(new Phrase(p.cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdam4D = new PdfPCell(new Phrase(p.claveUnidad, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdam5D = new PdfPCell(new Phrase("No ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell celdam6D = new PdfPCell(new Phrase(p.pesokg.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdam7D = new PdfPCell(new Phrase(p.totalMer.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdam8D = new PdfPCell(new Phrase(p.moneda, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdam9D = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));


                tablaMercancia.AddCell(celdam1);
                tablaMercancia.AddCell(celdam2);
                tablaMercancia.AddCell(celdam3);
                tablaMercancia.AddCell(celdam4);
                tablaMercancia.AddCell(celdam5);
                tablaMercancia.AddCell(celdam6);
                tablaMercancia.AddCell(celdam7);
                tablaMercancia.AddCell(celdam8);
                tablaMercancia.AddCell(celdam9);

                tablaMercancia.AddCell(celdam1D);
                tablaMercancia.AddCell(celdam2D);
                tablaMercancia.AddCell(celdam3D);
                tablaMercancia.AddCell(celdam4D);
                tablaMercancia.AddCell(celdam5D);
                tablaMercancia.AddCell(celdam6D);
                tablaMercancia.AddCell(celdam7D);
                tablaMercancia.AddCell(celdam8D);
                tablaMercancia.AddCell(celdam9D);
            }



            PdfPTable tablaAIR = new PdfPTable(tamanoColumnastres);
            tablaAIR.SetTotalWidth(tamanoColumnastres);
            tablaAIR.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaAIR.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaAIR.LockedWidth = true;
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell celda4 = new PdfPCell(new Phrase("Autotransporte Federal", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda5 = new PdfPCell(new Phrase("Identificación Vehicular", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda6 = new PdfPCell(new Phrase("Remolque", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            celda4.BackgroundColor = BaseColor.LIGHT_GRAY;
            celda4.HorizontalAlignment = Element.ALIGN_CENTER;
            celda4.Border = Rectangle.NO_BORDER;

            celda5.BackgroundColor = BaseColor.LIGHT_GRAY;
            celda5.HorizontalAlignment = Element.ALIGN_CENTER;
            celda5.Border = Rectangle.NO_BORDER;

            celda6.BackgroundColor = BaseColor.LIGHT_GRAY;
            celda6.HorizontalAlignment = Element.ALIGN_CENTER;
            celda6.Border = Rectangle.NO_BORDER;

            tablaAIR.AddCell(celda4);
            tablaAIR.AddCell(celda5);
            tablaAIR.AddCell(celda6);



            PdfPTable tablaAUVRe = new PdfPTable(tamanoColumnas9s);
            tablaAUVRe.SetTotalWidth(tamanoColumnas9s);
            tablaAUVRe.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaAUVRe.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaAUVRe.LockedWidth = true;

            PdfPCell celdaauvre1 = new PdfPCell(new Phrase("Permiso SCT", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaauvre2 = new PdfPCell(new Phrase("No. Permiso SCT", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaauvre3 = new PdfPCell(new Phrase("Nombre Aseguradora", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaauvre4 = new PdfPCell(new Phrase("Poliza Seguro", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaauvre5 = new PdfPCell(new Phrase("Configuración Vehicular", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaauvre6 = new PdfPCell(new Phrase("Placa Vehicular", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaauvre7 = new PdfPCell(new Phrase("Año Modelo", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaauvre8 = new PdfPCell(new Phrase("SubTipo Remolque", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaauvre9 = new PdfPCell(new Phrase("Placa", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

            celdaauvre1.BackgroundColor = colordefinido;
            celdaauvre2.BackgroundColor = colordefinido;
            celdaauvre3.BackgroundColor = colordefinido;
            celdaauvre4.BackgroundColor = colordefinido;
            celdaauvre5.BackgroundColor = colordefinido;
            celdaauvre6.BackgroundColor = colordefinido;
            celdaauvre7.BackgroundColor = colordefinido;
            celdaauvre8.BackgroundColor = colordefinido;
            celdaauvre9.BackgroundColor = colordefinido;

            PdfPCell celdaauvre1D = new PdfPCell(new Phrase(_templatePDF.auto.permisoSCT, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaauvre2D = new PdfPCell(new Phrase(_templatePDF.auto.NumPermiso, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaauvre3D = new PdfPCell(new Phrase(_templatePDF.auto.aseguraRC, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaauvre4D = new PdfPCell(new Phrase(_templatePDF.auto.polizaRC, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaauvre5D = new PdfPCell(new Phrase(_templatePDF.auto.configV, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaauvre6D = new PdfPCell(new Phrase(_templatePDF.auto.placaVM, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaauvre7D = new PdfPCell(new Phrase(_templatePDF.auto.anioMod, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaauvre8D = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaauvre9D = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));


            tablaAUVRe.AddCell(celdaauvre1);
            tablaAUVRe.AddCell(celdaauvre2);
            tablaAUVRe.AddCell(celdaauvre3);
            tablaAUVRe.AddCell(celdaauvre4);
            tablaAUVRe.AddCell(celdaauvre5);
            tablaAUVRe.AddCell(celdaauvre6);
            tablaAUVRe.AddCell(celdaauvre7);
            tablaAUVRe.AddCell(celdaauvre8);
            tablaAUVRe.AddCell(celdaauvre9);

            tablaAUVRe.AddCell(celdaauvre1D);
            tablaAUVRe.AddCell(celdaauvre2D);
            tablaAUVRe.AddCell(celdaauvre3D);
            tablaAUVRe.AddCell(celdaauvre4D);
            tablaAUVRe.AddCell(celdaauvre5D);
            tablaAUVRe.AddCell(celdaauvre6D);
            tablaAUVRe.AddCell(celdaauvre7D);
            tablaAUVRe.AddCell(celdaauvre8D);
            tablaAUVRe.AddCell(celdaauvre9D);

            PdfPTable tablaop = new PdfPTable(tamanoColumnasUno);
            tablaop.SetTotalWidth(tamanoColumnasUno);
            tablaop.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaop.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaop.LockedWidth = true;
            tablaop.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPCell cellda4 = new PdfPCell(new Phrase("Operador", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            cellda4.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellda4.HorizontalAlignment = Element.ALIGN_CENTER;
            cellda4.Border = Rectangle.NO_BORDER;

            tablaop.AddCell(cellda4);

            PdfPTable tablaOperador = new PdfPTable(tamanoColumnascuatro);
            tablaOperador.SetTotalWidth(tamanoColumnascuatro);
            tablaOperador.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaOperador.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaOperador.LockedWidth = true;
            tablaOperador.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell celdaop1 = new PdfPCell(new Phrase("RFC Operador", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaop2 = new PdfPCell(new Phrase("Número Licencia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaop3 = new PdfPCell(new Phrase("Nombre Operador", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaop4 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

            celdaop1.BackgroundColor = colordefinido;
            celdaop2.BackgroundColor = colordefinido;
            celdaop3.BackgroundColor = colordefinido;
            celdaop4.BackgroundColor = colordefinido;

            PdfPCell celdaop1D = new PdfPCell(new Phrase(operador.RFC, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaop2D = new PdfPCell(new Phrase(operador.NoLicencia, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaop3D = new PdfPCell(new Phrase(operador.Nombre, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaop4D = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));


            tablaOperador.AddCell(celdaop1);
            tablaOperador.AddCell(celdaop2);
            tablaOperador.AddCell(celdaop3);
            tablaOperador.AddCell(celdaop4);

            tablaOperador.AddCell(celdaop1D);
            tablaOperador.AddCell(celdaop2D);
            tablaOperador.AddCell(celdaop3D);
            tablaOperador.AddCell(celdaop4D);

            PdfPTable tablaDomicilioOp = new PdfPTable(tamanoColumnasUno);
            tablaDomicilioOp.SetTotalWidth(tamanoColumnasUno);
            tablaDomicilioOp.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaDomicilioOp.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaDomicilioOp.LockedWidth = true;
            tablaDomicilioOp.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell celda7 = new PdfPCell(new Phrase("Domicilio Operador", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            celda7.BackgroundColor = BaseColor.LIGHT_GRAY;
            celda7.HorizontalAlignment = Element.ALIGN_CENTER;
            celda7.Border = Rectangle.NO_BORDER;

            tablaDomicilioOp.AddCell(celda7);

            PdfPTable tablaDomOp = new PdfPTable(tamanoColumnas11);
            tablaDomOp.SetTotalWidth(tamanoColumnas11);
            tablaDomOp.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaDomOp.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaDomOp.LockedWidth = true;

            PdfPCell celdado1 = new PdfPCell(new Phrase("Calle", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado2 = new PdfPCell(new Phrase("Número Exterior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado3 = new PdfPCell(new Phrase("Numero Interior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado4 = new PdfPCell(new Phrase("Colonia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado5 = new PdfPCell(new Phrase("Localidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado6 = new PdfPCell(new Phrase("Referencia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado7 = new PdfPCell(new Phrase("Municipio", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado8 = new PdfPCell(new Phrase("Estado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado9 = new PdfPCell(new Phrase("Pais", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado10 = new PdfPCell(new Phrase("Código Postal", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdado11 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

            celdado1.BackgroundColor = colordefinido;
            celdado2.BackgroundColor = colordefinido;
            celdado3.BackgroundColor = colordefinido;
            celdado4.BackgroundColor = colordefinido;
            celdado5.BackgroundColor = colordefinido;
            celdado6.BackgroundColor = colordefinido;
            celdado7.BackgroundColor = colordefinido;
            celdado8.BackgroundColor = colordefinido;
            celdado9.BackgroundColor = colordefinido;
            celdado10.BackgroundColor = colordefinido;
            celdado11.BackgroundColor = colordefinido;

            PdfPCell celdado1D = new PdfPCell(new Phrase(operador.Calle, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado2D = new PdfPCell(new Phrase(operador.NumExt, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado3D = new PdfPCell(new Phrase(operador.NumInt, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado4D = new PdfPCell(new Phrase(operador.c_Colonia, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado5D = new PdfPCell(new Phrase(operador.c_Localidad, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado6D = new PdfPCell(new Phrase(operador.Referencia, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado7D = new PdfPCell(new Phrase(operador.c_Municipio, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado8D = new PdfPCell(new Phrase(operador.c_Estado, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado9D = new PdfPCell(new Phrase(operador.c_Pais, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado10D = new PdfPCell(new Phrase(operador.CP, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdado11D = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));

            tablaDomOp.AddCell(celdado1);
            tablaDomOp.AddCell(celdado2);
            tablaDomOp.AddCell(celdado3);
            tablaDomOp.AddCell(celdado4);
            tablaDomOp.AddCell(celdado5);
            tablaDomOp.AddCell(celdado6);
            tablaDomOp.AddCell(celdado7);
            tablaDomOp.AddCell(celdado8);
            tablaDomOp.AddCell(celdado9);
            tablaDomOp.AddCell(celdado10);
            tablaDomOp.AddCell(celdado11);

            tablaDomOp.AddCell(celdado1D);
            tablaDomOp.AddCell(celdado2D);
            tablaDomOp.AddCell(celdado3D);
            tablaDomOp.AddCell(celdado4D);
            tablaDomOp.AddCell(celdado5D);
            tablaDomOp.AddCell(celdado6D);
            tablaDomOp.AddCell(celdado7D);
            tablaDomOp.AddCell(celdado8D);
            tablaDomOp.AddCell(celdado9D);
            tablaDomOp.AddCell(celdado10D);
            tablaDomOp.AddCell(celdado11D);

            _documento.Add(tablaTitulo);
            _documento.Add(tablados);
            _documento.Add(tablaCartaPorte);
            _documento.Add(tablaDiv);
            _documento.Add(tablaReDes);
            _documento.Add(tablaUbicacionO);
            _documento.Add(tablaUO);
            _documento.Add(tablaUbicacionD);
            _documento.Add(tablaUD);
            _documento.Add(tablaM);
            _documento.Add(tablaMercancia);
            _documento.Add(tablaAIR);
            _documento.Add(tablaAUVRe);
            _documento.Add(tablaop);
            _documento.Add(tablaOperador);
            _documento.Add(tablaDomicilioOp);
            _documento.Add(tablaDomOp);

        }

        private String ReturnTiporelacion(String Clave)
        {
            return DecodificadorSAT.getTipoderelacion(Clave);
        }


        private void AgregarSellos()
        {
            StringBuilder cadenaOriginal = new StringBuilder();
            cadenaOriginal.Append("||");
            cadenaOriginal.Append("1.1|");
            cadenaOriginal.Append(_templatePDF.folioFiscalUUID + "|");
            cadenaOriginal.Append(_templatePDF.fechaEmisionCFDI.ToString() + "|");
            cadenaOriginal.Append(_templatePDF.selloDigitalSAT + "|");
            cadenaOriginal.Append(_templatePDF.noSerieCertificadoSAT + "||");

            float[] anchoColumnas = { 690f, 90f };
            PdfPTable tablaSellosQR = new PdfPTable(anchoColumnas);
            tablaSellosQR.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaSellosQR.SpacingBefore = 10.0f;
            tablaSellosQR.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaSellosQR.SetTotalWidth(anchoColumnas);
            //tablaSellosQR.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaSellosQR.LockedWidth = true;



            //Agregamos el codigo QR al documento
            StringBuilder codigoQR = new StringBuilder();
            codigoQR.Append("https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id=" + _templatePDF.folioFiscalUUID);
            codigoQR.Append("&re=" + _templatePDF.emisor.rfc); //RFC del Emisor
            codigoQR.Append("&rr=" + _templatePDF.receptor.rfc); //RFC del receptor
            codigoQR.Append("&tt=" + _templatePDF.total); //Total del comprobante 10 enteros y 6 decimales
            codigoQR.Append("&fe=" + _templatePDF.selloDigitalCFDI.Substring(_templatePDF.selloDigitalCFDI.Length - 8, 8)); //UUID del comprobante

            BarcodeQRCode pdfCodigoQR = new BarcodeQRCode(codigoQR.ToString(), 1, 1, null);
            Image img = pdfCodigoQR.GetImage();
            img.SpacingAfter = 0.0f;
            img.SpacingBefore = 0.0f;
            img.BorderWidth = 1.0f;

            //img.ScalePercent(100, 78);
            //img.border

            //tablaSellosQR.AddCell(tablaSellos);
            PdfPCell celdado11D = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));
            celdado11D.Border = Rectangle.NO_BORDER;
            tablaSellosQR.AddCell(celdado11D);

            tablaSellosQR.AddCell(img);

            _documento.Add(tablaSellosQR);

            //float[] anchoColumnasPie1 = { 780f };
            //PdfPTable tablaPie1 = new PdfPTable(anchoColumnasPie1);
            //tablaPie1.HorizontalAlignment = Element.ALIGN_RIGHT;
            //tablaPie1.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //tablaPie1.SpacingBefore = 10.0f;
            //tablaPie1.DefaultCell.Border = Rectangle.NO_BORDER;
            //tablaPie1.SetTotalWidth(anchoColumnasPie1);
            ////tablaSellosQR.HorizontalAlignment = Element.ALIGN_CENTER;
            //tablaPie1.LockedWidth = true;

            //tablaPie1.AddCell(new Phrase("\n\n\n\nTODOS LOS PEDIDOS PUEDEN TENER UNA VARIACION DE   +/-  10% EN LA FABRICACION.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            //tablaPie1.CompleteRow();
            //_documento.Add(tablaPie1);
            float[] anchoColumnasPie = { 330f, 80f, 330f };
            PdfPTable tablaPie = new PdfPTable(anchoColumnasPie);
            tablaPie.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaPie.DefaultCell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tablaPie.SpacingBefore = 10.0f;
            tablaPie.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaPie.SetTotalWidth(anchoColumnasPie);
            //tablaSellosQR.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaPie.LockedWidth = true;


            //tablaPie.AddCell(new Phrase("\n\n\n\nEN CASO DE MORAEL PAGO DEL IMPORTE TOTAL O PARCIAL DE LA PRESENTE FACTURA EL CLIENTE SE OBLIGA A PAGAR UN INTERES MORATORIO A RAZON DEL 3% MENSUAL , SIN QUE POR ESTE QUEDE PRORROGADO SU VENCIMIENTO.", new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL)));

            tablaPie.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL)));

            tablaPie.AddCell(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL)));
            tablaPie.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL)));
            tablaPie.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL)));

            tablaPie.AddCell(new Phrase("\nFIRMA RECIBIDO", new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL)));
            tablaPie.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL)));

            tablaPie.CompleteRow();
            //tablaPie.AddCell(new Phrase("\n\n\n\nTODOS LOS PEDIDOS PUEDEN TENER UNA VARIACION DE   +/-  10% EN LA FABRICACION.", new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL)));
            //tablaPie.CompleteRow();
            tablaPie.CompleteRow();

            tablaPie.CompleteRow();

            _documento.Add(tablaPie);


        }

        #endregion

    }




    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEvents : PdfPageEventHelper
    {

        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;


        #region Fields
        private string _header;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion


        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException)
            { }
            catch (System.IO.IOException)
            { }
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

            //iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

            //iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

            //Phrase p1Header = new Phrase("Sample Header Here", baseFontNormal);

            ////Create PdfTable object
            //PdfPTable pdfTab = new PdfPTable(3);

            ////We will have to create separate cells to include image logo and 2 separate strings
            ////Row 1
            //PdfPCell pdfCell1 = new PdfPCell();
            //PdfPCell pdfCell2 = new PdfPCell(p1Header);
            //PdfPCell pdfCell3 = new PdfPCell();
            String text = "Página " + writer.PageNumber + " de ";


            ////Add paging to header
            //{
            //    cb.BeginText();
            //    cb.SetFontAndSize(bf, 12);
            //    cb.SetTextMatrix(document.PageSize.GetRight(200), document.PageSize.GetTop(45));
            //    cb.ShowText(text);
            //    cb.EndText();
            //    float len = bf.GetWidthPoint(text, 12);
            //    //Adds "12" in Page 1 of 12
            //    cb.AddTemplate(headerTemplate, document.PageSize.GetRight(200) + len, document.PageSize.GetTop(45));
            //}

            //Add paging to footer
            //{
            //    cb.BeginText();
            //    cb.SetFontAndSize(bf, 9);
            //    cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
            //    //cb.MoveText(500,30);
            //    //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
            //    cb.ShowText(text);
            //    cb.EndText();
            //    float len = bf.GetWidthPoint(text, 9);
            //    cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

            //    float[] anchoColumasTablaTotales = { 600f };
            //    PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
            //    tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            //    tabla.SetTotalWidth(anchoColumasTablaTotales);
            //    tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            //    tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //    tabla.LockedWidth = true;
            //    tabla.AddCell(new Phrase("Este documento es una representación impresa de un CFDI 3.3", new Font(Font.FontFamily.HELVETICA, 10)));

            //    tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

            //}



            ////Row 2
            //PdfPCell pdfCell4 = new PdfPCell(new Phrase("Sub Header Description", baseFontNormal));
            ////Row 3


            //PdfPCell pdfCell5 = new PdfPCell(new Phrase("Date:" + PrintTime.ToShortDateString(), baseFontBig));
            //PdfPCell pdfCell6 = new PdfPCell();
            //PdfPCell pdfCell7 = new PdfPCell(new Phrase("TIME:" + string.Format("{0:t}", DateTime.Now), baseFontBig));


            ////set the alignment of all three cells and set border to 0
            //pdfCell1.HorizontalAlignment = Element.ALIGN_CENTER;
            //pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
            //pdfCell3.HorizontalAlignment = Element.ALIGN_CENTER;
            //pdfCell4.HorizontalAlignment = Element.ALIGN_CENTER;
            //pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
            //pdfCell6.HorizontalAlignment = Element.ALIGN_CENTER;
            //pdfCell7.HorizontalAlignment = Element.ALIGN_CENTER;


            //pdfCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
            //pdfCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
            //pdfCell4.VerticalAlignment = Element.ALIGN_TOP;
            //pdfCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
            //pdfCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
            //pdfCell7.VerticalAlignment = Element.ALIGN_MIDDLE;


            //pdfCell4.Colspan = 3;



            //pdfCell1.Border = 0;
            //pdfCell2.Border = 0;
            //pdfCell3.Border = 0;
            //pdfCell4.Border = 0;
            //pdfCell5.Border = 0;
            //pdfCell6.Border = 0;
            //pdfCell7.Border = 0;


            ////add all three cells into PdfTable
            //pdfTab.AddCell(pdfCell1);
            //pdfTab.AddCell(pdfCell2);
            //pdfTab.AddCell(pdfCell3);
            //pdfTab.AddCell(pdfCell4);
            //pdfTab.AddCell(pdfCell5);
            //pdfTab.AddCell(pdfCell6);
            //pdfTab.AddCell(pdfCell7);

            //pdfTab.TotalWidth = document.PageSize.Width - 80f;
            //pdfTab.WidthPercentage = 70;
            ////pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;


            ////call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
            ////first param is start row. -1 indicates there is no end row and all the rows to be included to write
            ////Third and fourth param is x and y position to start writing
            //  pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
            ////set pdfContent value

            ////DIbuja una linea para separar el encabezado
            //cb.MoveTo(20, document.PageSize.Height - 142);
            //cb.LineTo(document.PageSize.Width - 20, document.PageSize.Height - 142);
            //cb.Stroke();

            //Move the pointer and draw line to separate footer section from rest of page
            //cb.MoveTo(40, document.PageSize.GetBottom(50));
            //cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
            //cb.Stroke();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            //headerTemplate.BeginText();
            //headerTemplate.SetFontAndSize(bf, 12);
            //headerTemplate.SetTextMatrix(0, 0);
            //headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            //headerTemplate.EndText();

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 9);
            //footerTemplate.MoveText(550,30);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber).ToString());
            footerTemplate.EndText();
        }
    }
    #endregion



    public static class XmlAttributeExtensions
    {
        public static string GetValue(this XmlElement fuente, string name)
        {
            foreach (XmlAttribute atributo in fuente.Attributes)  // recorre los atributos del elemento en este caso concepto
            {
                if (atributo.Name.ToUpper() == name.ToUpper()) // compara los nombre que se requiere con el atributo en mayusculas
                {
                    string valor = atributo.Value;
                    return valor;
                }

            }
            return "";
        }
    }
}

