using iTextSharp.text;
using iTextSharp.text.pdf;

using SIAAPI.clasescfdi;
using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace SIAAPI.Reportes
{


    public class ProductoOC
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
        public string desc { get; internal set; }
        public float v_unitario = 0.00f;
        public float descuento { get; internal set; }

        public string Tipo = "Articulo";


    }

    public class ImpuestoOC
    {
        public string impuesto;
        public float tasa;
        public float importe;
    }

    public class RetencionOC
    {
        public string impuesto;
        public float importe;
    }

    public class DocumentoOrdenCompra
    {
        public string serie = string.Empty;
        public string folio = string.Empty;

        public string fecha = string.Empty;

        public string fechaRequerida = string.Empty;

        public string lugarExpedicion = string.Empty;
        public string formaPago = string.Empty;
        public string metodoPago = string.Empty;
        public string claveMoneda = string.Empty;
        public string condicionesdepago = string.Empty;
        public string firmadefinanzas = string.Empty;
        public string firmadecompras = string.Empty;

        public string Empresa = string.Empty;
        public string Direccion = string.Empty;
        public string Telefono = string.Empty;
        public string RFC = string.Empty;
        public string DireccionProveedor = string.Empty;


        public string Proveedor = string.Empty;
        public string RFCproveedor = string.Empty;
        public string Telefonoproveedor = string.Empty;
        public string Confianza = string.Empty;

        public string Observacion = string.Empty;

        public string cadenaOriginal = string.Empty;

        public float subtotal = 0.00f;
        public float total = 0.00f;
        public float descuento = 0.00f;

        public int IDALmacen =0;
        public string totalEnLetra = string.Empty;
        public float totalImpuestosRetenidos = 0.00f;


        public string tipo_cambio { get; internal set; }

        public string UsodelCFDI { get; set; }

        public string Autorizado = "Inactivo";

        public string Entregaren { get; set; }
        public string vendedor { get; set; }

        public List<ProductoOC> productos = new List<ProductoOC>();
        public List<ImpuestoOC> impuestos = new List<ImpuestoOC>();
        public List<RetencionOC> retenciones = new List<RetencionOC>();

       
    }

    public class CreaOrdenCompraPDF
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public CMYKColor colordefinido;
        public DocumentoOrdenCompra _templatePDF; //Objeto que contendra la información del documento pdf
        public string nombreDocumento = string.Empty;

        public CreaOrdenCompraPDF(System.Drawing.Image logo, DocumentoOrdenCompra Encorden)
        {
            _templatePDF = Encorden;
            ObtenerLetras();
            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;
            //Trabajos con el documento XML
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/COMPRA" + Encorden + ".pdf"); ;
            _documento = new Document(PageSize.LETTER);
            try
            {
                if (File.Exists(nombreDocumento))
                {
                    nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/COMPRA" + Encorden + DateTime.Now.Day + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".pdf");
                }
            }
            catch (Exception ERR)
            {
                string mensajederror = ERR.Message;
            }
            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            _writer.PageEvent = new ITextEventsOC(); // invoca la clase que esta mas abajo correspondiente al pie de pagina
            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();
            //Abrimos el documento
            _documento.Open();
            _cb = _writer.DirectContentUnder;
            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor("");
            AgregarDatosFactura();
            AgregarDatosReceptor();
            AgregarDatosProductos();
            AgregarTotales();
            Agregarpie();
            //     AgregarSellos();
            //Cerramoe el documento
            _documento.Close();
            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"" + _templatePDF.Proveedor  + _templatePDF.serie + _templatePDF.folio + ".pdf" +"\"");
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Write(_documento);
            //HttpContext.Current.Response.End();
        }
        private void Agregarpie()
        {
            float[] anchoColumasTablapie = { 600f };
            PdfPTable tablapie = new PdfPTable(anchoColumasTablapie);
            tablapie.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablapie.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapie.SetTotalWidth(anchoColumasTablapie);
            tablapie.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapie.LockedWidth = true;

            tablapie.AddCell(new Phrase("Horario de entrega de Lunes a Viernes de 8:00 am a 4:30 pm.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablapie.AddCell(new Phrase("SE RECIBIRA UNICA Y EXCLUSIVAMENTE LA CANTIDAD EXPUESTA EN ESTE PEDIDO, POR LO QUE NO SE ADMITIRA FALTANTE O SOBRANTE ALGUNO.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablapie.AddCell(new Phrase("Condiciones de Pago: " + _templatePDF.condicionesdepago, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablapie.AddCell(new Phrase("El proveedor se obliga a vender y entregar los productos y/o en proporcionar los servicios especificados en este documento, de acuerdo con los términos, condiciones y cláusulas de la Orden de compra, y de sus cláusulas especiales, modificaciones o complementos.Todo lo anterior, constituye un acuerdo final y completo entre las partes.Los términos, condiciones y cláusulas de esta Orden de Compra se encuentran en este documento.Las obligaciones del comprador quedan expresamente limitadas en los términos, condiciones y clausulas aquí contenidas.Cualquier otro término, condición o cláusula que proponga el Proveedor no será valida a menos que sea aceptada por escrito por el Comprador.Nos reservamos el derecho a realizar Auditoria en las instalaciones del Proveedor.\n\nDocumentos para recepcion indispensables\n-Ticket de entrega y copia\n-Esta Orden de Compra\n-Certificado de Calidad\nCertificado de fumigacion de la unidad que entrega", new Font(Font.FontFamily.HELVETICA, 6)));

            float anchocolumna1 = 250f;
            float anchocolumna2 = 250f;
            float anchocolumna3 = 100f;

            float[] anchoColumasTablaFirmas = { anchocolumna1, anchocolumna2, anchocolumna3 };
            PdfPTable tablafirmas = new PdfPTable(anchoColumasTablaFirmas);
            tablafirmas.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.DefaultCell.Border = Rectangle.NO_BORDER;
            tablafirmas.SetTotalWidth(anchoColumasTablaFirmas);
            tablafirmas.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.LockedWidth = true;
           
            tablafirmas.SpacingBefore = 25;

            iTextSharp.text.pdf.Barcode128 bc = new Barcode128();
            bc.TextAlignment = Element.ALIGN_CENTER;
            bc.Code = _templatePDF.folio.ToString();  //el id de la factura es el numero de ticket
            bc.StartStopText = false;
            bc.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
            bc.Extended = true;
            //bc.Font = null;

            iTextSharp.text.Image PatImage1 = bc.CreateImageWithBarcode(_cb, iTextSharp.text.BaseColor.BLACK, iTextSharp.text.BaseColor.BLACK);
            PatImage1.ScaleToFit(60, 40);



            PdfPCell barcideimage = new PdfPCell(PatImage1);
            //barcideimage.Colspan = 2;
            barcideimage.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            barcideimage.Border = 0;

        


           


            tablafirmas.AddCell(new Phrase("Autorizó.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablafirmas.AddCell(new Phrase("Aceptacion del departamento de finanzas", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablafirmas.AddCell(barcideimage);








            try
            {
                Image jpg = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/Firmacompras1.jpg"));
                jpg.ScaleToFit(100f, 150f);
                //PdfPCell imageCell = new PdfPCell(jpg);

                //  imageCell.Colspan = 2; // either 1 if you need to insert one cell
                //  imageCell.Border = 0;
                //  imageCell.HorizontalAlignment = (Element.ALIGN_CENTER);
                ////  imageCell.Width = anchocolumna1;
                //  tablafirmas.AddCell(jpg);

                //  jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(75, 100f);
              
                _documento.Add(jpg);

            }
            catch (Exception err)
            {
                //string mensajeerror = err.Message;
                //tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));

            }

            try
            {
                if (_templatePDF.Autorizado == "Activo")
                { 
                Image jpg1 = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/Firmacompras2.jpg"));
                jpg1.ScaleToFit(100f, 150f);
                jpg1.SetAbsolutePosition(325, 100f);


                //   imageCell1.Width = anchocolumna2;
                _documento.Add(jpg1);
                }

            }
            catch (Exception err)
            {
                //string mensajeerror = err.Message;
                //tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));

            }

            tablafirmas.AddCell(new Phrase(_templatePDF.firmadecompras, new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablafirmas.AddCell(new Phrase(_templatePDF.firmadefinanzas, new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));

            StringBuilder codigoQR = new StringBuilder();
            codigoQR.Append("http://" + SIAAPI.Properties.Settings.Default.Nombredelaaplicacion + "/EncOrdenCompra/Details/" + _templatePDF.folio);

            BarcodeQRCode pdfCodigoQR = new BarcodeQRCode(codigoQR.ToString(), 1, 1, null);
            Image img = pdfCodigoQR.GetImage();
            img.ScaleToFit(80f, 80f);
            img.SpacingAfter = 0.0f;
            img.SpacingBefore = 0.0f;
            img.BorderWidth = 1.0f;
            img.Alignment = Element.ALIGN_LEFT;
            //img.ScalePercent(100, 78);
            //img.border
            //PdfPCell imageCell2 = new PdfPCell(img);
            //imageCell2.Width = anchocolumna3;
            img.SetAbsolutePosition(500, 45f);
            
            _documento.Add(img);
            //tablafirmas.AddCell(img);
            tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));
            tablafirmas.AddCell(new Phrase("Nivel:"+ _templatePDF.Confianza, new Font(Font.FontFamily.HELVETICA, 8)));
            tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));
            tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));

            _documento.Add(tablapie);
            _documento.Add(tablafirmas);
        }



        #region Leer datos del .xml





        #endregion

        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            //Agrego la imagen al documento
            Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
            imagen.ScaleToFit(120, 100);
          //  imagen.Alignment = Element.ALIGN_TOP;
            imagen.SetAbsolutePosition(15f, (_documento.PageSize.Height - 50));
        //    Chunk logo = new Chunk(imagen, 1, imagen.Height +10);
            _documento.Add(imagen);
        }

        private void AgregarCuadro()
        {
         
            //_cb.SaveState();
            //_cb.BeginText();
            //_cb.MoveText(1, 1);
            //_cb.SetFontAndSize(_fuenteTitulos, 8);
            //_cb.ShowText("Faustino Rojas Arelano");
            //_cb.EndText();
            //_cb.RestoreState();

            //Agrego cuadro al documento
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
            p1.Alignment = Element.ALIGN_CENTER;
            p1.IndentationLeft = 50f;
            p1.IndentationRight = 100f;
            p1.SpacingBefore = 15;
            p1.Leading = 9;

            p1.Add(new Phrase(_templatePDF.Empresa, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.RFC, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Direccion, new Font(Font.FontFamily.HELVETICA, 8)));

            p1.Add("\n");
            p1.Add(new Phrase(SIAAPI.Properties.Settings.Default.paginaoficial, new Font(Font.FontFamily.HELVETICA, 10,Font.BOLD)));

            p1.Add("\n");
            p1.Add(new Phrase(SIAAPI.Properties.Settings.Default.Eslogan, new Font(Font.FontFamily.HELVETICA, 8)));


            p1.SpacingAfter = -80;
            p1.Add("\n");
            p1.Add("\n");

            //if (_templatePDF.emisor.telefono != string.Empty)
            //{
            //    p1.Add(new Phrase("Tel." + _templatePDF.emisor.telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            //    p1.Add("\n");
            //}
            _documento.Add(p1);
        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Orden de compra");
            _documento.AddSubject("Orden de Compra");
            _documento.AddTitle("ORDEN DE COMPRA");
            _documento.SetMargins(5, 5, 5, 5);
        }

        private void AgregarDatosFactura()
        {
            //Datos de la factura
            Paragraph p2 = new Paragraph();
            p2.IndentationLeft = 403f;
           
            p2.Leading = 10;
            p2.Alignment = Element.ALIGN_CENTER;

            p2.Add(new Phrase("ORDEN DE COMPRA NÚM: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase("\n"  + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 16)));




            p2.Add(new Phrase("\nFECHA "+ _templatePDF.fecha, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

            p2.Add("\n");
            p2.Add(new Phrase("\nFECHA EN LA QUE SE REQUIERE \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase(_templatePDF.fechaRequerida, new Font(Font.FontFamily.HELVETICA, 8)));

            p2.Add(new Phrase("\nCODIGO: FSG-27 ", new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add(new Phrase("\nREV. 1", new Font(Font.FontFamily.HELVETICA, 8)));
            p2.SpacingAfter = 10;

            _documento.Add(p2);
        }

        private void AgregarDatosReceptor()
        {
            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SpacingBefore = 5;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnaspro = { 80f, 220F, 20f, 80f, 200f };
            PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
            tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapro.SetTotalWidth(anchoColumnaspro);
            tablapro.HorizontalAlignment = Element.ALIGN_LEFT;
            tablapro.LockedWidth = true;

            Almacen almacen = new AlmacenContext().Almacenes.Find(_templatePDF.IDALmacen);

            PdfPCell celda0 = new PdfPCell(new Phrase("PROVEEDOR: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            string DireccionProveedor = string.Empty;


            PdfPCell celda1 = new PdfPCell(new Phrase(_templatePDF.Proveedor.ToUpper() + "\n DIRECCIÓN: " + _templatePDF.DireccionProveedor, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda2 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda3 = new PdfPCell(new Phrase("ENTREGAR EN: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda4 = new PdfPCell(new Phrase(almacen.Descripcion + " " + almacen.DIRECCION.ToUpper() + ", " + almacen.Colonia.ToUpper() + ", " + almacen.Municipio.ToUpper() + ", CP: " + almacen.CP + ", TEL. " + almacen.Telefono + ", RESPONSABLE: " + almacen.Responsable.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));




            celda0.Border = Rectangle.NO_BORDER;
            celda1.Border = Rectangle.NO_BORDER;
            celda2.Border = Rectangle.NO_BORDER;
            celda3.Border = Rectangle.NO_BORDER;
            celda4.Border = Rectangle.NO_BORDER;

            tablapro.AddCell(celda0);
            tablapro.AddCell(celda1);
            tablapro.AddCell(celda2);
            tablapro.AddCell(celda3);
            tablapro.AddCell(celda4);

            tablapro.CompleteRow();
            _documento.Add(tablapro);


            float[] anchoColumnasobserva = { 80f, 520f };
            PdfPTable tablaobservacion = new PdfPTable(anchoColumnasobserva);
            tablaobservacion.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaobservacion.SetTotalWidth(anchoColumnasobserva);
            tablaobservacion.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaobservacion.LockedWidth = true;

            //tablaobservacion.AddCell(new Phrase("Entregar en :".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //tablaobservacion.AddCell(new Phrase( almacen.Descripcion + " "+ almacen.DIRECCION.ToUpper() + ", " + almacen.Colonia.ToUpper() + ", "+ almacen.Municipio.ToUpper()+", Tel. "+ almacen.Telefono + " Resposable : " + almacen.Responsable.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));



            tablaobservacion.AddCell(new Phrase("OBSERVACION :".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaobservacion.AddCell(new Phrase(_templatePDF.Observacion, new Font(Font.FontFamily.HELVETICA, 8)));




            //      tablaDatosPrincipal.AddCell(tablaobservacion);
            //

            _documento.Add(tablaobservacion);

            float[] anchoColumnas = { 70f, 80f, 120F, 100f, 120f, 100f };
            PdfPTable tabla1 = new PdfPTable(anchoColumnas);
            tabla1.DefaultCell.Border = Rectangle.BOX;
            tabla1.SetTotalWidth(anchoColumnas);
            tabla1.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla1.LockedWidth = true;




            PdfPCell celdaMO = new PdfPCell(new Phrase("MONEDA ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaCR = new PdfPCell(new Phrase("CONDICIONES".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaUS = new PdfPCell(new Phrase("USO DE CFDI".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaFO = new PdfPCell(new Phrase("FORMA DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaME = new PdfPCell(new Phrase("METODO DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaTI = new PdfPCell(new Phrase("TIPO DE CAMBIO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            // PdfPCell celdaAL = new PdfPCell(new Phrase("ALMACEN".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));





            celdaMO.BackgroundColor = colordefinido;
            celdaUS.BackgroundColor = colordefinido;
            celdaCR.BackgroundColor = colordefinido;
            celdaFO.BackgroundColor = colordefinido;
            celdaME.BackgroundColor = colordefinido;
            celdaTI.BackgroundColor = colordefinido;
            // celdaAL.BackgroundColor = colordefinido;

            tabla1.AddCell(celdaMO);
            tabla1.AddCell(celdaCR);
            tabla1.AddCell(celdaUS);
            tabla1.AddCell(celdaFO);
            tabla1.AddCell(celdaME);
            tabla1.AddCell(celdaTI);
            //  tabla1.AddCell(celdaAL);





            tabla1.CompleteRow();


            tabla1.AddCell(new Phrase(_templatePDF.claveMoneda, new Font(Font.FontFamily.HELVETICA, 6)));
            tabla1.AddCell(new Phrase(_templatePDF.condicionesdepago, new Font(Font.FontFamily.HELVETICA, 6)));
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

            // tabla1.AddCell(new Phrase(_templatePDF.IDALmacen.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));

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
            float[] tamanoColumnas = { 60f, 35f, 50f, 70f, 200f, 70f, 60f, 55f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("CLAVE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaRequi = new PdfPCell(new Phrase("REQUI", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

            PdfPCell celda1 = new PdfPCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda2 = new PdfPCell(new Phrase("CLAVE UNIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda3 = new PdfPCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda4 = new PdfPCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda5 = new PdfPCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda6 = new PdfPCell(new Phrase("DESCUENTO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));



            celda0.BackgroundColor = colordefinido;
            celdaRequi.BackgroundColor = colordefinido;
            celda1.BackgroundColor = colordefinido;
            celda2.BackgroundColor = colordefinido;
            celda3.BackgroundColor = colordefinido;
            celda4.BackgroundColor = colordefinido;
            celda5.BackgroundColor = colordefinido;
            celda6.BackgroundColor = colordefinido;

            tablaProductosTitulos.AddCell(celda0);
            tablaProductosTitulos.AddCell(celdaRequi);
            tablaProductosTitulos.AddCell(celda1);
            tablaProductosTitulos.AddCell(celda2);
            tablaProductosTitulos.AddCell(celda3);
            tablaProductosTitulos.AddCell(celda4);
            tablaProductosTitulos.AddCell(celda5);
            tablaProductosTitulos.AddCell(celda6);









            var result2 = from line in _templatePDF.productos
                          group line by new { line.ClaveProducto, line.descripcion, line.Presentacion, line.Observacion } into csLine
                          select new Lineadeproductos
                          {
                              ClaveProducto = csLine.First().ClaveProducto,
                              Cantidad = csLine.Sum(c => decimal.Parse(c.cantidad)),
                              Unidad = csLine.First().c_unidad,
                              Descripcion = csLine.First().descripcion,
                              v_unitario = decimal.Parse(csLine.First().v_unitario.ToString()),
                              importe = csLine.Sum(c => Math.Round(decimal.Parse(c.importe.ToString()), 2)),
                              descuento = decimal.Parse(csLine.First().descuento.ToString()),
                              Presentacion = csLine.First().Presentacion,
                              Observacion = csLine.First().Observacion,
                              TipoArticulo = csLine.First().Tipo
                          };


            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);

            tablaProductosPrincipal.AddCell(celdaTitulos);
            foreach (Lineadeproductos p in result2)
            {

                float[] tamanoColumnasProductos = { 60f, 35f, 50f, 70f, 200f, 70f, 60f, 55f };
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

                tablaProductos.AddCell(new Phrase(p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 6)));
                try
                {
                    ProveedorContext db1 = new ProveedorContext();
                    string cadenaA = "select idarticulo as Dato from Articulo where cref='" + p.ClaveProducto + "'";
                    ClsDatoEntero idarticuloA = db1.Database.SqlQuery<ClsDatoEntero>(cadenaA).ToList().FirstOrDefault();

                    string cadenacaraA = "select id as Dato from Caracteristica where presentacion='" + p.Presentacion + "' and Articulo_idarticulo=" + idarticuloA.Dato;
                    ClsDatoEntero idcaracteristicaA = db1.Database.SqlQuery<ClsDatoEntero>(cadenacaraA).ToList().FirstOrDefault();

                    List<DetRequisiciones> detalles = new PrefacturaContext().Database.SqlQuery<DetRequisiciones>("Select * from detrequisiciones as d inner join elementosOrdenCompra as e on d.iddetrequisiciones=e.iddetdocumento where e.idordencompra=" + int.Parse(_templatePDF.folio) + " and d.Caracteristica_id=" + idcaracteristicaA.Dato + " and status!='Cancelado'").ToList();

                    string requis = "";
                    string NR = "N/A";
                    foreach (var nodo in detalles)
                    {
                        string nodoR = "";

                        nodoR = nodo.IDRequisicion.ToString();

                        requis += nodoR + "\n";

                    }
                    if (requis != "")
                    {
                        NR = requis;
                    }



                    tablaProductos.AddCell(new Phrase(NR, new Font(Font.FontFamily.HELVETICA, 6)));
                }
                catch (Exception err)
                {
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));
                }


                tablaProductos.AddCell(new Phrase(p.Cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Unidad, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdauni.Phrase = new Phrase(p.v_unitario.ToString("#,#0.0000"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);
                PdfPCell celdadescuento = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdadescuento.Phrase = new Phrase(p.descuento.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));

                tablaProductos.AddCell(celdadescuento);
                // AQUI CAMBIA DE RENGLON 
                //RFC PROVEEDOR
                ProveedorContext db = new ProveedorContext();
                string NombreProveedor = _templatePDF.Proveedor.Replace("'", "''");
                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].[Proveedores] where [Empresa] ='" + NombreProveedor + "'").ToList()[0];
                int p1 = c.Dato;

                string id = "select idarticulo as Dato from Articulo where cref='" + p.ClaveProducto + "'";
                ClsDatoEntero IDArticulo = db.Database.SqlQuery<ClsDatoEntero>(id).ToList().FirstOrDefault();

                string MATRIZ = "select cref as Dato from MatrizPrecioProv where idproveedor=" + p1 + " and idarticulo=" + IDArticulo.Dato;
                ClsDatoString cref = db.Database.SqlQuery<ClsDatoString>(MATRIZ).ToList().FirstOrDefault();


                if (cref != null)
                {


                    tablaProductos.AddCell(new Phrase(cref.Dato, new Font(Font.FontFamily.HELVETICA, 6)));
                }
                else
                {
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));

                }

                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));

                if (p.TipoArticulo != "Cintas")
                {
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                }
                else
                {
                    try
                    {

                        double ancho = new Clsmanejocadena().getdouble("ANCHO", p.Presentacion);
                        double largo = new Clsmanejocadena().getdouble("LARGO", p.Presentacion);

                        double numerodecintas = (double.Parse(p.Cantidad.ToString()) / (ancho / 1000)) / largo;
                        tablaProductos.AddCell(new Phrase(Math.Round(numerodecintas, 2).ToString(), new Font(Font.FontFamily.HELVETICA, 6)));
                        tablaProductos.AddCell(new Phrase("Cintas", new Font(Font.FontFamily.HELVETICA, 6)));
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                        tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                        tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

                    }

                }



                tablaProductos.AddCell(new Phrase(p.Presentacion, new Font(Font.FontFamily.TIMES_ROMAN, 8)));

                tablaProductos.CompleteRow();

                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 5)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));


                string cadena = "select idarticulo as Dato from Articulo where cref='" + p.ClaveProducto + "'";
                ClsDatoEntero idarticulo = db.Database.SqlQuery<ClsDatoEntero>(cadena).ToList().FirstOrDefault();

                string cadenacara = "select id as Dato from Caracteristica where presentacion='" + p.Presentacion + "' and Articulo_idarticulo=" + idarticulo.Dato;
                ClsDatoEntero idcaracteristica = db.Database.SqlQuery<ClsDatoEntero>(cadenacara).ToList().FirstOrDefault();


                //string ob = "select nota as Dato from detOrdenCompra where idProveedor=" + c.Dato + " and idcaracteristica=" + idcaracteristica.Dato;
                //ClsDatoString Observacion = db.Database.SqlQuery<ClsDatoString>(ob).ToList().FirstOrDefault();

                string ob1 = "select observacion as Dato from MatrizPrecioProv where idProveedor=" + c.Dato + " and idarticulo=" + idarticulo.Dato;
                ClsDatoString Observacionmatriz = db.Database.SqlQuery<ClsDatoString>(ob1).ToList().FirstOrDefault();



                tablaProductos.CompleteRow();

                // otro renglon si tiene observacion

                if (!string.IsNullOrEmpty(p.Observacion))
                {
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase(p.Observacion, new Font(Font.FontFamily.TIMES_ROMAN, 6)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

                }


                PdfPCell celdaProductos = new PdfPCell(tablaProductos);

                tablaProductosPrincipal.AddCell(celdaProductos);
            }


            _documento.Add(tablaProductosPrincipal);
        }



        //private void AgregarDatosProductos()
        //{
        //    float[] anchoColumnasTablaProductos = { 600f };
        //    PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
        //    //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
        //    tablaProductosPrincipal.SpacingBefore = 15;
        //    tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaProductosPrincipal.LockedWidth = true;


        //    //Datos de los productos
        //    float[] tamanoColumnas = { 60f, 50f, 70f, 230f, 75f, 60f, 55f };
        //    PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

        //    //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
        //    tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaProductosTitulos.LockedWidth = true;



        //    PdfPCell celda0 = new PdfPCell(new Phrase("CLAVE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda1 = new PdfPCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda2 = new PdfPCell(new Phrase("CLAVE UNIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda3 = new PdfPCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda4 = new PdfPCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda5 = new PdfPCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda6 = new PdfPCell(new Phrase("DESCUENTO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));



        //    celda0.BackgroundColor = colordefinido;
        //    celda1.BackgroundColor = colordefinido;
        //    celda2.BackgroundColor = colordefinido;
        //    celda3.BackgroundColor = colordefinido;
        //    celda4.BackgroundColor = colordefinido;
        //    celda5.BackgroundColor = colordefinido;
        //    celda6.BackgroundColor = colordefinido;

        //    tablaProductosTitulos.AddCell(celda0);
        //    tablaProductosTitulos.AddCell(celda1);
        //    tablaProductosTitulos.AddCell(celda2);
        //    tablaProductosTitulos.AddCell(celda3);
        //    tablaProductosTitulos.AddCell(celda4);
        //    tablaProductosTitulos.AddCell(celda5);
        //    tablaProductosTitulos.AddCell(celda6);





        //    float[] tamanoColumnasProductos = { 60f, 50f, 70f, 230f, 75f, 60f, 55f };
        //    PdfPTable tablaProductos = new PdfPTable(tamanoColumnas);
        //    //tablaProductos.SpacingBefore = 1;
        //    //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaProductos.DefaultCell.BorderWidthLeft = 0.1f;
        //    tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
        //    tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
        //    tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
        //    tablaProductos.SetTotalWidth(tamanoColumnas);
        //    tablaProductos.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaProductos.LockedWidth = true;




        //    //List<Lineadeproductos> result = _templatePDF.productos
        //    //    .GroupBy(l => l.descripcion)
        //    //    .SelectMany(cl => cl.Select(
        //    //        csLine => new Lineadeproductos
        //    //        {
        //    //            ClaveProducto = csLine.ClaveProducto,
        //    //            Cantidad = cl.Sum(c => decimal.Parse(c.cantidad)),
        //    //            Unidad = csLine.c_unidad,
        //    //            Descripcion = csLine.descripcion,
        //    //            v_unitario = decimal.Parse(csLine.v_unitario.ToString()),
        //    //            importe = cl.Sum(c =>Math.Round( decimal.Parse(c.importe.ToString()),2)),
        //    //            descuento = decimal.Parse(csLine.descuento.ToString()),
        //    //            Presentacion = csLine.Presentacion,
        //    // })).ToList<Lineadeproductos>();


        //    var result2 = from line in _templatePDF.productos
        //                                     group line by new { line.descripcion, line.Presentacion, line.Observacion } into csLine
        //                                     select new Lineadeproductos
        //                                     {
        //                                         ClaveProducto = csLine.First().ClaveProducto,
        //                                         Cantidad = csLine.Sum(c => decimal.Parse(c.cantidad)),
        //                                         Unidad = csLine.First().c_unidad,
        //                                         Descripcion = csLine.First().descripcion,
        //                                         v_unitario = decimal.Parse(csLine.First().v_unitario.ToString()),
        //                                         importe = csLine.Sum(c => Math.Round(decimal.Parse(c.importe.ToString()), 2)),
        //                                         descuento = decimal.Parse(csLine.First().descuento.ToString()),
        //                                         Presentacion = csLine.First().Presentacion,
        //                                         Observacion = csLine.First().Observacion,
        //                                         TipoArticulo = csLine.First().Tipo
        //                                     };


        //    foreach (Lineadeproductos p in result2)
        //    {
        //        tablaProductos.AddCell(new Phrase(p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 8)));

        //        tablaProductos.AddCell(new Phrase(p.Cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
        //        tablaProductos.AddCell(new Phrase(p.Unidad, new Font(Font.FontFamily.HELVETICA, 8)));
        //        tablaProductos.AddCell(new Phrase(p.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
        //        PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
        //        celdauni.Phrase = new Phrase(p.v_unitario.ToString("#,#0.0000"), new Font(Font.FontFamily.HELVETICA, 8));
        //        tablaProductos.AddCell(celdauni);
        //        PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
        //        celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
        //        tablaProductos.AddCell(celdaimporte);
        //        PdfPCell celdadescuento = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
        //        celdadescuento.Phrase = new Phrase(p.descuento.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));

        //        tablaProductos.AddCell(celdadescuento);
        //        // AQUI CAMBIA DE RENGLON 
        //        tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));


        //        if (p.TipoArticulo != "Cintas")
        //        {
        //            tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //            tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //        }
        //        else
        //        {
        //            try
        //            {

        //                double ancho = new Clsmanejocadena().getdouble("ANCHO", p.Presentacion);
        //                double largo = new Clsmanejocadena().getdouble("LARGO", p.Presentacion);

        //                double numerodecintas = (double.Parse( p.Cantidad.ToString()) / (ancho/1000))/largo;
        //                tablaProductos.AddCell(new Phrase(Math.Round( numerodecintas,2) .ToString(), new Font(Font.FontFamily.HELVETICA, 6)));
        //                tablaProductos.AddCell(new Phrase("Cintas", new Font(Font.FontFamily.HELVETICA, 6)));
        //            }
        //            catch(Exception  err)
        //            {
        //                string mensajeerror = err.Message;
        //                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

        //            }

        //        }



        //        tablaProductos.AddCell(new Phrase(p.Presentacion, new Font(Font.FontFamily.TIMES_ROMAN, 6)));
        //        tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //        tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //        tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

        //        // otro renglon si tiene observacion

        //        if (!string.IsNullOrEmpty(p.Observacion))
        //        {
        //            tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

        //            tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //            tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //            tablaProductos.AddCell(new Phrase(p.Observacion, new Font(Font.FontFamily.TIMES_ROMAN, 6)));
        //            tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //            tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //            tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //        }



        //    }


        //    PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
        //    tablaProductosPrincipal.AddCell(celdaTitulos);
        //    PdfPCell celdaProductos = new PdfPCell(tablaProductos);
        //    celdaProductos.MinimumHeight = 240;
        //    tablaProductosPrincipal.AddCell(celdaProductos);
        //    _documento.Add(tablaProductosPrincipal);
        //}

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

            foreach (ImpuestoOC i in _templatePDF.impuestos)
            {
                tablaImportes.AddCell(new Phrase(i.impuesto + " " + i.tasa.ToString("F2") + "%:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            foreach (RetencionOC i in _templatePDF.retenciones)
            {
                tablaImportes.AddCell(new Phrase("Retencion " + i.impuesto + ": ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            tablaImportes.AddCell(new Phrase("Total:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase(_templatePDF.total.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

            tablaTotales.AddCell(new Phrase("IMPORTE CON LETRA: " + _templatePDF.totalEnLetra.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            tablaTotales.AddCell(tablaImportes);
            _documento.Add(tablaTotales);
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
          

            _documento.Add(tablaSellosQR);
        }



        public string uso()
        {

            return DecodificadorSAT.getUso(_templatePDF.UsodelCFDI);
           
        }


        private void ObtenerLetras()
        {


            Numalet numaLet = new Numalet();
            numaLet.MascaraSalidaDecimal = "00/100 M.N.";
            numaLet.SeparadorDecimalSalida = "pesos";
            if (_templatePDF.claveMoneda =="USD" || _templatePDF.claveMoneda=="Dolar americano")
            {
                numaLet.SeparadorDecimalSalida = "dolares";
                numaLet.MascaraSalidaDecimal = "00/100";
            }
            if (_templatePDF.claveMoneda == "EUR" || _templatePDF.claveMoneda.ToUpper() == "EURO")
            {
                numaLet.SeparadorDecimalSalida = "Euros";
                numaLet.MascaraSalidaDecimal = "00/100";
            }
            numaLet.ApocoparUnoParteEntera = true;
            numaLet.LetraCapital = true;
            _templatePDF.totalEnLetra = numaLet.ToCustomString(double.Parse(_templatePDF.total.ToString()));


        }
        #endregion

    }


    public class Lineadeproductos
    {

        public Lineadeproductos() { }

        public string ClaveProducto { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public decimal v_unitario { get; set; }
        public decimal importe { get; set; }    
        public decimal descuento { get; set; }
        public string Presentacion { get; set; }

        public string Observacion { get; set; }

        public string TipoArticulo { get; set; }
    }




    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsOC : PdfPageEventHelper
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
        int totcountPage = 0;
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

           

            String text = "Página " + writer.PageNumber + " de ";
            String TextRevision = "REV. 1";


            
                cb.BeginText();
                cb.SetFontAndSize(bf, 9);
                cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
                //cb.MoveText(500,30);
                //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
                cb.ShowText(text);
                cb.EndText();
                float len = bf.GetWidthPoint(text, 9);
                cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

                float[] anchoColumasTablaTotales = {100f, 400f,100f };
                PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
                tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla.SetTotalWidth(anchoColumasTablaTotales);
                tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.LockedWidth = true;
                tabla.AddCell(new Phrase("Class Labels  S. de R.L. de C.V \n Fecha de Revisión:\n 27-10-2020", new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase("Este documento solo es valido si es obtenido de la lista maestra", new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase(""+ "\n"+ TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                //tabla.AddCell(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

            



        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);


           
            totcountPage = writer.PageNumber;

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 9);
            //footerTemplate.MoveText(550,30);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber).ToString());
            footerTemplate.EndText();
        }
    }
    #endregion
    


    }


