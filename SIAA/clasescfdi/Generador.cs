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

namespace Generador
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
        public string DomicilioFiscalReceptor { get; internal set; }
        public string RegimenFiscalReceptor { get; internal set; }


    }

    public class UUIdrelacionados
    {
        public string UUID = string.Empty;
       
        
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

    public class ImpuestoCFD
    {
        public string impuesto;
        public float tasa;
        public float importe;
    }

    public class RetencionCFD
    {
        public string impuesto;
        public float importe;
    }

    public class DocumentoPDF
    {
        public string serie = string.Empty;
        public string folio = string.Empty;
        public string Version = string.Empty;
        public string folioFiscalUUID = string.Empty;
        public string noSerieCertificadoSAT = string.Empty;
        public string noSerieCertificadoEmisor = string.Empty;
        public string fechaCertificacion = string.Empty;
        public string fechaEmisionCFDI = string.Empty;
        public String TipoDecomprobrante = string.Empty;
        public String TipoDeRelacion = string.Empty;

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
        public List<ProductoCFD> productos = new List<ProductoCFD>();
        public List<ImpuestoCFD> impuestos = new List<ImpuestoCFD>();
        public List<RetencionCFD> retenciones = new List<RetencionCFD>();
        public List<UUIdrelacionados> UUIDrelacionados = new List<UUIdrelacionados>();

        public string totalEnLetra = string.Empty;
        public float totalImpuestosRetenidos = 0.00f;

        public string Telefono = "";

        public string tipo_cambio { get; internal set; }

        
    }

    public class CreaPDF
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

        public EncPrefactura prefactura = null;

        public DetPrefactura detprefactura = null;

        public CMYKColor colordefinido;
        public CreaPDF(string rutaXML, System.Drawing.Image logo, string Telefono = "", bool descarga = false)
        {
            LeerArtributosXML(rutaXML);

            //Trabajos con el documento XML
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O").Replace("Ã±", "ñ") + ".pdf");
            _documento = new Document(PageSize.LETTER);
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
                    nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + DateTime.Now.Day + DateTime.Now.Day + DateTime.Now.Year+ DateTime.Now.Hour+ DateTime.Now.Minute + DateTime.Now.Second+ ".pdf"); ;
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
            AgregarDatosEmisor(Telefono);
            AgregarDatosFactura();
            AgregarDatosReceptorEmisorSimple();
            AgregarDatosProductos();
            AgregarTotales();
            AgregarSellos();

            //Cerramoe el documento

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

        public CreaPDF(string rutaXML, System.Drawing.Image logo, EncPrefactura _prefactura, string Telefono = "", bool descarga = false)
        {
            prefactura = _prefactura;
            colordefinido = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            LeerArtributosXML(rutaXML);

            //Trabajos con el documento XML
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + ".pdf");
            _documento = new Document(PageSize.LETTER);
            _documento.SetMargins(25, 10f, 20f,60f);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            //Creamos el documento
            try
            {
                int contadoarchivo = 1;
                while (File.Exists(nombreDocumento))
                {
                    
                        nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + "("+ contadoarchivo+ ").pdf");
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
            AgregarDatosEmisor(Telefono);
            AgregarDatosFactura();
            AgregarDatosReceptorEmisor();


            AgregarDatosProductosPrefactura();
            AgregarTotales();
            AgregarSellos();

            //Cerramoe el documento

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


        public CreaPDF(string rutaXML)
        {

            xDoc = new XmlDocument(); //Instancia documento pdf
            _templatePDF = new DocumentoPDF(); //Instancia que contendrá la información para llenar el pdf
            xDoc.LoadXml(rutaXML);
            ObtenerNodoCfdiComprobante();
            ObtenerUUidrelacionados();
            ObtenerNodoEmisor();
            ObtenerNodoReceptor();
            ObtenerNodoConceptos();
            ObtenerNodoComplementoDigital();
            ObtenerNodoImpuestos();

        }




        #region Leer datos del .xml

        private void LeerArtributosXML(string rutaXML)
        {
            xDoc = new XmlDocument(); //Instancia documento pdf
            _templatePDF = new DocumentoPDF(); //Instancia que contendrá la información para llenar el pdf
            xDoc.LoadXml(rutaXML);
            ObtenerNodoCfdiComprobante();
            ObtenerUUidrelacionados();
            ObtenerNodoEmisor();
            ObtenerNodoReceptor();
            ObtenerNodoConceptos();
            ObtenerNodoComplementoDigital();
            ObtenerNodoImpuestos();

        }

        private void ObtenerNodoCfdiComprobante()
        {
               decimal valFloat;
            if (xDoc.GetElementsByTagName( "cfdi:Comprobante") == null)
                return;

            XmlNodeList comprobante = xDoc.GetElementsByTagName("cfdi:Comprobante");
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
            try
            {
                if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Version")/*((XmlElement)comprobante[0]).GetAttribute("Serie")*/ != null)
                    _templatePDF.Version = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Version");// ((XmlElement)comprobante[0]).GetAttribute("Serie");
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                _templatePDF.Version = "";
            }
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

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Fecha")/*((XmlElement)comprobante[0]).GetAttribute("Fecha")*/ != null)
                //XmlAttributeExtensions.GetValue(nodo, "NoIdentificacion")
                _templatePDF.fechaEmisionCFDI = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Fecha");  // ((XmlElement)comprobante[0]).GetAttribute("Fecha");
            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Sello")/*((XmlElement)comprobante[0]).GetAttribute("Sello")*/ != null)
                _templatePDF.selloDigitalCFDI = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Sello"); // ((XmlElement)comprobante[0]).GetAttribute("Sello");
            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "NoCertificado")/*((XmlElement)comprobante[0]).GetAttribute("NoCertificado")*/ != null)
                _templatePDF.noSerieCertificadoEmisor = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "NoCertificado"); ((XmlElement)comprobante[0]).GetAttribute("NoCertificado");
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

                numaLet.ApocoparUnoParteEntera = true;
                numaLet.LetraCapital = true;
                _templatePDF.totalEnLetra = numaLet.ToCustomString(_templatePDF.total);
            }
            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Descuento")/*((XmlElement)comprobante[0]).GetAttribute("Descuento")*/ != null)
            {
                decimal.TryParse(XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Descuento")/*((XmlElement)comprobante[0]).GetAttribute("Descuento")*/, out valFloat);
                _templatePDF.descuento = valFloat;
            }


            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "MetodoPago")/*((XmlElement)comprobante[0]).GetAttribute("MetodoPago")*/ != null)
                _templatePDF.metodoPago = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "MetodoPago");// ((XmlElement)comprobante[0]).GetAttribute("MetodoPago");
            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "FormaPago")/*((XmlElement)comprobante[0]).GetAttribute("FormaPago")*/ != null)
                _templatePDF.formaPago = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "FormaPago");// ((XmlElement)comprobante[0]).GetAttribute("FormaPago");
            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "TipoDeComprobante")/*((XmlElement)comprobante[0]).GetAttribute("TipoDeComprobante")*/ != null)
                _templatePDF.TipoDecomprobrante = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "TipoDeComprobante");// ((XmlElement)comprobante[0]).GetAttribute("TipoDeComprobante");

            if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "TipoCambio")/*((XmlElement)comprobante[0]).GetAttribute("TipoCambio")*/ != null)
            {
                _templatePDF.tipo_cambio = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "TipoCambio");// ((XmlElement)comprobante[0]).GetAttribute("TipoCambio");
                if (_templatePDF.tipo_cambio == string.Empty && _templatePDF.claveMoneda == "MXN")
                {
                    _templatePDF.tipo_cambio = "1";
                }
                //else
                //{
                //    DateTime FF = DateTime.Parse(  _templatePDF.fechaEmisionCFDI);
                //    string fecfac = FF.Year.ToString() + "-" + FF.Month.ToString() + "-" + FF.Day.ToString();

                //    decimal tcd = 0;

                //    try
                //    {
                //        tcd = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>("select [dbo].[GetTipocambioCadena]('" + fecfac + "','USD', 'MXN') as Dato").ToList().FirstOrDefault().Dato;
                //    }
                //    catch (Exception err)
                //    {

                //    }
                //    _templatePDF.tipo_cambio = tcd.ToString();
                //}
            }
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

        private void ObtenerUUidrelacionados()
        {
            //Trabajamos con Emisor
            if (xDoc.GetElementsByTagName("cfdi:CfdiRelacionados") == null)
                return;

            XmlNodeList tipoderelacion = xDoc.GetElementsByTagName("cfdi:CfdiRelacionados");

            if (tipoderelacion.Count == 0)
            {
                return;
            }
            _templatePDF.TipoDeRelacion = XmlAttributeExtensions.GetValue(((XmlElement)tipoderelacion[0]), "TipoRelacion");// ((XmlElement)tipoderelacion[0]).GetAttribute("TipoRelacion");


            XmlNodeList lista = ((XmlElement)tipoderelacion[0]).GetElementsByTagName("cfdi:CfdiRelacionado");

            foreach (XmlElement nodo in lista)
            {
                UUIdrelacionados idrelacionado;
                idrelacionado = new UUIdrelacionados();
                idrelacionado.UUID = XmlAttributeExtensions.GetValue(nodo, "UUID");//nodo.GetAttribute("UUID").ToString();
                _templatePDF.UUIDrelacionados.Add(idrelacionado);
            }
            return;
        }
        private void ObtenerNodoReceptor()
        {
            //Trabajamos con receptor
            XmlNodeList receptor = xDoc.GetElementsByTagName("cfdi:Receptor");
            _templatePDF.receptor.razonSocial = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "Nombre"); //((XmlElement)receptor[0]).GetAttribute("Nombre");
            _templatePDF.receptor.rfc = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "Rfc"); //((XmlElement)receptor[0]).GetAttribute("Rfc");
            _templatePDF.receptor.usocfdi = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "UsoCFDI"); //(XmlElement)receptor[0]).GetAttribute("UsoCFDI");
          
            if (_templatePDF.Version == "4.0") { 


                _templatePDF.receptor.RegimenFiscalReceptor = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "RegimenFiscalReceptor");
                _templatePDF.receptor.DomicilioFiscalReceptor = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "DomicilioFiscalReceptor");

            }
        
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

                p.ClaveProducto = XmlAttributeExtensions.GetValue(nodo, "ClaveProdServ");  // nodo.GetAttribute("ClaveProdServ");

                p.id = (XmlAttributeExtensions.GetValue(nodo, "NoIdentificacion")/*nodo.GetAttribute("NoIdentificacion")*/ == "") ? conta.ToString() : XmlAttributeExtensions.GetValue(nodo, "NoIdentificacion");// nodo.GetAttribute("NoIdentificacion");
                p.cantidad = XmlAttributeExtensions.GetValue(nodo, "Cantidad");  // nodo.GetAttribute("Cantidad");
                p.c_unidad = XmlAttributeExtensions.GetValue(nodo, "ClaveUnidad"); //nodo.GetAttribute("ClaveUnidad");
                p.unidad = XmlAttributeExtensions.GetValue(nodo, "Unidad");// nodo.GetAttribute("Unidad");
                p.desc = XmlAttributeExtensions.GetValue(nodo, "Descripcion");// nodo.GetAttribute("Descripcion");
                p.v_unitario = float.Parse(XmlAttributeExtensions.GetValue(nodo, "ValorUnitario")); //nodo.GetAttribute("ValorUnitario"));
                p.importe = float.Parse(XmlAttributeExtensions.GetValue(nodo, "Importe")); //nodo.GetAttribute("Importe"));
                p.descuento = 0;

                _templatePDF.productos.Add(p);
            }
        }

        private void ObtenerNodoComplementoDigital()
        {
            XmlNodeList tfDigital = xDoc.GetElementsByTagName("tfd:TimbreFiscalDigital");
            if (tfDigital.Count <= 0)
                return;

            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "UUID")/*((XmlElement)tfDigital[0]).GetAttribute("UUID")*/ != null)
                _templatePDF.folioFiscalUUID = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "UUID");// ((XmlElement)tfDigital[0]).GetAttribute("UUID");
            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "NoCertificadoSAT")/*((XmlElement)tfDigital[0]).GetAttribute("NoCertificadoSAT")*/ != null)
                _templatePDF.noSerieCertificadoSAT = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "NoCertificadoSAT");// ((XmlElement)tfDigital[0]).GetAttribute("NoCertificadoSAT");
            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "FechaTimbrado")/*((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado")*/ != null)
                _templatePDF.fechaCertificacion = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "FechaTimbrado");// ((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado");
            if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "SelloSAT")/*((XmlElement)tfDigital[0]).GetAttribute("SelloSAT")*/ != null)
                _templatePDF.selloDigitalSAT = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "SelloSAT");// ((XmlElement)tfDigital[0]).GetAttribute("SelloSAT");
        }

        private void ObtenerNodoImpuestos()
        {
            float valFloat;
            XmlNodeList impuestos = xDoc.GetElementsByTagName("cfdi:Impuestos");
            try
            {
                float.TryParse(XmlAttributeExtensions.GetValue(((XmlElement)impuestos[impuestos.Count - 1]), "TotalImpuestosTrasladados")/*((XmlElement)impuestos[impuestos.Count - 1]).GetAttribute("TotalImpuestosTrasladados")*/, out valFloat);

                _templatePDF.totalImpuestosRetenidos = valFloat;

                if (xDoc.GetElementsByTagName("cfdi:Traslados") == null)
                    return;
                XmlNodeList traslados = xDoc.GetElementsByTagName("cfdi:Traslados");

                if (((XmlElement)traslados[0]).GetElementsByTagName("cfdi:Traslado") == null)
                    return;
                XmlNodeList lista = ((XmlElement)traslados[0]).GetElementsByTagName("cfdi:Traslados");


                try
                {
                   
                    Hashtable Tiva = new Hashtable();
             
                    Hashtable Tisr = new Hashtable();
              
                    Hashtable Tieps = new Hashtable();
                    
                 
           // foreach (XmlElement nodo in traslados)
           for (int i=0; i<= traslados.Count-2; i++)
                    {
                        float impor = 0;

                        XmlElement nodo  = (XmlElement) traslados[i].FirstChild;
                         float.TryParse(XmlAttributeExtensions.GetValue(nodo, "Importe")/*nodo.GetAttribute("Importe")*/, out impor);
                        string impuesto = XmlAttributeExtensions.GetValue(nodo, "Impuesto");//nodo.GetAttribute("Impuesto"); // impuesto

                        if ( impuesto=="002")
                        {
                            try
                            {
                                string tasa = XmlAttributeExtensions.GetValue(nodo, "TasaOCuota");//nodo.GetAttribute("TasaOCuota"); // tasa

                                ImpuestoCFD a = (ImpuestoCFD)Tiva[tasa];
                                if (a == null) // si no hay lo agrego
                                {
                                    a = new ImpuestoCFD();
                                    a.impuesto = "IVA";
                                    a.tasa = float.Parse(tasa) * 100;
                                    a.importe = impor;
                                    Tiva.Add(tasa, a);
                                }
                                else
                                {
                                    a.importe = a.importe + impor; // si ya hay lo quito y lo añado modificando la tasa
                                    Tiva.Remove(tasa);
                                    Tiva.Add(tasa, a);
                                }

                            }
                            catch (Exception erre)
                            {
                                string error = erre.Message;
                                // no existe Tasa o cuota por que es Exento
                            }
                            // primero busco si ya hay una tasa igual

                        } // fin de iva
                        if (impuesto == "001")
                        {
                            try
                            {
                                string tasa = XmlAttributeExtensions.GetValue(nodo, "TasaOCuota");//nodo.GetAttribute("TasaOCuota"); // tasa

                                ImpuestoCFD a = (ImpuestoCFD)Tisr[tasa];
                                if (a == null) // si no hay lo agrego
                                {
                                    a = new ImpuestoCFD();
                                    a.impuesto = "ISR";
                                    a.tasa = float.Parse(tasa) * 100;
                                    a.importe = impor;
                                    Tisr.Add(tasa, a);
                                }
                                else
                                {
                                    a.importe = a.importe + impor; // si ya hay lo quito y lo añado modificando la tasa
                                    Tisr.Remove(tasa);
                                    Tisr.Add(tasa, a);
                                }

                            }
                            catch (Exception erre)
                            {
                                string error = erre.Message;
                                // no existe Tasa o cuota por que es Exento
                            }
                            // primero busco si ya hay una tasa igual

                        } // fin de isr

                        if (impuesto == "003")
                        {
                            try
                            {
                                string tasa = XmlAttributeExtensions.GetValue(nodo, "TasaOCuota");//nodo.GetAttribute("TasaOCuota"); // tasa

                                ImpuestoCFD a = (ImpuestoCFD)Tieps[tasa];
                                if (a == null) // si no hay lo agrego
                                {
                                    a = new ImpuestoCFD();
                                    a.impuesto = "ISR";
                                    a.tasa = float.Parse(tasa) * 100;
                                    a.importe = impor;
                                    Tieps.Add(tasa, a);
                                }
                                else
                                {
                                    a.importe = a.importe + impor; // si ya hay lo quito y lo añado modificando la tasa
                                    Tieps.Remove(tasa);
                                    Tieps.Add(tasa, a);
                                }

                            }
                            catch (Exception erre)
                            {
                                string error = erre.Message;
                                // no existe Tasa o cuota por que es Exento
                            }
                            // primero busco si ya hay una tasa igual

                        } // fin de ieps
                    }

                    foreach (object llave in Tiva.Keys)
                    {
                        ImpuestoCFD impa = (ImpuestoCFD) Tiva[llave.ToString()];
                        _templatePDF.impuestos.Add(impa);
                    }
                    foreach (object llave in Tisr.Keys)
                    {
                        ImpuestoCFD impa = (ImpuestoCFD)Tisr[llave.ToString()];
                        _templatePDF.impuestos.Add(impa);
                    }
                    foreach (object llave in Tieps.Keys)
                    {
                        ImpuestoCFD impa = (ImpuestoCFD)Tieps[llave.ToString()];
                        _templatePDF.impuestos.Add(impa);
                    }




                }
                catch (Exception err)
            {

            }}
            catch
            { }

            XmlNodeList retenciones = xDoc.GetElementsByTagName("cfdi:Retenciones");
            if (retenciones.Count > 0)
            {

                if (((XmlElement)retenciones[0]).GetElementsByTagName("cfdi:Retencion") == null)
                    return;
                XmlNodeList listaRetenciones = ((XmlElement)retenciones[0]).GetElementsByTagName("cfdi:Retencion");

                foreach (XmlElement nodo in listaRetenciones)
                {
                    RetencionCFD r = new RetencionCFD();
                    r.impuesto = XmlAttributeExtensions.GetValue(nodo, "Impuesto"); //nodo.GetAttribute("Impuesto");
                    float.TryParse(XmlAttributeExtensions.GetValue(nodo, "Importe")/*nodo.GetAttribute("Importe")*/, out valFloat);
                    r.importe = valFloat;
                    _templatePDF.retenciones.Add(r);
                }
            }
        }

        #endregion

        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
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
                jpg.ScaleToFit(120f, 80F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(30f, 700f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                _documento.Add(jpg);
              //  doc.Add(paragraph);
            }
            catch (Exception err)
            {
				 string MENSAJEDEERROR = err.Message;
                //tablaencagral.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            }
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

            //Agrego cuadro al documento
            _cb.SetColorStroke(colordefinido); //Color de la linea
            _cb.SetColorFill(BaseColor.WHITE); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(410, 640, 195, 120);
            _cb.FillStroke();
        }

        private void AgregarDatosEmisor(String Telefono)
        {
            //Datos del emisor
            
            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 180f;
        
            p1.Leading = 29;
            p1.Add(new Phrase("DATOS DEL EMISOR", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            p1.Add("\n");
            p1.Leading = 9;
            p1.Add(new Phrase(_templatePDF.emisor.Nombre, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.emisor.rfc, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            String regimen_fiscal = DecodificadorSAT.getRegimen(_templatePDF.emisor.RegimenFiscal);

           
            p1.Add(new Phrase(regimen_fiscal, new Font(Font.FontFamily.HELVETICA, 6)));


            p1.Add(new Phrase("\n\nTipo de Comprobante ".ToUpper() + " :" + _templatePDF.TipoDecomprobrante, new Font(Font.FontFamily.HELVETICA, 10)));
           
            try
            {
                if (_templatePDF.TipoDeRelacion!= string.Empty)
                {
                    p1.Add(new Phrase("\nTipo de relacion ".ToUpper() + " :" + this.ReturnTiporelacion( _templatePDF.TipoDeRelacion), new Font(Font.FontFamily.HELVETICA, 5)));
                    p1.Add(new Phrase("\nUUID Relacionados\n".ToUpper(), new Font(Font.FontFamily.HELVETICA, 5)));
                    int regreso = 80;
                    foreach ( var elemen in _templatePDF.UUIDrelacionados)
                    {
                        p1.Add(new Phrase(elemen.UUID, new Font(Font.FontFamily.HELVETICA, 6)));

                        regreso += 10;
                    }
                    p1.SpacingAfter = regreso*-1;
                }
                else
                {
                    p1.SpacingAfter = -80;
                    p1.Add("\n");
                    p1.Add("\n");
                    p1.Add("\n");
                }
            }
            catch
            {

            }



            //if (_templatePDF.emisor.telefono != string.Empty)
            //{
            //    p1.Add(new Phrase("Tel." + _templatePDF.emisor.telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            //    p1.Add("\n");
            //}
            p1.SpacingBefore = 30;
            _documento.Add(p1);
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
            Paragraph p2 = new Paragraph();
            
            p2.IndentationLeft = 403f;
            p2.SpacingAfter = 18;
            p2.Leading = 10;
            p2.Alignment = Element.ALIGN_CENTER;
            if (_templatePDF.TipoDeRelacion==String.Empty)
                {
                p2.Add(new Phrase("FACTURA NÚM: " + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            }
            if (_templatePDF.TipoDeRelacion!= String.Empty)
            {
                if (_templatePDF.TipoDeRelacion =="03" || _templatePDF.TipoDeRelacion == "01")
                {
                    p2.Add(new Phrase("NOTA DE CRÉDITO: " + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
                }
                else
                {
                    p2.Add(new Phrase("FACTURA NÚM: " + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
                }

            }
            p2.Add("\n");
            p2.Add(new Phrase("FOLIO FISCAL (UUID): ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(_templatePDF.folioFiscalUUID, new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");
            p2.Add(new Phrase("NO. DE SERIE DEL CERTIFICADO DEL SAT:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(_templatePDF.noSerieCertificadoSAT, new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");
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
            _documento.Add(p2);
        }

        private void AgregarDatosReceptorEmisor()
        {
            float[] anchoColumasTablaDatosEmisorReceptor = { 280f, 320f };
            PdfPTable tablaDatosEmisorReceptor = new PdfPTable(anchoColumasTablaDatosEmisorReceptor);
            tablaDatosEmisorReceptor.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosEmisorReceptor.SetTotalWidth(anchoColumasTablaDatosEmisorReceptor);
            tablaDatosEmisorReceptor.SpacingBefore = 5;
            tablaDatosEmisorReceptor.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosEmisorReceptor.LockedWidth = true;

            try
            {
              
                    Clientes cliente = new ClientesContext().Clientes.Find(prefactura.IDCliente);

                    ClsColoresReporte colorf = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
                    CMYKColor colordefinidoX = colorf.color;

                    ClsColoresReporte colorfF = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado);
                    CMYKColor colorfuente = colorfF.color;


                    //Datos del receptor
                    float[] anchoColumnaspro = { 230f, 220F, 140f };
                    PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
                    tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablapro.SetTotalWidth(anchoColumnaspro);
                    tablapro.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tablapro.LockedWidth = true;



                    PdfPCell celda0 = new PdfPCell(new Phrase("CLIENTE: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
                    celda0.BackgroundColor = colordefinidoX;
                PdfPCell celda3 = new PdfPCell(new Phrase("ENTREGAR EN: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
                //PdfPCell celda3 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
                celda3.BackgroundColor = colordefinidoX;
                    PdfPCell celdaobserva = new PdfPCell(new Phrase("OBSERVACION :".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
                    celdaobserva.BackgroundColor = colordefinidoX;

                    PdfPCell celda1 = new PdfPCell(new Phrase(cliente.Nombre + "\n"  + cliente.RFC, new Font(Font.FontFamily.HELVETICA, 8)));


                //  PdfPCell celda4 = new PdfPCell(new Phrase(prefactura.Entrega + " ", new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celda4 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdaobser = new PdfPCell(new Phrase(prefactura.Observacion, new Font(Font.FontFamily.HELVETICA, 8)));

                
                PdfPCell RAZON = new PdfPCell(new Phrase("RAZÓN SOCIAL ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
                RAZON.BackgroundColor = colordefinidoX;
                PdfPCell REGIMEN = new PdfPCell(new Phrase("REGIMÉN FISCAL ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
                //PdfPCell celda3 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
                REGIMEN.BackgroundColor = colordefinidoX;
                PdfPCell sola = new PdfPCell(new Phrase("DOMICILIO FISCAL".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
                sola.BackgroundColor = colordefinidoX;

                PdfPCell RazonS = new PdfPCell(new Phrase(_templatePDF.receptor.razonSocial, new Font(Font.FontFamily.HELVETICA, 8)));

                string regimen=DecodificadorSAT.getRegimen(_templatePDF.receptor.RegimenFiscalReceptor);
                //  PdfPCell celda4 = new PdfPCell(new Phrase(prefactura.Entrega + " ", new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell RegimenF = new PdfPCell(new Phrase(regimen, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell Solaa = new PdfPCell(new Phrase(_templatePDF.receptor.DomicilioFiscalReceptor, new Font(Font.FontFamily.HELVETICA, 8)));





                tablapro.AddCell(celda0);
                    tablapro.AddCell(celda3);
                    tablapro.AddCell(celdaobserva);

                    tablapro.AddCell(celda1);
                    tablapro.AddCell(celda4);
                    tablapro.AddCell(celdaobser);


               

                if (_templatePDF.Version=="4.0") {
                    tablapro.AddCell(RAZON);
                    tablapro.AddCell(REGIMEN);
                    tablapro.AddCell(sola);
                    tablapro.AddCell(RazonS);
                    tablapro.AddCell(RegimenF);
                    tablapro.AddCell(Solaa);
                }
            
                //tablapro.AddCell(celda5);
                //tablapro.AddCell(celda6);

                tablapro.CompleteRow();

                    _documento.Add(tablapro);

               
            }
            catch(Exception err)
            {
                string mensajerror = err.Message;
            }





            float[] anchoColumnascosas = { 50f, 90f, 90f, 90f, 90f, 40f, 80f, 60f };
            PdfPTable tablaencabezado = new PdfPTable(anchoColumnascosas);
            tablaencabezado.DefaultCell.Border = Rectangle.BOX;
            tablaencabezado.SetTotalWidth(anchoColumnascosas);
            tablaencabezado.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaencabezado.LockedWidth = true;


            PdfPCell celdaMO = new PdfPCell(new Phrase("MONEDA ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaCR = new PdfPCell(new Phrase("CONDICIONES".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaUS = new PdfPCell(new Phrase("USO DE CFDI".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaFO = new PdfPCell(new Phrase("FORMA DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaME = new PdfPCell(new Phrase("METODO DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaTI = new PdfPCell(new Phrase("TC ".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaTC = new PdfPCell(new Phrase("COMPROBANTE".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaAL = new PdfPCell(new Phrase("O COMPRA".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            CMYKColor colordefinido;

            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;

            celdaMO.BackgroundColor = colordefinido;
            celdaUS.BackgroundColor = colordefinido;
            celdaCR.BackgroundColor = colordefinido;
            celdaFO.BackgroundColor = colordefinido;
            celdaME.BackgroundColor = colordefinido;
            celdaTI.BackgroundColor = colordefinido;
			celdaTC.BackgroundColor = colordefinido;
            celdaAL.BackgroundColor = colordefinido;


            tablaencabezado.AddCell(celdaMO);
            tablaencabezado.AddCell(celdaCR);
            tablaencabezado.AddCell(celdaUS);
            tablaencabezado.AddCell(celdaFO);
            tablaencabezado.AddCell(celdaME);
            tablaencabezado.AddCell(celdaTI);
			tablaencabezado.AddCell(celdaTC);
            tablaencabezado.AddCell(celdaAL);

            tablaencabezado.CompleteRow();





            PdfPCell celdaclavemoneda = new PdfPCell(new Phrase(_templatePDF.claveMoneda, new Font(Font.FontFamily.HELVETICA, 8)));
           // celdaclavemoneda.Colspan = 3;
          //  celdaclavemoneda.Border = Rectangle.NO_BORDER;
            tablaencabezado.AddCell(celdaclavemoneda);
            string condiciones;

           
            try {
                CondicionesPago cp = new CondicionesPagoContext().CondicionesPagos.Find(prefactura.IDCondicionesPago);
                condiciones = cp.Descripcion;
            }
            catch(Exception)
            {
               condiciones = "";//prefactura.CondicionesPago.Descripcion;
            }

            
            tablaencabezado.AddCell(new Phrase(condiciones.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));


            // string uso = DecodificadorSAT.getUso(_templatePDF.emisor.usocfdi);
            try
            {
                string usocfdi = DecodificadorSAT.getUso(_templatePDF.receptor.usocfdi);

                tablaencabezado.AddCell(new Phrase(usocfdi.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            }
            catch(Exception err)
            {
                string mensajeerror = err.Message;
                tablaencabezado.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            }

       

            try
            {
                string f_pago = DecodificadorSAT.getFormapago(_templatePDF.formaPago);
                tablaencabezado.AddCell(new Phrase(f_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
                tablaencabezado.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            }

            try
            {
                string m_pago = DecodificadorSAT.getMetodo(_templatePDF.metodoPago);
                tablaencabezado.AddCell(new Phrase(m_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
                tablaencabezado.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            }


            try
            {
                tablaencabezado.AddCell(new Phrase(_templatePDF.tipo_cambio.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
                tablaencabezado.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            }


            tablaencabezado.AddCell(new Phrase(_templatePDF.TipoDecomprobrante.ToString() + " " + DecodificadorSAT.getTipoComprobante(_templatePDF.TipoDecomprobrante.ToString()), new Font(Font.FontFamily.HELVETICA, 8)));



            string OC = string.Empty;
            try
            {
                OC = new ClsRastreaOC(prefactura.IDPrefactura).Rastreadeprefactura();
            }
            catch (Exception err)
            {
                OC = string.Empty;
            }

            tablaencabezado.AddCell(new Phrase(OC.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));

            tablaencabezado.CompleteRow();

            //_documento.Add(tabla);


           // tablaDatosEmisorReceptor.AddCell(tablaencabezado);
          //  _documento.Add(tablaDatosEmisorReceptor);
            _documento.Add(tablaencabezado);


        }

        private string ReturnClavePago(string clave)
        {
            string valor = "";

            valor = clave;

            return valor.ToString().ToUpper();
        }


        private void AgregarDatosReceptorEmisorSimple()
        {
            float[] anchoColumasTablaDatosEmisorReceptor = { 280f, 320f };
            PdfPTable tablaDatosEmisorReceptor = new PdfPTable(anchoColumasTablaDatosEmisorReceptor);
            tablaDatosEmisorReceptor.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosEmisorReceptor.SetTotalWidth(anchoColumasTablaDatosEmisorReceptor);
            tablaDatosEmisorReceptor.SpacingBefore = 5;
            tablaDatosEmisorReceptor.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosEmisorReceptor.LockedWidth = true;

            try
            {

               
                ClsColoresReporte colorf = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
                CMYKColor colordefinidoX = colorf.color;

                ClsColoresReporte colorfF = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado);
                CMYKColor colorfuente = colorfF.color;


                //Datos del receptor
                float[] anchoColumnaspro = { 590f };
                PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
                tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
                tablapro.SetTotalWidth(anchoColumnaspro);
                tablapro.HorizontalAlignment = Element.ALIGN_RIGHT;
                tablapro.LockedWidth = true;



                PdfPCell celda0 = new PdfPCell(new Phrase("CLIENTE: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
                celda0.BackgroundColor = colordefinidoX;
             
                PdfPCell celda1 = new PdfPCell(new Phrase(_templatePDF.receptor.razonSocial, new Font(Font.FontFamily.HELVETICA, 8)));


                PdfPCell celda4 = new PdfPCell(new Phrase(_templatePDF.receptor.rfc + " ", new Font(Font.FontFamily.HELVETICA, 8)));
                

                tablapro.AddCell(celda0);
                
                tablapro.AddCell(celda1);
                tablapro.AddCell(celda4);
                
                //tablapro.AddCell(celda5);
                //tablapro.AddCell(celda6);

                tablapro.CompleteRow();

                _documento.Add(tablapro);


            }
            catch (Exception err)
            {
                string mensajerror = err.Message;
            }





            float[] anchoColumnascosas = { 50f, 90f, 90f, 90f, 90f, 40f, 80f, 60f };
            PdfPTable tablaencabezado = new PdfPTable(anchoColumnascosas);
            tablaencabezado.DefaultCell.Border = Rectangle.BOX;
            tablaencabezado.SetTotalWidth(anchoColumnascosas);
            tablaencabezado.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaencabezado.LockedWidth = true;


            PdfPCell celdaMO = new PdfPCell(new Phrase("MONEDA ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaCR = new PdfPCell(new Phrase("CONDICIONES".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaUS = new PdfPCell(new Phrase("USO DE CFDI".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaFO = new PdfPCell(new Phrase("FORMA DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaME = new PdfPCell(new Phrase("METODO DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaTI = new PdfPCell(new Phrase("TC ".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaTC = new PdfPCell(new Phrase("COMPROBANTE".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaAL = new PdfPCell(new Phrase("O COMPRA".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            CMYKColor colordefinido;

            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;

            celdaMO.BackgroundColor = colordefinido;
            celdaUS.BackgroundColor = colordefinido;
            celdaCR.BackgroundColor = colordefinido;
            celdaFO.BackgroundColor = colordefinido;
            celdaME.BackgroundColor = colordefinido;
            celdaTI.BackgroundColor = colordefinido;
            celdaTC.BackgroundColor = colordefinido;
            celdaAL.BackgroundColor = colordefinido;


            tablaencabezado.AddCell(celdaMO);
            tablaencabezado.AddCell(celdaCR);
            tablaencabezado.AddCell(celdaUS);
            tablaencabezado.AddCell(celdaFO);
            tablaencabezado.AddCell(celdaME);
            tablaencabezado.AddCell(celdaTI);
            tablaencabezado.AddCell(celdaTC);
            tablaencabezado.AddCell(celdaAL);

            tablaencabezado.CompleteRow();





            PdfPCell celdaclavemoneda = new PdfPCell(new Phrase(_templatePDF.claveMoneda, new Font(Font.FontFamily.HELVETICA, 8)));
            // celdaclavemoneda.Colspan = 3;
            //  celdaclavemoneda.Border = Rectangle.NO_BORDER;
            tablaencabezado.AddCell(celdaclavemoneda);
            string condiciones;


            try
            {
                CondicionesPago cp = new CondicionesPagoContext().CondicionesPagos.Find(prefactura.IDCondicionesPago);
                condiciones = cp.Descripcion;
            }
            catch (Exception)
            {
                condiciones = "";//prefactura.CondicionesPago.Descripcion;
            }


            tablaencabezado.AddCell(new Phrase(condiciones.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));


            // string uso = DecodificadorSAT.getUso(_templatePDF.emisor.usocfdi);
            try
            {
                string usocfdi = DecodificadorSAT.getUso(_templatePDF.receptor.usocfdi);

                tablaencabezado.AddCell(new Phrase(usocfdi.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
                tablaencabezado.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            }



            try
            {
                string f_pago = DecodificadorSAT.getFormapago(_templatePDF.formaPago);
                tablaencabezado.AddCell(new Phrase(f_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
                tablaencabezado.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            }

            try
            {
                string m_pago = DecodificadorSAT.getMetodo(_templatePDF.metodoPago);
                tablaencabezado.AddCell(new Phrase(m_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
                tablaencabezado.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            }


            try
            {
                tablaencabezado.AddCell(new Phrase(_templatePDF.tipo_cambio.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
                tablaencabezado.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            }


            tablaencabezado.AddCell(new Phrase(_templatePDF.TipoDecomprobrante.ToString() + " " + DecodificadorSAT.getTipoComprobante(_templatePDF.TipoDecomprobrante.ToString()), new Font(Font.FontFamily.HELVETICA, 8)));



            string OC = string.Empty;
            try
            {
                OC = new ClsRastreaOC(prefactura.IDPrefactura).Rastreadeprefactura();
            }
            catch (Exception err)
            {
                OC = string.Empty;
            }

            tablaencabezado.AddCell(new Phrase(OC.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));

            tablaencabezado.CompleteRow();

            //_documento.Add(tabla);


            // tablaDatosEmisorReceptor.AddCell(tablaencabezado);
            //  _documento.Add(tablaDatosEmisorReceptor);
            _documento.Add(tablaencabezado);


        }

        private String ReturnTiporelacion(String Clave)
        {
          return  DecodificadorSAT.getTipoderelacion(Clave);
        }

        private void AgregarDatosProductos()
        {
            float[] anchoColumnasTablaProductos = { 590f };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 15;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaProductosPrincipal.LockedWidth = true;


            //Datos de los productos
            float[] tamanoColumnas = { 60f, 60f, 50f, 70f, 170f, 60f, 60f, 60f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);
            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;

            tablaProductosTitulos.AddCell(new Phrase("CLAVE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("NO. IDENTIFICACION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("CLAVE UNIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("DESCUENTO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));



            float[] tamanoColumnasProductos = { 60f, 60f, 50f, 70f, 170f, 60f, 60f, 60f };
            PdfPTable tablaProductos = new PdfPTable(tamanoColumnas);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductos.DefaultCell.BorderWidthLeft = 0.1f;
            tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
            tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
            tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
            tablaProductos.SetTotalWidth(tamanoColumnas);
            tablaProductos.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaProductos.LockedWidth = true;
            foreach (ProductoCFD p in _templatePDF.productos)
            {
                tablaProductos.AddCell(new Phrase(p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.id, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.cantidad, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.c_unidad, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.desc.Replace("=",""), new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdauni = new PdfPCell() { Rotation = 0,  HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f            };
                celdauni.Phrase =new Phrase( p.v_unitario.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f          };
                celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);
                PdfPCell celdadescuento = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdadescuento.Phrase = new Phrase(p.descuento.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));

                tablaProductos.AddCell(celdadescuento);
            }


            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
            tablaProductosPrincipal.AddCell(celdaTitulos);
            PdfPCell celdaProductos = new PdfPCell(tablaProductos);
            celdaProductos.MinimumHeight = 300;
            tablaProductosPrincipal.AddCell(celdaProductos);
            _documento.Add(tablaProductosPrincipal);
        }

        private void AgregarDatosProductosPrefactura()
        {
            float[] anchoColumnasTablaProductos = { 590f };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 15;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_RIGHT;;
            tablaProductosPrincipal.LockedWidth = true;


            //Datos de los productos
            float[] tamanoColumnas = { 60f, 60f, 50f, 70f, 170f, 60f, 60f, 60f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);
            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;

            tablaProductosTitulos.AddCell(new Phrase("CLAVE P", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("LOTE", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("CLAVE UNIDAD", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("DESCUENTO", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));


            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
            tablaProductosPrincipal.AddCell(celdaTitulos);


            var detalleprefacturas = new PrefacturaContext().Database.SqlQuery<DetPrefactura>("select * from Detprefactura where IDPrefactura=" + prefactura.IDPrefactura).ToList();

            foreach (DetPrefactura p in detalleprefacturas)
            {


                float[] tamanoColumnasProductos = { 60f, 60f, 50f, 70f, 170f, 60f, 60f, 60f };
                PdfPTable tablaProductos = new PdfPTable(tamanoColumnas);
                //tablaProductos.SpacingBefore = 1;
                tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductos.DefaultCell.BorderWidthLeft = 0.1f;
                tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
                tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
                tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
                tablaProductos.SetTotalWidth(tamanoColumnas);
                tablaProductos.HorizontalAlignment = Element.ALIGN_RIGHT;
                tablaProductos.LockedWidth = true;


                Articulo articulo = new ArticuloContext().Articulo.Find(p.IDArticulo);
				c_ClaveProductoServicio clavesat = new c_ClaveProductoServicioContext().c_ClaveProductoServicios.Find(articulo.Familia.IDProdServ);
                c_ClaveUnidad unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad);
                tablaProductos.AddCell(new Phrase(articulo.Cref, new Font(Font.FontFamily.HELVETICA, 6)));
                if (p.Lote.ToString()!="0")
                {
                    tablaProductos.AddCell(new Phrase(p.Lote, new Font(Font.FontFamily.HELVETICA, 6)));
                }
                else
                {
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));
                }
               
                tablaProductos.AddCell(new Phrase(p.Cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));
                tablaProductos.AddCell(new Phrase(unidad.Nombre, new Font(Font.FontFamily.HELVETICA, 6)));
                tablaProductos.AddCell(new Phrase(articulo.Descripcion.Replace("=",""), new Font(Font.FontFamily.HELVETICA, 6)));
                PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdauni.Phrase = new Phrase(p.Costo.ToString("C"), new Font(Font.FontFamily.HELVETICA, 6));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdaimporte.Phrase = new Phrase(p.Importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 6));
                tablaProductos.AddCell(celdaimporte);
                PdfPCell celdadescuento = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdadescuento.Phrase = new Phrase(p.Descuento.ToString("C"), new Font(Font.FontFamily.HELVETICA, 6));

                tablaProductos.AddCell(celdadescuento);

                string oc = "";
                try
                {
                    ClsRastreaOC rastrea = new ClsRastreaOC(p.IDPrefactura);
                    oc = rastrea.RastreaOC(p.IDPrefactura, p.Caracteristica_ID, p.Cantidad);
                }

                catch (Exception err)
                {

                }
                if (oc.Length>0)
                {
                    oc = "O.C." + oc;
                }
                 tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

                if (prefactura.IDCliente != 50)
                {
                    tablaProductos.AddCell(new Phrase(oc, new Font(Font.FontFamily.HELVETICA, 8)));
                }
                else
                {
                    tablaProductos.AddCell(new Phrase(p.Nota, new Font(Font.FontFamily.HELVETICA, 8)));
                }
                tablaProductos.AddCell(new Phrase("".ToString(), new Font(Font.FontFamily.HELVETICA, 4)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.presentacion, new Font(Font.FontFamily.COURIER, 6)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

                PdfPCell celdaProductos = new PdfPCell(tablaProductos);
               // celdaProductos.MinimumHeight = 300;
                tablaProductosPrincipal.AddCell(celdaProductos);


            }


          
            _documento.Add(tablaProductosPrincipal);
        }

        private void AgregarTotales()
        {

            float[] anchoColumasTablaTotales = { 420f, 180f };
            PdfPTable tablaTotales = new PdfPTable(anchoColumasTablaTotales);
            tablaTotales.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaTotales.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaTotales.SetTotalWidth(anchoColumasTablaTotales);
            tablaTotales.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaTotales.LockedWidth = true;

            float[] anchoColumnas = { 110, 70f };
            PdfPTable tablaImportes = new PdfPTable(anchoColumnas);
            tablaImportes.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaImportes.SetTotalWidth(anchoColumnas);
            tablaImportes.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            //tablaImportes.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaImportes.LockedWidth = true;
            if (_templatePDF.descuento > 0M)
            {
                decimal subtotal = _templatePDF.subtotal + _templatePDF.descuento;
                tablaImportes.AddCell(new Phrase("SUBTOTAL ANTES DE ANTICIPO:", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(_templatePDF.subtotal.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

                tablaImportes.AddCell(new Phrase("ANTICIPO:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(_templatePDF.descuento.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

                tablaImportes.AddCell(new Phrase("SUBTOTAL:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase((_templatePDF.subtotal - _templatePDF.descuento).ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }
            else
            {
                tablaImportes.AddCell(new Phrase("SUBTOTAL:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase((_templatePDF.subtotal ).ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

           

            foreach (ImpuestoCFD i in _templatePDF.impuestos)
            {
                tablaImportes.AddCell(new Phrase(i.impuesto + " " + i.tasa.ToString("F2") + "%:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            foreach (RetencionCFD i in _templatePDF.retenciones)
            {
                tablaImportes.AddCell(new Phrase("Retencion " + i.impuesto + ": ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            tablaImportes.AddCell(new Phrase("Total:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase((_templatePDF.total).ToString("C", new CultureInfo("es-MX")), new Font(Font.FontFamily.HELVETICA, 8)));

            tablaTotales.AddCell(new Phrase("IMPORTE CON LETRA: " + _templatePDF.totalEnLetra.ToUpper() + " " + _templatePDF.claveMoneda, new Font(Font.FontFamily.HELVETICA, 8)));
            tablaTotales.AddCell(tablaImportes);
            _documento.Add(tablaTotales);
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

            float[] anchoColumnas = { 500f, 100f };
            PdfPTable tablaSellosQR = new PdfPTable(anchoColumnas);
            tablaSellosQR.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaSellosQR.SpacingBefore = 10.0f;
            tablaSellosQR.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaSellosQR.SetTotalWidth(anchoColumnas);
            //tablaSellosQR.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaSellosQR.LockedWidth = true;

            float[] anchoColumnas1 = { 500f };
            PdfPTable tablaSellos = new PdfPTable(anchoColumnas1);
            tablaSellos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaSellos.DefaultCell.VerticalAlignment = Element.ALIGN_TOP;
            tablaSellos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaSellos.SetTotalWidth(anchoColumnas1);
            tablaSellos.HorizontalAlignment = Element.ALIGN_CENTER;
            //tablaSellos.LockedWidth = true;
            tablaSellos.AddCell(new Phrase("SELLO DIGITAL DEL CFDI:", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablaSellos.AddCell(new Phrase(_templatePDF.selloDigitalCFDI, new Font(Font.FontFamily.HELVETICA, 4)));
            tablaSellos.AddCell(new Phrase("SELLO DIGITAL DEL SAT", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablaSellos.AddCell(new Phrase(_templatePDF.selloDigitalSAT, new Font(Font.FontFamily.HELVETICA, 4)));
            tablaSellos.AddCell(new Phrase("CADENA ORIGINAL DEL COMPLEMENTO DE CERTIFICACIÓN DIGITAL DEL SAT:", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablaSellos.AddCell(new Phrase(cadenaOriginal.ToString(), new Font(Font.FontFamily.HELVETICA, 4)));

            //Agregamos el codigo QR al documento
            StringBuilder codigoQR = new StringBuilder();
            codigoQR.Append("https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id=" + _templatePDF.folioFiscalUUID);
            codigoQR.Append("&re=" + _templatePDF.emisor.rfc); //RFC del Emisor
            codigoQR.Append("&rr=" + _templatePDF.receptor.rfc); //RFC del receptor
            codigoQR.Append("&tt=" + _templatePDF.total); //Total del comprobante 10 enteros y 6 decimales
            codigoQR.Append("&fe=" +  _templatePDF.selloDigitalCFDI.Substring(_templatePDF.selloDigitalCFDI.Length-8,8)); //UUID del comprobante

            BarcodeQRCode pdfCodigoQR = new BarcodeQRCode(codigoQR.ToString(), 1, 1, null);
            Image img = pdfCodigoQR.GetImage();
            img.SpacingAfter = 0.0f;
            img.SpacingBefore = 0.0f;
            img.BorderWidth = 1.0f;
            //img.ScalePercent(100, 78);
            //img.border

            tablaSellosQR.AddCell(tablaSellos);
            tablaSellosQR.AddCell(img);

            _documento.Add(tablaSellosQR);
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
            footerTemplate.ShowText((writer.PageNumber ).ToString());
            footerTemplate.EndText();
        }
    }
    #endregion

    public sealed class Numalet
    {
        private const int UNI = 0, DIECI = 1, DECENA = 2, CENTENA = 3;
        private static string[,] _matriz = new string[CENTENA + 1, 10]
            {
                {null," uno", " dos", " tres", " cuatro", " cinco", " seis", " siete", " ocho", " nueve"},
                {" diez"," once"," doce"," trece"," catorce"," quince"," dieciseis"," diecisiete"," dieciocho"," diecinueve"},
                {null,null,null," treinta"," cuarenta"," cincuenta"," sesenta"," setenta"," ochenta"," noventa"},
                {null,null,null,null,null," quinientos",null," setecientos",null," novecientos"}
            };

        #region Miembros estáticos

        private const Char sub = (Char)26;
        //Cambiar acá si se quiere otro comportamiento en los métodos de clase
        public const String SeparadorDecimalSalidaDefault = "con";
        public const String MascaraSalidaDecimalDefault = "00'/100.-'";
        public const Int32 DecimalesDefault = 2;
        public const Boolean LetraCapitalDefault = false;
        public const Boolean ConvertirDecimalesDefault = false;
        public const Boolean ApocoparUnoParteEnteraDefault = false;
        public const Boolean ApocoparUnoParteDecimalDefault = false;

        #endregion

        #region Propiedades de instancia

        private Int32 _decimales = DecimalesDefault;
        private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
        private String _separadorDecimalSalida = SeparadorDecimalSalidaDefault;
        private Int32 _posiciones = DecimalesDefault;
        private String _mascaraSalidaDecimal, _mascaraSalidaDecimalInterna = MascaraSalidaDecimalDefault;
        private Boolean _esMascaraNumerica = true;
        private Boolean _letraCapital = LetraCapitalDefault;
        private Boolean _convertirDecimales = ConvertirDecimalesDefault;
        private Boolean _apocoparUnoParteEntera = false;
        private Boolean _apocoparUnoParteDecimal;

        /// <summary>
        /// Indica la cantidad de decimales que se pasarán a entero para la conversión
        /// </summary>
        /// <remarks>Esta propiedad cambia al cambiar MascaraDecimal por un valor que empieze con '0'</remarks>
        public Int32 Decimales
        {
            get { return _decimales; }
            set
            {
                if (value > 10) throw new ArgumentException(value.ToString() + " excede el número máximo de decimales admitidos, solo se admiten hasta 10.");
                _decimales = value;
            }
        }

        /// <summary>
        /// Objeto CultureInfo utilizado para convertir las cadenas de entrada en números
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
            set { _cultureInfo = value; }
        }

        /// <summary>
        /// Indica la cadena a intercalar entre la parte entera y la decimal del número
        /// </summary>
        public String SeparadorDecimalSalida
        {
            get { return _separadorDecimalSalida; }
            set
            {
                _separadorDecimalSalida = value;
                //Si el separador decimal es compuesto, infiero que estoy cuantificando algo,
                //por lo que apocopo el "uno" convirtiéndolo en "un"
                if (value.Trim().IndexOf(" ") > 0)
                    _apocoparUnoParteEntera = true;
                else _apocoparUnoParteEntera = false;
            }
        }

        /// <summary>
        /// Indica el formato que se le dara a la parte decimal del número
        /// </summary>
        public String MascaraSalidaDecimal
        {
            get
            {
                if (!String.IsNullOrEmpty(_mascaraSalidaDecimal))
                    return _mascaraSalidaDecimal;
                else return "";
            }
            set
            {
                //determino la cantidad de cifras a redondear a partir de la cantidad de '0' o '#' 
                //que haya al principio de la cadena, y también si es una máscara numérica
                int i = 0;
                while (i < value.Length
                    && (value[i] == '0')
                        | value[i] == '#')
                    i++;
                _posiciones = i;
                if (i > 0)
                {
                    _decimales = i;
                    _esMascaraNumerica = true;
                }
                else _esMascaraNumerica = false;
                _mascaraSalidaDecimal = value;
                if (_esMascaraNumerica)
                    _mascaraSalidaDecimalInterna = value.Substring(0, _posiciones) + "'"
                        + value.Substring(_posiciones)
                        .Replace("''", sub.ToString())
                        .Replace("'", String.Empty)
                        .Replace(sub.ToString(), "'") + "'";
                else
                    _mascaraSalidaDecimalInterna = value
                        .Replace("''", sub.ToString())
                        .Replace("'", String.Empty)
                        .Replace(sub.ToString(), "'");
            }
        }

        /// <summary>
        /// Indica si la primera letra del resultado debe estár en mayúscula
        /// </summary>
        public Boolean LetraCapital
        {
            get { return _letraCapital; }
            set { _letraCapital = value; }
        }

        /// <summary>
        /// Indica si se deben convertir los decimales a su expresión nominal
        /// </summary>
        public Boolean ConvertirDecimales
        {
            get { return _convertirDecimales; }
            set
            {
                _convertirDecimales = value;
                _apocoparUnoParteDecimal = value;
                if (value)
                {// Si la máscara es la default, la borro
                    if (_mascaraSalidaDecimal == MascaraSalidaDecimalDefault)
                        MascaraSalidaDecimal = "";
                }
                else if (String.IsNullOrEmpty(_mascaraSalidaDecimal))
                    //Si no hay máscara dejo la default
                    MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            }
        }

        /// <summary>
        /// Indica si de debe cambiar "uno" por "un" en las unidades.
        /// </summary>
        public Boolean ApocoparUnoParteEntera
        {
            get { return _apocoparUnoParteEntera; }
            set { _apocoparUnoParteEntera = value; }
        }

        /// <summary>
        /// Determina si se debe apococopar el "uno" en la parte decimal
        /// </summary>
        /// <remarks>El valor de esta propiedad cambia al setear ConvertirDecimales</remarks>
        public Boolean ApocoparUnoParteDecimal
        {
            get { return _apocoparUnoParteDecimal; }
            set { _apocoparUnoParteDecimal = value; }
        }

        #endregion

        #region Constructores

        public Numalet()
        {
            MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            SeparadorDecimalSalida = SeparadorDecimalSalidaDefault;
            LetraCapital = LetraCapitalDefault;
            ConvertirDecimales = _convertirDecimales;
        }

        public Numalet(Boolean ConvertirDecimales, String MascaraSalidaDecimal, String SeparadorDecimalSalida, Boolean LetraCapital)
        {
            if (!String.IsNullOrEmpty(MascaraSalidaDecimal))
                this.MascaraSalidaDecimal = MascaraSalidaDecimal;
            if (!String.IsNullOrEmpty(SeparadorDecimalSalida))
                _separadorDecimalSalida = SeparadorDecimalSalida;
            _letraCapital = LetraCapital;
            _convertirDecimales = ConvertirDecimales;
        }
        #endregion

        #region Conversores de instancia

        public String ToCustomString(Double Numero)
        { return Convertir((Decimal)Numero, _decimales, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, _apocoparUnoParteDecimal); }

        public String ToCustomString(String Numero)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, _cultureInfo, out dNumero))
                return ToCustomString(dNumero);
            else throw new ArgumentException("'" + Numero + "' no es un número válido.");
        }

        public String ToCustomString(Decimal Numero)
        { return ToString(Convert.ToDouble(Numero)); }

        public String ToCustomString(Int32 Numero)
        { return Convertir((Decimal)Numero, 0, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, false); }


        #endregion

        #region Conversores estáticos

        public static String ToString(Int32 Numero)
        {
            return Convertir((Decimal)Numero, 0, null, null, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        public static String ToString(Double Numero)
        { return Convertir((Decimal)Numero, DecimalesDefault, SeparadorDecimalSalidaDefault, MascaraSalidaDecimalDefault, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault); }

        public static String ToString(String Numero, CultureInfo ReferenciaCultural)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, ReferenciaCultural, out dNumero))
                return ToString(dNumero);
            else throw new ArgumentException("'" + Numero + "' no es un número válido.");
        }

        public static String ToString(String Numero)
        {
            return Numalet.ToString(Numero, CultureInfo.CurrentCulture);
        }

        public static String ToString(Decimal Numero)
        { return ToString(Convert.ToDouble(Numero)); }

        #endregion

        private static String Convertir(Decimal Numero, Int32 Decimales, String SeparadorDecimalSalida, String MascaraSalidaDecimal, Boolean EsMascaraNumerica, Boolean LetraCapital, Boolean ConvertirDecimales, Boolean ApocoparUnoParteEntera, Boolean ApocoparUnoParteDecimal)
        {
            Int64 Num;
            Int32 terna, pos, centenaTerna, decenaTerna, unidadTerna, iTerna;
            String numcad, cadTerna;
            StringBuilder Resultado = new StringBuilder();

            Num = (Int64)Math.Abs(Numero);

            if (Num >= 1000000000000 || Num < 0) throw new ArgumentException("El número '" + Numero.ToString() + "' excedió los límites del conversor: [0;1.000.000.000.000)");
            if (Num == 0)
                Resultado.Append(" cero");
            else
            {
                numcad = Num.ToString();
                iTerna = 0;
                pos = numcad.Length;

                do //Se itera por las ternas de atrás para adelante
                {
                    iTerna++;
                    cadTerna = String.Empty;
                    if (pos >= 3)
                        terna = Int32.Parse(numcad.Substring(pos - 3, 3));
                    else
                        terna = Int32.Parse(numcad.Substring(0, pos));

                    centenaTerna = (Int32)(terna / 100);
                    decenaTerna = terna - centenaTerna * 100;
                    unidadTerna = (decenaTerna - (Int32)(decenaTerna / 10) * 10);

                    if ((decenaTerna > 0) && (decenaTerna < 10))
                        cadTerna = _matriz[UNI, unidadTerna] + cadTerna;
                    else if ((decenaTerna >= 10) && (decenaTerna < 20))
                        cadTerna = cadTerna + _matriz[DIECI, decenaTerna - (Int32)(decenaTerna / 10) * 10];
                    else if (decenaTerna == 20)
                        cadTerna = cadTerna + " veinte";
                    else if ((decenaTerna > 20) && (decenaTerna < 30))
                        cadTerna = " veinti" + _matriz[UNI, unidadTerna].Substring(1, _matriz[UNI, unidadTerna].Length - 1);
                    else if ((decenaTerna >= 30) && (decenaTerna < 100))
                        if (unidadTerna != 0)
                            cadTerna = _matriz[DECENA, (Int32)(decenaTerna / 10)] + " y" + _matriz[UNI, unidadTerna] + cadTerna;
                        else
                            cadTerna += _matriz[DECENA, (Int32)(decenaTerna / 10)];

                    switch (centenaTerna)
                    {
                        case 1:
                            if (decenaTerna > 0) cadTerna = " ciento" + cadTerna;
                            else cadTerna = " cien" + cadTerna;
                            break;
                        case 5:
                        case 7:
                        case 9:
                            cadTerna = _matriz[CENTENA, (Int32)(terna / 100)] + cadTerna;
                            break;
                        default:
                            if ((Int32)(terna / 100) > 1) cadTerna = _matriz[UNI, (Int32)(terna / 100)] + "cientos" + cadTerna;
                            break;
                    }
                    //Reemplazo el 'uno' por 'un' si no es en las únidades o si se solicító apocopar
                    if ((iTerna > 1 | ApocoparUnoParteEntera) && decenaTerna == 21)
                        cadTerna = cadTerna.Replace("veintiuno", "veintiún");
                    else if ((iTerna > 1 | ApocoparUnoParteEntera) && unidadTerna == 1 && decenaTerna != 11)
                        cadTerna = cadTerna.Substring(0, cadTerna.Length - 1);
                    //Acentúo 'dieciseís', 'veintidós', 'veintitrés' y 'veintiséis'
                    else if (decenaTerna == 16) cadTerna = cadTerna.Replace("dieciseis", "dieciséis");
                    else if (decenaTerna == 22) cadTerna = cadTerna.Replace("veintidos", "veintidós");
                    else if (decenaTerna == 23) cadTerna = cadTerna.Replace("veintitres", "veintitrés");
                    else if (decenaTerna == 26) cadTerna = cadTerna.Replace("veintiseis", "veintiséis");
                    //Reemplazo 'uno' por 'un' si no es en las únidades o si se solicító apocopar (si _apocoparUnoParteEntera es verdadero) 

                    switch (iTerna)
                    {
                        case 3:
                            if (Num < 2000000) cadTerna += " millón";
                            else cadTerna += " millones";
                            break;
                        case 2:
                        case 4:
                            if (terna > 0) cadTerna += " mil";
                            break;
                    }
                    Resultado.Insert(0, cadTerna);
                    pos = pos - 3;
                } while (pos > 0);
            }
            //Se agregan los decimales si corresponde
            if (Decimales > 0)
            {
                Resultado.Append(" " + SeparadorDecimalSalida + " ");
                Int32 EnteroDecimal = (Int32)Math.Round((Double)(Numero - (Int64)Numero) * Math.Pow(10, Decimales), 0);
                if (ConvertirDecimales)
                {
                    Boolean esMascaraDecimalDefault = MascaraSalidaDecimal == MascaraSalidaDecimalDefault;
                    Resultado.Append(Convertir((Decimal)EnteroDecimal, 0, null, null, EsMascaraNumerica, false, false, (ApocoparUnoParteDecimal && !EsMascaraNumerica/*&& !esMascaraDecimalDefault*/), false) + " "
                        + (EsMascaraNumerica ? "" : MascaraSalidaDecimal));
                }
                else
                    if (EsMascaraNumerica) Resultado.Append(EnteroDecimal.ToString(MascaraSalidaDecimal));
                else Resultado.Append(EnteroDecimal.ToString() + " " + MascaraSalidaDecimal);
            }
            //Se pone la primer letra en mayúscula si corresponde y se retorna el resultado
            if (LetraCapital)
                return Resultado[1].ToString().ToUpper() + Resultado.ToString(2, Resultado.Length - 2);
            else
                return Resultado.ToString().Substring(1);
        }
    }

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
