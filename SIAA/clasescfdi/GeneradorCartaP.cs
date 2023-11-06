using System;
using System.Collections.Generic;
using System.Text;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Cfdi;
using Generador;
using System.Web;
using System.IO;
using SIAAPI.Facturas;
using System.Xml;
using System.util;
using static SIAAPI.clasescfdi.ClsCartaporte2;
using iTextSharp.text.pdf;
using iTextSharp.text;
using SIAAPI.clasescfdi;
using SIAAPI.Reportes;
using SIAAPI.Models.CartaPorte;
using SIAAPI.Models.Administracion;
using System.Globalization;

namespace GeneradorCartaP
{
    public class EmisorCartaP
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

    public class UUIdrelacionadosCartaP
    {
        public string UUID = string.Empty;


    }

   

    public class DocumentoPDFCartaP
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

        public EmisorCartaP emisor = new EmisorCartaP();
        public EmisorCartaP receptor = new EmisorCartaP();
        
        public List<UUIdrelacionadosCartaP> UUIdrelacionadosCartaP = new List<UUIdrelacionadosCartaP>();

        public string totalEnLetra = string.Empty;
        public float totalImpuestosRetenidos = 0.00f;

        public string Telefono = "";

        public string tipo_cambio { get; internal set; }


    }

    public class CreaPDFCartaP
    {
        ClsColoresReporte colorf;
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public DocumentoPDFCartaP _templatePDF; //Objeto que contendra la información del documento pdf
        XmlDocument xDocCarta; // Objeto para abrir el archivo xml

        public string nombreDocumento = string.Empty;

        ClsCartaporte2 _elemento = null;
        public CMYKColor colordefinido;

        public CreaPDFCartaP(ClsCartaporte2 cartaporte2,int IDCarta, System.Drawing.Image logo, string Telefono = "", bool descarga = false)
        {
            //LeerArtributosXML(rutaXML);
            _elemento = cartaporte2;
            //Trabajos con el documento XML
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + ".pdf");
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
                    nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + DateTime.Now.Day + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".pdf"); ;
                }

            }
            catch (Exception ERR)
            {
                string mensajederror = ERR.Message;
            }
            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            _writer.PageEvent = new ITextEventsCartaP();

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            AgregarDatosProductos(IDCarta);
          


            _documento.Close();
            _writer.Close();
            _writer.Dispose();
            

        }

       

        



        #region Leer datos del .xml

       
        //private void ObtenerNodoCfdiComprobante()
        //{
        //    decimal valFloat;
        //    if (xDocCarta.GetElementsByTagName("cfdi:Comprobante") == null)
        //        return;

        //    XmlNodeList comprobante = xDocCarta.GetElementsByTagName("cfdi:Comprobante");
        //    try
        //    {
        //        if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Serie")/*((XmlElement)comprobante[0]).GetAttribute("Serie")*/ != null)
        //            _templatePDF.serie = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Serie");// ((XmlElement)comprobante[0]).GetAttribute("Serie");
        //    }
        //    catch (Exception err)
        //    {
        //        string mensaje = err.Message;
        //        _templatePDF.serie = "";
        //    }
        //    try
        //    {
        //        if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Folio")/*((XmlElement)comprobante[0]).GetAttribute("Folio")*/ != null)
        //            _templatePDF.folio = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Folio");// ((XmlElement)comprobante[0]).GetAttribute("Folio");
        //    }
        //    catch (Exception err)
        //    {
        //        string mensaje = err.Message;
        //        _templatePDF.folio = "0";
        //    }
        //    if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Fecha")/*((XmlElement)comprobante[0]).GetAttribute("Fecha")*/ != null)
        //        //XmlAttributeExtensions.GetValue(nodo, "NoIdentificacion")
        //        _templatePDF.fechaEmisionCFDI = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Fecha");  // ((XmlElement)comprobante[0]).GetAttribute("Fecha");
        //    if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Sello")/*((XmlElement)comprobante[0]).GetAttribute("Sello")*/ != null)
        //        _templatePDF.selloDigitalCFDI = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Sello"); // ((XmlElement)comprobante[0]).GetAttribute("Sello");
        //    if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "NoCertificado")/*((XmlElement)comprobante[0]).GetAttribute("NoCertificado")*/ != null)
        //        _templatePDF.noSerieCertificadoEmisor = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "NoCertificado"); ((XmlElement)comprobante[0]).GetAttribute("NoCertificado");
        //    if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "SubTotal")/*((XmlElement)comprobante[0]).GetAttribute("SubTotal")*/ != null)
        //    {
        //        decimal.TryParse(XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "SubTotal")/*((XmlElement)comprobante[0]).GetAttribute("SubTotal")*/, out valFloat);
        //        _templatePDF.subtotal = valFloat;
        //    }
        //    if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Moneda")/*((XmlElement)comprobante[0]).GetAttribute("Moneda")*/ != null)
        //        _templatePDF.claveMoneda = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Moneda");// ((XmlElement)comprobante[0]).GetAttribute("Moneda");


        //    if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Total")/*((XmlElement)comprobante[0]).GetAttribute("Total")*/ != null)
        //    {
        //        decimal.TryParse(XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "Total")/*((XmlElement)comprobante[0]).GetAttribute("Total")*/, out valFloat);
        //        _templatePDF.total = valFloat;

        //        Numalet numaLet = new Numalet();
        //        numaLet.MascaraSalidaDecimal = "00/100 M.N.";
        //        if (_templatePDF.claveMoneda == "MXN")
        //        {
        //            numaLet.SeparadorDecimalSalida = "pesos";
        //        }
        //        if (_templatePDF.claveMoneda == "USD")
        //        {
        //            numaLet.SeparadorDecimalSalida = "dolares";
        //            numaLet.MascaraSalidaDecimal = "00/100";
        //        }
        //        if (_templatePDF.claveMoneda == "EUR")
        //        {
        //            numaLet.SeparadorDecimalSalida = "Euros";
        //            numaLet.MascaraSalidaDecimal = "00/100";
        //        }

        //        numaLet.ApocoparUnoParteEntera = true;
        //        numaLet.LetraCapital = true;
        //        _templatePDF.totalEnLetra = numaLet.ToCustomString(_templatePDF.total);
        //    }
            


        //       if (XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "TipoComprobante")/*((XmlElement)comprobante[0]).GetAttribute("TipoDeComprobante")*/ != null)
        //        _templatePDF.TipoDecomprobrante = XmlAttributeExtensions.GetValue(((XmlElement)comprobante[0]), "TipoComprobante");// ((XmlElement)comprobante[0]).GetAttribute("TipoDeComprobante");

           
        //}

        //private void ObtenerNodoEmisor()
        //{
        //    //Trabajamos con Emisor
        //    if (xDocCarta.GetElementsByTagName("cfdi:Emisor") == null)
        //        return;
        //    XmlNodeList emisor = xDocCarta.GetElementsByTagName("cfdi:Emisor");
        //    _templatePDF.emisor.rfc = XmlAttributeExtensions.GetValue(((XmlElement)emisor[0]), "Rfc");// ((XmlElement)emisor[0]).GetAttribute("Rfc");
        //    _templatePDF.emisor.Nombre = XmlAttributeExtensions.GetValue(((XmlElement)emisor[0]), "Nombre");// ((XmlElement)emisor[0]).GetAttribute("Nombre");
        //    _templatePDF.emisor.RegimenFiscal = XmlAttributeExtensions.GetValue(((XmlElement)emisor[0]), "RegimenFiscal");// ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");
        //}

        //private void ObtenerUUidrelacionados()
        //{
        //    //Trabajamos con Emisor
        //    if (xDocCarta.GetElementsByTagName("cfdi:CfdiRelacionados") == null)
        //        return;

        //    XmlNodeList tipoderelacion = xDocCarta.GetElementsByTagName("cfdi:CfdiRelacionados");

        //    if (tipoderelacion.Count == 0)
        //    {
        //        return;
        //    }
        //    _templatePDF.TipoDeRelacion = XmlAttributeExtensions.GetValue(((XmlElement)tipoderelacion[0]), "TipoRelacion");// ((XmlElement)tipoderelacion[0]).GetAttribute("TipoRelacion");


        //    XmlNodeList lista = ((XmlElement)tipoderelacion[0]).GetElementsByTagName("cfdi:CfdiRelacionado");

        //    foreach (XmlElement nodo in lista)
        //    {
        //        UUIdrelacionadosCartaP idrelacionado;
        //        idrelacionado = new UUIdrelacionadosCartaP();
        //        idrelacionado.UUID = XmlAttributeExtensions.GetValue(nodo, "UUID");//nodo.GetAttribute("UUID").ToString();
        //        _templatePDF.UUIdrelacionadosCartaP.Add(idrelacionado);
        //    }
        //    return;
        //}
        //private void ObtenerNodoReceptor()
        //{
        //    //Trabajamos con receptor
        //    XmlNodeList receptor = xDocCarta.GetElementsByTagName("cfdi:Receptor");
        //    _templatePDF.receptor.razonSocial = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "Nombre"); //((XmlElement)receptor[0]).GetAttribute("Nombre");
        //    _templatePDF.receptor.rfc = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "Rfc"); //((XmlElement)receptor[0]).GetAttribute("Rfc");
        //    _templatePDF.receptor.usocfdi = XmlAttributeExtensions.GetValue(((XmlElement)receptor[0]), "UsoCFDI"); //(XmlElement)receptor[0]).GetAttribute("UsoCFDI");
        //}

        //private void ObtenerNodoConceptos()
        //{
        //    ProductoCFDCartaP p;

        //    if (xDocCarta.GetElementsByTagName("cfdi:Conceptos") == null)
        //        return;
        //    XmlNodeList conceptos = xDocCarta.GetElementsByTagName("cfdi:Conceptos");

        //    if (((XmlElement)conceptos[0]).GetElementsByTagName("cfdi:Concepto") == null)
        //        return;
        //    XmlNodeList lista = ((XmlElement)conceptos[0]).GetElementsByTagName("cfdi:Concepto");
        //    int conta = 1;
        //    foreach (XmlElement nodo in lista)
        //    {
        //        p = new ProductoCFDCartaP();

        //        p.Descripcion = XmlAttributeExtensions.GetValue(nodo, "Descripcion");  // nodo.GetAttribute("ClaveProdServ");

        //        p.ID = (XmlAttributeExtensions.GetValue(nodo, "ID")/*nodo.GetAttribute("NoIdentificacion")*/ == "") ? conta.ToString() : XmlAttributeExtensions.GetValue(nodo, "NoIdentificacion");// nodo.GetAttribute("NoIdentificacion");
        //        p.Unidad = XmlAttributeExtensions.GetValue(nodo, "Unidad");  // nodo.GetAttribute("Cantidad");
        //        p.cantidad = float.Parse(XmlAttributeExtensions.GetValue(nodo, "cantidad")); //nodo.GetAttribute("ClaveUnidad");
        //        p.valorUnitario = float.Parse(XmlAttributeExtensions.GetValue(nodo, "valorUnitario"));// nodo.GetAttribute("Unidad");
        //        p.Importe = float.Parse(XmlAttributeExtensions.GetValue(nodo, "Importe"));// nodo.GetAttribute("Descripcion");
        //        p.ClaveProdServ = XmlAttributeExtensions.GetValue(nodo, "ClaveProdServ"); //nodo.GetAttribute("ValorUnitario"));
        //        p.ClaveUnidad = XmlAttributeExtensions.GetValue(nodo, "ClaveUnidad"); //nodo.GetAttribute("Importe"));
              

        //        _templatePDF.productos.Add(p);
        //    }
        //}
        //private void ObtenerNodoMercancias()
        //{
        //    ProductoCFDCartaP p;

        //    if (xDocCarta.GetElementsByTagName("cfdi:Mercancias") == null)
        //        return;
        //    XmlNodeList conceptos = xDocCarta.GetElementsByTagName("cfdi:Mercancias");

        //    if (((XmlElement)conceptos[0]).GetElementsByTagName("cfdi:Concepto") == null)
        //        return;
        //    XmlNodeList lista = ((XmlElement)conceptos[0]).GetElementsByTagName("cfdi:Concepto");
        //    int conta = 1;
        //    foreach (XmlElement nodo in lista)
        //    {
        //        p = new ProductoCFDCartaP();

        //        p.Descripcion = XmlAttributeExtensions.GetValue(nodo, "Descripcion");  // nodo.GetAttribute("ClaveProdServ");

        //        p.ID = (XmlAttributeExtensions.GetValue(nodo, "ID")/*nodo.GetAttribute("NoIdentificacion")*/ == "") ? conta.ToString() : XmlAttributeExtensions.GetValue(nodo, "NoIdentificacion");// nodo.GetAttribute("NoIdentificacion");
        //        p.Unidad = XmlAttributeExtensions.GetValue(nodo, "Unidad");  // nodo.GetAttribute("Cantidad");
        //        p.cantidad = float.Parse(XmlAttributeExtensions.GetValue(nodo, "cantidad")); //nodo.GetAttribute("ClaveUnidad");
        //        p.valorUnitario = float.Parse(XmlAttributeExtensions.GetValue(nodo, "valorUnitario"));// nodo.GetAttribute("Unidad");
        //        p.Importe = float.Parse(XmlAttributeExtensions.GetValue(nodo, "Importe"));// nodo.GetAttribute("Descripcion");
        //        p.ClaveProdServ = XmlAttributeExtensions.GetValue(nodo, "ClaveProdServ"); //nodo.GetAttribute("ValorUnitario"));
        //        p.ClaveUnidad = XmlAttributeExtensions.GetValue(nodo, "ClaveUnidad"); //nodo.GetAttribute("Importe"));


        //        _templatePDF.productos.Add(p);
        //    }
        //}
        //private void ObtenerNodoComplementoDigital()
        //{
        //    XmlNodeList tfDigital = xDocCarta.GetElementsByTagName("tfd:TimbreFiscalDigital");
        //    if (tfDigital.Count <= 0)
        //        return;

        //    if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "UUID")/*((XmlElement)tfDigital[0]).GetAttribute("UUID")*/ != null)
        //        _templatePDF.folioFiscalUUID = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "UUID");// ((XmlElement)tfDigital[0]).GetAttribute("UUID");
        //    if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "NoCertificadoSAT")/*((XmlElement)tfDigital[0]).GetAttribute("NoCertificadoSAT")*/ != null)
        //        _templatePDF.noSerieCertificadoSAT = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "NoCertificadoSAT");// ((XmlElement)tfDigital[0]).GetAttribute("NoCertificadoSAT");
        //    if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "FechaTimbrado")/*((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado")*/ != null)
        //        _templatePDF.fechaCertificacion = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "FechaTimbrado");// ((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado");
        //    if (XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "SelloSAT")/*((XmlElement)tfDigital[0]).GetAttribute("SelloSAT")*/ != null)
        //        _templatePDF.selloDigitalSAT = XmlAttributeExtensions.GetValue(((XmlElement)tfDigital[0]), "SelloSAT");// ((XmlElement)tfDigital[0]).GetAttribute("SelloSAT");
        //}

       #endregion

        #region Escribir datos en el .pdf

        //private void AgregarLogo(System.Drawing.Image logoEmpresa)
        //{

        //}

        //private void AgregarCuadro()
        //{
        //    _cb = _writer.DirectContentUnder;
        //    //_cb.SaveState();
        //    //_cb.BeginText();
        //    //_cb.MoveText(1, 1);
        //    //_cb.SetFontAndSize(_fuenteTitulos, 8);
        //    //_cb.ShowText("Faustino Rojas Arelano");
        //    //_cb.EndText();
        //    //_cb.RestoreState();

        //    colorf = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
        //    CMYKColor colordefinidoX = colorf.color;

        //    //Agrego cuadro al documento
        //    _cb.SetColorStroke(new BaseColor(colordefinidoX.R, colordefinidoX.G, colordefinidoX.B, colordefinidoX.A)); //Color de la linea
        //    _cb.SetColorFill(new BaseColor(colordefinidoX.R, colordefinidoX.G, colordefinidoX.B, colordefinidoX.A)); // Color del relleno
        //    _cb.SetLineWidth(3.5f); //Tamano de la linea
        //    _cb.Rectangle(410, 730, 10, 70);
        //    _cb.FillStroke();
        //}

        //private void AgregarDatosEmisor(String Telefono, System.Drawing.Image logoEmpresa)
        //{
        //    //Agrega logo en la primer columna
        //    Empresa empresa = new EmpresaContext().empresas.Find(2);

        //    //Datos del receptor
        //    float[] anchoColumnaencabezadogral = { 100f, 180F, 140f, 160 };
        //    PdfPTable tablaencagral = new PdfPTable(anchoColumnaencabezadogral);
        //    tablaencagral.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tablaencagral.SetTotalWidth(anchoColumnaencabezadogral);
        //    tablaencagral.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaencagral.LockedWidth = true;

        //    // logo en la primer columna

        //    if (logoEmpresa == null)
        //        return;
        //    //Agrego la imagen al documento
        //    //Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
        //    //imagen.ScaleToFit(140,100);
        //    //imagen.Alignment = Element.ALIGN_TOP;
        //    //Chunk logo = new Chunk(imagen, 1, -40);
        //    //_documento.Add(logo);

        //    try
        //    {
        //        Image jpg = iTextSharp.text.Image.GetInstance(logoEmpresa, System.Drawing.Imaging.ImageFormat.Jpeg); // Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/logo.jpg"));

        //        Paragraph paragraph = new Paragraph();
        //        paragraph.Alignment = Element.ALIGN_RIGHT;
        //        jpg.ScaleToFit(75f, 50F); //ancho y largo de la imagen
        //        jpg.Alignment = Image.ALIGN_RIGHT;
        //        jpg.SetAbsolutePosition(30f, 670f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
        //        tablaencagral.AddCell(jpg);
        //        //  doc.Add(paragraph);
        //    }
        //    catch (Exception err)
        //    {
        //        string MENSAJEDEERROR = err.Message;
        //        tablaencagral.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
        //    }


        //    Paragraph p1 = new Paragraph();

        //    string direccionC = "CALLE " + empresa.Calle + " NO. " + empresa.NoExt + ", COL. " + empresa.Colonia + "";

        //    string muni = empresa.Municipio + ", ESTADO DE MEXICO, CP. " + empresa.CP;

        //    p1.Add(new Phrase(_templatePDF.emisor.Nombre, new Font(Font.FontFamily.HELVETICA, 10)));
        //    p1.Add("\n");
        //    p1.Add(new Phrase("" + _templatePDF.emisor.rfc, new Font(Font.FontFamily.HELVETICA, 10)));
        //    p1.Add("\n");
        //    p1.Add(new Phrase("PLANTA\n", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
        //    p1.Add(new Phrase(direccionC.ToUpperInvariant(), new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL)));
        //    p1.Add("\n");
        //    p1.Add(new Phrase(muni.ToUpperInvariant(), new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL)));
        //    p1.Add("\n");
        //    p1.Add(new Phrase("TELEFONO " + empresa.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
        //    p1.Add("\n");
        //    String regimen_fiscal = DecodificadorSAT.getRegimen(_templatePDF.emisor.RegimenFiscal);


        //    p1.Add(new Phrase(regimen_fiscal, new Font(Font.FontFamily.HELVETICA, 6)));


        //    p1.Add(new Phrase("\n\nLUGAR DE EXPEDICION " + empresa.Municipio.ToUpperInvariant(), new Font(Font.FontFamily.HELVETICA, 10)));


        //    try
        //    {
        //        if (_templatePDF.TipoDeRelacion != string.Empty)
        //        {
        //            p1.Add(new Phrase("\nTipo de relacion ".ToUpper() + " :" + this.ReturnTiporelacion(_templatePDF.TipoDeRelacion), new Font(Font.FontFamily.HELVETICA, 5)));
        //            p1.Add(new Phrase("\nUUID Relacionados\n".ToUpper(), new Font(Font.FontFamily.HELVETICA, 5)));

        //            foreach (var elemen in _templatePDF.UUIdrelacionadosCartaP)
        //            {
        //                p1.Add(new Phrase(elemen.UUID, new Font(Font.FontFamily.HELVETICA, 6)));


        //            }

        //        }

        //    }
        //    catch
        //    {

        //    }

        //    tablaencagral.AddCell(p1);



        //    Paragraph pQ = new Paragraph();

        //    pQ.Leading = 29;
        //    int IDOficina = 0;
           
                

            
   


        //    tablaencagral.AddCell(pQ);

        //    Paragraph p2 = new Paragraph();



        //    p2.Alignment = Element.ALIGN_CENTER;
        //    if (_templatePDF.TipoDeRelacion == String.Empty)
        //    {
        //        p2.Add(new Phrase("FACTURA : " + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
        //    }
        //    if (_templatePDF.TipoDeRelacion != String.Empty)
        //    {
        //        if (_templatePDF.TipoDeRelacion == "03" || _templatePDF.TipoDeRelacion == "01")
        //        {
        //            p2.Add(new Phrase("NOTA DE CRÉDITO: " + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
        //        }
        //        else
        //        {
        //            p2.Add(new Phrase("FACTURA NÚM: " + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
        //        }

        //    }
        //    p2.Add("\n");
        //    p2.Add(new Phrase("FOLIO FISCAL (UUID): ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    p2.Add("\n");
        //    p2.Add(new Phrase(_templatePDF.folioFiscalUUID, new Font(Font.FontFamily.HELVETICA, 8)));
        //    p2.Add("\n");
        //    p2.Add(new Phrase("NO. DE SERIE DEL CERTIFICADO DEL SAT:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    p2.Add("\n");
        //    p2.Add(new Phrase(_templatePDF.noSerieCertificadoSAT, new Font(Font.FontFamily.HELVETICA, 8)));
        //    p2.Add("\n");
        //    p2.Add(new Phrase("NO. DE SERIE DEL CERTIFICADO DEL EMISOR:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    p2.Add("\n");
        //    p2.Add(new Phrase(_templatePDF.noSerieCertificadoEmisor, new Font(Font.FontFamily.HELVETICA, 8)));
        //    p2.Add("\n");
        //    p2.Add(new Phrase("FECHA Y HORA DE CERTIFICACIÓN:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    p2.Add("\n");
        //    p2.Add(new Phrase(_templatePDF.fechaCertificacion, new Font(Font.FontFamily.HELVETICA, 8)));
        //    p2.Add("\n");
        //    p2.Add(new Phrase("FECHA Y HORA DE EMISIÓN DE CFDI:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    p2.Add("\n");
        //    p2.Add(new Phrase(_templatePDF.fechaEmisionCFDI, new Font(Font.FontFamily.HELVETICA, 8)));
        //    p2.Add("\n");
        //    p2.Add("\n");

        //    PdfPCell celda0 = new PdfPCell(p2);
        //    celda0.HorizontalAlignment = Element.ALIGN_CENTER;
        //    celda0.Border = 0;

        //    tablaencagral.AddCell(celda0);


        //    _documento.Add(tablaencagral);


        //}

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("FACTURA CFDI");
            _documento.AddSubject("DOCUMENTO CREADO APARTIR DE UN XML");
            _documento.AddTitle("FACTURA");
            _documento.SetMargins(5, 5, 5, 5);
        }
        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            //Agrego la imagen al documento
            Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
            imagen.ScaleToFit(100, 80);
            imagen.Alignment = Element.ALIGN_TOP;
            Chunk logo = new Chunk(imagen, 1, -80);
            _documento.Add(logo);
        }
        private void AgregarDatosProductos(int id)
        {

            CMYKColor colorcelda = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;
            CMYKColor colorTITULO = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

            ParqueVehicular vehicular = new ParqueVehicularDBContext().ParqueVe.Find(_elemento.IDTransporte);
            c_ConfigAutotransporte _ConfigAutotransporte = new c_ConfigAutotransporteDBContext().ConfigAutotransporte.Find(vehicular.IDVehiculo);
            VOperadores operador = new ChoferesContext().VOperadores.Find(_elemento.IDOperador);




            float[] tamanoColumnasPrincipal = { 780f };
            PdfPTable tabla = new PdfPTable(tamanoColumnasPrincipal);
            tabla.SetTotalWidth(tamanoColumnasPrincipal);
            tabla.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
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
            tablaTitulo.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaTitulo.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaTitulo.LockedWidth = true;
            PdfPCell cell0 = new PdfPCell(new Phrase("COMPLEMENTO CARTA PORTE", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            tablaTitulo.AddCell(cell0);

            PdfPTable tablados = new PdfPTable(tamanoColumnasPrincipal);
            tablados.SetTotalWidth(tamanoColumnasPrincipal);
            tablados.HorizontalAlignment = Element.ALIGN_LEFT;
            tablados.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablados.LockedWidth = true;

            PdfPCell cell1 = new PdfPCell(new Phrase("Carta Porte", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablados.AddCell(cell1);

            PdfPTable tablaCartaPorte = new PdfPTable(tamanoColumnas7s);
            tablaCartaPorte.SetTotalWidth(tamanoColumnas7s);
            tablaCartaPorte.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaCartaPorte.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaCartaPorte.LockedWidth = true;

            PdfPCell cellV = new PdfPCell(new Phrase("Versión", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellT = new PdfPCell(new Phrase("Translado Internacional", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellD = new PdfPCell(new Phrase("Total Distancia Recorrida", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellP = new PdfPCell(new Phrase("Peso Total Mercancía", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellN = new PdfPCell(new Phrase("No. Total Mercancías", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellC = new PdfPCell(new Phrase("Clave Transporte", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cell = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellV1 = new PdfPCell(new Phrase("1.0", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellT1 = new PdfPCell(new Phrase("No", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellD1 = new PdfPCell(new Phrase(_elemento.DistanciaRecorrida.ToString() + "Km", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellP1 = new PdfPCell(new Phrase(_elemento.PesoTotal.ToString() + "Kg", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellN1 = new PdfPCell(new Phrase(_elemento.NumTotalMercancias.ToString(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cellC1 = new PdfPCell(new Phrase("01- Autotransporte Federal", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell cell11 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));




            tablaCartaPorte.AddCell(cellV);
            tablaCartaPorte.AddCell(cellT);
            tablaCartaPorte.AddCell(cellD);
            tablaCartaPorte.AddCell(cellP);
            tablaCartaPorte.AddCell(cellN);
            tablaCartaPorte.AddCell(cellC);
            tablaCartaPorte.AddCell(cell);
            tablaCartaPorte.AddCell(cellV1);
            tablaCartaPorte.AddCell(cellT1);
            tablaCartaPorte.AddCell(cellD1);
            tablaCartaPorte.AddCell(cellP1);
            tablaCartaPorte.AddCell(cellN1);
            tablaCartaPorte.AddCell(cellC1);
            tablaCartaPorte.AddCell(cell11);

            PdfPTable tablaDiv = new PdfPTable(tamanoColumnasDos);
            tablaDiv.SetTotalWidth(tamanoColumnasDos);
            tablaDiv.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDiv.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDiv.LockedWidth = true;

            PdfPCell cellda = new PdfPCell(new Phrase("Remitente", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celldados = new PdfPCell(new Phrase("Destinatario", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            tablaDiv.AddCell(cellda);
            tablaDiv.AddCell(celldados);

            PdfPTable tablaReDes = new PdfPTable(tamanoColumnas7);
            tablaReDes.SetTotalWidth(tamanoColumnas7);
            tablaReDes.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaReDes.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaReDes.LockedWidth = true;

            PdfPCell celdarr = new PdfPCell(new Phrase("RFC Remitente", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdanr = new PdfPCell(new Phrase("Nombre Remitente", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdafhs = new PdfPCell(new Phrase("Fecha y Hora Salida", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdard = new PdfPCell(new Phrase("RFC Destinatario", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdand = new PdfPCell(new Phrase("Nombre Destinatario", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdafhll = new PdfPCell(new Phrase("Fecha y Hora Llegada", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celdarr1 = new PdfPCell(new Phrase(_elemento.Receptora.RFC, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdanr1 = new PdfPCell(new Phrase(_elemento.Receptora.RazonSocial, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdafhs1 = new PdfPCell(new Phrase(_elemento.FechaSalida.ToString("s"), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaespacio1 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdard1 = new PdfPCell(new Phrase(_elemento.Emisora.RFC, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdand1 = new PdfPCell(new Phrase(_elemento.Emisora.RazonSocial, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdafhll1 = new PdfPCell(new Phrase(_elemento.FechaLlegada.ToString("s"), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


            tablaReDes.AddCell(celdarr);
            tablaReDes.AddCell(celdanr);
            tablaReDes.AddCell(celdafhs);
            tablaReDes.AddCell(celda);
            tablaReDes.AddCell(celdard);
            tablaReDes.AddCell(celdand);
            tablaReDes.AddCell(celdafhll);
            tablaReDes.AddCell(celdarr1);
            tablaReDes.AddCell(celdanr1);
            tablaReDes.AddCell(celdafhs1);
            tablaReDes.AddCell(celdaespacio1);
            tablaReDes.AddCell(celdard1);
            tablaReDes.AddCell(celdand1);
            tablaReDes.AddCell(celdafhll1);

            PdfPTable tablaUbicacionO = new PdfPTable(tamanoColumnasUno);
            tablaUbicacionO.SetTotalWidth(tamanoColumnasUno);
            tablaUbicacionO.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaUbicacionO.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaUbicacionO.LockedWidth = true;

            PdfPCell celda1 = new PdfPCell(new Phrase("Ubicación Origen", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            tablaUbicacionO.AddCell(celda1);

            PdfPTable tablaUO = new PdfPTable(tamanoColumnas11);
            tablaUO.SetTotalWidth(tamanoColumnas11);
            tablaUO.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaUO.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaUO.LockedWidth = true;


            PdfPCell celdauo111 = new PdfPCell(new Phrase("Calle", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo21 = new PdfPCell(new Phrase("Número Exterior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo31 = new PdfPCell(new Phrase("Numero Interior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo41 = new PdfPCell(new Phrase("Colonia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo51 = new PdfPCell(new Phrase("Localidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo61 = new PdfPCell(new Phrase("Referencia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo71 = new PdfPCell(new Phrase("Municipio", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo81 = new PdfPCell(new Phrase("Estado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo91 = new PdfPCell(new Phrase("Pais", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo101 = new PdfPCell(new Phrase("Código Postal", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo1111 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celdauo1 = new PdfPCell(new Phrase(_elemento.Origen.Calle, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo2 = new PdfPCell(new Phrase(_elemento.Origen.NumeroExterior, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo3 = new PdfPCell(new Phrase(_elemento.Origen.NumeroInterior, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo4 = new PdfPCell(new Phrase(_elemento.Origen.Colonia, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo5 = new PdfPCell(new Phrase(_elemento.Origen.Localidad, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo6 = new PdfPCell(new Phrase(_elemento.Origen.Referencia, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo7 = new PdfPCell(new Phrase(_elemento.Origen.Municipio, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo8 = new PdfPCell(new Phrase(_elemento.Origen.Estado, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo9 = new PdfPCell(new Phrase(_elemento.Origen.Pais, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo10 = new PdfPCell(new Phrase(_elemento.Origen.CodigoPostal, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdauo11 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            tablaUO.AddCell(celdauo111);
            tablaUO.AddCell(celdauo21);
            tablaUO.AddCell(celdauo31);
            tablaUO.AddCell(celdauo41);
            tablaUO.AddCell(celdauo51);
            tablaUO.AddCell(celdauo61);
            tablaUO.AddCell(celdauo71);
            tablaUO.AddCell(celdauo81);
            tablaUO.AddCell(celdauo91);
            tablaUO.AddCell(celdauo101);
            tablaUO.AddCell(celdauo1111); 

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

            PdfPTable tablaUbicacionD = new PdfPTable(tamanoColumnasUno);
            tablaUbicacionD.SetTotalWidth(tamanoColumnasUno);
            tablaUbicacionD.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaUbicacionD.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaUbicacionD.LockedWidth = true;

            PdfPCell celda2 = new PdfPCell(new Phrase("Ubicación Destino", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            tablaUbicacionD.AddCell(celda2);

            PdfPTable tablaUD = new PdfPTable(tamanoColumnas11);
            tablaUD.SetTotalWidth(tamanoColumnas11);
            tablaUD.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaUD.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaUD.LockedWidth = true;

            PdfPCell celdaud1 = new PdfPCell(new Phrase("Calle", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud2 = new PdfPCell(new Phrase("Número Exterior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud3 = new PdfPCell(new Phrase("Numero Interior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud4 = new PdfPCell(new Phrase("Colonia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud5 = new PdfPCell(new Phrase("Localidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud6 = new PdfPCell(new Phrase("Referencia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud7 = new PdfPCell(new Phrase("Municipio", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud8 = new PdfPCell(new Phrase("Estado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud9 = new PdfPCell(new Phrase("Pais", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud10 = new PdfPCell(new Phrase("Código Postal", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaud11 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell Destinoceldaud1 = new PdfPCell(new Phrase(_elemento.Destino.Calle, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud2 = new PdfPCell(new Phrase(_elemento.Destino.NumeroExterior, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud3 = new PdfPCell(new Phrase(_elemento.Destino.NumeroInterior, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud4 = new PdfPCell(new Phrase(_elemento.Destino.Colonia, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud5 = new PdfPCell(new Phrase(_elemento.Destino.Localidad, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud6 = new PdfPCell(new Phrase(_elemento.Destino.Referencia, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud7 = new PdfPCell(new Phrase(_elemento.Destino.Municipio, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud8 = new PdfPCell(new Phrase(_elemento.Destino.Estado, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud9 = new PdfPCell(new Phrase(_elemento.Destino.Pais, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud10 = new PdfPCell(new Phrase(_elemento.Destino.CodigoPostal, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Destinoceldaud11 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


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
            tablaUD.AddCell(Destinoceldaud1);
            tablaUD.AddCell(Destinoceldaud2);
            tablaUD.AddCell(Destinoceldaud3);
            tablaUD.AddCell(Destinoceldaud4);
            tablaUD.AddCell(Destinoceldaud5);
            tablaUD.AddCell(Destinoceldaud6);
            tablaUD.AddCell(Destinoceldaud7);
            tablaUD.AddCell(Destinoceldaud8);
            tablaUD.AddCell(Destinoceldaud9);
            tablaUD.AddCell(Destinoceldaud10);
            tablaUD.AddCell(Destinoceldaud11);

            PdfPTable tablaM = new PdfPTable(tamanoColumnasUno);
            tablaM.SetTotalWidth(tamanoColumnasUno);
            tablaM.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaM.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaM.LockedWidth = true;

            PdfPCell celda3 = new PdfPCell(new Phrase("Mercancías", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            tablaM.AddCell(celda3);



            PdfPTable tablaMercancia = new PdfPTable(tamanoColumnas9);
            tablaMercancia.SetTotalWidth(tamanoColumnas9);
            tablaMercancia.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaMercancia.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaMercancia.LockedWidth = true;

            PdfPCell celdam1 = new PdfPCell(new Phrase("Bienes Transportados", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdam2 = new PdfPCell(new Phrase("Descripción", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdam3 = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdam4 = new PdfPCell(new Phrase("Clave Unidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdam5 = new PdfPCell(new Phrase("Material Peligroso", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdam6 = new PdfPCell(new Phrase("Peso en Kg", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdam7 = new PdfPCell(new Phrase("Valor Mercancia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdam8 = new PdfPCell(new Phrase("Moneda", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdam9 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaUO.AddCell(celdam1);
            tablaUO.AddCell(celdam2);
            tablaUO.AddCell(celdam3);
            tablaUO.AddCell(celdam4);
            tablaUO.AddCell(celdam5);
            tablaUO.AddCell(celdam6);
            tablaUO.AddCell(celdam7);
            tablaUO.AddCell(celdam8);
            tablaUO.AddCell(celdam9);

            foreach (MercanciaCP mercancia in _elemento.Listaconceptos.conceptos)
            {
                PdfPCell Mercanciaceldam1 = new PdfPCell(new Phrase(mercancia.CodigoFamiliaPS, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell Mercanciaceldam2 = new PdfPCell(new Phrase(mercancia.Descripcion, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell Mercanciaceldam3 = new PdfPCell(new Phrase(mercancia.cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell Mercanciaceldam4 = new PdfPCell(new Phrase(mercancia.Claveunidad, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell Mercanciaceldam5 = new PdfPCell(new Phrase(mercancia.materialpeligroso, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell Mercanciaceldam6 = new PdfPCell(new Phrase(mercancia.PesoKg.ToString(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell Mercanciaceldam7 = new PdfPCell(new Phrase(mercancia.valor.ToString(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell Mercanciaceldam8 = new PdfPCell(new Phrase(mercancia.clavemoneda, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell Mercanciaceldam9 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaUO.AddCell(Mercanciaceldam1);
                tablaUO.AddCell(Mercanciaceldam2);
                tablaUO.AddCell(Mercanciaceldam3);
                tablaUO.AddCell(Mercanciaceldam4);
                tablaUO.AddCell(Mercanciaceldam5);
                tablaUO.AddCell(Mercanciaceldam6);
                tablaUO.AddCell(Mercanciaceldam7);
                tablaUO.AddCell(Mercanciaceldam8);
                tablaUO.AddCell(Mercanciaceldam9);
            }

          

            PdfPTable tablaAIR = new PdfPTable(tamanoColumnastres);
            tablaM.SetTotalWidth(tamanoColumnastres);
            tablaM.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaM.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaM.LockedWidth = true;

            PdfPCell celda4 = new PdfPCell(new Phrase("Autotransporte Federal", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda5 = new PdfPCell(new Phrase("Identificación Vehicular", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda6 = new PdfPCell(new Phrase("Remolque", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
          
            tablaAIR.AddCell(celda4);
            tablaAIR.AddCell(celda5);
            tablaAIR.AddCell(celda6);



            PdfPTable tablaAUVRe = new PdfPTable(tamanoColumnas9s);
            tablaAUVRe.SetTotalWidth(tamanoColumnas9s);
            tablaAUVRe.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaAUVRe.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaAUVRe.LockedWidth = true;

            PdfPCell celdaauvre1 = new PdfPCell(new Phrase("Permiso SCT", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaauvre2 = new PdfPCell(new Phrase("No. Permiso SCT", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaauvre3 = new PdfPCell(new Phrase("Nombre Aseguradora", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaauvre4 = new PdfPCell(new Phrase("Poliza Seguro", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaauvre5 = new PdfPCell(new Phrase("Configuración Vehicular", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaauvre6 = new PdfPCell(new Phrase("Placa Vehicular", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaauvre7 = new PdfPCell(new Phrase("Año Modelo", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaauvre8 = new PdfPCell(new Phrase("SubTipo Remolque", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaauvre9 = new PdfPCell(new Phrase("Placa", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


            PdfPCell Autoceldaauvre1 = new PdfPCell(new Phrase(vehicular.ClavePermisoSCT, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Autoceldaauvre2 = new PdfPCell(new Phrase(vehicular.NoPermisoSCT, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Autoceldaauvre3 = new PdfPCell(new Phrase(vehicular.Aseguradora, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Autoceldaauvre4 = new PdfPCell(new Phrase(vehicular.PolizaSeguro, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Autoceldaauvre5 = new PdfPCell(new Phrase(_ConfigAutotransporte.ClaveNom, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Autoceldaauvre6 = new PdfPCell(new Phrase(vehicular.PlacaVehiculo, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Autoceldaauvre7 = new PdfPCell(new Phrase(vehicular.AnnoVehiculo.ToString(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Autoceldaauvre8 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Autoceldaauvre9 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


            tablaAUVRe.AddCell(celdaauvre1);
            tablaAUVRe.AddCell(celdaauvre2);
            tablaAUVRe.AddCell(celdaauvre3);
            tablaAUVRe.AddCell(celdaauvre4);
            tablaAUVRe.AddCell(celdaauvre5);
            tablaAUVRe.AddCell(celdaauvre6);
            tablaAUVRe.AddCell(celdaauvre7);
            tablaAUVRe.AddCell(celdaauvre8);
            tablaAUVRe.AddCell(celdaauvre9);

            tablaAUVRe.AddCell(Autoceldaauvre1);
            tablaAUVRe.AddCell(Autoceldaauvre2);
            tablaAUVRe.AddCell(Autoceldaauvre3);
            tablaAUVRe.AddCell(Autoceldaauvre4);
            tablaAUVRe.AddCell(Autoceldaauvre5);
            tablaAUVRe.AddCell(Autoceldaauvre6);
            tablaAUVRe.AddCell(Autoceldaauvre7);
            tablaAUVRe.AddCell(Autoceldaauvre8);
            tablaAUVRe.AddCell(Autoceldaauvre9);

            PdfPTable tablaop = new PdfPTable(tamanoColumnasUno);
            tablaop.SetTotalWidth(tamanoColumnasUno);
            tablaop.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaop.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaop.LockedWidth = true;

            PdfPCell cellda4 = new PdfPCell(new Phrase("Operador", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaop.AddCell(cellda4);

            PdfPTable tablaOperador = new PdfPTable(tamanoColumnascuatro);
            tablaOperador.SetTotalWidth(tamanoColumnascuatro);
            tablaOperador.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaOperador.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaOperador.LockedWidth = true;

            PdfPCell celdaop1 = new PdfPCell(new Phrase("RFC Operador", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaop2 = new PdfPCell(new Phrase("Número Licencia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaop3 = new PdfPCell(new Phrase("Nombre Operador", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdaop4 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell Opeceldaop1 = new PdfPCell(new Phrase(operador.RFC, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Opeceldaop2 = new PdfPCell(new Phrase(operador.NoLicencia, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Opeceldaop3 = new PdfPCell(new Phrase(operador.Nombre, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell Opeceldaop4 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            tablaOperador.AddCell(celdaop1);
            tablaOperador.AddCell(celdaop2);
            tablaOperador.AddCell(celdaop3);
            tablaOperador.AddCell(celdaop4);

            tablaOperador.AddCell(Opeceldaop1);
            tablaOperador.AddCell(Opeceldaop2);
            tablaOperador.AddCell(Opeceldaop3);
            tablaOperador.AddCell(Opeceldaop4);

            PdfPTable tablaDomicilioOp = new PdfPTable(tamanoColumnasUno);
            tablaDomicilioOp.SetTotalWidth(tamanoColumnasUno);
            tablaDomicilioOp.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDomicilioOp.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDomicilioOp.LockedWidth = true;

            PdfPCell celda7 = new PdfPCell(new Phrase("Domicilio Operador", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            tablaDomicilioOp.AddCell(celda7);

            PdfPTable tablaDomOp = new PdfPTable(tamanoColumnas11);
            tablaDomOp.SetTotalWidth(tamanoColumnas11);
            tablaDomOp.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDomOp.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDomOp.LockedWidth = true;

            PdfPCell celdado1 = new PdfPCell(new Phrase("Calle", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado2 = new PdfPCell(new Phrase("Número Exterior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado3 = new PdfPCell(new Phrase("Numero Interior", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado4 = new PdfPCell(new Phrase("Colonia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado5 = new PdfPCell(new Phrase("Localidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado6 = new PdfPCell(new Phrase("Referencia", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado7 = new PdfPCell(new Phrase("Municipio", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado8 = new PdfPCell(new Phrase("Estado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado9 = new PdfPCell(new Phrase("Pais", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado10 = new PdfPCell(new Phrase("Código Postal", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdado11 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell DomOpeceldado1 = new PdfPCell(new Phrase(operador.Calle, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado2 = new PdfPCell(new Phrase(operador.NumExt, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado3 = new PdfPCell(new Phrase(operador.NumInt, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado4 = new PdfPCell(new Phrase(operador.c_Colonia, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado5 = new PdfPCell(new Phrase(operador.c_Localidad, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado6 = new PdfPCell(new Phrase(operador.Referencia, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado7 = new PdfPCell(new Phrase(operador.c_Municipio, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado8 = new PdfPCell(new Phrase(operador.c_Estado, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado9 = new PdfPCell(new Phrase(operador.c_Pais, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado10 = new PdfPCell(new Phrase(operador.CP, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell DomOpeceldado11 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


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

            tablaDomOp.AddCell(DomOpeceldado1);
            tablaDomOp.AddCell(DomOpeceldado2);
            tablaDomOp.AddCell(DomOpeceldado3);
            tablaDomOp.AddCell(DomOpeceldado4);
            tablaDomOp.AddCell(DomOpeceldado5);
            tablaDomOp.AddCell(DomOpeceldado6);
            tablaDomOp.AddCell(DomOpeceldado7);
            tablaDomOp.AddCell(DomOpeceldado8);
            tablaDomOp.AddCell(DomOpeceldado9);
            tablaDomOp.AddCell(DomOpeceldado10);
            tablaDomOp.AddCell(DomOpeceldado11);

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
        
        //private void AgregarDatosEmisor(String Telefono)
        //{
        //    //Datos del emisor
        //    Paragraph p1 = new Paragraph();
        //    p1.IndentationLeft = 150f;
        //    p1.SpacingAfter = 20;
        //    p1.Leading = 9;

        //    p1.Add(new Phrase(empresa.RazonSocial, new Font(Font.FontFamily.HELVETICA, 10)));
        //    p1.Add("\n");
        //    p1.Add(new Phrase(empresa.RFC, new Font(Font.FontFamily.HELVETICA, 10)));
        //    p1.Add("\n");
        //    p1.Add(new Phrase(empresa.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
        //    p1.Add("\n");
        //    p1.Add(new Phrase(empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + "\n" + empresa.Municipio + "\n" + empresa.Estados.Estado, new Font(Font.FontFamily.HELVETICA, 8)));

        //    p1.Add("\n");
        //    p1.SpacingAfter = -70;
        //    p1.Add("\n");
        //    p1.Add("\n");

        //    //if (_templatePDF.emisor.telefono != string.Empty)
        //    //{
        //    //    p1.Add(new Phrase("Tel." + _templatePDF.emisor.telefono, new Font(Font.FontFamily.HELVETICA, 8)));
        //    //    p1.Add("\n");
        //    //}
        //    _documento.Add(p1);
        //}








        #endregion

    }




    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsCartaP : PdfPageEventHelper
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


            String text = "Página " + writer.PageNumber + " ";
            //p2.IndentationLeft = 600f;
            //p2.SpacingAfter = 20;

            cb.BeginText();
            cb.SetFontAndSize(bf, 9);
            cb.SetTextMatrix(document.PageSize.GetRight(90), document.PageSize.GetBottom(30));
            //cb.MoveText(500,30);
            //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
            //cb.ShowText(text);
            cb.EndText();
            float len = bf.GetWidthPoint(text, 9);
            cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));
            float[] anchoColumasTablaTotales = { 770f, 700f };
            PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumasTablaTotales);

            tabla.HorizontalAlignment = Element.ALIGN_RIGHT;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            tabla.LockedWidth = true;

            tabla.AddCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

            tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

        }


        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 9);
            //footerTemplate.MoveText(550,30);
            footerTemplate.SetTextMatrix(0, 0);
            //footerTemplate.ShowText((writer.PageNumber - 1).ToString());
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
