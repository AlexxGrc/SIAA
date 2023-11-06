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
using SIAAPI.ViewModels.Cfdi;

namespace SIAAPI.Reportes
{
   
    public class ReporteListadoFacPag
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;

        PdfWriter _writer;
        int _totalColumn = 8;

        PdfPTable _pdfTable = new PdfPTable(8);
        //PdfPTable tablae = new PdfPTable(6);


        PdfPCell _PdfPCell;
        public DateTime fechaini { get; set; }
        public DateTime fechafin { get; set; }

       

        public string Titulo = "";
        MemoryStream _memoryStream = new MemoryStream();
        List<VPagoFacMesS_NC_A> pagoFacturas = new List<VPagoFacMesS_NC_A>();

        #endregion

        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(DateTime fechaInicial, DateTime fechaFinal)

        {
            fechaini = fechaInicial;
            fechafin = fechaFinal;
            
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
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"ReportePagos.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }
        public List<VPagoFacMesS_NC_A> GetPagoFacturas()
        {
            List<VPagoFacMesS_NC_A> pagoFactura = new List<VPagoFacMesS_NC_A>();
            try
            {

                string FI = fechaini.Year.ToString() + "-" + fechaini.Month.ToString() + "-" + fechaini.Day.ToString();
                string FF = fechafin.Year.ToString() + "-" + fechafin.Month.ToString() + "-" + fechafin.Day.ToString();

                string cadenaSQL = "select * from VPagoFacMesS_NC_A where  FechaPago BETWEEN '" + FI + "' and '" + FF + " 23:59:59.999' order by nombre_cliente";

                VPagoFacMesS_NC_AContext db = new VPagoFacMesS_NC_AContext();
                pagoFactura = db.Database.SqlQuery<VPagoFacMesS_NC_A>(cadenaSQL).ToList();

            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return pagoFactura;
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
            Empresa empresa = new EmpresaContext().empresas.Find(2);


            Image jpg = Image.GetInstance(byteArrayToImage(empresa.Logo), BaseColor.WHITE);

            jpg.ScaleToFit(105f, 75f);

            CMYKColor colorletratitulo;

            colorletratitulo = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

            PdfPCell imageCell = new PdfPCell(jpg);
            _PdfPCell = new PdfPCell((imageCell));
            _PdfPCell.Border = 0;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 15f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Facturas pagadas", _fontStyle));
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.Border = 0;
            _PdfPCell.BackgroundColor = BaseColor.WHITE;
            _PdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_PdfPCell);
            _documento.Add(_pdfTable);
            float[] anchoColumnal = {150f, 450f };

          
            //Aquí termina la modificación del encabezado


            float[] anchoColumnasfechas = { 100f, 50f, 25f, 225f, 100f, 100f };

            PdfPTable tablafechas = new PdfPTable(anchoColumnasfechas);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablafechas.SetTotalWidth(anchoColumnasfechas);
            tablafechas.SpacingBefore = 0;
            tablafechas.HorizontalAlignment = Element.ALIGN_LEFT;
            tablafechas.LockedWidth = true;
            Font _fontStylefecha = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell _tablafechas1 = new PdfPCell(new Phrase("Periodo del ", _fontStylefecha));
            _tablafechas1.Border = 0;
            _tablafechas1.FixedHeight = 20f;
            _tablafechas1.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablafechas1.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas1.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas1);

            string FI = fechaini.ToString("dd/MM/yyyy");
            PdfPCell _tablafechas2 = new PdfPCell(new Phrase(FI, _fontStylefecha));
            _tablafechas2.Border = 0;
            _tablafechas2.FixedHeight = 10f;
            _tablafechas2.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas2.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas2.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas2);

            PdfPCell _tablafechas3 = new PdfPCell(new Phrase("al ", _fontStylefecha));
            _tablafechas3.Border = 0;
            _tablafechas3.FixedHeight = 10f;
            _tablafechas3.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas3.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas3.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas3);

            string FF = fechafin.ToString("dd/MM/yyyy");
            PdfPCell _tablafechas4 = new PdfPCell(new Phrase(FF, _fontStylefecha));
            _tablafechas4.Border = 0;
            _tablafechas4.FixedHeight = 10f;
            _tablafechas4.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas4.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas4.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas4);

            PdfPCell _tablafechas5 = new PdfPCell(new Phrase("Fecha de impresión: ", _fontStylefecha));
            _tablafechas5.Border = 0;
            _tablafechas5.FixedHeight = 10f;
            _tablafechas5.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablafechas5.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas5.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas5);

            DateTime fecAct = DateTime.Today;
            string FA = fecAct.ToString("dd/MM/yyyy");
            PdfPCell _tablafechas6 = new PdfPCell(new Phrase(FA, _fontStylefecha));
            _tablafechas6.Border = 0;
            _tablafechas6.FixedHeight = 10f;
            _tablafechas6.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas6.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas6.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas6);
            //tablafechas.CompleteRow();
            _documento.Add(tablafechas);

            CMYKColor colordefinido;

            colordefinido = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;



            //No debe exceder de 600 Float
            //Encabezado de la tabla de datos
            float[] anchoColumnasPagofactura = { 30f, 35f, 50f, 30f, 30f, 30f, 35f, 70f, 50f, 50f, 50f, 30f, 40f,70f };

            PdfPTable tablae = new PdfPTable(anchoColumnasPagofactura);
            tablae.WidthPercentage = 100;
            tablae.HorizontalAlignment = Element.ALIGN_LEFT;


            _fontStyle = FontFactory.GetFont("Tahoma", 6f, colorletratitulo);
            _PdfPCell = new PdfPCell(new Phrase("ID", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;

            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Fecha de Pago", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Importe Pagado", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Moneda de Pago", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);


            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Serie", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);




            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Número", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Fecha Factura", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Cliente", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Subtotal", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);


            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("IVA", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Total", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Moneda", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Importe Saldo Insoluto", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Vendedor", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _documento.Add(tablae);


            #endregion

        }
        private void ReportBody()
        {

            #region Table Body


            _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);

            //  
            List<VPagoFacMesS_NC_A> pagoFac = new List<VPagoFacMesS_NC_A>();

            pagoFac = GetPagoFacturas();
            foreach (VPagoFacMesS_NC_A p in pagoFac)
            {


                // creamos una tabla para imprimir los los datos 

                float[] anchoColumnasPagofactura = { 30f, 35f, 50f, 30f, 30f, 30f, 35f, 70f, 50f, 50f, 50f, 30f, 40f, 70f };

                // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor


                PdfPTable tablaPagofactura = new PdfPTable(anchoColumnasPagofactura);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaPagofactura.SetTotalWidth(anchoColumnasPagofactura);
                tablaPagofactura.SpacingBefore = 3;
                tablaPagofactura.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaPagofactura.LockedWidth = true;

                Font _fontStyleparaestePagofactura = new Font(Font.FontFamily.HELVETICA, 5, Font.NORMAL);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(p.ID.ToString(), _fontStyleparaestePagofactura));
                _PdfPCell1.Border = 0;
                _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell1);

                PdfPCell _PdfPCell2 = new PdfPCell(new Phrase(p.fechapago.ToString("dd/MM/yyyy"), _fontStyleparaestePagofactura));
                _PdfPCell2.Border = 0;
                _PdfPCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell2.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell2);

                PdfPCell _PdfPCell3 = new PdfPCell(new Phrase(p.importepagado.ToString(), _fontStyleparaestePagofactura));
                _PdfPCell3.Border = 0;
                _PdfPCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell3.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell3);

                PdfPCell _PdfPCell4 = new PdfPCell(new Phrase(p.Monedapago, _fontStyleparaestePagofactura));
                _PdfPCell4.Border = 0;
                _PdfPCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell4.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell4);
                PdfPCell _PdfPCell41 = new PdfPCell(new Phrase(p.serie.ToString(), _fontStyleparaestePagofactura));
                _PdfPCell41.Border = 0;
                _PdfPCell41.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell41.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell41.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell41);

                PdfPCell _PdfPCell5 = new PdfPCell(new Phrase(p.numero.ToString(), _fontStyleparaestePagofactura));
                _PdfPCell5.Border = 0;
                _PdfPCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell5.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell5);

                PdfPCell _PdfPCell6 = new PdfPCell(new Phrase(p.fechaFactura.ToString("dd/MM/yyyy"), _fontStyleparaestePagofactura));
                _PdfPCell6.Border = 0;
                _PdfPCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell6.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell6);

                PdfPCell _PdfPCell7 = new PdfPCell(new Phrase(p.nombre_cliente, _fontStyleparaestePagofactura));
                _PdfPCell7.Border = 0;
                _PdfPCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell7.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell7.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell7);

                PdfPCell _PdfPCell8 = new PdfPCell(new Phrase(p.subtotal.ToString(), _fontStyleparaestePagofactura));
                _PdfPCell8.Border = 0;
                _PdfPCell8.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell8.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell8.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell8);

                PdfPCell _PdfPCell9 = new PdfPCell(new Phrase(p.iva.ToString(), _fontStyleparaestePagofactura));
                _PdfPCell9.Border = 0;
                _PdfPCell9.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell9.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell9.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell9);

                PdfPCell _PdfPCell10 = new PdfPCell(new Phrase(p.total.ToString(), _fontStyleparaestePagofactura));
                _PdfPCell10.Border = 0;
                _PdfPCell10.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell10.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell10.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell10);


                PdfPCell _PdfPCell12 = new PdfPCell(new Phrase(p.Monedapago, _fontStyleparaestePagofactura));
                _PdfPCell12.Border = 0;
                _PdfPCell12.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell12.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell12.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell12);

                PdfPCell _PdfPCell13 = new PdfPCell(new Phrase(p.Importesaldoinsoluto.ToString(), _fontStyleparaestePagofactura));
                _PdfPCell13.Border = 0;
                _PdfPCell13.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell13.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell13.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell13);

                PdfPCell _PdfPCell14 = new PdfPCell(new Phrase(p.Vendedor, _fontStyleparaestePagofactura));
                _PdfPCell14.Border = 0;
                _PdfPCell14.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell14.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell14.BackgroundColor = BaseColor.WHITE;
                tablaPagofactura.AddCell(_PdfPCell14);

                _documento.Add(tablaPagofactura);

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





