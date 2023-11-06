using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
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

    public class ProductoCotizapros
    {

        public string cantidad { get; set; }
        public string descripcion = string.Empty;
       
        public float valorUnitario = 0.00f;
        public float importe = 0.00f;
        public string Unidad { get; set; }
        public int idarticulo { get; set; }
        public string Presentacion { get; set; }

        public string Observacion = string.Empty;
      
        public float descuento { get;  set; }
    }

  
    public class Documentocotizapros
    {

        public string folio { get; set; }
        public string prospecto { get; set; }
        public string fecha { get; set; }

       public string condiciones { get; set; }

        public string Atencion { get; set; }
       
        public string Observacion = string.Empty;

        public string Moneda = string.Empty;

        public int Vigencia { get; set; }

        public decimal subtotal = 0.00M;
        public decimal total = 0.00M;
        public decimal iva = 0.00M;
       

        public int IDPedido = 0;

      

        public List<ProductoCotizapros> productos = new List<ProductoCotizapros>();
       

        public string totalEnLetra = string.Empty;

        public string tipo_cambio { get;  set; }


        public String Vendedor { get; set; }
    }

    public class CreaCotizaprosPDF
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public CMYKColor colordefinido;
        public Documentocotizapros _templatePDF; //Objeto que contendra la información del documento pdf
        XmlDocument xDoc; // Objeto para abrir el archivo xml
        public string nombreDocumento = string.Empty;
        public CreaCotizaprosPDF(System.Drawing.Image logo, Documentocotizapros PE)
        {
            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;
            _templatePDF = PE;
            ObtenerLetras();
            //Trabajos con el documento XML
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/COTIZAPROSPECTO" + PE + ".pdf"); ;
            _documento = new Document(PageSize.LETTER);
            try
            {
                if (File.Exists(nombreDocumento))
                {
                    nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/COTIZAPROSPECTO" + PE + DateTime.Now.Day + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".pdf");
                }
            }
            catch (Exception ERR)
            {
                string mensajederror = ERR.Message;
            }
            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            _writer.PageEvent = new ITextEventsCotizapros(); // invoca la clase que esta mas abajo correspondiente al pie de pagina
            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();
            //Abrimos el documento
            _documento.Open();
           AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor("");
           AgregarDatosFactura();
            AgregarDatosReceptorEmisor();
            AgregarDatosProductos();
            AgregarTotales();
            Agregarpie();
            //     AgregarSellos();
            //Cerramoe el documento
            _documento.Close();
            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"" + _templatePDF.prospecto +  _templatePDF.folio + ".pdf" + "\"");
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
            tablapie.AddCell(new Phrase("LOS PRECIOS ASENTADOS SON ANTES DE IVA, Y ESTAN SUJETOS A CAMBIOS S.P.A\nVIGENCIA DE LA COTIZACION 10 DIAS NATURALES\nLOS PRECIOS ASENTADOS SON POR CORRIDA SIN CAMBIOS.\n LAS PRODUCCIONES PUEDEN TENER UNA VARIACION DE 10%+/- DE LA CANTIDAD SOLICITADA.\n LOS PRECIOS L.A.B MEXICO, D.F. Y AREA METROPOLITANA.", new Font(Font.FontFamily.HELVETICA, 6)));
            //tablapie.AddCell(new Phrase("Horario de entrega de Lunes a Viernes de 8:00 am a 5:30 pm.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            //tablapie.AddCell(new Phrase("SE RECIBIRA UNICA Y EXCLUSIVAMENTE LA CANTIDAD EXPUESTA EN ESTE PEDIDO, POR LO QUE NO SE ADMITIRA FALTANTE O SOBRANTE ALGUNO.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablapie.AddCell(new Phrase("Vigencia de la cotización: " + _templatePDF.Vigencia +" Dias", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //tablapie.AddCell(new Phrase("El proveedor se obliga a vender y entregar los productos y/o en proporcionar los servicios especificados en este documento, de acuerdo con los términos, condiciones y cláusulas del Pedido, y de sus cláusulas especiales, modificaciones o complementos.Todo lo anterior, constituye un acuerdo final y completo entre las partes.Los términos, condiciones y cláusulas de éste Pedido se encuentran en este documento.Las obligaciones del comprador quedan expresamente limitadas en los términos, condiciones y clausulas aquí contenidas.Cualquier otro término, condición o cláusula que proponga el Proveedor no será valida a menos que sea aceptada por escrito por el Comprador.Nos reservamos el derecho a realizar Auditoria en las instalaciones del Proveedor.\n\nDocumentos para recepcion indispensables\n-Factura original y copia\n-Pedido\n-Certificado de Calidad", new Font(Font.FontFamily.HELVETICA, 6)));

            float[] anchoColumasTablaFirmas = { 500f, 100f };
            PdfPTable tablafirmas = new PdfPTable(anchoColumasTablaFirmas);
            tablafirmas.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.DefaultCell.Border = Rectangle.NO_BORDER;
            tablafirmas.SetTotalWidth(anchoColumasTablaFirmas);
            tablafirmas.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.LockedWidth = true;



            tablafirmas.AddCell(new Phrase("\nVendedor", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            //tablafirmas.AddCell(new Phrase("Firma de conformidad", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            StringBuilder codigoQR = new StringBuilder();
            codigoQR.Append("http://" + SIAAPI.Properties.Settings.Default.Nombredelaaplicacion + "/cotizapros/Details/" + _templatePDF.folio);


            BarcodeQRCode pdfCodigoQR = new BarcodeQRCode(codigoQR.ToString(), 1, 1, null);
            Image img = pdfCodigoQR.GetImage();
            img.SpacingAfter = 0.0f;
            img.SpacingBefore = 0.0f;
            img.BorderWidth = 1.0f;
            //img.ScalePercent(100, 78);
            //img.border


            tablafirmas.AddCell(img);

            tablafirmas.AddCell(new Phrase("__________________________________", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            //tablafirmas.AddCell(new Phrase(_templatePDF.firmadefinanzas, new Font(Font.FontFamily.HELVETICA, 6)));
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
            try
            {
                Image jpg = iTextSharp.text.Image.GetInstance(logoEmpresa, System.Drawing.Imaging.ImageFormat.Jpeg);
                //using (Image image = Image.FromStream(new MemoryStream(bitmap)))
                //{
                //    image.Save("output.jpg", ImageFormat.Jpeg);  // Or Png
                //}

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(140f, 140F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(30f, 750f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
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
            _cb.Rectangle(428, 704, 20, 80);
            _cb.FillStroke();
        }
        private void AgregarDatosEmisor(String Telefono)
        {
            //Datos del emisor


            EmpresaContext dbe = new EmpresaContext();
            Empresa empresa = dbe.empresas.Find(2);

            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 155f;
            p1.IndentationRight = 150f;

            p1.Leading = 9;
            p1.Alignment = Element.ALIGN_CENTER;
            p1.Add(new Phrase(empresa.RazonSocial, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase(empresa.Calle +" "+empresa.NoExt +" "+ empresa.NoInt, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            p1.Add(new Phrase(empresa.Colonia +","+empresa.Municipio+","+empresa.Estados.Estado , new Font(Font.FontFamily.HELVETICA, 7)));
          
            p1.Add("\n");
            p1.Add(new Phrase("55 2620 4200, 55 2620 4362 ,55 2620 4199. ", new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add(new Phrase("\n\n"+SIAAPI.Properties.Settings.Default.paginaoficial, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
        
            p1.Add(new Phrase("\n"+SIAAPI.Properties.Settings.Default.Eslogan, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.SpacingAfter = -50;

            _documento.Add(p1);
        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Cotizacion Prospecto");
            _documento.AddSubject("Cotizacion Prospecto");
            _documento.AddTitle("Cotizacion Prospecto");
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

            p2.Add(new Phrase("COTIZACION P NÚM: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase("\n" +  _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 16)));

            p2.Add(new Phrase("\nFECHA " + _templatePDF.fecha, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

            p2.Add("\n");
           
            p2.SpacingAfter = 35;
            _documento.Add(p2);
        }

        private void AgregarDatosReceptorEmisor()
        {
            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SpacingBefore = 15;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnaspro = { 80f, 150F, 20f, 80f, 120f };
            PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
            tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapro.SetTotalWidth(anchoColumnaspro);
            tablapro.HorizontalAlignment = Element.ALIGN_LEFT;
            tablapro.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("CLIENTE: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celda1 = new PdfPCell(new Phrase(_templatePDF.prospecto.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda2 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda3 = new PdfPCell(new Phrase("ATENCION: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celda4 = new PdfPCell(new Phrase(_templatePDF.Atencion.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
           





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

            float[] anchoColumnas = { 200f, 200f, 200f };
            PdfPTable tabla = new PdfPTable(anchoColumnas);
            tabla.DefaultCell.Border = Rectangle.BOX;
            tabla.SetTotalWidth(anchoColumnas);
            tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.LockedWidth = true;



            PdfPCell celdaMO = new PdfPCell(new Phrase("MONEDA ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaUS = new PdfPCell(new Phrase("VENDEDOR".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaFO = new PdfPCell(new Phrase("CONDICIONES DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
           
           
          




            celdaMO.BackgroundColor = colordefinido;
            celdaUS.BackgroundColor = colordefinido;
            celdaFO.BackgroundColor = colordefinido;
           
         
            tabla.AddCell(celdaMO);
            tabla.AddCell(celdaUS);
            tabla.AddCell(celdaFO);
           
            tabla.CompleteRow();


            tabla.AddCell(new Phrase(_templatePDF.Moneda, new Font(Font.FontFamily.HELVETICA, 10)));


            tabla.AddCell(new Phrase(_templatePDF.Vendedor, new Font(Font.FontFamily.HELVETICA, 10)));





            tabla.AddCell(new Phrase(_templatePDF.condiciones, new Font(Font.FontFamily.HELVETICA, 10)));



        

           
            
            tabla.CompleteRow();

            ////tablaDatosPrincipal.AddCell(tabla);
            //////
            ////_documento.Add(tablaDatosPrincipal);


            float[] anchoColumnasobserva = { 80f, 520f };
            PdfPTable tablaobservacion = new PdfPTable(anchoColumnasobserva);
            tablaobservacion.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaobservacion.SetTotalWidth(anchoColumnasobserva);
            tablaobservacion.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaobservacion.LockedWidth = true;
           
            PdfPCell celdaob = new PdfPCell(new Phrase("OBSERVACIÓN :".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            celdaob.Border = Rectangle.NO_BORDER;
            
            tablaobservacion.AddCell(celdaob);

            tablaobservacion.AddCell(new Phrase(_templatePDF.Observacion, new Font(Font.FontFamily.HELVETICA, 8)));

            tablaobservacion.CompleteRow();




            tablaDatosPrincipal.AddCell(tabla);
            //tablaDatosPrincipal.AddCell(tabla1);
            tablaDatosPrincipal.AddCell(tablaobservacion);
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
            float[] tamanoColumnas = { 60f, 60f, 335f,  70f, 75f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;



           

            PdfPCell celda1 = new PdfPCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda2 = new PdfPCell(new Phrase("UNIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda3 = new PdfPCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda4 = new PdfPCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda5 = new PdfPCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
          



       
            celda1.BackgroundColor = colordefinido;
            celda2.BackgroundColor = colordefinido;
            celda3.BackgroundColor = colordefinido;
            celda4.BackgroundColor = colordefinido;
            celda5.BackgroundColor = colordefinido;
         
         
            tablaProductosTitulos.AddCell(celda1);
            tablaProductosTitulos.AddCell(celda2);
            tablaProductosTitulos.AddCell(celda3);
            tablaProductosTitulos.AddCell(celda4);
            tablaProductosTitulos.AddCell(celda5);
           





            float[] tamanoColumnasProductos = { 60f, 60f, 335f, 70f, 75f };
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






            var result2 = from line in _templatePDF.productos
                          group line by new { line.descripcion, line.Presentacion, line.Observacion } into csLine
                          select new LineadeproductosCoPr
                          {
                          
                              Cantidad = csLine.Sum(c => decimal.Parse(c.cantidad)),
                              Unidad= csLine.First().Unidad,
                              Descripcion = csLine.First().descripcion,
                              v_unitario = decimal.Parse(csLine.First().valorUnitario.ToString()),
                              importe = csLine.Sum(c => Math.Round(decimal.Parse(c.importe.ToString()), 2)),
                            
                          };


            foreach (LineadeproductosCoPr p in result2)
            {
                

                tablaProductos.AddCell(new Phrase(p.Cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Unidad, new Font(Font.FontFamily.HELVETICA, 8)));

                tablaProductos.AddCell(new Phrase(p.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdauni.Phrase = new Phrase(p.v_unitario.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);
              
              
             

                tablaProductos.CompleteRow();
               



                


            }


            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
            tablaProductosPrincipal.AddCell(celdaTitulos);
            PdfPCell celdaProductos = new PdfPCell(tablaProductos);
            celdaProductos.MinimumHeight = 280;
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
         

            tablaImportes.AddCell(new Phrase("SUBTOTAL:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase(_templatePDF.subtotal.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

          
                tablaImportes.AddCell(new Phrase( "IVA " , new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(_templatePDF.iva.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
           

            tablaImportes.AddCell(new Phrase("Total:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase(_templatePDF.total.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));


            tablaTotales.AddCell(new Phrase("IMPORTE CON LETRA: " + _templatePDF.totalEnLetra.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            tablaTotales.AddCell(tablaImportes);

            tablaTotales.AddCell(new Phrase("Fecha y hora de impresion: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString(), new Font(Font.FontFamily.HELVETICA, 8)));
            tablaTotales.AddCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8)));
            //tablaTotales.AddCell(tablaImportes);



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



     


        private void ObtenerLetras()
        {


            Numalet numaLet = new Numalet();
            numaLet.MascaraSalidaDecimal = "00/100 M.N.";
            numaLet.SeparadorDecimalSalida = "pesos";
            numaLet.ApocoparUnoParteEntera = true;
            numaLet.LetraCapital = true;




            if (_templatePDF.Moneda == "Peso Mexicano")
            {
                numaLet.SeparadorDecimalSalida = "pesos";
            }
            if (_templatePDF.Moneda == "Dolar americano")
            {
                numaLet.SeparadorDecimalSalida = "dolares";
                numaLet.MascaraSalidaDecimal = "00/100";
            }
            if (_templatePDF.Moneda == "Euro")
            {
                numaLet.SeparadorDecimalSalida = "Euros";
                numaLet.MascaraSalidaDecimal = "00/100";
            }




            _templatePDF.totalEnLetra = numaLet.ToCustomString(double.Parse(_templatePDF.total.ToString()));


        }
        #endregion

    }


    public class LineadeproductosCoPr
    {

        public LineadeproductosCoPr() { }

      
        public decimal Cantidad { get; set; }
      public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public decimal v_unitario { get; set; }
        public decimal importe { get; set; }
       
    }




    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsCotizapros : PdfPageEventHelper
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
            String TextRevision = "REV. 1";

            cb.BeginText();
            cb.SetFontAndSize(bf, 9);
            cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
            //cb.MoveText(500,30);
            //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
            //cb.ShowText(text);
            cb.EndText();
            float len = bf.GetWidthPoint(text, 9);
            cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

            float[] anchoColumasTablaTotales = { 480f, 120f };
            PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumasTablaTotales);
            tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.LockedWidth = true;
            tabla.AddCell(new Phrase("Este documento es una cotización para prospecto ", new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

        }



        //}

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



}
