using iTextSharp.text; //Librerias iTextSharp para generar pdf
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion; //Referencia a las carpetas que usamos en el proyecto
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
//↓↑--
namespace SIAAPI.Reportes
{
    public class RprtLstTipoVendedor
    {
        //↓--variables
        Document documento;
        PdfPTable tablaPdf;//se usa tambien en prepare report
        PdfWriter escritor;
        PdfPCell celdaPdf;
        MemoryStream flujoMemoria = new MemoryStream();
        static CMYKColor COLORDEREPORTE = new ClsColoresReporte(Properties.Settings.Default.ColorReporte).color;
        static CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(Properties.Settings.Default.ColorFuenteEncabezado).color;
        Font fontFecha = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // fuente para: encabezado documento (titulo y fecha de impresión)
        Font fontEncCont = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE); // fuente para:
        Font fontCont = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL); // fuente para:
        //public VFamiliaContext db = new VFamiliaContext();
        public List<TipoVendedor> ListaDatosBD_TipoVendedor = new List<TipoVendedor>();
        string titulo = "";
        static DateTime fecAct = DateTime.Today;
        string fechaActual = fecAct.ToString("dd/MM/yyyy");
        //↑--variables

        public byte[] PrepareReport()
        {
            documento = new Document(PageSize.LETTER, 20f, 10f, 20f, 30f);
            escritor = PdfWriter.GetInstance(documento, HttpContext.Current.Response.OutputStream);
            escritor.PageEvent = new ITextEvents();
            documento.Open();

            this.encabezadoReporte();
            this.cuerpoReporte();

            documento.Close();

            ////Se comenta si esta directo en el Controller
            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Reporte familias atributos.pdf" + "\"");
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Write(documento);
            //HttpContext.Current.Response.End();

            return flujoMemoria.ToArray();
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
            titulo = "REPORTE LISTA TIPOS VENDEDOR";
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

            //↓--Titulo
            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 250f;
            p1.Leading = 20;
            p1.Add(new Phrase(titulo, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.SpacingAfter = 40;
            documento.Add(p1);
            //↑--Titulo

            //↓--Creacion de tabla para encabezado documento
            float[] anchoColEncDoc = { 580f, 20f };//ancho de las columnas del encabezado del documento
            PdfPTable tablaEncDoc = new PdfPTable(anchoColEncDoc);

            tablaEncDoc.SetTotalWidth(anchoColEncDoc);
            tablaEncDoc.SpacingBefore = 0;
            tablaEncDoc.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaEncDoc.LockedWidth = true;

            PdfPCell celdaEncDoc = new PdfPCell(new Phrase("Fecha de impresión: " + fechaActual, fontFecha));
            celdaEncDoc.Border = 0;
            celdaEncDoc.FixedHeight = 10f;
            celdaEncDoc.HorizontalAlignment = Element.ALIGN_RIGHT;
            celdaEncDoc.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaEncDoc.BackgroundColor = BaseColor.WHITE;
            tablaEncDoc.AddCell(celdaEncDoc);

            celdaEncDoc = new PdfPCell(new Phrase(" ", fontFecha));
            celdaEncDoc.Border = 0;
            //celdaEncDoc.FixedHeight = 10f;
            celdaEncDoc.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaEncDoc.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaEncDoc.BackgroundColor = BaseColor.WHITE;
            tablaEncDoc.AddCell(celdaEncDoc);

            //tablaencabezado.CompleteRow();
            documento.Add(tablaEncDoc);
            //↑--Creacion de tabla para encabezado documento

            //↓--Creacion de tabla para encabezado de tabla de contenido
            float[] anchoColEncCont = { 50f, 550f };//ancho de las columas del encabezado de tabla de contenido
            PdfPTable tablaEncCont = new PdfPTable(anchoColEncCont);
            tablaEncCont.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaEncCont.WidthPercentage = 100;

            celdaPdf = new PdfPCell(new Phrase("ID", fontEncCont));
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

            documento.Add(tablaEncCont);
            //↑--Creacion de tabla para encabezado de tabla de contenido
        }
        private void cuerpoReporte()
        {
            foreach (TipoVendedor BDTipoVendedor in ListaDatosBD_TipoVendedor)
            {
                //↓--Creacion de tabla para contenido de tabla
                float[] anchoColCont = { 50f, 550f };

                //string moneda = new c_MonedaContext().c_Monedas.Find(articulo.IDMoneda).ClaveMoneda;
                //string tipoart = new ArticuloContext().TipoArticulo.Find(articulo.IDTipoArticulo).Descripcion;
                //string claveuni = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad).Nombre;

                PdfPTable tablaCont = new PdfPTable(anchoColCont);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaCont.SetTotalWidth(anchoColCont);
                tablaCont.SpacingBefore = 3;
                tablaCont.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaCont.LockedWidth = true;

                celdaPdf = new PdfPCell(new Phrase(BDTipoVendedor.IDTipoVendedor.ToString(), fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(BDTipoVendedor.DescripcionVendedor, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                tablaCont.CompleteRow();
                documento.Add(tablaCont);
                //↑--Creacion de tabla para contenido de tabla
            }
        }
        //Extensión de la clase pdfPageEvenHelper
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
    }
}