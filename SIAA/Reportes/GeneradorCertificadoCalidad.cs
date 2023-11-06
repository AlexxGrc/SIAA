using Antlr.Runtime.Tree;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Calidad;
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


    public class DocumentoCe
    {
        public string lote = string.Empty;
        public string Etiqueta = string.Empty;
        public string fecha = string.Empty;
        public string Presentacion = string.Empty;
        public decimal CantidadLiberada = 0;
        public string Muestreo = string.Empty;
        public int Inspeccion = 0;
        public string Responsable = string.Empty;
        public int Orden = 0;
        public int Pedido = 0;
        public int Liberacion = 0;
        public string Empresa = string.Empty;
        public string Direccion = string.Empty;
        public string Telefono = string.Empty;
        public string RFC = string.Empty;
        public int IDArticulo = 0;
        public string Letra = string.Empty;
        public string PresentacionResultado = string.Empty;
        public string PresentacionFinalUnidad = string.Empty;
        public string MetodoResultado = string.Empty;
        public string PresentacionTintas = string.Empty;
        public string ClaveArticulo = string.Empty;

        public string Cliente = string.Empty;
        public string RFCCliente = string.Empty;
        public string TelefonoCliente = string.Empty;


        public List<ProductoRe> productos = new List<ProductoRe>();
        public List<ImpuestoRe> impuestos = new List<ImpuestoRe>();
        public List<RetencionRe> retenciones = new List<RetencionRe>();


    }

    public class CreaCertificadoPDF
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public DocumentoCe _templatePDF; //Objeto que contendra la información del documento pdf
        XmlDocument xDoc; // Objeto para abrir el archivo xml
        public CMYKColor colordefinido;
        public CMYKColor colorfuente;

        public CMYKColor colorencabezadodefinido;
        public String Embobinado = "A";
        public string nombreDocumento = string.Empty;
        public CreaCertificadoPDF(System.Drawing.Image logo, DocumentoCe Certificado)
        {


            _documento = new Document(PageSize.LETTER);

            //_documento = new Document(PageSize.LETTER.Rotate());
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Certificado de la Orden" + Certificado.Orden + ".pdf");

            //_documento.SetMargins(5, 5, 10, 50);

            try
            {
                if (File.Exists(nombreDocumento))
                {
                    nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Certificado de la Orden" + Certificado.Orden + "" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".pdf"); ;
                }

            }
            catch (Exception ERR)
            {
                string mensajederror = ERR.Message;
            }


            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));

            _writer.PageEvent = new ITextEventsCe(); // invoca la clase que esta mas abajo correspondiente al pie de pagina


            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;
            colorfuente = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
            _templatePDF = Certificado;
            //ObtenerLetras();
            //Trabajos con el documento XML
            //_documento = new Document(PageSize.LETTER);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            //Creamos el documento
            //_writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            //_writer.PageEvent = new ITextEventsCe(); // invoca la clase que esta mas abajo correspondiente al pie de pagina

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor("");
            AgregarDatosFactura();



            AgregarDatosProductos();
            //AgregarTotales();

            Agregarpie();

            //     AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"" + "Certificado Orden" + _templatePDF.Orden + ".pdf" + "\"");
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Write(_documento);
            //HttpContext.Current.Response.End();

        }

        private void Agregarpie()
        {


            float[] anchoColumasTablaFirmasF = { 50 };
            PdfPTable tablafirmasF = new PdfPTable(anchoColumasTablaFirmasF);
            tablafirmasF.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmasF.DefaultCell.Border = Rectangle.NO_BORDER;
            tablafirmasF.SetTotalWidth(anchoColumasTablaFirmasF);
            tablafirmasF.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmasF.LockedWidth = true;


            float[] anchoColumasTablaFirmas = { 130,90,160,90,130f };
            PdfPTable tablafirmas = new PdfPTable(anchoColumasTablaFirmas);
            tablafirmas.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.DefaultCell.Border = Rectangle.NO_BORDER;
            tablafirmas.SetTotalWidth(anchoColumasTablaFirmas);
            tablafirmas.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.LockedWidth = true;

            string FirmaCalidad = "";

            if (_templatePDF.Responsable == "RafaelC")
            {
                FirmaCalidad = "Firma Rafael Cedillo";
            }
            if (_templatePDF.Responsable == "UrielP")
            {
                FirmaCalidad = "Firma Uriel Perez";
            }
            if (_templatePDF.Responsable == "ClaudiaD")
            {
                FirmaCalidad = "Firma Claudia Dionicio";
            }
            if (_templatePDF.Responsable == "Norma A")
            {
                FirmaCalidad = "Firma Norma Guadarrama";
            }
            if (_templatePDF.Responsable == "eduardo")
            {
                FirmaCalidad = "Firma Eduardo Cristóbal Ortega Valencia";
            }
            if (_templatePDF.Responsable == "sendy")
            {
                FirmaCalidad = "Firma Sendey Vianey Torres Santos";
            }
            try
            {
                iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/" + FirmaCalidad + ".png"));
                imagen.BorderWidth = 0;
                imagen.Alignment = Element.ALIGN_RIGHT;
                float percentage = 0.0f;
                percentage = 150 / imagen.Width;
                // imagen.ScalePercent(percentage * 10);
                tablafirmasF.AddCell(imagen);
                tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablafirmas.AddCell(tablafirmasF);
            }
            catch (Exception err)
            {
                
            }

           
            tablafirmas.CompleteRow  ();
           

            //tablafirmas.AddCell(new Phrase(_templatePDF.firmadefinanzas, new Font(Font.FontFamily.HELVETICA, 8)));
            tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            string nombreCalidad = "";

            if (_templatePDF.Responsable == "RafaelC")
            {
                nombreCalidad = "Ing. Rafael Cedillo Flores";
            }
            if (_templatePDF.Responsable == "UrielP")
            {
                nombreCalidad = "Uriel Pérez García";
            }
            if (_templatePDF.Responsable == "ClaudiaD")
            {
                nombreCalidad = "Ing. Claudia Iveth Dionicio Álvarez";
            }
            if (_templatePDF.Responsable == "Norma A")
            {
                nombreCalidad = "TSU. Norma Angélica Guadarrama";
            }
            if (_templatePDF.Responsable == "eduardo")
            {
                nombreCalidad = "Eduardo Cristóbal Ortega Valencia";
            }
            if (_templatePDF.Responsable == "sendy")
            {
                nombreCalidad = "Sendey Vianey Torres Santos";
            }
            tablafirmas.AddCell(new Phrase(nombreCalidad+" \nInspector de Calidad\ncalidad@class-labels.com\n\n\n\n\n\n\n", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            tablafirmas.CompleteRow();




            _documento.Add(tablafirmas);

            float[] anchoColumasTabladirec = { 290,290f };
            PdfPTable tabladire = new PdfPTable(anchoColumasTabladirec);
            tabladire.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tabladire.DefaultCell.Border = Rectangle.NO_BORDER;
            tabladire.SetTotalWidth(anchoColumasTabladirec);
            tabladire.HorizontalAlignment = Element.ALIGN_CENTER;
            tabladire.LockedWidth = true;

            string Direccionmatri = "OFICINAS MEXICO\n3ra Sur No 39, Col. Independencia, Tultitlan de Escobedo, Edo de Mex. 54915\nTels. 5526204199/ 4200 / 4168, interior de la republica 018005618628\nwww.class-labels.com";
            PdfPCell direccionmatriz = new PdfPCell(new Phrase(Direccionmatri, new Font(Font.FontFamily.HELVETICA, 4, Font.BOLD)));
            direccionmatriz.BorderWidth = 0;
            direccionmatriz.HorizontalAlignment = Element.ALIGN_CENTER;
            tabladire.AddCell(direccionmatriz);
            string Direccionleon = "OFICINAS LEON\nAgua Marina No. 518, Fracc. Guadalupe, Leon Gto.\nTels. 477711 3083/ 1235 \nwww.class-labels.com";
            PdfPCell direccionl = new PdfPCell(new Phrase(Direccionleon, new Font(Font.FontFamily.HELVETICA,4 , Font.BOLD)));
            direccionl.BorderWidth = 0;
            direccionl.HorizontalAlignment = Element.ALIGN_CENTER;

            tabladire.AddCell(direccionl);


            _documento.Add(tabladire);
        }






        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            //Agrego la imagen al documento
            Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
            imagen.ScaleToFit(140, 100);
            imagen.Alignment = Element.ALIGN_TOP;
            Chunk logo = new Chunk(imagen, 1, -60);
            _documento.Add(logo);
        }

        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;

            //Agrego cuadro al documento
            _cb.SetColorStroke(colordefinido); //Color de la linea
            _cb.SetColorFill(colordefinido); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(406, 700, 20, 85);
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
            p1.SpacingAfter = -100;
            p1.Add("\n");
            p1.Add("\n");


            _documento.Add(p1);
        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Certificado");
            _documento.AddSubject("Certificado");
            _documento.AddTitle("CERTIFICADO");
            _documento.SetMargins(5, 5, 5, 5);
        }

        private void AgregarDatosFactura()
        {
            //Datos de la factura
            Paragraph p2 = new Paragraph();
            p2.IndentationLeft = 403f;
            p2.SpacingBefore = 40;
            p2.Leading = 10;
            p2.Alignment = Element.ALIGN_CENTER;



            p2.Add(new Phrase("\nFOLIO \n" + _templatePDF.Liberacion, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            p2.Add(new Phrase("\nFECHA " + _templatePDF.fecha, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

            p2.Add("\n");
            p2.Add(new Phrase("CÓDIGO: FSG-56", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLUE)));
            p2.Add(new Phrase("\nREV. 1", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

            p2.SpacingAfter = 10;
            _documento.Add(p2);
        }





        private void AgregarDatosProductos()
        {
            float[] anchoColumnasTablaProductos = { 600f };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);

            tablaProductosPrincipal.SpacingBefore = 25;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tablaProductosPrincipal.LockedWidth = true;


            tablaProductosPrincipal.AddCell(new Phrase("\n"+ _templatePDF.Empresa + " emite el siguiente certificado de calidad en base a lo establecido en la norma ISO 9001:2015 y al cumplimiento de la misma se determina que se cumple con los estándares de calidad para etiqueta blanca, fondeada o impresa.\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));
            tablaProductosPrincipal.AddCell(new Phrase("La certificación de nuestros productos está basada en la medición y evaluación del producto "+_templatePDF.ClaveArticulo+" "+_templatePDF.Etiqueta+" mediante las tablas ANSI que garantizan los requerimientos del cliente.\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));
            tablaProductosPrincipal.AddCell(new Phrase("El certificado es generado para "+_templatePDF.Cliente+" bajo nuestro Sistema de Gestion de Calidad.\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));

            float[] ancho2 = { 600f };
            PdfPTable tabla2 = new PdfPTable(ancho2);
            tabla2.DefaultCell.Border = Rectangle.NO_BORDER;

            tabla2.SetTotalWidth(ancho2);

            tabla2.SpacingBefore = 0;
            tabla2.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla2.LockedWidth = true;

            PdfPCell titulo = new PdfPCell(new Phrase("\nCERTIFICADO DE CALIDAD", new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, BaseColor.BLACK)));
            titulo.Border = PdfPCell.NO_BORDER;
            titulo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

            tabla2.AddCell(titulo);




            string[] arraydatos;
            arraydatos = _templatePDF.Presentacion.Split(',');
            string[] arraydatosMetodo;
            arraydatosMetodo = _templatePDF.MetodoResultado.Split(',');
            string[] arraydatosResultado;
            arraydatosResultado = _templatePDF.PresentacionResultado.Split(',');




            int cuantos = arraydatos.Length;

            string acc = "";
            string valor = "";
            string acc1 = "";
            string valor1 = "";
            string acc2 = "";
            string valor2 = "";
            string acc4 = "";
            string valor4 = "";
            float[] tamanoenc = new float[cuantos];
            for (int i = 0; i < arraydatos.Length; i++)
            {
                tamanoenc[i] = 50f;

            }
            float[] t1 = { 120f };

            float[] anchotabla = { 120f, 120f, 120f, 120f, 120f };
            PdfPTable tablaDatos = new PdfPTable(anchotabla);
            tablaDatos.DefaultCell.Border = Rectangle.NO_BORDER;

            tablaDatos.SetTotalWidth(anchotabla);

            tablaDatos.SpacingBefore = 15;
            tablaDatos.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tablaDatos.LockedWidth = true;
            PdfPTable tablaencpre = new PdfPTable(t1);
            PdfPTable tablaencpre1 = new PdfPTable(t1);
            PdfPTable tablaencpre2 = new PdfPTable(t1);
            PdfPTable tablaencpre3 = new PdfPTable(t1);
            PdfPTable tablaencpre4 = new PdfPTable(t1);

            tablaencpre.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaencpre1.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaencpre2.HorizontalAlignment = Element.ALIGN_CENTER;

            tablaencpre3.HorizontalAlignment = Element.ALIGN_CENTER; 
            tablaencpre4.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaencpre.SetTotalWidth(t1);
            tablaencpre.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre.LockedWidth = true;
            tablaencpre1.SetTotalWidth(t1);
            tablaencpre1.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre1.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre1.LockedWidth = true;
            tablaencpre2.SetTotalWidth(t1);
            tablaencpre2.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre2.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre2.LockedWidth = true;
            tablaencpre3.SetTotalWidth(t1);
            tablaencpre3.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre3.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre3.LockedWidth = true;
            tablaencpre4.SetTotalWidth(t1);
            tablaencpre4.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre4.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencpre4.LockedWidth = true;

            PdfPCell celdaprenc = new PdfPCell();
            PdfPCell celdavalor = new PdfPCell();
            PdfPCell celdaunidad = new PdfPCell();

            PdfPCell celdaresultado = new PdfPCell();
            PdfPCell celdametodo = new PdfPCell();

            celdaprenc = new PdfPCell(new Phrase("ATRIBUTO", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, BaseColor.WHITE)));
            celdaunidad = new PdfPCell(new Phrase("UNIDAD", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, BaseColor.WHITE)));

            celdavalor = new PdfPCell(new Phrase("ESPECIFICACIÓN", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, BaseColor.WHITE)));
            celdametodo = new PdfPCell(new Phrase("MÉTODO", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, BaseColor.WHITE)));
            celdaresultado = new PdfPCell(new Phrase("RESULTADO", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, BaseColor.WHITE)));
            celdaprenc.BackgroundColor = colordefinido;
            celdavalor.BackgroundColor = colordefinido;
            celdaunidad.BackgroundColor = colordefinido;
            celdametodo.BackgroundColor = colordefinido;
            celdaresultado.BackgroundColor = colordefinido;
            tablaencpre.AddCell(celdaprenc);
            tablaencpre1.AddCell(celdavalor);
            tablaencpre4.AddCell(celdaunidad);
            tablaencpre2.AddCell(celdametodo);
            tablaencpre3.AddCell(celdaresultado);

            for (int i = 0; i < arraydatos.Length; i++)
            {
                celdaprenc = new PdfPCell();
                celdavalor = new PdfPCell();
                celdametodo = new PdfPCell();
                celdaresultado = new PdfPCell();

                celdaunidad = new PdfPCell();




                string cuenta = arraydatos[i];
                string[] arraydatoscortados;
                try
                {
                    arraydatoscortados = cuenta.Split(':');
                    acc = arraydatoscortados[0] /*+ ": " + arraydatoscortados[1] + "   "*/;
                    valor = arraydatoscortados[1];

                }
                catch (Exception err)
                {

                }
                if (acc == "EMBOBINADO")
                {
                    if (valor == "")
                    {
                        valor = "A";
                    }
                    Embobinado = valor;
                }
                string[] arraydatosPrsentacionMetodo;
                arraydatosPrsentacionMetodo = _templatePDF.MetodoResultado.Split(',');
                string[] arraydatoscortadosMetodo;
                arraydatoscortadosMetodo = arraydatosPrsentacionMetodo[i].Split(':');
                acc1 = arraydatoscortadosMetodo[0] /*+ ": " + arraydatoscortados[1] + "   "*/;
                valor1 = arraydatoscortadosMetodo[1];

                try
                {
                    string[] arraydatosPrsentacionU;
                    arraydatosPrsentacionU = _templatePDF.PresentacionFinalUnidad.Split(',');
                    string[] arraydatoscortadosUnidad;
                    arraydatoscortadosUnidad = arraydatosPrsentacionU[i].Split(':');
                    acc4 = arraydatoscortadosUnidad[0] /*+ ": " + arraydatoscortados[1] + "   "*/;
                    valor4 = arraydatoscortadosUnidad[1];

                }
                catch (Exception err)
                {

                }
            
                string[] arraydatosPrsentacionResultado;
                arraydatosPrsentacionResultado = _templatePDF.PresentacionResultado.Split(',');
                string[] arraydatoscortadosResultado;
                arraydatoscortadosResultado = arraydatosPrsentacionResultado[i].Split(':');
                acc2 = arraydatoscortadosResultado[0] /*+ ": " + arraydatoscortados[1] + "   "*/;
                valor2 = arraydatoscortadosResultado[1];

                celdaprenc = new PdfPCell(new Phrase(acc, new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
                
               
               
                if (acc.Contains("TINTAS"))
                {

                    float[] tamanoColumnasProductos1 = { 15f };
                    PdfPTable tablaProductos1 = new PdfPTable(tamanoColumnasProductos1);
                    //tablaProductos.SpacingBefore = 1;
                    //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaProductos1.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tablaProductos1.DefaultCell.BorderWidthLeft = 0.0f;
                    tablaProductos1.DefaultCell.BorderWidthRight = 0.0f;
                    tablaProductos1.DefaultCell.BorderWidthBottom = 0.0f;
                    tablaProductos1.DefaultCell.BorderWidthTop = 0.0f;
                    tablaProductos1.SetTotalWidth(tamanoColumnasProductos1);
                    tablaProductos1.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    tablaProductos1.LockedWidth = true;

                    //tablaProductos.SplitLate = false;
                    //tablaProductos.SplitRows = true;
                    iTextSharp.text.Image imageM = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/MENORIGUAL.PNG")); //image1.ScalePercent(50f);
                    imageM.ScaleAbsoluteWidth(15);
                    imageM.ScaleAbsoluteHeight(15);
                    PdfPCell V4M = new PdfPCell(imageM);
                    V4M.Border = PdfPCell.NO_BORDER;
                    V4M.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    //tablaProductos.AddCell(image1);
                    tablaProductos1.AddCell(V4M);



                    float[] tamanoColumnasProductos = { 10f};
                    PdfPTable tablaProductos = new PdfPTable(tamanoColumnasProductos);
                    //tablaProductos.SpacingBefore = 1;
                    //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tablaProductos.DefaultCell.BorderWidthLeft = 0.0f;
                    tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
                    tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
                    tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
                    tablaProductos.SetTotalWidth(tamanoColumnasProductos);
                    tablaProductos.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    tablaProductos.LockedWidth = true;

                    //tablaProductos.SplitLate = false;
                    //tablaProductos.SplitRows = true;
                    iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/DELTAE.png")); //image1.ScalePercent(50f);
                    image1.ScaleAbsoluteWidth(10);
                    image1.ScaleAbsoluteHeight(10);
                    PdfPCell V4I = new PdfPCell(image1);
                    V4I.Border = PdfPCell.NO_BORDER;
                    V4I.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    //tablaProductos.AddCell(image1);
                    tablaProductos.AddCell(V4I);


                    //celdavalor = new PdfPCell(tablaProductos1);
                    // celdaunidad = new PdfPCell(tablaProductos);
                    //celdaresultado = new PdfPCell(tablaProductos1);
                    celdavalor = new PdfPCell(new Phrase("Igual menor a 3 Delta E".ToUpper(), new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
                    celdaunidad = new PdfPCell(new Phrase("Delta E".ToUpper(), new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
                    celdaresultado = new PdfPCell(new Phrase("OK", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));




                }
                else
                {
                    celdavalor = new PdfPCell(new Phrase(valor, new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));

                    celdaunidad = new PdfPCell(new Phrase(valor4, new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
                    celdaresultado = new PdfPCell(new Phrase(valor2, new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));

                }

                celdametodo = new PdfPCell(new Phrase(valor1, new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
                
                tablaencpre.AddCell(celdaprenc);
                tablaencpre1.AddCell(celdavalor);
                tablaencpre4.AddCell(celdaunidad);
              
                tablaencpre2.AddCell(celdametodo);
                tablaencpre3.AddCell(celdaresultado);


            }


            tablaDatos.AddCell(tablaencpre);
            tablaDatos.AddCell(tablaencpre1);
            tablaDatos.AddCell(tablaencpre4);
          
            tablaDatos.AddCell(tablaencpre2);
            tablaDatos.AddCell(tablaencpre3);
            //_documento.Add(tablaDatos);

            //////////////////////////////////////////////////////////////
            ///Tintas

            float[] ancho50 = { 600f };
            PdfPTable tabla50 = new PdfPTable(ancho50);
            tabla50.DefaultCell.Border = Rectangle.NO_BORDER;

            tabla50.SetTotalWidth(ancho50);

            tabla50.SpacingBefore = 15;
            tabla50.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tabla50.LockedWidth = true;
            tabla50.DefaultCell.Border = 0;

            ///
            PdfPCell celdaI = new PdfPCell(new Phrase("(*) Procedimientos internos", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.WHITE)));

            tabla50.AddCell(celdaI);
            float[] ancho5 = { 200f, 200f, 200f };
            PdfPTable tabla5 = new PdfPTable(ancho5);
            tabla5.DefaultCell.Border = Rectangle.NO_BORDER;

            tabla5.SetTotalWidth(ancho5);

            tabla5.SpacingBefore = 15;
            tabla5.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tabla5.LockedWidth = true;
            tabla5.DefaultCell.Border = 0;


            PdfPCell celda1 = new PdfPCell(new Phrase("No Lote", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda2 = new PdfPCell(new Phrase(_templatePDF.lote, new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));
            string lote = "S/R";
            if (_templatePDF.lote != "")
            {
                lote = "";
            }
            PdfPCell celda3 = new PdfPCell(new Phrase(lote + "\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));
            celda1.BackgroundColor = colordefinido;

            //celda1.Border = Rectangle.NO_BORDER;
            celda1.Border = PdfPCell.BOTTOM_BORDER;
            celda1.BorderColor = BaseColor.WHITE;

            celda2.Border = PdfPCell.BOTTOM_BORDER;
            celda2.BorderColor = BaseColor.BLACK;
            celda3.Border = PdfPCell.BOTTOM_BORDER;
            celda3.BorderColor = BaseColor.BLACK;

            tabla5.AddCell(celda1);
            tabla5.AddCell(celda2);
            tabla5.AddCell(celda3);
            PdfPCell celda4 = new PdfPCell(new Phrase("Fecha de elaboración", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda5 = new PdfPCell(new Phrase(_templatePDF.fecha, new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));

            PdfPCell celda6 = new PdfPCell(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));
            celda4.BackgroundColor = colordefinido;
            celda4.Border = PdfPCell.BOTTOM_BORDER;
            celda4.BorderColor = BaseColor.WHITE;
            celda5.Border = PdfPCell.BOTTOM_BORDER;
            celda5.BorderColor = BaseColor.BLACK;
            celda6.Border = PdfPCell.BOTTOM_BORDER;
            celda6.BorderColor = BaseColor.BLACK;

            tabla5.AddCell(celda4);
            tabla5.AddCell(celda5);
            tabla5.AddCell(celda6);



            float[] ancho8 = { 300f, 300f };
            PdfPTable tabla8 = new PdfPTable(ancho8);
            tabla8.DefaultCell.Border = Rectangle.NO_BORDER;

            tabla8.SetTotalWidth(ancho8);

            tabla8.SpacingBefore = 15;
            tabla8.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tabla8.LockedWidth = true;

            PdfPCell celda16 = new PdfPCell(new Phrase("Cantidad liberada:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            CalidadLimiteAceptacion calidad = new CalidadLimiteAceptacionContext().CalidadLimitesAceptacion.Where(s => s.CodigoLetra == _templatePDF.Letra).FirstOrDefault();
            PdfPCell celda17 = new PdfPCell(new Phrase(_templatePDF.CantidadLiberada.ToString() + " Mill." + "\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));
            celda16.Border = 0;
            celda17.Border = 0;

            tabla8.AddCell(celda16);
            tabla8.AddCell(celda17);

            PdfPCell celdaM = new PdfPCell(new Phrase("Muestreo:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            
            PdfPCell celdaM1 = new PdfPCell(new Phrase(_templatePDF.Muestreo.ToString() + " ANSI\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));
            celdaM.Border = 0;
            celdaM1.Border = 0;

            tabla8.AddCell(celdaM);
            tabla8.AddCell(celdaM1);



            PdfPCell celdaNivel = new PdfPCell(new Phrase("Nivel de Inspección:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            Inspeccion nivelInspeccion = new CalidadContext().Inspecciones.Where(s => s.IDInspeccion == _templatePDF.Inspeccion).FirstOrDefault();
            PdfPCell celdaNivel2 = new PdfPCell(new Phrase(nivelInspeccion.Descripcion + "\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));
            celdaNivel.Border = 0;
            celdaNivel2.Border = 0;

            tabla8.AddCell(celdaNivel);
            tabla8.AddCell(celdaNivel2);


            PdfPCell celda18 = new PdfPCell(new Phrase("Producto conforme:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));

            PdfPCell celda19 = new PdfPCell(new Phrase("OK" + "\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));
            celda18.Border = 0;
            celda19.Border = 0;


            tabla8.AddCell(celda18);
            tabla8.AddCell(celda19);



            PdfPCell celda22 = new PdfPCell(new Phrase("Responsable de muestreo:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));

            PdfPCell celda23 = new PdfPCell(new Phrase(_templatePDF.Responsable + "\n", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL)));



            celda22.Border = 0;
            celda23.Border = 0;

            tabla8.AddCell(celda22);
            tabla8.AddCell(celda23);
            float[] ancho7 = { 600f };
            PdfPTable tabla7 = new PdfPTable(ancho7);
            tabla7.DefaultCell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tabla7.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla7.SetTotalWidth(ancho7);
            tabla7.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tabla7.LockedWidth = true;


            string cadenafinal = "NOTA 1: CLASS LABELS S. DE R.L. NO SE RESPONSABILIZA POR EL USO, ALMACENAMIENTO, MANEJO Y TRANSPORATACION INADECUADA DEL PRODUCTO QUE OCASIONE ALGUN DAÑO O DETERIORO DEL MISMO Y SERA REEMPLAZO A MENOS QUE SE COMPRUEBE QUE ES DEFECTO DE FABRICACION. ";
                cadenafinal += "\n\nNOTA 2: CON LOS RESULTADOS OBTENIDOS DE LAS EVALUACIONES DEL PRODUCTO DESCRITO EN ESTE CERTIFICADO, REPRESENTA EL LOTE TOTAL PRODUCIDO.\n\n";
            cadenafinal += "NOTA 3: DEBIDO A LAS NUMEROSAS VARIABLES QUE PUEDEN EXISTIR MEDIANTE EL USO DE NUESTRO PRODUCTO , ESTE CERTIFICADO NO EXIME AL CLIENTE DE REALIZAR SUS PROPIOS CONTROLES Y ENSAYOS DE APLICACION.";

            PdfPCell celdac = new PdfPCell();
            celdac =new PdfPCell( new Phrase(cadenafinal, new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL)));
            celdac.BorderWidthBottom = 1;
            celdac.BorderWidthTop = 1;
            tabla7.AddCell(celdac);





            _documento.Add(tabla2);
            _documento.Add(tablaProductosPrincipal);
            _documento.Add(tablaDatos);


            //_documento.Add(tablaencpre2);
            _documento.Add(tabla50);
            _documento.Add(tabla5);
            _documento.Add(tabla8);
            _documento.Add(tabla7);
        }




        #endregion

    }






    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsCe : PdfPageEventHelper
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

            float[] anchoColumasTablaTotales = { 100f, 400f, 100f };
            PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumasTablaTotales);
            tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.LockedWidth = true;
            tabla.AddCell(new Phrase("Class Labels  S. de R.L. de C.V \n Fecha de Revisión:\n 27-10-2020", new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase("" + "\n" + TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            //tabla.AddCell(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
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
            footerTemplate.ShowText((writer.PageNumber).ToString());
            footerTemplate.EndText();
        }
    }
    #endregion



}
