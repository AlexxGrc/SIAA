using iTextSharp.text; //Librerias iTextSharp para generar pdf
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion; //Referencia a las carpetas que usamos en el proyecto
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Comisiones;
using SIAAPI.Models;

namespace SIAAPI.Reportes
{
    public class ReporteCierreVentas
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;

        PdfWriter _writer;
        int _totalColumn = 8;

        string _Titulo = "";
        int _idarticulo = 0;

      

        PdfPTable _pdfTable = new PdfPTable(8);
        //PdfPTable tablae = new PdfPTable(6);


        PdfPCell _PdfPCell;

         public int idmes { get; set; }
         public int anio { get; set; }

        public string mesnombre = "";

        public string Titulo = "";
        MemoryStream _memoryStream = new MemoryStream();

        List<VCierreVentas> cierrevtas = new List<VCierreVentas>();

        #endregion



        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(int _ANIO, int _IDMes)

        {
           
            anio = _ANIO;
 idmes = _IDMes;
            #region
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            _documento.Open();

            // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});
            #endregion
            this.ReportHeader();
            this.ReportBody();

            //_pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();


            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"ReporteCierreVentas.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }

        public List<VCierreVentas> GetCierreVtas(int idmes, int annio)
        {
            if (anio == 0 || idmes == 0)
            {
                anio = DateTime.Now.Year;
                idmes = DateTime.Now.Month;
            }
            List<VCierreVentas> cierrevtas = new List<VCierreVentas>();
            try
            {
                VCierreVentasContext db3 = new VCierreVentasContext();
                string cadenaCV = "select * from dbo.VCierreVentas where IDMes = " + idmes + " and Año= " + anio + "";
                cierrevtas = db3.Database.SqlQuery<VCierreVentas>(cadenaCV).ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return cierrevtas;
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

        private void ReportHeader()
        {
            #region Table head

//se modifico de aquí
            float[] anchoColumnal = { 150f, 450f };

            PdfPTable tablal = new PdfPTable(anchoColumnal);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablal.SetTotalWidth(anchoColumnal);
            tablal.SpacingBefore = 0;
            tablal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablal.LockedWidth = true;
            Font _fontStyleencabezadol = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño


            Empresa empresa = new EmpresaContext().empresas.Find(2);
            System.Drawing.Image logo = byteArrayToImage(empresa.Logo);
            Image jpg = Image.GetInstance(logo, BaseColor.WHITE);
            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_RIGHT;
            jpg.ScaleToFit(105f, 75F); //ancho y largo de la imagen
            jpg.Alignment = Image.ALIGN_RIGHT;
            jpg.SetAbsolutePosition(50f, 715f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera (+derecha/-izquierda,+arriba/-abajo)
            //_documento.Add(jpg);
            tablal.AddCell(jpg);

            paragraph.Clear();//ahora utilizo la clase Paragraph 
            paragraph.Font = new Font(FontFactory.GetFont("Arial", 18, Font.BOLD));
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.Add("Reporte Cierre Ventas");
            PdfPCell cell2 = new PdfPCell();
            cell2.Border = Rectangle.NO_BORDER;
            cell2.PaddingTop = -7;
            cell2.AddElement(paragraph);
            cell2.Colspan = 3;
            paragraph.Clear();
            tablal.AddCell(cell2);
            _documento.Add(tablal);
//Aquí termina la modificación del encabezado

            float[] anchoColumnasencabezado = { 50,100,350f, 100f };

            PdfPTable tablaencabezado = new PdfPTable(anchoColumnasencabezado);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablaencabezado.SetTotalWidth(anchoColumnasencabezado);
            tablaencabezado.SpacingBefore = 0;
            tablaencabezado.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencabezado.LockedWidth = true;
            Font _fontStyleencabezado = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell _tablaencabezado1 = new PdfPCell(new Phrase("Periodo: ", _fontStyleencabezado));
            _tablaencabezado1.Border = 0;
            _tablaencabezado1.FixedHeight = 10f;
            _tablaencabezado1.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablaencabezado1.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado1.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado1);

            c_MesesContext db = new c_MesesContext();
            var meses = db.c_Meses.Single(m => m.IDMes == idmes);
            mesnombre = meses.Mes.ToString();

            PdfPCell _tablaencabezado2 = new PdfPCell(new Phrase(mesnombre + "-" + anio  , _fontStyleencabezado));
            _tablaencabezado2.Border = 0;
            // _tablaencabezado4.FixedHeight = 10f;
            _tablaencabezado2.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaencabezado2.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado2.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado2);

            PdfPCell _tablaencabezado3 = new PdfPCell(new Phrase("Fecha de impresión: ", _fontStyleencabezado));
            _tablaencabezado3.Border = 0;
            _tablaencabezado3.FixedHeight = 10f;
            _tablaencabezado3.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablaencabezado3.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado3.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado3);

            DateTime fecAct = DateTime.Today;
            string FA = fecAct.ToString("dd/MM/yyyy");
            PdfPCell _tablaencabezado4 = new PdfPCell(new Phrase(FA, _fontStyleencabezado));
            _tablaencabezado4.Border = 0;
            // _tablaencabezado4.FixedHeight = 10f;
            _tablaencabezado4.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaencabezado4.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado4.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado4);

            //tablaencabezado.CompleteRow();
            _documento.Add(tablaencabezado);


            //No debe exceder de 600 Float
            

            float[] anchoColumnasencart = { 50f, 30f, 80f, 50f, 80f, 80f, 80f, 80f, 50f, 20f };

            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

            PdfPTable tablae = new PdfPTable(anchoColumnasencart);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;

            // aqui podremos cambiar la fuente de lcientes y su tamanño

            Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);

            CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

            Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);




            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Mes", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Año", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Vendedor", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Pedidos Totales", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Ventas MXN", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Ventas Dls", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Cuota Establecida", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Cuota Alcanzada", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Moneda", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Comisión",  _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            _pdfTable.CompleteRow();
            _documento.Add(tablae);

            #endregion

        }
        private void ReportBody()
        {

            #region Table Body


            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            //  
            List<VCierreVentas> cierreV = new List<VCierreVentas>();
            cierreV = GetCierreVtas(idmes, anio);

            foreach (VCierreVentas c in cierreV)
            {


                // creamos una tabla para imprimir los los datos del cliente
                // como son 4 columnas a imprimir 600entre las 4 

                float[] anchoColumnasCierreV = { 50f, 30f, 80f, 50f, 80f, 80f, 80f, 50f, 50f, 20f };

                // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor


                PdfPTable tablaCierreV = new PdfPTable(anchoColumnasCierreV);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaCierreV.SetTotalWidth(anchoColumnasCierreV);
                tablaCierreV.SpacingBefore = 3;
                tablaCierreV.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaCierreV.LockedWidth = true;

                Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(c.Mes, _fontStylecliente));
                _PdfPCell1.Border = 0;
                _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                tablaCierreV.AddCell(_PdfPCell1);

                PdfPCell _PdfPCell4 = new PdfPCell(new Phrase(c.Año.ToString(), _fontStylecliente));
                _PdfPCell4.Border = 0;
                _PdfPCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell4.BackgroundColor = BaseColor.WHITE;
                tablaCierreV.AddCell(_PdfPCell4);


                _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                _PdfPCell = new PdfPCell(new Phrase(c.Vendedor, _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCierreV.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                _PdfPCell = new PdfPCell(new Phrase(c.PedidosTotales.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCierreV.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                _PdfPCell = new PdfPCell(new Phrase(c.VentasMXN.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCierreV.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                _PdfPCell = new PdfPCell(new Phrase(c.VentasUSD.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCierreV.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                _PdfPCell = new PdfPCell(new Phrase(c.CuotaEstablecida.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCierreV.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                _PdfPCell = new PdfPCell(new Phrase(c.CuotaAlcanzada.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCierreV.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                _PdfPCell = new PdfPCell(new Phrase(c.Moneda, _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCierreV.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                _PdfPCell = new PdfPCell(new Phrase(c.Comision.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCierreV.AddCell(_PdfPCell);

                _documento.Add(tablaCierreV);

            }
            #endregion
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


}





