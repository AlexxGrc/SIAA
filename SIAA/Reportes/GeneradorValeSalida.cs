using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace SIAAPI.Reportes
{

    public class ProductoVale
    {
        public int iddetValeSalida { get; set; }
        public string cantidad { get; set; }
        public string descripcion = string.Empty;
        public string unidad = string.Empty;
        public float valorUnitario = 0.00f;
        public float importe = 0.00f;
        public string numIdentificacion = string.Empty;
        public int idarticulo { get; set; }
        public string Presentacion = string.Empty;

        public string Observacion = string.Empty;
        public string Nota = string.Empty;
        public int OProduccion = 0;
        public string ClaveProducto { get; internal set; }
        public string id { get; internal set; }
        public string c_unidad { get; internal set; }
        public string desc { get; internal set; }
        public float v_unitario = 0.00f;
        public string almacen { get; set; }
        public string Moneda { get; internal set; }
    }

  
  

    public class DocumentoVale
    {
        //public string serie = string.Empty;
        public string folio = string.Empty;

        public string fecha = string.Empty;

        public string Solicito = string.Empty;
        public string Entregado = string.Empty;
        public string Concepto = string.Empty;
       

       
        public string Observacion = string.Empty;

       
        public int IDValeSalida = 0;

        public string Empresa = string.Empty;
        public string Direccion = string.Empty;
        public string Telefono = string.Empty;
        public string RFC = string.Empty;
        public List<ProductoVale> productos = new List<ProductoVale>();
       
        
    }

    public class CreaValePDF
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public CMYKColor colordefinido;
        public DocumentoVale _templatePDF; //Objeto que contendra la información del documento pdf
        XmlDocument xDoc; // Objeto para abrir el archivo xml

        public string nombreDocumento = string.Empty;
        public CreaValePDF(System.Drawing.Image logo, DocumentoVale PE)
        {
            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;
            _templatePDF = PE;
            //ObtenerLetras();
            //Trabajos con el documento XML
            _documento = new Document(PageSize.LETTER);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEventsVale(); // invoca la clase que esta mas abajo correspondiente al pie de pagina

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();

            for (int con = 0; con < 3; con++)
            {

                //AgregarLogo(logo);
                //AgregarCuadro();
                AgregarDatosEmisor(logo,"");
                //AgregarDatosFactura();

                AgregarDatosReceptorEmisor();

                AgregarDatosProductos();

                Agregarpie();
            }
            //AgregarLogo(logo);
            //AgregarCuadro();
            //AgregarDatosEmisor("");
            //AgregarDatosFactura();

            //AgregarDatosReceptorEmisor();

            //AgregarDatosProductos();
          
            //Agregarpie();

            //     AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"" + _templatePDF.folio + ".pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

        }
        private void Agregarpie()
        {
            float[] anchoColumasTablapie = { 600f };
            PdfPTable tablapie = new PdfPTable(anchoColumasTablapie);
            tablapie.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapie.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapie.SetTotalWidth(anchoColumasTablapie);
            tablapie.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapie.LockedWidth = true;

            //tablapie.AddCell(new Phrase("Horario de entrega de Lunes a Viernes de 8:00 am a 5:30 pm.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            //tablapie.AddCell(new Phrase("SE RECIBIRA UNICA Y EXCLUSIVAMENTE LA CANTIDAD EXPUESTA EN ESTE PEDIDO, POR LO QUE NO SE ADMITIRA FALTANTE O SOBRANTE ALGUNO.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            //tablapie.AddCell(new Phrase("Condiciones de Pago: " + _templatePDF.condicionesdepago, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //tablapie.AddCell(new Phrase("El proveedor se obliga a vender y entregar los productos y/o en proporcionar los servicios especificados en este documento, de acuerdo con los términos, condiciones y cláusulas del Pedido, y de sus cláusulas especiales, modificaciones o complementos.Todo lo anterior, constituye un acuerdo final y completo entre las partes.Los términos, condiciones y cláusulas de éste Pedido se encuentran en este documento.Las obligaciones del comprador quedan expresamente limitadas en los términos, condiciones y clausulas aquí contenidas.Cualquier otro término, condición o cláusula que proponga el Proveedor no será valida a menos que sea aceptada por escrito por el Comprador.Nos reservamos el derecho a realizar Auditoria en las instalaciones del Proveedor.\n\nDocumentos para recepcion indispensables\n-Factura original y copia\n-Pedido\n-Certificado de Calidad", new Font(Font.FontFamily.HELVETICA, 6)));

            float[] anchoColumasTablaFirmas = { 150F, 150F, 150F, 150F };
            PdfPTable tablafirmas = new PdfPTable(anchoColumasTablaFirmas);
            tablafirmas.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.DefaultCell.Border = Rectangle.NO_BORDER;
            tablafirmas.SetTotalWidth(anchoColumasTablaFirmas);
            tablafirmas.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.LockedWidth = true;


            tablafirmas.AddCell(new Phrase("\n\n\n" + _templatePDF.Solicito, new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));


            ClsDatoEntero idusuario = new ArticuloContext().Database.SqlQuery<ClsDatoEntero>("select Usuario as Dato from movimientoarticulo where Accion='Vale de salida' and noDocumento=" + _templatePDF.IDValeSalida).ToList().FirstOrDefault();
            string nombreAutoriza = "";
            try
            {
                User usuario = new UserContext().Users.Find(idusuario.Dato);
                nombreAutoriza = usuario.Username;

            }
            catch (Exception err)
            {

            }
            
            tablafirmas.AddCell(new Phrase("\n\n\n" + nombreAutoriza, new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));



            tablafirmas.AddCell(new Phrase("\n__________________________________\n\n Solicita", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));

            tablafirmas.AddCell(new Phrase("\n__________________________________\n\n Autorizo", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablafirmas.AddCell(new Phrase("\n__________________________________\n\n Chofer", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablafirmas.AddCell(new Phrase("\n__________________________________\n\n Recibí Mercancia", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablapie.AddCell(new Phrase("\n\nEste documento solo es valido si es obtenido de la misma maestra \n\n\n", new Font(Font.FontFamily.HELVETICA, 8)));


            _documento.Add(tablafirmas);
            _documento.Add(tablapie);

        }





        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            try
            {
                Image jpg = iTextSharp.text.Image.GetInstance(logoEmpresa, System.Drawing.Imaging.ImageFormat.Jpeg);
              
                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(140f, 130F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(20f, 720f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                _documento.Add(jpg);
                //  doc.Add(paragraph);
            }
            catch (Exception err)
            {

            }
        }

        private void AgregarDatosEmisor(System.Drawing.Image logoEmpresa, String Telefono)
        {
            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SpacingBefore = 15;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnaspro = { 200f,200f,200f };
            PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
            tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapro.SetTotalWidth(anchoColumnaspro);
            tablapro.HorizontalAlignment = Element.ALIGN_LEFT;
            tablapro.LockedWidth = true;

            if (logoEmpresa == null)
                return;
            try
            {
                Image jpg = iTextSharp.text.Image.GetInstance(logoEmpresa, System.Drawing.Imaging.ImageFormat.Jpeg);

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(140f, 130F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(20f, 720f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                                                    //_documento.Add(jpg);
                                                    //  doc.Add(paragraph);
                tablapro.AddCell(jpg);
            }
            catch (Exception err)
            {
                tablapro.AddCell("");
            }




            PdfPCell celda0 = new PdfPCell(new Phrase(_templatePDF.Empresa + "\n"+ _templatePDF.RFC + " \n" + _templatePDF.Telefono+ "\n" + _templatePDF.Direccion, new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            celda0.Border = Rectangle.NO_BORDER;
            celda0.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapro.AddCell(celda0);
            PdfPCell celda1 = new PdfPCell(new Phrase("Folio: \n"+ _templatePDF.folio+ "\nFECHA " + _templatePDF.fecha+ "\nCÓDIGO: FSG-25 ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            celda1.Border = Rectangle.NO_BORDER;
            celda1.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapro.AddCell(celda1);

            
           
            //Datos del emisor
            //Paragraph p1 = new Paragraph();
            //p1.IndentationLeft = 155f;
            //p1.IndentationRight = 150f;
            //p1.SpacingBefore = 20f;
            //p1.Leading = 9;
            //p1.Alignment = Element.ALIGN_CENTER;
            //p1.Add(new Phrase(_templatePDF.Empresa, new Font(Font.FontFamily.HELVETICA, 8)));
            //p1.Add("\n");
            //p1.Add(new Phrase("" + _templatePDF.RFC, new Font(Font.FontFamily.HELVETICA, 8)));
            //p1.Add("\n");
            //p1.Add(new Phrase("" + _templatePDF.Telefono, new Font(Font.FontFamily.HELVETICA, 6)));
            //p1.Add("\n");
            //p1.Add(new Phrase("" + _templatePDF.Direccion, new Font(Font.FontFamily.HELVETICA, 6)));

            //p1.Add("\n");
            //p1.Add("\n");
            //p1.Add(new Phrase(SIAAPI.Properties.Settings.Default.paginaoficial, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
            //p1.Add("\n");
            //p1.Add(new Phrase(SIAAPI.Properties.Settings.Default.Eslogan, new Font(Font.FontFamily.HELVETICA, 6)));
            //p1.SpacingAfter = -80;

            _documento.Add(tablapro);
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
       
        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Vale");
            _documento.AddSubject("Vale");
            _documento.AddTitle("VALE");
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

            p2.Add(new Phrase("Folio: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase("\n" + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 16)));

            p2.Add(new Phrase("\nFECHA " + _templatePDF.fecha, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

            p2.Add("\n");
           
            p2.Add(new Phrase("\nCÓDIGO: FSG-25 ", new Font(Font.FontFamily.HELVETICA, 8)));

            p2.SpacingAfter = 30;
            _documento.Add(p2);
        }

        private void AgregarDatosReceptorEmisor()
        {
            float[] anchoColumasTablapie = { 600f };
            PdfPTable tablapie = new PdfPTable(anchoColumasTablapie);
            tablapie.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapie.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapie.SetTotalWidth(anchoColumasTablapie);
            tablapie.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapie.LockedWidth = true;
            tablapie.AddCell(new Phrase("VALE DE SALIDA\n", new  Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));


            _documento.Add(tablapie);

            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            //tablaDatosPrincipal.SpacingBefore = 10;
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



            PdfPCell celda0 = new PdfPCell(new Phrase("Entregado a: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celda1 = new PdfPCell(new Phrase(_templatePDF.Entregado.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda2 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda3 = new PdfPCell(new Phrase("Concepto de Salida: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda4 = new PdfPCell(new Phrase(_templatePDF.Concepto, new Font(Font.FontFamily.HELVETICA, 8)));

            PdfPCell celda5 = new PdfPCell(new Phrase("Solicito: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celda6 = new PdfPCell(new Phrase(_templatePDF.Solicito.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda7 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda8 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda9 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));



            celda0.Border = Rectangle.NO_BORDER;
            celda1.Border = Rectangle.NO_BORDER;
            celda2.Border = Rectangle.NO_BORDER;
            celda3.Border = Rectangle.NO_BORDER;
            celda4.Border = Rectangle.NO_BORDER;
            celda5.Border = Rectangle.NO_BORDER;
            celda6.Border = Rectangle.NO_BORDER;
            celda7.Border = Rectangle.NO_BORDER;
            celda8.Border = Rectangle.NO_BORDER;
            celda9.Border = Rectangle.NO_BORDER;




            tablapro.AddCell(celda0);
            tablapro.AddCell(celda1);
            tablapro.AddCell(celda2);
            tablapro.AddCell(celda3);
            tablapro.AddCell(celda4);
            tablapro.AddCell(celda5);
            tablapro.AddCell(celda6);
            tablapro.AddCell(celda7);
            tablapro.AddCell(celda8);
            tablapro.AddCell(celda9);

            tablapro.CompleteRow();
            _documento.Add(tablapro);


            PdfPCell celda10 = new PdfPCell(new Phrase("Observación:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda11 = new PdfPCell(new Phrase(_templatePDF.Observacion, new Font(Font.FontFamily.HELVETICA, 8)));

            celda10.Border = Rectangle.NO_BORDER;
            celda11.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.AddCell(celda10);
            tablaDatosPrincipal.AddCell(celda11);

            _documento.Add(tablaDatosPrincipal);

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
            float[] anchoColumnas = { 70f, 50f, 60f, 50f, 230f, 65f, 75f };
            PdfPTable tabla = new PdfPTable(anchoColumnas);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumnas);
            tabla.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.LockedWidth = true;


            PdfPCell celdaMO = new PdfPCell(new Phrase("CÓDIGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaUS = new PdfPCell(new Phrase("NP".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaFO = new PdfPCell(new Phrase("Cantidad".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaME = new PdfPCell(new Phrase("Unidad".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaTI = new PdfPCell(new Phrase("Descripción".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaAL = new PdfPCell(new Phrase("Costo".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaFE = new PdfPCell(new Phrase("Importe".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));





            celdaMO.BackgroundColor = colordefinido;
            celdaUS.BackgroundColor = colordefinido;
            celdaFO.BackgroundColor = colordefinido;
            celdaME.BackgroundColor = colordefinido;
            celdaTI.BackgroundColor = colordefinido;
            celdaAL.BackgroundColor = colordefinido;
            celdaFE.BackgroundColor = colordefinido;
            tabla.AddCell(celdaMO);
            tabla.AddCell(celdaUS);
            tabla.AddCell(celdaFO);
            tabla.AddCell(celdaME);
            tabla.AddCell(celdaTI);
            tabla.AddCell(celdaAL);
            tabla.AddCell(celdaFE);





            float[] tamanoColumnasProductos = { 70f, 50f, 60f, 50f, 230f, 65f, 75f };
            PdfPTable tablaProductos = new PdfPTable(tamanoColumnasProductos);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductos.DefaultCell.BorderWidthLeft = 0.1f;
            tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
            tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
            tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
            tablaProductos.SetTotalWidth(tamanoColumnasProductos);
            //tablaProductos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductos.LockedWidth = true;






            var result2 = from line in _templatePDF.productos
                          group line by new { line.descripcion, line.Presentacion } into csLine
                          select new LineadeproductosVale
                          {
                              ClaveProducto = csLine.First().ClaveProducto,
                              Cantidad = csLine.Sum(c => decimal.Parse(c.cantidad)),
                              Unidad = csLine.First().c_unidad,
                              Descripcion = csLine.First().descripcion,
                              v_unitario = decimal.Parse(csLine.First().v_unitario.ToString()),
                              importe = csLine.Sum(c => Math.Round(decimal.Parse(c.importe.ToString()), 2)),
                              moneda = csLine.First().Moneda,
                              Presentacion = csLine.First().Presentacion,
                             
                          };


            foreach (LineadeproductosVale p in result2)
            {
                tablaProductos.AddCell(new Phrase(p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 6)));
                tablaProductos.AddCell(new Phrase(p.Presentacion, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Unidad, new Font(Font.FontFamily.HELVETICA, 6)));
                tablaProductos.AddCell(new Phrase(p.Descripcion, new Font(Font.FontFamily.HELVETICA, 5)));
                PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdauni.Phrase = new Phrase(p.v_unitario.ToString("C") + " "+ p.moneda, new Font(Font.FontFamily.HELVETICA, 6));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);

                //tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                //tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                //tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                //tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                //tablaProductos.AddCell(new Phrase( p.Presentacion, new Font(Font.FontFamily.HELVETICA, 6)));
                //tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                //tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));



            }


            PdfPCell celdaTitulos = new PdfPCell(tabla);
            tablaProductosPrincipal.AddCell(celdaTitulos);
            PdfPCell celdaProductos = new PdfPCell(tablaProductos);
            //celdaProductos.MinimumHeight = 280;
            tablaProductosPrincipal.AddCell(celdaProductos);
            _documento.Add(tablaProductosPrincipal);
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



  

      
        #endregion

    }


    public class LineadeproductosVale
    {

        public LineadeproductosVale() { }


        public int IDArticulo { get; set; }
        public string ClaveProducto { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public string Almacen { get; set; }
        public string Descripcion { get; set; }
        public decimal v_unitario { get; set; }
        public decimal importe { get; set; }
        //public decimal descuento { get; set; }
        public string Presentacion { get; set; }
        public string moneda { get; set; }

    }




    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsVale : PdfPageEventHelper
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
            //tabla.AddCell(new Phrase("Este documento solo es valido si es obtenido de la misma maestra ", new Font(Font.FontFamily.HELVETICA, 8)));
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
