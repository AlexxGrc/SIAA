using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Inventarios;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SIAAPI.Reportes
{
  

    public class GeneradorEtiqprod
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;



        List<WorkinProcess> lotes = new List<WorkinProcess>(); //Objeto que contendra la información del documento pdf
        int Ordendeproduccion { get; set; }



        public string nombreDocumento = string.Empty;
        public GeneradorEtiqprod(System.Drawing.Image logo, List<WorkinProcess> _lotes, int _Ordendeproduccion)
        {
            lotes = _lotes;
            Ordendeproduccion = _Ordendeproduccion;
            //Trabajos con el documento XML
            _documento = new Document(new Rectangle(200, 300).Rotate());
            _documento.SetMargins(0f, 0f, 0f, 0f);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/ETIQUETASProduccion" + _Ordendeproduccion + ".pdf"); ;


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


            //    AgregarLogo(logo);
            //  AgregarCuadro();
            //  AgregarDatosEmisor("");
            AgregarDatosFactura();

            AgregarDatosReceptorEmisor();

            AgregarDatosProductos();


            //     AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"EtiquetasRecepcion+" + Recepcion  +".pdf" + "\"");
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Write(_documento);
            //HttpContext.Current.Response.End();

        }







        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {

            if (logoEmpresa == null)
                return;
            //Agrego la imagen al documento
            //Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
            //imagen.ScaleToFit(140,100);
            //imagen.Alignment = Element.ALIGN_TOP;
            //Chunk logo = new Chunk(imagen, 1, -40);
            //_documento.Add(logo);

            try
            {
                // Image jpg = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/logo.jpg"));
                Image jpg = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(140f, 100F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(30f, 300f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                _documento.Add(jpg);
                //  doc.Add(paragraph);
            }
            catch (Exception err)
            {

            }
        }

        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;


            //Agrego cuadro al documento
            _cb.SetColorStroke(new CMYKColor(0, 29, 50, 70)); //Color de la linea
                                                              //  _cb.SetColorFill(new CMYKColor(0, 29, 50, 70)); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(240, 130, 50, 100);
            //  _cb.FillStroke();
        }

        private void AgregarDatosEmisor(String Telefono)
        {

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

        private void AgregarDatosFactura()
        {
            //Datos de la factura

        }

        private void AgregarDatosReceptorEmisor()
        {


        }


        private void AgregarDatosProductos()
        {










            foreach (WorkinProcess lote in lotes)
            {

                float[] tamanoColumnasProductos = { 300f };
                PdfPTable tablaProductos = new PdfPTable(tamanoColumnasProductos);
                //tablaProductos.SpacingBefore = 1;
                //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
              
                tablaProductos.DefaultCell.BorderWidthLeft = 0.0f;
                tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
                tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
                tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
                tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                tablaProductos.SetTotalWidth(tamanoColumnasProductos);
                tablaProductos.HorizontalAlignment = Element.ALIGN_CENTER;
                tablaProductos.LockedWidth = true;
                tablaProductos.SpacingBefore = 70;


                tablaProductos.AddCell(new Phrase("\nOrden de Produccion NÚM: ", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                tablaProductos.AddCell(new Phrase("\n" + Ordendeproduccion, new Font(Font.FontFamily.HELVETICA, 24)));

                tablaProductos.AddCell(new Phrase("\nFECHA " + DateTime.Now.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));


                OrdenProduccion OP = new OrdenProduccionContext().OrdenesProduccion.Find(Ordendeproduccion);

                tablaProductos.AddCell(new Phrase("\nCliente " , new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

                tablaProductos.AddCell(new Phrase(OP.Cliente.Nombre, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

                


                tablaProductos.AddCell(new Phrase("\n" + lote.loteinterno, new Font(Font.FontFamily.COURIER, 11, Font.BOLD)));

               


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