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
using System.Net.Mail;
using System.Net;
using SIAAPI.clasescfdi;
namespace SIAAPI.Reportes
{


    public class ProductoRecepcionD
    {

        public string cantidad = string.Empty;
        public string descripcion = string.Empty;
        public string unidad = string.Empty;
        public float valorUnitario = 0.00f;
        public float importe = 0.00f;
        public string numIdentificacion = string.Empty;

        public string Presentacion = string.Empty;
        public string Observacion = string.Empty;
        public string ClaveProducto { get; internal set; }
        public string id { get; internal set; }
        public string c_unidad { get; internal set; }
        public string cref { get; internal set; }
        public string desc { get; internal set; }
        public float v_unitario = 0.00f;
        public float descuento { get; internal set; }
    }

    public class ImpuestoRecepcionD
    {
        public string impuesto;
        public float tasa;
        public float importe;
    }

    public class RetencionRecepcionD
    {
        public string impuesto;
        public float importe;
    }

    public class DocumentoRecepcionD
    {
        public string serie = string.Empty;
        public string folio = string.Empty;

        public string fecha = string.Empty;

        public string fechaRequerida = string.Empty;

        public string lugarExpedicion = string.Empty;
        public string formaPago = string.Empty;
        public string metodoPago = string.Empty;
        public string claveMoneda = string.Empty;


        public string Empresa = string.Empty;
        public string Direccion = string.Empty;
        public string Telefono = string.Empty;
        public string RFC = string.Empty;


        public string Proveedor = string.Empty;
        public string RFCproveedor = string.Empty;
        public string Telefonoproveedor = string.Empty;

        public string Observacion = string.Empty;

        public string cadenaOriginal = string.Empty;
        public string DocumentoFactura = string.Empty;

        public float subtotal = 0.00f;
        public float total = 0.00f;
        public float descuento = 0.00f;

        public string IDALmacen = "00 NO ESPECIFICADO";

        public List<ProductoRecepcionD> productos = new List<ProductoRecepcionD>();
        public List<ImpuestoRecepcionD> impuestos = new List<ImpuestoRecepcionD>();
        public List<RetencionRecepcionD> retenciones = new List<RetencionRecepcionD>();

        public string totalEnLetra = string.Empty;
        public float totalImpuestosRetenidos = 0.00f;


        public string tipo_cambio { get; internal set; }

        public string UsodelCFDI { get; set; }
    }

    public class CreaDevolucionRecepcionnPDF
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public CMYKColor colordefinido;
        public DocumentoRecepcionD _templatePDF; //Objeto que contendra la información del documento pdf
        XmlDocument xDoc; // Objeto para abrir el archivo xml
        public string nombreDocumento = string.Empty;
        public CreaDevolucionRecepcionnPDF(System.Drawing.Image logo, DocumentoRecepcionD DE)
        {
            _templatePDF = DE;
            ObtenerLetras();

            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;
            //Trabajos con el documento XML
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/DEVOLUCIONRECEPCION" + DE + ".pdf"); ;
            _documento = new Document(PageSize.LETTER);
            try
            {
                if (File.Exists(nombreDocumento))
                {
                    nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/DEVOLUCIONRECEPCION" + DE + DateTime.Now.Day + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".pdf");
                }
            }
            catch (Exception ERR)
            {
                string mensajederror = ERR.Message;
            }
            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            _writer.PageEvent = new ITextEventsRec();
            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();
            //Abrimos el documento
            _documento.Open();
            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor("");
            AgregarDatosFactura();
            AgregarDatosReceptor();
            AgregarDatosProductos();
            AgregarTotales();
            AgregarSellos();
            //Cerramoe el documento
            _documento.Close();
            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment;" + "filename=Recepcion" + _templatePDF.serie + _templatePDF.folio + ".pdf");
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Write(_documento);
            //HttpContext.Current.Response.End();
           

        }



        #region Leer datos del .xml





        #endregion

        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            try
            {
                Image jpg = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/logo.jpg"));

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(140f, 140F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(30f, 680f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                _documento.Add(jpg);
                //  doc.Add(paragraph);
            }
            catch (Exception err)
            {

            }
        }

        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;

            _cb.SetColorStroke(colordefinido); //Color de la linea
            _cb.SetColorFill(colordefinido); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(408, 694, 20, 100);
            _cb.FillStroke();
        }

        private void AgregarDatosEmisor(String Telefono)
        {
            //Datos del emisor
            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 155f;
            p1.IndentationRight = 150f;
            p1.SpacingBefore = 30f;
            p1.Leading = 9;
            p1.Alignment = Element.ALIGN_CENTER;
            p1.Add(new Phrase(_templatePDF.Empresa, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.RFC, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Direccion, new Font(Font.FontFamily.HELVETICA, 8)));

            p1.Add("\n");
            p1.Add("\n");
            p1.Add(new Phrase(SIAAPI.Properties.Settings.Default.paginaoficial, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
            p1.Add("\n");
            p1.Add(new Phrase(SIAAPI.Properties.Settings.Default.Eslogan, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.SpacingAfter = -80;

            _documento.Add(p1);
        }
        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("DEVOLUCIÓN");
            _documento.AddSubject("Devolución");
            _documento.AddTitle("DEVOLUCIÓN");
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

            p2.Add(new Phrase("DEVOLUCIÓN NÚM: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase("\n" + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 16)));

            p2.Add(new Phrase("\nFECHA \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase(_templatePDF.fecha, new Font(Font.FontFamily.HELVETICA, 8)));


            p2.Add("\n");
            p2.Add(new Phrase("\nCODIGO: ITOQ-CPRAS-003 ", new Font(Font.FontFamily.HELVETICA, 8)));


            p2.SpacingAfter = 40;
            _documento.Add(p2);
        }

        private void AgregarDatosReceptor()
        {
            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SpacingBefore = 15;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnaspro = { 80f, 200f, 80f, 100f, 60f, 80f };
            PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
            tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapro.SetTotalWidth(anchoColumnaspro);
            tablapro.HorizontalAlignment = Element.ALIGN_LEFT;
            tablapro.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("PROVEEDOR: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celda1 = new PdfPCell(new Phrase(_templatePDF.Proveedor.ToUpper() + "\n DIRECCIÓN: " + _templatePDF.Direccion, new Font(Font.FontFamily.HELVETICA, 8)));
            //PdfPCell celda2 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda3 = new PdfPCell(new Phrase("TELEFONO: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda4 = new PdfPCell(new Phrase(_templatePDF.Telefonoproveedor, new Font(Font.FontFamily.HELVETICA, 8)));

            PdfPCell celda5 = new PdfPCell(new Phrase("TICKET: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda6 = new PdfPCell(new Phrase(_templatePDF.DocumentoFactura, new Font(Font.FontFamily.HELVETICA, 8)));




            celda0.Border = Rectangle.NO_BORDER;
            celda1.Border = Rectangle.NO_BORDER;
            //celda2.Border = Rectangle.NO_BORDER;
            celda3.Border = Rectangle.NO_BORDER;
            celda4.Border = Rectangle.NO_BORDER;
            celda5.Border = Rectangle.NO_BORDER;
            celda6.Border = Rectangle.NO_BORDER;

            tablapro.AddCell(celda0);
            tablapro.AddCell(celda1);
            //tablapro.AddCell(celda2);
            tablapro.AddCell(celda3);
            tablapro.AddCell(celda4);
            tablapro.AddCell(celda5);
            tablapro.AddCell(celda6);

            tablapro.CompleteRow();
            _documento.Add(tablapro);

            float[] anchoColumnas = { 60f, 120F, 100f, 100f, 100f, 120f };
            PdfPTable tabla1 = new PdfPTable(anchoColumnas);
            tabla1.DefaultCell.Border = Rectangle.BOX;
            tabla1.SetTotalWidth(anchoColumnas);
            tabla1.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla1.LockedWidth = true;




            PdfPCell celdaMO = new PdfPCell(new Phrase("MONEDA ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaUS = new PdfPCell(new Phrase("USO DE CFDI".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaFO = new PdfPCell(new Phrase("FORMA DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaME = new PdfPCell(new Phrase("METODO DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaTI = new PdfPCell(new Phrase("TIPO DE CAMBIO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaAL = new PdfPCell(new Phrase("ALMACEN".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));





            celdaMO.BackgroundColor = colordefinido;
            celdaUS.BackgroundColor = colordefinido;
            celdaFO.BackgroundColor = colordefinido;
            celdaME.BackgroundColor = colordefinido;
            celdaTI.BackgroundColor = colordefinido;
            celdaAL.BackgroundColor = colordefinido;

            tabla1.AddCell(celdaMO);
            tabla1.AddCell(celdaUS);
            tabla1.AddCell(celdaFO);
            tabla1.AddCell(celdaME);
            tabla1.AddCell(celdaTI);
            tabla1.AddCell(celdaAL);





            tabla1.CompleteRow();


            tabla1.AddCell(new Phrase(_templatePDF.claveMoneda, new Font(Font.FontFamily.HELVETICA, 6)));
            tabla1.AddCell(new Phrase(_templatePDF.UsodelCFDI, new Font(Font.FontFamily.HELVETICA, 6)));

            string f_pago = DecodificadorSAT.getFormapago(_templatePDF.formaPago);


            tabla1.AddCell(new Phrase(f_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));



            string m_pago = DecodificadorSAT.getMetodo(_templatePDF.metodoPago);

            tabla1.AddCell(new Phrase(m_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));


            ///cambio de renglon
            ///
            PdfPCell celdatc = new PdfPCell(new Phrase(_templatePDF.tipo_cambio, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            celdatc.HorizontalAlignment = Element.ALIGN_CENTER;



            tabla1.AddCell(celdatc);

            tabla1.AddCell(new Phrase(_templatePDF.IDALmacen.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));

            tabla1.CompleteRow();

            tablaDatosPrincipal.AddCell(tabla1);
            //
            _documento.Add(tablaDatosPrincipal);

        }

        private string ReturnClavePago(string clave)
        {
            string valor = "";

            valor = clave;

            return valor.ToString().ToUpper();
        }


        private String ReturnTiporelacion(String Clave)
        {
            String Valor = "";
            if (Clave == "01")
            {
                Valor = "01 Nota de crédito de los documentos relacionados";
            }

            if (Clave == "02")
            {
                Valor = "02 Nota de débito de los documentos relacionados";
            }

            if (Clave == "03")
            {
                Valor = "03 Devolución de mercancía sobre facturas o traslados previos";
            }
            if (Clave == "04")
            {
                Valor = "04 Sustitución de los CFDI previos";
            }
            if (Clave == "05")
            {
                Valor = "05 Traslados de mercancias facturados previamente";
            }
            if (Clave == "06")
            {
                Valor = "06 Factura generada por los traslados previos";
            }
            if (Clave == "07")
            {
                Valor = "07 CFDI por aplicación de anticipo";
            }
            return Valor;

        }

        private void AgregarDatosProductos()
        {
            float[] anchoColumnasTablaProductos = { 600f };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 15;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosPrincipal.LockedWidth = true;


            //Datos de los productos
            float[] tamanoColumnas = { 50f, 100f, 220f, 65f, 65f, 100f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;



            // PdfPCell celda0 = new PdfPCell(new Phrase("CLAVE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

            PdfPCell celda1 = new PdfPCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda2 = new PdfPCell(new Phrase("PRODUCTO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda3 = new PdfPCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda6 = new PdfPCell(new Phrase("DESCUENTO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda4 = new PdfPCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda5 = new PdfPCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));



            //celda0.BackgroundColor = colordefinido;
            celda1.BackgroundColor = colordefinido;
            celda2.BackgroundColor = colordefinido;
            celda3.BackgroundColor = colordefinido;
            celda4.BackgroundColor = colordefinido;
            celda5.BackgroundColor = colordefinido;
            celda6.BackgroundColor = colordefinido;

            //tablaProductosTitulos.AddCell(celda0);
            tablaProductosTitulos.AddCell(celda1);
            tablaProductosTitulos.AddCell(celda2);
            tablaProductosTitulos.AddCell(celda3);
            tablaProductosTitulos.AddCell(celda6);
            tablaProductosTitulos.AddCell(celda4);
            tablaProductosTitulos.AddCell(celda5);






            float[] tamanoColumnasProductos = { 50f, 100f, 220f, 65f, 65f, 100f };
            PdfPTable tablaProductos = new PdfPTable(tamanoColumnas);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductos.DefaultCell.BorderWidthLeft = 0.1f;
            tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
            tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
            tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
            tablaProductos.SetTotalWidth(tamanoColumnas);
            tablaProductos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductos.LockedWidth = true;
            foreach (ProductoRecepcionD p in _templatePDF.productos)
            {
                // tablaProductos.AddCell(new Phrase(p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 8)));

                tablaProductos.AddCell(new Phrase(p.cantidad, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.cref, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.descripcion + "  \n Clave:" + p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdadescuento = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdadescuento.Phrase = new Phrase(p.descuento.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));

                tablaProductos.AddCell(celdadescuento);


                PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdauni.Phrase = new Phrase(p.v_unitario.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);

                // AQUI CAMBIA DE RENGLON 
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                //tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Presentacion, new Font(Font.FontFamily.TIMES_ROMAN, 6)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));





            }


            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
            tablaProductosPrincipal.AddCell(celdaTitulos);
            PdfPCell celdaProductos = new PdfPCell(tablaProductos);
            celdaProductos.MinimumHeight = 240;
            tablaProductosPrincipal.AddCell(celdaProductos);
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
            if (_templatePDF.descuento > 0.00f)
            {
                float subtotal = _templatePDF.subtotal + _templatePDF.descuento;
                tablaImportes.AddCell(new Phrase("SUBTOTAL ANTES DE DESC.:", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(subtotal.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

                tablaImportes.AddCell(new Phrase("Descuento:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(_templatePDF.descuento.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            tablaImportes.AddCell(new Phrase("SUBTOTAL:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase(_templatePDF.subtotal.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

            foreach (ImpuestoRecepcionD i in _templatePDF.impuestos)
            {
                tablaImportes.AddCell(new Phrase(i.impuesto + " " + i.tasa.ToString("F2") + "%:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            foreach (RetencionRecepcionD i in _templatePDF.retenciones)
            {
                tablaImportes.AddCell(new Phrase("Retencion " + i.impuesto + ": ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            tablaImportes.AddCell(new Phrase("Total:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase(_templatePDF.total.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));


            tablaTotales.AddCell(new Phrase("IMPORTE CON LETRA: " + _templatePDF.totalEnLetra.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));



            tablaTotales.AddCell(tablaImportes);
            _documento.Add(tablaTotales);


            float[] anchoColumnasobserva = { 600f };

            PdfPTable tablaobservacion = new PdfPTable(anchoColumnasobserva);
            tablaobservacion.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaobservacion.SetTotalWidth(anchoColumnasobserva);
            tablaobservacion.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaobservacion.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("OBSERVACIONES: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celda1 = new PdfPCell(new Phrase(_templatePDF.Observacion.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            celda0.Border = Rectangle.NO_BORDER;
            celda1.Border = Rectangle.NO_BORDER;
            tablaobservacion.AddCell(celda0);
            tablaobservacion.AddCell(celda1);

            _documento.Add(tablaobservacion);

            float[] anchoColumnasoFirmas = { 600f };
            PdfPTable tablafirmas = new PdfPTable(anchoColumnasoFirmas);
            tablafirmas.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablafirmas.DefaultCell.Border = Rectangle.NO_BORDER;
            tablafirmas.SetTotalWidth(anchoColumnasoFirmas);
            tablafirmas.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.LockedWidth = true;

            PdfPCell celdaf1 = new PdfPCell(new Phrase("AUTORIZA", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            //PdfPCell celdaf2 = new PdfPCell(new Phrase("SOLICITANTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            celdaf1.Border = Rectangle.NO_BORDER;
            //celdaf2.Border = Rectangle.NO_BORDER;

            celdaf1.HorizontalAlignment = Element.ALIGN_CENTER;
            //celdaf2.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.AddCell("\n");
            tablafirmas.AddCell("\n");
            tablafirmas.AddCell("\n");
            tablafirmas.AddCell("\n");
            tablafirmas.AddCell(celdaf1);
            //tablafirmas.AddCell(celdaf2);

            _documento.Add(tablafirmas);



        }

        private void AgregarSellos()
        {


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

            //Agregamos el codigo QR al documento
            StringBuilder codigoQR = new StringBuilder();
            codigoQR.Append("https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx/serie=" + _templatePDF.serie + ",folio" + _templatePDF.folio);


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



        public string uso()
        {
            string usocfdi = string.Empty;
            if (_templatePDF.UsodelCFDI == "G01")
            {
                usocfdi = "G01: Adquisición de mercancias";
            }

            if (_templatePDF.UsodelCFDI == "G02")
            {
                usocfdi = "G02: Devoluciones, descuentos o bonificaciones";
            }

            if (_templatePDF.UsodelCFDI == "G03")
            {
                usocfdi = "G03: Gastos en general";
            }

            if (_templatePDF.UsodelCFDI == "I01")
            {
                usocfdi = "I01: Construcciones";
            }

            if (_templatePDF.UsodelCFDI == "I02")
            {
                usocfdi = "I02: Mobilario y equipo de oficina por inversiones";
            }

            if (_templatePDF.UsodelCFDI == "I03")
            {
                usocfdi = "I03: Equipo de transporte";
            }

            if (_templatePDF.UsodelCFDI == "I04")
            {
                usocfdi = "I04: Equipo de computo y accesorios";
            }

            if (_templatePDF.UsodelCFDI == "I05")
            {
                usocfdi = "I05: Dados, troqueles, moldes, matrices y herramental";
            }

            if (_templatePDF.UsodelCFDI == "I06")
            {
                usocfdi = "I06: Comunicaciones telefónicas";
            }

            if (_templatePDF.UsodelCFDI == "I08")
            {
                usocfdi = "I08: Otra maquinaria y equipo";
            }

            if (_templatePDF.UsodelCFDI == "D01")
            {
                usocfdi = "D01: Honorarios médicos, dentales y gastos hospitalarios.";
            }

            if (_templatePDF.UsodelCFDI == "D02")
            {
                usocfdi = "D02: Gastos médicos por incapacidad o discapacidad";
            }

            if (_templatePDF.UsodelCFDI == "D03")
            {
                usocfdi = "D03: Gastos funerales.";
            }

            if (_templatePDF.UsodelCFDI == "G01")
            {
                usocfdi = "G01: Adquisición de mercancias";
            }

            if (_templatePDF.UsodelCFDI == "D05")
            {
                usocfdi = "D05: Intereses reales efectivamente pagados por créditos hipotecarios (casa habitación).";
            }

            if (_templatePDF.UsodelCFDI == "D06")
            {
                usocfdi = "D06: Aportaciones voluntarias al SAR.";
            }

            if (_templatePDF.UsodelCFDI == "D07")
            {
                usocfdi = "D07: Primas por seguros de gastos médicos.";
            }

            if (_templatePDF.UsodelCFDI == "G01")
            {
                usocfdi = "G01: Adquisición de mercancias";
            }

            if (_templatePDF.UsodelCFDI == "D08")
            {
                usocfdi = "D08: Gastos de transportación escolar obligatoria.";
            }

            if (_templatePDF.UsodelCFDI == "D09")
            {
                usocfdi = "D09: Depósitos en cuentas para el ahorro, primas que tengan como base planes de pensiones.";
            }


            if (_templatePDF.UsodelCFDI == "D10")
            {
                usocfdi = "D10: Pagos por servicios educativos (colegiaturas)";
            }

            if (_templatePDF.UsodelCFDI == "P01")
            {
                usocfdi = "P01: Por definir";
            }
            return usocfdi;
        }


        private void ObtenerLetras()
        {


            NumaletReD numaLet = new NumaletReD();
            numaLet.MascaraSalidaDecimal = "00/100 M.N.";
            numaLet.SeparadorDecimalSalida = "pesos";
            numaLet.ApocoparUnoParteEntera = true;
            numaLet.LetraCapital = true;
            _templatePDF.totalEnLetra = numaLet.ToCustomString(double.Parse(_templatePDF.total.ToString()));


        }
        #endregion

    }









    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsRecD : PdfPageEventHelper
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
            String TextRevision = "REV. 2";


            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 9);
                cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
                //cb.MoveText(500,30);
                //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
                cb.ShowText(text);
                cb.EndText();
                float len = bf.GetWidthPoint(text, 9);
                cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

                float[] anchoColumasTablaTotales = { 600f };
                PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
                tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla.SetTotalWidth(anchoColumasTablaTotales);
                tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.LockedWidth = true;
                tabla.AddCell(new Phrase("Este documento es una Devolución ", new Font(Font.FontFamily.HELVETICA, 10)));
                tabla.AddCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

            }



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

    public sealed class NumaletReD
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

        public NumaletReD()
        {
            MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            SeparadorDecimalSalida = SeparadorDecimalSalidaDefault;
            LetraCapital = LetraCapitalDefault;
            ConvertirDecimales = _convertirDecimales;
        }

        public NumaletReD(Boolean ConvertirDecimales, String MascaraSalidaDecimal, String SeparadorDecimalSalida, Boolean LetraCapital)
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

}
