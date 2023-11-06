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
    public class ProductoEm
    {

        public decimal cantidad { get; set; }
        public string observacion = string.Empty;
        public string lote = string.Empty;
        public string loteMP = string.Empty;
        public string serie = string.Empty;
        public int iddetempaque = 0;
        

        public string Observacion = string.Empty;
        //internal object iddetempaque;

        public string pedimento { get; internal set; }
        public decimal cantEmp { get; set; }
        public int idorden { get; set; }
        public string status { get; internal set; }
        public int cajas { get; set; }
        public int paquetes { get; set; }

        public int NP { get; set; }

        public string cref { get; set; }

        public string numIdentificacion { get; set; }
    }

   

    public class DocumentoEm
    {
        public string serie = string.Empty;
        public string folio = string.Empty;

        public string fecha = string.Empty;

        public string estado = string.Empty;

        public string lugarEntrega = string.Empty;
        public int version { get; set; }
        public string metodoPago = string.Empty;
        public string claveMoneda = string.Empty;
        public string condicionesdepago = string.Empty;
        public string firmadefinanzas = string.Empty;
        public string firmadecompras = string.Empty;

        public string Empresa = string.Empty;
        public string Direccion = string.Empty;
        public string Telefono = string.Empty;
        public string RFC = string.Empty;


        public string Cliente = string.Empty;
        public string Vendedor = string.Empty;
        public bool facturacionExacta { get; set; }

        public string Observacion = string.Empty;

        public string cadenaOriginal = string.Empty;

        public float subtotal = 0.00f;
        public float total = 0.00f;
        public float descuento = 0.00f;

        public int IDEncpack = 0;
        public int IDPedido = 0;

        public List<ProductoEm> productos = new List<ProductoEm>();
      
    }

    public class CreaEmPDF
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public DocumentoEm _templatePDF; //Objeto que contendra la información del documento pdf
        XmlDocument xDoc; // Objeto para abrir el archivo xml
        CMYKColor colordefinido;
        CMYKColor colorfuente;

        public CreaEmPDF(System.Drawing.Image logo, DocumentoEm Empaque)
        {
            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;
            colorfuente = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
            _templatePDF = Empaque;
            //ObtenerLetras();
            //Trabajos con el documento XML
            _documento = new Document(PageSize.LETTER);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEventsEm(); // invoca la clase que esta mas abajo correspondiente al pie de pagina

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
            //AgregarTotales();

            //Agregarpie();

            //     AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"" + _templatePDF.Cliente + _templatePDF.serie + _templatePDF.folio + ".pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

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
            imagen.ScaleToFit(140, 100);
            imagen.Alignment = Element.ALIGN_TOP;
            Chunk logo = new Chunk(imagen, 1, -40);
            _documento.Add(logo);
        }

        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;


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
            p1.SpacingAfter = 20;
            p1.IndentationLeft = 150f;

            p1.Leading = 9;

            p1.Add(new Phrase(_templatePDF.Empresa, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.RFC, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Direccion, new Font(Font.FontFamily.HELVETICA, 8)));

            p1.Add("\n");
            p1.SpacingAfter = -70;
            p1.Add("\n");
            p1.Add("\n");
            p1.Add("\n");
            p1.Add("\n");
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
            _documento.AddKeywords("Solicitud Empaque");
            _documento.AddSubject("Solicitud Empaque");
            _documento.AddTitle("SOLICITID EMPAQUE");
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

            p2.Add(new Phrase("SOLICITUD NÚM: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase("\n" + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 16)));

            p2.Add(new Phrase("\nFECHA " + _templatePDF.fecha, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));


            p2.Add(new Phrase("\nCODIGO: FSG-27 ", new Font(Font.FontFamily.HELVETICA, 8)));

            p2.SpacingAfter = 20;
            _documento.Add(p2);
        }

        private void AgregarDatosReceptorEmisor()
        {
            EncPack empaque = new EncPackContext().EncPackaging.Find(_templatePDF.IDEncpack);
            EncPedido pedido = new PedidoContext().EncPedidos.Find(empaque.idPedido);
            Vendedor vendedor = new VendedorContext().Vendedores.Find(pedido.IDVendedor);


            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SpacingBefore = 15;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnaspro = { 300f,300f };
            PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
            tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapro.SetTotalWidth(anchoColumnaspro);
            tablapro.HorizontalAlignment = Element.ALIGN_LEFT;
            tablapro.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("CLIENTE: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
            celda0.BackgroundColor = colordefinido;
            PdfPCell celda3 = new PdfPCell(new Phrase("VENDEDOR: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
            celda3.BackgroundColor = colordefinido;
            

            PdfPCell celda1 = new PdfPCell(new Phrase(empaque.Cliente , new Font(Font.FontFamily.HELVETICA, 8)));


            PdfPCell celda4 = new PdfPCell(new Phrase( vendedor.Nombre, new Font(Font.FontFamily.HELVETICA, 8)));
            


            tablapro.AddCell(celda0);
            tablapro.AddCell(celda3);
            

            tablapro.AddCell(celda1);
            tablapro.AddCell(celda4);
            
            //tablapro.AddCell(celda5);
            //tablapro.AddCell(celda6);

            tablapro.CompleteRow();
            _documento.Add(tablapro);

            float[] anchoColumnas = { 300f,300f };
            PdfPTable tabla = new PdfPTable(anchoColumnas);
            tabla.DefaultCell.Border = Rectangle.BOX;
            tabla.SetTotalWidth(anchoColumnas);
            tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.LockedWidth = true;



            PdfPCell celdaA = new PdfPCell(new Phrase("EMPACAR EN: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
            PdfPCell celdaAA = new PdfPCell(new Phrase("LUGAR DE ENTREGA:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8 , Font.BOLD, colorfuente)));
            




            celdaA.BackgroundColor = colordefinido;
            celdaAA.BackgroundColor = colordefinido;
            
            tabla.AddCell(celdaA);
            tabla.AddCell(celdaAA);
            
            tabla.CompleteRow();


            tabla.AddCell(new Phrase("Versión "+_templatePDF.version.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase(_templatePDF.lugarEntrega, new Font(Font.FontFamily.HELVETICA, 8)));


            tabla.CompleteRow();


            float[] anchoColumnasOB = { 600f };
            PdfPTable tablaOB = new PdfPTable(anchoColumnasOB);
            tablaOB.DefaultCell.Border = Rectangle.BOX;
            tablaOB.SetTotalWidth(anchoColumnasOB);
            tablaOB.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaOB.LockedWidth = true;



            PdfPCell celdaB = new PdfPCell(new Phrase("OBSERVACIONES: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
           
            celdaB.BackgroundColor = colordefinido;

            tablaOB.AddCell(celdaB);


            tablaOB.CompleteRow();

            tablaOB.AddCell(new Phrase(_templatePDF.Observacion, new Font(Font.FontFamily.HELVETICA, 8)));
           
            tablaOB.CompleteRow();


            float[] anchoColumnasfac = { 600f };
            PdfPTable tablafac = new PdfPTable(anchoColumnasfac);
            tablafac.DefaultCell.Border = Rectangle.BOX;
            tablafac.SetTotalWidth(anchoColumnasfac);
            tablafac.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafac.LockedWidth = true;



            PdfPCell celdaC = new PdfPCell(new Phrase("FACTURACIÓN: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorfuente)));
            

            celdaC.BackgroundColor = colordefinido;


            tablafac.AddCell(celdaC);


            tablafac.CompleteRow();

            if (_templatePDF.facturacionExacta)
            {
                tablafac.AddCell(new Phrase("El cliente solicita facturación exacta a su pedido", new Font(Font.FontFamily.HELVETICA, 8)));
            }
    else
    {
                tablafac.AddCell(new Phrase("El cliente acepta margen adicional de fabricacion a su pedido", new Font(Font.FontFamily.HELVETICA, 8)));
            }

            tablafac.CompleteRow();


            float[] anchoColumnasobserva = { 80f, 520f };
            PdfPTable tablaobservacion = new PdfPTable(anchoColumnasobserva);
            tablaobservacion.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaobservacion.SetTotalWidth(anchoColumnasobserva);
            tablaobservacion.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaobservacion.LockedWidth = true;





            tablaDatosPrincipal.AddCell(tabla);
            tablaDatosPrincipal.AddCell(tablaOB);
            tablaDatosPrincipal.AddCell(tablafac);

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
            float[] tamanoColumnas = { 55f, 20f, 30f, 50f, 50f, 50f, 70f, 30f,35f, 70F, 50f, 40f, 50f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("CREF", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda1 = new PdfPCell(new Phrase("NP", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda2 = new PdfPCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda3 = new PdfPCell(new Phrase("LOTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda4 = new PdfPCell(new Phrase("LOTE_MP", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda5 = new PdfPCell(new Phrase("SERIE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda6 = new PdfPCell(new Phrase("PEDIMENTO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda7 = new PdfPCell(new Phrase("CANT_EMP", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda8 = new PdfPCell(new Phrase("ID_ORDEN", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda9 = new PdfPCell(new Phrase("OBSERVACIÓN", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda10 = new PdfPCell(new Phrase("ESTADO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda11 = new PdfPCell(new Phrase("CAJAS", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda12 = new PdfPCell(new Phrase("PAQUETES", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            



            celda0.BackgroundColor = colordefinido;
            celda1.BackgroundColor = colordefinido;
            celda2.BackgroundColor = colordefinido;
            celda3.BackgroundColor = colordefinido;
            celda4.BackgroundColor = colordefinido;
            celda5.BackgroundColor = colordefinido;
            celda6.BackgroundColor = colordefinido;
            celda7.BackgroundColor = colordefinido;
            celda8.BackgroundColor = colordefinido;
            celda9.BackgroundColor = colordefinido;
            celda10.BackgroundColor = colordefinido;
            celda11.BackgroundColor = colordefinido;
            celda12.BackgroundColor = colordefinido;

            tablaProductosTitulos.AddCell(celda0);
            tablaProductosTitulos.AddCell(celda1);
            tablaProductosTitulos.AddCell(celda2);
            tablaProductosTitulos.AddCell(celda3);
            tablaProductosTitulos.AddCell(celda4);
            tablaProductosTitulos.AddCell(celda5);
            tablaProductosTitulos.AddCell(celda6);
            tablaProductosTitulos.AddCell(celda7);
            tablaProductosTitulos.AddCell(celda8);
            tablaProductosTitulos.AddCell(celda9);
            tablaProductosTitulos.AddCell(celda10);
            tablaProductosTitulos.AddCell(celda11);
            tablaProductosTitulos.AddCell(celda12);





            float[] tamanoColumnasProductos = { 55f, 20f, 30f, 50f, 50f, 50f, 70f, 30f, 35f, 70f, 50f, 40f, 50f };
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
                          group line by new { line.iddetempaque, line.cref } into csLine
                          select new LineadeproductosEm
                          {
                              iddetempaque= int.Parse(csLine.First().iddetempaque.ToString()),
                              Cref = csLine.First().cref,
                              NP = csLine.First().NP,
                              cantidad = decimal.Parse(csLine.First().cantidad.ToString()),
                              lote = csLine.First().lote,
                              lotemp = csLine.First().loteMP,
                              serie = csLine.First().serie,
                              pedimento = csLine.First().pedimento,
                              cantemp = decimal.Parse(csLine.First().cantEmp.ToString()),
                              idorden = int.Parse(csLine.First().idorden.ToString()),
                              observacion=csLine.First().observacion,
                              estado=csLine.First().status,
                              cajas= int.Parse(csLine.First().cajas.ToString()),
                              paquetes= int.Parse(csLine.First().paquetes.ToString())

                          };


            foreach (LineadeproductosEm p in result2)
            {
                tablaProductos.AddCell(new Phrase(p.Cref, new Font(Font.FontFamily.HELVETICA, 8)));

                tablaProductos.AddCell(new Phrase(p.NP.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.lote, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdauni.Phrase = new Phrase(p.lotemp, new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdaimporte.Phrase = new Phrase(p.serie, new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);
                PdfPCell celdadescuento = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdadescuento.Phrase = new Phrase(p.pedimento, new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdadescuento);
                tablaProductos.AddCell(new Phrase(p.cantemp.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.idorden.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.observacion, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.estado, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.cajas.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.paquetes.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
               

            }


            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
            tablaProductosPrincipal.AddCell(celdaTitulos);
            PdfPCell celdaProductos = new PdfPCell(tablaProductos);
            celdaProductos.MinimumHeight = 300;
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


    public class LineadeproductosEm
    {

        public LineadeproductosEm() { }

        public int iddetempaque { get; set; }
        public string Cref { get; set; }
        public int NP { get; set; }
        public decimal cantidad { get; set; }
        public string Descripcion { get; set; }
        public string lote { get; set; }
        public string lotemp { get; set; }
        public string serie { get; set; }
        public string pedimento { get; set; }
        public decimal cantemp { get; set; }
        public int idorden { get; set; }
        public string observacion { get; set; }
        public string estado { get; set; }
        public int cajas { get; set; }
        public int paquetes { get; set; }
    }




    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsEm : PdfPageEventHelper
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
            tabla.AddCell(new Phrase("Este documento es una Solicitud de Empaque ", new Font(Font.FontFamily.HELVETICA, 8)));
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
