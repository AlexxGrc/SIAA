using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace SIAAPI.Reportes
{
    public class ReporteMovInteralPorFecha
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;
        PdfWriter _writer;
        int _totalColumn = 8;
        PdfPTable _pdfTable = new PdfPTable(8);
        PdfPTable tablae = new PdfPTable(8);
        PdfPCell _PdfPCell;
        public DateTime fechaini { get; set; }
        public DateTime fechafin { get; set; }
        public int IDMovimiento { get; set; }
        MemoryStream _memoryStream = new MemoryStream();
        List<MovimientoInvGetSet> movimientos = new List<MovimientoInvGetSet>();
        #endregion();

        public MovimientoGetSetContext db = new MovimientoGetSetContext();
        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(DateTime _fechaini, DateTime _fechafin)

        {
            fechaini = _fechaini;
            fechafin = _fechafin;

            movimientos = this.GetMovimiento();

            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;
            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            _documento.Open();

            // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});
            this.ReportHeader();
            this.ReportBody();

            //  _pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();


            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Reporte movimiento interalmacen.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


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

        //Obtener la lista de vendedores en un periodo de fechas
        public List<MovimientoInvGetSet> GetMovimiento()
        {
            DateTime fecIni = fechaini; 
            DateTime fecFin = fechafin;
            
            string FI = fecIni.Year.ToString() + "-" + fecIni.Month.ToString() + "-" + fecIni.Day.ToString();
            string FF = fecFin.Year.ToString() + "-" + fecFin.Month.ToString() + "-" + fecFin.Day.ToString();

            List<MovimientoInvGetSet> movimientos = new List<MovimientoInvGetSet>();
            try
            {
                //string cadena = "select * from MovInterAlmacen where FechaMovimiento >= '"+fecIni+"' and FechaMovimiento <='"+fecFin+"'";
                string cadena = "select MIA.IDMovimiento,MIA.FechaMovimiento, AlmS.Descripcion as 'AlmS', Art.Descripcion, Car.Presentacion, MIA.Lote, MIA.Cantidad, TraS.Nombre as 'TraS', TraE.Nombre as 'TraE', AlmE.Descripcion as 'AlmE', MIA.Observacion from MovInterAlmacen MIA inner join Almacen as AlmS on MIA.IDAlmacenS = AlmS.IDAlmacen inner join Almacen as AlmE on MIA.IDAlmacenE = AlmE.IDAlmacen inner join Articulo as Art on MIA.IDArticulo = Art.IDArticulo inner join Caracteristica as Car on MIA.IDCaracteristica = Car.ID inner join Trabajador as TraS on MIA.IDTrabajadorS = TraS.IDTrabajador inner join Trabajador as TraE on MIA.IDTrabajadorE = TraE.IDTrabajador where FechaMovimiento >= '" + FI + "' and FechaMovimiento <='" + FF + "'";
                movimientos = db.Database.SqlQuery<MovimientoInvGetSet>(cadena).ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return movimientos;
        }

        private void ReportHeader()
        {

            string _Titulo = "REPORTE MOVIMIENTO ENTRE ALMACENES";
            //↓--Logo desde BD
            Empresa empresa = new EmpresaContext().empresas.Find(2);//se usa para buscar el logo de la empresa en la bd
            System.Drawing.Image logo = byteArrayToImage(empresa.Logo);
            Image jpg = Image.GetInstance(logo, BaseColor.WHITE);
            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_RIGHT;
            jpg.ScaleToFit(105f, 75F); //largo y ancho de la imagen
            jpg.Alignment = Image.ALIGN_RIGHT;
            jpg.SetAbsolutePosition(50f, 715f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera (+derecha/-izquierda,+arriba/-abajo)
            _documento.Add(jpg);
            //↑--Logo desde BD

            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 250f;
            p1.Leading = 20;
            p1.Add(new Phrase(_Titulo, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.SpacingAfter = 40;
            _documento.Add(p1);

            //↓--Creacion de tabla para encabezado documento
            float[] anchoColEncDoc = {400f, 200f};//ancho de las columnas del encabezado del documento
            PdfPTable tablaEncDoc = new PdfPTable(anchoColEncDoc);
            Font fontFecha = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // fuente para: encabezado documento (titulo y fecha de impresión)

            tablaEncDoc.SetTotalWidth(anchoColEncDoc);
            tablaEncDoc.SpacingBefore = 0;
            tablaEncDoc.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaEncDoc.LockedWidth = true;
 
            DateTime fechaact = DateTime.Today;
            string fecAct = fechaact.ToString("dd/MM/yyyy");
            string fecIni = fechaini.ToString("dd/MM/yyyy");
            string fecFin = fechafin.ToString("dd/MM/yyyy");

            PdfPCell celdaEncDoc = new PdfPCell(new Phrase("Periodo del: "+fecIni +" al: "+fecFin, fontFecha));
            celdaEncDoc.Border = 0;
            //celdaEncDoc.FixedHeight = 10f;
            celdaEncDoc.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaEncDoc.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaEncDoc.BackgroundColor = BaseColor.WHITE;
            tablaEncDoc.AddCell(celdaEncDoc);
             
            celdaEncDoc = new PdfPCell(new Phrase("Fecha de impresión: "+fecAct, fontFecha));
            celdaEncDoc.Border = 0;
            //celdaEncDoc2.FixedHeight = 20f;
            celdaEncDoc.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaEncDoc.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaEncDoc.BackgroundColor = BaseColor.WHITE;
            tablaEncDoc.AddCell(celdaEncDoc);

            //tablaencabezado.CompleteRow();
            _documento.Add(tablaEncDoc);
            //↑--Creacion de tabla para encabezado documento

            //↓--Creacion de tabla para encabezado de tabla de contenido
            float[] anchoColEncCont = {30f, 54f, 54f, 78f, 78f, 30f, 54f, 54f, 54f, 54f, 54f};//ancho de las columas del encabezado de tabla de contenido
            PdfPTable tablaEncCont = new PdfPTable(anchoColEncCont);
            PdfPCell celdaPdf;
            CMYKColor COLORDEREPORTE = new ClsColoresReporte(Properties.Settings.Default.ColorReporte).color;
            CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(Properties.Settings.Default.ColorFuenteEncabezado).color;
            Font fontEncCont = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE); // fuente para:
            tablaEncCont.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaEncCont.WidthPercentage = 100;

            celdaPdf = new PdfPCell(new Phrase("No. mov.", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Fecha mov.", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Alm. salida", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Artículo", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Presentación", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Lote", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Cantidad", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Entregó", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Recibió", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Alm. recibe", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Observación", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            tablaEncCont.CompleteRow();//tablaPdf esta declarada arriba 
            _documento.Add(tablaEncCont);
            //↑--Creacion de tabla para encabezado de tabla de contenido

        }

        private void ReportBody()
        {
            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            //  

            foreach (MovimientoInvGetSet movimiento in movimientos)
            {
                Font fontCont = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL); // fuente para:
                float[] anchoColCont = { 30f, 54f, 54f, 78f, 78f, 30f, 54f, 54f, 54f, 54f, 54f };
                PdfPTable tablaCont = new PdfPTable(anchoColCont);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaCont.SetTotalWidth(anchoColCont);
                tablaCont.SpacingBefore = 3;
                tablaCont.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaCont.LockedWidth = true;

                //Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell celdaPdf = new PdfPCell(new Phrase(movimiento.IDMovimiento.ToString(), fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.FechaMovimiento.ToString("dd/MM/yyyy"), fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.AlmS, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.Descripcion, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.Presentacion, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.Lote, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.Cantidad.ToString(), fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.TraS, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.TraE, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.AlmE, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                celdaPdf = new PdfPCell(new Phrase(movimiento.Observacion, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                tablaCont.CompleteRow();
                _documento.Add(tablaCont);
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

    public class MovimientoInvGetSet
    {
        [Key]
        public int IDMovimiento { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string AlmS { get; set; }
        public string Descripcion { get; set; }
        public string Presentacion { get; set; }
        public string Lote { get; set; }
        public decimal Cantidad { get; set; }
        public string TraS { get; set; }
        public string TraE { get; set; }
        public string AlmE { get; set; }
        public string Observacion { get; set; }
    }

    public class MovimientoGetSetContext : DbContext
    {
        public MovimientoGetSetContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VendedorPeContext>(null);
        }
        //public DbSet<VendedorPedidoDls> Vendedor { get; set; }


    }



}