using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Inventarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace SIAAPI.Reportes
{


    public class CreaEtiquetaMP
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;



        List<Clslotemp> lotes = new List<Clslotemp>(); //Objeto que contendra la información del documento pdf
        int Recepcion { get; set; }



        public string nombreDocumento = string.Empty;
        public CreaEtiquetaMP(System.Drawing.Image logo, List<Clslotemp> _lotes, int _Recepcion)
        {
            lotes = _lotes;
            Recepcion = _Recepcion;
            //Trabajos con el documento XML
            _documento = new Document(new Rectangle(300, 450).Rotate());
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/ETIQUETAS" + Recepcion + ".pdf"); ;


            if (File.Exists(nombreDocumento))
            {
                File.Delete(nombreDocumento);
            }

            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.CreateNew));
            _writer.PageEvent = new ITextEventsEtiqMP(); // invoca la clase que esta mas abajo correspondiente al pie de pagina

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();



        

            AgregarDatosProductos();


            //     AgregarSellos();

            //Cerramoe el documento
            _documento.Close();


        }




      

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Etiqueta MP");
            _documento.AddSubject("Etiqueta MP");
            _documento.AddTitle("Etiqueta MP");
            _documento.SetMargins(5, 5, 5, 5);
        }


        private void AgregarDatosProductos()
        {

            Articulo articulo;
            foreach (Clslotemp lote in lotes)
            {
                articulo = new ArticuloContext().Articulo.Find(lote.IDArticulo);

                float[] tamanoColumnasTitulo = { 420f };
                PdfPTable tablaTitulos = new PdfPTable(tamanoColumnasTitulo);
                  tablaTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                  tablaTitulos.DefaultCell.BorderWidthLeft = 0.0f;
                  tablaTitulos.DefaultCell.BorderWidthRight = 0.0f;
                  tablaTitulos.DefaultCell.BorderWidthBottom = 0.0f;
                  tablaTitulos.DefaultCell.BorderWidthTop = 0.0f;
                  tablaTitulos.SetTotalWidth(tamanoColumnasTitulo);
                  tablaTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
                  tablaTitulos.LockedWidth = true;



                tablaTitulos.AddCell(new Phrase("CLASS LABELS \n", new Font(Font.FontFamily.HELVETICA,8, Font.BOLD)));
                tablaTitulos.AddCell(new Phrase(articulo.Cref+"\n", new Font(Font.FontFamily.HELVETICA, 30, Font.BOLD)));
                tablaTitulos.AddCell(new Phrase(articulo.Descripcion + "\n", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                _documento.Add(tablaTitulos);

                float[] tamanoColumnasProductos = { 120f,150f,120f,50f };
                PdfPTable tablaProductos = new PdfPTable(tamanoColumnasProductos);
                //tablaProductos.SpacingBefore = 1;



                //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductos.DefaultCell.BorderWidthLeft = 0.0f;
                tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
                tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
                tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
                tablaProductos.SetTotalWidth(tamanoColumnasProductos);
                tablaProductos.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductos.LockedWidth = true;
               
                //_documento.NewPage();
                Paragraph p2 = new Paragraph();
                p2.IndentationLeft = 210f;
                //  p2.SpacingAfter = 18;
                p2.Leading = 10;
                p2.Alignment = Element.ALIGN_CENTER;

                p2.Add(new Phrase("Recepcion NÚM: ", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
                p2.Add(new Phrase("\n" + Recepcion, new Font(Font.FontFamily.HELVETICA, 16)));

                p2.Add(new Phrase("\nFECHA " + DateTime.Now.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

                p2.Add(new Phrase("\n"+lote.Largo +" Mts", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));

                PdfPCell celda = new PdfPCell(p2);
                celda.Colspan = 1;
                celda.Border = 1;
                 
                tablaProductos.AddCell(p2);
                //_documento.NewPage();
                Paragraph p3 = new Paragraph();
                p3.IndentationLeft = 210f;
                //  p2.SpacingAfter = 18;
                p3.Leading = 10;
                p3.Alignment = Element.ALIGN_CENTER;

               
                
                p3.Add(new Phrase("\n\nLote " + lote.Lote, new Font(Font.FontFamily.HELVETICA, 10)));
                p3.Add(new Phrase("\n\nlote Interno " + lote.LoteInterno, new Font(Font.FontFamily.HELVETICA, 8)));
                    p3.Add(new Phrase("\n\nTicket " + lote.Facturaprov, new Font(Font.FontFamily.HELVETICA, 8)));

                    PdfPCell celda3 = new PdfPCell(p3);
                celda3.Colspan = 3;
                celda3.Border = 0;

                tablaProductos.AddCell(celda3);
                        
                tablaProductos.CompleteRow();

                Paragraph pESPACIO = new Paragraph();
             

                pESPACIO.Add(new Phrase("\n" , new Font(Font.FontFamily.HELVETICA, 16)));
                tablaProductos.AddCell(pESPACIO);
                tablaProductos.CompleteRow();


                StringBuilder codigoQR = new StringBuilder();
                codigoQR.Append("http://" + SIAAPI.Properties.Settings.Default.Nombredelaaplicacion + "/EncRecepcion/Details/" + Recepcion);


                BarcodeQRCode pdfCodigoQR = new BarcodeQRCode(codigoQR.ToString(), 1, 1, null);
                Image img = pdfCodigoQR.GetImage();
                img.SpacingAfter = 0.0f;
                img.SpacingBefore = 0.0f;
                img.BorderWidth = 1.0f;
                //img.ScalePercent(100, 78);

                tablaProductos.AddCell(img);

                string[] datosloteinterno = lote.LoteInterno.Split('/');
                string nodecinta = "";
                try
                {
                    nodecinta = datosloteinterno[datosloteinterno.Length - 1];
                }
                catch(Exception err)
                {

                }

                PdfPCell celdacinta = new PdfPCell(new Phrase("No de cinta\n"+nodecinta, new Font(Font.FontFamily.TIMES_ROMAN, 24)));
                celdacinta.HorizontalAlignment = Element.ALIGN_CENTER;
                celdacinta.VerticalAlignment = Element.ALIGN_CENTER;
                celdacinta.Border = 0;
                tablaProductos.AddCell(celdacinta);
                


                StringBuilder codigoQR2 = new StringBuilder();
                codigoQR2.Append(lote.LoteInterno);


                BarcodeQRCode pdfCodigoQR2 = new BarcodeQRCode(codigoQR2.ToString(), 1, 1, null);
                Image img2 = pdfCodigoQR2.GetImage();
                img2.SpacingAfter = 0.0f;
                img2.SpacingBefore = 0.0f;
                img2.BorderWidth = 1.0f;

                tablaProductos.AddCell(img2);

                tablaProductos.CompleteRow();

                _documento.Add(tablaProductos);


                _documento.Add(Chunk.NEXTPAGE);
            }

            //img.border









        }






        #region Extensión de la clase pdfPageEvenHelper
        public class ITextEventsEtiqMP : PdfPageEventHelper
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

            public int Remision { get; set; }
            #endregion


            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    PrintTime = DateTime.Now;
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    headerTemplate = cb.CreateTemplate(30, 30);
                    footerTemplate = cb.CreateTemplate(30, 30);
                }
                catch (DocumentException)
                { }
                catch (System.IO.IOException)
                { }
            }

            public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnEndPage(writer, document);

                iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

                iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

                Phrase p1Header = new Phrase("Sample Header Here", baseFontNormal);

                ////Create PdfTable object
                //PdfPTable pdfTab = new PdfPTable(3);

                ////We will have to create separate cells to include image logo and 2 separate strings
                ////Row 1
                //PdfPCell pdfCell1 = new PdfPCell();
                //PdfPCell pdfCell2 = new PdfPCell(p1Header);
                //PdfPCell pdfCell3 = new PdfPCell();
                String text = "";// "Página " + writer.PageNumber + " de ";


                ////Add paging to header
                //{
                //    cb.BeginText();
                //    cb.SetFontAndSize(bf, 12);
                //    cb.SetTextMatrix(document.PageSize.GetRight(10), document.PageSize.GetTop(10));
                //    cb.ShowText(text);
                //    cb.EndText();
                //    float len = bf.GetWidthPoint(text, 12);
                //    //Adds "12" in Page 1 of 12
                //    cb.AddTemplate(headerTemplate, document.PageSize.GetRight(10) + len, document.PageSize.GetTop(10));
                //}

                //    Add paging to footer
                {
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 9);
                    cb.SetTextMatrix(document.PageSize.GetRight(10), document.PageSize.GetBottom(10));
                    //cb.MoveText(500,30);
                    //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
                    cb.ShowText(text);
                    cb.EndText();
                    float len = bf.GetWidthPoint(text, 9);
                    cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

                    float[] anchoColumasTablaTotales = { 280f };
                    PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
                    tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                    tabla.SetTotalWidth(anchoColumasTablaTotales);
                    tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla.LockedWidth = true;
                    //  tabla.AddCell(new Phrase("Recepcion " + Remision, new Font(Font.FontFamily.HELVETICA, 10)));

                    tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(10), writer.DirectContent);

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
                //  pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
                ////set pdfContent value

                ////DIbuja una linea para separar el encabezado
                //cb.MoveTo(20, document.PageSize.Height - 142);
                //cb.LineTo(document.PageSize.Width - 20, document.PageSize.Height - 142);
                //cb.Stroke();

                //Move the pointer and draw line to separate footer section from rest of page
                //cb.MoveTo(40, document.PageSize.GetBottom(50));
                //cb.LineTo(document

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
}