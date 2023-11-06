using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.Entity;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;

//↓↑--

namespace SIAAPI.Reportes
{
    public class ReportePorUnidad
    {
        //--Variables iTextSharp
        Document documento;
        PdfWriter escritor;
        PdfPTable tablaPdf;//se usa tambien en prepare report
        PdfPTable tablaPdf1;
        PdfPCell celdaPdf;
        List<UnidadGetSet> articulos = new List<UnidadGetSet>();//Lo usa el foreach
        UnidadGetSetContext db = new UnidadGetSetContext();
        MemoryStream flujoMemoria = new MemoryStream();
        static CMYKColor COLORDEREPORTE = new ClsColoresReporte(Properties.Settings.Default.ColorReporte).color;
        static CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(Properties.Settings.Default.ColorFuenteEncabezado).color;
        Font fontFecha = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // fuente para: encabezado documento (titulo y fecha de impresión)
        Font fontEncCont = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE); // fuente para:
        Font fontCont = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL); // fuente para:
        //Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD); //fuente para:
        //Font _fontStyleencabezado = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD); //fuente para:
        //Font _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);
        //--Variables
        string _Titulo = "";
        public int IDClaveUnidad { get; set; }//variable que se usa en la vista

        public byte[] prepararReporte(int _IDUnidad)

        {
            IDClaveUnidad = _IDUnidad;

            articulos = this.GetUnidad(IDClaveUnidad);

            documento = new Document(PageSize.LETTER, 20f, 10f, 20f, 30f);
            //documento.SetMargins(20f, 10f, 20f, 30f);
            tablaPdf = new PdfPTable(8);
            tablaPdf1 = new PdfPTable(5);
            escritor = PdfWriter.GetInstance(documento, HttpContext.Current.Response.OutputStream);
            escritor.PageEvent = new ITextEvents();
            tablaPdf.WidthPercentage = 100;
            tablaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaPdf1.WidthPercentage = 100;
            tablaPdf1.HorizontalAlignment = Element.ALIGN_LEFT;
            documento.Open();

            this.encabezadoReporte();
            this.cuerpoReporte();

            documento.Close();

            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Reporte por unidad.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(documento);
            HttpContext.Current.Response.End();

            return flujoMemoria.ToArray();
        }

        public List<UnidadGetSet> GetUnidad(int idUnidad)
        {
            List<UnidadGetSet> rep = new List<UnidadGetSet>();
            try
            {
                if (idUnidad != 0)
                {
                    string cadena = "select distinct idarticulo, cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo, namefoto from Articulo where IDClaveUnidad ="+idUnidad+" order by Descripcion";
                    rep = db.Database.SqlQuery<UnidadGetSet>(cadena).ToList();
                }
                else
                {
                    string cadena = "select distinct idarticulo, cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo, namefoto from Articulo order by IDClaveUnidad, Descripcion";
                    rep = db.Database.SqlQuery<UnidadGetSet>(cadena).ToList();
                }
            }
            catch (SqlException err)
            {
                string mensajeError = err.Message;
            }

            return rep;
        }

        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            System.Drawing.Image returnImage = null;
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                returnImage = System.Drawing.Image.FromStream(ms, true);//Exception occurs here
            }
            catch { }
            return returnImage;
        }

        private void encabezadoReporte()
        {
            _Titulo = "REPORTE GENERAL POR UNIDAD";
            //↓--Logo desde BD
            Empresa empresa = new EmpresaContext().empresas.Find(2);//se usa para buscar el logo de la empresa en la bd
            System.Drawing.Image logo = byteArrayToImage(empresa.Logo);
            Image jpg = Image.GetInstance(logo, BaseColor.WHITE);
            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_RIGHT;
            jpg.ScaleToFit(105f, 75F); //largo y ancho de la imagen
            jpg.Alignment = Image.ALIGN_RIGHT;
            jpg.SetAbsolutePosition(50f, 715f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera (+derecha/-izquierda,+arriba/-abajo)
            documento.Add(jpg);
            //↑--Logo desde BD

            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 250f;
            p1.Leading = 20;
            p1.Add(new Phrase(_Titulo, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.SpacingAfter = 40;
            documento.Add(p1);

            //↓--Creacion de tabla para encabezado documento
            float[] anchoColEncDoc = { 500f, 100f };//ancho de las columnas del encabezado del documento
            PdfPTable tablaEncDoc = new PdfPTable(anchoColEncDoc);

            tablaEncDoc.SetTotalWidth(anchoColEncDoc);
            tablaEncDoc.SpacingBefore = 0;
            tablaEncDoc.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaEncDoc.LockedWidth = true;

            //Font _fontStyleencabezado = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD); aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell celdaEncDoc = new PdfPCell(new Phrase("Fecha de impresión: ", fontFecha));
            celdaEncDoc.Border = 0;
            celdaEncDoc.FixedHeight = 10f;
            celdaEncDoc.HorizontalAlignment = Element.ALIGN_RIGHT;
            celdaEncDoc.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaEncDoc.BackgroundColor = BaseColor.WHITE;
            tablaEncDoc.AddCell(celdaEncDoc);

            DateTime fecAct = DateTime.Today;
            string fechaActual = fecAct.ToString("dd/MM/yyyy");

            PdfPCell celdaEncDoc1 = new PdfPCell(new Phrase(fechaActual, fontFecha));
            celdaEncDoc1.Border = 0;
            //celdaEncDoc1.FixedHeight = 10f;
            celdaEncDoc1.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaEncDoc1.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaEncDoc1.BackgroundColor = BaseColor.WHITE;
            tablaEncDoc.AddCell(celdaEncDoc1);

            //tablaencabezado.CompleteRow();
            documento.Add(tablaEncDoc);
            //↑--Creacion de tabla para encabezado documento

            //↓--Creacion de tabla para encabezado de tabla de contenido
            float[] anchoColEncCont = { 100f, 300f, 70f, 80f, 50f };//ancho de las columas del encabezado de tabla de contenido
            PdfPTable tablaEncCont = new PdfPTable(anchoColEncCont);

            tablaEncCont.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaEncCont.WidthPercentage = 100;

            celdaPdf = new PdfPCell(new Phrase("CREF", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Descripción", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Tipo artículo", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Tipo unidad", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Moneda", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            //tablaPdf.CompleteRow();//tablaPdf esta declarada arriba 
            documento.Add(tablaEncCont);
            //↑--Creacion de tabla para encabezado de tabla de contenido
        }
        private void cuerpoReporte()
        {
            //_fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            foreach (UnidadGetSet articulo in articulos)
            {
                float[] anchoColCont = { 100f, 300f, 70f, 80f, 50f };

                string moneda = new c_MonedaContext().c_Monedas.Find(articulo.IDMoneda).ClaveMoneda;
                string familia = new ArticuloContext().TipoArticulo.Find(articulo.IDTipoArticulo).Descripcion;
                string claveuni = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad).Nombre;

                PdfPTable tablaCont = new PdfPTable(anchoColCont);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaCont.SetTotalWidth(anchoColCont);
                tablaCont.SpacingBefore = 3;
                tablaCont.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaCont.LockedWidth = true;

                //Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell celdaPdf = new PdfPCell(new Phrase(articulo.Cref, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                PdfPCell celdaPdf1 = new PdfPCell(new Phrase(articulo.Descripcion, fontCont));
                celdaPdf1.Border = 0;
                celdaPdf1.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf1.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf1.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf1);

                PdfPCell celdaPdf2 = new PdfPCell(new Phrase(familia, fontCont));
                celdaPdf2.Border = 0;
                celdaPdf2.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf2.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf2.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf2);

                PdfPCell celdaPdf3 = new PdfPCell(new Phrase(claveuni, fontCont));
                celdaPdf3.Border = 0;
                celdaPdf3.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf3.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf3.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf3);

                PdfPCell celdaPdf4 = new PdfPCell(new Phrase(moneda, fontCont));
                celdaPdf4.Border = 0;
                celdaPdf4.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf4.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf4.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf4);

                tablaCont.CompleteRow();

                // añadimos la tabla al documento princlipal para que la imprimia
                //_documento.Add(imagen);
                documento.Add(tablaCont);
            }
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


                String text = "Página " + writer.PageNumber + " de ";

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
                    //tabla.AddCell(new Phrase("Este documento es una representación impresa de un CFDI 3.3", new Font(Font.FontFamily.HELVETICA, 10)));

                    tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

                }
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
    }

    public class UnidadGetSet
    {
        [Key]
        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        public string Descripcion { get; set; }
        public bool esKit { get; set; }
        public int IDMoneda { get; set; }
        public int IDFamilia { get; set; }
        public int IDTipoArticulo { get; set; }
        public int IDClaveUnidad { get; set; }
        public string Presentacion { get; set; }
        public string nameFoto { get; set; }

    }

    public class UnidadGetSetContext : DbContext
    {
        public UnidadGetSetContext() : base("name=DefaultConnection")

        {
            Database.SetInitializer<UnidadGetSetContext>(null);
        }
        public DbSet<Caracteristica> Caracteristica { get; set; }
        public DbSet<Familia> Familia { get; set; }
        public DbSet<c_Moneda> Moneda { get; set; }
        public DbSet<Articulo> ArticuloC { get; set; }
    }
}