using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.Reportes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace AcuseCancelacion
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
        public string folioFiscalUUID = string.Empty;
        public int EstatusUUID = 0;
        public string noSerieCertificadoSAT = string.Empty;
        public string noSerieCertificadoEmisor = string.Empty;
        public string fechaCertificacion = string.Empty;
        public DateTime fechaEmisionCFDI;
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

        public AcuseCancelacionF prefactura = null;

        public DetPrefactura detprefactura = null;
        public CMYKColor colordefinido;

        public CreaPDF(string rutaXML, System.Drawing.Image logo)
        {
            LeerArtributosXML(rutaXML);

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
            _writer.PageEvent = new ITextEvents();

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            AgregarLogo(logo);
            //AgregarCuadro();
            AgregarDatosEmisor(logo);
            AgregarDatosFactura();
            AgregarDatosReceptorEmisor();
            


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

        public CreaPDF(string rutaXML, System.Drawing.Image logo, AcuseCancelacionF _prefactura)
        {
            prefactura = _prefactura;
            colordefinido = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;


            LeerArtributosXML(rutaXML);

            //Trabajos con el documento XML
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + "-" + _templatePDF.receptor.razonSocial.Replace("Ó", "O").Replace("?", "O") + ".pdf");
            _documento = new Document(PageSize.LETTER);
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


            //colorf = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            //CMYKColor colordefinidoX = colorf.color;
            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            AgregarLogo(logo);
           // AgregarCuadro();
            AgregarDatosEmisor(logo);
            AgregarDatosFactura();
            AgregarDatosReceptorEmisor();


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
            ObtenerDatosXML(rutaXML);
            

        }




        #region Leer datos del .xml

        private void LeerArtributosXML(string rutaXML)
        {
            xDoc = new XmlDocument(); //Instancia documento pdf
            _templatePDF = new DocumentoPDF(); //Instancia que contendrá la información para llenar el pdf
            //xDoc.LoadXml(rutaXML);
            ObtenerDatosXML(rutaXML);
            

        }

        private void ObtenerDatosXML(string xml)
        {
            
            xDoc.LoadXml(xml);

            XmlSerializer serializer = new XmlSerializer(typeof(Acuse));
            Acuse elemento;
            using (StringReader reader = new StringReader(xml))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (Acuse)serializer.Deserialize(reader);
            }

            _templatePDF.emisor.rfc = elemento.RfcEmisor;
            _templatePDF.fechaEmisionCFDI = elemento.Fecha;
            _templatePDF.folioFiscalUUID = elemento.Folios.UUID;
            _templatePDF.EstatusUUID = elemento.Folios.EstatusUUID;
            _templatePDF.selloDigitalCFDI = elemento.Signature.SignatureValue;


        }

        #endregion

        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            
        }

       

        private void AgregarDatosEmisor(System.Drawing.Image logoEmpresa)
        {
            //Agrega logo en la primer columna
            Empresa empresa = new EmpresaContext().empresas.Find(2);

            //Datos del receptor
            float[] anchoColumnaencabezadogral = { 150f, 450F };
            PdfPTable tablaencagral = new PdfPTable(anchoColumnaencabezadogral);
            tablaencagral.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaencagral.SetTotalWidth(anchoColumnaencabezadogral);
            tablaencagral.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaencagral.LockedWidth = true;

            // logo en la primer columna

            if (logoEmpresa == null)
                return;
            

            try
            {
               
                Image jpg = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/haciendalogo.jpg")); // Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/logo.jpg"));

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_BOTTOM;
                jpg.ScaleToFit(70f, 30F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_BOTTOM;
                jpg.SetAbsolutePosition(30f, 670f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                tablaencagral.AddCell(jpg);
                //  doc.Add(paragraph);
            }
            catch (Exception err)
            {
                string MENSAJEDEERROR = err.Message;
                tablaencagral.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            }

            Paragraph p1 = new Paragraph();

            p1.Add("\n");
            p1.Add("\n");
            p1.Add("\n");
            p1.Alignment = Element.ALIGN_RIGHT; 

            p1.Add(new Phrase("\n\n     ACUSE DE CANCELACIÓN DE CFDI ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));

            tablaencagral.AddCell(p1);

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

        private void AgregarDatosReceptorEmisor()
        {

            try
            {

                //Datos del receptor
                float[] anchoColumnas = { 100f, 200f, 300F };
                PdfPTable tabla = new PdfPTable(anchoColumnas);
                tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla.SetTotalWidth(anchoColumnas);
                tabla.HorizontalAlignment = Element.ALIGN_LEFT;
                tabla.LockedWidth = true;


                string motivoDes = "";
                try
                {
                    RegistroCancelacionFacturas registro = new MotivoCancelacionContext().Database.SqlQuery<RegistroCancelacionFacturas>("select*from RegistroCancelacionFacturas where IDFactura="+prefactura.IDFactura).FirstOrDefault();
                    MotivosCancelacion motivos = new MotivoCancelacionContext().MotivoCancelacions.Find(registro.Motivo);
                    motivoDes = motivos.DescripcionCan;
                }
                catch(Exception err)
                {

                }

                string estatus = DecodificadorSAT.getStatusUUID(_templatePDF.EstatusUUID.ToString());

                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase("Fecha y hora de solicitud: ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(_templatePDF.fechaEmisionCFDI.ToString("dd/MM/yyyy hh:mm:ss tt"), new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));

                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase("RFC Emisor:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(_templatePDF.emisor.rfc, new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));

                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase("Folio Fiscal:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(_templatePDF.folioFiscalUUID, new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));

                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase("Estatus de Proceso de Cancelación:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(estatus, new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));

                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase("Motivo de Cancelación:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(motivoDes, new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));

                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase("CFDI Reemplaza:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase("---", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                tabla.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));


                _documento.Add(tabla);

                float[] anchoColumnas1 = { 100f, 200f, 300F };
                PdfPTable tabla_ = new PdfPTable(anchoColumnas1);
                tabla_.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla_.SetTotalWidth(anchoColumnas1);
                tabla_.HorizontalAlignment = Element.ALIGN_LEFT;
                tabla_.LockedWidth = true;

                tabla_.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA,10)));
                tabla_.AddCell(new Phrase("Sello Digital SAT: ",new Font(Font.FontFamily.HELVETICA,10,Font.BOLD)));
                tabla_.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10)));
               
                _documento.Add(tabla_);

                float[] anchoColumnas2 = { 100f, 400f, 100f };
                PdfPTable tabla1_ = new PdfPTable(anchoColumnas2);
                tabla1_.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla1_.SetTotalWidth(anchoColumnas2);
                tabla1_.HorizontalAlignment = Element.ALIGN_LEFT;
                tabla1_.LockedWidth = true;

                tabla1_.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla1_.AddCell(new Phrase(_templatePDF.selloDigitalCFDI, new Font(Font.FontFamily.HELVETICA, 10)));
                tabla1_.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));

                _documento.Add(tabla1_);

            }
            catch (Exception err)
            {
                string mensajerror = err.Message;
            }

        }

        #endregion

    }


}