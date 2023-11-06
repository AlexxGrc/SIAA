

using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Reportes
{
    public class ProveedorReporte
    {

        #region Declaration
        private Document _documento;
        Font _fontStyle;
        PdfWriter _writer;
        int _totalColumn = 6;

        PdfPTable _pdfTable = new PdfPTable(6);
        PdfPCell _PdfPCell;
        MemoryStream _memoryStream = new MemoryStream();
        List<Proveedor> _proveedor = new List<Proveedor>();
        #endregion
   
        // aqui los puedes pasar como parametro a l reporte

        public byte[] PrepareReport(List<Proveedor> proveedor)
        {
            _proveedor = proveedor;
            #region
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);
         
            _documento.SetMargins(20f, 20f, 20f, 50f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            _documento.Open();
            
            _pdfTable.SetWidths(new float[] { 100f, 50f, 50f, 50f, 50f, 50f});
            #endregion
            this.ReportHeader();
            this.ReportBody();
            
            _pdfTable.HeaderRows = 4;
            _documento.Add(_pdfTable);
            _documento.Close();

            return _memoryStream.ToArray();
            

        }

 
        private void ReportHeader()
        {
            #region Table head
            DateTime Hoy = DateTime.Today;
            string fecha_actual = Hoy.ToString("dd/MM/yyyy");
            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            _PdfPCell = new PdfPCell(new Phrase(fecha_actual, _fontStyle));
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _PdfPCell.Border = 0;
            _PdfPCell.BackgroundColor = BaseColor.WHITE;
            _PdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_PdfPCell);
            
           
            //string imagepath = "C:\\Users\\VANE-PC\\Desktop\\Classlabel\\SIAAPI\\SIAAPI";
            // HttpContext.Current.Server.MapPath("~/images/logo.png");

            // add a image
            //Image jpg = iTextSharp.text.Image.GetInstance(imagepath + "\\logo.jpg");
            Image jpg = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/logo.jpg"));
            PdfPCell imageCell = new PdfPCell(jpg);
            _PdfPCell=new PdfPCell((imageCell));
            _PdfPCell.Border = 0;
            _pdfTable.AddCell(_PdfPCell);



            _fontStyle = FontFactory.GetFont("Tahoma", 15f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Calificación de Proveedor", _fontStyle));
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.Border = 0;
            _PdfPCell.BackgroundColor = BaseColor.WHITE;
            _PdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_PdfPCell);
           


            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            _PdfPCell = new PdfPCell(new Phrase("   ", _fontStyle)); // aqui te pongo la descripcion del almacen
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.Border = 0;
            _PdfPCell.BackgroundColor = BaseColor.WHITE;
            _PdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_PdfPCell);
          

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Proveedor", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Confianza", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Servicio", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Tiempo Entrega", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Calidad", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);
           
            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Producto", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);
            _pdfTable.CompleteRow();
            #endregion




        }



        private void ReportBody()
        {

            #region Table Body
            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);

            foreach (Proveedor proveedor in _proveedor)
            {
                _PdfPCell = new PdfPCell(new Phrase(proveedor.Empresa, _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_PdfPCell);

                if (proveedor.Confianza.Equals("Confiable"))
                {
                    _PdfPCell = new PdfPCell(new Phrase(proveedor.Confianza, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = BaseColor.YELLOW;
                    _pdfTable.AddCell(_PdfPCell);
                }
                else
                {
                    _PdfPCell = new PdfPCell(new Phrase(proveedor.Confianza, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = BaseColor.PINK;
                    _pdfTable.AddCell(_PdfPCell);
                }

                _PdfPCell = new PdfPCell(new Phrase(proveedor.Servicio.ToString().ToString(), _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_PdfPCell);

                _PdfPCell = new PdfPCell(new Phrase(proveedor.Tentrego.ToString().ToString(), _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_PdfPCell);


                _PdfPCell = new PdfPCell(new Phrase(proveedor.Calidad.ToString(), _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_PdfPCell);

                _PdfPCell = new PdfPCell(new Phrase(proveedor.Producto, _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_PdfPCell);


                _pdfTable.CompleteRow();

                #endregion
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

                //iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

                //iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

                //Phrase p1Header = new Phrase("Sample Header Here", baseFontNormal);

                ////Create PdfTable object
                //PdfPTable pdfTab = new PdfPTable(3);

                ////We will have to create separate cells to include image logo and 2 separate strings
                ////Row 1
                //PdfPCell pdfCell1 = new PdfPCell();
                //PdfPCell pdfCell2 = new PdfPCell(p1Header);
                //PdfPCell pdfCell3 = new PdfPCell();
                String text = "Página " + writer.PageNumber + " de ";


                ////Add paging to header
                //{
                //    cb.BeginText();
                //    cb.SetFontAndSize(bf, 12);
                //    cb.SetTextMatrix(document.PageSize.GetRight(200), document.PageSize.GetTop(45));
                //    cb.ShowText(text);
                //    cb.EndText();
                //    float len = bf.GetWidthPoint(text, 12);
                //    //Adds "12" in Page 1 of 12
                //    cb.AddTemplate(headerTemplate, document.PageSize.GetRight(200) + len, document.PageSize.GetTop(45));
                //}

                //Add paging to footer
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



                ////Row 2
                //PdfPCell pdfCell4 = new PdfPCell(new Phrase("Sub Header Description", baseFontNormal));
                ////Row 3


                //PdfPCell pdfCell5 = new PdfPCell(new Phrase("Date:" + PrintTime.ToShortDateString(), baseFontBig));
                //PdfPCell pdfCell6 = new PdfPCell();
                //PdfPCell pdfCell7 = new PdfPCell(new Phrase("TIME:" + string.Format("{0:t}", DateTime.Now), baseFontBig));


                ////set the alignment of all three cells and set border to 0
                //pdfCell1.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell3.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell4.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell6.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell7.HorizontalAlignment = Element.ALIGN_CENTER;


                //pdfCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
                //pdfCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                //pdfCell4.VerticalAlignment = Element.ALIGN_TOP;
                //pdfCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
                //pdfCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
                //pdfCell7.VerticalAlignment = Element.ALIGN_MIDDLE;


                //pdfCell4.Colspan = 3;



                //pdfCell1.Border = 0;
                //pdfCell2.Border = 0;
                //pdfCell3.Border = 0;
                //pdfCell4.Border = 0;
                //pdfCell5.Border = 0;
                //pdfCell6.Border = 0;
                //pdfCell7.Border = 0;


                ////add all three cells into PdfTable
                //pdfTab.AddCell(pdfCell1);
                //pdfTab.AddCell(pdfCell2);
                //pdfTab.AddCell(pdfCell3);
                //pdfTab.AddCell(pdfCell4);
                //pdfTab.AddCell(pdfCell5);
                //pdfTab.AddCell(pdfCell6);
                //pdfTab.AddCell(pdfCell7);

                //pdfTab.TotalWidth = document.PageSize.Width - 80f;
                //pdfTab.WidthPercentage = 70;
                ////pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;


                ////call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
                ////first param is start row. -1 indicates there is no end row and all the rows to be included to write
                ////Third and fourth param is x and y position to start writing
                //pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
                ////set pdfContent value

                ////Move the pointer and draw line to separate header section from rest of page
                //cb.MoveTo(40, document.PageSize.Height - 100);
                //cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
                //cb.Stroke();

                //Move the pointer and draw line to separate footer section from rest of page
                //cb.MoveTo(40, document.PageSize.GetBottom(50));
                //cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
                //cb.Stroke();
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